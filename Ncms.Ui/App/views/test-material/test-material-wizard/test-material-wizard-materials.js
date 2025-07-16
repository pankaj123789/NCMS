define([
	'modules/custom-validator',
	'services/test-material-data-service',
	'modules/enums',
	'services/util'
],
	function (customValidator, testMaterialService, enums, util) {
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
				TestMaterialAssignments: ko.observableArray()
			};

			var emptyModel = ko.toJS(serverModel);

			var validator = customValidator.create(serverModel);

			var vm = {
				summary: defaultParams.summary,
				model: serverModel,
				taskType: ko.observable(),
				testMaterials: ko.observableArray(),
				tasks: ko.observableArray(),
				readOnly: ko.observable(false),
				draggingTypeId: ko.observable(),
			};

			ko.computed(function () {
				var testMaterialAssignments = [];

				ko.utils.arrayForEach(vm.tasks(), function (t) {
					ko.utils.arrayForEach(t.TestMaterials(), function (tm) {
						testMaterialAssignments.push({
							TestTaskId: t.Id,
							TestMaterialId: tm.Id
						});
					});
				});

				serverModel.TestMaterialAssignments(testMaterialAssignments);
			});

			serverModel.TestMaterialAssignments.subscribe(function () {
				vm.summary.TestMaterialAssignments(serverModel.TestMaterialAssignments());
			});

			vm.drop = function (data, model) {
				var testMaterial = ko.utils.arrayFirst(vm.testMaterials(), function (tm) {
					return tm.Id === data.Id;
				});

				if (testMaterial) {
					return;
				}

				vm.testMaterials.push(data);
				vm.taskType(data.TypeId);

				if (!data.Task) {
					return;
				}

				data.Task.TestMaterials.remove(data);
				data.Task = null;
			};

			vm.taskTypes = ko.computed(function () {
				var taskTypes = [];

				ko.utils.arrayForEach(vm.testMaterials(), function (tm) {
					var taskType = ko.utils.arrayFirst(taskTypes, function (tt) {
						return tt.TypeId === tm.TypeId;
					});

					if (taskType) {
						return;
					}

					taskTypes.push({
						TypeId: tm.TypeId,
						TypeLabel: '{0}: {1}'.format(tm.TypeLabel, tm.TypeDescription),
						TestMaterialStatusType: ko.observable(tm.StatusId),
					});
				});

				return taskTypes.sort(util.sortBy('TypeId'));
			});

			vm.testMaterialsFromTaskType = ko.computed(function () {
				return ko.utils.arrayFilter(vm.testMaterials(), function (tm) {
					return tm.TypeId === vm.taskType();
				});
			});

			vm.selectedTaskType = function () {
				return ko.utils.arrayFirst(vm.taskTypes(), function (tt) {
					return vm.taskType() === tt.TypeId;
				});
			};

			vm.testMaterialsFromTaskTypeAndStatusType = ko.computed(function () {
				var selectedTaskType = vm.selectedTaskType();
				var testMaterials = ko.utils.arrayFilter(vm.testMaterialsFromTaskType(), function (tm) {
					return tm.StatusId <= (selectedTaskType ? selectedTaskType.TestMaterialStatusType() : -1);
				});
				return testMaterials.sort(util.sortBy('StatusId', 'ApplicantsRangeTypeId', 'Id'));
			});

			vm.showMoreButton = ko.computed(function () {
				return vm.testMaterialsFromTaskType().length != vm.testMaterialsFromTaskTypeAndStatusType().length;
			});

			vm.taskTypeOptions = {
				value: vm.taskType,
				multiple: false,
				options: vm.taskTypes,
				optionsValue: 'TypeId',
				optionsText: 'TypeLabel',
			};

			vm.isValid = function () {
				var defer = Q.defer();

				testMaterialService.post(vm.summary.Request(), 'testSpecificationMaterials').then(function (data) {
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

				loadMaterials();
				loadTasks();
			};

			vm.showMore = function () {
				var taskType = vm.selectedTaskType();
				if (!taskType || taskType.TestMaterialStatusType() === enums.TestMaterialStatusType.UsedByApplicants) {
					return;
				}

				var testMaterialsLength = vm.testMaterialsFromTaskTypeAndStatusType().length;
				taskType.TestMaterialStatusType(taskType.TestMaterialStatusType() + 1);

				if (testMaterialsLength === vm.testMaterialsFromTaskTypeAndStatusType().length) {
					vm.showMore();
				}
			};

			vm.dragStart = function (item) {
				if (vm.readOnly()) {
					return false;
				}

				vm.draggingTypeId(item.TypeId);
			};

			vm.dragEnd = function () {
				vm.draggingTypeId(null);
			};

			return vm;

			function loadTasks() {
                testMaterialService.post(vm.summary.Request(),'getTestSpecificationTasks').then(function (data) {
					var tasks = ko.viewmodel.fromModel(data.Results);
					ko.utils.arrayForEach(tasks(), function (t) {
						t.Selected = ko.observable(false);
						t.TestMaterials = ko.observableArray();
						t.AllowDrop = ko.computed(function () {
							return !vm.draggingTypeId() || (!t.TestMaterials().length && vm.draggingTypeId() === t.TypeId());
						});
						t.Css = ko.computed(function () {
							if (t.TestMaterials().length) {
								return 'panel-success';
							}
							if (!t.TestMaterials().length) {
								return 'panel-danger';
							}
						});
						t.drop = function (data, model) {
							if (data.TypeId !== model.TypeId() || t.TestMaterials().length) {
								return;
							}

							vm.testMaterials.remove(data);
							if (data.Task) {
								data.Task.TestMaterials.remove(data);
							}

							data.Task = model;
							model.TestMaterials.push(data);
						};
					});
					vm.tasks(tasks());
				});
			}

			function loadMaterials() {
				var req = $.extend(vm.summary.Request(), { Skip: 0, Take: 1000 });
                testMaterialService.post(req,'getTestSpecificationMaterials').then(function (data) {
                    ko.utils.arrayForEach(data.Results, function (tm) {

                        tm.BadgeTooltip = util.getTestMaterialStatusToolTip(tm.StatusId, tm.LastUsedDate);
                        tm.BadgeColor = util.getTestMaterialStatusColor(tm.StatusId);
                        tm.BadgeText = util.getTestMaterialStatusText(tm.StatusId);
						tm.DomainBadgeColor = util.getTestMaterialDomainColor(tm.TestMaterialDomainId);
						tm.IsSourceTestMaterialType = tm.TestMaterialTypeId == enums.TestMaterialTypeName.Source;
						tm.IsSourceTestMaterialTypeText = ko.Localization('Naati.Resources.TestMaterial.resources.Source');
						tm.ApplicantsRangeType = '';
						for (var i in enums.ApplicantsRangeType) {
							if (enums.ApplicantsRangeType[i] === tm.ApplicantsRangeTypeId) {
								tm.ApplicantsRangeType = ko.Localization('Naati.Resources.ApplicantsRangeType.resources.{0}'.format(i));
							}
						}
					});

					vm.testMaterials(data.Results);
				});
			}
		}
	});