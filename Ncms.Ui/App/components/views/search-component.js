define([
    'components/views/custom-search'
], function (customSearch) {
    function loadTemplate() {
        var defer = Q.defer();

        require(['text!components/views/search-component.html'], function (template) {
            $('body').append(template);
            defer.resolve(template);
        });

        return defer.promise;
    }

    function ViewModel(params) {
        var self = this;

        var defaultParams = {
            tableDefinition: {}
        };

        $.extend(true, defaultParams, params); // Perform deep extend in case params already has a controller object

        self.tableComponent = {
            name: 'table-component',
            params: defaultParams.tableDefinition
        };

        var templateAttached = $('#tableTemplate').length != 0;
        self.isLoaded = ko.observable(templateAttached);

        defaultParams.resultTemplate = {
            'if': self.isLoaded,
            name: 'tableTemplate',
            data: self
        }

        defaultParams.tableDefinition.dataTable.source.subscribe(function () {
            $("#" + defaultParams.tableDefinition.id).hide();
        }, null, "beforeChange");

        defaultParams.tableDefinition.dataTable.initComplete = function () {
            $("#" + defaultParams.tableDefinition.id).show();
        };

        customSearch.call(self, defaultParams);

        if (!self.isLoaded()) {
            loadTemplate().then(function () {
                self.isLoaded(true);
            });
        }
    };

    return ViewModel;
});
