define([
    'services/application-data-service',
    'services/screen/message-service',
    'services/setup-data-service'
],
    function (applicationService, messageService, setupService) {
        return {
            getInstance: getInstance
        };

        function getInstance(apiadmin, isCreate) {
            
            var composed = false;

            const mask = 'xxxxxxxxxxxxxxxx';

            var serverModel = $.extend({
                ApiAccessId: ko.observable(),

                Name: ko.observable(),
                Permissions: ko.observable(),
                PublicKey: ko.observable(),
                PrivateKey: ko.observable(),
                MaskedPublicKey: ko.observable(),
                MaskedPrivateKey: ko.observable(),
                Active: ko.observable(),
                InstitutionId: ko.observable(),
                PermissionIds: ko.observableArray([]),
                IsPublicKeyMasked: ko.observable(),
                IsPrivateKeyMasked: ko.observable()
            }, apiadmin);

            var vm = {
                apiadmin: serverModel,
                title: ko.pureComputed(function() {
                    return '{0} - {1}'.format(ko.Localization('Naati.Resources.Epi.resources.EditApiClient'),
                        serverModel.Name());
                }),
                locations: ko.observableArray(),
                isCreate: isCreate,
                institutions: ko.observableArray(),
                permissions: ko.observableArray(),
                compositionComplete: compositionComplete,
            };

            vm.institutionOptions = {
                value: serverModel.InstitutionId,
                multiple: false,
                options: vm.institutions,
                optionsValue: 'Id',
                optionsText: 'DisplayName',
            };

            vm.apiEndpointOptions = {
                selectedOptions: serverModel.PermissionIds,
                multiple: true,
                initialise: true,
                options: vm.permissions,
                optionsValue: 'Id',
                optionsText: 'DisplayName'
            };

            vm.reGenerateApiKeys = function () {

                setupService.getFluid('NewGuid').then(function (data) {

                    serverModel.PublicKey(data);

                    if (serverModel.MaskedPublicKey() == mask) {
                        alert("Public Key Regenerated");
                    }
                    else {
                        serverModel.MaskedPublicKey(serverModel.PublicKey());
                    }
                });

                setupService.getFluid('NewGuid').then(function (data) {

                    serverModel.PrivateKey(data);

                    if (serverModel.MaskedPrivateKey() == mask) {
                        alert("Private Key Regenerated");
                    }
                    else {
                        serverModel.MaskedPrivateKey(serverModel.PrivateKey());
                    }
                });
            };

            vm.copyPublicKeyToClipboard = function () {
                navigator.clipboard.writeText(vm.apiadmin.PublicKey());
                alert("Text Copied");
            };

            vm.copyPrivateKeyToClipboard = function () {
                navigator.clipboard.writeText(vm.apiadmin.PrivateKey());
                alert("Text Copied");
            };

            vm.activate = function () {
                applicationService.getFluid('lookuptype/InstitutionById').then(vm.institutions);
                setupService.getFluid('GetApiPermissionOptions').then(vm.permissions);
                if (isCreate) {
                    serverModel.IsPublicKeyMasked(false);
                    serverModel.IsPrivateKeyMasked(false);
                }
                else {
                    serverModel.MaskedPublicKey(mask);
                    serverModel.MaskedPrivateKey(mask);
                    serverModel.IsPublicKeyMasked(true);
                    serverModel.IsPrivateKeyMasked(true);
                }
            };

            vm.updatePublicKey = function (data, event) {
                serverModel.PublicKey(serverModel.MaskedPublicKey);
            }

            vm.updatePrivateKey = function (data, event) {
                serverModel.PrivateKey(serverModel.MaskedPrivateKey);
            }

            vm.togglePublicKeyVisible = function () {

                if (serverModel.MaskedPublicKey() == mask) {
                    serverModel.MaskedPublicKey(serverModel.PublicKey());
                    serverModel.IsPublicKeyMasked(false);
                }
                else {
                    serverModel.MaskedPublicKey(mask);
                    serverModel.IsPublicKeyMasked(true);
                }
            }

            vm.togglePrivateKeyVisible = function () {

                if (serverModel.MaskedPrivateKey() == mask) {
                    serverModel.MaskedPrivateKey(serverModel.PrivateKey());
                    serverModel.IsPrivateKeyMasked(false);
                }
                else {
                    serverModel.MaskedPrivateKey(mask);
                    serverModel.IsPrivateKeyMasked(true);
                }
            }

            vm.showPublicKeyTooltip = {
                container: 'body',
                placement: 'bottom',
                trigger: 'hover',
                title: function () {
                    if (serverModel.MaskedPublicKey() == mask) {
                        tooltip = ko.Localization('Naati.Resources.Api.resources.ShowKey')
                    }

                    if (serverModel.MaskedPublicKey() != mask) {
                        tooltip = ko.Localization('Naati.Resources.Api.resources.HideKey');
                    }

                    return tooltip
                }
            };

            vm.showPrivateKeyTooltip = {
                container: 'body',
                placement: 'bottom',
                trigger: 'hover',
                title: function () {
                    if (serverModel.MaskedPrivateKey() == mask) {
                        tooltip = ko.Localization('Naati.Resources.Api.resources.ShowKey')
                    }

                    if (serverModel.MaskedPrivateKey() != mask) {
                        tooltip = ko.Localization('Naati.Resources.Api.resources.HideKey');
                    }

                    return tooltip
                }
            };

            vm.copyToClipboard = {
                container: 'body',
                placement: 'bottom',
                trigger: 'hover',
                title: ko.Localization('Naati.Resources.Api.resources.CopyToClipboard'),
            };

            vm.load = function() {

            };

            function compositionComplete() {
                composed = true;

            }

            return vm;
        }
    });

