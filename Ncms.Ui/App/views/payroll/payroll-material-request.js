define([
    'views/shell',
    'services/material-request-data-service',
    'views/payroll/payroll-material-request/user-grouping'
], function (shell, materialRequestService, userGrouping) {

    var vm = {
        users: ko.observableArray([]),
        title: shell.titleWithSmall,
        searched: ko.observable(false)
    };

    var userGroupingInstance = userGrouping.getInstance({ items: vm.users })
    vm.userGroupingCompose = {
        view: 'views/payroll/payroll-material-request/user-grouping',
        model: userGroupingInstance
    };

    vm.expandAll = function () {
        userGroupingInstance.expandAll();
    };

    vm.collapseAll = function () {
        userGroupingInstance.collapseAll();
    };

    vm.activate = function () {
        vm.users([]);

        materialRequestService.getFluid('materialrequesttoapprove').then(
            function (data) {
                vm.searched(true);
                var users = ko.viewmodel.fromModel(data)();
                addApprovedFlag(users);
                ko.utils.arrayForEach(users, function (u) {
                    addApprovedFlag(u.Items(), u);
                    ko.utils.arrayForEach(u.Items(), function (t) {
                        addApprovedFlag(t.Items(), t);
                    });
                });
                vm.users(users);
            });
    };

    vm.approve = function () {
        var req = copyProperties(vm.users);

        materialRequestService.post(req, 'approvepayment').then(
            function () {
                vm.activate();
            });
    };

    function copyProperties(items) {
        return ko.utils.arrayMap(items(), function (i) {
            var value = {};

            for (var p in i) {
                if (!ko.isObservable(i[p])) {
                    continue;
                }
                if (p == 'Parent') {
                    continue;
                }
                if (p == 'Items') {
                    value[p] = copyProperties(i[p]);
                    continue;
                }
                value[p] = i[p];
            }

            return ko.toJS(value);
        });
    }

    var preventBubbling = false;
    function addApprovedFlag(items, parent) {
        ko.utils.arrayForEach(items, function (item) {
            item.Approved = ko.observable(false);
            item.Parent = parent;
            item.Approved.subscribe(function () {
                if (preventBubbling) {
                    return;
                }
                approveChildren(item, parent);
            });
        });
    }

    function approveChildren(item, parent) {
        preventBubbling = true;
        setChildren(item);
        setParent(parent);
        preventBubbling = false;
    }

    function setChildren(item) {
        if (!('Items' in item)) {
            return;
        }

        var shouldSetParent = false;
        var processChildren = [];
        ko.utils.arrayForEach(item.Items(), function (i) {
            if (!('Approved' in i)) {
                return;
            }
            shouldSetParent = true;
            i.Approved(item.Approved());
            processChildren.push(i);
        });

        ko.utils.arrayForEach(processChildren, function (i) {
            setChildren(i);
        });

        if (shouldSetParent) {
            setParent(item);
        }
    }

    function setParent(parent) {
        if (!parent) {
            return;
        }

        var indeterminate = ko.utils.arrayFilter(parent.Items(), function (pi) {
            return pi.Approved() == null;
        });

        var approved = ko.utils.arrayFilter(parent.Items(), function (pi) {
            return pi.Approved();
        });

        if (indeterminate.length) {
            parent.Approved(null);
        }
        else if (!approved.length) {
            parent.Approved(false);
        }
        else if (approved.length == parent.Items().length) {
            parent.Approved(true);
        }
        else {
            parent.Approved(null);
        }

        setParent(parent.Parent);
    }

    return vm;
});
