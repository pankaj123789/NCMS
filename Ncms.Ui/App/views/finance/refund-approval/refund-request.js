define([
    'views/finance/refund-approval/refund-policy'
], function (refundPolicy) {
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
            refundMethodTypes: ko.observableArray()
        };

        ko.computed(function () {
            vm.refundMethodTypes(defaultParams.refundMethodTypes());
            var items = ko.utils.arrayMap(defaultParams.items(), function (i) {
                i.expanded = ko.observable(false);
                i.refundMethodTypeOptions = {
                    value: i.RefundMethodTypeId,
                    multiple: false,
                    options: vm.refundMethodTypes,
                    optionsValue: 'Id',
                    optionsText: 'DisplayName',
                    disable: ko.observable(false)
                };
                i.refundMethodType = ko.computed(function () {
                    return GetRefundMethodType(i.RefundMethodTypeId());
                });
                i.refundPercentageString = ko.computed(function () { return (i.RefundPercentage() * 100) + '%' });
                i.RefundPercentage = ko.computed(function () { return i.RefundPercentage() });

                i.groupingCompose = {
                    view: 'views/finance/refund-approval/refund-policy',
                    model: refundPolicy.getInstance(
                        {
                            policy: i.Policy,
                            comments: i.Comments,
                            bankDetails: i.BankDetails
                        })
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

        function GetRefundMethodType(refundMethodTypeId) {
            if (!vm.refundMethodTypes().length) {
                return;
            }
            var type = ko.utils.arrayFirst(vm.refundMethodTypes(), function (t) {
                return t.Id === refundMethodTypeId;
            });

            return type.DisplayName;
        }

        return vm;
    }
});
