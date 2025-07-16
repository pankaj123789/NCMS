define([
    'services/system-data-service'
], function (systemService) {
    return {
        getInstance: getInstance
    };

    function getInstance() {

        var vm = {
            showfiledeletereport: showfiledeletereport,
            reportOutput: ko.observable(),
            showReport: ko.observable(false)
        };

        vm.reportOutput.subscribe(function () { vm.showReport(true) });

        function showfiledeletereport() {
            systemService.getFluid('getfiledeletereport').then(function (data) {
                vm.reportOutput(data);
            });
        }

        return vm;
    }
});