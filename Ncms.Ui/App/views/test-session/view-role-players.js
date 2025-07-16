define([
    'plugins/router',
    'modules/collapser',
    'services/testsession-data-service',
    'services/screen/date-service',
    'services/util',
    'modules/enums'
],
    function (router, collapser, testsessionService, dateService, util, enums) {
        var testSessionModel = {
            Id: ko.observable(),
            Name: ko.observable(),
            TestDate: ko.observable(),
            PublicNote: ko.observable(),
            TestSessionId: ko.observable(0),
            CredentialType: ko.observable(),
            CredentialTypeId: ko.observable(),
            RehearsalNotes: ko.observable(),
            TestLocationId: ko.observable(),
            RehearsalDate: ko.observable(),
        };

        var emptyModel = ko.toJS(testSessionModel);

        var vm = {
            testsession: testSessionModel,
            testSessionName: ko.observable(),
            credentialAndDates: ko.observable(),
            skills: ko.observableArray(),
            rolePlayerActions: ko.observableArray([])
        };

        vm.title = ko.computed(function () {
            var model = ko.toJS(testSessionModel);
            return 'TS{0}, {1}, {2}'.format(model.Id, model.Name, model.CredentialType);
        });

        vm.collapser = collapser.create(vm.skills);
        vm.collapser.changeState = function (data) {
            loadRolePlayers(data.data);
        };

        vm.save = function () {
            var req = {
                RolePlayerActions: ko.utils.arrayMap(vm.rolePlayerActions(), function (rolePlayer) {
                    return {
                        TestSessionRolePlayerId: rolePlayer.TestSessionRolePlayerId,
                        SystemActionTypeId: rolePlayer.LastSystemActionTypeId()
                    };
                })
            };

            testsessionService.post(req, 'viewRolePlayers/Update').then(function () {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));

                var skills = [];
                ko.utils.arrayForEach(vm.rolePlayerActions(), function (rolePlayer) {
                    if (skills.indexOf(rolePlayer.Skill.SkillId) == -1) {
                        skills.push(rolePlayer.Skill.SkillId);
                        rolePlayer.Skill.RolePlayersLoaded = false;
                        loadRolePlayers(rolePlayer.Skill);
                    }
                });

                vm.rolePlayerActions.removeAll();
            });
        };

        vm.canSave = ko.computed(function () {
            return vm.rolePlayerActions().length;
        });

        $.extend(vm, {
            title: ko.computed(function () {
                return '{0}'.format(vm.testSessionName());
            }),
            subtitle: ko.computed(function () {
                return '{0}'.format(vm.credentialAndDates());
            })
        });

        vm.canActivate = function (testsessionId, query) {
            queryString = query || {};
            testsessionId = parseInt(testsessionId);

            ko.viewmodel.updateFromModel(testSessionModel, emptyModel);

            testSessionModel.TestSessionId(testsessionId);

            loadData();

            return true;
        };

        vm.back = function () {
            router.navigateBack();
            return false;
        };

        function bindTabs(skill) {
            skill.componentValidations = {};

            var active = true;
            var tabs = ko.utils.arrayMap(skill.Tasks(), function (component) {
                var data = getTemplateData(component, skill);
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
                        name: 'view-role-player-task',
                        data: data
                    },
                    valid: function () {
                        return skill.componentValidations[component.Id()].isValid();
                    },
                    validate: function () {
                        return skill.componentValidations[component.Id()].isValid();
                    },
                    clearValidation: function () {
                        if (skill.componentValidations[component.Id()].errors) {
                            return skill.componentValidations[component.Id()].errors.showAllMessages(false);
                        }
                    }
                };

                active = false;
                return tab;
            });

            skill.tabOptions.tabs(tabs);
        }

        function updateFromModel(observable) {
            return function (data) {
                ko.viewmodel.updateFromModel(observable, data);
            };
        }

        function getTemplateData(component, skill) {
            var showLanguage2 = ko.computed(function () {
                if (!skill) return false;
                return skill.Language1Id != skill.Language2Id;
            });

            var lists = {
                assignedRolePlayers1Primary: ko.observableArray(),
                assignedRolePlayers2Primary: ko.observableArray(),
                assignedRolePlayers1Secondary: ko.observableArray(),
                assignedRolePlayers2Secondary: ko.observableArray()
            };

            function sort(observable) {
                return ko.computed(function () {
                    return observable().sort(util.sortBy('DisplayOrder'));
                });
            }

            function buildRolePlayerLists() {
                for (var l in lists) {
                    lists[l].removeAll();
                }

                associateToList(skill);
            }

            function associateToList(skill) {
                var newRolePlayers = skill.RolePlayers();

                ko.utils.arrayForEach(newRolePlayers, function (rp) {
                    var associated = ko.utils.arrayFirst(rp.Details(), function (d) {
                        return d.TestComponentId == component.Id();
                    });

                    if (associated) {
                        var type = associated.RolePlayerRoleTypeId == enums.RolePlayerRoleType.PrimaryRolePlayer ? 'Primary' : 'Secondary';
                        var language = associated.LanguageId == skill.Language1Id ? '1' : (associated.LanguageId == skill.Language2Id ? '2' : null);

                        if (language) {
                            return lists['assignedRolePlayers' + language + type].push(rp);
                        }
                    }
                });
            }

            skill.componentValidations[component.Id()] = ko.validatedObservable(lists);
            buildRolePlayerLists();

            return {
                testSession: testSessionModel,
                assignedRolePlayers1Primary: sort(lists.assignedRolePlayers1Primary),
                assignedRolePlayers2Primary: sort(lists.assignedRolePlayers2Primary),
                assignedRolePlayers1Secondary: sort(lists.assignedRolePlayers1Secondary),
                assignedRolePlayers2Secondary: sort(lists.assignedRolePlayers2Secondary),
                skill: skill,
                showLanguage2: showLanguage2
            };
        }

        function processSkills(values) {
            var skills = ko.utils.arrayMap(values, function (data) {
                var sk = data.Skill;

                sk.Tasks = ko.viewmodel.fromModel(data.Tasks);
                sk.valid = ko.observable(true);
                sk.componentValidations = {};
                sk.tabOptions = {
                    tabs: ko.observableArray()
                };
                sk.validate = function () {
                    return sk.tabOptions.component.validate();
                };
                sk.RolePlayers = ko.observableArray();
                sk.RolePlayersLoaded = false;

                if (values.length === 1) {
                    loadRolePlayers(sk);
                }

                return sk;
            });

            vm.skills(skills);
        }

        function loadRolePlayers(skill) {
            if (skill.RolePlayersLoaded) return;

            testsessionService.getFluid('viewRolePlayers/roleplayers', { TestSessionId: testSessionModel.TestSessionId(), SkillId: skill.SkillId }).then(function (data) {
                ko.utils.arrayForEach(data, function (rolePlayer) {
                    var languages = [];

                    if (rolePlayer.LanguageIds.indexOf(skill.Language1Id) >= 0) {
                        languages.push(skill.Language1DisplayName);
                    }
                    if (rolePlayer.LanguageIds.indexOf(skill.Language2Id) >= 0) {
                        languages.push(skill.Language2DisplayName);
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

                    rolePlayer.Skill = skill;
                    rolePlayer.GenderDescription = ko.Localization('Naati.Resources.Shared.resources.' + (rolePlayer.Gender == 'M' ? 'Male' : (rolePlayer.Gender == 'F' ? 'Female' : 'Unspecified')));
                    rolePlayer.Details = ko.observableArray(rolePlayer.Details);
                    rolePlayer.OriginalLastSystemActionTypeId = rolePlayer.LastSystemActionTypeId;
                    rolePlayer.LastSystemActionTypeId = ko.observable(rolePlayer.LastSystemActionTypeId);
                    rolePlayer.Languages = languages.join(', ');
                    rolePlayer.Rating = '(' + (rolePlayer.Rating ? $.formatNumber(rolePlayer.Rating, { format: "##.0", locale: "us" }) : '0') + '/10)';
                    rolePlayer.AssignedTasks = ko.computed(function () {
                        var tasks = ko.utils.arrayMap(rolePlayer.Details(), function (d) {
                            var task = ko.utils.arrayFirst(skill.Tasks(), function (t) {
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

                    rolePlayer.stateOptions = {
                        value: rolePlayer.LastSystemActionTypeId,
                        options: ko.observableArray(),
                        addChooseOption: false,
                        multiple: false,
                        optionsValue: 'Id',
                        optionsText: 'Name'
                    };

                    testsessionService.getFluid('viewRolePlayers/' + rolePlayer.LastSystemActionTypeId() + '/testSessionRolePLayerActions').then(rolePlayer.stateOptions.options);

                    rolePlayer.Status = status;
                    rolePlayer.LastSystemActionTypeId.subscribe(function () {
                        stackToSave(rolePlayer);
                    });
                });

                skill.RolePlayersLoaded = true;
                skill.RolePlayers(data);
                bindTabs(skill);
            });
        }

        function stackToSave(rolePlayer) {
            var stacked = ko.utils.arrayFirst(vm.rolePlayerActions(), function (r) {
                return r.TestSessionRolePlayerId == rolePlayer.TestSessionRolePlayerId;
            });

            if (stacked) {
                if (rolePlayer.LastSystemActionTypeId() == rolePlayer.OriginalLastSystemActionTypeId) {
                    return vm.rolePlayerActions.remove(stacked);
                }

                return;
            }

            if (rolePlayer.LastSystemActionTypeId() == rolePlayer.OriginalLastSystemActionTypeId) return;

            vm.rolePlayerActions.push(rolePlayer);
        }

        function loadSkills() {
            testsessionService.getFluid(testSessionModel.TestSessionId() + '/topsection').then(updateFromModel(testSessionModel));

            testsessionService.getFluid('viewRolePlayers/' + testSessionModel.TestSessionId() + '/testSessionSkills').then(function (data) {
                var promises = [];

                ko.utils.arrayForEach(data, function (d) {
                    promises.push(testsessionService.getFluid('viewRolePlayers/' + testSessionModel.TestSessionId() + '/' + d.Id + '/skillDetails'));
                });

                Q.all(promises).then(processSkills);
            });
        }

        function loadData() {
            loadtopsection();
            loadSkills();
        }

        function loadtopsection() {
            testsessionService.getFluid(testSessionModel.TestSessionId() + '/topsection').then(function (data) {
                ko.viewmodel.updateFromModel(testSessionModel, data);

                var heardingFistLine = 'TS{0}, {1}, {2}'.format(data.Id, data.Name, data.CredentialType);
                var heardingSecondLine = moment(data.RehearsalDate).format('L');
                vm.testSessionName(heardingFistLine);
                vm.credentialAndDates(heardingSecondLine);
            });
        }

        return vm;

    });