define(['services/requester'],
    function (requester) {
        var onlineDirectory = requester('onlinedirectory');
        var questionTypeControl = {};
        var lookups = {};

        questionTypeControl[ENUMS.AnswerTypes.RadioOptions] = 'views/wizard/check-question';
        questionTypeControl[ENUMS.AnswerTypes.LanguageSelector] = 'views/wizard/list-question';

        lookups[ENUMS.AnswerTypes.LanguageSelector] = {
            action: 'languages',
            optionsValue: 'Id',
            optionsText: 'DisplayName'
        };

        var vm = {
            loaded: ko.observable(false),
            recaptchaToken: ko.observable(),
            introcontent: ko.observable(),
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
            finish: search,
            finishLabel: 'Search'
        };

        vm.activate = function () {
            var interval = setInterval(function () {
                try {
                    grecaptcha.render(
                        'recaptcha',
                        {
                            sitekey: "6LcAPjMUAAAAAO9MGOKhFfw6hEwmK_OjO8G_pH_z",
                            callback: recaptchaCallback,
                            size: "invisible"
                        }
                    );
                    clearInterval(interval);
                }
                catch (err) { }
            }, 500);
            loadSettings();
        };

        vm.getHomeUrl = function () {
            return window.baseUrl;
        };

        var recaptchaDefer = null;
        function recaptchaCallback(token) {
            recaptchaDefer.resolve(token);
            vm.recaptchaToken(token);
        }

        function getNextQuestionFromSection(section) {
            var defer = Q.defer();

            var request = {
                SectionId: section.Id(),
                Form: {
                    Sections: prepareRequest()
                }
            };

            onlineDirectory.post(request, 'nextquestion').then(function (data) {
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
            onlineDirectory.getFluid('settings').then(function (data) {
                vm.introcontent(data.IntroContent);

                ko.utils.arrayForEach(data.Sections, function (d) {
                    d.current = false;
                    d.skipped = false;
                    d.complete = false;
                    d.show = false;
                    d.Questions = [];
                });

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

        function search() {
            recaptchaDefer = Q.defer();

            if (!window.kosettings.skipRecaptcha && !vm.recaptchaToken()) {
                grecaptcha.execute();
            }
            else {
                recaptchaDefer.resolve();
            }

            recaptchaDefer.promise.then(function () {
                var skills = getSkills();
                var form = '<form action="' + window.baseUrl + 'OnlineDirectory/NewSearch" method="POST">' +
  '<input type="hidden" name="g-recaptcha-response" value="' + vm.recaptchaToken() + '">' +
  '<input type="hidden" name="FirstLanguageId" value="' + skills.FirstLanguageId + '">' +
  '<input type="hidden" name="SecondLanguageId" value="' + skills.SecondLanguageId + '">';

                for (var i = 0; i < skills.Skills.length; i++) {
                    form += '<input type="hidden" name="Skills" value="' + skills.Skills[i] + '">';
                }

                form += '</form>';

                $(form).appendTo('body').submit();
            });
        }

        function getSkills() {
            var languageQuestion = null;
            var sections = vm.wizardOptions.sections();

            for (var i = 0; i < sections.length; i++) {
                var s = sections[i];

                languageQuestion = ko.utils.arrayFirst(s.Questions(), function (q) {
                    return q.Type() === ENUMS.AnswerTypes.LanguageSelector;
                });

                if (languageQuestion) {
                    break;
                }
            }

            if (!languageQuestion || !languageQuestion.Response()) {
                return null;
            }

            var tmp = languageQuestion.Response().split('$');
            var languages = tmp[0].split('|');
            var skills = tmp[1].split(',');

            return {
                FirstLanguageId: languages[0],
                SecondLanguageId: languages.length > 1 ? languages[1] : null,
                Skills: skills
            };
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
                        return onlineDirectory.post(request, lookup.action);
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