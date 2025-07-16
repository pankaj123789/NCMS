define([
    'services/application-data-service',
], function (applicationService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            action: null
        };

        $.extend(defaultParams, params);

        var serverModel = {

        };

        var vm = {
            action: defaultParams.action,
            label: ko.observable()
        };


        vm.load = function () {
            var request = {
                action: defaultParams.action().Id
            };

            applicationService.getFluid('stepMessage', request).then(function (data) {
                vm.label(data);
            });
        };

        vm.postData = function () {
            return ko.toJS(serverModel);
        };

        vm.isValid = function () {
            return true;
        };

        vm.activate = function () {

        }

        return vm;
    }
});