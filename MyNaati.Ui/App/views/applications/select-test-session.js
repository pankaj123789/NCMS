define(['services/requester'],
    function (requester) {
        var applications = requester('applications');
        var questionTypeControl = {};
        var lookups = {};

        questionTypeControl[ENUMS.AnswerTypes.RadioOptions] = 'views/wizard/check-question';
		questionTypeControl[ENUMS.AnswerTypes.TestDetails] = 'views/applications/special-controls/test-details';
		questionTypeControl[ENUMS.AnswerTypes.PaymentControl] = 'views/applications/special-controls/payment-control';

        lookups[ENUMS.AnswerTypes.LanguageSelector] = {
            action: 'languages',
            optionsValue: 'Id',
            optionsText: 'DisplayName'
        };

        var vm = {
			loaded: ko.observable(false),
            testSessionId: ko.observable(),
            credentialRequestId: ko.observable(),
            credentialApplicationId: ko.observable(),
			submitted: ko.observable(false),
			submissionMessage: ko.observable(),
            title: ko.observable('Select Test Session'),
            disablePayPalUi: ko.observable(),
            credentialSkill: ko.observable()
        };

        vm.loaderOptions = {
            name: 'loader',
            params: {
                show: ko.observable(true),
                text: ko.observable('Loading...')
            }
        };

        vm.wizardOptions = {
            sections: ko.observableArray(),
            getNextQuestionFromSection: getNextQuestionFromSection,
            finish: apply,
            finishLabel: 'Finish'
        };

        vm.activate = function () {
            vm.testSessionId(parseInt(window.TestSessionId));
            vm.credentialRequestId(parseInt(window.CredentialRequestId));
            vm.credentialApplicationId(parseInt(window.CredentialApplicationId));
            window.TestSessionId = undefined;
            window.CredentialRequestId = undefined;
            window.CredentialApplicationId = undefined;
            loadSettings();
        };

        vm.getHomeUrl = function () {
            return window.baseUrl;
        };

        function getNextQuestionFromSection(section) {
            var defer = Q.defer();

            var request = {
                SectionId: section.Id(),
                Form: {
                    Sections: prepareRequest()
                },
                TestSessionId: vm.testSessionId(),
                CredentialRequestId: vm.credentialRequestId(),
                CredentialApplicationId: vm.credentialApplicationId()
            };

            applications.post(request, 'selecttestsession/nextquestion').then(function (data) {
                defer.resolve(prepareQuestion(data, section));
            });

            return defer.promise;
        }

        function prepareRequest() {
            var request = ko.toJS(vm.wizardOptions.sections);
            ko.utils.arrayForEach(request, function (r) {
                ko.utils.arrayForEach(r.Questions, function (q) {
                    delete q.section;
                    if (q.Answers) {
                        ko.utils.arrayForEach(q.Answers, function (a) {
                            delete a.question;
                        });
                    }
                });
            });
            return request;
        }

        function loadSettings() {
            applications.getFluid('selecttestsession/settings', { testSessionId: vm.testSessionId(), credentialRequestId: vm.credentialRequestId(), credentialApplicationId: vm.credentialApplicationId() }).then(function (data) {
                ko.utils.arrayForEach(data.Sections, function (d) {
                    d.current = false;
                    d.skipped = false;
                    d.complete = false;
                    d.show = false;
                    d.Questions = [];
                });

                vm.disablePayPalUi(data.DisablePayPalUi);
                vm.credentialSkill(data.Skill);

                var sections = ko.viewmodel.fromModel(data.Sections);
                vm.loaderOptions.params.show(false);
                getNextQuestionFromSection(sections()[0]).then(function (question) {
                    sections()[0].Questions.push(question);
                    vm.wizardOptions.sections(sections());
                });
            });
        }

        function prepareQuestion(question, section) {
            if (!question || question.Id === 0) {
                return null;
            }

            question.composeReady = false;
			question.current = false;
			question.Response = question.Response || null;

            var q = ko.viewmodel.fromModel(question);
            subscribeResponse(q);

            q.section = section;
            fillCompose(q).then(function () {
                q.composeReady(true);
            });

            return q;
        }

        function subscribeResponse(question) {
            var clearNextIfChange = question.Type() === ENUMS.AnswerTypes.RadioOptions || question.Type() === ENUMS.AnswerTypes.CheckOptions || isListControl(question.Type());
            if (clearNextIfChange) {
                var isSingleResponse = question.Type() === ENUMS.AnswerTypes.RadioOptions || isListControl(question.Type());
                var observable = isSingleResponse ? question.Response : question.Responses;
                observable.subscribe(clearNextQuestionsIfNotCurrent(question));
            }
        }

        function clearNextQuestionsIfNotCurrent(question) {
            return function () {
                if (!question.current() && questionInSection(question)) {
                    // I'm using timeout just to prevent the same event from 'change'
                    clearNextQuestions(question);
                }
            };
        }

        function questionInSection(question) {
            return ko.utils.arrayFirst(vm.wizardOptions.sections(), function (section) {
                return ko.utils.arrayFirst(section.Questions(), function (q) {
                    return q.Id() === question.Id();
                }) != null;
            }) != null;
        }

        function clearNextQuestions(question) {
            var sections = vm.wizardOptions.sections();
            var clear = false;
            for (var i = 0; i < sections.length; i++) {
                var s = sections[i];

                if (clear) {
                    s.current(false);
                    s.complete(false);
                    s.skipped(false);
                    s.show(false);
                }

                var questions = s.Questions();
                var j = 0;

                while (j != questions.length) {
                    var q = questions[j];
                    if (clear) {
                        s.Questions.remove(q);
                        q.current(false);
                        q.Response(null);
                        q.Responses([]);

                        var validation = q.getValidation();
                        if (validation && validation.errors) {
                            validation.errors.showAllMessages(false);
                        }
                    }
                    else {
                        j++;
                    }
                    clear = clear || q === question;
                }
            }

            question.current(true);	
            question.section.current(true);
            question.section.show(true);
        }

        function apply() {
            var request = {
                SectionId: 0,
                Form: {
                    Sections: prepareRequest()
                },
                TestSessionId: vm.testSessionId(),
                CredentialRequestId: vm.credentialRequestId(),
                CredentialApplicationId: vm.credentialApplicationId(),
            };

			applications.post(request, 'selecttestsession/applyembedded').then(function (data) {
				if (data.PayOnlineResponse.IsPayByCreditCard && !data.PayOnlineResponse.IsSecurePayStatusSuccess) {

					var errorMessageLine1 = 'Could not process the credit card: ' + data.PayOnlineResponse.ErrorMessage;
					var errorMessageLine2 = 'We are unable to proceed with your request. Please try again and contact NAATI if the problem persists.';

					var questionModel = findQuestion(data.PayOnlineResponse.FeesQuestionId).getCompose().model;
                    questionModel.feesOptions.model.paymentMethod.CreditCardFailMessageLine1(errorMessageLine1);
                    questionModel.feesOptions.model.paymentMethod.CreditCardFailMessageLine2(errorMessageLine2);
					vm.wizardOptions.component.loaderOptions.params.show(false);

					return;
                }
                if (data.PayOnlineResponse.IsPayByPayPal && !data.PayOnlineResponse.IsPayPalStatusSuccess) {

                    var payPalMessageLine = "<p style='color: red'>Could not process the PayPal transaction: " + data.PayOnlineResponse.ErrorMessage +
                        " We are unable to proceed with your request. Please try again and contact NAATI if the problem persists.</p>";

                    var questionModel = findQuestion(data.PayOnlineResponse.FeesQuestionId).getCompose().model;
                    questionModel.feesOptions.model.paymentMethod.PayPalMessageLine(payPalMessageLine);
                    vm.wizardOptions.component.loaderOptions.params.show(false);

                    return;
                }

				vm.title($('<div>' + data.ConfirmContent + '</div>').find('h2:first').text());
				vm.submitted(true);
				vm.submissionMessage(data.ConfirmContent);
			});
		}

		function findQuestion(questionId) {
			var sections = vm.wizardOptions.sections();
			for (var i = sections.length - 1; i >= 0; i--) {
				var questions = sections[i].Questions();

				for (var j = questions.length - 1; j >= 0; j--) {
					var question = questions[j];
					if (question.Id() === questionId) {

						return question;
					}
				}

			}
			return null;
		}

        function fillCompose(question) {
            var promise = bindSpecialControl(question);
            if (isListControl(question.Type())) {
                var request = {
                    Sections: prepareRequest(),
                };

                var lookup = lookups[question.Type()];

                var selectComponentOptions = $.extend(true, {}, lookup, {
                    multiple: false,
                    loadPromise: function () {
                        return applications.post(request, lookup.action);
                    }
                });

                question.getSelectOptions = function () {
                    return selectComponentOptions;
                };
            }
            return promise;
        }

        function bindSpecialControl(question) {
            var control = questionTypeControl[question.Type()];
            var defer = Q.defer();
            require([control], function (model) {
                var instance = model.getInstance({
					question: question,
                    testSessionId: vm.testSessionId,
                    credentialRequestId: vm.credentialRequestId,
                    credentialApplicationId: vm.credentialApplicationId,
                    serviceName: 'selecttestsession',
                    naatiReference: "APP-" + vm.credentialApplicationId() + "-" + vm.credentialSkill(),
                    naatiUnitType: "Test Fee",
                    disablePayPal: vm.disablePayPalUi()
				});

                question.getCompose = function () {
                    return {
                        view: control,
                        model: instance
                    };
                };

                question.getValidation = function () {
                    return instance.validation;
                };

                defer.resolve(question);
            });

            return defer.promise;
        }

        function isListControl(type) {
            var listControls = [
                ENUMS.AnswerTypes.LanguageSelector
            ];

            return $.inArray(type, listControls) != -1;
        };

        return vm;
    }
);