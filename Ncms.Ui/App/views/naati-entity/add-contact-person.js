define([
        'services/screen/date-service',
        'services/institution-data-service',
        'plugins/router',
        'modules/shared-filters',
        'modules/common',
        'services/util',
        'services/screen/message-service'
],
    function(dateService, institutionService, router, sharedFilters, common, util, message) {


        var serverModel = {
            Name: ko.observable().extend({ required: true, maxLength: 500 }),
            Email: ko.observable().extend({ required: true, email: true, maxLength: 500 }),
            Phone: ko.observable().extend({ maxLength: 500 }),
            PostalAddress: ko.observable(),
            Description: ko.observable(),
            InstitutionId: ko.observable(),
            ContactPersonId: ko.observable()
        };

        var cleanModel = ko.toJS(serverModel);

        var vm = {
            person: serverModel,
            save: save,
            tryClose: tryClose,
            windowTitle: ko.observable('Add Contact Person'),
            isCreated: ko.observable(true)
        };

        vm.dirtyFlag = new ko.DirtyFlag(vm.person, false);
        var validation = ko.validatedObservable(vm.person);

        vm.activate = function(institutionId, contactPersonId) {
            ko.viewmodel.updateFromModel(serverModel, cleanModel);
            vm.person.InstitutionId(institutionId);
            vm.dirtyFlag().reset();
            validation.errors.showAllMessages(false);

            if (contactPersonId) {
                vm.windowTitle('Update Contact Person');
                vm.isCreated(false);
                institutionService.getFluid('contactPersonById/' + contactPersonId).then(function(data) {
                    vm.person.Name(data.Name);
                    vm.person.Email(data.Email);
                    vm.person.Phone(data.Phone);
                    vm.person.PostalAddress(data.PostalAddress);
                    vm.person.Description(data.Description);
                    vm.person.ContactPersonId(contactPersonId);
                });
            } else {
                vm.isCreated(true);
                vm.windowTitle('Add Contact Person');
            }
        };

       

        function save() {
            var isValid = validation.isValid();
            validation.errors.showAllMessages(!isValid);

            if (isValid) {
                var json = ko.toJS(vm.person);

                if (vm.isCreated()) {
                    institutionService.post(json, 'add')
                        .then(
                            function() {
                                toastr.success('A new contact person has been added.');
                                close();
                            });
                } else {
                    institutionService.post(json, 'update')
                                     .then(
                                         function() {
                                             toastr.success('The contact person has been saved.');
                                             close();

                                         });

                }
            }
        }

        function tryClose() {
            if (vm.dirtyFlag().isDirty()) {
                message.confirm({
                    title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                    content: ko.Localization('Naati.Resources.Shared.resources.AreYouSureYouWantToCancel')
                })
                    .then(
                        function(answer) {
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
