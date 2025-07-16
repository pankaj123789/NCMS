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
            };

            $.extend(defaultParams, params);

            var serverModel = {
                SkillId: ko.observable()
            };

            var emptyModel = ko.toJS(serverModel);

			var validator = customValidator.create(serverModel);

            serverModel.SkillId.subscribe(function (value) {
                vm.summary.SkillId(value);
			});

            var vm = {
                summary: defaultParams.summary,
                model: serverModel,
                skills : ko.observableArray(),
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
                        select: selectSkill,
                        deselect: selectSkill,
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

                testSessionService.post(vm.summary.Request(), 'allocateroleplayerswizard/testSessionSkill').then(function (data) {
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
                testSessionService.post(vm.summary.Request(),'allocateroleplayerswizard/getTestSessionSkills').then(function (data) {
					vm.skills(data.Results);
				});
            };

            function selectSkill(e, dt) {
                var indexes = dt.rows('.selected').indexes();

                if (!indexes.length) {
                    return serverModel.SkillId(null);
                }

                validator.reset();
				var skill = vm.skills()[indexes[0]];
                serverModel.SkillId(skill.SkillId);
            }

            vm.postData = function () {
                return { SkillId: ko.toJS(vm.model.SkillId) };
            };

            return vm;
        }
    });