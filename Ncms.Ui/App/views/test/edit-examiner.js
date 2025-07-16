define([
        'services/examiner-data-service',
        'services/test-data-service',
        'services/payroll-data-service',
        'services/screen/message-service',
        'services/screen/date-service',
        'modules/enums'
],
function (examinerService, testService, payrollService, message, dateService, enums) {
    return {
        getInstance: getInstance
    };

    function getInstance() {
        var defer = null;
        var eventDefer = Q.defer();
        var chairSuffix = ko.Localization('Naati.Resources.Roles.resources.Chair');

        var vm = {
            activate: activate,
            compositionComplete: compositionComplete,

            examinersOptions: ko.observableArray([]),
            //examinerTypesOptions: ko.pureComputed({ read: getExaminerTypeOptions }),
            //examinerTypesOptionsAfterRender: function(option, item) {
            //    ko.applyBindingsToNode(option, { disable: item.disable }, item);
            //},
            productSpecificationsOptions: ko.observableArray(),
            examiner: ko.observable({}),
            // examinerPaperLost: ko.observable(false),
            //examinerProductSpecificationId: ko.observable(),
            initialProductSpecificationId: null,
            initialExaminerSentDate:null,
            initialExaminerReceivedDate: null,
            initialExaminerToPayrollDate: null,
            save: save,
            cancel: cancel,
            //jobID: ko.observable(),
            //currentTest: ko.observable(),
            test: ko.observable({}),
            jobExaminerId: ko.observable(),
            //testAttendanceList: ko.observableArray([]),
            showEdit: showEdit,
            //languageId: ko.observable()
        };
        vm.CurrentExaminerPaperLost = ko.pureComputed({
            read: function () {
                return vm.currentExaminer.ExaminerPaperLost();
            },
       
            write: function(value) {
                vm.currentExaminer.ExaminerPaperLost(value);
            }
        });

        vm.test.subscribe(loadExaminerOptions);
        vm.examiner.subscribe(editExaminer);
        vm.currentExaminer = getExaminer();

        vm.currentExaminer.ExaminerCost.extend({ required: true });
        vm.currentExaminer.ExaminerType.extend({ required: true });
        vm.currentExaminer.ProductSpecificationId.extend({ required: true });

        //vm.examinerPaperLost.subscribe(function (newValue) {
        //    vm.currentExaminer.ExaminerPaperLost(newValue);
        //});

        ko.computed(function () {
            if (vm.examiner.ProductSpecificationId) {
                vm.currentExaminer.ProductSpecificationId(vm.examiner.ProductSpecificationId());

                var specs = $.grep(vm.productSpecificationsOptions(),
                    function (s) {
                        return s.Id === vm.examiner.ProductSpecificationId();
                    });

                if (specs.length) {
                    vm.currentExaminer.ExaminerCost(specs[0].CostPerUnit);
                }
            }
        });

        vm.canChangeCode = ko.computed(
            function () {
                var result = true;
                if (vm.currentExaminer.PayRollStatusId()) {
                    var status = vm.currentExaminer.PayRollStatusId();
           
                    result = status === enums.PayrollStatusType.NotReceived || status === enums.PayrollStatusType.Received;

                    if (!result && status !== enums.PayrollStatusType.Complete) {
                        result = vm.isFinalized();
                    }
                }

                return result;
            });

        vm.isFinalized = function () {
            var testStatus = vm.test().TestStatusTypeId ? vm.test().TestStatusTypeId() : '';
            return testStatus === enums.TestStatusType.Finalised;
        };

        var validation = ko.validatedObservable(vm.currentExaminer);
        vm.dirtyFlag = new ko.DirtyFlag([vm.currentExaminer], false);

        var canClose = false;

        return vm;

        function activate() {
            loadProductSpecifications();
        }

        function compositionComplete() {
            $('#editExaminerModal').on('hide.bs.modal', function (e) {
                tryClose(e);
            });

            $(window).on('editExaminerCancel',
                function () {
                    close();
                });
        }

        function tryClose(event) {
            if (event.target.id !== 'editExaminerModal') {
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

        function close() {
            canClose = true;
            $('#editExaminerModal').modal('hide');
            canClose = false;
        }

        //function getExaminerTypeOptions() {
        //    if (!vm.test().TestStatusTypeId) return [];

        //    var testStatus = vm.test().TestStatusTypeId();
        //    var allowPaidReviewer = testStatus === enums.TestStatusType.Finalised ||
        //        testStatus === enums.TestStatusType.UnderPaidReview;
        //    var options = [];
        //    for (var i = 0; i < enums.ExaminerTypes.list.length; i++) {
        //        var e = enums.ExaminerTypes.list[i];
        //        var disable = (allowPaidReviewer && e !== enums.ExaminerTypes.PaidReviewer) ||
        //            (e === enums.ExaminerTypes.PaidReviewer && !allowPaidReviewer);
        //        options.push({
        //            Type: e,
        //            Name: ko.Localization('Naati.Resources.Test.resources.ExaminerType' + e),
        //            disable: disable
        //        });
        //    }

        //    return options;
        //}

        function getExaminer() {
            var examiner = {
                JobExaminerId: ko.observable(),
                Examiner : {
                    PersonName: ko.observable(),
                },
                PayRollStatusId: ko.observable(),
                ExaminerCost: ko.observable(),
                DueDate: ko.observable(),
                ThirdExaminer: ko.observable(false),
                PaidReviewer: ko.observable(false),
                ExaminerPaperLost: ko.observable(false),
                ExaminerToPayrollDate: ko.observable(),
                ExaminerSentDate: ko.observable(),
                ExaminerPaperReceivedDate: ko.observable(),
                ExaminerType: ko.observable(),
                ProductSpecificationId: ko.observable(),
                PayRollStatus: ko.observable(),
                ProductSpecificationChangedDate: ko.observable(), // round trip
                ProductSpecificationChangedUserId: ko.observable(), // round trip
                ExaminerReceivedDate: ko.observable()
             
            };
            
            examiner.ExaminerTypeDisplayText = ko.pureComputed({
                read: function () {
                  
                    if (!vm.currentExaminer || !vm.currentExaminer.ExaminerType) {
                        return '';
                    }

                   return ko.Localization('Naati.Resources.Test.resources.ExaminerType' + vm.currentExaminer.ExaminerType());
                },
                write: function() {}
            });

            examiner.DisplayName = ko.computed({
                read: function () {

                    var ex = $.grep(vm.examinersOptions(),
                        function (v) {
                            return v.JobExaminerId === vm.currentExaminer.JobExaminerId();
                        });
                    if (ex.length) {
                        return ex[0].PersonName;
                    }

                    if (!vm.currentExaminer)
                        return null;

                        return vm.currentExaminer.Examiner.PersonName();
                    },
                    // just to prevent a updatemodel error
                    write: function () { }
                });

            //examiner.ExaminerType.subscribe(function (newValue) {
            //    examiner.PaidReviewer(newValue === enums.ExaminerTypes.PaidReviewer);
            //    examiner.ThirdExaminer(newValue === enums.ExaminerTypes.ThirdExaminer);
            //});

            //examiner.ThirdExaminer.subscribe(function (newValue) {
            //    if (newValue) {
            //        examiner.ExaminerType(enums.ExaminerTypes.ThirdExaminer);
            //    }
            //});

            //examiner.PaidReviewer.subscribe(function (newValue) {
            //    if (newValue) {
            //        examiner.ExaminerType(enums.ExaminerTypes.PaidReviewer);
            //    }
            //});

            examiner.ExaminerToPayrollDate.extend({
                dateGreaterThan: {
                    params: examiner.ExaminerSentDate,
                    message: ko.Localization('Naati.Resources.Shared.resources.PayrollDateGreaterThanSentDate')
                }
            });

            var testStatus = vm.test().TestStatusTypeId ? vm.test().TestStatusTypeId() : 0;
            var allowPaidReviewer = testStatus === enums.TestStatusType.Finalised ||
                testStatus === enums.TestStatusType.UnderPaidReview;
            if (allowPaidReviewer) {
                examiner.PaidReviewer(true);
            }

            return examiner;
        }


        function save() {
            var editingDefer = Q.defer();
            var promise = editingDefer.promise;

            if (!vm.dirtyFlag().isDirty()) {
                editingDefer.resolve();
                return promise;
            }

            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                editingDefer.resolve('Invalid');
                return null;
            }

            var request = {
                Examiners: [ko.toJS(vm.currentExaminer)],
                TestAttendanceId: vm.test().TestAttendanceId(),
                JobId: vm.examiner().JobId
            };
            request.TestAttendanceId = vm.test().TestAttendanceId();
            
            request.SkillId = vm.test().SkillId();
            request.CredentialTypeId = vm.test().CredentialTypeId();
            request.Examiners[0].DueDate = dateService.toPostDate(request.Examiners[0].DueDate);;
            request.DueDate = request.Examiners[0].DueDate;

            for (var i = 0; i < request.Examiners.length; i++) {
                request.Examiners[i].ExaminerSentDateChanged = request.Examiners[i].ExaminerSentDate !== vm.initialExaminerSentDate;
                request.Examiners[i].ExaminerReceivedDateChanged = request.Examiners[i].ExaminerReceivedDate !== vm.initialExaminerReceivedDate;
                request.Examiners[i].ExaminerToPayrollDateChanged = request.Examiners[i].ExaminerToPayrollDate !== vm.initialExaminerToPayrollDate;
                request.Examiners[i].ExaminerSentDate = dateService.toPostDate(request.Examiners[i].ExaminerSentDate);
                request.Examiners[i].ExaminerReceivedDate = dateService.toPostDate(request.Examiners[i].ExaminerReceivedDate);
                request.Examiners[i].ExaminerPaperReceivedDate = dateService.toPostDate(request.Examiners[i].ExaminerPaperReceivedDate);
                request.Examiners[i].ExaminerToPayrollDate = dateService.toPostDate(request.Examiners[i].ExaminerToPayrollDate);
                request.Examiners[i].ProductSpecificationChanged = request.Examiners[i].ProductSpecificationId !== vm.initialProductSpecificationId;
               
            }

            testService.post(request, 'examiner/update').then(function (response) {
                processPostExaminer(request, response);
                defer.resolve(request);
                editingDefer.resolve(request);
                close();
            });

            return promise;
        }


        function getNewJobs(response, request) {
            return $.grep(response.JobIds, function (e) {
                return request.JobId !== e;
            });
        }

        function processPostExaminer(request, response) {

            var newJobs = getNewJobs(response, request);
            var paidReviewer = request.Examiners.find(function (element) { return element.PaidReviewer });
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

                var names = $.map(request.Examiners, function (e) { return e.DisplayName; });
                var messageKey = 'Naati.Resources.Test.resources.ExaminersAddedSuccess';
                var messageContent = ko.Localization(messageKey);

                messageContent = messageContent.format(
                    names.join(', '),
                    request.TestAttendanceId
                );

                message.alert({
                    title: ko.Localization('Naati.Resources.Shared.resources.UpdatedSuccessfully'),
                    content: messageContent
                });

            } else {
                toastr.error(ko.Localization('Naati.Resources.Test.resources.ExaminerAddFail')
                    .replace('{0}', response.Errors.length));
            }

            var json = ko.toJS(getExaminer());
            loadExaminer(json);
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

        function edit(e) {
            var examiner = $.extend({}, e);

            examiner.DueDate = dateService.toUIDate(examiner.DueDate);
            examiner.ExaminerSentDate = dateService.toUIDate(examiner.ExaminerSentDate);
            examiner.ExaminerPaperReceivedDate = dateService.toUIDate(examiner.ExaminerPaperReceivedDate);
            examiner.ExaminerToPayrollDate = dateService.toUIDate(examiner.ExaminerToPayrollDate);
            examiner.ExaminerReceivedDate = dateService.toUIDate(examiner.ExaminerReceivedDate);

            return loadExaminer(examiner);
        }

        function loadExaminer(examiner) {
            validation.errors.showAllMessages(false);
            ko.viewmodel.updateFromModel(vm.currentExaminer, examiner, true).onComplete(function () {
                //if (!examiner.PaidReviewer && !examiner.ThirdExaminer) {
                //    vm.currentExaminer.ExaminerType(enums.ExaminerTypes.Original);
                //}
                vm.currentExaminer.ExaminerType(examiner.ExaminerTypeName);
                vm.currentExaminer.ExaminerPaperLost(examiner.ExaminerPaperLost);
                vm.currentExaminer.ProductSpecificationId(examiner.ProductSpecificationId);
                vm.initialProductSpecificationId = examiner.ProductSpecificationId;
                vm.initialExaminerSentDate= examiner.ExaminerSentDate,
                vm.initialExaminerReceivedDate = examiner.ExaminerReceivedDate,
                vm.initialExaminerToPayrollDate = examiner.ExaminerToPayrollDate,
                vm.currentExaminer.ExaminerCost(examiner.ExaminerCost);
                validation.errors.showAllMessages(false);
                vm.dirtyFlag().reset();
            });
        }

        function loadExaminerOptions() {
            examinerService.getFluid(vm.test().Language1Id() + '/' + vm.test().Language2Id() + '/' + vm.test().CredentialTypeId() + '/' + vm.test().TestAttendanceId()+'/byLanguageAndCredentialType').then(
                function (data) {
                    var items = [];
                    ko.utils.arrayForEach(data,
                        function (item) {
                            item.DisplayName = item.IsChair ? item.PersonName += chairSuffix : item.PersonName;
                            items.push(item);
                        });
                    vm.examinersOptions(data);
                });
        }
    
        function editExaminer()
        {
            edit(vm.examiner());
            $('#editExaminerModal').modal('show');
            
        }

        function showEdit(options) {
            defer = Q.defer();

            vm.test(options.test);
            vm.jobExaminerId(options.jobExaminerId);
            examinerService.getFluid('examiner/' + vm.jobExaminerId() +'/false').then(function (data) {
                vm.examiner(data);
            });
           // vm.jobID(jobId);
           // vm.testAttendanceList([testId]);
           // vm.languageId(languageId);
            return defer.promise;
        }

    

        function loadProductSpecifications() {
            payrollService.getFluid('productsForMarkingClaims').then(function(data) {
                var activeProductsForMarkingClaims = data.filter(function(item) { return !item.Inactive });
                vm.productSpecificationsOptions(activeProductsForMarkingClaims);
            });
        }
    }
});
