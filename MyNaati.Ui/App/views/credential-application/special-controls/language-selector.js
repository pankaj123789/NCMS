define([
	'views/wizard/list-question',
	'views/credential-application/special-controls/credential-inprogress',
	'views/credential-application/special-controls/credential-future'
	],
	function (listQuestion, credentialInProgress, credentialFuture) {
		return {
			getInstance: getInstance
		};

		function getInstance(options) {
			var vm = {
				applicationId: options.applicationId,
				naatiNumber: options.naatiNumber,
				token: options.token,
				question: options.question,
				credentials: ko.observableArray(),
				clearNextQuestions: options.clearNextQuestions,
				applicationFormId: options.applicationFormId,
				credentialApplication: options.credentialApplication
			};

			vm.inProgressCredentialsOptions = {
				view: 'views/credential-application/special-controls/credential-inprogress',
				model: credentialInProgress.getInstance(vm)
			};

			vm.futureCredentialsOptions = {
				view: 'views/credential-application/special-controls/credential-future',
				model: credentialFuture.getInstance(vm)
			};

			vm.languageOptions = {
				view: 'views/wizard/list-question',
				model: listQuestion.getInstance(options)
			};

			vm.validation = vm.languageOptions.model.validation;

			vm.load = function () {
				vm.inProgressCredentialsOptions.model.load();
				vm.futureCredentialsOptions.model.load();
			};

			return vm;
		}
	});