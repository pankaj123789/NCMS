define([], function () {
    function viewModel(params) {
        var self = this;

        self.tabs = ko.observableArray([]);
        self.tabContainerId = null;
        self.type = 'tabs';
        self.event = {};

        $.extend(self, params);

        $.extend(self, {
            validate: function () {
                return ko.utils.arrayFilter(self.tabs(), function (tab) {
                    return tab.validate ? !tab.validate() : false;
                }).length === 0;
            },
            clearValidation: function () {
                ko.utils.arrayForEach(self.tabs(), function (tab) {
                    tab.clearValidation && tab.clearValidation();
                });
            },
            save: function () {
                var promises = [];

                ko.utils.arrayForEach(self.tabs(), function (tab) {
                    if (tab.save) {
                        var value = tab.save();

                        if (Q.isPromise(value)) {
                            promises.push(value);
                        }
                    }
                });

                return Q.allSettled(promises);
            },
            tabsComputed: ko.pureComputed(function () {
                return ko.utils.arrayMap(self.tabs(), function (t) {
                    var baseTab = {
                        id: null,
                        label: null,
                        active: false,
                        click: null,
                        icon: null,
                        composition: null,
                        disabled: ko.observable(false),
                        tabs: [],
                        tabTemplate: null,
                        template: null,
                        type: 'template',
                        visible: ko.observable(true),
                        valid: ko.observable(true),
                        tooltip: ''
                    };

                    var tabOptions = $.extend({}, baseTab, t);
                    var tab = $.extend({}, t, tabOptions);

                    $.each(tab.tabs, function (i, it) {
                        tabOptions = $.extend({}, baseTab, it);
                        $.extend(it, tabOptions);
                    });

                    return tab;
                });
            })
        });

        params.component = self;
    }

    return viewModel;
});
