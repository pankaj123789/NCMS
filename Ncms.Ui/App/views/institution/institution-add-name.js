define([
        'services/screen/date-service',
        'services/screen/message-service',
        'services/institution-data-service',
        'plugins/router'
],
    function(dateService, message, institutionService, router) {

        var serverModel = {
            NaatiNumber: ko.observable(),
            InstitutionId: ko.observable(),
            EntityId: ko.observable(),
            Name: ko.observable().extend({ required: true,maxLength: 100 }),
            AbbreviatedName: ko.observable().extend({ maxLength: 10 })
        };

        var emptyServerModel = ko.toJS(serverModel);

        var vm = {
            canActivate: canActivate,
            institution: serverModel,
            save: save,
            tryClose: tryClose
        };

        vm.dirtyFlag = new ko.DirtyFlag(vm.institution, false);

        var validation = ko.validatedObservable(vm.institution);

        $.extend(vm,
        {
            windowTitle: ko.pureComputed(
                function () {
                    return 'Organisation Add Name: ' + vm.institution.NaatiNumber();
                }),
            canSave: ko.pureComputed(
                function () {
                    return vm.dirtyFlag().isDirty();
                })
        });

        function canActivate(naatiNumber) {
            ko.viewmodel.updateFromModel(vm.institution, emptyServerModel);
            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }

            return loadinstitution(naatiNumber);
        }

        function loadinstitution(naatiNumber) {
            return institutionService.getFluid(naatiNumber)
                .then(
                    function (data) {
                        if (data) {
                            vm.institution.NaatiNumber(data.NaatiNumber);
                            vm.institution.EntityId(data.EntityId);
                            vm.institution.InstitutionId(data.InstitutionId);
                            vm.dirtyFlag().reset();
                            return true;
                        }
                        return false;
                    },
                    function () {
                        return false;
                    });
        }

        function save() {
            if (!validation.isValid()) {
                return validation.errors.showAllMessages();
            }

            var json = ko.toJS(vm.institution);
            json.AbbreviatedName = json.AbbreviatedName || '';
            institutionService.post(json, vm.institution.NaatiNumber() + '/addname')
                .then(
                    function () {
                        toastr.success('Name added successfully');
                        close();
                    });
        }

        function tryClose() {
            if (vm.dirtyFlag().isDirty()) {
                message.confirm({
                    title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                    content: ko.Localization('Naati.Resources.Shared.resources.AreYouSureYouWantToCancel')
                })
                    .then(
                        function (answer) {
                            if (answer === 'yes') {
                                close();
                            }
                        });
            } else {
                close();
            }
        }

        function close() {
            router.navigateBack();
        }

        return vm;
    });
