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
            TestSessionId: ko.observable(0),
            CredentialType: ko.observable(),
            CredentialTypeId: ko.observable(),
            RehearsalNotes: ko.observable(),
            TestLocationId: ko.observable(),
        };

        var emptyModel = ko.toJS(testSessionModel);

        var vm = {
            testsession: testSessionModel,
            testSessionName: ko.observable(),
            credentialAndDates: ko.observable(),
            skills: ko.observableArray(),
            rolePlayers: ko.observableArray([]),
            rolePlayerActions: ko.observableArray([])
        };

        vm.collapser = collapser.create(vm.skills);

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
                loadSkills();
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

        function processLanguage(skill, languageName) {
            var id = skill[languageName + 'Id'];
            if (!id) return;

            var language = { Id: id, DisplayName: skill[languageName + 'DisplayName'] };

            language.tableDefinition = {
                headerTemplate: 'manageroleplayers-header-template',
                rowTemplate: 'manageroleplayers-row-template'
            };

            language.tableDefinition.dataTable = {
                source: ko.computed(function () {
                    return ko.utils.arrayFilter(skill.RolePlayers(), function (rp) {
                        return rp.LanguageIds.indexOf(id) >= 0;
                    });
                }),
                columnDefs: [
                    {
                        targets: [-1, -2],
                        orderable: false
                    },
                    {
                        targets: 3,
                        render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateTimeDisplayFormat)
                    }
                ],
                order: [
                    [0, "asc"]
                ]
            };

            var languages = skill.Languages();
            languages.push(language);
            skill.Languages(languages);
            skill.collapser.expandAll();
        }

        function processSkills(values) {
            var skills = ko.utils.arrayMap(values, function (data) {
                var sk = data.Skill;
                sk.Tasks = ko.viewmodel.fromModel(data.Tasks);
                sk.RolePlayers = ko.observableArray();
                sk.Languages = ko.observableArray();
                sk.collapser = collapser.create(sk.Languages);

                processLanguage(sk, 'Language1');
                processLanguage(sk, 'Language2');
                loadRolePlayers(sk);

                return sk;
            });

            vm.skills(skills);
        }

        function loadSkills() {
            testsessionService.getFluid(testSessionModel.TestSessionId() + '/topsection').then(util.updateFromModel(testSessionModel));

            testsessionService.getFluid('viewRolePlayers/' + testSessionModel.TestSessionId() + '/testSessionSkills').then(function (data) {
                var promises = [];

                ko.utils.arrayForEach(data, function (d) {
                    promises.push(testsessionService.getFluid('viewRolePlayers/' + testSessionModel.TestSessionId() + '/' + d.Id + '/skillDetails'));
                });

                Q.all(promises).then(processSkills);
            });
        }

        function processRolePlayers(data, skill) {
            var rolePlayers = [];

            ko.utils.arrayForEach(data, function (rolePlayer) {
                var languages = [];

                if (rolePlayer.LanguageIds.indexOf(skill.Language1Id) >= 0) {
                    languages.push(skill.Language1DisplayName);
                }
                if (rolePlayer.LanguageIds.indexOf(skill.Language2Id) >= 0) {
                    languages.push(skill.Language2DisplayName);
                }

                ko.utils.arrayForEach(rolePlayer.Details, function (d) {
                    d.RolePlayerRoleType = ko.Localization('Naati.Resources.TestSession.resources.' + (d.RolePlayerRoleTypeId == enums.RolePlayerRoleType.PrimaryRolePlayer ? 'Primary' : 'Backup'));
                    d.TaskName = ko.utils.arrayFirst(skill.Tasks(), function (t) {
                        return t.Id() === d.TestComponentId;
                    }).TaskName;
                    d.Skill = d.LanguageId == skill.Language1Id ? skill.Language1DisplayName : (d.LanguageId == skill.Language2Id ? skill.Language2DisplayName : null);
                });

                rolePlayer.MatchTestLocation = ko.utils.arrayFirst(rolePlayer.AvailableTestLocations, function (a) {
                    return a.Id == testSessionModel.TestLocationId();
                });

                var notMatchTestLocation = ko.utils.arrayFilter(rolePlayer.AvailableTestLocations, function (a) {
                    return a.Id != testSessionModel.TestLocationId();
                });

                rolePlayer.NotMatchTestLocationTooltip = null;

                if (notMatchTestLocation.length) {
                    notMatchTestLocation = util.distinctBy(notMatchTestLocation, 'DisplayName');
                    var locations = ko.utils.arrayMap(notMatchTestLocation, function (l) {
                        return l.DisplayName;
                    });
                    rolePlayer.NotMatchTestLocationTooltip = '<ul class="m-l-n-md"><li>' + locations.join('</li><li>') + '</li></ul>';
                }

                rolePlayer.Skill = skill;
                rolePlayer.Details = ko.observableArray(rolePlayer.Details);
                rolePlayer.OriginalLastSystemActionTypeId = rolePlayer.LastSystemActionTypeId;
                rolePlayer.LastSystemActionTypeId = ko.observable(rolePlayer.LastSystemActionTypeId);
                rolePlayer.Rating = (rolePlayer.Rating ? $.formatNumber(rolePlayer.Rating, { format: "##.0", locale: "us" }) : '0') + '/10';
                rolePlayer.Languages = languages.join(', ');
                rolePlayer.GenderDescription = ko.Localization('Naati.Resources.Shared.resources.' + (rolePlayer.Gender == 'F' ? 'Female' : 'Male'));

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
                rolePlayer.StatusCss = util.getRolePlayerStatusCss(rolePlayer.RolePlayerStatusId);
                rolePlayer.LastSystemActionTypeId.subscribe(function () {
                    stackToSave(rolePlayer);
                });

                rolePlayers.push(rolePlayer);
            });

            skill.RolePlayers(rolePlayers);
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

        function loadRolePlayers(skill) {
            testsessionService.getFluid('viewRolePlayers/roleplayers', { TestSessionId: testSessionModel.TestSessionId(), SkillId: skill.SkillId }).then(function (data) {
                processRolePlayers(data, skill);
            });
        }

        function loadData() {
            loadtopsection();
            loadSkills();
        }

        function loadtopsection() {
            testsessionService.getFluid(testSessionModel.TestSessionId() + '/topsection').then(function (data) {
                ko.viewmodel.updateFromModel(testSessionModel, data);

                var heardingFistLine = 'TS' + data.Id + ', ' + data.Name;
                var heardingSecondLine = '{0} - {1} {2}'.format(data.CredentialType, dateService.toUIDate(data.RehearsalDate), data.RehearsalTime);
                vm.testSessionName(heardingFistLine);
                vm.credentialAndDates(heardingSecondLine);
            });
        }

        return vm;

    });