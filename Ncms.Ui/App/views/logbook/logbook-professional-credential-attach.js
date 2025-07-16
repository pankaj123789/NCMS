define([
	'views/document/document-form',
	'services/logbook-data-service'],
	function (documentTable,  logbookService) {
		return {
			getInstance: getInstance
		};

		function getInstance() {
			var vm = {
				activities: ko.observableArray(),
				naatiNumber: ko.observable(),
			};

			var defer = null;

            vm.attach = function (credentialId, naatiNumber, certificationPeriodId) {
				defer = Q.defer();

                logbookService.getFluid(
                    "WorkPracticeActivities",
                    {
                        credentialId: credentialId,
                        naatiNumber: naatiNumber,
                        certificationPeriodId: certificationPeriodId
                    }).then(vm.activities);

				$('#attachActivityModal').modal('show');

				return defer.promise;
			};

			vm.select = function (activity) {
                defer.resolve(activity.Id);
				$('#attachActivityModal').modal('hide');
			};

			return vm;
		}
	}
);