define(['services/requester'],
	function (requester) {
		return {
			getInstance: getInstance
		};

		function getInstance(options) {
			var applications = requester('applications');

			var vm = {
				testAttendanceId: options.testAttendanceId,
				testTasks: ko.observableArray([]),
				serviceName: options.serviceName,
				canContinue: ko.observable(false),
			};

			vm.load = function () {
				vm.canContinue(false);
				vm.testTasks([]);
				applications.getFluid(vm.serviceName + '/tasks', { TestAttendanceId: vm.testAttendanceId() }).then(function (data) {
					vm.testTasks(data);
					vm.canContinue(true);
				});
			};

			return vm;
		}
	});