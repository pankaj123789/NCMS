define([
], function () {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            policy: ko.observable(),
            comments: ko.observable(),
        };

        $.extend(defaultParams, params);

        var vm = {
            policy: ko.observable(),
            comments: ko.observable(),
            bankDetails: ko.observable(),
        };


        ko.computed(function () {
            vm.policy(defaultParams.policy());
            vm.comments(defaultParams.comments());
            vm.bankDetails(defaultParams.bankDetails());
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
