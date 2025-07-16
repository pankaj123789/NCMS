define([
], function () {
    var self = this;
    var blockedInstances = [];

    self.getBlockedInstances = function () {
        return blockedInstances.slice();
    }

    self.unblockRequestFrom = function (instance) {
        var i = ko.utils.arrayFirst(blockedInstances, function (b) {
            return b.instance == instance;
        });

        if (i) {
            blockedInstances.splice(index, 1);
        }
    }

    self.blockRequestFrom = function (instance) {
        if (self.canRequest(instance)) {
            var blockedInstance = { instance: instance, functions: getAllFuncs(instance) };
            blockedInstances.push(blockedInstance);
        }
    }

    self.canRequest = function (instance) {
        return blockedInstances.indexOf(instance) == -1;
    }

    function getAllFuncs(toCheck) {
        var props = [];
        var obj = toCheck;
        var props = Object.getOwnPropertyNames(obj);
        var funcs = [];
        for (var i = 0; i < props.length; i++) {
            var prop = props[i];
            if (prop != props[i + 1] && typeof toCheck[prop] == 'function') {
                funcs.push(toCheck[prop]);
            }
        }
        return funcs;
    }

    return self;
});
