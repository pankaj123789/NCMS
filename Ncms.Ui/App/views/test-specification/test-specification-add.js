define([
    'modules/enums',
    'services/util',
    'services/testspecification-data-service',
    'services/application-data-service',
    'services/material-request-data-service',
], function (enums, util, testSpecificationService, applicationService, materialRequestService) {
    var vm = {
        getInstance: getInstance
    };

    return vm;

    function getInstance() {
        var defer = null;

        var serverModel = {
            Id: ko.observable(),
            Title: ko.observable().extend({ required: true, maxLength: 255 }),
            CredentialTypeId: ko.observable().extend({ required: true })
        };

        var emptyModel = ko.toJS(serverModel);
        var validation = ko.validatedObservable(serverModel);

        var vm = {
            isSaveClicked: ko.observable(false),
            testSpecification: serverModel,
            modalId: util.guid(),
            languages: ko.observableArray(),
            credentialTypes: ko.observableArray(),
            skills: ko.observableArray(),
            testComponentTypes: ko.observableArray(),
            domains: ko.observableArray(),
            baseOnSkill: ko.observable(),
            baseOnLanguage: ko.observable(),
            disableDomainOptions: ko.observable()
    };


        ko.computed(function () {
            var selectedTestComponentType = ko.utils.arrayFirst(vm.testComponentTypes(), function (t) {
                return t.Id === serverModel.TestComponentTypeId();
            }) || {};

        });


        vm.credentialTypeOptions = {
            value: serverModel.CredentialTypeId,
            multiple: false,
            options: vm.credentialTypes,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
        };

        ko.computed(function () {
            var credentialTypeId = serverModel.CredentialTypeId();
            if (!credentialTypeId) {
                vm.testComponentTypes([]);
                vm.domains([]);
                return vm.skills([]);
            }

            var request = { CredentialTypeIds: [credentialTypeId] };
            applicationService.getFluid('testtask', request).then(function (data) {

                $.each(data, function (i, d) {
                    var availability = !d.Active ? '(Unavailable)' : '';
                    d.DisplayName = '{0} - {1}{2} {3}'.format(d.DisplayName, 'SPEC', d.TestSpecificationId, availability);
                });

                vm.testComponentTypes(data);
            });


        });

        vm.activate = function () {
            //currentUser.hasPermission(enums.SecNoun.TestMaterial, enums.SecVerb.Create).then(function (data) { vm.disableDomainOptions(!data)});
            applicationService.getFluid('lookuptype/CredentialType').then(vm.credentialTypes);
        };

        vm.canSave = ko.pureComputed(function () {
            return !vm.isSaveClicked();
        });

        vm.save = function () {
            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return;
            }
            vm.isSaveClicked(true);

            var req = ko.toJS(serverModel);
            testSpecificationService.post(req, 'addTestSpecification').then(function (data) {
                req.Id = data;
                defer.resolve(req);
                vm.close();
            });
        };

        vm.show = function () {
            defer = Q.defer();

            vm.isSaveClicked(false);

            ko.viewmodel.updateFromModel(serverModel, emptyModel);

            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }

            $('#' + vm.modalId).modal('show');

            return defer.promise;
        };

        vm.close = function () {
            $('#' + vm.modalId).modal('hide');
        };

        return vm;
    }
});