define([
    'services/server-callback-processor',
    'durandal/app',
    'plugins/router'
], function (servercallbackprocessor, app, router) {
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

    return function (url) {
        url = window.baseUrl + url;
        return new Resource(url);
    };

    function cancelRequests() {
        $.abortableXhrPool.abortAll();
    }

    function escapeHTML(value) {
        if (!value) {
            return null;
        }
        else if ($.isPlainObject(value)) {
            for (var p in value) {
                if (typeof value[p] === 'string') {
                    value[p] = $("<div/>").text(value[p]).html();
                    value[p] = value[p].replace("&amp;", "&");
                    continue;
                }

                escapeHTML(value[p]);
            }
        }
        else if ($.isArray(value)) {
            for (var i = 0; i < value.length; i++) {
                if (typeof value[i] === 'string') {
                    value[i] = $("<div/>").text(value[i]).html();
                    value[i] = value[i].replace("&amp;", "&");
                    return;
                }

                escapeHTML(value[i]);
            }
        }
    }

    function removeProperties(object, propertiesToRemove) {
        if (!propertiesToRemove.length) {
            return object;
        }
        else if ($.isArray(object)) {
            for (var i = 0; i < object.length; i++) {
                removeProperties(object[i], propertiesToRemove);
            }
        }
        else if ($.isPlainObject(object)) {
            $.each(propertiesToRemove, function () {
                var property = this;
                var deep = property.split('.');
                var propertyName = deep[0];
                if (propertyName in object) {
                    if (deep.length > 1) {
                        removeProperties(object[propertyName], [property.substring(propertyName.length + 1)]);
                    }
                    else {
                        delete object[propertyName];
                    }
                }
            });
        }
    }

    function stringify(parameters, allowHtmlIn) {
        if (!parameters) {
            return null;
        }

        allowHtmlIn = allowHtmlIn || [];

        if (typeof parameters === 'string') {
            return escape(parameters);
        }

        var copy = $.extend(true, {}, parameters);
        removeProperties(copy, allowHtmlIn);
        escapeHTML(copy);

        if (allowHtmlIn.length) {
            var original = $.extend(true, {}, parameters);
            copy = $.extend(true, original, copy);
        }

        return JSON.stringify(copy);
    }

    /// Use ajaxBehaviour.allowHtmlInProperties as array to define properties that allows HTML tag. 
    /// - Use '.' to navigate in deep
    /// - The deep navigation is allowed in properties of array type
    /// - Eg: ajaxBehaviour.allowHtmlInProperties = ['a', 'a.b', 'a.b.c']
    function Resource(url) {
        this.get = function get(parameters, ajaxBehaviour) {
            return promiseAjaxCall('GET', url, parameters, ajaxBehaviour);
        }

        this.getFluid = function getFluid(parameters, queryString, ajaxBehaviour) {
            return promiseAjaxCall('GET', url + '/' + parameters, queryString, ajaxBehaviour);
        }

        this.download = function download(parameters, queryString) {
            var downloadUrl = url + '/' + parameters;
            if (queryString) {
                downloadUrl += '?' + $.param(queryString);
            }
            document.location.href = downloadUrl;
        }

        this.post = function post(parameters, action, ajaxBehaviour) {
            return promiseAjaxCall('POST', url + (action ? '/' + action : ''), stringify(parameters, (ajaxBehaviour || {}).allowHtmlInProperties), ajaxBehaviour);
        }

        this.put = function put(id, parameters, action, ajaxBehaviour) {
            return promiseAjaxCall('PUT', url + (action ? '/' + action : '') + '/' + id, stringify(parameters, (ajaxBehaviour || {}).allowHtmlInProperties), ajaxBehaviour);
        }

        this.remove = function remove(id, action, parameters, ajaxBehaviour) {
            return promiseAjaxCall('DELETE', url + (action ? '/' + action : '') + '/' + id, stringify(parameters, (ajaxBehaviour || {}).allowHtmlInProperties), ajaxBehaviour);
        }

        this.url = function () {
            return url;
        }

        function promiseAjaxCall(type, url, parameters, ajaxBehaviour) {
            var options = {
                type: type,
                url: url,
                cache: false,
                async: true,
                contentType: 'application/json; charset=utf-8',
                data: parameters,
                success: function (response) {
                    // detecting redirect to login page
                    if (typeof (response) === 'string' && response.indexOf('id="UserName" name="UserName"') != -1 && response.indexOf('id="Password" name="Password"') != -1) {
                        return window.location.href = window.baseUrl;
                    }

                    if (response && response.hasOwnProperty('data')) {
                        var data = response.data;
                        if (parameters && parameters.supressResponseMessages) {
                            response.messages = [];
                        }
                        servercallbackprocessor.showMessages(response.messages, response.warnings, response.errors);
                        defer.resolve(data);
                    } else {
                        defer.resolve(response);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    var obj = {
                        xhr: XMLHttpRequest,
                        text: textStatus
                    };
                    defer.reject(obj);
                    servercallbackprocessor.showError(XMLHttpRequest, textStatus, errorThrown);
                }
            };

            $.extend(options, ajaxBehaviour);

            var defer = Q.defer();
            var jqXhr = $.ajax(options);

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

            return defer.promise;
        }
    }
});
