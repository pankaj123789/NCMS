define(['services/requester'],
    function(requester) {

        return function(url) {
			return new QueuedResource(url);
        };

        function QueuedResource(url) {
        	var service = requester(url);
        	var promises = [];

        	function queueQuery(promiseFunction) {
        	    promises.push(promiseFunction);

        	    var defer = Q.defer();

        	    var interval = setInterval(function () {
        	        if (promises[0] === promiseFunction) {
        	            clearInterval(interval);
        	            promiseFunction().then(function (data) {
        	                promises.shift();
        	                defer.resolve(data);
        	            }, function() {
        	                promises.shift();
        	                defer.reject();
        	            });
        	        }
        	    }, 100);

        	    return defer.promise;
            }

            this.get = function get(parameters, ajaxBehaviour) {

                return queueQuery(function() { return service.get(parameters, ajaxBehaviour) });
            }

            this.getFluid = function getFluid(parameters, queryString, ajaxBehaviour) {
            	return queueQuery(function () { return service.getFluid(parameters, queryString, ajaxBehaviour) });
            }

            this.download = function download(parameters, queryString) {
                service.download(parameters, queryString);
            }

            this.post = function post(parameters, action, ajaxBehaviour) {
            	return queueQuery(function () { return service.post(parameters, action, ajaxBehaviour) });
            }

            this.put = function put(id, parameters, action, ajaxBehaviour) {
            	return queueQuery(function () { return service.put(id, parameters, action, ajaxBehaviour) });
            }

            this.remove = function remove(id, parameters, ajaxBehaviour) {
            	return queueQuery(function () { return service.remove(id, parameters, ajaxBehaviour) });
            }

            this.url = function () {

                return service.url;
            }
        }
    });