define([
    'views/shell',
    'services/payroll-data-service'
], function (shell, payrollService) {
    var vm = {
        users: ko.observableArray([]),
        title: shell.titleWithSmall
    };

    vm.payrollComponent = {
        name: 'payroll',
        params: {
            users: vm.users,
            headerTemplate: 'payroll-header-template',
            rowTemplate: 'payroll-row-template',
            groupHeaderTemplate: 'group-header-template',
            userHeaderTemplate: 'user-header-template'
        }
    };

    vm.activate = function () {
        return payrollService.getFluid('validate').then(vm.users);
    }

    vm.validate = function (marking, e) {
        var group = marking.Group;
        var user = group.User;

        payrollService.post(marking.JobExaminerId, 'validate').then(function () {
            group.Markings.remove(marking);

            if (!group.Markings().length)
                user.Groups.remove(group);

            if (!user.Groups().length)
                vm.users.remove(user);
        });
    };

    vm.expandAll = function () {
        vm.payrollComponent.params.component.expandAll();
    };

    vm.collapseAll = function () {
        vm.payrollComponent.params.component.collapseAll();
    };

    return vm;
});