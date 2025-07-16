define([
    'modules/common',
    'plugins/router',
    'services/screen/message-service',
    'services/screen/date-service',
    'services/panel-member-data-service',
    'services/entity-data-service',
    'services/system-data-service',
    'modules/enums'
], function (common, router, message, dateService, panelMemberDataService, entityService, systemService, enums) {
    var defer = null;

    var serverModel = {
        PanelMembershipId: ko.observable(),
        PanelId: ko.observable().extend({ required: true }),
        PersonId: ko.observable().extend({ required: true }),
        PanelRoleId: ko.observable().extend({ required: true }),
        StartDate: ko.observable().extend({ required: true }),
        EndDate: ko.observable().extend({ required: true }),
        CredentialTypeIds: ko.observableArray([]),
        MaterialCredentialTypeIds: ko.observableArray([]),
        CoordinatorCredentialTypeIds: ko.observableArray([]),
        NaatiNumber: ko.observable()
    };

    var emptyModel = ko.toJS(serverModel);

    var vm = {
        examinerRoles: ko.observableArray([]),
        markingStatus: ko.observableArray(['InProgress', 'Overdue']),
        markingRequests: ko.observableArray([]),
        unavailability: ko.observableArray([]),
        roles: ko.observableArray([]),
        credentialTypes: ko.observableArray([]),
        editMode: ko.observable(false),
        title: ko.pureComputed(function () {
            return ko.Localization(vm.editMode() ? 'Naati.Resources.Panel.resources.EditMember' : 'Naati.Resources.Panel.resources.AddMember');
        }),
        currentMember: serverModel,
        naatiNumber: ko.observable(''),
        personName: ko.observable(''),
        setDetails: function (member) {
            var startDateFormatted = moment(member.StartDate).format(CONST.settings.shortDateDisplayFormat);
            var endDateFormatted = moment(member.EndDate).format(CONST.settings.shortDateDisplayFormat);

            vm.currentMember.PanelMembershipId(member.PanelMembershipId);
            vm.currentMember.PanelId(member.PanelId);
            vm.currentMember.PersonId(member.PersonId);
            vm.currentMember.NaatiNumber(member.NaatiNumber);

            vm.naatiNumber(member.NaatiNumber);
            vm.personName(member.PersonName);
            vm.currentMember.PanelRoleId(member.PanelRoleId);
            vm.currentMember.StartDate(startDateFormatted);
            vm.currentMember.EndDate(endDateFormatted);
            setCredentialTypes(member.CredentialTypeIds, member.MaterialCredentialTypeIds, member.CoordinatorCredentialTypeIds).then(function () {
                dirtyFlag().reset();
            });
        },
        clearDetails: function () {
            vm.currentMember.PanelMembershipId(null);
            vm.currentMember.PanelId(null);
            vm.currentMember.PersonId(null);
            vm.naatiNumber(null);
            vm.personName(null);
            vm.currentMember.PanelRoleId(null);
            vm.currentMember.StartDate(null);
            vm.currentMember.EndDate(null);
            vm.currentMember.CredentialTypeIds([]);
            vm.currentMember.MaterialCredentialTypeIds([]);
            vm.currentMember.CoordinatorCredentialTypeIds([]);
            vm.clearValidation();
            vm.currentMember.NaatiNumber(null);
        },
        setNewDetails: function (panelId) {
            var currentDate = moment();

            var startDate = moment([currentDate.month() > 5 ? currentDate.year() : currentDate.year() - 1, 6, 1]);
            var endDate = moment([currentDate.month() > 5 ? currentDate.year() + 1 : currentDate.year(), 5, 30]);

            var startDateFormatted = startDate.format(CONST.settings.shortDateDisplayFormat);
            var endDateFormatted = endDate.format(CONST.settings.shortDateDisplayFormat);

            vm.clearDetails();

            vm.currentMember.PanelId(panelId);
            vm.currentMember.StartDate(startDateFormatted);
            vm.currentMember.EndDate(endDateFormatted);
            setCredentialTypes([]).then(function () {
                dirtyFlag().reset();
            });
        },
        createMember: function (panelId) {
            defer = Q.defer();

            vm.editMode(false);
            vm.setNewDetails(panelId);

            return defer.promise;
        },
        editMember: function (member) {
            defer = Q.defer();

            vm.editMode(true);
            vm.setDetails(member);

            return defer.promise;
        },
        validate: function () {
            var isValid = vm.validation.isValid();

            vm.validation.errors.showAllMessages(!isValid);

            return isValid;
        },
        clearValidation: function () {
            vm.validation.errors.showAllMessages(false);
        },
        parseMember: function () {
            var parsedMember = ko.toJS(vm.currentMember);

            parsedMember.StartDate = dateService.format(parsedMember.StartDate);
            parsedMember.EndDate = dateService.format(parsedMember.EndDate);

            return parsedMember;
        },
        save: function () {
            if (vm.validate()) {
                var request = new Array();
                request[0] = {
                    PersonId: vm.parseMember().PersonId,
                    PanelId: vm.parseMember().PanelId,
                    PanelRoleId: vm.parseMember().PanelRoleId,
                    StartDate: vm.parseMember().StartDate,
                    EndDate: vm.parseMember().EndDate,
                    PanelMembershipId: vm.parseMember().PanelMembershipId
                };
                //returns true when an overlapping membership is detected.
                panelMemberDataService.post({ Items: request }, 'HasOverlappingMembership').then(
                    function (data) {
                        if (data === true) {
                            toastr.error(
                                ko.Localization('Naati.Resources.Panel.resources.OverlappingPanelMembership'));
                        } else {
                            panelMemberDataService.getFluid('hasPersonEmailAddress/' + vm.parseMember().PersonId).then(
                                function (data) {
                                    if (data === true) {
                                        panelMemberDataService.post(vm.parseMember()).then(function () {
                                            dirtyFlag().reset();
                                            toastr.success(
                                                ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                                            defer.resolve(vm.parseMember());
                                        });
                                    } else {
                                        toastr.error(ko.Localization('Naati.Resources.Shared.resources.EmailNotFound'));
                                        defer.resolve(vm.parseMember());
                                    }
                                });
                        }
                    });
            }
        }
    };

    $.extend(true, vm, {
        validation: ko.validatedObservable(vm.currentMember),
        startDateOptions: {
            value: vm.currentMember.StartDate,
            resattr: {
                placeholder: 'Naati.Resources.Shared.resources.StartDate'
            }
        },
        endDateOptions: {
            value: vm.currentMember.EndDate,
            resattr: {
                placeholder: 'Naati.Resources.Shared.resources.EndDate'
            }
        },
        credentialTypeOptions: {
            selectedOptions: vm.currentMember.CredentialTypeIds,
            multiple: true,
            options: ko.observableArray(),
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            //afterRender: function () { panelMemberDataService.getFluid('AvailableCredentialTypes/' + vm.currentMember.PanelId()  + '/' + (vm.currentMember.PanelMembershipId() || 0)) },
            multiselect: { enableFiltering: true }
        },
        materialCredentialTypeOptions: {
            selectedOptions: vm.currentMember.MaterialCredentialTypeIds,
            multiple: true,
            options: ko.observableArray(),
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            multiselect: { enableFiltering: true }
        },
        coordinatorCredentialTypeOptions: {
            selectedOptions: vm.currentMember.CoordinatorCredentialTypeIds,
            multiple: true,
            options: ko.observableArray(),
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            multiselect: { enableFiltering: true }
        },
        canActivate: function (panelId, panelMembershipId) {
            ko.viewmodel.updateFromModel(serverModel, emptyModel);

            vm.markingRequests([]);
            vm.markingStatus(['In Progress', 'Overdue']);
            serverModel.PanelMembershipId(panelMembershipId);
            serverModel.PanelId(panelId);

            if (!panelMembershipId) {
                vm.createMember(panelId);
                return true;
            }

            return loadMembership();
        }
    });

    vm.roleName = ko.computed(function () {
        var role = ko.utils.arrayFirst(vm.roles(), function (r) {
            return vm.currentMember.PanelRoleId() === r.PanelRoleId;
        });
        return (role || {}).Name
    });

    vm.credentialTypeOptions.disable = ko.pureComputed(function () {
        return !vm.credentialTypeOptions.options().length;
    });

    var dirtyFlag = new ko.DirtyFlag([serverModel], false);
    vm.canSave = ko.pureComputed(function () {
        return dirtyFlag().isDirty() && currentUser.hasPermissionSync(enums.SecNoun.PanelMember, enums.SecVerb.Update);
    });

    vm.close = function () {
        if (dirtyFlag().isDirty()) {
            message.confirm({
                title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                content: ko.Localization('Naati.Resources.Shared.resources.ConfirmBeforeSaving')
            })
                .then(
                    function (answer) {
                        if (answer === 'yes') {
                            router.navigate('panel/' + serverModel.PanelId());
                        }
                    });
        } else {
            router.navigate('panel/' + serverModel.PanelId());
        }
    };

    var filteredMarkingRequests = ko.observableArray([]);

    function isMarkingStatusSelected(status) {
        return vm.markingStatus().includes(status);
    }

    function applyFilter() {
        filteredMarkingRequests([]);

        ko.utils.arrayFilter(vm.markingRequests(),
            function (markingRequest) {
                markingRequest.Status = markingRequest.Status === 'InProgress' ? 'In Progress' : markingRequest.Status;
                if (vm.markingStatus().includes(markingRequest.Status)) {
                    filteredMarkingRequests.push(markingRequest);
                }
            });
    }

    function pushOrRemoveMarkingStatus(status, push) {
        if (push) {
            vm.markingStatus.push(status);
            return;
        }

        vm.markingStatus.remove(status);
    }

    var unavailabilityTableComponent = {
        name: 'table-component',
        params: {
            id: 'unavailabilityTable',
            headerTemplate: 'unavailability-header-template',
            rowTemplate: 'unavailability-row-template'
        }
    };

    unavailabilityTableComponent.params.dataTable = {
        source: vm.unavailability,
        scrollCollapse: true,
        paging: false,
        searching: false,
        //ordering: false,
        info: false,
        columnDefs: [
            { targets: [0, 1], render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat) }
        ],
        order: [
            [0, "desc"]
        ]
    };

    var markingRequestsTableComponent = {
        name: 'table-component',
        params: {
            id: 'markingRequestsTable',
            headerTemplate: 'marking-requests-header-template',
            rowTemplate: 'marking-requests-row-template'
        }
    };

    markingRequestsTableComponent.params.dataTable = {
        source: filteredMarkingRequests,
        scrollCollapse: true,
        paging: false,
        info: false,
        columnDefs: [
            { targets: [0, 1], render: $.fn.dataTable.render.nullableMoment }
        ],
        buttons: {
            dom: {
                button: {
                    tag: 'div',
                    className: 'option'
                },
                buttonLiner: {
                    tag: null
                }
            },
            buttons: [
                {
                    text: '<label class="i-switch i-switch-md bg-info m-l"><input type="checkbox" /><i></i><span>' + ko.Localization('Naati.Resources.ExaminerStatus.resources.Overdue') + '</span></label>',
                    className: '',
                    init: function (dt, node, config) {
                        node.find('input[type="checkbox"]').prop('checked', isMarkingStatusSelected('Overdue'));
                    },
                    action: function (e, dt, node, config) {
                        var checked = !node.find('input[type="checkbox"]').prop('checked');
                        node.find('input[type="checkbox"]').prop('checked', checked);
                        pushOrRemoveMarkingStatus('Overdue', checked);
                        applyFilter();
                    }
                },
                {
                    text: '<label class="i-switch i-switch-md bg-info m-l"><input type="checkbox" /><i></i><span>' + ko.Localization('Naati.Resources.ExaminerStatus.resources.Pending') + '</span></label>',
                    className: '',
                    init: function (dt, node, config) {
                        node.find('input[type="checkbox"]').prop('checked', isMarkingStatusSelected('In Progress'));
                    },
                    action: function (e, dt, node, config) {
                        var checked = !node.find('input[type="checkbox"]').prop('checked');
                        node.find('input[type="checkbox"]').prop('checked', checked);
                        pushOrRemoveMarkingStatus('In Progress', checked);
                        applyFilter();
                    }
                },
                {
                    text: '<label class="i-switch i-switch-md bg-info m-l"><input type="checkbox" /><i></i><span>' + ko.Localization('Naati.Resources.ExaminerStatus.resources.Submitted') + '</span></label>',
                    className: '',
                    init: function (dt, node, config) {
                        node.find('input[type="checkbox"]').prop('checked', isMarkingStatusSelected('Submitted'));
                    },
                    action: function (e, dt, node, config) {
                        var checked = !node.find('input[type="checkbox"]').prop('checked');
                        node.find('input[type="checkbox"]').prop('checked', checked);
                        pushOrRemoveMarkingStatus('Submitted', checked);
                        applyFilter();
                    }
                }
            ]
        }
    };

    vm.getExaminerCss = function (status) {
        if (status == 'Overdue') {
            return 'text-danger';
        }
        else if (status == 'In Progress') {
            return 'text-info';
        }
        else if (status == 'Submitted') {
            return 'text-success';
        }
    };

    var isExaminer = ko.computed(function () {
        return ko.utils.arrayFirst(vm.examinerRoles(), function (e) {
            return parseInt(e) === serverModel.PanelRoleId();
        });
    });

    vm.tabOptions = {
        id: 'membershipEdit',
        tabs: ko.observableArray([
            {
                id: 'markingRequests',
                active: isExaminer,
                visible: isExaminer,
                label: ko.Localization('Naati.Resources.Panel.resources.MarkingRequests'),
                type: 'template',
                template: {
                    name: 'examiner-requests-template',
                    data: {
                        markingRequestsTableComponent: markingRequestsTableComponent
                    }
                }
            },
            {
                id: 'examinerUnavailability',
                active: ko.computed(function () {
                    return !isExaminer();
                }),
                label: ko.Localization('Naati.Resources.Panel.resources.ExaminerUnavailability'),
                type: 'template',
                template: {
                    name: 'unavailability-template',
                    data: {
                        unavailabilityTableComponent: unavailabilityTableComponent
                    }
                }
            }
        ])
    };

    function loadMembership() {
        panelMemberDataService.getFluid('value/ExaminerRoles').then(function (value) {
            vm.examinerRoles(value.split(','));
        });
        entityService.getFluid('panelRole').then(vm.roles);
        common.functions().getLookup('CredentialType').then(vm.credentialTypes);

        panelMemberDataService.getFluid(serverModel.PanelMembershipId() + '/unavailability').then(vm.unavailability);
        panelMemberDataService.getFluid(serverModel.PanelMembershipId() + '/markingrequests').then(function (data) {
            vm.markingRequests(data);
            applyFilter();
        });

        var request = JSON.stringify({
            PanelId: serverModel.PanelId(),
            MembershipId: serverModel.PanelMembershipId()
        });

        return panelMemberDataService.get({ request: request }).then(function (data) {
            var member = panelMemberDataService.mapMembership(data[0], vm.credentialTypes());
            vm.editMember(member);
            return data.length > 0;
        });
    }

    function setCredentialTypes(credentialTypesIds, materialCredentialTypes, coordinatorCredentialTypes) {
        return panelMemberDataService.getFluid('AvailableCredentialTypes/' + vm.currentMember.PanelId() + '/' + (vm.currentMember.PanelMembershipId() || 0)).then(function (data) {
            vm.credentialTypeOptions.options(data);
            vm.materialCredentialTypeOptions.options(data);
            vm.coordinatorCredentialTypeOptions.options(data);

            vm.currentMember.CredentialTypeIds(credentialTypesIds);
            vm.currentMember.MaterialCredentialTypeIds(materialCredentialTypes);
            vm.currentMember.CoordinatorCredentialTypeIds(coordinatorCredentialTypes);
        });
    }

    return vm;
});
