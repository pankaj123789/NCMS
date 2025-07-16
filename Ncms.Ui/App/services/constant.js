//This will be auto load by bundle.config
/*jshint -W020 */
CONST = (function () {
    /*jshint +W020 */
    var eventNames = {
        cancelLoadingIndicator: 'cancelLoadingIndicator',
        cancelAjaxRequest: 'cancelAjaxRequest',
        showLoadingIndicator: 'showLoadingIndicator',
        loadingIndicatorStatusChanged: 'loadingIndicatorStatusChanged',
    };

    var settings = {
        shortDateDisplayFormat: 'DD/MM/YYYY',
        shortDateTimeDisplayFormat: 'DD/MM/YYYY LT',
        longDateTimeDisplayFormat: 'DD/MM/YYYY LTS',
        timeDisplayFormat: 'LT',
        yearMonthDayFormat: 'YYYY MM DD'
    };

    var invoiceTypes = {
        invoice: 1,
        creditNote: 2,
        bill: 3,
    };

    var invoiceStatuses = {
        draft: 0,
        open: 1,
        paid: 2,
        canceled: 3
    };

    var paymentTypes = {
        cash: 1,
        cheque: 2,
        eft: 3,
        amex: 4
    };

    var vm = {
        invoiceStatuses: invoiceStatuses,
        invoiceTypes: invoiceTypes,
        paymentTypes: paymentTypes,
        eventNames: eventNames,
        settings: settings,
    };

    return vm;
})();
