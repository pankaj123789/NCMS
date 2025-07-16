define([
	'views/credential-application/special-controls/credential-inprogress',
	'views/credential-application/special-controls/credential-future'
	],
	function (credentialInProgress, credentialFuture) {
		var credentialRequestLimit = ko.observable();

		return {
			getInstance: getInstance
		};

		function getInstance(options) {
			var vm = {
				applicationId: options.applicationId,
				naatiNumber: options.naatiNumber,
				token: options.token,
				question: options.question,
				categories: ko.observableArray(),
				levels: ko.observableArray(),
				skills: ko.observableArray(),
				duplicatedCredentialRequestMessage: ko.observable(),
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

			vm.credentialRequests = options.credentialRequests.extend({
				validation: [
					{
						validator: function (val) {
							return val.length;
						},
						message: 'Please add at least one credential request.'
					},
					{
						validator: function (val) {
                            return vm.applicationFormId == ENUMS.OnlineForms.Recertification || val.length <= credentialRequestLimit();
						},
						message: function () {
                            return 'A maximum of ' + credentialRequestLimit() + ' credentials can be applied for.';
						}
					}
				]
			});

			vm.validation = ko.validatedObservable([vm.credentialRequests]);

			ko.computed(function () {
				var credentialRequests = ko.toJS(vm.credentialRequests());
				vm.question.Response(JSON.stringify(credentialRequests));
			});

			vm.load = function () {
				vm.credentialApplication.credentialRequestLimit().then(credentialRequestLimit);
				vm.inProgressCredentialsOptions.model.load();
				vm.futureCredentialsOptions.model.load();
			}

			vm.credentialSelectorOptions = {
				credentialRequests: vm.credentialRequests,
				credentialsPromise: function () {
					return vm.credentialApplication.credentials(request());
				},
				categoriesPromise: function () {
					return vm.credentialApplication.categories(request());
				},
				levelsPromise: function () {
					return vm.credentialApplication.levels(request());
				},
				skillsPromise: function () {
					return vm.credentialApplication.skills(request());
				},
				deleteCredentialPromise: function (credentialRequest) {
					var defer = Q.defer();
					vm.credentialApplication.deleteCredential({ Id: credentialRequest.Id, ApplicationId: vm.applicationId(), Token: vm.token() }).then(function (data) {
						vm.clearNextQuestions(vm.question);
						defer.resolve(data);
					});
					return defer.promise;
				},
				createCredentialPromise: function (req) {
					var defer = Q.defer();
					req = $.extend(req, request());
					vm.credentialApplication.createCredential(req).then(function (data) {
						vm.clearNextQuestions(vm.question);
						defer.resolve(data);
					});
					return defer.promise;
				},
				validation: vm.validation
			};

			function request() {
				var request = {
					ApplicationId: vm.applicationId(),
					NAATINumber: vm.naatiNumber(),
					QuestionId: vm.question.Id(),
					CategoryId: vm.credentialSelectorOptions.component.request.CategoryId(),
					LevelId: vm.credentialSelectorOptions.component.request.LevelId(),
					CredentialTypes: vm.credentialSelectorOptions.component.request.CredentialTypes() || '',
					SkillId: vm.credentialSelectorOptions.component.request.SkillId(),
                    Token: vm.token(),
                    ApplicationFormId: vm.applicationFormId()
				};

				return request;
			}

			return vm;
		}
	});