define([
    'services/util-data-service',
    'services/util',
], function (utilDataService, util) {
    service = utilDataService('api/personimage');
    service.newUrl = function (naatiNumber, request) {
        return this.url() + '/' + naatiNumber + '?_=' + util.guid() + (request ? "&" + $.param(request) : "");

    };
    return service;
});