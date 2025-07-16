define([
    'plugins/router',
    'views/shell',
    'views/system/user-add',
    'services/user-data-service',
    'services/util'
], function (router, shell, userAdd, userService, util) {

    var userAddInstance = userAdd.getInstance();

    var vm = {
        title: shell.titleWithSmall,
        tableDefinition: {
            id: util.guid(),
            headerTemplate: 'users-header-template',
            rowTemplate: 'users-row-template'
        },
        users: ko.observableArray([]),
        computedUsers: ko.observableArray([]),
        showInactive: ko.observable(false),
        idSearchValue: ko.observable()
    };

    ko.computed(function () {
        vm.computedUsers([]);
        var roles = '';
        var users = ko.utils.arrayFilter(vm.users(), function (u) {
            ko.utils.arrayForEach(u.UserRoles, function (d) {
                roles += d + ', ';
            });
            u.Roles = roles.slice(0, -2);
            roles = '';
            return u.Active || (!u.Active && vm.showInactive());
        });
        vm.computedUsers(users);
    });

    vm.tableDefinition.dataTable = {
        source: vm.computedUsers,
        columnDefs: [
            { targets: -2, orderable: false }
        ],
        order: [
            [0, "asc"],
            [7, "asc"]
        ],
        buttons: {
            dom: {
                button: {
                    tag: 'label',
                    className: ''
                },
                buttonLiner: {
                    tag: null
                }
            },
            buttons: [{
                text: '<input type="checkbox" /><i></i><span>' + ko.Localization('Naati.Resources.SystemResources.resources.ShowInactive') + '</span>',
                className: 'i-switch i-switch-md bg-info m-l',
                init: function (dt, node, config) {
                    node.children('input[type="checkbox"]').prop('checked', vm.showInactive());
                },
                action: function (e, dt, node, config) {
                    var checked = !node.children('input[type="checkbox"]').prop('checked');
                    node.children('input[type="checkbox"]').prop('checked', checked);
                    vm.showInactive(checked);
                }
            }]
        }
    };

    vm.activate = function () {
        vm.showInactive(false);
        userService.getFluid('userSearch').then(function (data) {
            vm.users(data);
        });

    };

    vm.edit = function (user) {
        edit(user.Id);
    };

    vm.activeMessage = function (user) {
        if (user.Active) {
            return ko.Localization('Naati.Resources.Shared.resources.Active');
        }

        return ko.Localization('Naati.Resources.Shared.resources.NotActive');
    };

    vm.userAddOptions = {
        view: 'views/system/user-add',
        model: userAddInstance
    };

    vm.add = function () {
        userAddInstance.show().then(function (data) {
            edit(data.Id);
        });
    };

    vm.idSearchClick = function () {
        userService.getFluid('getUserDetailsById/' + vm.idSearchValue(), { supressResponseMessages: true })
            .then(function (data) {
                if (!data) {
                    return toastr.success(ko.Localization('Naati.Resources.Shared.resources.NotFound').format('User', vm.idSearchValue()));
                }
                router.navigate('system/user/' + vm.idSearchValue());
            },
            function () {
                return toastr.success(ko.Localization('Naati.Resources.Shared.resources.NotFound').format('User', vm.idSearchValue()));
            });
    };

    vm.checkEnterPressed = function (data, e) {
        var keyCode = e.which || e.keyCode;
        if (keyCode == 13) {
            vm.idSearchClick();
            return false;
        }
        return true;
    };

    return vm;

    function edit(id) {
        router.navigate('system/user/' + id);
    }
});

