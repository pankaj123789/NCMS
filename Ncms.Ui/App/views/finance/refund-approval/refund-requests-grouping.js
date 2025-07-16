define([
    'views/finance/refund-approval/refund-request'
], function (refundRequests) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            items: ko.observableArray([]),
            refundMethodTypes: ko.observableArray()
        };

        $.extend(defaultParams, params);

        var vm = {
            items: ko.observableArray([]),
            searched: ko.observable(false),
        };

        ko.computed(function () {
            var items = ko.utils.arrayMap(defaultParams.items(), function (i) {
                i.expanded = ko.observable(false);
                i.groupingCompose = {
                    view: 'views/finance/refund-approval/refund-request',
                    model: refundRequests.getInstance({ items: i.Items, expanded: i.expanded, refundMethodTypes: defaultParams.refundMethodTypes })
                };
                return i;
            });
            vm.items(items);
        });

        vm.expandAll = function () {
            ko.utils.arrayForEach(vm.items(), function (i) {
                i.expanded(true);
                i.groupingCompose.model.expandAll();
            });
        };

        vm.collapseAll = function () {
            ko.utils.arrayForEach(vm.items(), function (i) {
                i.expanded(false);
                i.groupingCompose.model.collapseAll();
            });
        };

        return vm;
    }
});
