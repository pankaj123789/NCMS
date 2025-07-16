define([
    'services/test-data-service',
    'services/screen/date-service',
    'services/screen/message-service',
    'modules/enums'
],
    function (testService, dateService, message, enums) {

        var serverModel = {
            Id: ko.observable(),
            ExaminerName: ko.observable(),
            ReceivedDate: ko.observable(),
            TestDate: ko.observable(),
            AttendanceId: ko.observable(),
            CredentialType: ko.observable(),
            Skill: ko.observable(),
            Supplementary: ko.observable(),
            TestComponents: ko.observableArray(),
			TestStatusTypeId: ko.observable(),
			TestResultStatusTypeId: ko.observable(),
            OriginalTestResultStatusTypeId: ko.observable(),
            ResultAutoCalculation: ko.observable(),
            Feedback: ko.observable()
        };

        var emptyModel = ko.toJS(serverModel);

        var vm = {
            result: serverModel,
            closeLink: ko.computed(function () {
                return '#test/' + serverModel.AttendanceId() + '?tab=' + (serverModel.TestStatusTypeId() === enums.TestStatusType.UnderPaidReview ? 'paidReview' : 'results');
			}),
			disable: ko.computed(function () {
				return serverModel.OriginalTestResultStatusTypeId() !== enums.TestResultType.NotKnown;
            }),
            showMinCommentLength: ko.observable(true),
        };

        var dirtyFlag = new ko.DirtyFlag([serverModel], false);

        vm.canSave = ko.pureComputed(function () {
            return dirtyFlag().isDirty();
        });

        vm.rubricOptions = {
            testComponents: serverModel.TestComponents,
            disable: vm.disable,
            showMinCommentLength: vm.showMinCommentLength,
            showResult: ko.pureComputed(
                function() {
                    return !serverModel.ResultAutoCalculation();
                })
        };

        vm.receivedDateDateOptions = {
            value: serverModel.ReceivedDate,
			disable: vm.disable,
            resattr: {
                placeholder: 'Naati.Resources.Test.resources.ReceivedDate'
            },
            css: 'form-control w-sm'
        };

        var validation = ko.validatedObservable([]);

        vm.save = function () {
            var defer = Q.defer();

            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return defer.promise;
            }

            function proceed() {
                var request = ko.toJS(serverModel);
                request.ReceivedDate = dateService.toPostDate(request.ReceivedDate);

                testService.post(request, 'rubricmarks')
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

        function resetValidation() {
            if (validation.errors) {
                return validation.errors.showAllMessages(false);
            }
        }

        function loadRubric() {
            return testService.getFluid('rubricmarks/' + serverModel.Id()).then(function (data) {
                if (data.ReceivedDate) {
                    data.ReceivedDate = moment(data.ReceivedDate).format(CONST.settings.shortDateDisplayFormat);
                }

                data.TestComponents = ko.utils.arrayFilter(data.TestComponents, function (component) {
                    return !data.Supplementary || (data.Supplementary && component.MarkingResultTypeId === enums.MarkingResultType.Original);
                });

                ko.viewmodel.updateFromModel(serverModel, data);

                var comments = [];
                ko.utils.arrayForEach(serverModel.TestComponents(), function (component) {
                    ko.utils.arrayForEach(component.Competencies(), function (competence) {
                        ko.utils.arrayForEach(competence.Assessments(), function (assessment) {
                            assessment.Comment.extend({ maxLength: 2000 }); comments
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

        return vm;
    });