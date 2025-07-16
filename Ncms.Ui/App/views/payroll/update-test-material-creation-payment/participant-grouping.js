define([
    'modules/common',
    'views/payroll/update-test-material-creation-payment/test-grouping'
], function (common, testGrouping) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var requestsToBeVerifiedResource = ko.Localization('Naati.Resources.MaterialRequest.resources.RequestsToBeVerified');
        var oldestApprovedResource = ko.Localization('Naati.Resources.MaterialRequest.resources.OldestApproved');
        var costFormatResource = ko.Localization('Naati.Resources.Payroll.resources.CostFormat');

        var defaultParams = {
            items: ko.observableArray([]),
        };

        $.extend(defaultParams, params);

        var vm = {
            items: ko.observableArray([]),
            searched: ko.observable(false),
        };

        vm.requestsToBeVerified = function (item) {
            return requestsToBeVerifiedResource.format(item.TotalRequests());
        };

        vm.cost = function (item) {
            return costFormatResource.format(ko.textCurrencyFormat(item.Amount()));
        };

        vm.oldestApproved = function (item) {
            var date = common.functions().humanizeDate(moment(item.OldestApprovedDate()).toDate());
            return oldestApprovedResource.format(date);
        };

        ko.computed(function () {
            var items = ko.utils.arrayMap(defaultParams.items(), function (i) {
                i.expanded = ko.observable(false);
                i.groupingCompose = {
                    view: 'views/payroll/update-test-material-creation-payment/test-grouping',
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
