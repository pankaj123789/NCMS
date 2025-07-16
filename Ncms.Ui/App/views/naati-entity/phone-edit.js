define([
    'services/screen/date-service',
    'services/person-data-service',
    'plugins/router',
    'modules/shared-filters',
    'modules/common',
    'services/util',
    'services/screen/message-service',
    'services/institution-data-service'
],
    function (dateService, personDataService, router, sharedFilters, common, util, message, institutionService) {
        var phonesType = ko.observableArray();

        var serverModel = {
            PhoneId: ko.observable(),
            EntityId: ko.observable(),
            LocalNumber: ko.observable().extend({
                required: true,
                pattern: /^\+?\d+$/
            }),
            IncludeInPd: ko.observable(false),
            PrimaryContact: ko.observable(false),
            AllowSmsNotification: ko.observable(),
            Note: ko.observable(),
            ExaminerCorrespondence: ko.observable(),
            IsExaminer: ko.observable()
        }

        var vm = {
            canActivate: canActivate,
            activate: activate,
            phone: serverModel,
            phones: ko.observableArray(),
            validateInGoogle: ko.observable(true),
            save: save,
            tryClose: tryClose,
            windowTitle: ko.observable('Edit Phone'),
            suburb: ko.observable(),
            editMode: ko.observable(),
            PrimaryContact: ko.observable(),
            IncludeInPd: ko.observable(),
            dirtyFlag: new ko.DirtyFlag([serverModel], false),
            IsOrganisation: ko.observable()
        };


        vm.setIsOrganisationFlag = function (flag) {
            vm.IsOrganisation(flag);
        };

        serverModel.EntityId.subscribe(function (newValue) {
            if (!newValue) return vm.phones([]);

            (vm.IsOrganisation() ? institutionService : personDataService).getFluid(newValue + '/contactdetails')
                .then(function (data) {
                    vm.phones(data.Phones);
                });
        });

        var validation = ko.validatedObservable(vm.phone);

        function canActivate(entityId, phoneId) {
            if (phoneId !== undefined) {
                return loadPhone(entityId, phoneId);
            } else {
                checkExaminerRole(entityId);
                vm.editMode(false);
                return true;
            }
        }

        function checkExaminerRole(entityId) {
            return (vm.IsOrganisation() ? institutionService : personDataService).getFluid('checkExaminerRole/' + entityId)
                .then(
                    function (data) {
                        vm.phone.IsExaminer(data);
                    });
        }

        function activate(entityId, phoneId) {
            if (vm.editMode()) {
                edit();
            } else {
                create(entityId);
            }
        }

        function clear() {
            util.resetModel(vm.phone);
        }

        function create(entityId) {
            return (vm.IsOrganisation() ? institutionService : personDataService).getFluid(entityId + '/primaryPhone').then(function (data) {

                clear();
                vm.phone.PhoneId(null);
                vm.phone.EntityId(entityId);
                vm.dirtyFlag().reset();
                validation.errors.showAllMessages(false);

                if (data == null)
                    vm.phone.PrimaryContact(true);
                return true;
            });
        }

        function loadPhone(entityId, phoneId) {
            return (vm.IsOrganisation() ? institutionService : personDataService).getFluid(entityId + '/phone/' + phoneId)
                .then(
                    function (data) {
                        clear();
                        data.LocalNumber = $.trim(data.LocalNumber);
                        data.LocalNumber = (data.LocalNumber[0] === '+' ? '+' : '') + data.LocalNumber.replace(/([^0-9])/g, '');
                        ko.viewmodel.updateFromModel(vm.phone, data);
                        vm.editMode(true);
                        return true;
                    },
                    function () {
                        return false;
                    });
        }

        function edit() {
            vm.dirtyFlag().reset();
            validation.errors.showAllMessages(false);
        }

        function save() {
            var isValid = validation.isValid();
            validation.errors.showAllMessages(!isValid);

            if (isValid) {
                var json = ko.toJS(vm.phone);
                (vm.IsOrganisation() ? institutionService : personDataService).post(json, 'phone')
                    .then(
                        function () {
                            toastr.success('Phone number saved');
                            close();
                        });
            }
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
        };

        return vm;
    });
