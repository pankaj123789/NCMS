define([
     'services/person-data-service'
    ],
    function (personService) {
        return {
            getInstance: getInstance
        };

        function getInstance() {
            var vm = {
                message: ko.observable(),
                naatiNumber: ko.observable()
            };
            
            vm.show = function (naatiNumber) {

                vm.defer = Q.defer();
                var confirmationMessage = ko.Localization('Naati.Resources.Person.resources.DeregisterMfaConfirmation');
                vm.message(confirmationMessage);
                vm.naatiNumber(naatiNumber);
                $('#deregisterMfaModal').modal('show');
                return vm.defer.promise;
            };

            vm.close = function() {
                $('#deregisterMfaModal').modal('hide');
            }

            vm.deregister = function() {
                personService.post({ NaatiNumber: vm.naatiNumber() }, 'deleteMfaAccount').then(function(data) {
                    return vm.defer.resolve(data);
                });
            }

            vm.activate = function () {}

            return vm;
        }
    });
