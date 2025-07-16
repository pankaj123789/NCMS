define([
    'views/payroll/payroll-material-request/test-grouping'
], function (testGrouping) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var firstApprovalByResource = ko.Localization('Naati.Resources.MaterialRequest.resources.FirstApprovalBy');
        var defaultParams = {
            items: ko.observableArray([]),
        };

        $.extend(defaultParams, params);

        var vm = {
            items: ko.observableArray([]),
            searched: ko.observable(false),
        };

        vm.formatUserName = function (user) {
            return firstApprovalByResource.format(user.UserDisplayName());
        };

        ko.computed(function () {
            var items = ko.utils.arrayMap(defaultParams.items(), function (i) {
                i.expanded = ko.observable(false);
                i.groupingCompose = {
                    view: 'views/payroll/payroll-material-request/test-grouping',
                    model: testGrouping.getInstance({ items: i.Items, expanded: i.expanded })
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
