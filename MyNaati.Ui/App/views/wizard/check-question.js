define([],
	function () {
		return {
			getInstance: getInstance
		};

		function getInstance(options) {
			var vm = {
				question: options.question,
			};

			vm.question.Response.extend({
				validation: {
					validator: function (val) {
						return (vm.question.Responses() && vm.question.Responses().length) || val;
					},
					message: 'This answer is required.'
				},
				maxLength: 500
			});

			vm.validation = ko.validatedObservable([vm.question.Response]);

			vm.answerSelected = function (answer, question) {
				if (question.Type() === ENUMS.AnswerTypes.RadioOptions) {
					return question.Response() === answer.Id();
				}

				return ko.utils.arrayFirst(question.Responses(), function (r) {
					return r === answer.Id();
				});
			};

			init();

			function init() {
				var question = vm.question;
				var isSingleResponse = question.Type() === ENUMS.AnswerTypes.RadioOptions;
				var observable = isSingleResponse ? question.Response : question.Responses;

				var preventTryValueSubscriber = false;

				observable.subscribe(function (answer) {
					preventTryValueSubscriber = true;
					question.tryValue($.isArray(answer) ? answer.slice() : answer);
					preventTryValueSubscriber = false;
				});

				question.tryValue = isSingleResponse ? ko.observable() : ko.observableArray();
				question.tryValue.subscribe(function (answer) {
					if (preventTryValueSubscriber) {
						return;
					}

					if (compareAnswer(answer, observable(), isSingleResponse)) {
						return;
					}

					if (question.current()) {
						// I'm using timeout just to prevent the same event from 'change'
						return setTimeout(function () { setAnswer(observable, answer, isSingleResponse); }, 0);
					}

					mbox.confirm({ title: 'Change Answer', content: 'The next sections will reload if you change the answer. Are you sure that you want to change the answer?' }).then(function (argument) {
						if (argument === 'yes') {
							setAnswer(observable, answer, isSingleResponse);
						}
						else if (argument === 'no') {
							setAnswer(question.tryValue, observable(), isSingleResponse);
						}
					});
				});
			}

			function compareAnswer(a, b, isSingleResponse) {
				if (isSingleResponse) {
					return a === b;
				}

				if (!a && b || a && !b) {
					return false;
				}

				if (a.length !== b.length) {
					return false;
				}

				for (var i = 0; i < a.length; i++) {
					if (a[i] !== b[i]) {
						return false;
					}
				}

				return true;
			}

			function setAnswer(observable, answer, isSingleResponse) {
				if (isSingleResponse) {
					return observable(answer);
				}

				observable((answer || []).slice());
			}

			return vm;
		}
	});