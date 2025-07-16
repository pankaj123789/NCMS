define([
    'services/util',
    'modules/enums',
    'services/application-data-service',
    'services/screen/message-service'
], function (util, enums, applicationService, messageService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            wizardService: null,
            materialRequest: null,
            outputMaterial: null,
            action: null,
            materialRound: null,
            stepId: null
        };

        $.extend(defaultParams, params);

        var vm = {
            tableId: util.guid(),
            materialRequest: defaultParams.materialRequest,
            outputMaterial: defaultParams.outputMaterial,
            tasks: ko.observableArray(),
            maxTasks: ko.observable(),
            members: ko.observableArray(),
            allMembers: ko.observableArray(),
            submitted: ko.observable(false),
            haveParticipantsDeselectedWithHoursValidated: ko.observable(false),
            coordinator: ko.observable({}).extend({
                validation: {
                    validator: function (val) {
                        if (!val) {
                            return val;
                        }
                        if (!val.Tasks) {
                            return false;
                        }
                        var notAssignedTasks = ko.utils.arrayFilter(val.Tasks(),
                            function (task) {
                                return !task.MaterialRequestTaskTypeId();
                            });
                        return notAssignedTasks.length === 0;

                    },
                    message: ko.Localization('Naati.Resources.MaterialRequest.resources.SelectAtLeastOneCollaborator')
                }
            }),
            collaborators: ko.observableArray([]),
            coordinatorId: ko.observable().extend({ required: true }),
            showOnlySelected: ko.observable(false),
            coordinatorOptions: {}
        };

        var coordinatorValidation = ko.validatedObservable([vm.coordinator]);
        var collaboratorsValidation = ko.validatedObservable(vm.collaborators);

        vm.preSelectMembers = function (timeout) {
            setTimeout(function () {
                vm.collaborators([]);
                var table = $("#" + vm.tableId).DataTable();
                var indexes = [];
                for (var i = 0; i < vm.members().length; i++) {
                    var member = vm.members()[i];
                    if (member.PreSelected() || member.Selected()) {
                        member.PreSelected(false);
                        member.Selected(true);
                        vm.collaborators.push(member);
                        indexes.push(i);
                    }
                }
                table.rows(indexes)
                    .nodes()
                    .to$()
                    .addClass('selected');
            }, timeout || 500);
        };

        vm.tableDefinition = {
            select: {
                style: 'multi+shift'
            },
            columnDefs: [
                { orderable: false, className: 'select-checkbox', targets: 0 },
                { targets: -1, orderable: false },
                { orderData: 1, targets: 1 }
            ],
            events: {
                select: selectTable,
                deselect: selectTable
            },
            buttons: {
                dom: {
                    button: {
                        tag: 'label',
                        className: ''
                    },
                    buttonLiner: {
                        tag: null
                    }
                },
                buttons: [{
                    text: '<input type="checkbox" checked /><i></i><span>' + ko.Localization('Naati.Resources.MaterialRequest.resources.ShowOnlySelected') + '</span>',
                    className: 'i-switch i-switch-md bg-info m-l',
                    init: function (dt, node, config) {
                        node.children('input[type="checkbox"]').prop('checked', vm.showOnlySelected());
                    },
                    action: function (e, dt, node, config) {
                        var checked = !node.children('input[type="checkbox"]').prop('checked');
                        node.children('input[type="checkbox"]').prop('checked', checked);
                        vm.showOnlySelected(checked);
                    }
                }]
            }
        };

        vm.load = function () {
            vm.submitted(false);
            applicationService.getFluid('lookuptype/MaterialRequestTaskType').then(function (data) {
                vm.maxTasks(data.length);
                data.unshift({ Id: '', DisplayName: ko.Localization('Naati.Resources.Shared.resources.Choose') });
                vm.tasks(data);

                defaultParams.wizardService.getFluid('wizard/members/' + vm.materialRequest.PanelId() + '/' + vm.outputMaterial.CredentialTypeId() + '/' + vm.materialRequest.MaterialRequestId()).then(function (data) {
                    var members = [];
                    ko.utils.arrayForEach(data, function (d, i) {
                        if (!d.Tasks) {
                            d.Tasks = [];
                        }

                        d.Selected = false;
                        d.IsCoordinator = d.IsCoordinator || false;
                        var member = ko.viewmodel.fromModel(d);

                        if (!member.Tasks().length) {
                            vm.addTask(member, true);
                        }

                        ko.utils.arrayForEach(member.Tasks(), function (t) {
                            addValidation(t, member);
                            addAvailableTasks(member, t);
                            selectMemberOnChangeProperties(member, t);
                        });

                        if (member.IsCoordinator()) {
                            member.Selected(true);
                            vm.coordinator(member);
                        }
                        else {
                            members.push(member);
                        }
                    });

                    vm.allMembers(members);
                    collaboratorsValidation = ko.validatedObservable(vm.collaborators);
                    clearValidation();
                });
            });
        };

        vm.addTask = function (member, preventSelect) {
            if (!preventSelect) {
                member.Selected(true);
                vm.preSelectMembers(1);
            }

            var newTask = {
                MaterialRequestTaskTypeId: ko.observable(),
                HoursSpent: ko.observable(),
            };

            addValidation(newTask, member);
            addAvailableTasks(member, newTask);
            member.Tasks.push(newTask);
            selectMemberOnChangeProperties(member, newTask);
        };

        vm.removeTask = function (member, task) {
            member.Tasks.remove(task);
        };

        vm.isValid = function () {
            vm.submitted(true);
            var isCoordinatorValid = coordinatorValidation.isValid();
            var isCollaboratorValid = true;

            for (var i = 0; i < vm.collaborators().length; i++) {
                var collaboratorValidation = ko.validatedObservable(vm.collaborators()[i]);
                isCollaboratorValid = collaboratorValidation.isValid();
                if (!isCollaboratorValid) {
                    collaboratorValidation.errors.showAllMessages();
                }
            }

            if (!isCoordinatorValid && coordinatorValidation.errors) {
                coordinatorValidation.errors.showAllMessages();
            }

            var isValid = isCoordinatorValid && isCollaboratorValid;
            if (isValid) {
                if (vm.haveParticipantsDeselectedWithHoursValidated()) {
                    return true;
                }

                if (haveParticipantsDeselectedWithHours()) {
                    var defer = Q.defer();
                    var messageConfig = {
                        title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
                        content: ko.Localization('Naati.Resources.MaterialRequest.resources.HaveParticipantsDeselectedWithHoursMessage')
                    };
                    messageService.confirm(messageConfig).then(function (answer) {
                        if (answer == 'yes') {
                            vm.haveParticipantsDeselectedWithHoursValidated(true);
                            defer.resolve(true);
                        }
                    });
                    return defer.promise;
                }

                return true;
            }

            return isValid;
        };

        vm.postData = function () {
            var members = [];

            if (vm.coordinator()) {
                members.push({
                    PanelMembershipId: vm.coordinator().Id(),
                    MemberTypeId: enums.MaterialRoundMembmershipType.Coordinator,
                    Tasks: ko.toJS(vm.coordinator().Tasks)
                });
            }

            ko.utils.arrayForEach(vm.collaborators(),
                function (collaborator) {
                    members.push({
                        PanelMembershipId: collaborator.Id(),
                        MemberTypeId: enums.MaterialRoundMembmershipType.Collaborator,
                        Tasks: ko.toJS(collaborator.Tasks)
                    });
                });

            return { SelectedMembers: members };
        };

        function haveParticipantsDeselectedWithHours() {
            return ko.utils.arrayFilter(vm.allMembers(), function (m) {
                return !m.Selected() && ko.utils.arrayFilter(m.Tasks(), function (t) {
                    return t.HoursSpent() > 0 && t.MaterialRequestTaskTypeId() > 0;
                }).length > 0;
            }).length > 0;
        }

        function clearValidation() {
            if (coordinatorValidation.errors || collaboratorsValidation.errors) {
                coordinatorValidation.errors.showAllMessages(false);
                collaboratorsValidation.errors.showAllMessages(false);
            }
        }

        function addAvailableTasks(member, task) {
            var otherTasks = filterOtherTasks(member, task);

            task.AvailableTasks = ko.computed(function () {
                return ko.utils.arrayFilter(vm.tasks(), function (t) {
                    return !t.Id || ko.utils.arrayFirst(otherTasks(), function (otherTask) {
                        return t.Id === otherTask;
                    }) == null;
                });
            });

            task.AvailableTasks.subscribe(function () {
                if (task.AvailableTasks().length == 2) {
                    task.MaterialRequestTaskTypeId(task.AvailableTasks()[1].Id);
                }
            });
        }

        function filterOtherTasks(member, newTask) {
            return ko.computed(function () {
                return ko.utils.arrayMap(member.Tasks(), function (t) {
                    if (t === newTask) {
                        return null;
                    }
                    return t.MaterialRequestTaskTypeId();
                });
            });
        }

        function selectTable(e, dt) {
            vm.collaborators([]);
            ko.utils.arrayForEach(vm.allMembers(), function (m) {
                m.Selected(false);
            });

            var indexes = dt.rows('.selected').indexes();

            if (!indexes.length) {
                return;
            }

            for (var i = 0; i < indexes.length; i++) {
                var member = vm.allMembers()[indexes[i]];
                member.Selected(true);
                vm.collaborators.push(member);
            }
        }

        function addValidation(newTask, member) {
            var requiredCondition = {
                onlyIf: function () {
                    return vm.submitted() && member.Selected();
                }
            };

            newTask.HoursSpent.extend({
                maxLength: 3,
                required: requiredCondition
            });

            newTask.MaterialRequestTaskTypeId.extend({
                required: requiredCondition
            });
        }


        function selectMemberOnChangeProperties(member, newTask) {
            function selectMember() {
                member.Selected(true);
                vm.preSelectMembers(1);
            }

            newTask.MaterialRequestTaskTypeId.subscribe(function (val) {
                if (val) {
                    selectMember();
                }
            });

            newTask.HoursSpent.subscribe(selectMember);
        }

        ko.computed(function () {
            vm.members([]);
            var filteredMembers = ko.utils.arrayFilter(vm.allMembers(), function (m) {
                return !vm.showOnlySelected() || m.Selected();
            });
            vm.members(filteredMembers);
        });

        return vm;
    }
});