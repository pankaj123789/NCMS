define([
    'services/setup-data-service',
    'views/system/apiadmin-details',
    'modules/enums'
],
    function (setupService, apiAdminDetails, enums) {

        var serverModel = {
            ApiAccessId: ko.observable(),
            Name: ko.observable(),
            PublicKey: ko.observable().extend({ required: true, maxLength: 255 }),
            PrivateKey: ko.observable().extend({ required: true, maxLength: 255 }),
            Permissions: ko.observable(),
            Active: ko.observable(),
            InstitutionId: ko.observable().extend({ required: true }),
            PermissionIds: ko.observableArray([]).extend({ required: true }),
        };

        serverModel.Active(true);

        var emptyModel = ko.toJS(serverModel);

        var vm = {
            apiAdmin: serverModel,
            title: ko.pureComputed(function () {
                return '{0} - {1}'.format(ko.Localization('Naati.Resources.Api.resources.EditApiClient'), serverModel.Name());
            }),
            locations: ko.observableArray()
        };

        var apiAdminDetailsInstance = apiAdminDetails.getInstance(serverModel, false);
        apiAdminDetailsInstance.load();

        vm.apiAdminDetailsOptions = {
            view: 'views/system/apiadmin-details.html',
            model: apiAdminDetailsInstance
        };

        var dirtyFlag = new ko.DirtyFlag([serverModel], false);

        vm.canSave = ko.pureComputed(function () {
            return true;
        });

        var validation = ko.validatedObservable(serverModel);

        vm.save = function () {
            var defer = Q.defer();

            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return defer.promise;
            }

            var request = ko.toJS(serverModel);

            setupService.post(request, 'saveapiadmin')
                .then(function () {
                    dirtyFlag().reset();
                    defer.resolve('fulfilled');
                    toastr.success(ko.Localization('Naati.Resources.Api.resources.SavedSuccessfully'));
                });

            return defer.promise;
        };

        vm.reGenerateApiKeys = function () {
            setupService.getFluid('NewGuid').then(serverModel.PublicKey);
            setupService.getFluid('NewGuid').then(serverModel.PrivateKey);
        };

        vm.copyToClipboard = function () {

        };

        vm.canActivate = function (id, query) {

            queryString = query || {};
            id = parseInt(id);

            ko.viewmodel.updateFromModel(serverModel, emptyModel);


            serverModel.ApiAccessId(id);

            return currentUser.hasPermission(enums.SecNoun.System, enums.SecVerb.Manage).then(
                function (canUpdate) {
                    if (!canUpdate) {
                        return false;
                    }

                    return loadApiAdmin();
                });
        };

        function resetValidation() {
            if (validation.errors) {
                return validation.errors.showAllMessages(false);
            }
        };

        function loadApiAdmin() {

            return setupService.getFluid('apiadmin/' + serverModel.ApiAccessId()).then(function (data) {

                ko.viewmodel.updateFromModel(serverModel, data[0]);

                apiAdminDetailsInstance.compositionComplete();

                resetValidation();
                dirtyFlag().reset();

                return true;
            });
        }

        return vm;

    });

