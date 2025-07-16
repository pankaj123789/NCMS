define([
    'modules/custom-validator',
    'services/screen/date-service',
    'services/testsession-data-service'
], function (customValidator, dateService, testSessionService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            application: null,
            action: null,
            credentialRequest: null,
            stepId: null,
            wizardService: null,
            session: null
    };

        $.extend(defaultParams, params);

        var serverModel = {
            NotifyApplicantsChecked: ko.observable(),
            NotifyRolePlayersChecked: ko.observable(),
            NotifyApplicantsMessage: ko.observable(),
            NotifyRolePlayersMessage: ko.observable(),
            OnDisableApplicantsMessage: ko.observable(),
            OnDisableRolePlayersMessage: ko.observable(),
            ReadOnly: ko.observable(),
            IsRolePlayerAvailable: ko.observable()
        };

        var vm = {
            option: serverModel,
            optionPromise: null
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
            var sessionDetails = ko.toJS(defaultParams.session);
           
            sessionDetails.TestDate = sessionDetails.TestDate ? dateService.toPostDate(sessionDetails.TestDate) : null;
            defaultParams.wizardService.post(sessionDetails,'getCheckOptionMessage').then(function (data) {
                ko.viewmodel.updateFromModel(serverModel, data);
            });          
        }

        return vm;
    }
});