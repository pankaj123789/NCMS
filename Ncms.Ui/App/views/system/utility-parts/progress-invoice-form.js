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
        InvoiceNo: ko.observable().extend({ required: true }),
        InvoiceId: ko.observable().extend({ required: true }),
        PaymentId: ko.observable().extend({ required: true })
    };

    var vm = {
        title: shell.titleWithSmall,
        serverModel: serverModel,
    };

    vm.serverModelValidation = ko.validatedObservable([serverModel.ApplicationId, serverModel.InvoiceNo, serverModel.InvoiceId, serverModel.PaymentId]);

    vm.progressInvoice = function () {

        if (vm.validateServerModel()) {
            var request = {
                ApplicationId: serverModel.ApplicationId(),
                InvoiceNo: serverModel.InvoiceNo(),
                InvoiceId: serverModel.InvoiceId(),
                PaymentId: serverModel.PaymentId()
            };
            financeService.post(request, 'progressInvoice').then(function (result) {
                toastr.success(ko.Localization('Naati.Resources.Finance.resources.InvoiceHasProgressed'));
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
