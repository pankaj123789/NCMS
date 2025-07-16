define([
    'views/shell',
    'services/application-data-service',
    'views/finance/refund-approval/refund-requests-grouping'
], function (shell, applicationService, refundRequestGrouping) {

    var vm = {
        refundGroups: ko.observableArray([]),
        title: shell.titleWithSmall,
        searched: ko.observable(false),
        refundMethodTypes: ko.observableArray()
    };

    var refundRequestGroupingInstance = refundRequestGrouping.getInstance({ items: vm.refundGroups, refundMethodTypes: vm.refundMethodTypes })
    vm.refundRequestGroupingCompose = {
        view: 'views/finance/refund-approval/refund-requests-grouping',
        model: refundRequestGroupingInstance
    };

    vm.expandAll = function () {
        refundRequestGroupingInstance.expandAll();
    };

    vm.collapseAll = function () {
        refundRequestGroupingInstance.collapseAll();
    };

    vm.activate = function () {
        vm.refundGroups([]);

        applicationService.getFluid('refundRequestToApprove').then(
            function (data) {
                vm.searched(true);
                var refundRequests = ko.viewmodel.fromModel(data)();
                addApprovedFlag(refundRequests);
                ko.utils.arrayForEach(refundRequests, function (u) {
                    addApprovedFlag(u.Items(), u);
                });
                vm.refundGroups(refundRequests);

                applicationService.getFluid('lookuptype/RefundMethodType').then(function (methodTypes) {
                    vm.refundMethodTypes(methodTypes);
                });
            });
    };

    vm.approve = function () {
        var req = copyProperties(vm.refundGroups);

        applicationService.post(req, 'approverefundrequests').then(
            function () {
                vm.activate();
            });
    };

    vm.isValid = ko.computed(function () {
        return ko.utils.arrayFirst(vm.refundGroups(), function (i) {
            return ko.utils.arrayFirst(i.Items(), function (j) {
                return j.RefundAmount() && j.RefundAmount() <= j.InitialPaidAmount();
            });
        });
    });

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
