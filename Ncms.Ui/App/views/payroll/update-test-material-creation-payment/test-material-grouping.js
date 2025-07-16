define([
], function () {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            items: ko.observableArray([]),
            isLoading: ko.observable(false)
        };

        $.extend(defaultParams, params);

        var vm = {
            items: defaultParams.items,
            isLoading: defaultParams.isLoading
        };

        vm.remove = function (item) {
            item.Removed(true);
        };

        return vm;
    }
});
