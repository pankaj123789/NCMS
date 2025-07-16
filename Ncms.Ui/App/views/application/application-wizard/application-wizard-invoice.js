define([
    'services/application-data-service',
], function (applicationService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var serverModel = {
            InvoiceTo: ko.observable(),
            DueDate: ko.observable(),
            Branding: ko.observable(),
            ReferenceText: ko.observable(),
            UserOfficeAbbreviation: ko.observable(),
            Lines: ko.observableArray(),
            DoNotInvoice: ko.observable()
        };

        var defaultParams = {
            application: null,
            action: null,
            credentialRequest: null
        };

        $.extend(defaultParams, params);

        var vm = {
            application: defaultParams.application,
            action: defaultParams.action,
            credentialRequest: defaultParams.credentialRequest,
            invoice: serverModel,
            tableDefinition: {
                searching: false,
                paging: false,
                oLanguage: { sInfoEmpty: '', sInfo: '' }
            }
        };

        serverModel.InvoiceReference = ko.computed({
            read: function () {
                return serverModel.UserOfficeAbbreviation() + ' - ' + vm.application.ApplicationReference() + (serverModel.ReferenceText() ? ' - ' + serverModel.ReferenceText() : '');
            },
            write: function () { }//just to prevent error on ko.viewmodel.updateFromModel
        });

        vm.load = function () {
            var request = {
                ApplicationId: vm.application.ApplicationId(),
                ActionId: Number(vm.action().Id),
                CredentialRequestId: vm.credentialRequest.Id(),
            };

            applicationService.post(request, 'wizard/invoice').then(function (data) {
                ko.viewmodel.updateFromModel(serverModel, data);
            });
        };

        vm.postData = function () {
            return ko.toJS(serverModel);
        };

        vm.sumExGst = sumarize('ExGst');
        vm.sumGst = sumarize('Gst');
        vm.sumTotal = sumarize('Total');

        function sumarize(field) {
            return ko.pureComputed(function () {
                var total = 0;
                ko.utils.arrayForEach(serverModel.Lines(), function (l) {
                    total += l[field]();
                });
                return total;
            });
        }

        return vm;
    }
});