define([
    'views/shell',
    'services/finance-data-service',
    'modules/enums',
], function (shell, financeService, enums) {

        var testSession = {
        Id: ko.observable(),
        Name: ko.observable(),
    };

    var serverModel = {
        ApplicationId: ko.observable().extend({ required: true }),
        PaymentReference: ko.observable().extend({ required: true }),
        CreditNoteNo: ko.observable().extend({ required: true })
    };

    var vm = {
        title: shell.titleWithSmall,
        serverModel: serverModel,
    };

    vm.serverModelValidation = ko.validatedObservable([serverModel.ApplicationId, serverModel.PaymentReference, serverModel.CreditNoteNo]);

    vm.progressCreditNote = function () {

        if (vm.validateServerModel()) {
            var request = {
                ApplicationId: serverModel.ApplicationId(),
                CreditNoteNo: serverModel.CreditNoteNo(),
                PaymentReference: serverModel.PaymentReference()
            };
            financeService.post(request, 'progressCreditNote').then(function (result) {
                toastr.success(ko.Localization('Naati.Resources.Finance.resources.CreditNoteHasProgressed'));
                clear();
            });
        }
    }

    vm.validateServerModel = function () {
        var isValid = vm.serverModelValidation.isValid();

        vm.serverModelValidation.errors.showAllMessages(!isValid);

        return isValid;
    };

    return vm;

});
