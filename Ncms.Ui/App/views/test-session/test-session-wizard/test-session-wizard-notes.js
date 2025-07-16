define([
    'modules/custom-validator',
    'services/testsession-data-service'
], function (customValidator, testsessionService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            session: null
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
            session: defaultParams.session,
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

            testsessionService.getFluid('wizard/notes', ko.toJS(vm.session)).then(function (data) {
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
            var request = $.extend(ko.toJS(vm.session), vm.postData());

            testsessionService.post(request, 'wizard/notes').then(function (data) {
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

        return vm;
    }
});