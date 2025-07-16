define([
        'services/screen/message-service',
        'services/test-data-service',
        'services/examiner-data-service',
        'services/language-data-service',
        'services/payroll-data-service',
        'services/screen/date-service',
        'services/util',
        'modules/enums'
    ],
    function (message,
        testService,
        examinerService,
        languageService,
        payrollService,
        dateService,
        util,
        enums) {

        return {
            getInstance: getInstance
        };

        function getInstance() {
            var defer = null;
            var eventDefer = Q.defer();
            var chairSuffix = ' (' + ko.Localization('Naati.Resources.Roles.resources.Chair') + ')';

            var vm = {
                activate: activate,
                compositionComplete: compositionComplete,
                test: ko.observable({}),
                dueDate: ko.observable(),
                testAttendanceList: ko.observableArray([]),
                showAdd: showAdd,
                close: close,
                save: save,
                event: eventDefer.promise,
                examinersOptionsFull: ko.observableArray([]),
                examinerTypesOptionsAfterRender: function(option, item) {
                    ko.applyBindingsToNode(option, { disable: item.disable }, item);
                },
                productSpecificationsOptions: ko.observableArray(),
                getExaminerType: getExaminerType,
                // jobList: ko.observableArray(),
                // languageId: ko.observable(),
                // language: ko.observable(),
                //additionalDescription: ko.observable(),
                //englishLanguageId: null,
                examiners: ko.observableArray([]),
                inEditingMode: inEditingMode,
                cancel: cancel,
                pushExaminer: pushExaminer,
                examinersToSave: ko.observableArray([]),
                removeExaminer: removeExaminer,
                datatable: {
                    paging: false,
                    searching: false,
                    info: false
                },
                modalId: util.guid(),
                examinerProductSpecificationId: ko.observable()
        };

            vm.addMode = ko.pureComputed(function() {
                return !vm.test().CurrentJobId || !vm.test().CurrentJobId();
            });

            vm.test.subscribe(loadExaminerOptions);
            vm.test.subscribe(loadExaminers);

            function loadExaminerOptions() {
                examinerService.getFluid(vm.test().Language1Id() + '/' + vm.test().Language2Id() + '/' + vm.test().CredentialTypeId() + '/' + vm.test().TestAttendanceId()+'/byLanguageAndCredentialType').then(
                    function(data) {
                        var items = [];
                        ko.utils.arrayForEach(data,
                            function(item) {
                                item.DisplayName = item.IsChair ? item.PersonName += chairSuffix : item.PersonName;
                                items.push(item);
                            });

                        vm.examinersOptionsFull(items);
                    });
            }

            function loadExaminers() {

                examinerService.get({ request: JSON.stringify({ TestAttendanceIds: [vm.test().TestAttendanceId()] }) }).then(vm.examiners);
            }

            //languageService.getFluid('English').then(function (data) {
            //    vm.englishLanguageId = data.LanguageId;
            //});

            // var oldLanguageIdValue;
            //vm.languageId.subscribe(function (oldValue) {
            //   oldLanguageIdValue = oldValue;
            // }, null, 'beforeChange');

            //vm.languageId.subscribe(function (newValue) {
            //    if (oldLanguageIdValue !== newValue) {
            //        examinerService.get({ request: JSON.stringify({ LanguageId: [newValue, vm.englishLanguageId] }) })
            //            .then(function (data) {
            //                ko.utils.arrayForEach(data, function (membership) {
            //                    if (membership.IsChair) {
            //                        membership.Name += chairSuffix;
            //                    }
            //                });

            //                vm.examinersOptionsFull(data);
            //            });
            //    }
            //});
            
           

            //vm.currentTest = ko.pureComputed(function () {
            //    return vm.testAttendanceList().length ? vm.testAttendanceList()[0] : null;
            //});

            //vm.testStatusList = ko.pureComputed(function () {
            //    return ko.utils.arrayMap(vm.testAttendanceList(), function (testAttendance) {
            //        return testAttendance.testStatus();
            //    });
            //});

            //vm.distinctTestStatusList = ko.pureComputed(function () {
            //    return ko.utils.arrayGetDistinctValues(vm.testStatusList());
            //});

            //vm.testsShareSameStatus = ko.pureComputed(function () {
            //    return vm.distinctTestStatusList().length === 1;
            //});

            vm.allowPaidReviewer = ko.pureComputed(function () {
                return vm.test().TestStatusTypeId &&
                (vm.test().TestStatusTypeId() === enums.TestStatusType.UnderPaidReview);

            });

            vm.showExaminerType = ko.pureComputed(function () {
                return vm.examiners().length || vm.allowPaidReviewer();
            });

            //vm.testAttendanceIds = ko.pureComputed(function () {
            //    return ko.utils.arrayMap(vm.testAttendanceList(), function (test) {
            //        return test.testId();
            //    });
            //});

            //vm.testAttendanceIds.subscribe(function (value) {
            //    vm.examiners([]);

            //    if (!value.length) {
            //        return;
            //    }

            //    examinerService.get({ request: JSON.stringify({ TestAttendanceId: vm.testAttendanceIds(), ActiveExaminersOnly: false }) }).then(function (data) {
            //        vm.examiners(data);
            //    });
            //});

            vm.examinersOptions = ko.pureComputed({
                read: function () {
                    return $.grep(vm.examinersOptionsFull(), function (option) {
                        var inserted = $.grep(vm.examinersToSave(), function (toSave) {
                            return option.PanelMembershipId === toSave.PanelMembershipId;
                        });

                        return inserted.length === 0;
                    });
                }
            });



            ko.computed(function () {
                if (vm.examinerProductSpecificationId() != null && vm.examinerProductSpecificationId() != undefined) {

                    vm.currentExaminer.ProductSpecificationId(vm.examinerProductSpecificationId());

                    var spec = getProductSpecification(vm.examinerProductSpecificationId());
                    if (spec) {
                        vm.currentExaminer.ExaminerCost(spec.CostPerUnit);
                    } else {
                        vm.currentExaminer.ProductSpecificationId(null);
                        vm.currentExaminer.ExaminerCost(null);
                    }
                }
            });

            vm.examinerTypesOptions = ko.pureComputed(getExaminerTypeOptions);

            vm.currentExaminer = getExaminer();

            var required = {
                required: {
                    onlyIf: inEditingMode
                }
            };

            vm.currentExaminer.PanelMembershipId.extend(required);
            vm.currentExaminer.ExaminerCost.extend(required);
            vm.currentExaminer.ExaminerType.extend({
                required: {
                    onlyIf: function () {
                        return inEditingMode() && vm.showExaminerType();
                    }
                }
            });
            vm.currentExaminer.ProductSpecificationId.extend({ required: true });

            var validationDueDate = ko.validatedObservable(vm.dueDate);
            var validation = ko.validatedObservable(vm.currentExaminer);

            vm.isValid = ko.observable(true);
            vm.validate = validate;
            vm.clearValidation = clearValidation;

            vm.dirtyFlag = new ko.DirtyFlag([vm.examinersToSave], false);

            vm.canSave = ko.computed(function () {
                return vm.dirtyFlag().isDirty();
            });

            var canClose = false;

            return vm;

            function activate() {
                loadProductSpecifications();
            }

            function compositionComplete() {
                $('#' + vm.modalId).on('hide.bs.modal', function (e) {
                    tryClose(e);
                });

                $(window).on('examinerCancel', function () {
                    close();
                });
            }

            function tryClose(event) {
                if (event.target.id !== vm.modalId) {
                    return;
                }

                if (canClose) {
                    return;
                }

                event.preventDefault();
                event.stopImmediatePropagation();

                if (vm.dirtyFlag().isDirty()) {
                    message.confirm({
                            title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                            content: ko.Localization('Naati.Resources.Shared.resources.AreYouSureYouWantToCancel')
                        })
                        .then(function (answer) {
                            if (answer === 'yes') {
                                close();
                            }
                        });
                } else {
                    close();
                }
            }

            function showAdd(options) {
                defer = Q.defer();
                vm.testAttendanceList(options.testAttendanceList || []);
                vm.test(options.test);
                vm.dueDate(dateService.toUIDate(moment().add(14, 'days').format()));
                vm.examinerProductSpecificationId(null);
                var json = ko.toJS(getExaminer());
                loadExaminer(json);
                vm.examinersToSave([]);
                vm.dirtyFlag().reset();

                $('#' + vm.modalId).modal('show');

                return defer.promise;
            }

            function close() {
                vm.testAttendanceList([]);
                canClose = true;
                $('#' + vm.modalId).modal('hide');
                canClose = false;
            }

            function getExaminerTypeOptions() {
                var options = [];
                for (var i = 0; i < enums.ExaminerTypes.list.length; i++) {
                    var e = enums.ExaminerTypes.list[i];

                    options.push({
                        Type: e,
                        Name: ko.Localization('Naati.Resources.Test.resources.ExaminerType' + e),
                        disable: disableExaminerType(e)
                    });
                }

                return options;
            }

            function disableExaminerType(e) {
                
                if (!vm.test().TestStatusTypeId) {
                    return true;
                }
                var testStatus = vm.test().TestStatusTypeId();
                var isFinalised = testStatus === enums.TestStatusType.Finalised;
                var isUnderPaidReview = testStatus === enums.TestStatusType.UnderPaidReview;
                var isInProgress = testStatus === enums.TestStatusType.InProgress;
                var isSat = testStatus === enums.TestStatusType.Sat;
                var isUnderReview = testStatus === enums.TestStatusType.UnderReview;

                if (isFinalised) {
                    return true;
                }

                if (e === enums.ExaminerTypes.PaidReviewer) {
                    return !isUnderPaidReview;
                }
                else if(e === enums.ExaminerTypes.ThirdExaminer) {
                    return !isInProgress && !isUnderReview;
                }
                if (e === enums.ExaminerTypes.Original) {
                    return !isInProgress && !isSat;
                }

                return true;
            }

            function getExaminerType(e) {
                var examinerType = enums.ExaminerTypes.Original;

                if (e.ThirdExaminer) {
                    examinerType = enums.ExaminerTypes.ThirdExaminer;
                } else if (e.PaidReviewer) {
                    examinerType = enums.ExaminerTypes.PaidReviewer;
                }

                return ko.Localization('Naati.Resources.Test.resources.ExaminerType' + examinerType);
            }

            function getExaminer() {
                var examiner = {
                    PanelMembershipId: ko.observable(),
                    ExaminerCost: ko.observable(),
                    ThirdExaminer: ko.observable(false),
                    PaidReviewer: ko.observable(false),
                    ProductSpecificationId: ko.observable(null),
                    ProductSpecificationChanged: ko.observable(true),
                    ExaminerSentDateChanged: ko.observable(false),
                    ExaminerReceivedDateChanged: ko.observable(false),
                    ExaminerToPayrollDateChanged: ko.observable(false)
                };

                examiner.ExaminerType = ko.computed({
                    read: function () {
                        if (examiner.PaidReviewer()) {
                            return enums.ExaminerTypes.PaidReviewer;
                        }

                        if (examiner.ThirdExaminer()) {
                            return enums.ExaminerTypes.ThirdExaminer;
                        }

                        return enums.ExaminerTypes.Original;
                    },
                    write: function (newValue) {
                        examiner.PaidReviewer(newValue === enums.ExaminerTypes.PaidReviewer);
                        examiner.ThirdExaminer(newValue === enums.ExaminerTypes.ThirdExaminer);
                    }
                });

                if (!disableExaminerType(enums.ExaminerTypes.PaidReviewer)) {
                    examiner.PaidReviewer(true);
                }
                else if (disableExaminerType(enums.ExaminerTypes.Original) && vm.test()) {
                    examiner.ThirdExaminer(true);
                }

                return examiner;
            }

            function clearValidation() {
                vm.isValid(true);
                validation.errors.showAllMessages(false);
            }

            function validate() {
                vm.isValid(validation.isValid());

                if (!vm.isValid()) {
                    validation.errors.showAllMessages();
                }

                return vm.isValid();
            }

            function pushExaminer() {
                var saveRequest = prepareToSave();

                if (saveRequest.resolve) {
                    return;
                }

                var examiner = $.extend({}, saveRequest.examiner, getComposedExaminer(saveRequest.examiner.PanelMembershipId));
                var examiners = vm.examinersToSave();
                vm.examinersToSave([]);
                examiners.push(examiner);
                vm.examinersToSave(examiners);

                var json = ko.toJS(getExaminer());
                loadExaminer(json);
            }

            function removeExaminer(examiner) {
                message.confirm({
                        title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
                        content: ko.Localization('Naati.Resources.Shared.resources.ThisRecordWillBeDeleted')
                    })
                    .then(function (answer) {
                        if (answer === 'yes') {
                            var examiners = vm.examinersToSave();
                            vm.examinersToSave([]);
                            examiners = $.grep(examiners, function (value) {
                                return value !== examiner;
                            });

                            vm.examinersToSave(examiners);
                        }
                    });
            }

            function prepareToSave() {
                var result = {
                    resolve: false,
                    resolveArgument: null,
                    examiner: null
                };

                if (vm.addMode()) {
                    result.resolve = true;
                    return add();
                }

                if (!inEditingMode()) {
                    result.resolve = true;
                    result.resolveArgument = 'Invalid';
                    return result;
                }

                if (!validation.isValid()) {
                    validation.errors.showAllMessages();
                    result.resolve = true;
                    result.resolveArgument = 'Invalid';
                    return result;
                }

                result.examiner = ko.toJS(vm.currentExaminer);

                return result;
            }

            function getSaveRequest() {
                var request = {
                    Examiners: ko.toJS(vm.examinersToSave),
                    SkillId: vm.test().SkillId(),
                    CredentialTypeId: vm.test().CredentialTypeId(),
                    DueDate: dateService.toPostDate(vm.dueDate())
                };

                var testdata = [];
                var testAttedanceList = vm.testAttendanceList();

                if (testAttedanceList.length) {

                    for (var i =0; i< testAttedanceList.length; i++) {
                        var testItem = testAttedanceList[i];
                        testdata.push({
                            JobId: testItem.JobId,
                            TestAttendanceId: testItem.AttendanceId,
                        });
                    }
                } else {
                    testdata.push({
                        JobId: vm.test().CurrentJobId(),
                        TestAttendanceId: vm.test().TestAttendanceId(),
                    });
                }
                request.TestDataModels = testdata;
                return request;
            }

            function save() {

                var editingDefer = Q.defer();
                var promise = editingDefer.promise;

                if (!validationDueDate.isValid()) {
                    validationDueDate.errors.showAllMessages();
                    editingDefer.resolve('Invalid');

                    return promise;
                }

                var request = getSaveRequest();

                testService.post(request, 'examiner').then(function (response) {
                    processPostExaminer(request, response);

                    defer.resolve(request);
                    editingDefer.resolve(request);
                });

                return promise;
                
            }
            

            function add() {
                var result = {
                    resolve: false,
                    resolveArgument: null,
                    examiner: null
                };

                if (!validation.isValid()) {
                    validation.errors.showAllMessages();
                    result.resolve = true;
                    result.resolveArgument = 'Invalid';
                    return result;
                }

                if (!inEditingMode()) {
                    result.resolve = true;
                    result.resolveArgument = 'Invalid';
                    return result;
                }

                result.examiner = ko.toJS(vm.currentExaminer);

                return result;
            }

            function getNewJobs(response, request)
            {
                return $.grep(response.JobIds, function (e) {
                    return !request.TestDataModels.find(function(element) {
                        return element.JobId === e;
                    });
                });
            }

            function processPostExaminer(request, response) {
                var newJobs = getNewJobs(response, request);
                var paidReviewer = request.Examiners.find(function(element) { return element.PaidReviewer });
                if (paidReviewer && newJobs.length) {
                    eventDefer.notify({
                        name: 'NewReview',
                        data: response.JobId
                    });
                } else {
                    eventDefer.notify({
                        name: 'SaveExaminer',
                        data: {
                            request: request,
                            response: response
                        }
                    });
                }

                if (newJobs) {
                    
                    if (newJobs.length) {
                        toastr.success(ko.Localization('Naati.Resources.Test.resources.JobsCreated')
                            .replace('{0}', newJobs.join(',')));
                    }

                    var names = $.map(request.Examiners, function (e) { return e.PersonName; });
                    var messageKey = 'Naati.Resources.Test.resources.ExaminersAddedSuccess';
                    var messageContent = ko.Localization(messageKey);

                    messageContent = messageContent.format(
                        names.join(', '),
                        request.TestDataModels.map(function (x) { return x.TestAttendanceId; }).join(',')
                    );

                    message.alert({
                        title: ko.Localization('Naati.Resources.Shared.resources.AddedSuccessfully'),
                        content: messageContent
                    });

                } else {
                      toastr.error(ko.Localization('Naati.Resources.Test.resources.ExaminerAddFail')
                        .replace('{0}', response.Errors.length));
                }

                loadExaminer(ko.toJS(getExaminer()));
            }

            function getComposedExaminer(panelMembershipId) {
                var ex = $.grep(vm.examinersOptionsFull(), function (e) {
                    return e.PanelMembershipId === panelMembershipId;
                })[0];

                return $.extend({}, ex);
            }

            function cancel() {
                message.confirm({
                        title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                        content: ko.Localization('Naati.Resources.Shared.resources.AreYouSureYouWantToCancel')
                    })
                    .then(function (answer) {
                        if (answer === 'yes') {
                            var json = ko.toJS(getExaminer());
                            loadExaminer(json);

                            var evt = new CustomEvent('examinerCancel');
                            window.dispatchEvent(evt);
                        }
                    });
            }

            function inEditingMode() {
                return (vm.currentExaminer.PanelMembershipId() || 0) ||
                    parseFloat(vm.currentExaminer.ExaminerCost() || '0') !== 0;
            }

            function loadExaminer(examiner) {
                validation.errors.showAllMessages(false);
                ko.viewmodel.updateFromModel(vm.currentExaminer, examiner, true).onComplete(function () {
                    if (!examiner.PaidReviewer && !examiner.ThirdExaminer) {
                        vm.currentExaminer.ExaminerType(enums.ExaminerTypes.Original);
                    }
                    vm.examinerProductSpecificationId(null);

                    validation.errors.showAllMessages(false);
                });
            }

            function loadProductSpecifications() {
                payrollService.getFluid('productsForMarkingClaims').then(function (data) {
                    var activeProductsForMarkingClaims = data.filter(function (item) { return !item.Inactive });
                    vm.productSpecificationsOptions(activeProductsForMarkingClaims);
                });
            }

            function getProductSpecification(id) {
                var specs = $.grep(vm.productSpecificationsOptions(), function (s) {
                    return s.Id === id;
                });

                if (specs && specs.length === 1) {
                    return specs[0];
                }

                return null;
            }

        }
    });
