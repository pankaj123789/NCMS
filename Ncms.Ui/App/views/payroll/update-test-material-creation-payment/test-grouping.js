define([
    'views/payroll/update-test-material-creation-payment/test-material-grouping'
], function (testMaterialGrouping) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var costFormatResource = ko.Localization('Naati.Resources.Payroll.resources.CostFormat');

        var defaultParams = {
            items: ko.observableArray([]),
        };

        $.extend(defaultParams, params);

        var vm = {
            items: ko.observableArray([]),
        };

        ko.computed(function () {
            var items = ko.utils.arrayMap(defaultParams.items(), function (i) {
                var isLoading = i.Loadings().length;
                var items = isLoading ? i.Loadings : i.Claims;
                i.expanded = ko.observable(false);
                i.groupingCompose = {
                    view: 'views/payroll/update-test-material-creation-payment/test-material-grouping',
                    model: testMaterialGrouping.getInstance({ items: items, isLoading: isLoading })
                };
                return i;
            });

            vm.items(items);
        });

        vm.cost = function (item) {
            return costFormatResource.format(ko.textCurrencyFormat(item.Total()));
        };

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
