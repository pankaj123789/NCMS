define([
    'modules/custom-validator',
], function (customValidator) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            wizardService: null,
            materialRequest: null,
            action: null,
            materialRound: null,
            stepId: null
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
            materialRequest: defaultParams.materialRequest,
            materialRound: defaultParams.materialRound,
            settings: settings,
            action: defaultParams.action
        };

        vm.wizardNotesOptions = {
            PublicNote: serverModel.PublicNote,
            PrivateNote: serverModel.PrivateNote,
            ShowPublicNote: settings.ShowPublicNote,
            ShowPrivateNote: settings.ShowPrivateNote,
            PublicNotesHelpText: ko.observable(ko.Localization('Naati.Resources.MaterialRequest.resources.PublicNotesHelp'))
        };

        vm.load = function () {
            validator.reset();

            defaultParams.wizardService.getFluid('wizard/notes/' + vm.action().Id).then(function (data) {
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

            var request = {
                MaterialRequest: vm.materialRequest,
                Action : vm.action().Id,
                MaterialRound: vm.materialRound,
                Notes: ko.toJS(serverModel)
            };
            $.extend(request);
            defaultParams.wizardService.post(request, 'wizard/validateNotes').then(function (data) {
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