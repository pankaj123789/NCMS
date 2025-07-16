define([
    'services/requester-manager-service',
    'services/servercallbackprocessor',
    'services/util',
    'durandal/app',
    'plugins/router'
], function (requesterManager, servercallbackprocessor, util, app, router) {
    app.on(CONST.eventNames.cancelAjaxRequest).then(cancelRequests);
    router.on('router:navigation:processing').then(cancelRequests);

    // these are xhr requests that are aborted when we navigate.
    $.abortableXhrPool = [];

    // these are xhr requests for which we can only have one at a time.
    $.singularXhrPool = {};

    $.abortableXhrPool.abortAll = function () {
        $(this).each(function (i, jqXhr) {   //  cycle through list of recorded connection
            jqXhr.abort();  //  aborts connection
            $.abortableXhrPool.splice(i, 1); //  removes from list by index

            // remove the item from our singularXhrPool
            for (var xhrKey in $.singularXhrPool) {
                if ($.singularXhrPool.hasOwnProperty(xhrKey)) {
                    if ($.singularXhrPool[xhrKey] == jqXhr) {
                        delete $.singularXhrPool[xhrKey];
                    }
                }
            }
        });
    }

    $.ajaxSetup({
        complete: function (jqXhr) {
            var i = $.abortableXhrPool.indexOf(jqXhr);
            if (i > -1) $.abortableXhrPool.splice(i, 1);

            // remove the item from our singularXhrPool
            for (var xhrKey in $.singularXhrPool) {
                if ($.singularXhrPool.hasOwnProperty(xhrKey)) {
                    if ($.singularXhrPool[xhrKey] == jqXhr) {
                        delete $.singularXhrPool[xhrKey];
                    }
                }
            }
        }
    });

    var getUrl = window.location;
    window.baseUrl = getUrl.protocol + '//' + getUrl.host + '/' + getUrl.pathname.split('/')[1] + '/';

    return function (url) {
        url = window.baseUrl + url;
        return new Resource(url);
    };

    function cancelRequests() {
        $.abortableXhrPool.abortAll();
    }

    function Resource(url) {
        var self = this;
        self.parentInstanceId = null;

        self.get = function get(parameters, ajaxBehaviour) {
            return promiseAjaxCall('GET', url, parameters, ajaxBehaviour);
        }

        self.getFluid = function getFluid(parameters, queryString, ajaxBehaviour) {
            return promiseAjaxCall('GET', url + '/' + parameters, queryString, ajaxBehaviour);
        }

        self.download = function download(parameters, queryString) {
            var downloadUrl = url + '/' + parameters;
            if (queryString) {
                downloadUrl += '?' + $.param(queryString);
            }
            document.location.href = downloadUrl;
        }

        self.post = function post(parameters, action, ajaxBehaviour) {
            return promiseAjaxCall('POST', url + (action ? '/' + action : ''), JSON.stringify(parameters), ajaxBehaviour);
        }

        self.put = function put(id, parameters, action, ajaxBehaviour) {
            return promiseAjaxCall('PUT', url + (action ? '/' + action : '') + '/' + id, JSON.stringify(parameters), ajaxBehaviour);
        }

        self.remove = function remove(id, parameters, ajaxBehaviour) {
            return promiseAjaxCall('DELETE', url + '/' + id, JSON.stringify(parameters), ajaxBehaviour);
        }

        self.url = function () {
            return url;
        }

        self.ajax = function (type, parameters, ajaxBehaviour) {
            return ajax(type, url, parameters, ajaxBehaviour);
        }

        function ajax (type, url, parameters, ajaxBehaviour, defer) {
            function success (response) {
                if (response && response.hasOwnProperty('data')) {
                    var data = response.data;
                    if (parameters && parameters.supressResponseMessages) {
                        response.messages = [];
                    }
                    servercallbackprocessor.showMessages(response.messages, response.warnings, response.errors);
                    if (defer) {
                        defer.resolve(data);
                    }
                    else {
                        return data;
                    }
                } else {
                    if (defer) {
                        defer.resolve(response);
                    }
                }
            }

            function error (XMLHttpRequest, textStatus, errorThrown) {
                var obj = {
                    xhr: XMLHttpRequest,
                    text: textStatus
                };
                if (defer) {
                    defer.reject(obj);
                }
                var supressResponseMessages = parameters && parameters.supressResponseMessages;
                if (!supressResponseMessages) {
                    servercallbackprocessor.showError(XMLHttpRequest, textStatus, errorThrown);
                }
            }

            var options = {
                type: type,
                url: url,
                cache: false,
                async: defer ? true : false,
                contentType: 'application/json; charset=utf-8',
                data: parameters,
                timeout:900000
            };

            if (defer) {
                options.success = success;
                options.error = error;
            }

            $.extend(options, ajaxBehaviour);

            var jqXhr = $.ajax(options);

            if (!defer) {
                if (jqXhr.status != 200) {
                    error(jqXhr, jqXhr.statusText);
                }
                return success(jqXhr.responseJSON);
            }

            if (options.singular) {
                // abort existing requests
                if ($.singularXhrPool[url]) {
                    $.singularXhrPool[url].abort();
                }

                $.singularXhrPool[url] = jqXhr;
            }

            if (options.abortable) {
                // add to abort pool
                $.abortableXhrPool.push(jqXhr);
            }
        }

        function promiseAjaxCall(type, url, parameters, ajaxBehaviour) {
            var defer = Q.defer();

            if (util.callerIn(requesterManager.getBlockedInstances())) {
                return defer.promise;
            }

            ajax(type, url, parameters, ajaxBehaviour, defer);

            return defer.promise;
        }
    }
});
