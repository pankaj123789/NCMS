define([
    'plugins/router',
    'modules/enums',
    'services/screen/date-service',
    'services/screen/message-service',
    'services/material-request-data-service',
    'services/panel-data-service',
    'services/panel-member-data-service',
    'services/panel-note-data-service',
    'views/panels/details',
    'views/panels/memberDetails',
    'views/shared/note',
    'modules/common',
    'services/util',
    'components/views/tab-lazy-load-controller'
], function (router, enums, dateService, messageService, materialRequestDataService, panelDataService, panelMemberDataService, panelNoteDataService, panelDetailsModel, memberDetailsModel, noteModel, common, util, tabController) {
    function getpanelDetailsInstance() {
        var panelDetailsInstance = panelDetailsModel.getInstance();

        panelDetailsInstance.noteCss('form-group col-md-9');

        return panelDetailsInstance;
    }

    function getFinancialYear(date) {
        var endOfYear = date.month() > 5; // moments months are 0 based, i.e. 5 is June
        var startOfFinancialYear = endOfYear ? date.year() : date.year() - 1;
        var endOfFinancialYear = endOfYear ? date.year() + 1 : date.year();

        return {
            startDate: moment({ year: startOfFinancialYear, month: 6, day: 1 }),
            endDate: moment({ year: endOfFinancialYear, month: 5, day: 30 })
        };
    }
    var functions = common.functions();
    var defer = null;

    var compositionComplete = false;
    var queryString;
    var vm = {
        tabId: ko.observable(),
        currentPanel: ko.observable(null),
        panelMembers: ko.pureComputed(function () {
            var panel = vm.currentPanel();
            return panel && panel.panelMembers ? panel.panelMembers() : [];
        }),
        hasExaminersAllocated: ko.pureComputed(function () {
            var panel = vm.currentPanel();
            return panel ? panel.HasExaminersAllocated : false;
        }),
        selectedMembers: ko.observableArray([]),
        reappointDates: {
            startDate: ko.observable(null).extend({ required: true }),
            endDate: ko.observable(null).extend({ required: true })
        },
        materialRequests: ko.observableArray([]),
        showOldMembership: ko.observable(false),
        panelDetailsInstance: ko.observable(null),
        memberDetailsInstance: memberDetailsModel.getInstance(),
        canCreateTestMaterial: ko.observable(),
        noteInstance: noteModel.getInstance(),
        bindDetails: ko.pureComputed(function () {
            return vm.panelDetailsInstance() !== null;
        }),
        credentialTypes: ko.observableArray([]),
        membersTableComponent: {
            name: 'table-component',
            params: {
                id: 'membershipTable',
                headerTemplate: 'panel-member-header-template',
                rowTemplate: 'panel-member-row-template'
            }
        },
        materialRequestsTableComponent: {
            name: 'table-component',
            params: {
                id: 'materialRequestsTable',
                headerTemplate: 'material-requests-header-template',
                rowTemplate: 'material-requests-row-template'
            },
            order: [
                [0, "desc"]
            ]
        },
        onTabClick: function (tabOption) {
            var panelId = vm.panelId();
            var tabId = tabOption.id;

            var currentUrl = window.location;
            var baseUrl = currentUrl.protocol + '//' + currentUrl.host + '/' + currentUrl.pathname.split('/')[1];
            var url = baseUrl + '#panel';

            if (panelId) {
                url += '/' + panelId;
            }

            url += '?tab=' + tabId;

            window.history.replaceState(null, document.title, url);
        },
        canActivate: function (panelId, query) {
            defer = Q.defer();
            queryString = query;
            if (compositionComplete) {
                processRequest();
            }


            panelDataService.get({ request: JSON.stringify({ PanelId: panelId }) }).then(function (panels) {
                var panel = panels[0];

                panel.panelMembers = ko.observableArray([]);
                vm.currentPanel(panel);
                functions.getLookup('CredentialType').then(function (data) {
                    vm.credentialTypes(data);
                    return defer.resolve(true);
                });

            });

            return defer.promise;
        },
        activate: function (panelId, query) {
            vm.showOldMembership(false);

            vm.panelDetailsInstance(getpanelDetailsInstance());
            vm.panelDetailsInstance().setDetails(vm.currentPanel());

            vm.getPanelMembers();
            vm.noteInstance.getNotes();
            vm.getMaterialRequests(panelId);

            currentUser.hasPermission(enums.SecNoun.MaterialRequest, enums.SecVerb.Create).then(vm.canCreateTestMaterial);

            return true;
        },
        compositionComplete: function () {
            compositionComplete = true;
            processRequest();
        },
        parsePanel: function () {
            var request = ko.toJS(vm.panelDetailsInstance().panel);
            request.CommissionedDate = dateService
                .toPostDate(moment(vm.panelDetailsInstance().panel.CommissionedDate()));
            return request;
        },
        panelId: function () {
            return vm.currentPanel().PanelId;
        },
        parseRequest: function () {
            return JSON.stringify({
                PanelId: vm.panelId(),
                StartDate: null,
                EndDate: vm.showOldMembership() ? null : dateService.today()
            });
        },
        getPanelMembers: function () {
            
            if (!currentUser.hasPermissionSync(enums.SecNoun.PanelMember, enums.SecVerb.Read))
            {
              return Promise.resolve([]);
            }

            var getMembersDefer = Q.defer();

            panelMemberDataService.get({ request: vm.parseRequest() }).then(function (data) {

                ko.utils.arrayForEach(data, panelMemberDataService.getMembershipMapper(vm.credentialTypes()));

                vm.currentPanel().panelMembers(data);

                getMembersDefer.resolve(data);
            });

            return getMembersDefer.promise;
        },
        reappointPanelMembers: function () {
            if (vm.validateReappointDates()) {
                var overlapRequest = [];
                ko.utils.arrayForEach(vm.selectedMembers(),
                    function (row) {
                        var id = row[8];
                        vm.panelMembers().forEach(function (item) {
                            if (item.PanelMembershipId == id) {
                                overlapRequest.push({
                                    PersonId: item.PersonId,
                                    PanelId: item.PanelId,
                                    PanelRoleId: item.PanelRoleId,
                                    StartDate: moment(vm.reappointDates.startDate(), CONST.settings.shortDateDisplayFormat).format(CONST.settings.yearMonthDayFormat),
                                    EndDate: moment(vm.reappointDates.endDate(), CONST.settings.shortDateDisplayFormat).format(CONST.settings.yearMonthDayFormat),
                                    PanelMembershipId: 0 // we want to pick up overlaps that include the existing membership
                                });
                            }
                        });
                    });

                //returns true when an overlapping membership is detected.
                panelMemberDataService.post({ Items: overlapRequest }, 'HasOverlappingMembership').then(
                    function (data) {
                        if (data === true) {
                            toastr.error(
                                ko.Localization('Naati.Resources.Panel.resources.OverlappingPanelMembership'));
                        } else {
                            var request = {
                                panelId: vm.panelId(),
                                panelMembershipNumbers: ko.utils.arrayMap(vm.selectedMembers(),
                                    function (row) {
                                        return row[8];
                                    }),
                                startDate: moment(vm.reappointDates.startDate(), CONST.settings.shortDateDisplayFormat).format(CONST.settings.yearMonthDayFormat),
                                endDate: moment(vm.reappointDates.endDate(), CONST.settings.shortDateDisplayFormat).format(CONST.settings.yearMonthDayFormat)
                            };

                            panelMemberDataService.post(request, 'Reappoint').then(
                                function (data) {
                                    toastr.success(
                                        ko.Localization('Naati.Resources.Panel.resources.ReappointedSuccessfully'));

                                    if (data.length > 0) {
                                        vm.getPanelMembers();
                                    }

                                    $('#reappointMembersModal').modal('hide');
                                });
                        }
                    });
            }
        },
        showReappointMembersModal: function (selectedRows) {
            vm.selectedMembers(selectedRows);

            var financialYear = getFinancialYear(moment());

            vm.reappointDates.startDate(financialYear.startDate.format(CONST.settings.shortDateDisplayFormat));
            vm.reappointDates.endDate(financialYear.endDate.format(CONST.settings.shortDateDisplayFormat));

            vm.clearReappointDatesValidation();

            $('#reappointMembersModal').modal('show');
        },
        save: function () {
            if (vm.panelDetailsInstance().validate()) {
                panelDataService.post(vm.parsePanel()).then(function () {
                    toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                    defer.resolve({ name: 'Save', argument: vm.parsePanel() });
                });
            }
        },
        cancel: function () {
            router.navigate('panel');
        },
        showCantDeleteMessage: function () {
            messageService.alert({
                title: 'Can Not Delete Panel',
                content: ko.Localization('Naati.Resources.Panel.resources.HasExaminersAllocated')
            });
        },
        tryDelete: function () {
            messageService.remove().then(function (answer) {
                if (answer === 'yes') {
                    vm.delete();
                }
            });
        },
        'delete': function () {
            panelDataService.remove(vm.panelId()).then(function () {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.DeletedSuccessfully'));
                router.navigate('panels');
                defer.resolve({ name: 'Delete', argument: vm.parsePanel() });
                vm.currentPanel(null);
                router.navigate('panel');
            });
        },
        tryRemoveMember: function (panelMember) {
            messageService.remove().then(function (answer) {
                if (answer === 'yes') {
                    vm.removeMember(panelMember);
                }
            });
        },
        removeMember: function (panelMember) {
            panelMemberDataService.remove(panelMember.PanelMembershipId).then(function () {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.DeletedSuccessfully'));
                vm.getPanelMembers();
            });
        },
        viewMaterialRequests: function (examiner) {
            vm.materialRequests([]);
            $('#materialRequestsModal').modal('show');

            materialRequestDataService.getFluid('panelRequests/' + panelId()).then(function (data) {
                vm.materialRequests(data);
            });
        },
        getMaterialRequests: function (panelId) {
            vm.materialRequests([]);

            if (!currentUser.hasPermissionSync(enums.SecNoun.MaterialRequest, enums.SecVerb.Read)) {
                return;
            }

            materialRequestDataService.getFluid('panelRequests/' + panelId).then(function (data) {

                ko.utils.arrayForEach(data, function (materialRequest) {
                    materialRequest.RoundStatusClass = util.getMaterialRequestRoundStatusCss(materialRequest.RoundStatusTypeId);
                    materialRequest.RequestStatusClass = util.getMaterialRequestStatusCss(materialRequest.RequestStatusTypeId);

                    if (materialRequest.SourceMaterialId == 0) {
                        materialRequest.SourceMaterialId = '';
                    }
                });

                vm.materialRequests(data);
            });
        },
        createNewTestMaterial: function () {
            router.navigate('material-request-wizard/' + enums.MaterialRequestWizardActions.CreateMaterialRequest + '/' + vm.panelId());
        },
        editMaterialRequest: function (materialRequest) {
            router.navigate('material-request/' + materialRequest.Id);
        }
    };

    vm.tabComponentOptions = {
        id: 'panelEditTabs',
        tabs: tabController.generateTabs({
            activeTab: vm.tabId,
            tabs: [
                {
                    id: 'members',
                    name: 'Naati.Resources.Panel.resources.Members',
                    
                    type: 'template',
                    template: {
                        data: vm,
                        name: 'panel-tab-members'
                    },
                    click: vm.onTabClick
                },
                {
                    id: 'panel-notes',
                    name: 'Naati.Resources.Shared.resources.Notes',
                    type: 'template',
                    template: {
                        data: vm,
                        name: 'panel-tab-notes'
                    },
                    click: vm.onTabClick
                },
                {
                    id: 'testMaterialRequests',
                    name: 'Naati.Resources.Panel.resources.TestMaterialRequests',
                    type: 'template',
                    template: {
                        data: vm,
                        name: 'panel-tab-testMaterialRequests'
                    },
                    click: vm.onTabClick
                }
            ]
        })
    };

    vm.reappointDatesValidation = ko.validatedObservable(vm.reappointDates);

    vm.validateReappointDates = function () {
        var isValid = vm.reappointDatesValidation.isValid();

        vm.reappointDatesValidation.errors.showAllMessages(!isValid);

        return isValid;
    };

    vm.clearReappointDatesValidation = function () {
        vm.reappointDatesValidation.errors.showAllMessages(false);
    };


    vm.membersTableComponent.params.dataTable = {
        source: vm.panelMembers,
        select: {
            style: 'multi+shift'
        },
        initComplete: function () {
            currentUser.hasPermission(enums.SecNoun.Panel, enums.SecVerb.Read).then(function (permission) {
                if (permission) {
                    $('#showOldMembership').closest('label').show();
                }
                else {
                    $('#showOldMembership').closest('label').hide();
                }
            });
        },
        columnDefs: [
            { targets: 8, orderable: false },
            { targets: [6, 7], render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat) }
        ],
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
                text: '<span class="glyphicon glyphicon-plus"></span><span>' +
                    ko.Localization('Naati.Resources.Panel.resources.AddMember') + '</span>',
                className: 'btn btn-success',
                enabled: currentUser.hasPermissionSync(enums.SecNoun.PanelMember, enums.SecVerb.Create),
                action: function (e, dt, node, config) {
                    vm.memberDetailsInstance.createMember(vm.panelId()).then(function () {
                        vm.getPanelMembers();
                    });

                }
            }, {
                text: '<span>' + ko.Localization('Naati.Resources.Panel.resources.Reappoint') + '</span>',
                className: 'btn btn-primary m-l',
                action: function (e, dt, node, config) {
                    var selectedRows = dt.rows({ selected: true });

                    if (selectedRows.any()) {
                        vm.showReappointMembersModal(selectedRows.data());
                    }
                }
            }, {
                text: '<input id="showOldMembership" type="checkbox" checked /><i></i><span>' + ko.Localization('Naati.Resources.Panel.resources.ShowOldMembership') + '</span>',
                className: 'i-switch i-switch-md bg-info m-l',
                init: function (dt, node, config) {
                    node.children('input[type="checkbox"]').prop('checked', vm.showOldMembership());
                },
                action: function (e, dt, node, config) {
                    var checked = !node.children('input[type="checkbox"]').prop('checked');
                    node.children('input[type="checkbox"]').prop('checked', checked);
                    vm.showOldMembership(checked);
                    vm.getPanelMembers();
                }
            }]
        }
    };

    vm.materialRequestsTableComponent.params.dataTable = {
        source: vm.materialRequests,
        columnDefs: [
            { targets: -1, orderable: false },
            { targets: 0, render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat) }
        ]
    };

    vm.reappointStartDateOptions = {
        value: vm.reappointDates.startDate,
        resattr: {
            placeholder: 'Naati.Resources.Shared.resources.StartDate'
        }
    };

    vm.reappointEndDateOptions = {
        value: vm.reappointDates.endDate,
        resattr: {
            placeholder: 'Naati.Resources.Shared.resources.EndDate'
        }
    };


    $.extend(vm.noteInstance, {
        getNotesPromise: function () {
            return panelNoteDataService.getFluid(vm.panelId());
        },
        postNotesPromise: function () {
            return panelNoteDataService.post($.extend(vm.noteInstance.parseNote(), {
                PanelId: vm.panelId()
            }));
        },
        removeNotesPromise: function (note) {
            return panelNoteDataService.remove(note.NoteId, vm.panelId());
        }
    });

    function processRequest() {
        var tabId = null;

        if (queryString) {
            tabId = queryString['tab'];
        }
        else {
            tabId = 'members';
        }

        if (!tabId) {
            tabId = 'members';
        }

        vm.tabId(tabId);
    }

    return vm;
});
