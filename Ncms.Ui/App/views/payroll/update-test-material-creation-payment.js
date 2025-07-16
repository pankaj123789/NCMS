define([
    'views/shell',
    'services/material-request-data-service',
    'services/screen/message-service',
    'views/payroll/update-test-material-creation-payment/participant-grouping'
], function (shell, materialRequestService, message, grouping) {
    var removeOptions = {
        content: ko.Localization('Naati.Resources.MaterialRequest.resources.AreYouSureMoveToPending'),
        yes: ko.Localization('Naati.Resources.Shared.resources.Ok')
    };

    var vm = {
        items: ko.observableArray([]),
        title: shell.titleWithSmall,
        searched: ko.observable(false)
    };

    var groupingInstance = grouping.getInstance({ items: vm.items })
    vm.groupingCompose = {
        view: 'views/payroll/update-test-material-creation-payment/participant-grouping',
        model: groupingInstance
    };

    vm.canMarkAsPaid = ko.computed(function () {
        return ko.utils.arrayFirst(vm.items(), function (i) {
            return i.PaymentReference();
        });
    });

    vm.expandAll = function () {
        groupingInstance.expandAll();
    };

    vm.collapseAll = function () {
        groupingInstance.collapseAll();
    };

    vm.activate = function () {
        vm.items([]);

        materialRequestService.getFluid('memberstopay').then(
            function (data) {
                vm.searched(true);
                var users = ko.viewmodel.fromModel(data)();
                ko.utils.arrayForEach(users, function (u) {
                    ko.utils.arrayForEach(u.Items(), function (i) {
                        ko.utils.arrayForEach(i.Claims(), function (c) {
                            c.Removed.subscribe(function () {
                                remove(c);
                            });
                        });

                        ko.utils.arrayForEach(i.Loadings(), function (l) {
                            l.Removed.subscribe(function () {
                                remove(l);
                            });
                        });
                    });
                });
                vm.items(users);
            });
    };

    vm.markAsPaid = function () {
        var req = copyProperties(vm.items);

        materialRequestService.post(req, 'MarkAsPaid').then(
            function () {
                vm.activate();
            });
    };

    var preventRemoveBubble = false;
    function remove(item) {
        if (!item.Removed() || preventRemoveBubble) {
            return;
        }

        message.remove(removeOptions).then(function (answer) {
            if (answer != 'yes') {
                preventRemoveBubble = true;
                item.Removed(false);
                preventRemoveBubble = false;
                return;
            }

            var req = copyProperties(vm.items);
            materialRequestService.post(req, 'unapprove').then(
                function () {
                    vm.activate();
                });
        });
    };

    function copyProperties(items) {
        return ko.utils.arrayMap(items(), function (i) {
            var value = {};

            for (var p in i) {
                if (!ko.isObservable(i[p])) {
                    continue;
                }
                if (p == 'Items' || p == 'Loadings' || p == 'Claims') {
                    value[p] = copyProperties(i[p]);
                    continue;
                }

                value[p] = i[p]();
            }

            return value;
        });
    }

    return vm;
});
