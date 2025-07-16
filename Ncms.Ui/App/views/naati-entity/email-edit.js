define([
        'services/screen/date-service',
        'services/person-data-service',
        'plugins/router',
        'modules/shared-filters',
        'modules/common',
        'services/util',
        'services/screen/message-service',
        'modules/enums',
        'services/institution-data-service',
],
    function (dateService, personDataService, router, sharedFilters, common, util, message, enums, institutionService) {

        var serverModel = {
            EmailId: ko.observable(),
            EntityId: ko.observable(),
            Email: ko.observable().extend({
                required: true,
                pattern: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
            }),
            IncludeInPd: ko.observable(),
            IsPreferredEmail: ko.observable(),
            Note: ko.observable(),
            PrimaryContact: ko.observable(),
            ExaminerCorrespondence: ko.observable(),
            ShowExaminer: ko.observable()
        };

        var vm = {
            canActivate: canActivate,
            activate: activate,
            email: serverModel,
            emails: ko.observableArray(),
            validateInGoogle: ko.observable(true),
            save: save,
            tryClose: tryClose,
            windowTitle: ko.observable('Edit Email'),
            suburb: ko.observable(),
            editMode: ko.observable(),
            originalPreferredEmail: ko.observable(false),
            isMyNaatiRegistered: ko.observable(),
            IsExaminer: ko.observable(),
            IsOrganisation: ko.observable(),
            canUpdateMyNaatiRegistration: ko.observable(false)
        };

        vm.setIsOrganisationFlag = function (flag) {
            vm.IsOrganisation(flag);
        };

        serverModel.EntityId.subscribe(function (newValue) {
            if (!newValue) return vm.emails([]);

            (vm.IsOrganisation() ? institutionService : personDataService).getFluid(newValue + '/contactdetails')
                .then(function (data) {
                    if (!serverModel.IsPreferredEmail()) {
                        serverModel.IsPreferredEmail(data.Emails.length === 0);
                        vm.primaryDirtyFlag().reset();
                    }
                    vm.isMyNaatiRegistered(data.IsMyNaatiRegistered);
                    vm.emails(data.Emails);
                });
        });

        vm.dirtyFlag = new ko.DirtyFlag([vm.email], false);
        vm.emailDirtyFlag = new ko.DirtyFlag([vm.email.Email], false);
        vm.primaryDirtyFlag = new ko.DirtyFlag([vm.email.IsPreferredEmail], false);

        vm.showMyNaatiWarningForEmail = ko.computed(function() {
            return vm.isMyNaatiRegistered() && vm.originalPreferredEmail() && vm.emailDirtyFlag().isDirty();
        });

        vm.showMyNaatiWarningForPrimary = ko.computed(function () {
            return vm.isMyNaatiRegistered() && vm.primaryDirtyFlag().isDirty();
        });

        vm.canSave = ko.computed(function () {
            return vm.dirtyFlag().isDirty();
        });

        vm.disableAdminFunctions = ko.computed(function () {
            return vm.isMyNaatiRegistered() && !vm.canUpdateMyNaatiRegistration();
        });

        vm.disableAddressEdit = ko.computed(function () {
            return vm.originalPreferredEmail() && vm.disableAdminFunctions();
        });

        vm.primaryFlagLockReason = ko.computed(function() {
            if (vm.isMyNaatiRegistered() && vm.originalPreferredEmail()) {
                return ko.Localization('Naati.Resources.Person.resources.PrimaryEmailChangeHint1');
            }
            if (vm.disableAdminFunctions()) {
                return ko.Localization('Naati.Resources.Person.resources.PrimaryEmailChangeHint2');
            }
            if (vm.originalPreferredEmail()) {
                return ko.Localization('Naati.Resources.Person.resources.PrimaryEmailChangeHint3');
            }
        });

        var validation = ko.validatedObservable(vm.email);


        function canActivate(entityId, emailId) {
            if (emailId !== undefined) {
                return loadEmail(entityId, emailId);
            } else {
                checkExaminerRole(entityId);
                vm.editMode(false);
                return true;
            }
        }

        function checkExaminerRole(entityId) {
            return (vm.IsOrganisation() ? institutionService : personDataService).getFluid('checkExaminerRole/' + entityId)
                .then(
                    function(data) {
                        vm.email.ShowExaminer(data);
                    });
        }

        function activate(entityId, emailId) {

            currentUser.hasPermission(enums.SecNoun.PersonMyNaatiRegistration, enums.SecVerb.Update).then(vm.canUpdateMyNaatiRegistration);

            if (vm.editMode()) {
                edit();
            } else {
                create(entityId);
            }
        }


        function clear() {
            util.resetModel(serverModel);
        }

        function create(entityId) {
            clear();
            serverModel.EmailId(null);
            serverModel.EntityId(entityId);
            vm.originalPreferredEmail(false);
            vm.dirtyFlag().reset();
            vm.emailDirtyFlag().reset();
            vm.primaryDirtyFlag().reset();
            validation.errors.showAllMessages(false);
            return true;
        }

        function loadEmail(entityId, emailId) {
            return (vm.IsOrganisation() ? institutionService : personDataService).getFluid(entityId + '/email/' + emailId)
                .then(
                    function (data) {
                        clear();
                        ko.viewmodel.updateFromModel(vm.email, data);
                        vm.editMode(true);
                        vm.originalPreferredEmail(data.IsPreferredEmail);
                        return true;
                    },
                    function () {
                        return false;
                    });
        }

        function edit() {
            vm.dirtyFlag().reset();
            vm.emailDirtyFlag().reset();
            vm.primaryDirtyFlag().reset();
            validation.errors.showAllMessages(false);
        }

        function save() {
            var isValid = validation.isValid();
            validation.errors.showAllMessages(!isValid);

            if (isValid) {
                var json = ko.toJS(vm.email);
                (vm.IsOrganisation() ? institutionService : personDataService).post(json, 'email')
                    .then(
                        function () {
                            toastr.success('Email address saved');
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
