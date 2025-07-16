define([
    'services/test-data-service',
    'services/screen/message-service',
    'modules/enums'
],
    function (testService, message, enums) {

        var serverModel = {
            NaatiNumber: ko.observable(),
            ApplicationReference: ko.observable(),
            ApplicationId: ko.observable(),
            Id: ko.observable(),
            AttendanceId: ko.observable(),
            ApplicationType: ko.observable(),
            CredentialType: ko.observable(),
            Skill: ko.observable(),
            TestDate: ko.observable(),
            TestStatus: ko.observable(),
            TestStatusTypeId: ko.observable(),
            TestResultStatus: ko.observable(),
            TestLocation: ko.observable(),
            Venue: ko.observable(),
            Supplementary: ko.observable(),
            TestMaterialIds: ko.observableArray(),
            TestComponents: ko.observableArray(),
            TestResultStatusTypeId: ko.observable(),
            OriginalTestResultStatusTypeId: ko.observable(),
            ResultAutoCalculation: ko.observable(),
        };

        var emptyModel = ko.toJS(serverModel);

        serverModel.TestAttendanceId = ko.pureComputed(function () {
            return serverModel.AttendanceId();
        });

        serverModel.TestResultId = ko.pureComputed(function () {
            return serverModel.Id();
        });

        serverModel.CredentialTypeInternalName = ko.pureComputed(function () {
            return serverModel.CredentialType();
        });

        var vm = {
            result: serverModel,
            isReview: ko.observable(),
            closeLink: ko.computed(function () {
                return '#test/' + serverModel.AttendanceId() + '?tab=' + (serverModel.TestStatusTypeId() === enums.TestStatusType.UnderPaidReview ? 'paidReview' : 'results');
            }),
            disable: ko.computed(function () {
                return serverModel.OriginalTestResultStatusTypeId() !== enums.TestResultType.NotKnown;
            }),
            showMinCommentLength: ko.observable(true),
            computeFinalRubric: computeFinalRubric
        };

        var dirtyFlag = new ko.DirtyFlag([serverModel], false);

        vm.canSave = ko.pureComputed(function () {
            return dirtyFlag().isDirty();
        });

        var validation = ko.validatedObservable([]);
        vm.rubricOptions = {
            testComponents: serverModel.TestComponents,
            disable: vm.disable,
            showMinCommentLength: vm.showMinCommentLength,
            showResult: ko.pureComputed(
                function () {
                    return !serverModel.ResultAutoCalculation();
                })
        };

        vm.save = function () {
            var defer = Q.defer();

            if (!vm.rubricOptions.component.validate()) {
                validation.errors.showAllMessages();
                return defer.promise;
            }

            function proceed() {
                var request = ko.toJS(serverModel);

                testService.post(request, 'rubricfinal')
                    .then(function () {
                        dirtyFlag().reset();
                        defer.resolve('fulfilled');
                        vm.rubricOptions.component.clearChanges();
                        toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                    });
            }

            var changedAssessment = vm.rubricOptions.component.changedAssessment();
            if (changedAssessment.length) {
                var tasks = ko.utils.arrayMap(changedAssessment, function (c) {
                    return 'Task {0}{1} - {2}: {3} from {4} to {5}. '.format(
                        c.component.TypeLabel(),
                        c.component.Label(),
                        c.competence.Name(),
                        c.assessment.Name(),
                        c.oldBand.Label(),
                        c.newBand.Label()
                    );
                });

                var content = '<p>{0}</p><ul><li>{1}</li></ul>'.format(
                    ko.Localization('Naati.Resources.Test.resources.TasksResultChanged'),
                    tasks.join('</li><li>')
                );

                message.confirm({ content: content }).then(function (answer) {
                    if (answer != 'yes') {
                        return;
                    }

                    proceed();
                });
            }
            else {
                proceed();
            }

            return defer.promise;
        };

        vm.canActivate = function (id, query) {
            queryString = query || {};
            id = parseInt(id);

            ko.viewmodel.updateFromModel(serverModel, emptyModel);

            serverModel.Id(id);

            return loadRubric();
        };

        vm.disableComputeFinalRubric = ko.computed(function () {
            return vm.disable() || vm.isReview();
        });

        vm.showComputeFinalRubric = ko.computed(function () {
            return !vm.disable() && !vm.isReview() && !serverModel.Supplementary();
        });

        function resetValidation() {
            if (validation.errors) {
                return validation.errors.showAllMessages(false);
            }
        };

        function loadRubric() {
            return testService.getFluid('rubricfinal/' + serverModel.Id()).then(function (data) {
                ko.viewmodel.updateFromModel(serverModel, data);

                testService.getFluid(data.AttendanceId + '/summary').then(function (summary) {
                    vm.isReview(summary.LastReviewTestResultId);
                });


                var comments = [];
                ko.utils.arrayForEach(serverModel.TestComponents(), function (component) {
                    component.IsSupplementar = ko.observable(serverModel.Supplementary() && component.MarkingResultTypeId() === enums.MarkingResultType.Original);
                    ko.utils.arrayForEach(component.Competencies(), function (competence) {
                        ko.utils.arrayForEach(competence.Assessments(), function (assessment) {
                            assessment.Comment.extend({ maxLength: 4000 });
                            comments.push(assessment.Comment);
                        });
                    });
                });

                validation(comments);
                resetValidation();
                dirtyFlag().reset();

                return true;
            });
        }

        function computeFinalRubric() {
            return testService.getFluid('computeFinalRubric/' + serverModel.Id())
                .then(function (data) {
                    ko.viewmodel.updateFromModel(serverModel, data);
                });
        }

        return vm;
    });