define([], function () {
    function ViewModel(params) {
        var self = this;

        self.filterId = params.id;
        self.filterName = params.name;
        self.componentName = params.component;
        self.componentOptions = params.componentOptions;
        self.controller = params.componentOptions.controller;
        self.filterClass = params.filterClass || ko.observable('col-lg-2');
        self.helpText = params.helpText || ko.observable();
        self.showLabel = $.extend({ showLabel: ko.observable(true) }, { showLabel: params.showLabel }).showLabel;
        self.isDefault = params.isDefault;

        self.css = ko.pureComputed(function () {
            return 'filter ' + self.filterClass();
        });

        if (self.controller) {
            self.controller.getFilterJson = self.controller.getFilterJson || {};
            self.controller.getFilterJson[self.filterId] = function () {
                if (self.componentOptions.component && self.componentOptions.component.getJson) {
                    return self.componentOptions.component.getJson();
                }

                return {};
            }

            self.controller.validateFilter = self.controller.validateFilter || {};
            self.controller.validateFilter[self.filterId] = function () {
                if (self.componentOptions.component && self.componentOptions.component.validateFilter) {
                    return self.componentOptions.component.validateFilter();
                }

                return true;
            }
        }

        self.remove = function () {
            if (self.controller) {
                self.controller.remove({ id: self.filterId });
            }
        };
    };

    return ViewModel;
});
