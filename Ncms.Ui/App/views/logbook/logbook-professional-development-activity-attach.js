define([
	'views/document/document-form',
	'services/person-data-service',
	'services/logbook-data-service'],
	function (documentTable, personDataService, logbookService) {
		return {
			getInstance: getInstance
		};

		function getInstance() {
			var vm = {
				activities: ko.observableArray(),
				naatiNumber: ko.observable(),
			};

			var defer = null;

			vm.attach = function (naatiNumber, currentPeriodId) {
				defer = Q.defer();

				personDataService.getFluid(naatiNumber + '/activities/' + currentPeriodId).then(vm.activities);
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