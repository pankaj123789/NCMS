define([], function () {
    var self = {
        register: register,
        unregister: unregister,
        locked: locked
    };

    var hub = $.connection.lockedRegistryHub;
    var client = hub.client;
    var server = hub.server;
    var connected = false;

    // Create a function that the hub can call back to display messages.
    client.no = function () {
        $(self).trigger('no');
    };

    client.yes = function () {
        $(self).trigger('yes');
    };

    $.connection.hub.stateChanged(connectionStateChanged);
    $.connection.hub.start();

    return self;

    function connectionStateChanged(state) {
        connected = state.newState == 1;
    }

    function register(name, id) {
        executeOrRetry(function () {
            server.register(name, id);
        });
    }

    function unregister(name, id) {
        executeOrRetry(function () {
            server.unregister(name, id);
        });
    }

    function locked(name, id) {
        executeOrRetry(function () {
            server.locked(name, id);
        });
    }

    function editing(name, id) {
        executeOrRetry(function () {
            server.locked(name, id);
        });
    }

    function executeOrRetry(callback, count) {
        count = count || 4;
        if (!connected) {
            if (count > 1) {
                setTimeout(function () {
                    executeOrRetry(callback, --count);
                }, 500);
            }
            return;
        };
        callback();
    }
});