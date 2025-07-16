define([
], function () {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {

        };

        $.extend(defaultParams, params);

        var serverModel = {

        };

        var vm = {
            //TODO should this be dynamic, retrieve from service (like in application-wizard-message) 
            label: ko.observable(ko.Localization('Naati.Resources.Application.resources.CredentialRequestWizardMessage'))
        };


        vm.load = function () {
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