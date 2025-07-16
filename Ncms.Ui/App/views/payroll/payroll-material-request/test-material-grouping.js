define([
    'views/shell',
    'views/payroll/payroll-material-request/participant-grouping'
], function (shell, participantGrouping) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
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
                    view: 'views/payroll/payroll-material-request/participant-grouping',
                    model: participantGrouping.getInstance({ items: i.Items, expanded: i.expanded })
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
