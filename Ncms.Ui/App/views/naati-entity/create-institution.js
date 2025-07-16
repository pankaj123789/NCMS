define([
    'plugins/router',
    'services/institution-data-service',
    'services/screen/message-service'
], function (router, institutionService, messageService) {
    return {
        getInstance: getInstance
    };

    function getInstance() {
        
        var serverModel = {
            Name: ko.observable().extend({ required: true }),
            NaatiNumber: ko.observable().extend({
                digit: true,
                // UC-BackOffice-3035 VR1
                min: { params: 900000, message: ko.Localization('Naati.Resources.Institution.resources.NaatiNumberValidationMessage') },
                max: { params: 949999, message: ko.Localization('Naati.Resources.Institution.resources.NaatiNumberValidationMessage') }
            })
        };

        var cleanModel = ko.toJS(serverModel);

        var vm = {
            modalId: 'createInstitutionModal',
            institution: serverModel
        };

        var validation = ko.validatedObservable(vm.institution);
        vm.dirtyFlag = new ko.DirtyFlag(vm.institution, false);
        vm.canSave = ko.computed(function () {
             return vm.dirtyFlag().isDirty();
        });
    
        vm.create = function() {
            if (!validation.isValid()) {
                return validation.errors.showAllMessages();
            }

            institutionService.post(ko.toJS(vm.institution), 'checkDuplicatedInstitution').then(function(data) {
                if (!data.IsSuccessful) {
                    if (data.InstitutionId > 0) {
                        //when it is unsuccessful get its contact person 
                        institutionService.getFluid('contactPerson/' + data.InstitutionId).then(function(contacts) {
                            var contactPersons = [];
                            $.each(contacts,
                                function(idx, val) {
                                    contactPersons.push(val.Name);
                                });
                            contactPersons = contactPersons.join(", ");

                            var text;
                            //show error msg
                            if (!data.IsWarned) {
                                text = ko.Localization('Naati.Resources.Institution.resources.ErrorInsertInstitution');
                                messageService.alert({
                                    title: 'Error',
                                    content: text.format(data.Name, data.NaatiNumber, contactPersons)
                                });
                            } else {
                                // show warning message
                                text = ko.Localization('Naati.Resources.Institution.resources.WarnedInsertInstitution');
                                messageService.confirm({
                                    title: 'Error',
                                    content: text.format(data.Name, data.NaatiNumber, contactPersons)
                                }).then(
                                    function(answer) {
                                        if (answer === 'yes') {
                                            createInstitution();
                                        }
                                    });
                            }

                        });
                    } else {
                        var text = ko
                                  .Localization('Naati.Resources.Institution.resources.ErrorInsertInstitutionWithNaatiNoPerson');
                        messageService.alert({
                            title: 'Error',
                            content: text.format(data.Name, data.NaatiNumber)
                        });
                    }
                } else {
                    createInstitution();
                }
            });
        };

        function createInstitution() {
            institutionService.post(ko.toJS(vm.institution), 'createInstitution')
                       .then(function(data) {
                           toastr.success('A new institution has been created.');
                           $('#' + vm.modalId).modal('hide');
                           router.navigate('organisation/' + data.NaatiNumber);
                       });
        }

        vm.show = function () {
            ko.viewmodel.updateFromModel(serverModel, cleanModel);
            $('#' + vm.modalId).modal('show');
        };

        return vm;
    }
});