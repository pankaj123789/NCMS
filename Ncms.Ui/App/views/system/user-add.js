define([
    'modules/enums',
    'services/util',
    'services/application-data-service',
    'services/system-data-service',
    'services/user-data-service'
], function (enums, util, applicationService, systemService, userService) {
    var vm = {
        getInstance: getInstance
    };

    return vm;

    function getInstance() {
        var defer = null;

        var serverModel = {
            NonWindowsUser: ko.observable(),
            UserName: ko.observable().extend({
                required: true,
                maxLength: 255,
                pattern: {
                    params: /^[\w\.\-@]*$/,
                    message: 'User Name may only contain letters, numbers, and the following: . _ - @'
                }
            }),
            FullName: ko.observable().extend({ required: true, maxLength: 255 }),
            Password: ko.observable(),
            OfficeId: ko.observable().extend({ required: true }),
            Email: ko.observable().extend({ required: true, email: true, maxLength: 255 }),
            UpdatePassword: true
        };

        serverModel.Password.extend({
            required: {
                onlyIf: function() {
                    return serverModel.NonWindowsUser();
                }
            },
            minLength: {
                params: 10,
                onlyIf: function() {
                    return serverModel.NonWindowsUser();
                }
            },
            maxLength: 255,
            pattern: {
                onlyIf: serverModel.NonWindowsUser(),
                params: /^(?=.*[a-z])(?=.*[A-Z])(?=.*[\d])(?=.*[^\da-zA-Z]).*$/,
                message: "Passwords must contain a mix of upper case, lower case, numbers and symbols"
            }
        });

        var emptyModel = ko.toJS(serverModel);
        var validation = ko.validatedObservable(serverModel);

        var vm = {
            user: serverModel,
            modalId: util.guid(),
            credentialTypes: ko.observableArray(),
            offices: ko.observableArray(),
            domainName: ko.observable(),
            showPassword: ko.observable(),
        };

        serverModel.User = ko.computed({
            write: function (value) {
                if (serverModel.NonWindowsUser()) {
                    serverModel.Email(value);
                    return;
                }
                serverModel.Email(value + vm.domainName());
                serverModel.UserName(value + vm.domainName());
            },
            read: function () {
                if (serverModel.NonWindowsUser()) {
                    return serverModel.Email();
                }
                return (serverModel.Email() || '').replace(vm.domainName(), '');
            }
        });

        vm.officeOptions = {
            value: serverModel.OfficeId,
            multiple: false,
            options: vm.offices,
            optionsValue: 'Id',
            optionsText: 'DisplayName'
        };

        vm.activate = function () {
            applicationService.getFluid('lookuptype/OfficeAbbreviation').then(vm.offices);
            systemService.getFluid('domainname').then(vm.domainName);
        };

        vm.canSave = ko.computed(function () {
            return validation.isValid();
        });

        vm.save = function () {
            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return;
            }

            var req = ko.toJS(serverModel);
            userService.post(req, 'createOrUpdateUser').then(function (data) {
                req.Id = data.Id;
                defer.resolve(req);
                vm.close();
            });
        };

        vm.show = function () {
            defer = Q.defer();

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

        vm.viewPassword = function () {
            vm.showPassword(!vm.showPassword());
        }
        return vm;
    }
});