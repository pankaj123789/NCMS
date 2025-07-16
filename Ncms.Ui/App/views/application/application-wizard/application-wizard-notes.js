define([
    'modules/custom-validator',
    'services/application-data-service',
], function (customValidator, applicationService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            application: null,
            action: null,
            credentialRequest: null
        };

        $.extend(defaultParams, params);

        var serverModel = {
            PublicNote: ko.observable(),
            PrivateNote: ko.observable()
        };

        var validator = customValidator.create(serverModel);

        var settings = {
            ShowPublicNote: ko.observable(true),
            ShowPrivateNote: ko.observable(true),
            RequirePublicNote: ko.observable(true),
            RequirePrivateNote: ko.observable(false)
        };

        var vm = {
            note: serverModel,
            application: defaultParams.application,
            settings: settings,
            action: defaultParams.action,
            credentialRequest: defaultParams.credentialRequest,
        };

        vm.wizardNotesOptions = {
            PublicNote: serverModel.PublicNote,
            PrivateNote: serverModel.PrivateNote,
            ShowPublicNote: settings.ShowPublicNote,
            ShowPrivateNote: settings.ShowPrivateNote,
            PublicNotesHelpText: ko.observable(ko.Localization('Naati.Resources.Application.resources.PublicNotesHelp'))
        };

        vm.load = function () {
            validator.reset();

            var request = {
                actionId: vm.action().Id,
                applicationId: vm.application.ApplicationId()
            };

            applicationService.getFluid('noterules', request).then(function (data) {
                ko.viewmodel.updateFromModel(settings, data);
                serverModel.PublicNote.extend({ required: data.RequirePublicNote });
                serverModel.PrivateNote.extend({ required: data.RequirePrivateNote });
            });
        };

        vm.postData = function () {
            return ko.toJS(serverModel);
        };

        vm.isValid = function () {
            var defer = Q.defer();

            var request = ko.toJS(serverModel);
            $.extend(request, { ApplicationId: vm.application.ApplicationId(), Action: vm.action().Id, CredentialRequestId: vm.credentialRequest.Id() });
            applicationService.post(request, 'wizard/notes').then(function (data) {
                validator.reset();

                ko.utils.arrayForEach(data.InvalidFields, function (i) {
                    validator.setValidation(i.FieldName, false, i.Message);
                });

                validator.isValid();

                defer.resolve(!data.InvalidFields.length);
            });

            return defer.promise;
        };

        return vm;
    }
});