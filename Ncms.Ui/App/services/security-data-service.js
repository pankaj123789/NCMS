define(['services/util-data-service'], function (utilDataService) {
    var service = utilDataService('api/security');
    service.get = function (parameters, ajaxBehaviour) {
        return service.ajax('GET', parameters, ajaxBehaviour);
    };
    return service;
});