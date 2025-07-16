define([
    'modules/enums',

    'services/institution-data-service',
    'services/application-data-service',
    'services/screen/date-service',
],
    function (enums, institutionService, applicationService, dateService) {

        var serverModel = {
            EndorsedQualificationId: ko.observable(),
            InstitutionId: ko.observable().extend({ required: true }),

            Location: ko.observable().extend({ maxLength: 200, required: true }), 

            Qualification: ko.observable().extend({ required: true, maxLength: 200 }),
            EndorsementPeriodFrom: ko.observable().extend({ required: true, dateGreaterThan: moment('1/1/1753').format('l') }),
            CredentialTypeId: ko.observable().extend({ required: true }),
            Active: ko.observable(false),
            Notes: ko.observable()
        };

        serverModel.EndorsementPeriodTo = ko.observable().extend({ required: true, dateGreaterThan: serverModel.EndorsementPeriodFrom });

        var emptyModel = ko.toJS(serverModel);

        var vm = {
            endorsedQualification: serverModel,
            credentialTypes: ko.observableArray(),
            institutions: ko.observableArray(),
        };

        vm.institutionOptions = {
            value: serverModel.InstitutionId,
            multiple: false,
            options: vm.institutions,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            disable : true
        };

        vm.credentialTypeOptions = {
            value: serverModel.CredentialTypeId,
            multiple: false,
            options: vm.credentialTypes,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            enableWithPermission: 'EndorsedQualification.Manage',
        };

        vm.EndorsementPeriodFromOptions = {
            value: vm.endorsedQualification.EndorsementPeriodFrom,
            css: 'form-control'
        };

        vm.EndorsementPeriodToOptions = {
            value: vm.endorsedQualification.EndorsementPeriodTo,
            css: 'form-control'
        };

        var dirtyFlag = new ko.DirtyFlag([serverModel], false);

        vm.canSave = ko.pureComputed(function () {
            return dirtyFlag().isDirty() && currentUser.hasPermission(enums.SecNoun.EndorsedQualification,enums.SecVerb.Update);
        });

        var validation = ko.validatedObservable(serverModel);
        
        vm.save = function () {
            var defer = Q.defer();

            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return defer.promise;
            }

            var request = ko.toJS(serverModel);
            request.EndorsementPeriodFrom = request.EndorsementPeriodFrom ? dateService.toPostDate(request.EndorsementPeriodFrom) : null;
            request.EndorsementPeriodTo = request.EndorsementPeriodTo ? dateService.toPostDate(request.EndorsementPeriodTo) : null;

            institutionService.post(request, 'createOrUpdateQualification')
                .then(function () {
                    dirtyFlag().reset();
                    defer.resolve('fulfilled');
                    toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                });

            return defer.promise;
        };

        vm.canActivate = function (id, query) {
            queryString = query || {};
            id = parseInt(id);

            ko.viewmodel.updateFromModel(serverModel, emptyModel);

            serverModel.EndorsedQualificationId(id);

            return loadEndorsedQualification();
        };

        function resetValidation() {
            if (validation.errors) {
                return validation.errors.showAllMessages(false);
            }
        };

        function loadEndorsedQualification() {
            applicationService.getFluid('lookuptype/CredentialType').then(vm.credentialTypes);
            applicationService.getFluid('lookuptype/InstitutionById').then(vm.institutions);

            return institutionService.getFluid('getEndorsedQualification/' + serverModel.EndorsedQualificationId()).then(function (data) {
                data.EndorsementPeriodFrom = moment(data.EndorsementPeriodFrom).toDate();
                data.EndorsementPeriodTo = moment(data.EndorsementPeriodTo).toDate();
                ko.viewmodel.updateFromModel(serverModel, data);
                
                resetValidation();
                dirtyFlag().reset();

                return true;
            });
        }

        return vm;

    });