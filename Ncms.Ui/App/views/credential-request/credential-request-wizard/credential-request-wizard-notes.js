define([
    'modules/custom-validator',
    'services/credentialrequest-data-service'
], function (customValidator, credentialrequestService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            summary: null,
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
            summary: defaultParams.summary,
            enable: ko.observable(true)
        };

        vm.wizardNotesOptions = {
            PublicNote: serverModel.PublicNote,
            PrivateNote: serverModel.PrivateNote,
            ShowPublicNote: settings.ShowPublicNote,
            ShowPrivateNote: settings.ShowPrivateNote,
            Enable: vm.enable,
            PublicNotesHelpText: ko.observable(ko.Localization('Naati.Resources.Application.resources.PublicNotesHelp'))
        };

        vm.load = function () {
            validator.reset();
            vm.enable(true);

            credentialrequestService.post(request(), 'noterules').then(function (data) {
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

            credentialrequestService.post(request(), 'notes').then(function (data) {
                validator.reset();

                ko.utils.arrayForEach(data.InvalidFields, function (i) {
                    validator.setValidation(i.FieldName, false, i.Message);
                });

                validator.isValid();

                var isValid = !data.InvalidFields.length;
                defer.resolve(isValid);
                vm.enable(!isValid);
            });

            return defer.promise;
        };

        function request() {
            var json = vm.summary.Request();
            return $.extend(json, ko.toJS(serverModel));
        }

        return vm;
    }
});