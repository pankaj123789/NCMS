define([
    'views/payroll/payroll-material-request/user-grouping'
], function (userGrouping) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            items: ko.observableArray([]),
        };

        $.extend(defaultParams, params);

        var vm = {
            items: defaultParams.items,
        };

        return vm;
    }
});
