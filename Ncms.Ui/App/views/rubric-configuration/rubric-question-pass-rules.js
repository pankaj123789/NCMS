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
        title: ko.computed(function () {
            return '{0} - #{1} - {2}'.format(shell.titleWithSmall(), testSpecificationModel.Id(), testSpecificationModel.Description());
        }),
        showDescriptions: ko.observable(false),
        testSpecificationId: ko.observable(),
        canSave: ko.observable(false),
        isWriter: ko.observable(false)
    };

    vm.rubricOptions = {
        testComponents: serverModel.TestComponents,
        showDescriptions: vm.showDescriptions,
        testSpecificationId: vm.testSpecificationId,
        isWriter: vm.isWriter,
        addBand: function (args) {
            var condition = addBand(args);
            args.testComponent.Conditions.push(condition);
            vm.canSave(true);
        }
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

        rubricConfigurationService.getFluid('questionpassrule/' + testSpecificationId).then(function (data) {
            var configurations = ko.viewmodel.fromModel(data.Configurations);
            var testComponents = ko.viewmodel.fromModel(data.TestComponents);

            ko.utils.arrayForEach(testComponents(), function (t) {
                t.Conditions = ko.observableArray();
                t.tableTemplate = 'question-pass-table-template';
                t.conditionOptions = {
                    conditions: t.Conditions,
                    headerTemplate: 'conditions-header-template',
                    rowTemplate: 'conditions-row-template',
                    detailsRowTemplate: 'rubric-question-details-row-template',
                    isWriter: vm.isWriter,
                    isDirty: vm.canSave
                };

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
                        testComponent: t
                    };

                    var condition = addBand(args);
                    condition.group(c.RuleGroup());
                    t.Conditions.push(condition);
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
            var conditions = testComponent.Conditions();

            ko.utils.arrayForEach(conditions, function (c) {
                data.push({
                    TestSpecificationId: vm.testSpecificationId(),
                    TestComponentTypeId: testComponent.TestComponentTypeId(),
                    RubricMarkingAssessmentCriterionId: c.assessment().Id(),
                    MaximumBandLevel: c.band() + 1,
                    RuleGroup: c.group()
                });
            });
        }

        rubricConfigurationService.post(data, 'questionpassrule/' + vm.testSpecificationId()).then(function (saved) {
            if (!saved) return;

            toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
            vm.canSave(false);
        });
    };

    vm.close = function () {
        router.navigateBack();
    };

    function addBand(args) {
        var assessment = args.assessment;
        var band = args.band;
        var data = args.testComponent;

        var condition = {
            assessment: ko.observable(assessment),
            band: ko.observable(band),
            group: ko.observable(),
            showChildren: ko.observable(false),
            children: null,
            toggleChildren: function () { },
            isWriter: vm.isWriter,
            remove: function () {
                messageService.remove().then(function (answer) {
                    if (answer !== 'yes') {
                        return;
                    }

                    data.Conditions.remove(condition);
                    vm.canSave(true);
                });
            }
        };

        condition.simpleDescription = ko.computed(function () {
            return 'At least Band <em>{0}</em>'.format(condition.band() + 1);
        });

        condition.description = ko.computed(function () {
            return '{0} in Criterion <em>{1}</em>'.format(condition.simpleDescription(), condition.assessment().Name());
        });

        return condition;
    }

    return vm;
});
