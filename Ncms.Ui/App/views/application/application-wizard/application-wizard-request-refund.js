define([
    'modules/custom-validator',
    'services/application-data-service',
    'modules/enums'
], function (customValidator, applicationService, enums) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {

        var defaultParams = {
            credentialRequest: null
        };

        $.extend(defaultParams, params);

        var serverModel = {
            Policy: ko.observable(),
            RefundPercentage: ko.observable(),
            RefundMethodType: ko.observable().extend({ required: true }),
            CredentialWorkflowFeeId: ko.observable(), 
            PaidAmount: ko.observable(),
            RefundableAmount: ko.observable(),
            InvoiceNumber: ko.observable(),
            Comments: ko.observable().extend({ maxLength: 500 }),
            BankDetails: ko.observable(),
            RefundAmount: ko.observable(0),
        };

        var vm = {
            refundDetails: serverModel,
            loading: ko.observable(true),
            refundMethodTypes: ko.observableArray(),
            refundPercentage: ko.computed(function () { return computeRefundPercentage() + '%' }),
            message: ko.observable(),
            calculatedAmount: ko.observable(0),
            submitted: ko.observable(false)
        };

        serverModel.RefundAmount.extend({
            required: true,
            validation: {
                validator: function (val) {
                    return val && serverModel.PaidAmount() && val <= serverModel.PaidAmount();
                },
                message: ko.Localization('Naati.Resources.Application.resources.InvalidRefundAmount')
            }
        });

        vm.showBankDetails = ko.pureComputed(function () {
            return serverModel.RefundMethodType() === enums.RefundMethodTypeName.DirectDeposit;
        });

        vm.requiredBankDetails = ko.pureComputed(function () {
            return serverModel.RefundMethodType() === enums.RefundMethodTypeName.DirectDeposit && vm.submitted();
        });

        serverModel.BankDetails.extend({ required: vm.requiredBankDetails, maxLength: 200 });

        var validator = customValidator.create(serverModel);

        vm.refundMethodTypeOptions = {
            value: serverModel.RefundMethodType,
            multiple: false,
            options: vm.refundMethodTypes,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            disable: ko.observable(false)
        };

        vm.load = function () {
            validator.reset();
            vm.loading(true);
            vm.submitted(false);

            var request = {
                credentialRequestId: defaultParams.credentialRequest.Id()
            };

            applicationService.getFluid('wizard/refundDetails', request).then(function (data) {
                ko.viewmodel.updateFromModel(serverModel, data);
                serverModel.RefundAmount(computeRefundAmount());
                vm.calculatedAmount(computeRefundAmount());

                applicationService.getFluid('lookuptype/RefundMethodType').then(function (methodTypes) {

                    var message = ko.Localization('Naati.Resources.Application.resources.RefundNotCalculated');;
                    var availableMethodTypes = data.AvailableRefundMethodTypes;
                    serverModel.RefundMethodType(null);

                    if (data.RefundCalculationResultType === enums.RefundCalculationResultType.RefundAvailable) {
                        message = '';
                        serverModel.RefundMethodType(availableMethodTypes[0]);
                    }
                    else if (data.RefundCalculationResultType === enums.RefundCalculationResultType.NoRefund) {
                        message = ko.Localization('Naati.Resources.Application.resources.NoRefundCalculated');
                    }

                    vm.message(message);
                    var types = ko.utils.arrayFilter(methodTypes,
                        function (mt) {
                            return availableMethodTypes.includes(mt.Id);
                        });
                    vm.refundMethodTypes(types);
                });
            });
        };

        vm.postData = function () {
            var requestModel = {
                RefundPercentage: computeRefundPortion(),
                RefundAmount: serverModel.RefundAmount(),
                RefundMethodTypeId: serverModel.RefundMethodType(),
                CredentialWorkflowFeeId: serverModel.CredentialWorkflowFeeId(),
                Comments: serverModel.Comments(),
                BankDetails: null
            };

            if (requestModel.RefundMethodTypeId === enums.RefundMethodTypeName.DirectDeposit) {
                requestModel.BankDetails = serverModel.BankDetails();
            }

            return ko.toJS(requestModel);
        };

        vm.isValid = function () {
            vm.submitted(true);
            return validator.isValid();
        };

        vm.activate = function () {
        };

        function computeRefundPercentage() {
            if (serverModel.PaidAmount()) {
                return serverModel.RefundAmount() / serverModel.RefundableAmount() * 100;
            }
            return 0;
        }

        function computeRefundPortion() {
            if (serverModel.PaidAmount()) {
                return serverModel.RefundAmount() / serverModel.RefundableAmount();
            }
            return 0;
        }

        function computeRefundAmount() {
            if (serverModel) {
                return serverModel.RefundableAmount() * serverModel.RefundPercentage();
            }
            return 0;
        }



        return vm;
    }
});