define([
	'services/testsession-data-service',
	'services/util',
	'modules/custom-validator',
	'modules/enums'
],
	function (testsessionService, util, customValidator, enums) {
		var SourceNameFormat = '{0} ({1}) - {2} {3}';

		return {
			getInstance: getInstance
		};

		function getInstance(params) {
			var componentValidations = {};

			var defaultParams = {
				summary: null
			};

			$.extend(defaultParams, params);

			var serverModel = {
				Skill: ko.observable(),
				Tasks: ko.observableArray(),
				RolePlayers: ko.observableArray()
			};

			var testSessionModel = {
				TestLocationId: ko.observable(),
				VenueId: ko.observable()
			};

			var vm = {
				summary: defaultParams.summary,
				availableRolePlayers: ko.observableArray(),
				settings: serverModel,
				submitted: ko.observable(false),
				validated: ko.observable(false),

			};

			var validator = customValidator.create(serverModel);

            vm.load = function () {
                vm.validated(false);
				vm.readOnly(false);
				validator.reset();

				testsessionService.getFluid(vm.summary.TestSessionId() + '/topsection').then(updateFromModel(testSessionModel));

				testsessionService.post(vm.summary.Request(), 'allocateroleplayerswizard/AvailableAndExistingRolePlayers').then(function (data) {
					ko.utils.arrayForEach(data, function (rolePlayer) {
						var languages = [];

						if (rolePlayer.LanguageIds.indexOf(serverModel.Skill().Language1Id) >= 0) {
							languages.push(serverModel.Skill().Language1DisplayName);
						}
						if (rolePlayer.LanguageIds.indexOf(serverModel.Skill().Language2Id) >= 0) {
							languages.push(serverModel.Skill().Language2DisplayName);
						}

						rolePlayer.MatchTestLocation = ko.utils.arrayFirst(rolePlayer.AvailableTestLocations, function (a) {
							return a.Id == testSessionModel.TestLocationId();
						});

						var notMatchTestLocation = ko.utils.arrayFilter(rolePlayer.AvailableTestLocations, function (a) {
							return a.Id != testSessionModel.TestLocationId();
						});

						rolePlayer.NotMatchTestLocationTooltip = null;

						if (notMatchTestLocation.length) {
                            notMatchTestLocation = util.distinctBy(notMatchTestLocation, 'DisplayName');
                            rolePlayer.NotMatchTestLocationTooltip = {
								trigger: 'hover',
								html: true,
								content: function () {
									var locations = ko.utils.arrayMap(notMatchTestLocation, function (l) {
										return l.DisplayName;
									});
									return '<ul class="m-l-n-md"><li>' + locations.join('</li><li>') + '</li></ul>';
								},
								placement: 'left'
							};
						}

						rolePlayer.GenderDescription = ko.Localization('Naati.Resources.Shared.resources.' + (rolePlayer.Gender == 'M' ? 'Male' : (rolePlayer.Gender == 'F' ? 'Female' : 'Unspecified')));
						rolePlayer.OriginalDetails = rolePlayer.Details.slice();
						rolePlayer.Details = ko.observableArray(rolePlayer.Details);
						rolePlayer.Languages = languages.join(', ');
						rolePlayer.RatingDisplay = '(' + (rolePlayer.Rating ? $.formatNumber(rolePlayer.Rating, { format: "##.0", locale: "us" }) : '0') + '/10)';
						rolePlayer.AssignedTasks = ko.computed(function () {
							var tasks = ko.utils.arrayMap(rolePlayer.Details(), function (d) {
								var task = ko.utils.arrayFirst(serverModel.Tasks(), function (t) {
									return t.Id() == d.TestComponentId;
								});

								return "{0}{1}".format(task.TypeLabel(), task.TaskLabel());
							});
							return tasks.join(', ');
						});

						var status = '';

						for (var st in enums.RolePlayerStatusType) {
							var rolePlayerStatusType = enums.RolePlayerStatusType[st];
							if (rolePlayerStatusType == rolePlayer.RolePlayerStatusId) {
								status = st;
								break;
							}
						}

						rolePlayer.Status = status;
					});

					vm.availableRolePlayers(data);
				});
				testsessionService.post(vm.summary.Request(), 'allocateroleplayerswizard/getAllocationDetails').then(updateFromModel(serverModel));
			};

			vm.isValid = function () {
                vm.submitted(true);

			    var defer = Q.defer();

				var req = vm.summary.Request();
				req.RolePlayers = vm.postData().RolePlayers;

				testsessionService.post(req, 'allocateroleplayerswizard/allocateroleplayers').then(function (data) {
					validator.reset();

					ko.utils.arrayForEach(data, function (i) {
						validator.setValidation(i.FieldName, false, i.Message);
                    });
				    var validTabsOptions = vm.tabOptions.component.validate();

                    if (!validTabsOptions) {
                        validator.setValidation("RolePlayers", false, ko.Localization('Naati.Resources.TestSession.resources.RolePlayerAsignmentIncomplete'));
                       
                    }


				    validator.isValid();

                    var isValid = !data.length && (validTabsOptions || vm.validated());
                    vm.validated(true);

					defer.resolve(isValid);
					vm.readOnly(isValid);
				});

				return defer.promise;
			};

			vm.postData = function () {
				var req = ko.utils.arrayMap(serverModel.RolePlayers(), function (rp) {
					var value = $.extend({}, rp);
					value.Details = rp.Details();
					delete value.NotMatchTestLocationTooltip;
					return value;
				});
				return { RolePlayers: req }; // TODO: Return entire roleplayer model
			};

			vm.readOnly = ko.observable(false);
			serverModel.Tasks.subscribe(bindTabs);

			vm.tabOptions = {
				tabs: ko.observableArray()
            };

            vm.rolePlayerStatusClass = util.getRolePlayerStatusCss;


			function bindTabs() {
				componentValidations = {};

				var active = true;
				var tabs = ko.utils.arrayMap(serverModel.Tasks(), function (component) {
					var data = getTemplateData(component);
					var tab = {
						active: active,
						id: util.guid(),
						label: ko.computed(function () {
							var label = "{0}{1}".format(component.TypeLabel(), component.TaskLabel());
							return label;
						}),
						tooltip: component.TaskName(),
						type: 'template',
						template: {
							name: 'allocate-role-player-task',
							data: data
						},
						valid: function () {
							return componentValidations[component.Id()].isValid();
						},
						validate: function () {
							return componentValidations[component.Id()].isValid();
						},
						clearValidation: function () {
							if (componentValidations[component.Id()].errors) {
								return componentValidations[component.Id()].errors.showAllMessages(false);
							}
						}
					};

					active = false;
					return tab;
				});

				vm.tabOptions.tabs(tabs);
			}

			function updateFromModel(observable) {
				return function (data) {
					ko.viewmodel.updateFromModel(observable, data);
				};
			}

			function getTemplateData(component) {
				vm.availableRolePlayers.subscribe(buildRolePlayerLists);

				var showOutOfArea = ko.observable(false);

				function filterByShowOutOfArea(observable) {
					return ko.computed(function () {
						if (showOutOfArea()) {
							return observable().sort(util.sortBy('DisplayOrder'));
						}

						return ko.utils.arrayFilter(observable(), function (o) {
							return o.IsInTestLocation;
						}).sort(util.sortBy('DisplayOrder'));
					});
				}

				function sort(observable) {
					return ko.computed(function () {
						return observable().sort(util.sortBy('DisplayOrder'));
					});
				}

				var showLanguage2 = ko.computed(function () {
					if (!serverModel.Skill()) return false;
					return serverModel.Skill().Language1Id != serverModel.Skill().Language2Id;
				});

				var lists = {
					elegibleRolePlayers: ko.observableArray(),
                    assignedRolePlayers1Primary: ko.observableArray().extend({
					    validation: {
					        validator: function (val) {
					            return !vm.submitted() ? true : val && val.length != 0;
					        },
					        message: ko.Localization('Naati.Resources.TestSession.resources.PrimaryRolePlayerForLanguage1Required')
					    }
					}),
					assignedRolePlayers2Primary: ko.observableArray().extend({
					    validation: {
					        validator: function (val) {
					            return !showLanguage2() ? true :
					                !vm.submitted() ? true : val && val.length != 0;
					        },
					        message: ko.Localization('Naati.Resources.TestSession.resources.PrimaryRolePlayerForLanguage2Required')
					    }
					}),
					assignedRolePlayers1Secondary: ko.observableArray(),
					assignedRolePlayers2Secondary: ko.observableArray()
				};

				function buildRolePlayerLists() {
					for (var l in lists) {
						lists[l].removeAll();
					}

					serverModel.RolePlayers([]);
					associateToList(vm.availableRolePlayers());
				}

				function associateToList(newRolePlayers) {
					serverModel.RolePlayers(newRolePlayers);

					ko.utils.arrayForEach(newRolePlayers, function (rp) {
						var associated = ko.utils.arrayFirst(rp.Details(), function (d) {
							return d.TestComponentId == component.Id();
						});

						if (associated) {
							var type = associated.RolePlayerRoleTypeId == enums.RolePlayerRoleType.PrimaryRolePlayer ? 'Primary' : 'Secondary';
							var language = associated.LanguageId == serverModel.Skill().Language1Id ? '1' : (associated.LanguageId == serverModel.Skill().Language2Id ? '2' : null);

							if (language) {
								return lists['assignedRolePlayers' + language + type].push(rp);
							}
						}

						lists.elegibleRolePlayers.push(rp);
					});
				}

				function prepareToPush(data, languageId, newRolePlayerRoleType) {
					data.RolePlayerStatusId = enums.RolePlayerStatusType.Pending;

					var associated = ko.utils.arrayFirst(data.Details(), function (d) {
						return d.TestComponentId == component.Id() && (!languageId || d.LanguageId == languageId);
					});

					if (associated) {
						if (!newRolePlayerRoleType) {
							var index = data.Details().indexOf(associated);
							if (index > -1) {
								var details = data.Details();
								details.splice(index, 1);
								data.Details(details);
							}
						}
						else {
							associated.RolePlayerRoleTypeId = newRolePlayerRoleType;
						}
					}
					else if (languageId) {
						var details = data.Details();
						details.push({
							TestComponentId: component.Id(),
							RolePlayerRoleTypeId: newRolePlayerRoleType,
							LanguageId: languageId
						});
						data.Details(details);
					}

					for (var l in lists) {
						lists[l].remove(data);
					}
				}

				componentValidations[component.Id()] = ko.validatedObservable(lists);

				return {
					testSession: testSessionModel,
					dragStart: function (data) {
						if (vm.readOnly() || data.ReadOnly) {
							return false;
						}
					},
					dropSource: function (data) {
						prepareToPush(data, null);
						lists.elegibleRolePlayers.push(data);
					},
					dropTarget1Primary: function (data, model) {
						if (data.LanguageIds.indexOf(model) <= -1) {
							return;
						}
						prepareToPush(data, model, enums.RolePlayerRoleType.PrimaryRolePlayer);
						lists.assignedRolePlayers1Primary.push(data);
					},
					dropTarget2Primary: function (data, model) {
						if (data.LanguageIds.indexOf(model) <= -1) {
							return;
						}
						prepareToPush(data, model, enums.RolePlayerRoleType.PrimaryRolePlayer);
						lists.assignedRolePlayers2Primary.push(data);
					},
					dropTarget1Secondary: function (data, model) {
						if (data.LanguageIds.indexOf(model) <= -1) {
							return;
						}
						prepareToPush(data, model, enums.RolePlayerRoleType.SecondaryRolePlayer);
						lists.assignedRolePlayers1Secondary.push(data);
					},
					dropTarget2Secondary: function (data, model) {
						if (data.LanguageIds.indexOf(model) <= -1) {
							return;
						}
						prepareToPush(data, model, enums.RolePlayerRoleType.SecondaryRolePlayer);
						lists.assignedRolePlayers2Secondary.push(data);
					},
					assignedRolePlayers1Primary: sort(lists.assignedRolePlayers1Primary),
					assignedRolePlayers2Primary: sort(lists.assignedRolePlayers2Primary),
					assignedRolePlayers1Secondary: sort(lists.assignedRolePlayers1Secondary),
					assignedRolePlayers2Secondary: sort(lists.assignedRolePlayers2Secondary),
					elegibleRolePlayers: filterByShowOutOfArea(lists.elegibleRolePlayers),
					skill: serverModel.Skill,
					showLanguage2: showLanguage2,
					toggleOutOfAreaRolPlayers: function () {
						showOutOfArea(!showOutOfArea());
					},
					toggleOutOfAreaRolPlayersText: ko.computed(function () {
						return ko.Localization(showOutOfArea() ? 'Naati.Resources.TestSession.resources.HideOutOfArea' : 'Naati.Resources.TestSession.resources.ShowOutOfArea');
					})
				};
			}

			return vm;
		}
	}
);