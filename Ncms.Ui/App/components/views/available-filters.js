define([], function () {
    function addCssClass(css, classToAdd) {
        if (!~css().indexOf(classToAdd)) {
            css(classToAdd + ' ' + css());
        }
    }

    function ViewModel(params) {
        var self = this;

        function bindDefaultFilters() {
            ko.utils.arrayForEach(self.initialFilters(), function (initialFilter) {
                if (initialFilter.isDefault) {
                    self.select(initialFilter);
                }
            });
        }

        var defaultParams = {
            filters: ko.observableArray([]),
            controller: null
        }

        $.extend(defaultParams, params);

        ko.utils.arrayForEach(defaultParams.filters(), function (filter) {
            filter.css = filter.css || ko.observable('');

            addCssClass(filter.css, 'filter');

            if (filter.required) {
                addCssClass(filter.css, 'required');
            }
        });

        self.initialFilters = ko.observableArray(defaultParams.filters.slice());
        self.currentFilters = ko.observableArray(self.initialFilters.slice());
        self.controller = defaultParams.controller;

        if (self.controller) {
            self.controller.availableFiltersViewModel = self;
        }

        self.add = function (filterId) {
            var filter = ko.utils.arrayFirst(self.initialFilters(), function (initialFilter) {
                return initialFilter.id === filterId;
            });

            if (filter && self.currentFilters.indexOf(filter) == -1) {
                self.currentFilters.unshift(filter);
            }
        }

        self.clear = function () {
            self.currentFilters(self.initialFilters.slice());
        }

        self.select = function (filter, data) {
            var remove = false;

            if (self.controller) {
                remove = self.controller.select(filter, data);
            }

            if (remove) {
                self.currentFilters.remove(function (currentFilter) { return currentFilter.id === filter.id; });
            }
        }

        bindDefaultFilters();
    };

    return ViewModel;
});
