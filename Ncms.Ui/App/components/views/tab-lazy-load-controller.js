define([
    'services/requester-manager-service',
    'services/util'
], function (requesterManager, util) {
    var self = this;

    self.generateTabs = function (params) {
        var tabs = ko.observableArray();
        var activeTab = ko.observable();

        function changeActiveTab(activeTab) {
            if (!activeTab) {
                return;
            }

            var active = false;
            ko.utils.arrayForEach(tabs(), function (t) {
                if (active) {
                    t.active(false);
                    return;
                }

                active = ko.toJS(t.id) == activeTab;
                t.active(active);
            });
        }

        function buildTabs() {
            var ts = tabs();
            for (var i = 0; i < ts.length; i++) {
                var tab = visibilityRule(ts[i]);
                tab.visible.subscribe(activeFirst);
            }

            tabs(ts);
        }

        function activeFirst() {
            if (activeTab()) {
                return;
            }

            var active = false;
            ko.utils.arrayForEach(tabs(), function (t) {
                if (active) {
                    t.active(false);
                    return;
                }

                active = t.visible() && (!('disabled' in t) || !t.disabled());
                t.active(active);
            });
        }

        function visibilityRule(t) {
            var hasRoles = 'roles' in t;
            var hasPermission = ko.observable(true);

            if (hasRoles) {
                hasPermission(false);
            }

            var visible = t.visible || ko.observable(true);
            if (!ko.isObservable(visible)) {
                visible = ko.observable(visible);
            }

            t.visible = ko.computed(function () {
                return visible() && hasPermission();
            });

            var active = t.active || ko.observable(false);
            if (!ko.isObservable(active)) {
                active = ko.observable(active);
            }

            t.active = ko.observable(active());
            active.subscribe(t.active);

            if (hasRoles) {
                checkPermission(t).then(hasPermission);
            }

            return t;
        }

        function checkPermission(t) {
            var defer = Q.defer();
            currentUser.hasPermission(t.roles).then(function (data) {
                requesterManager[!data ? 'blockRequestFrom' : 'unblockRequestFrom'](t.composition.model);
                defer.resolve(data);
            });
            return defer.promise;
        }

        if (!params.tabs) {
            return;
        }

        if (!ko.isObservable(params.tabs)) {
            tabs(params.tabs);
        }
        else {
            tabs = params.tabs;
        }

        if (!ko.isObservable(params.activeTab)) {
            activeTab(params.activeTab);
        }
        else {
            activeTab = params.activeTab;
        }

        activeTab.subscribe(changeActiveTab);
        buildTabs();
        changeActiveTab(activeTab());
        return tabs;
    }

    return self;
});
