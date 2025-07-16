define([
    'services/util',
    'services/screen/message-service',
    'services/entity-data-service',
    'services/person-data-service',
    'plugins/router'
],
    function (util, message, entityService, personDataService, router) {

        var serverModel = {
            NaatiNumber: ko.observable(),
            PractitionerNumber: ko.observable(),
            PersonId: ko.observable(),
            EntityId: ko.observable(),
            TitleId: ko.observable(),
            GivenName: ko.observable().extend({ required: true }),
            AlternativeGivenName: ko.observable(),
            OtherNames: ko.observable(),
            Surname: ko.observable().extend({ required: true }),
            AlternativeSurname: ko.observable()
        };

        var emptyServerModel = ko.toJS(serverModel);

        var vm = {
            canActivate: canActivate,
            person: serverModel,
            save: save,
            tryClose: tryClose
        };

        vm.dirtyFlag = new ko.DirtyFlag(vm.person, false);

        var validation = ko.validatedObservable(vm.person);

        $.extend(vm,
            {
                windowTitle: ko.pureComputed(
                    function () {
                        return util.getPersonSubScreenTitle(serverModel.NaatiNumber(),
                            serverModel.GivenName(), serverModel.Surname(),
                            serverModel.PractitionerNumber(),
                            'Add Name');
                    }),
                canSave: ko.pureComputed(
                    function () {
                        return vm.dirtyFlag().isDirty();
                    }),
                titleOptions: {
                    value: vm.person.TitleId,
                    multiple: false,
                    initialise: true,
                    multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null },
                    options: ko.observableArray()
                },
                preventNumber: function (model, e) {
                    var keyCode = (e.keyCode ? e.keyCode : e.which);
                    if (keyCode > 47 && keyCode < 58) {
                        e.preventDefault();
                        return false;
                    }
                    return true;
                }
            });

        function canActivate(naatiNumber) {
            ko.viewmodel.updateFromModel(vm.person, emptyServerModel);
            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }

            entityService.getFluid('title').then(function (data) {
                var mapped = $.map(data, function (e) {
                    return { value: e.TitleId, text: e.Title };
                });
                vm.titleOptions.options(mapped);
            });

            return loadPerson(naatiNumber);
        }

        function loadPerson(naatiNumber) {
            return personDataService.getFluid(naatiNumber)
                .then(
                    function (data) {
                        if (data) {
                            ko.viewmodel.updateFromModel(serverModel, data);
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

            var json = ko.toJS(vm.person);
            json.GivenName = (json.GivenName || "").trim();
            json.OtherNames = (json.OtherNames || "").trim();
            json.Surname = (json.Surname || "").trim();

            personDataService.post(json, vm.person.NaatiNumber() + '/addname')
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
