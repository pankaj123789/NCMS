define([
    'services/screen/date-service',
], function (dateService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            materialRequest: null,
            outputMaterial:null,
            action: null,
            materialRound: null,
            stepId: null,
            wizardService: null
        };

        $.extend(defaultParams, params);

        var vm = {
            materialRequest: defaultParams.materialRequest,
            materialRound: defaultParams.materialRound,
            action: defaultParams.action,
            outputMaterial: defaultParams.outputMaterial,
        };

        vm.dueDateOptions = {
            value: vm.materialRound.DueDate,
            placeholder: ko.Localization('Naati.Resources.MaterialRequest.resources.DueDate')
        };

        var validation = ko.validatedObservable(vm.materialRound);  

        vm.load = function () {
            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }
            defaultParams.wizardService.getFluid('wizard/rounddetails/' + vm.materialRequest.MaterialRequestId() + '/' + vm.action().Id + '/' + vm.outputMaterial.TestComponentTypeId()).then(function (data) {
                data.DueDate = moment(data.DueDate).toDate();
                data.MaxBillableHours = data.DefaultBillableHours;
                ko.viewmodel.updateFromModel(vm.materialRound, data);
            });
        };

        vm.postData = function () {
            var data = ko.toJS(vm.materialRound);
            data.DueDate = dateService.toPostDate(data.DueDate);
            data.RoundNumber = data.Round;
            return data;
        };

        vm.isValid = function () {
            var isValid = validation.isValid();

            if (!isValid) {
                validation.errors.showAllMessages();
            }

            return isValid;
        };

        return vm;
    }
});