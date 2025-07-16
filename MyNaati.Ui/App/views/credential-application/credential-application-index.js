define(['services/credential-application-service'],
    function (applicationService) {
        var credentialApplication = applicationService;
        var questionTypeControl = {};
        var lookups = {};

        questionTypeControl[ENUMS.AnswerTypes.Date] = 'views/wizard/date-question';
        questionTypeControl[ENUMS.AnswerTypes.RadioOptions] = 'views/wizard/check-question';
        questionTypeControl[ENUMS.AnswerTypes.CheckOptions] = 'views/wizard/check-question';
        questionTypeControl[ENUMS.AnswerTypes.TestLocation] = 'views/wizard/list-question';
        questionTypeControl[ENUMS.AnswerTypes.ProductSelector] = 'views/wizard/list-question';
        questionTypeControl[ENUMS.AnswerTypes.CountrySelector] = 'views/wizard/list-question';
        questionTypeControl[ENUMS.AnswerTypes.EndorsedQualification] = 'views/wizard/list-question';
        questionTypeControl[ENUMS.AnswerTypes.EndorsedQualificationInstitution] = 'views/wizard/list-question';
        questionTypeControl[ENUMS.AnswerTypes.EndorsedQualificationLocation] = 'views/wizard/list-question';
        questionTypeControl[ENUMS.AnswerTypes.Input] = 'views/wizard/text-question';
        questionTypeControl[ENUMS.AnswerTypes.Email] = 'views/wizard/email-question';
        questionTypeControl[ENUMS.AnswerTypes.RecertificationCredentialSelector] = {
            view: 'views/wizard/check-question',
            model: 'views/wizard/recertification-credential'
        };

        questionTypeControl[ENUMS.AnswerTypes.LanguageSelector] = 'views/credential-application/special-controls/language-selector';
        questionTypeControl[ENUMS.AnswerTypes.PersonVerification] = 'views/credential-application/special-controls/person-verification';
        questionTypeControl[ENUMS.AnswerTypes.PersonDetails] = 'views/credential-application/special-controls/person-details';
        questionTypeControl[ENUMS.AnswerTypes.DocumentUpload] = 'views/credential-application/special-controls/document-upload';
        questionTypeControl[ENUMS.AnswerTypes.CredentialSelector] = 'views/credential-application/special-controls/credential-selector';
        questionTypeControl[ENUMS.AnswerTypes.PersonPhoto] = 'views/credential-application/special-controls/person-photo';
        questionTypeControl[ENUMS.AnswerTypes.TestSessions] = 'views/credential-application/special-controls/test-sessions';
        questionTypeControl[ENUMS.AnswerTypes.CredentialSelectorUpgradeAndSameLevel] = 'views/credential-application/special-controls/credential-selector-upgrade-same-level';
        questionTypeControl[ENUMS.AnswerTypes.Fees] = 'views/credential-application/special-controls/fees';

        lookups[ENUMS.AnswerTypes.EndorsedQualificationInstitution] = {
            action: 'endorsedqualificationinstitution',
            optionsValue: 'Id',
            optionsText: 'Name'
        };
        lookups[ENUMS.AnswerTypes.EndorsedQualificationLocation] = {
            action: 'endorsedqualificationlocation',
            optionsValue: 'Id',
            optionsText: 'Name',
            request: function (question) {
                var institutionId = 0;
                var institutionQuestion = findQuestion(ENUMS.AnswerTypes.EndorsedQualificationInstitution, false);

                if (institutionQuestion) {
                    var questionModel = institutionQuestion.getCompose().model;
                    institutionId = questionModel.question.Response();
                }

                return {
                    ApplicationId: vm.applicationId(),
                    NAATINumber: vm.naatiNumber(),
                    QuestionId: question.Id(),
                    ApplicationFormId: vm.applicationFormId(),
                    InstitutionId: institutionId,
                    Token: vm.token()
                };
            }
        };
        lookups[ENUMS.AnswerTypes.EndorsedQualification] = {
            action: 'endorsedqualification',
            optionsValue: 'Id',
            optionsText: 'Name',
            request: function (question) {
                var institutionId = 0;
                var location = '';

                var institutionQuestion = findQuestion(ENUMS.AnswerTypes.EndorsedQualificationInstitution, false);
                var locationQuestion = findQuestion(ENUMS.AnswerTypes.EndorsedQualificationLocation, false);

                if (institutionQuestion) {
                    var questionModel = institutionQuestion.getCompose().model;
                    institutionId = questionModel.question.Response();
                }

                if (locationQuestion) {
                    var questionLocationModel = locationQuestion.getCompose().model;
                    location = questionLocationModel.question.Response();
                }

                return {
                    ApplicationId: vm.applicationId(),
                    NAATINumber: vm.naatiNumber(),
                    QuestionId: question.Id(),
                    ApplicationFormId: vm.applicationFormId(),
                    InstitutionId: institutionId,
                    Location: location,
                    Token: vm.token()
                };
            }
        };

        lookups[ENUMS.AnswerTypes.LanguageSelector] = {
            action: 'languages',
            optionsValue: 'Id',
            optionsText: 'Name'
        };
        lookups[ENUMS.AnswerTypes.TestLocation] = {
            action: 'locations',
            optionsValue: 'Id',
            optionsText: 'Name'
        };
        lookups[ENUMS.AnswerTypes.CountrySelector] = {
            action: 'countries',
            optionsValue: 'SamId',
            optionsText: 'DisplayText',
            addChooseOption: false
        };;

        var vm = {
            showSubmissionResult: ko.observable(false),
            showSubmissionResultContent: ko.observable(),
            loaded: ko.observable(false),
            sections: [],
            questions: [],
            specialControls: {},
            listControls: {},
            naatiNumber: ko.observable(),
            applicationId: ko.observable(),
            forms: ko.observable(),
            applicationFormId: ko.observable(),
            title: ko.observable(),
            homeApplicationLink: window.baseUrl + "Apply",
            credentialRequests: ko.observableArray(),
            token: ko.observable(),
            homeLink: ko.observable(window.baseUrl),
            enableActions: ko.observable(true)
        };

        vm.loaderOptions = {
            name: 'loader',
            params: {
                show: ko.observable(true),
                text: ko.observable('Loading forms...')
            }
        };

        vm.formSelectorClass = ko.pureComputed(function () {
            if (!vm.forms() || !vm.forms().length) {
                return 'col-sm-12';
            }

            return 'col-sm-' + Math.max(parseInt(12 / vm.forms().length), 4);
        });

        vm.activate = function () {
            var pathArray = window.location.pathname.toUpperCase().split('/APPLY/');
            var applicationFormId = 0;
            var applicationFormUrl = pathArray[pathArray.length - 1];
            if (applicationFormUrl && pathArray.length > 1) {
                credentialApplication.forms().then(function (data) {
                    if (!data.IsAuthenticated)
                        vm.homeLink(data.BaseUrl);

                    for (var i = 0; i < data.Results.length; i++) {

                        var form = data.Results[i];
                        if (form.Url.toUpperCase() === applicationFormUrl.toUpperCase()) {
                            vm.title('Apply for ' + form.DisplayName);
                            applicationFormId = form.Id;
                            vm.applicationFormId(applicationFormId);
                            loadSections();
                            return true;
                        }
                    }
                    window.location.href = window.baseUrl + "Apply";
                    return false;

                });

            }
            else {
                credentialApplication.forms().then(function (data) {
                    vm.loaded(true);
                    vm.loaderOptions.params.show(false);

                    vm.title('Applications Home');

                    ko.utils.arrayForEach(data.Results, function (r) {
                        var icon = 'Diploma.png';

                        if (r.Id === ENUMS.OnlineForms.CCL || r.Id === ENUMS.OnlineForms.EmptyCCLV2 || r.Id === ENUMS.OnlineForms.CCLV2) {
                            icon = 'CCL-testing.png';
                        }
                        else if (r.Id === ENUMS.OnlineForms.CLA) {
                            icon = 'CLA-testing.png';
                        }
                        else if (r.Id === ENUMS.OnlineForms.Certification) {
                            icon = 'certification-testing.png';
                        }
                        else if (r.Id === ENUMS.OnlineForms.CertificationPractitioner) {
                            icon = 'additional-certifications.png';
                        }
                        else if (r.Id === ENUMS.OnlineForms.Recertification) {
                            icon = 'recertification.png';
                        }
                        else if (r.Id === ENUMS.OnlineForms.AdvancedAndSpecialist) {
                            icon = 'advanced-and-specialist.png';
                        }
                        else if (r.Id === ENUMS.OnlineForms.PracticeTest) {
                            icon = 'PracticeTest.svg';
                        }

                        r.Icon = window.baseUrl + 'Content/Images/' + icon;
                    });
                    if (!data.IsAuthenticated)
                        vm.homeLink(data.BaseUrl);

                    vm.forms(data.Results);
                });
            }
        };

        vm.selectForm = function (form) {
            window.location.href = window.baseUrl + "Apply/" + form.Url;
        };

        vm.isSpecialControl = function (type) {
            return true;
        };

        vm.isListControl = function (type) {
            var listControls = [
                ENUMS.AnswerTypes.EndorsedQualification,
                ENUMS.AnswerTypes.EndorsedQualificationInstitution,
                ENUMS.AnswerTypes.EndorsedQualificationLocation,
                ENUMS.AnswerTypes.LanguageSelector,
                ENUMS.AnswerTypes.TestLocation,
                ENUMS.AnswerTypes.ProductSelector,
                ENUMS.AnswerTypes.CountrySelector
            ];

            return $.inArray(type, listControls) != -1;
        };

        vm.next = function (section) {
            return vm.wizardOptions.component.next(section);
        };

        vm.getLastAnswer = function () {
            var question = getCurrentQuestion();
            if (!question) {
                return null;
            }

            return getSelectedAnswer(question);
        };

        vm.getLastAnswerFunction = function () {
            var answer = vm.getLastAnswer();

            if (!answer) {
                return null;
            }

            return answer.Function;
        };

        vm.wizardOptions = {
            sections: ko.observableArray(),
            getNextQuestionFromSection: getNextQuestionFromSection,
            finish: finish,
            enableActions: vm.enableActions
        };

        function getSelectedAnswer(questionObservable) {
            if (!questionObservable) {
                return null;
            }

            var question = ko.toJS(questionObservable);
            var answers = question.Answers;
            if (!answers)
                return null;

            var response = question.Response;
            if (!response) {
                return null;
            }

            var answer = ko.utils.arrayFirst(answers, function (a) {
                return a.Id === response;
            });

            return answer;
        }

        function findQuestion(id, useType) {

            var sections = vm.wizardOptions.sections();
            for (var i = sections.length - 1; i >= 0; i--) {
                var questions = sections[i].Questions();

                for (var j = questions.length - 1; j >= 0; j--) {
                    var question = questions[j];
                    if ((useType ? question.Id() : question.Type()) === id) {
                        return question;
                    }
                }

            }
            return null;
        }

        function finish() {
            var request = {
                Sections: prepareRequest(),
                ApplicationId: vm.applicationId() || 0,
                NaatiNumber: vm.naatiNumber() || 0,
                ApplicationFormId: vm.applicationFormId() || 0,
                Token: vm.token() || 0
            };

            var func = vm.getLastAnswerFunction();

            if (!func) {

                return credentialApplication.submit(request).then(function (data) {

                    if (data.PayOnlineResponse != null && data.PayOnlineResponse.IsPayByCreditCard && !data.PayOnlineResponse.IsSecurePayStatusSuccess) {

                        var errorMessageLine1 = 'Could not process the credit card: ' + data.PayOnlineResponse.ErrorMessage;
                        var errorMessageLine2 = 'We are unable to proceed with your request. Please try again and contact NAATI if the problem persists.';

                        var questionModel = findQuestion(data.PayOnlineResponse.FeesQuestionId).getCompose().model;
                        questionModel.feesOptions.model.paymentMethod.CreditCardFailMessageLine1(errorMessageLine1);
                        questionModel.feesOptions.model.paymentMethod.CreditCardFailMessageLine2(errorMessageLine2);
                        vm.wizardOptions.component.loaderOptions.params.show(false);

                        return;
                    }
                    if (data.SaveApplicationFormResponse.ErrorMessage) {
                        vm.title('An Error Occurred');
                        vm.showSubmissionResult(true);
                        vm.showSubmissionResultContent(data.SaveApplicationFormResponse.ErrorMessage);
                    }
                    else {
                        vm.title('Application submission result');
                        vm.showSubmissionResult(true);
                        vm.showSubmissionResultContent(data.ConfirmContent);
                        //window.location.href = window.baseUrl + "apply/submission-result/" + data.SaveApplicationFormResponse.ApplicationReference;
                    }
                });
            }

            if (func.Type === ENUMS.AnswerFunctionTypes.Submit) {
                credentialApplication.submit(request).then(function () {
                    redirectTo(func.Parameter);
                });
            }
            else if (func.Type === ENUMS.AnswerFunctionTypes.Redirect) {
                redirectTo(func.Parameter);
            }
            else if (func.Type === ENUMS.AnswerFunctionTypes.Delete) {
                credentialApplication.remove(request).then(function () {
                    redirectTo(func.Parameter);
                });
            }
        }

        function loadSections() {
            credentialApplication.sections({ ApplicationFormId: vm.applicationFormId() }).then(function (data) {
                vm.sections = data.sort(sortByDisplayOrder);

                ko.utils.arrayForEach(vm.sections, function (s, i) {
                    s.show = false;
                    s.current = false;
                    s.complete = false;
                    s.skipped = false;

                    var questions = s.Questions.sort(sortByDisplayOrder);

                    ko.utils.arrayForEach(questions, function (q, j) {
                        q.Answers = (q.Answers || []).sort(sortByDisplayOrder);
                        q.current = false;
                        q.composeReady = false;
                    });

                    var section = ko.viewmodel.fromModel(s);
                    section.Questions([]);

                    if (i === 0) {
                        if (questions.length) {
                            var q = prepareQuestion(section, questions[0]);
                            section.Questions.push(q);
                        }
                    }

                    vm.wizardOptions.sections.push(section);
                });

                vm.loaded(true);
                vm.loaderOptions.params.show(false);
            });
        }

        function prepareQuestion(section, question) {
            var q = ko.viewmodel.fromModel(question);


            q.section = section;

            ko.utils.arrayForEach(q.Answers(), function (a) {
                a.question = q;
            });

            fillCompose(q).then(function () {
                q.Response.extend({ deferred: true });
                q.Responses.extend({ deferred: true });
                subscribeResponse(q);
                q.composeReady(true);
            });


            return q;
        }

        function sortByDisplayOrder(a, b) {
            return ((a.DisplayOrder < b.DisplayOrder) ? -1 : ((a.DisplayOrder > b.DisplayOrder) ? 1 : 0));
        }

        function fillCompose(question) {
            var promise = bindSpecialControl(question);
            if (vm.isListControl(question.Type())) {
                var lookup = lookups[question.Type()];

                var request = {
                    ApplicationId: vm.applicationId(),
                    NAATINumber: vm.naatiNumber(),
                    QuestionId: question.Id(),
                    ApplicationFormId: vm.applicationFormId(),
                    Token: vm.token()
                };

                if (lookup.request) {
                    request = lookup.request(question);
                }

                var selectComponentOptions = $.extend(true, {}, lookup, {
                    multiple: false,
                    loadPromise: function () {
                        return credentialApplication.lookup(lookup.action, request);
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
            var view = null;
            var modelScript = null;

            if (typeof (control) === 'string') {
                view = control;
                modelScript = control;
            }
            else {
                view = control.view;
                modelScript = control.model;
            }

            var defer = Q.defer();
            require([modelScript], function (model) {
                var instance = model.getInstance({
                    applicationFormId: vm.applicationFormId,
                    applicationId: vm.applicationId,
                    naatiNumber: vm.naatiNumber,
                    question: question,
                    sections: prepareRequest(),
                    credentialRequests: vm.credentialRequests,
                    clearNextQuestions: clearNextQuestions,
                    loaderOptions: vm.loaderOptions,
                    credentialApplication: credentialApplication,
                    next: vm.next,
                    token: vm.token,
                    enableActions: vm.enableActions
                });

                question.getCompose = function () {
                    return {
                        view: view,
                        model: instance
                    };
                };

                question.getValidation = function () {
                    return instance.validation;
                };

                Q.Promise.resolve(instance.promise).then(function (val) {
                    defer.resolve(question);
                });

            });

            return defer.promise;
        }

        function redirectTo(url) {
            document.location.href = url;
        }

        function subscribeResponse(question) {
            var clearNextIfChange = question.Type() === ENUMS.AnswerTypes.RadioOptions ||
                question.Type() === ENUMS.AnswerTypes.CheckOptions ||
                question.Type() === ENUMS.AnswerTypes.RecertificationCredentialSelector ||
                vm.isListControl(question.Type()) ||
                question.Type() === ENUMS.AnswerTypes.DocumentUpload;
            if (clearNextIfChange) {
                var isSingleResponse = question.Type() === ENUMS.AnswerTypes.RadioOptions || vm.isListControl(question.Type()) ||
                    question.Type() === ENUMS.AnswerTypes.DocumentUpload;
                var observable = isSingleResponse ? question.Response : question.Responses;
                observable.subscribe(clearNextQuestionsIfNotCurrent(question));
            }
            question.Response.subscribe(responseChanged);
            question.Responses.subscribe(responseChanged);
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
            preventSave = true;

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

            preventSave = false;
        }

        var preventSave = false;
        function responseChanged() {
            if (preventSave) {
                return;
            }

            var request = {
                Sections: prepareRequest(),
                ApplicationId: vm.applicationId() || 0,
                NaatiNumber: vm.naatiNumber() || 0,
                ApplicationFormId: vm.applicationFormId() || 0,
                Token: vm.token() || 0
            };

            credentialApplication.save(request).then(function (data) {
                vm.applicationId(data.ApplicationId);
            });
        }

        function prepareRequest() {
            var request = ko.toJS(vm.wizardOptions.sections);
            ko.utils.arrayForEach(request, function (r) {
                ko.utils.arrayForEach(r.Questions, function (q) {
                    delete q.section;
                    ko.utils.arrayForEach(q.Answers, function (a) {
                        delete a.question;
                    });
                });
            });
            return request;
        }

        function getNextQuestionFromSection(section, exclusiveQuestion) {
            if (!section) {
                return null;
            }

            var viewSection = ko.utils.arrayFirst(vm.sections, function (s) {
                return s.Id === section.Id();
            });

            if (!exclusiveQuestion) {
                var qs = section.Questions();
                exclusiveQuestion = qs[qs.length - 1];
            }

            var answer = getSelectedAnswer(exclusiveQuestion);

            if (answer && answer.Function) {
                return false;
            }

            var startIndex = 0;
            var questions = viewSection.Questions;
            if (exclusiveQuestion) {
                ko.utils.arrayFirst(questions, function (q, i) {
                    if (exclusiveQuestion.Id() === q.Id) {
                        startIndex = i + 1;
                        return true;
                    }
                });
            }

            if (startIndex >= questions.length) {
                return null;
            }

            var nextQuestion = prepareQuestion(section, questions[startIndex]);

            var defer = Q.defer();
            Q.Promise.resolve(checkLogic(nextQuestion)).then(function (logic) {
                if (logic) {
                    if (hasTokens(nextQuestion)) {
                        credentialApplication.replaceTokens({ QuestionId: nextQuestion.Id(), ApplicationFormId: vm.applicationFormId() }).then(function (data) {
                            ko.viewmodel.updateFromModel(nextQuestion, data);
                            defer.resolve(nextQuestion);
                        });
                    }
                    else {
                        defer.resolve(nextQuestion);
                    }
                }
                else {
                    Q.Promise.resolve(getNextQuestionFromSection(section, nextQuestion)).then(defer.resolve);
                }
            });

            return defer.promise;
        }

        function hasTokens(question) {
            var questionToken = question.HasTokens && question.HasTokens();
            if (questionToken) {
                return true;
            }

            if (!question.Answers || !question.Answers().length) {
                return false;
            }

            var answerToken = false;
            ko.utils.arrayForEach(question.Answers(), function (a) {
                answerToken = answerToken || (a.HasTokens && a.HasTokens());
            });

            return answerToken;
        }

        function checkLogic(question) {
            var logics = question.Logics();
            if (!logics || !logics.length) {
                return true;
            }

            var logicGroups = groupBy(logics, 'Group');

            var deferArray = [];

            for (var i = 0; i < Object.keys(logicGroups).length; i++) {
                var logicGroup = logicGroups[i];
                var joinWithAnd = logicGroup[0].And(); // use And() value from first logic of the group
                deferArray.push(checkLogicGroup(logicGroup));
            }

            var defer = Q.defer();

            Promise.all(deferArray).then(function (values) {
                var result = null;

                for (var i = 0; i < values.length; i++) {
                    var groupResult = values[i];

                    if (result == null) {
                        result = groupResult;
                    }

                    if (joinWithAnd) {
                        result = result && groupResult;
                    }
                    else {
                        result = result || groupResult;
                    }
                }

                defer.resolve(result);
            });

            return defer.promise;
        }

        function checkLogicGroup(logicsGrop) {
            var orderedLogics = logicsGrop.sort(function (a, b) { return a.Order() - b.Order() });
            var deferArray = [];

            for (var j = 0; j < orderedLogics.length; j++) {
                var logic = orderedLogics[j];
                var answerId = logic.AnswerId();
                var skillId = logic.SkillId();

                if (logic.Type() === ENUMS.QuestionLogicTypes.CredentialType) {
                    deferArray.push(checkCredentialType(answerId));
                }
                else if (logic.Type() === ENUMS.QuestionLogicTypes.CredentialRequestPathType) {
                    deferArray.push(checkPathType(answerId));
                }
                else if (logic.Type() === ENUMS.QuestionLogicTypes.PdPoints) {
                    var defer = Q.defer();
                    deferArray.push(defer.promise);
                    (function (defer) {
                        credentialApplication.pdPointsMet({ ApplicationId: vm.applicationId() }).then(function (data) {
                            defer.resolve(data.Met);
                        });
                    })(defer);
                }
                else if (logic.Type() === ENUMS.QuestionLogicTypes.WorkPractice) {
                    var defer = Q.defer();
                    deferArray.push(defer.promise);
                    (function (defer) {
                        credentialApplication.workPracticeMet({ ApplicationId: vm.applicationId() }).then(function (data) {
                            defer.resolve(data.Met);
                        });
                    })(defer);
                }
                else if (logic.Type() === ENUMS.QuestionLogicTypes.Skill) {
                    deferArray.push(checkSkill(skillId));
                }
                else {
                    deferArray.push(checkAnswerOption(answerId));
                }
            }

            var defer = Q.defer();
            Promise.all(deferArray).then(function (values) {
                var groupResult = null;

                for (var j = 0; j < values.length; j++) {
                    var logic = orderedLogics[j];
                    var isAnswerSelected = values[j];

                    if (logic.Not()) {
                        isAnswerSelected = !isAnswerSelected;
                    }

                    if (groupResult == null) {
                        groupResult = isAnswerSelected;
                    }

                    if (logic.And()) {
                        groupResult = groupResult && isAnswerSelected;
                    }
                    else {
                        groupResult = groupResult || isAnswerSelected;
                    }
                }

                defer.resolve(groupResult);
            });

            return defer.promise;
        }

        function groupBy(items, propertyName) {
            return items.reduce(function (reducedVector, item) {
                (reducedVector[item[propertyName]()] = reducedVector[item[propertyName]()] || []).push(item);
                return reducedVector;
            }, {});
        };

        function checkAnswerOption(answerId) {
            var sections = vm.wizardOptions.sections();
            for (var i = 0; i < sections.length; i++) {
                var section = sections[i];
                if (!section.show()) {
                    continue;
                }

                var questions = section.Questions();
                for (var j = 0; j < questions.length; j++) {
                    var question = questions[j];

                    if (question.Type() !== ENUMS.QuestionLogicTypes.AnswerOption && question.Type() !== ENUMS.QuestionLogicTypes.CredentialType) {
                        continue;
                    }

                    var responses = question.Responses() || [];
                    for (var k = 0; k < responses.length; k++) {
                        var response = responses[k];
                        if (response === answerId) {
                            return true;
                        }
                    }

                    if (question.Response() === answerId) {
                        return true;
                    }
                }
            }

            return false;
        }

        function checkCredentialType(credentialTypeId) {

            var requests = vm.credentialRequests();
            for (var i = 0; i < requests.length; i++) {
                var request = requests[i];
                if (request.LevelId === credentialTypeId) {
                    return true;
                }
            }
            return false;
        }

        function checkSkill(skillId) {

            var requests = vm.credentialRequests();
            for (var i = 0; i < requests.length; i++) {
                var request = requests[i];
                if (request.SkillId === skillId) {
                    return true;
                }
            }
            return false;
        }

        function checkPathType(credentialPathTypeId) {

            var requests = vm.credentialRequests();
            for (var i = 0; i < requests.length; i++) {
                var request = requests[i];
                if (request.PathId === credentialPathTypeId) {
                    return true;
                }
            }
            return false;
        }

        function getCurrentSection() {
            return ko.utils.arrayFirst(vm.wizardOptions.sections(), function (s) {
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

        return vm;
    }
);