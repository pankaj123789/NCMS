define([
    'services/rubric-configuration-data-service',
    'services/screen/message-service',
    'plugins/router'
], function (rubricConfigurationService, message, router) {

    var serverModel = {
        BandId: ko.observable(),
        Label: ko.observable().extend({ required: true, maxLength: 255 }),
        Description: ko.observable(),
        Level: ko.observable(),
        CriterionName: ko.observable(),
        CriterionLabel: ko.observable()
    };

    var vm = {
        activate: activate,
        windowTitle: ko.observable(),
        rubricModel: serverModel,
        save: save,
        tryClose: tryClose,
        wysiwygOptions: {
            value: serverModel.Description
        }
    };

    var validation = ko.validatedObservable(vm.rubricModel);
    vm.dirtyFlag = new ko.DirtyFlag([vm.rubricModel], false);
    vm.canSave = ko.computed(function () {
        return vm.dirtyFlag().isDirty();
    });

    function save() {
        var isValid = validation.isValid();
        validation.errors.showAllMessages(!isValid);

        if (isValid) {
            var json = ko.toJS(vm.rubricModel);
            rubricConfigurationService.post(json, 'markingband')
                .then(
                    function () {
                        toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                        vm.dirtyFlag().reset();
                    });
        }
    }

    function activate(id) {
        rubricConfigurationService.getFluid('markingband/' + id).then(function (data) {
            vm.windowTitle(ko.Localization('Naati.Resources.RubricConfiguration.resources.MarkingBand') + ' ' + id + " - " + data.RubricMarkingBand.Label);

            data.RubricMarkingBand.BandId = id;
            ko.viewmodel.updateFromModel(serverModel, data.RubricMarkingBand);
            vm.dirtyFlag().reset();
        });
    }

    function tryClose() {
        if (vm.dirtyFlag().isDirty()) {
            message.confirm({
                title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                content: ko.Localization('Naati.Resources.Shared.resources.AreYouSureYouWantToCancel')
            })
                .then(
                    function (answer) {
                        if (answer === 'yes') {
                            close();
                        }
                    });
        } else {
            close();
        }
    }

    function close() {
        router.navigateBack();
    };
    return vm;
});