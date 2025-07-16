define([
    'plugins/router',
    'views/shell',
    'modules/enums',
    'services/util',
    'services/testspecification-data-service',
    'services/rubric-configuration-data-service',
    'services/screen/message-service'
], function (router, shell, enums, util, testSpecificationService, rubricConfigurationService, messageService) {
    var testSpecificationModel = {
        Id: ko.observable(),
        Description: ko.observable(),
    };

    var serverModel = {
        TestComponents: ko.observableArray(),
    }

    var vm = {
        conditions: {},
        title: ko.computed(function () {
            return '{0} - #{1} - {2}'.format(shell.titleWithSmall(), testSpecificationModel.Id(), testSpecificationModel.Description());
        }),
        testSpecificationId: ko.observable(),
        canSave: ko.observable(false),
        isWriter: ko.observable(false)
    };

    for (var i in enums.TestResultEligibilityType) {
        var value = enums.TestResultEligibilityType[i];
        vm.conditions[value] = ko.observableArray();
    }

    var conditionOptions = {
        showIfEmpty: true,
        headerTemplate: 'test-question-rules-header-template',
        rowTemplate: 'test-question-rules-row-template',
        detailsRowTemplate: 'test-question-rules-details-row-template',
        isWriter: vm.isWriter,
        isDirty: vm.canSave
    };

    vm.passConditionOptions = $.extend({}, conditionOptions, { conditions: vm.conditions[enums.TestResultEligibilityType.Pass] });
    vm.concededPassConditionOptions = $.extend({}, conditionOptions, { conditions: vm.conditions[enums.TestResultEligibilityType.ConcededPass] });
    vm.supplementaryTestConditionOptions = $.extend({}, conditionOptions, { conditions: vm.conditions[enums.TestResultEligibilityType.SupplementaryTest] });

    vm.canActivate = function () {
        currentUser.hasPermission(enums.SecNoun.TestSpecification, enums.SecVerb.Update).then(vm.isWriter);
        return true;
    };

    vm.activate = function (testSpecificationId) {
        for (var i in enums.TestResultEligibilityType) {
            var value = enums.TestResultEligibilityType[i];
            vm.conditions[value]([]);
        }

        testSpecificationService.getFluid(testSpecificationId).then(function (data) {
            ko.viewmodel.updateFromModel(testSpecificationModel, data[0]);
        });

        vm.testSpecificationId(testSpecificationId);
        rubricConfigurationService.getFluid('testquestionrule/' + testSpecificationId).then(function (data) {
            var configurations = ko.viewmodel.fromModel(data.Configurations);

            ko.utils.arrayForEach(configurations(), function (c) {
                var args = {
                    testComponentTypeId: c.TestComponentTypeId(),
                    minimumQuestionsAttempted: c.MinimumQuestionsAttempted(),
                    minimumQuestionsPassed: c.MinimumQuestionsPassed(),
                    testResultEligibilityTypeId: c.TestResultEligibilityTypeId(),
                    group: c.RuleGroup()
                };

                addTask(args);
            });

            vm.canSave(false);

            data.TestComponents.splice(0, 0, {
                TestComponentTypeId: null,
                TypeName: "any task type"
            });

            serverModel.TestComponents(data.TestComponents);
        });
    };

    vm.save = function () {
        var data = [];

        var conditions = [];
        for (var i in vm.conditions) {
            conditions = conditions.concat(vm.conditions[i]());
        }

        ko.utils.arrayForEach(conditions, function (c) {
            data.push({
                TestSpecificationId: vm.testSpecificationId(),
                TestComponentTypeId: c.testComponentTypeId(),
                TestResultEligibilityTypeId: c.testResultEligibilityTypeId(),
                MinimumQuestionsAttempted: c.minimumQuestionsAttempted(),
                MinimumQuestionsPassed: c.minimumQuestionsPassed(),
                RuleGroup: c.group()
            });
        });

        rubricConfigurationService.post(data, 'testquestionrule/' + vm.testSpecificationId()).then(function (saved) {
            toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
            vm.canSave(false);
        });
    };

    vm.close = function () {
        router.navigateBack();
    };

    vm.addPassTask = function () {
        addTask({ testResultEligibilityTypeId: enums.TestResultEligibilityType.Pass });
    };

    vm.addConcededPassTask = function () {
        addTask({ testResultEligibilityTypeId: enums.TestResultEligibilityType.ConcededPass });
    };

    vm.addSupplementaryTestTask = function () {
        addTask({ testResultEligibilityTypeId: enums.TestResultEligibilityType.SupplementaryTest });
    };

    function addTask(args) {
        var minimumQuestionsAttempted = args.minimumQuestionsAttempted;
        var minimumQuestionsPassed = args.minimumQuestionsPassed;
        var testComponentTypeId = args.testComponentTypeId;
        var testResultEligibilityTypeId = args.testResultEligibilityTypeId;
        var group = args.group;

        var condition = {
            group: ko.observable(group),
            showChildren: ko.observable(false),
            minimumQuestionsAttempted: ko.observable(minimumQuestionsAttempted || 1),
            minimumQuestionsPassed: ko.observable(minimumQuestionsPassed || 0),
            testResultEligibilityTypeId: ko.observable(testResultEligibilityTypeId || enums.TestResultEligibilityType.Pass),
            testComponentTypeId: ko.observable(testComponentTypeId),
            children: null,
            toggleChildren: function () { },
            isWriter: vm.isWriter,
            remove: function () {
                messageService.remove().then(function (answer) {
                    if (answer !== 'yes') {
                        return;
                    }

                    vm.conditions[condition.testResultEligibilityTypeId()].remove(condition);
                    vm.canSave(true);
                });
            }
        };

        condition.testComponentTypeOptions = {
            multiple: false,
            options: serverModel.TestComponents,
            value: condition.testComponentTypeId,
            selectedValue: ko.computed(function () {
                return (ko.utils.arrayFirst(serverModel.TestComponents(), function (e) {
                    return e.TestComponentTypeId == condition.testComponentTypeId();
                }) || { TypeName: '' }).TypeName;
            })
        };

        ko.computed(function () {
            condition.minimumQuestionsAttempted();
            condition.testComponentTypeId();
            condition.minimumQuestionsPassed();
            condition.testResultEligibilityTypeId();
            vm.canSave(true);
        });

        vm.conditions[condition.testResultEligibilityTypeId()].push(condition);
        vm.canSave(true);

        return condition;
    }

    return vm;
});
