define([
    'plugins/router',
    'services/screen/date-service',
    'services/testresult-data-service',
    'services/test-data-service',
    'services/examiner-data-service',
    'services/screen/message-service',
    'services/util',
    'modules/enums',
    'modules/common',

], function (router, dateService, testresultService, testService, examinerService, message, util, enums, common) {

    return {
        getInstance: function (editMarksInstance, confirmationInstance) {
            var eventDefer = Q.defer();
            var functions = common.functions();

            var testResultServerModel = {
                TestResultId: ko.observable(),
                CurrentJobId: ko.observable(),
                ResultTypeId: ko.observable(1),
                ResultChecked: ko.observable(),
                AllowCalculate: ko.observable(),
                IncludePreviousMarks: ko.observable(),
                AllowIssue: ko.observable(),
                AutomaticIssuingExaminer: ko.observable(),
                CommentsGeneral: ko.observable(),
                ProcessedDate: ko.observable(),
                SatDate: ko.observable(),
                ThirdExaminerRequired: ko.observable(),
                DueDate: ko.observable(),
                OriginalResultChecked: ko.observable(),
                OriginalProcessedDate: ko.observableArray(),
                OriginalResultTypeId: ko.observableArray(),
                OriginalDueDate: ko.observable(),
                EligibleForConcededPass: ko.observable(),
                EligibleForSupplementary: ko.observable(),
            };

            testResultServerModel.FormattedProcessedDate = ko.pureComputed({
                read: function () {
                    var date = testResultServerModel.ProcessedDate();
                    if (date) {
                        return moment(date).format(CONST.settings.shortDateDisplayFormat);
                    }

                    return date;
                },
                write: function (value) {
                    var date = dateService.toPostDate(value);
                    testResultServerModel.ProcessedDate(date);
                }
            });

            testResultServerModel.FormattedDueDate = ko.pureComputed({
                read: function () {
                    var date = testResultServerModel.DueDate();
                    if (date) {
                        return moment(date).format(CONST.settings.shortDateDisplayFormat);
                    }
                    return date;
                },
                write: function (value) {
                    var date = dateService.toPostDate(value);
                    testResultServerModel.DueDate(date);
                }
            });


            var vm = {
                testResult: testResultServerModel,
                examiners: ko.observableArray(),
                test: ko.observable(),
                event: eventDefer.promise,
                isLoading: false,
                originalResultInstance: ko.observable(),
                isReview: function () {
                    return vm && vm.originalResultInstance();
                },
                marksTabId: ko.observable(),
                selectedResultsSubscriber: null,
                specification: {
                    Components: ko.observableArray([]),
                    OverallPassMark: ko.observable()
                },
                testDateSubscriber: null,
                editMarksInstance: editMarksInstance,
                confirmationInstance: confirmationInstance,
                requireThirdMarking: requireThirdMarking,
                getSumMarks: getSumMarks,
                getSumMarksForExaminer: getSumMarksForExaminer,
                willPass: willPass,
                willFail: willFail,
                hasPassed: ko.observable(),
                totalMarks: totalMarks,
                systemParameter: {},
                load: load,
                resultTypeIdOptions: ko.observableArray([]),
                resultTypeOptionsAfterRender: setOptionDisable,

                selectedResults: ko.observableArray(),
                allowResultOverride: ko.observable(false),
                getExaminerMarking: getExaminerMarking,
                getComponentResult: getComponentResult,
                getMark: getMark,
                getResult: getResult,
                getExaminerComputedResultText: getExaminerComputedResultText,
                examinerComputedEligibleForPass: examinerComputedEligibleForPass,
                getRubricResult: getRubricResult,
                getPrimaryReasonForFailure: getPrimaryReasonForFailure,
                validate: validate,
                clearValidation: clearValidation,
                save: save,
                activate: activate,
                formatDate: formatDate,
                computeFinalRubric: computeFinalRubric,
                computeResults: computeResults,
                ComputedEligibleForPass: ko.observable(),
                ComputedEligibleForConcededPass: ko.observable(),
                ComputedEligibleForSupplementary: ko.observable(),
                ResultAutoCalculation: ko.observable(),
                disableFields: ko.pureComputed(function () {

                    if (vm.isloading) {
                        return true;
                    }
                    var viewModel = vm;

                    return viewModel.testResult.OriginalResultChecked();
                }),
                disableResultChecked: ko.pureComputed(function () {
                    var viewModel = vm;
                    if (vm.originalResultInstance()) {
                        viewModel = vm.originalResultInstance();
                    }

                    var hasLoaded = viewModel && viewModel.test();

                    if (!hasLoaded || !vm.test()) {
                        return true;
                    }
                    var thereIsSupplemantary = !vm.test().Supplementary() && vm.test().SupplementaryCredentialRequest();
                    var testResultIssued = !vm.isReview() && (vm.test().LastReviewTestResultId() || vm.test().CredentialRequestStatusTypeId() !== enums.CredentialRequestStatusTypes.TestSat);
                    var reviewIsued = vm.isReview() && vm.test().CredentialRequestStatusTypeId() !== enums.CredentialRequestStatusTypes.UnderPaidTestReview;

                    return thereIsSupplemantary || testResultIssued || reviewIsued;
                }),
                editMarks: function (examiner) {
                    if (vm.isRubric()) {
                        if (examiner) {
                            return router.navigate('test-rubric-marks/' + examiner.JobExaminerId);
                        }
                        return router.navigate('test-rubric-final/' + vm.testResult.TestResultId());
                    }
                    var testResultId = vm.testResult.TestResultId();
                    var jobExaminerId = examiner ? examiner.JobExaminerId : null;
                    vm.editMarksInstance.edit(testResultId, vm.includePreviousMarks(), jobExaminerId).then(function (data) {
                        eventDefer.notify({
                            name: (examiner ? 'Mark' : 'Result') + 'Saved',
                            data: data
                        });
                    });
                },
                noneSelected: ko.pureComputed(function () {
                    return ko.Localization('Naati.Resources.Shared.resources.NoneSelected');
                }),
                canOverrideResults: ko.observable(false)
            };

            var numSelectedResults = 0;

            vm.allowResultOverride = ko.pureComputed({
                read: function () {
                    return !vm.testResult.AllowCalculate();
                },
                write: function (value) {
                    vm.testResult.AllowCalculate(!value);

                    if (vm.isLoading) {
                        return;
                    }

                    saveMarksOverridden();
                }
            });

            vm.allowIssue = ko.pureComputed({
                read: function () {
                    return vm.testResult.AllowIssue();
                },
                write: function (value) {
                    if (vm.isLoading) {
                        return;
                    }
                    vm.testResult.AllowIssue(value);
                }
            });

            vm.includePreviousMarks = ko.pureComputed({
                read: function () {
                    return vm.testResult.IncludePreviousMarks();
                },
                write: function (value) {

                    if (vm.isLoading) {
                        return;
                    }
                    vm.testResult.IncludePreviousMarks(value);
                    saveMarksOverridden();
                    updateTestComponentResult();
                }
            });

            vm.confirmAllowCalculate = function (data, event) {
                if (vm.allowResultOverride()) {
                    event.stopImmediatePropagation();

                    message.confirm({
                        title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                        content: ko.Localization('Naati.Resources.Test.resources.ConfirmAllowCalculate')
                    })
                        .then(function (answer) {
                            vm.allowResultOverride(answer !== 'yes');
                        });

                    return false;
                }

                return true;
            }

            vm.disableEditResult = ko.pureComputed(function () {
                return !vm.allowResultOverride() || vm.disableFields() || !vm.selectedResults() || !vm.selectedResults().length;
            });

            vm.disableOverrideMarks = ko.computed(function () {
                return vm.disableFields() || !vm.selectedResults() || !vm.selectedResults().length || !vm.canOverrideResults();
            });

            vm.overrideMarksToolTip = ko.computed(function () {
                if (!vm.canOverrideResults()) {
                    return 'Naati.Resources.Test.resources.MarksOverriddenRequiresAdministratorTooltip';
                } else {
                    return 'Naati.Resources.Test.resources.MarksOverriddenTooltip';
                }
            });

            vm.hasFailed = ko.pureComputed(function () {
                return vm.testResult.ResultTypeId && (vm.testResult.ResultTypeId() === enums.TestResultType.Failed);
            });


            vm.ExaminerFromTestResultIDSorted = ko.computed(function () {

                return vm.examiners().sort(function (left, right) {
                    return left.JobExaminerId === right.JobExaminerId ? 0 : (left.JobExaminerId < right.JobExaminerId ? -1 : 1);
                });
            });

            vm.dueDateOptions = {
                disable: ko.computed(function () {
                    return vm.disableFields() || !currentUser.hasPermissionSync(enums.SecNoun.TestResult, enums.SecVerb.Manage);
                }),
                value: vm.testResult.FormattedDueDate,
                resattr: {
                    placeholder: 'Naati.Resources.Test.resources.ResultDue'
                }
            };

            vm.ResultChecked = ko.pureComputed({
                read: function () {
                    return vm.testResult.ResultChecked();
                },
                write: function (value) {


                    if (vm.isLoading) {
                        return;
                    }

                    if (value === vm.testResult.OriginalResultChecked()) {

                        vm.testResult.ResultTypeId(testResultServerModel.OriginalResultTypeId());
                        vm.testResult.ProcessedDate(testResultServerModel.OriginalProcessedDate());

                        vm.testResult.DueDate(testResultServerModel.OriginalDueDate());

                    } else {

                        if (value) {
                            vm.testResult.ProcessedDate(new Date());
                            vm.testResult.ResultTypeId(null);
                        } else {
                            vm.testResult.ProcessedDate(null);
                            vm.testResult.ResultTypeId(1);
                        }
                    }
                    vm.testResult.ResultChecked(value);
                }
            });

            vm.passed = ko.pureComputed({
                read: function () {
                    return vm.testResult.ResultTypeId &&
                        (vm.testResult.ResultTypeId() === enums.TestResultType.Passed);
                }
            });

            vm.failed = ko.pureComputed({
                read: function () {
                    if (vm.isloading) {
                        return false;
                    }
                    return vm.hasFailed;
                }
            });

            vm.processedDateOptions = {
                value: vm.testResult.FormattedProcessedDate,
                disable: vm.disableFields,
                resattr: {
                    placeholder: 'Naati.Resources.Test.resources.ProcessedDate'
                }
            };

            vm.testResult.ResultTypeId.extend({
                required: ko.pureComputed(function () {
                    return vm.testResult.ResultChecked();
                })
            });

            vm.testResult.FormattedProcessedDate.extend({
                required: ko.pureComputed(function () {
                    return vm.testResult.ResultChecked();
                })
            });

            vm.testResult.FormattedDueDate.extend({
                required: ko.pureComputed(function () {
                    return vm.testResult.TestResultId();
                })
            });

            vm.isRubric = function () {
                return vm.test() && vm.test().MarkingSchemaTypeId() === enums.MarkingSchemaType.Rubric;
            };

            vm.disableEditMarks = ko.computed(function () {
                return vm.isRubric() ? false : vm.disableFields() || !currentUser.hasPermissionSync(enums.SecNoun.TestResult, enums.SecVerb.Update);
            });

            vm.disableCalculateMarks = function (examiner) {
                return vm.disableFields() || !canCalculateExaminerRubricResult(examiner) || !currentUser.hasPermissionSync(enums.SecNoun.TestResult, enums.SecVerb.Update);
            }

            vm.examiners.subscribe(selectResults);

            vm.dirtyFlag = new ko.DirtyFlag([
                vm.selectedResults,
                vm.testResult,
                vm.includePreviousMarks,
                vm.allowIssue
            ], false);

            var validation = ko.validatedObservable(vm.testResult);

            vm.isValid = ko.observable(true);

            vm.isSelectableExaminer = function (jobExminerId) {

                if (!currentUser.hasPermissionSync(enums.SecNoun.TestResult, enums.SecVerb.Update)) {
                    return false;
                }

                if (vm.isRubric()) {
                    return !vm.disableFields();
                }
                var componentResults = ((vm.getExaminerMarking(jobExminerId) || {}).TestComponentResults || []).length;
                return !vm.disableFields() && componentResults > 0;
            }

            vm.showFailOptions = function () {
                return vm.testResult.ResultTypeId() === enums.TestResultType.Failed;
            }

            vm.showSupplementaryCheck = function () {
                return vm.test() && vm.test().AllowSupplementary();
            }

            vm.showEligibleForConcededPassCheck = function () {
                return vm.test() && vm.test().HasDowngradePaths();
            }

            vm.disableEligibleForSupplementary = function () {
                return vm.disableFields() || vm.test().SupplementaryCredentialRequest();
            }

            vm.disableEligibleForConcededPass = function () {
                return vm.disableFields();
            }

            vm.disableComputeFinalRubric = ko.computed(function () {
                return vm.disableFields();
            });

            vm.showComputedFinalResult = ko.computed(function () {
                return vm.isRubric() && vm.ResultAutoCalculation() && vm.test() && !vm.test().Supplementary() && !vm.isReview();;
            });

            vm.showComputedResult = ko.computed(function () {
                return vm.isRubric() && vm.ResultAutoCalculation() && vm.test() && !vm.test().Supplementary();
            });


            return vm;

            function saveMarksOverridden() {
                updateTestResult().then(function () {
                    if (vm.testResult.AllowCalculate()) {
                        updateTestComponentResult();
                    }
                });
            }

            function reload() {
                eventDefer.notify({
                    name: 'reload',
                    data: {}
                });
            }

            function activate() {
                vm.canOverrideResults(currentUser.hasPermissionSync(enums.SecNoun.TestResult, enums.SecVerb.Override));
            }

            function setOptionDisable(resultTypeOption, item) {
                ko.applyBindingsToNode(resultTypeOption, {
                    disable: ko.pureComputed(function () {
                        var disableNotKnown = item.Id === enums.TestResultType.NotKnown && vm.ResultChecked;
                        var disableNotInvalidated = item.Id !== enums.TestResultType.Invalidated && (!vm.selectedResults() || !vm.selectedResults().length);
                        return disableNotKnown || disableNotInvalidated;
                    })
                }, item);
            }

            function updateTestComponentResult() {
                var jobExaminerIds = vm.selectedResults();
                var testResultId = vm.testResult.TestResultId();

                var resultAdded = jobExaminerIds.length > numSelectedResults;
                numSelectedResults = jobExaminerIds.length;

                var promises = [];

                if (resultAdded) {
                    // get the result that was just added
                    var examinerResult = vm.examiners().find(function (x) {
                        return x.JobExaminerId == jobExaminerIds[jobExaminerIds.length - 1];
                    });
                    // auto compute the task results (if applicable)
                    if (examinerResult.RubricOptions() && examinerResult.RubricOptions().ResultAutoCalculation) {
                        promises.push(computeResults(examinerResult, true));
                    }
                }

                return Q.all(promises).then(function () {
                    var data = {
                        TestResultId: testResultId,
                        JobExaminersId: jobExaminerIds,
                        IncludePreviousMarks: vm.includePreviousMarks()
                    };
                    return examinerService.post(data, 'countmarks').then(reload);
                });
            }

            // TODO this is business logic and should be in a business service, not in the UI code
            function requireRubricThirdMarking() {
                if (!vm.isRubric()) {
                    return false;
                }
                var includedExaminers = ko.utils.arrayFilter(vm.examiners(),
                    function (examiner) {
                        return isIncludedExaminer(examiner);
                    });

                if (includedExaminers.length !== 2) {
                    return false;
                }

                var components = vm.specification.Components();

                for (var c = 0; c < components.length; c++) {
                    var na = "NotDefined";
                    var componentResult = na;
                    var component = components[c];
                    for (var e = 0; e < includedExaminers.length; e++) {
                        var examiner = includedExaminers[e];

                        var examinerComponent = getRubricComponent(component, examiner);

                        if (!examinerComponent) {
                            return false;
                        }

                        if (componentResult === na) {
                            componentResult = examinerComponent;
                        }

                        if (componentResult.WasAttempted !== examinerComponent.WasAttempted) {
                            return true;
                        }

                        if (componentResult.Successful !== examinerComponent.Successful) {
                            return true;
                        }
                    }
                }
                return false;
            }

            function requireThirdMarking() {

                if (requireRubricThirdMarking()) {
                    return true;
                }

                var generalResults = [false, false];

                var examiners = vm.examiners() || [];

                var totalExaminers = 0;
                examiners.forEach(function (examiner) {
                    var found = vm.selectedResults().find(function (item) { return item === examiner.JobExaminerId });
                    if (found) {
                        totalExaminers++;
                    }
                });

                if (totalExaminers !== 2) {
                    return false;
                }

                for (var k = 0; k < examiners.length; k++) {

                    var currentExaminer = examiners[k];
                    generalResults[k] = getSumMarksForExaminer(currentExaminer.JobExaminerId) >= vm.specification.OverallPassMark();

                    if (!generalResults[k]) {
                        continue;
                    }

                    if (!currentExaminer.ExaminerMarkings.length) {
                        generalResults[k] = false;
                        continue;
                    }

                    for (var i = 0; i < currentExaminer.ExaminerMarkings.length; i++) {
                        var examinerMarkings = currentExaminer.ExaminerMarkings[i];
                        var results = examinerMarkings.TestComponentResults;
                        for (var j = 0; j < results.length; j++) {
                            var m = getMark(results[j].ComponentNumber, currentExaminer.JobExaminerId);

                            if (!m || m.Mark === null || m.Mark === undefined) {
                                generalResults[k] = false;


                            } else {
                                generalResults[k] = generalResults[k] && (m.Mark >= m.PassMark);
                            }

                            if (!generalResults[k]) {
                                break;
                            }
                        }
                    }
                }

                return generalResults[0] !== generalResults[1];
            }

            function getSumMarks() {
                var result = 0;

                $.each(vm.specification.Components(), function (i, e) {
                    result += e.Mark();
                });

                return Math.round(result * 100) / 100;
            }

            function getSumMarksForExaminer(jobExaminerId) {
                var result = 0;

                $.each(vm.specification.Components(), function (i, e) {
                    var mark = getMark(e.ComponentNumber(), jobExaminerId);

                    if (!mark || mark.Mark === null || mark.Mark === undefined) {
                        return;
                    }

                    result += mark.Mark;
                });

                return Math.round(result * 100) / 100;
            }

            function willPass() {

                return vm.testResult.ResultTypeId &&
                    (vm.testResult.ResultTypeId === enums.TestResultType.Passed);
            }

            function willFail() {
                return vm.testResult.ResultTypeId && vm.testResult.ResultTypeId() === enums.TestResultType.Failed;
            }

            function totalMarks() {
                if (vm.specification.Components().length === 0) {
                    return 0;
                }

                var total = 0;

                $.each(vm.specification.Components(), function (i, d) {
                    total += d.TotalMarks();
                });

                return total;
            }

            function getPrimaryReasonForFailure(e) {
                var mark = getExaminerMarking(e.JobExaminerId);
                if (!mark || !mark.PrimaryReasonForFailure) {
                    return null;
                }

                var id = mark.PrimaryReasonForFailure;

                if (id === 1) {
                    return 'Lack of proficiency in English';
                }
                else if (id === 2) {
                    return 'Lack of proficiency in LOTE';
                }
                else if (id === 3) {
                    return 'Lack of transacting skills';
                }

                return '';
            }

            function getExaminerMarking(jobExaminerId) {
                var grepped = $.grep(vm.examiners(), function (e) {
                    return e.JobExaminerId === jobExaminerId;
                });

                if (grepped.length > 0 && grepped[0].ExaminerMarkings.length > 0) {
                    return grepped[0].ExaminerMarkings[0];
                }

                return {};
            }

            function getComponentResult(component) {
                var grepped = $.grep(vm.specification.Components(), function (e) {
                    return e.ComponentNumber() === component.ComponentNumber();
                });

                if (grepped.length > 0) {
                    return grepped[0];
                }

                return {};
            }

            function getMark(componentNumber, jobExaminerId) {
                var examinerMarking = getExaminerMarking(jobExaminerId) || {};
                var grepped = $.grep(examinerMarking.TestComponentResults || [], function (e) {
                    return e.ComponentNumber === componentNumber;
                });

                if (grepped.length > 0) {
                    return grepped[0];
                }

                return {};
            }

            var defaultResult = null;
            function getResult(component, examiner) {
                return ko.pureComputed(function () {
                    var rubricComponent = getRubricComponent(component, examiner);

                    if (rubricComponent === defaultResult) {
                        return rubricComponent;
                    }

                    var minCommentsLength = getMinExaminerCommentsLength(examiner);
                    return getRubricResult(rubricComponent, minCommentsLength);
                });
            }

            function getRubricComponent(component, examiner) {
                var rubricComponent = getRubric(component, examiner);

                if (!rubricComponent || (vm.test().Supplementary() && rubricComponent.MarkingResultTypeId === enums.MarkingResultType.FromOriginal)) {
                    return defaultResult;
                }

                return rubricComponent;
            }

            function isIncludedExaminer(examiner) {
                if (examiner.ExaminerMarkings.length > 0) {
                    if (examiner.ExaminerMarkings[0].CountMarks) {
                        return true;
                    }
                }
                return false;
            }

            function examinerHasRubricMarks(examiner) {
                var rubricOptions = examiner.RubricOptions();
                if (!vm.isRubric() || !rubricOptions) {

                    return false;
                }

                for (var i = 0; i < rubricOptions.TestComponents.length; i++) {
                    var testComponent = rubricOptions.TestComponents[i];
                    var componentModel = ko.viewmodel.fromModel(testComponent);
                    if (getResult(componentModel, examiner)()) {
                        return true;
                    }
                }

                return false;
            };

            function canCalculateExaminerRubricResult(examiner) {
                return examiner.RubricOptions() &&
                    examiner.RubricOptions().ResultAutoCalculation &&
                    examinerHasRubricMarks(examiner) &&
                    isIncludedExaminer(examiner);
            }

            function examinerComputedEligibleForPass(examiner) {
                return canCalculateExaminerRubricResult(examiner) && examiner.RubricOptions().ComputedEligibleForPass;
            }

            function getExaminerComputedResultText(examiner) {

                if (canCalculateExaminerRubricResult(examiner)) {

                    if (examinerComputedEligibleForPass(examiner)) {
                        return ko.Localization('Naati.Resources.Test.resources.Pass');
                    }
                    return ko.Localization('Naati.Resources.Test.resources.Fail');;
                }
                return '';
            }

            function getRubricResult(rubricComponent, minCommentsLength) {
                var bandResource = ko.Localization('Naati.Resources.Test.resources.Band');
                var result = '{0}';

                if (!rubricComponent) {
                    return result.format(defaultResult);
                }

                if (rubricComponent.WasAttempted === false) {
                    return result.format(ko.Localization('Naati.Resources.Test.resources.NotAttempted'));
                }

                if (rubricComponent.WasAttempted === null) {
                    return result.format(defaultResult);
                }

                if (!rubricComponent.Competencies) {
                    return result.format(defaultResult);
                }

                var bands = '';
                ko.utils.arrayMap(rubricComponent.Competencies, function (c) {
                    ko.utils.arrayMap(c.Assessments, function (a) {
                        if (a.SelectedBand) {
                            var band = ko.utils.arrayFilter(a.Bands,
                                function (item) { return item.Id === a.SelectedBand })[0];

                            bands += '<li>{0}: {1} {2} {3}</li>'.format(
                                a.Label,
                                bandResource,
                                band.Level,
                                a.Comment && a.Comment.length >= minCommentsLength ? '<span class="fa fa-comment"></span>' : '');
                        }
                    });
                });

                var isSuccessful = rubricComponent.Successful === true;
                var isUnsuccessful = rubricComponent.Successful === false;

                if (bands) {
                    bands = '<ul class="list-unstyled">{0}</ul><div class="{1}">{2} {3}</div>'.format(
                        bands,
                        isSuccessful ? 'text-success' : (isUnsuccessful ? 'text-danger' : null),
                        isSuccessful ? '<span class="fa fa-check"></span>' : (isUnsuccessful ? '<span class="fa fa-times"></span>' : null),
                        isSuccessful ? ko.Localization('Naati.Resources.Shared.resources.Successful') : (isUnsuccessful ? ko.Localization('Naati.Resources.Shared.resources.Unsuccessful') : null));

                    return result.format(bands);
                }

                return result.format(defaultResult);
            }

            function getRubric(component, examiner) {
                var rubricOptions = examiner.RubricOptions();
                if (!rubricOptions || !rubricOptions.TestComponents) {
                    return defaultResult;
                }

                var rubricComponent = ko.utils.arrayFirst(rubricOptions.TestComponents, function (c) {
                    return c.Id === component.Id();
                });

                return rubricComponent;
            }

            function getMinExaminerCommentsLength(examiner) {
                var rubricOptions = examiner.RubricOptions();
                if (!rubricOptions || !rubricOptions.MinCommentsLength) {
                    return 0;
                }

                return rubricOptions.MinCommentsLength;
            }

            function validate() {
                if (!isDirty()) {
                    return true;
                }
                var isValid = true;

                if (!(isValid &= validation.isValid())) {
                    validation.errors.showAllMessages();
                }

                vm.isValid(isValid ? true : false);

                return vm.isValid();
            }

            function clearValidation() {
                vm.isValid(true);
                validation.errors.showAllMessages(false);
            }

            function updateTestResult() {
                var defer = Q.defer();
                var testResult = ko.toJS(vm.testResult);
                testResult.ProcessedDate = dateService.toPostDate(testResult.ProcessedDate);
                testresultService.post(testResult, 'validate').then(function (data) {
                    if (data.length) {
                        vm.confirmationInstance.show(testResult, data).then(function () {
                            defer.resolve(true);
                        });

                    } else {
                        testresultService.post(testResult, 'update')
                            .then(function () { defer.resolve(true) });
                    }
                });


                return defer.promise;

            }

            function save() {
                var defer = Q.defer();
                var promise = defer.promise;

                if (!isDirty()) {
                    defer.resolve('Leave');
                    return promise;
                }

                if (!validate()) {
                    defer.resolve('Invalid');
                    return promise;
                }

                var testResultId = vm.testResult.TestResultId;
                if (!testResultId) {
                    defer.resolve('Invalid');
                    return promise;
                }

                return updateTestResult().then(defer.resolve(true));
            }

            function reset() {
                vm.marksTabId(util.guid());

                if (vm.selectedResultsSubscriber) {
                    vm.selectedResultsSubscriber.dispose();
                }

                vm.selectedResults([]);
            }

            function loadComponents() {
                testresultService.getFluid('specification/' + vm.testResult.TestResultId()).then(function (response) {
                    var components = ko.viewmodel.fromModel(response.Components);

                    ko.utils.arrayForEach(components(), function (component) {
                        component.ComponentInOriginal = ko.pureComputed(function () {
                            if (vm.originalResultInstance()) {
                                return ko.utils.arrayFirst(vm.originalResultInstance().specification.Components(), function (c) {
                                    return c.Id() === component.Id();
                                });
                            }

                            return null;
                        });

                        component.IsInOriginal = ko.pureComputed(function () {
                            return component.ComponentInOriginal() != null && vm.test().Supplementary();
                        });

                        component.OriginalResult = ko.pureComputed(function () {
                            if (component.IsInOriginal()) {
                                return getComponentResult(component.ComponentInOriginal()).Mark;
                            }
                            return null;
                        });

                        component.RubricOptions = ko.observable();
                        component.MinCommentsLength = ko.observable(0);
                        component.RubricResult = ko.pureComputed(function () {
                            return getRubricResult(component.RubricOptions(), component.MinCommentsLength());
                        });
                        component.isReadOnly = ko.pureComputed(function () {
                            return component.RubricOptions() && component.RubricOptions().ReadOnly;
                        });
                        component.isSupplementary = ko.pureComputed(function () {
                            return vm.test() && vm.test().Supplementary() && component.RubricOptions() && !component.RubricOptions().ReadOnly;
                        });
                    });

                    vm.specification.Components(components());
                    vm.specification.OverallPassMark(response.OverallPassMark);

                    if (vm.isRubric()) {
                        testService.getFluid('rubricfinal/' + vm.testResult.TestResultId()).then(function (data) {
                            var testComponents = data.TestComponents;
                            vm.ResultAutoCalculation(data.ResultAutoCalculation);
                            vm.ComputedEligibleForConcededPass(data.ComputedEligibleForConcededPass);
                            vm.ComputedEligibleForPass(data.ComputedEligibleForPass);
                            vm.ComputedEligibleForSupplementary(data.ComputedEligibleForSupplementary);
                            ko.utils.arrayForEach(vm.specification.Components(), function (c) {
                                var component = ko.utils.arrayFirst(testComponents, function (tc) {
                                    return tc.Id === c.Id();
                                });

                                if (!component) {
                                    return;
                                }

                                c.RubricOptions(component);
                                c.MinCommentsLength(data.MinCommentsLength);
                            });
                        });
                    }
                });
            }

            function loadTestResult(data) {

                ko.viewmodel.updateFromModel(vm.testResult, data, true).onComplete(function () {
                    loadComponents();
                    loadResultConfigurations();
                    resetDirtyFlag();
                    vm.isLoading = false;
                });

                return;
            }

            function selectResults() {
                var validItems = [];
                vm.examiners().forEach(function (item) {
                    if (item.ExaminerMarkings.length > 0) {
                        if (item.ExaminerMarkings[0].CountMarks) {
                            validItems.push(item.JobExaminerId);
                        }
                    }
                });
                vm.selectedResults(validItems);
                vm.selectedResultsSubscriber = vm.selectedResults.subscribe(updateTestComponentResult);
            }

            function loadResultConfigurations() {

                vm.testResult.OriginalResultChecked(vm.testResult.ResultChecked());

                vm.testResult.OriginalResultTypeId(vm.testResult.ResultTypeId());
                vm.testResult.OriginalProcessedDate(vm.testResult.ProcessedDate());
                vm.testResult.OriginalDueDate(vm.testResult.DueDate());
            }

            function load(test, testResultId, testExaminers, originalResultInstance) {
                vm.isLoading = true;
                reset();
                vm.originalResultInstance(originalResultInstance);
                var testResultExaminers = ko.utils.arrayFilter(testExaminers(), function (item) {
                    return item.TestResultId === testResultId;
                });

                vm.test(test);
                testresultService.getFluid('testResult/' + testResultId).then(loadTestResult);

                var isRubric = vm.isRubric();
                ko.utils.arrayForEach(testResultExaminers, function (e) {
                    e.RubricOptions = ko.observable();

                    if (isRubric) {
                        testService.getFluid('rubricmarks/' + e.JobExaminerId).then(function (data) {
                            e.RubricOptions(data);
                        });
                    }
                });

                vm.examiners(testResultExaminers);

                numSelectedResults = vm.selectedResults().length;

                functions.getLookup('TestResultType').then(function (data) {
                    var filtered = [];

                    if (vm.test().ApplicationType().toLowerCase().includes('practice')) {
                        filtered = data.filter(function (item) {
                            // We want to remove the failed and passed options if the application is a practice test
                            if (item.DisplayName == 'Failed' || item.DisplayName == 'Passed') {
                                return false;
                            }

                            return true;
                        });

                        vm.resultTypeIdOptions(filtered);
                    }
                    else {
                        filtered = data.filter(function (item) {
                            return item.DisplayName != 'Issue Practice Test Results';
                        });
                        vm.resultTypeIdOptions(filtered);
                    }
                });
            }
            function formatDate(date) {
                return dateService.toUIDate(date);
            }

            function resetDirtyFlag() {
                vm.dirtyFlag().reset();
            }

            function isDirty() {
                return vm.dirtyFlag().isDirty();
            }

            function computeResults(jobExaminer, silent) {
                return testService.getFluid('computeRubricResults/' + jobExaminer.JobExaminerId)
                    .then(function () {
                        testService.getFluid('rubricmarks/' + jobExaminer.JobExaminerId)
                            .then(function (data) {
                                jobExaminer.RubricOptions(data);
                                if (!silent) {
                                    toastr.success("Results updated");
                                }
                            });
                    });
            }

            function computeFinalRubric() {
                var examiners = vm.examiners();
                var promises = [];
                for (var i = 0; i < examiners.length; i++) {

                    var examiner = examiners[i];
                    if (examinerHasRubricMarks(examiner)) {
                        promises.push(computeResults(examiner, true));
                    }
                }

                Q.all(promises).then(function () {
                    return testService.getFluid('computeFinalRubric/' + vm.testResult.TestResultId())
                        .then(function (data) {
                            var request = ko.toJS(data);
                            testService.post(request, 'rubricfinal').then(function () {
                                testresultService.getFluid('testResult/' + vm.testResult.TestResultId())
                                    .then(loadTestResult);
                                toastr.success("Results updated");
                            });
                        });
                });
            }
        }
    };
});
