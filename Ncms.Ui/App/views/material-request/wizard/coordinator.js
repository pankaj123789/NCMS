define([
    'services/util',
    'modules/enums',
    'services/material-request-data-service',
], function (util, enums, materialRequestService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            wizardService: null,
            materialRequest: null,
            outputMaterial: null,
            action: null,
            materialRound: null,
            stepId: null
        };

        $.extend(defaultParams, params);

        var vm = {
            materialRequest: defaultParams.materialRequest,
            outputMaterial: defaultParams.outputMaterial,
            members: ko.observableArray(),
            coordinatorId: ko.observable().extend({ required: true }),
            coordinatorOptions: {},
            activeRequests: ko.observableArray(),
        };


        vm.coordinatorId.subscribe(updateActiveRequests);
        vm.tableDefinition = {
            headerTemplate: 'active-requests-header-template',
            rowTemplate: 'active-requests-row-template',
            dataTable: {
                source: vm.activeRequests,
                columnDefs: [
                    { targets: -1, orderable: false }
                ]
            }
        }

        var validation = ko.validatedObservable([vm.coordinatorId]);


        vm.coordinatorOptions = {
            value: vm.coordinatorId,
            multiple: false,
            options: vm.members,
            optionsValue: 'Id',
            optionsText: 'Name',
        };

        vm.load = function () {
            defaultParams.wizardService.getFluid('wizard/coordinator/' + vm.materialRequest.PanelId() + '/' + vm.outputMaterial.CredentialTypeId() + '/' + vm.materialRequest.MaterialRequestId()).then(function (data) {     
                ko.utils.arrayForEach(data,
                    function (item) {
                        if (item.IsCoordinatorCredentialType) {
                            item.Name = item.Name + ' (' + ko.Localization('Naati.Resources.MaterialRequest.resources.Coordinator') + ')';
                        }
                        if (item.PreSelected) {
                            vm.coordinatorId(item.Id);
                        }
                    });

                vm.members(data);
            });

           
        };

        vm.isValid = function () {
            var isValid = validation.isValid();

            if (!isValid) {
                validation.errors.showAllMessages();
            }

            return isValid;
        };

        vm.postData = function () {

            var data = {
                PanelMembershipId: vm.coordinatorId()
            };

            return data;
        };


        function updateActiveRequests(value) {
            if (value <= 0) {
                return;
            }

            var result = ko.utils.arrayFilter(vm.members(),
                function (m) {
                    return m.Id === value;
                });

            if (!result.length) {
                return;
            }

            var coordinatorNaatiNumber = result[0].NaatiNumber;

            materialRequestService.getFluid('activeCoordinatorRequests/' + coordinatorNaatiNumber).then(function (data) {

                ko.utils.arrayForEach(data,
                    function (materialRequest) {
                        materialRequest.StatusClass = util.getMaterialRequestStatusCss(materialRequest.RequestStatusTypeId);
                    });

                vm.activeRequests(data);
            });
        }

        return vm;
    }
});