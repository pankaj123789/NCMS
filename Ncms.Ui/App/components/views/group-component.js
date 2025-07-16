define([], function () {
    function ViewModel(params) {
           var self = this;
           params.component = self;
          self.componentOptions = params;

          for (var i = 0; i < self.componentOptions.length; i++) {
              var filter = self.componentOptions[i];
               filter.filterClass = filter.filterClass || 'col-lg-6';
               var filterOptions = filter.componentOptions;
               if (i > 0 && filterOptions.dependant) {
                   var optionsLoader = getOptionsLoader(filter);
                   var componentOptions = self.componentOptions[i - 1];
                   var parentFilterOptions = componentOptions.componentOptions;
                   parentFilterOptions.selectedOptions = ko.observableArray(componentOptions.data ? componentOptions.data.selectedOptions : null);
                   parentFilterOptions.selectedOptions.subscribe(optionsLoader);

                   filterOptions.disable = filter.componentOptions.disable || getDisableFunction(parentFilterOptions);
               }
          }

          self.getJson = function() {

              var jsonData = [];
              for (var i =0; i< self.componentOptions.length; i++) {
                  var filter = self.componentOptions[i];
                  jsonData.push({ id: filter.id, data: filter.componentOptions.component.getJson() });
              }
              return jsonData;
          }

          function getDisableFunction(filterOptions) {

              var currentFilterOptions = filterOptions;
              function hasSelectedOptions() {
                  return !currentFilterOptions.selectedOptions || !currentFilterOptions.selectedOptions().length;
              }

              return ko.pureComputed(hasSelectedOptions);
          }

        function getOptionsLoader(filter) {

            var currentFilter = filter;
            function loadOptions(selectedOptions) {

                if (!selectedOptions.length) {
                    currentFilter.componentOptions.options([]);
                    return;
                }

                var loader = currentFilter.componentOptions.optionsLoader;
                loader(selectedOptions).then(currentFilter.componentOptions.options);
            };

            return loadOptions;
        }
    }

    return ViewModel;
});