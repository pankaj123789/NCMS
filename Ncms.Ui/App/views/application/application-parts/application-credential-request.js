define([
    'plugins/router',
    'modules/enums',
    'services/application-data-service',
],
function (router, enums, applicationService) {
    return {
        getInstance: getInstance
    };

    function getInstance() {
        var vm = {
            credentialRequests: ko.observableArray(),
            countries: ko.observableArray(),
            applicationId: ko.observable(),
            selectAction: function () { },
        };

        vm.credentialRequestOptions = {
            name: 'credential-request',
            params: {
                credentialRequests: vm.credentialRequests,
                showActions: true,
                hideApplicationType: true,
                selectAction: function (credentialRequest, action) {
                    vm.selectAction(ko.toJS(action), credentialRequest);
                }
            }
        };

        var dirtyFlag = new ko.DirtyFlag([vm.credentialRequests], false);

        vm.isEnable = ko.observable(true);

        vm.load = function (applicationId) {
            vm.applicationId(applicationId);
            applicationService.getFluid(vm.applicationId() + '/credentialrequests').then(function (data) {
                var credentialRequests = ko.viewmodel.fromModel(data);
                vm.credentialRequests(credentialRequests());
                dirtyFlag().reset();
            });
        };

        vm.isDirty = ko.pureComputed(function () {
            return dirtyFlag().isDirty();
        });

        vm.resetDirtyFlag = function () {
            dirtyFlag().reset();
        };

        vm.newCredentialRequest = function () {
            vm.selectAction({ Id: enums.ApplicationWizardActions.NewCredentialRequest });
        };
        
        vm.save = function () {
            var defer = Q.defer();
            var promise = defer.promise;

            var models = ko.viewmodel.toModel(vm.credentialRequests);
            applicationService.post({ ApplicationId: vm.applicationId(), CredentialRequests: models }, 'credentialrequest').then(function () {
                defer.resolve('fulfilled');
            });

            return promise;
        };

        return vm;
    }
});
