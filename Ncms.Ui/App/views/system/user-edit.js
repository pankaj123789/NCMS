define([
        'modules/enums',
        'services/application-data-service',
    'services/system-data-service',
        'services/user-data-service'
    ],
    function(enums, applicationService, systemService, userService) {

        var serverModel = {
            Id: ko.observable(),
            UserName: ko.observable().extend({
                required: true,
                maxLength: 255,
                pattern: {
                    params: /^[\w\.\-@]*$/,
                    message: 'User Name may only contain letters, numbers, and the following: . _ - @'
                },
            }),
            FullName: ko.observable().extend({ required: true, maxLength: 255 }),
            Email: ko.observable().extend({
                required: true,
                email: true,
                maxLength: 255,
                validation: {
                    validator: function (val) {
                        if (!serverModel.NonWindowsUser()) {
                            return true;
                        }

                        if (val && vm) {
                            var tmp = val.toUpperCase().split('@');
                            return ('@' + tmp[tmp.length - 1]) != vm.domainName().toUpperCase();
                        }

                        return false;
                    },
                    message: function () {
                        return (vm ? vm.domainName() : '') + ' is just allowed for Windows Users';
                    }
                }
            }),
            OfficeId: ko.observable().extend({ required: true }),
            Notes: ko.observable().extend({ maxLength: 500 }),
            Active: ko.observable(),
            RoleIds: ko.observableArray(),
            SystemUser: ko.observable(),
            NonWindowsUser: ko.observable(),
            UpdatePassword: ko.observable(0),
            Password: ko.observable()
        };

        var emptyModel = ko.toJS(serverModel);

        var vm = {
            user: serverModel,
            title: ko.pureComputed(function() {
                return '{0} - #{1}'.format(ko.Localization('Naati.Resources.Shared.resources.EditUser'),
                    serverModel.Id());
            }),
            systemUserFlagTxt: ko.pureComputed(function() {
                return serverModel.SystemUser()
                    ? '(System User)'
                    : serverModel.NonWindowsUser()
                    ? '(Non-Windows User)'
                    : '';
            }),
            credentialTypes: ko.observableArray(),
            offices: ko.observableArray(),
            domainName: ko.observable(),
            userRoles: ko.observableArray(),
            tableDefinition: {
                id: 'usereditTable',
                headerTemplate: 'useredit-header-template',
                rowTemplate: 'useredit-row-template'
            },
            showPassword: ko.observable()
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
                return (serverModel.Email() || '').replace(new RegExp(vm.domainName(), "ig"), '');
            }
        });

        serverModel.Password.extend({
            required: {
                onlyIf: function () {
                    return serverModel.UpdatePassword();
                }
            },
            minLength: {
                params: 10,
                onlyIf: function () {
                    return serverModel.UpdatePassword();
                }
            },
            maxLength: 255,
            pattern: {
                onlyIf: serverModel.UpdatePassword(),
                params: /^(?=.*[a-z])(?=.*[A-Z])(?=.*[\d])(?=.*[^\da-zA-Z]).*$/,
                message: "Passwords must contain a mix of upper case, lower case, numbers and symbols"
            }
        });
          
        vm.tableDefinition.dataTable = {
            source: vm.userRoles,
            order: [
                [1, "asc"]
            ],
            select: {
                style: 'multi+shift'
            },
            events: {
                select: selectTable,
                deselect: selectTable,
                length: pageLengthChanged
            },
            createdRow: function (row, data, index) {

                if (serverModel.RoleIds().length > 0) {
                    if (serverModel.RoleIds().includes(parseInt(data[0]))) {
                        $(row).addClass("selected");
                    }
                }

            }
        };

        function pageLengthChanged(e) {
            var pageLength = $(e.target).DataTable().page.len();
            if (pageLength !== 10) {
                $(e.target).closest('.panel-body').css("height", "100%");
            } else {
                $(e.target).closest('.panel-body').css("height", "775px");
            }
        }

        function selectTable(e, dt) {
            serverModel.RoleIds([]);

            var indexes = dt.rows('.selected').indexes();

            if (!indexes.length) {
                return;
            }

            for (var i = 0; i < indexes.length; i++) {
                serverModel.RoleIds.push(vm.userRoles()[indexes[i]].Id);
            }
        }

        vm.officeOptions = {
            value: serverModel.OfficeId,
            multiple: false,
            options: vm.offices,
            optionsValue: 'Id',
            optionsText: 'DisplayName'
        };
        
        var dirtyFlag = new ko.DirtyFlag([serverModel], false);

        var validation = ko.validatedObservable(vm);

        self.ajaxError = ko.observable(false);

        vm.canSave = ko.computed(function () {
            return (!serverModel.SystemUser() || (validation.isValid() && dirtyFlag().isDirty())) && !ajaxError();
        });

        vm.save = function () {
            var defer = Q.defer();

            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return defer.promise;
            }

            var request = ko.toJS(serverModel);

            userService.post(request, 'createOrUpdateUser')
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

            serverModel.Id(id);

            return loadUser();
        };

        vm.viewPassword = function () {
            vm.showPassword(!vm.showPassword());
        }

        function resetValidation() {
            if (validation.errors) {
                return validation.errors.showAllMessages(false);
            }
        };

        function loadUser() {
            applicationService.getFluid('lookuptype/OfficeAbbreviation').then(vm.offices);
            systemService.getFluid('domainname').then(vm.domainName);

            userService.getFluid('userRoles').then(function (data) {
                vm.userRoles(data);
            });

            return userService.getFluid('getUserDetailsById/' + serverModel.Id()).then(function (data) {
                data.UserName = $.trim(data.UserName);

                if (!data.NonWindowsUser && data.UserName.indexOf('@') > 0) {
                    var parts = data.UserName.split('@');
                    vm.domainName('@' + parts[1]);
                }

                serverModel.UserName(data.UserName);

                var roleIds = data.RoleIds;
                delete data.RoleIds;
                ko.viewmodel.updateFromModel(serverModel, data);
                serverModel.RoleIds(roleIds);

                resetValidation();
                dirtyFlag().reset();

                return true;
            });
        }

        var ajaxEventName = 'ajaxError';

        self.activate = function () {
            $(document).on(ajaxEventName + '.wizard', function () {
                self.ajaxError(true);
            });
        };

        self.deactivate = function () {
            self.ajaxError(false);
            $(document).off(ajaxEventName + '.wizard');
        };

        return vm;

    });

