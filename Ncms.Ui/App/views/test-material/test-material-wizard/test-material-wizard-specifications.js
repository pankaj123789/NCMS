define([
        'modules/custom-validator',
        'services/test-material-data-service'
],
    function (customValidator,  testMaterialService) {
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
                model: serverModel,
                testSpecifications: ko.observableArray(),
                venues: ko.observableArray(),
                readOnly: ko.observable(false),
                tableDefinition: {
                    columnDefs: [
                        {
                            orderable: false,
                            className: 'select-checkbox',
                            targets: 0
                        }
                    ],
                    order: [
                        [1, "asc"]
                    ],
                    select: {
                        style: 'single',
                        info: false
                    },
                    events: {
                        select: selectTestSession,
                        deselect: selectTestSession,
                        'user-select': function (e) {
                            if (vm.readOnly()) {
                                e.preventDefault();
                            }
                        }
                    }
                },
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
                testMaterialService.post(vm.summary.Request(),'getTestSpecifications').then(function (data) {
					vm.testSpecifications(data.Results);
				});
            };

            function selectTestSession(e, dt) {
                var indexes = dt.rows('.selected').indexes();

                if (!indexes.length) {
					return serverModel.TestSpecificationId(null);
                }

                validator.reset();
				var testSpecification = vm.testSpecifications()[indexes[0]];
				serverModel.TestSpecificationId(testSpecification.Id);
            }

            return vm;
        }
    });