define([
        'modules/custom-validator',
        'services/testsession-data-service'
],
    function (customValidator,  testSessionService) {
        return {
            getInstance: getInstance
        };

        function getInstance(params) {
            var defaultParams = {
                summary: null,
                selectedTestSpecification: null
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
                        select: selectTestSpecificaiton,
                        deselect: selectTestSpecificaiton,
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

                testSessionService.post(vm.summary.Request(), 'allocateroleplayerswizard/testSpecifications').then(function (data) {
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
                testSessionService.post(vm.summary.Request(),'allocateroleplayerswizard/getTestSpecifications').then(function (data) {
					vm.testSpecifications(data.Results);
				});
            };

            function selectTestSpecificaiton(e, dt) {
                var indexes = dt.rows('.selected').indexes();

                if (!indexes.length) {
					return serverModel.TestSpecificationId(null);
                }

                validator.reset();
				var testSpecification = vm.testSpecifications()[indexes[0]];
				serverModel.TestSpecificationId(testSpecification.Id);
            }

            vm.postData = function () {
                return {
                    TestSpecificationId: ko.toJS(vm.model.TestSpecificationId)};
            };

            return vm;
        }
    });