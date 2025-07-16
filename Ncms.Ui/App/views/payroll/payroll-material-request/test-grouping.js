define([
    'views/payroll/payroll-material-request/test-material-grouping'
], function (testMaterialGrouping) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var requestsToBeVerifiedResource = ko.Localization('Naati.Resources.MaterialRequest.resources.RequestsToBeVerified');
        var oldestSubmittedClaimResource = ko.Localization('Naati.Resources.MaterialRequest.resources.OldestSubmittedClaim');
        var defaultParams = {
            items: ko.observableArray([]),
        };

        $.extend(defaultParams, params);

        var vm = {
            items: ko.observableArray([]),
        };

        ko.computed(function () {
            var items = ko.utils.arrayMap(defaultParams.items(), function (i) {
                i.expanded = ko.observable(false);
                i.groupingCompose = {
                    view: 'views/payroll/payroll-material-request/test-material-grouping',
                    model: testMaterialGrouping.getInstance({ items: i.Items, expanded: i.expanded })
                };
                return i;
            });

            vm.items(items);
        });

        vm.requestsToBeVerified = function (item) {
            return requestsToBeVerifiedResource.format(item.TotalRequests());
        };

        vm.oldestSubmittedClaim = function (item) {
            var date = moment().diff(moment(item.OldestSubmittedate()), 'days');
            return oldestSubmittedClaimResource.format(date);
        };

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
