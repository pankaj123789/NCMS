define([
        'services/credentialrequest-data-service'
],
    function (credentialrequestService) {
        return {
            getInstance: getInstance
        };

        function getInstance(params) {
            var defaultParams = {
                summary: null,
                selectedTestSession: null
            };

            $.extend(defaultParams, params);

            var vm = {
                summary: defaultParams.summary,
                selectedTestSession: defaultParams.selectedTestSession,
                selectedIds: ko.observableArray(),
                applicants: ko.observableArray(),
                existingApplicants: ko.observableArray()
            };
            
            vm.wizardNewApplicantsOptions = {
                selectedIds: vm.selectedIds,
                applicants: vm.applicants,
                existingApplicants: vm.existingApplicants,
                selectedTestSession: vm.selectedTestSession,
                allowSelfAssign: vm.selectedTestSession.AllowSelfAssign(),
                venueCapacity: ko.pureComputed(function () {
                    return vm.summary.Request().VenueCapacity;
                }),
                isValidPromise: function () {
                    return credentialrequestService.post(vm.summary.Request(), 'newapplicants');
                }
            };

            vm.load = function () {
                credentialrequestService.getFluid('newapplicants', vm.summary.Request()).then(vm.applicants);
                credentialrequestService.getFluid('existingapplicants', vm.summary.Request()).then(vm.existingApplicants);
            };

            vm.isValid = function () {
                return vm.wizardNewApplicantsOptions.component.isValid();
            };

            return vm;
        }
    });