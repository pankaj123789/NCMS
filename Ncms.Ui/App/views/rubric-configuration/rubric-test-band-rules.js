define([
    'plugins/router',
    'views/shell',
    'modules/enums',
    'services/testspecification-data-service',
    'services/rubric-configuration-data-service',
    'services/screen/message-service'
], function (router, shell, enums, testSpecificationService, rubricConfigurationService, messageService) {
    var testSpecificationModel = {
        Id: ko.observable(),
        Description: ko.observable(),
    };

    var serverModel = {
        TestComponents: ko.observableArray(),
    };

    var vm = {
        isWriter: ko.observable(false),
        title: ko.computed(function () {
            return '{0} - #{1} - {2}'.format(shell.titleWithSmall(), testSpecificationModel.Id(), testSpecificationModel.Description());
        }),
        showDescriptions: ko.observable(false),
        testSpecificationId: ko.observable(),
        canSave: ko.observable(false)
    };

    vm.rubricOptions = {
        isWriter: vm.isWriter,
        testComponents: serverModel.TestComponents,
        showDescriptions: vm.showDescriptions,
        testSpecificationId: vm.testSpecificationId,
        addConditionOptions: [
            {
                text: ko.Localization('Naati.Resources.RubricConfiguration.resources.Pass'),
                action: function (args) {
                    args.testResultEligibilityTypeId = enums.TestResultEligibilityType.Pass;
                    addCondition(args);
                }
            },
            {
                text: ko.Localization('Naati.Resources.RubricConfiguration.resources.ConcededPass'),
                action: function (args) {
                    args.testResultEligibilityTypeId = enums.TestResultEligibilityType.ConcededPass;
                    addCondition(args);
                }
            },
            {
                text: ko.Localization('Naati.Resources.RubricConfiguration.resources.SupplementaryTest'),
                action: function (args) {
                    args.testResultEligibilityTypeId = enums.TestResultEligibilityType.SupplementaryTest;
                    addCondition(args);
                }
            },
        ],
        isDirty: vm.canSave
    };

    vm.toggleBandDescriptions = function () {
        vm.showDescriptions(!vm.showDescriptions());
    };

    vm.toggleDescriptionsLabel = ko.computed(function () {
        return ko.Localization(vm.showDescriptions() ? 'Naati.Resources.RubricConfiguration.resources.HideBandDescriptions' : 'Naati.Resources.RubricConfiguration.resources.ShowBandDescriptions');
    });

    vm.canActivate = function () {
        currentUser.hasPermission(enums.SecNoun.TestSpecification, enums.SecVerb.Update).then(vm.isWriter);
        return true;
    };

    vm.activate = function (testSpecificationId) {
        vm.testSpecificationId(testSpecificationId);

        testSpecificationService.getFluid(testSpecificationId).then(function (data) {
            ko.viewmodel.updateFromModel(testSpecificationModel, data[0]);
        });

        rubricConfigurationService.getFluid('testbandrule/' + testSpecificationId).then(function (data) {
            var configurations = ko.viewmodel.fromModel(data.Configurations);
            var testComponents = ko.viewmodel.fromModel(data.TestComponents);
            var conditionOptions = {
                headerTemplate: 'test-band-header-template',
                rowTemplate: 'test-band-row-template',
                detailsRowTemplate: 'test-band-details-row-template',
                isDirty: vm.canSave,
                isWriter: vm.isWriter,
            };

            ko.utils.arrayForEach(testComponents(), function (t) {
                t.conditions = {};

                for (var i in enums.TestResultEligibilityType) {
                    var value = enums.TestResultEligibilityType[i];
                    t.conditions[value] = ko.observableArray();
                }

                t.tableTemplate = 'test-band-table-template';
                t.passConditionOptions = $.extend({}, conditionOptions, { conditions: t.conditions[enums.TestResultEligibilityType.Pass] });
                t.concededPassConditionOptions = $.extend({}, conditionOptions, { conditions: t.conditions[enums.TestResultEligibilityType.ConcededPass] });
                t.supplementaryTestConditionOptions = $.extend({}, conditionOptions, { conditions: t.conditions[enums.TestResultEligibilityType.SupplementaryTest] });

                ko.utils.arrayForEach(configurations(), function (c) {
                    if (c.TestComponentTypeId() != t.TestComponentTypeId()) {
                        return;
                    }

                    var assessment = null;

                    ko.utils.arrayFirst(t.Competencies(), function (competence) {
                        assessment = ko.utils.arrayFirst(competence.Assessments(), function (a) {
                            return a.Id() == c.RubricMarkingAssessmentCriterionId();
                        });

                        return assessment;
                    });

                    var args = {
                        assessment: assessment,
                        band: c.MaximumBandLevel() - 1,
                        numberOfQuestions: c.NumberOfQuestions(),
                        testResultEligibilityTypeId: c.TestResultEligibilityTypeId(),
                        testComponent: t
                    };

                    var condition = addBand(args);
                    condition.group(c.RuleGroup());
                    t.conditions[args.testResultEligibilityTypeId].push(condition);
                });
            });

            serverModel.TestComponents(testComponents());
        });
    };

    vm.save = function () {
        var testComponents = serverModel.TestComponents();
        var data = [];

        for (var i = 0; i < testComponents.length; i++) {
            var testComponent = testComponents[i];
            var conditions = [];
            for (var c in testComponent.conditions) {
                conditions = conditions.concat(testComponent.conditions[c]());
            }

            ko.utils.arrayForEach(conditions, function (c) {
                data.push({
                    TestSpecificationId: vm.testSpecificationId(),
                    TestComponentTypeId: testComponent.TestComponentTypeId(),
                    RubricMarkingAssessmentCriterionId: c.assessment().Id(),
                    NumberOfQuestions: c.numberOfQuestions(),
                    TestResultEligibilityTypeId: c.testResultEligibilityTypeId(),
                    MaximumBandLevel: c.band() + 1,
                    RuleGroup: c.group()
                });
            });
        }

        rubricConfigurationService.post(data, 'testbandrule/' + vm.testSpecificationId()).then(function (saved) {
            if (!saved) return;

            toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
            vm.canSave(false);
        });
    };

    vm.close = function () {
        router.navigateBack();
    };

    function addCondition(args) {
        args.testComponent.conditions[args.testResultEligibilityTypeId].push(addBand(args));
        vm.canSave(true);
    }

    function addBand(args) {
        var assessment = args.assessment;
        var band = args.band;
        var data = args.testComponent;
        var numberOfQuestions = args.numberOfQuestions;
        var testResultEligibilityTypeId = args.testResultEligibilityTypeId;

        var condition = {
            assessment: ko.observable(assessment),
            numberOfQuestions: ko.observable(numberOfQuestions || 1),
            band: ko.observable(band),
            group: ko.observable(),
            testResultEligibilityTypeId: ko.observable(testResultEligibilityTypeId || enums.TestResultEligibilityType.Pass),
            showChildren: ko.observable(false),
            children: null,
            isWriter: vm.isWriter,
            toggleChildren: function () { },
            remove: function () {
                messageService.remove().then(function (answer) {
                    if (answer !== 'yes') {
                        return;
                    }

                    data.conditions[condition.testResultEligibilityTypeId()].remove(condition);
                    vm.canSave(true);
                });
            }
        };

        condition.numberOfQuestions.subscribe(function () {
            vm.canSave(true);
        });

        return condition;
    }

    return vm;
});
