//This will be auto load by bundle.config
/*jshint -W020 */
CONST = (function () {
    /*jshint +W020 */
    var settings = {
        shortDateDisplayFormat: 'DD/MM/YYYY',
        shortDateTimeDisplayFormat: 'DD/MM/YYYY LT',
        timeDisplayFormat: 'LT',
        yearMonthDayFormat: 'YYYY MM DD'
    };

    var eventNames = {
        cancelLoadingIndicator: 'cancelLoadingIndicator',
        cancelAjaxRequest: 'cancelAjaxRequest',
        showLoadingIndicator: 'showLoadingIndicator',
        loadingIndicatorStatusChanged: 'loadingIndicatorStatusChanged',
    };

    var invoiceTypes = {
        invoice: 1,
        creditNote: 2,
        bill: 3,
        payPal: 4
    };

    var vm = {
        eventNames: eventNames,
        settings: settings,
        maxUIColumns: 12,
        invoiceTypes: invoiceTypes
    };

    return vm;
})();