define([
    'modules/custom-validator',
    'services/test-material-data-service'
],
    function (customValidator, testMaterialService) {
        return {
            getInstance: getInstance
        };

        function getInstance(params) {
            var defaultParams = {
                summary: null,
                selectedTestSession: null
            };

            $.extend(defaultParams, params);

            var serverModel = {
                TestSpecificationId: ko.observable()
            };

            var emptyModel = ko.toJS(serverModel);

            var validator = customValidator.create(serverModel);

            serverModel.TestSpecificationId.subscribe(function (value) {
                vm.summary.TestSpecificationId(value);
            });

            var vm = {
                summary: defaultParams.summary,
                examiners: ko.observableArray(),
                readOnly: ko.observable(false),
                tableDefinition: {
                    searching: false,
                    paging: false,
                    oLanguage: { sInfoEmpty: '', sInfo: '' },
                    order: [
                        [1, "asc"]
                    ]
                }
            };

            vm.isValid = function () {
                var defer = Q.defer();

                testMaterialService.post(vm.summary.Request(), 'testSpecifications').then(function (data) {
                    validator.reset();

                    ko.utils.arrayForEach(data.InvalidFields,
                        function (i) {
                            validator.setValidation(i.FieldName, false, i.Message);
                        });

                    validator.isValid();

                    var isValid = !data.InvalidFields.length;
                    defer.resolve(isValid);
                    vm.readOnly(isValid);
                });

                return defer.promise;
            };

            vm.load = function () {
                vm.readOnly(false);
                validator.reset();
                ko.viewmodel.updateFromModel(serverModel, emptyModel);
                testMaterialService.post(vm.summary.Request(),'getExaminersAndRolePlayers').then(function (data) {
                    vm.examiners(data.Results);
                });
            };

            return vm;
        }
    });