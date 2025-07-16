define([
    'views/payroll/payroll-material-request/claim-grouping'
], function (claimGrouping) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            items: ko.observableArray([])
        };

        $.extend(defaultParams, params);

        var vm = {
            items: ko.observableArray([])
        };

        ko.computed(function () {
            var items = ko.utils.arrayMap(defaultParams.items(), function (i) {
                i.expanded = ko.observable(false);
                i.groupingCompose = {
                    view: 'views/payroll/payroll-material-request/claim-grouping',
                    model: claimGrouping.getInstance({ items: i.Items })
                };
                return i;
            });

            vm.items(items);
        });

        vm.expandAll = function () {
            ko.utils.arrayForEach(vm.items(), function (i) {
                i.expanded(true);
            });
        };

        vm.collapseAll = function () {
            ko.utils.arrayForEach(vm.items(), function (i) {
                i.expanded(false);
            });
        };

        return vm;
    }
});
