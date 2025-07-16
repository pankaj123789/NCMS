define([
    'services/system-data-service'
], function (systemService) {
        return {
            getInstance: getInstance
        };

    function getInstance(systemValueConfigs) {

            var vm = {
                systemValueConfigs: systemValueConfigs,
                systemValuesReady: ko.observable(false)
            };

            getSystemValues();

            vm.save = function (systemValueConfig) {
                var request = systemValueConfig.SystemValues;

                systemService.post(request, 'postUpdatedSystemValues').then(function (data) {
                    if (data.InvalidFields.length) {
                        systemValueConfig.validationErrors(data.InvalidFields);
                        systemValueConfig.validationError(true);
                        return;
                    }

                    toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                    systemValueConfig.validationError(false);
                });

                
            }

            function getSystemValues() {
                var valuesToProcess = 0;
                var valuesProcessed = 0;
                var systemValuePromise =
                    new Promise((resolve, reject) => {
                        ko.utils.arrayForEach(vm.systemValueConfigs, function (item) {
                            item.validationError = ko.observable(false);
                            item.validationErrors = ko.observableArray([]);

                            valuesToProcess += item.SystemValues.length;

                            item.SystemValues.forEach(function (item) {
                                systemService.getFluid("value/{0}".format(item.Key)).then(function (value) {
                                    if (item.DataType != "bool") {
                                        item.Value = value;
                                    }
                                    else if (value == "1" || value.toLowerCase() == "true") {
                                        item.Value = true
                                    }
                                    else if (value == "0" || value.toLowerCase() == "false") {
                                        item.Value = false;
                                    }
                                    else {
                                        item.Value = value;
                                    }
                                    valuesProcessed++;

                                    if (valuesProcessed == valuesToProcess) {
                                        resolve(true);
                                    }
                                })
                            });
                        });
                    });
                        

                systemValuePromise.then((value) => {
                    vm.systemValuesReady(value);
                });
            }

            return vm;
        }
});