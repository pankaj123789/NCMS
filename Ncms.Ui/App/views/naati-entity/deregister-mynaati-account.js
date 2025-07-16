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
                userName: ko.observable()
            };
            
            vm.show = function (userName, personName) {

                vm.defer = Q.defer();
                var confirmationMessage = ko.Localization('Naati.Resources.Person.resources.DeregisterConfirmation')
                    .format(personName);
                vm.message(confirmationMessage);
                vm.userName(userName);
                $('#deregisterMyNaatiModal').modal('show');
                return vm.defer.promise;
            };

            vm.close = function() {
                $('#deregisterMyNaatiModal').modal('hide');
            }

            vm.deregister = function() {
                personService.post({ UserName: vm.userName() }, 'deleteMyNaatiUser').then(function(data) {
                    return vm.defer.resolve(data);
                });
            }

            vm.activate = function () {}

            return vm;
        }
    });
