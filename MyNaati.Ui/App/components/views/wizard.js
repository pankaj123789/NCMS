define(['services/credential-application-service'], function (applicationService) {
	function ViewModel(params) {
		var self = this;
		var options = {
			sections: ko.observableArray(),
			getNextQuestionFromSection: function () { return null; },
			finish: function () { },
			finishLabel: 'Finish'
		};

		options = $.extend(true, options, params);

		params.component = self;

		self.loaderOptions = {
			name: 'loader',
			params: {
				show: ko.observable(false),
				text: ko.observable('Loading...')
			}
		};

		self.sections = options.sections;
		self.getNextQuestionFromSection = options.getNextQuestionFromSection;
		self.performFinish = options.finish;
		self.finishLabel = options.finishLabel;
		self.canContinue = ko.observable(true);
		self.nextQuestion = ko.observable();

        var enableActions = options.enableActions || ko.observable(true);
        self.enableActions = ko.computed(function () {
            return enableActions() && self.canContinue();
        });

		var loadingNextQuestion = [];
        self.next = function (currentSection) {
			self.canContinue(false);

			if (!currentSection) {
				currentSection = getCurrentSection();
			}

			var question = getCurrentQuestion();
			validateQuestion(question).then(function (isValid) {

				if (!isValid) {
					self.canContinue(true);
					return;
				}

				self.loaderOptions.params.show(true);
				Promise.all(loadingNextQuestion).then(function () {
					self.canContinue(true);
					self.loaderOptions.params.show(false);

					if (!self.nextQuestion()) {
						return;
					}

					question.current(false);
					var section = self.nextQuestion().section;
					var hasQuestion = ko.utils.arrayFirst(section.Questions(), function (q) {
						return q.Id() === self.nextQuestion().Id();
					}) != null;

					if (!hasQuestion) {
						self.nextQuestion().current(true);
						section.Questions.push(self.nextQuestion());
					}

					goToSection(section);
					activateAndScrollToQuestion(self.nextQuestion());
					cacheNextQuestion();
				});
			});
		};

		self.finish = function () {
			self.loaderOptions.params.show(true);
			Promise.all(loadingNextQuestion).then(function () {
				self.loaderOptions.params.show(false);

				if (self.nextQuestion()) {
					return self.next();
				}

				self.loaderOptions.params.show(true);
				self.loaderOptions.params.text('Processing...');

				validateAllSections().then(function (isValid) {
					if (isValid) {
						finish();
					}
					else {
						self.loaderOptions.params.show(false);
					}
				});
			});
		};

		self.sections.subscribe(activateFirstQuestion);
		activateFirstQuestion();

		function activateFirstQuestion() {
			if (!self.sections().length || !self.sections()[0].Questions().length) {
				return;
			}

			var section = self.sections()[0];
			var question = section.Questions()[0];
			if (!question.current()) {
				question.current(true);
				goToSection(section);
				cacheNextQuestion();
				activateAndScrollToQuestion(question);
			}
		}

		function finish() {
			var question = getCurrentQuestion();

			validateQuestion(question).then(function (isValid) {
				if (!isValid) {
					self.loaderOptions.params.show(false);
					return;
				}

				self.performFinish();
			});
		}

		function validateAllSections() {
			var sections = ko.utils.arrayFilter(self.sections(), function (s) {
				return s.complete() || s.current();
			});

			return validateAsyncRecursiveSections(0, sections);
		}

		function validateAsyncRecursiveSections(index, sections) {
			if (index >= sections.length) {
				return Q.Promise.resolve(true);
			}

			var defer = Q.defer();

			var section = sections[index];
			var questions = section.Questions();

			validateAsyncRecursiveQuestions(0, questions).then(function (isValid) {
				if (!isValid) {
					return defer.resolve(isValid);
				}

				validateAsyncRecursiveSections(index + 1, sections).then(function (isValid) {
					defer.resolve(isValid);
				});
			});

			return defer.promise;
		}

		function validateAsyncRecursiveQuestions(index, questions) {
			if (index >= questions.length) {
                return Q.Promise.resolve(true);
			}

			var defer = Q.defer();
			var question = questions[index];
			var validation = question.getValidation();

			if (!validation) {
				return Q.Promise.resolve(true);
			}

			Q.Promise.resolve(validation.isValid()).then(function (isValid) {
				if (!isValid) {
					scrollToQuestion(question);
					validation.errors.showAllMessages();
					return defer.resolve(isValid);
				}

				validateAsyncRecursiveQuestions(index + 1, questions).then(function (isValid) {
					defer.resolve(isValid);
				});
			});

			return defer.promise;
		}

		function validateQuestion(question) {
			var defer = Q.defer();
			var validation = question.getValidation();

			if (!validation) {
				return Q.Promise.resolve(true);
			}

			Q.Promise.resolve(validation.isValid()).then(function (isValid) {
				if (!isValid) {
					validation.errors.showAllMessages();
				}
				return defer.resolve(isValid);
			});


			return defer.promise;
		}

		function activateAndScrollToQuestion(question) {
			if (question.composeReady()) {
				load(question);
			}
			question.composeReady.subscribe(function () {
				load(question);
			});

			question.Response.subscribe(function () {
				cacheNextQuestion();
			});
			question.Responses.subscribe(function () {
				cacheNextQuestion();
			});
		}

		function load(question) {
			var model = question.getCompose().model;
			if (model.load) {
				model.load();
			}
			scrollToQuestion(question);
		}

		function cacheNextQuestion(section, defer) {
			self.canContinue(false);

			if (!defer) {
				defer = Q.defer();
				loadingNextQuestion.push(defer.promise);
			}

			if (!section) {
				section = getCurrentSection();
			}

			if (!section) {
				self.nextQuestion(null);
				self.canContinue(true);
				return defer.resolve();
			}

			Q.Promise.resolve(self.getNextQuestionFromSection(section)).then(function (nextQuestion) {
				if (loadingNextQuestion[loadingNextQuestion.length - 1] !== defer.promise) {
					return defer.resolve();
				}

				if (nextQuestion) {
					self.nextQuestion(nextQuestion);
					self.canContinue(true);
					return defer.resolve();
				}

				if (typeof (nextQuestion) == typeof (true) && !nextQuestion) {
					self.nextQuestion(null);
					self.canContinue(true);
					return defer.resolve();
				}

				var nextSection = getNextSection(section);
				if (!nextSection) {
					self.nextQuestion(null);
					self.canContinue(true);
					return defer.resolve();
				}

				cacheNextQuestion(nextSection, defer);
			});
		}

		function getNextSection(currentSection) {
			currentSection = currentSection || getCurrentSection();

			var sections = self.sections();
			var index = -1;
			for (var i = 0; i < sections.length; i++) {
				if (sections[i] === currentSection) {
					index = i;
					break;
				}
			}

			var nextIndex = index + 1;
			if (sections.length === nextIndex) {
				return null;
			}

			return sections[nextIndex];
		}

		function getCurrentSection() {
			return ko.utils.arrayFirst(self.sections(), function (s) {
				return s.current();
			});
		}

		function getCurrentQuestion() {
			var section = getCurrentSection();
			if (!section) {
				return null;
			}

			var questions = section.Questions();
			if (!questions || !questions.length) {
				return null;
			}

			return questions[questions.length - 1];
		}

		function goToSection(nextSection) {
			var section = getCurrentSection();
			if (nextSection === section) {
				return;
			}

			if (section) {
				if (section.current()) {
					section.complete(true);
				}
				else {
					section.skipped(true);
				}

				section.current(false);
			}

			nextSection.show(true);
			nextSection.current(true);
		}

		function scrollToQuestion(question) {
			var container = $('html');
			var scrollTo = $('#question' + question.Id());

			if (!scrollTo.length) {
				return setTimeout(function () { scrollToQuestion(question); }, 100);
			}
			container.animate({
				scrollTop: scrollTo.offset().top - parseInt($('.content').css('padding-top')) - 50,
			}, 'slow');
		}
	}

	return ViewModel;
});