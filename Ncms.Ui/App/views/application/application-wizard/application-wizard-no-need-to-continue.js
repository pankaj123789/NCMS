define([
    'services/util',
    'modules/enums',
], function (util, enums) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {

        var vm = {
            session: params.preRequisiteSession,
            credentialPrerequisiteRequest: params.credentialPrerequisiteRequest,
            newApplications: ko.observable(true),
            credentialOnHold: ko.observable(false),
            finalStepType: params.finalStepType    
        };

        vm.load = function () {
            switch (vm.finalStepType) {
                case 'NoSelectedNewApplications':
                    if (vm.session.selectedApplications.length == 0) {
                        vm.newApplications(false);
                    }

                    vm.credentialPrerequisiteRequest.CreateApplications = false;
                    break;

                case 'CredentialWillBePutOnHold':
                    vm.credentialOnHold(true)
                    break;

                default:
                    break;
            } 
        }

        vm.postData = function () {
            switch (vm.finalStepType) {
                case 'NoSelectedNewApplications':
                    return vm.credentialPrerequisiteRequest;
                    break;

                case 'CredentialWillBePutOnHold':
                    return;
                    break;

                default:
                    return;
            } 
        }

        return vm;
    }
});