define([
    'services/testresult-data-service',
    'modules/enums',
    'services/util'
], function (testresultService, enums, util) {
    return {
        getInstance: getInstance
    };

    function getInstance() {

        var vm = {
            modalId: util.guid(),
            messages: ko.observableArray([]),
            testResult: ko.observable(),
            isAdmin: ko.observable(false)
        };

       

        var defer;

        vm.show = function (testResult, messages) {
            defer = Q.defer();

            currentUser.hasPermission(enums.SecNoun.TestResult, enums.SecVerb.Override).then(function (data) {
                vm.isAdmin(data);
                vm.messages(messages);
                vm.testResult(testResult);
                $('#' + vm.modalId).modal('show');
            });
          
            return defer.promise;
        };
        
        vm.close = function () {
            $('#' + vm.modalId).modal('hide');
            defer.resolve();
        };

        vm.save = function () {

            testresultService.post(vm.testResult(), 'update').then(function (data) {
                vm.close();
            });
        };

        return vm;
    }
});