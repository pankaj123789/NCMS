define([
    'modules/enums',
    'services/util',
    'services/institution-data-service',
    'services/application-data-service',
    'services/screen/date-service',
], function (enums, util, institutionService, applicationService, dateService) {
    var vm = {
        getInstance: getInstance
    };

    return vm;

    function getInstance() {

        var defer = null;

        var serverModel = {
            EndorsedQualificationId: ko.observable(),
            InstitutionId: ko.observable().extend({ required: true }),
            Location: ko.observable().extend({ required: true }),
            Qualification: ko.observable().extend({ required: true, maxLength: 200 }),
            EndorsementPeriodFrom: ko.observable().extend({ required: true, dateGreaterThan: moment('1/1/1753').format('l') }),
            CredentialTypeId: ko.observable().extend({ required: true }),
            Active: ko.observable(false)
        };

        serverModel.EndorsementPeriodTo = ko.observable().extend({ required: true, dateGreaterThan: serverModel.EndorsementPeriodFrom });

        var emptyModel = ko.toJS(serverModel);
        var validation = ko.validatedObservable(serverModel);

        var vm = {
            isSaveClicked : ko.observable(false),
            endorsedQualification: serverModel,
            modalId: util.guid(),
            credentialTypes: ko.observableArray(),
            institutions: ko.observableArray(),
        };
      
        vm.credentialTypeOptions = {
            value: serverModel.CredentialTypeId,
            multiple: false,
            options: vm.credentialTypes,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
        };

        vm.institutionOptions = {
            value: serverModel.InstitutionId,
            multiple: false,
            options: vm.institutions,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
        };

        vm.EndorsementPeriodFromOptions = {
            value: vm.endorsedQualification.EndorsementPeriodFrom,
            css: 'form-control'
        };

        vm.EndorsementPeriodToOptions = {
            value: vm.endorsedQualification.EndorsementPeriodTo,
            css: 'form-control'
        };
        
        vm.activate = function () {
            applicationService.getFluid('lookuptype/CredentialType').then(vm.credentialTypes);
            applicationService.getFluid('lookuptype/InstitutionById').then(vm.institutions);
        };

        vm.canSave = ko.pureComputed(function () {
            return !vm.isSaveClicked() && currentUser.hasPermissionSync(enums.SecNoun.EndorsedQualification, enums.SecVerb.Create);
        });

        vm.save = function () {

            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return;
            }
            vm.isSaveClicked(true);
           
            var req = ko.toJS(serverModel);
            req.EndorsementPeriodFrom = req.EndorsementPeriodFrom ? dateService.toPostDate(req.EndorsementPeriodFrom) : null;
            req.EndorsementPeriodTo = req.EndorsementPeriodTo ? dateService.toPostDate(req.EndorsementPeriodTo) : null;
            institutionService.post(req, 'createOrUpdateQualification').then(function (data) {
                req.EndorsedQualificationId = data.Id;
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