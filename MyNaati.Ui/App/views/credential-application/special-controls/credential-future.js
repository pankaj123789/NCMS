define([],
	function () {
		return {
			getInstance: getInstance
		};

		function getInstance(options) {
			var credentialApplication = options.credentialApplication;

			var vm = {
				applicationId: options.applicationId,
				naatiNumber: options.naatiNumber,
				token: options.token,
				question: options.question,
				credentials: ko.observableArray(),
				clearNextQuestions: options.clearNextQuestions,
				applicationFormId: options.applicationFormId
			};

			vm.load = function () {
				credentialApplication.activeAndFutureCredentials({ NAATINumber: vm.naatiNumber(), Token: vm.token() }).then(vm.credentials);
			}

			return vm;
		}
	});