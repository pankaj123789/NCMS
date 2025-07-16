define([
    'plugins/router',
    'modules/common',
    'modules/enums',
    'services/institution-data-service',
    'views/naati-entity/add-contact-person',
    'services/screen/message-service'
],
function(router, common, enums, institutionService, contactPerson, message) {
    return {
        getInstance: getInstance
    };

    function getInstance() {
        var vm = {
            entityId: ko.observable(),
            institutionId: ko.observable(),
            contactPersons: ko.observableArray(),
            tableDefinition: {
                id: 'contactPersonTable',
                headerTemplate: 'contact-person-header-template',
                rowTemplate: 'contact-person-row-template'
            },
            deleteContactPerson: deleteContactPerson
        };

        function deleteContactPerson(data) {

            message.confirm({
                title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                content: ko.Localization('Naati.Resources.Shared.resources.AreYouSure')
            })
                     .then(
                         function(answer) {
                             if (answer === 'yes') {
                                 institutionService.post({ contactPersonId: data.ContactPersonId }, 'inactive').then(function(data) {
                                     if (data === true) {
                                         toastr.success('The person has been removed.');
                                         institutionService.getFluid('contactPerson/' + vm.institutionId()).then(function(data) {
                                             vm.contactPersons(data);
                                         });
                                     }
                                 });
                             }
                         });

          
        }

        vm.newContactPerson = function() {
            router.navigate('add-contact-person/' + vm.institutionId());
        }

        vm.editContactPerson = function(data) {
            router.navigate('add-contact-person/' + vm.institutionId() + "/" + data.ContactPersonId);
        }

        vm.tableDefinition.dataTable = {
            source: vm.contactPersons,
            columnDefs: [
                {
                    targets: -1,
                    orderable: false
                }
            ],
            order: [
                [0, "desc"]
            ]
        };

        vm.load = function(institutionId, entityId) {
            vm.institutionId(institutionId);
            vm.entityId(entityId);
            institutionService.getFluid('contactPerson/'+vm.institutionId()).then(function(data) {
                vm.contactPersons(data);
            });
        };

        return vm;
    }
});
