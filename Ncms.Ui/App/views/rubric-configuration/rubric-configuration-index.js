define([
    'plugins/router',
    'views/shell',
    'services/testspecification-data-service',
    'services/rubric-configuration-data-service',
    'modules/enums'
], function (router, shell, testSpecificationService, rubricConfigurationService, enums) {
    var testSpecificationModel = {
        Id: ko.observable(),
        Description: ko.observable(),
    };

    var serverModel = {
        TestComponents: ko.observableArray(),
    }

    var vm = {
        title: ko.computed(function () {
            return '{0} - #{1} - {2}'.format(shell.titleWithSmall(), testSpecificationModel.Id(), testSpecificationModel.Description());
        }),
        testSpecificationId: ko.observable(),
        isWriter: ko.observable(false)
    };

    vm.rubricOptions = {
        testComponents: serverModel.TestComponents,
        isWriter: vm.isWriter,
        showDescriptions: true,
        testSpecificationId: vm.testSpecificationId,
        addConditionOptions: [
            {
                icon: 'fa-pencil-alt',
                action: function (args) {
                    router.navigate('rubric-configuration/marking-band/' + args.markingBand.Id());
                }
            }
        ]
    };

    vm.canActivate = function () {
        currentUser.hasPermission(enums.SecNoun.TestSpecification, enums.SecVerb.Update).then(vm.isWriter);
        return true;
    };

    vm.activate = function (testSpecificationId) {
        vm.testSpecificationId(testSpecificationId);

        testSpecificationService.getFluid(testSpecificationId).then(function (data) {
            ko.viewmodel.updateFromModel(testSpecificationModel, data[0]);
        });

        rubricConfigurationService.getFluid(testSpecificationId).then(function (data) {
            ko.viewmodel.updateFromModel(serverModel, data);
        });
    };

    vm.close = function () {
        router.navigate('test-specification/' + vm.testSpecificationId());
    };

    return vm;
});
