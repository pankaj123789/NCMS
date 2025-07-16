define([
    'plugins/router',
    'modules/common',
    'modules/enums',
    'modules/custom-validator',
    'services/util',
    'services/screen/message-service',
    'services/test-material-data-service',
    'services/testsession-data-service',
    'services/screen/date-service',
    'services/application-data-service',
    'views/test-session/assign-material',
    'views/test-session/download-material',
    'views/test-session/validate-test-sitting'
],
    function (router, common, enums, customValidator, util, messageService, testmaterialService, testSessionService, dateService, applicationService, materialModel, downloadMaterialModel, validateTestSittinModel) {

        var queryString;

        var serverModel = {
            TestSessionId: ko.observable(0),
            CredentialType: ko.observable(),
            CredentialTypeId: ko.observable(),
        };

        var dirtyFlag = new ko.DirtyFlag([serverModel], false);
        var validator = customValidator.create(serverModel);
        var emptyModel = ko.toJS(serverModel);
        var bypassUserSelect = false;

        var vm = {
            testsession: serverModel,
            readOnly: ko.observable(),
            actions: ko.observableArray(),
            testSessionName: ko.observable(),
            testSessionAddress: ko.observable(),
            testSessionAddressUrl: ko.observable(),
            txtMarkAsComplete: ko.observable(),
            isCompletedStatus: ko.observable(false),
            testsessionApplicants: ko.observableArray(),
            materials: {},

            selectedApplicants: ko.observableArray(),
            existingApplicants: ko.observableArray(),

            createMaterialInstance: materialModel.getInstance(),
            downloadAllMaterialInstance: downloadMaterialModel.getInstance(),
            validateTestSittingInstance: validateTestSittinModel.getInstance(),

            tableDefinition: {
                id: 'testsitting-table',
                headerTemplate: 'testsitting-header-template',
                rowTemplate: 'testsetting-row-template'
            },
        };

        vm.tableDefinition.dataTable = {
            source: vm.testsessionApplicants,
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
                buttons: [
                    {
                        text: '<span class="glyphicon glyphicon-export"></span><span>' + ko.Localization('Naati.Resources.Shared.resources.ExportToCSV') + '</span>',
                        className: 'btn btn-default',
                        enabled: true,
                        extend: 'csv',
                        title: 'Applicants {0}'.format(moment().format('YYYY-MM-DD')),
                        exportOptions: {
                            columns: ':not(:last-child)',
                            format: {
                                body: function (data, column) {
                                    var value = $.trim($('<div>').html(data).text());
                                    if (column === 9) {
                                        var $el = $(data);
                                        value = ko.utils.arrayMap($el.find('li'), function (li) {
                                            return $(li).text();
                                        }).join(' ');
                                    }
                                    return value;
                                }
                            }
                        }
                    },
                    {
                        text: '<i class="fa fa-plus-square"></i><span> Expand All</span>',
                        className: 'btn btn-default ml-1',
                        enabled: true,
                        action: expandAll
                    },
                    {
                        text: '<i class="fa fa-minus-square-o"></i><span> Collapse All</span>',
                        className: 'btn btn-default ml-1',
                        enabled: true,
                        action: collapseAll
                    }
                ]
            },
            initComplete: enableDisableDataTableButtons,
            columnDefs: [
                {
                    targets: -1,
                    orderable: false
                },
                {
                    targets: 3,
                    render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                }
            ],
            order: [
                [0, "asc"]
            ],
            select: {
                style: 'multi+shift'
            },
            events: {
                select: selectTable,
                deselect: selectTable,
                'user-select': function (e) {
                    if (vm.readOnly() || bypassUserSelect) {
                        e.preventDefault();
                    }
                }
            }
        };

        vm.selectedIds = function () {
            return ko.utils.arrayMap(vm.selectedApplicants(), function (s) {
                return s.AttendanceId();
            });
        };

        vm.selectedSkills = function () {
            return ko.utils.arrayMap(vm.selectedApplicants(), function (s) {
                return s.Skill();
            });
        };

        var bypassSelect = false;
        function selectTable(e, dt) {
            if (bypassSelect) {
                return;
            }

            var indexes = dt.rows('.selected').indexes();
            vm.selectedApplicants([]);

            if (!indexes.length) {
                return;
            }

            for (var i = 0; i < indexes.length; i++) {
                vm.selectedApplicants.push(vm.testsessionApplicants()[indexes[i]]);
            }
        }

        $.extend(vm, {
            title: ko.computed(function () {
                return '{0}'.format(vm.testSessionName());
            }),
            subtitle: ko.computed(function () {
                return '{0}'.format(vm.testSessionAddress());
            })
        });

        vm.canActivate = function (testsessionId, query) {
            queryString = query || {};
            testsessionId = parseInt(testsessionId);

            ko.viewmodel.updateFromModel(serverModel, emptyModel);

            serverModel.TestSessionId(testsessionId);

            loadData();

            return true;
        };

        vm.canAssign = ko.computed(function () {
            return vm.selectedApplicants().length > 0;
        });

        vm.canRemoveTestMaterials = ko.computed(function () {
            return vm.selectedApplicants().length > 0;
        });

        vm.canDownload = ko.computed(function () {
            return false;
        });

        vm.canMarkAsComplete = ko.computed(function () {

            if (vm.txtMarkAsComplete() === 'Reopen Test Session') {
                return currentUser.hasPermission(enums.SecNoun.TestSitting, enums.SecVerb.Manage)
                    .then(function (hasAdminPermission) {
                        return hasAdminPermission;
                    });
            }

            return true;
        });


        vm.markAsComplete = function () {

            messageService.confirm({ title: ko.Localization('Naati.Resources.Shared.resources.AreYouSure'), content: vm.txtMarkAsComplete() }).then(
                function (answer) {
                    if (answer === 'yes') {

                        if (vm.txtMarkAsComplete() === "Mark as Complete") {
                            testSessionService.getFluid(serverModel.TestSessionId() + '/markascomplete').then(function (data) {
                                if (data === 'ApplicantNotSit') {
                                    toastr.error(ko.Localization('Naati.Resources.TestSession.resources.CanNotMarkAsCompleted'));

                                } else if (data === 'ApplicantTestMaterialNotAssigned') {
                                    toastr.error(ko.Localization('Naati.Resources.TestSession.resources.ApplicantTestMaterialNotAssigned'));
                                }
                                else {
                                    vm.txtMarkAsComplete('Reopen Test Session');
                                    toastr.success('Test session marked as complete.');
                                }
                            });
                        } else {

                            var t = String(vm.txtMarkAsComplete());
                            if (t === "Reopen Test Session") {
                                testSessionService.getFluid(serverModel.TestSessionId() + '/reopentestsession').then(function (data) {
                                    vm.txtMarkAsComplete('Mark as Complete');
                                    toastr.success('Test session reopened.');
                                });
                            }
                        }

                    }
                });
        }

        vm.validatesTestSitting = function () {

            testSessionService.getFluid(serverModel.TestSessionId() + '/validateTestSitting').then(function (data) {
                if (!data.length) {
                    toastr.success(ko.Localization('Naati.Resources.TestSession.resources.NoIssues'));
                } else {
                    vm.validateTestSittingInstance.show(data);
                }
            });



        }

        vm.back = function () {
            router.navigateBack();
            return false;
        };

        vm.assignTestMaterial = function () {
            if (vm.selectedApplicants().length === 0) {
                toastr.error('You must select the applicants to assign test material.');
            } else {
                if (vm.selectedSkills().length > 1) {
                    if (vm.selectedApplicants()[0].Supplementary()) {
                        return toastr.error("You must assign test materials for supplementary tests one test at a time.");
                    }

                    var allElementSame = vm.selectedSkills().every(function (val, i, arr) { return val === arr[0]; });
                    if (!allElementSame) {
                        toastr.error("You must select applicants who have the same skill.");
                    } else {
                        getTestMaterialByAttendenceId(vm.selectedIds());
                    }

                } else {
                    getTestMaterialByAttendenceId(vm.selectedIds());
                }
            }

            return true;
        };

        vm.removeTestMaterials = function () {
            if (vm.selectedApplicants().length === 0) {
                toastr.error('You must select at least one applicant to remove Test Material.');
            } else {

                removeTestMaterialByAttendenceId(vm.selectedIds());
            }

            return true;
        };


        function removeTestMaterialByAttendenceId(ids) {

            messageService.confirm({ title: ko.Localization('Naati.Resources.Shared.resources.AreYouSure'), content: ko.Localization('Naati.Resources.TestSession.resources.RemoveTestMaterialsConfirmMessage') }).then(
                function (answer) {
                    if (answer === 'yes') {

                        vm.createMaterialInstance.removeTestMaterials(ids).then(function (data) {
                            reloadData().then(function () {
                                toastr.success('Successfully remove test materials.');
                            });
                        });
                    }
                });
        }


        vm.showDetails = function (i, e) {
            var target = $(e.target);
            var tr = target.closest('tr');
            var dt = tr.closest('#' + vm.tableDefinition.id).DataTable();
            var row = dt.row(tr);

            if (row.child.isShown()) {
                target.removeClass('fa-chevron-down').addClass('fa-chevron-right');
                row.child.hide();
            } else {
                target.removeClass('fa-chevron-right').addClass('fa-chevron-down');
                showLines(i, row);
            }
        }

        vm.downloadAllMaterial = function () {
            vm.downloadAllMaterialInstance.downloadAllMaterial(serverModel.TestSessionId());
        };

        vm.getActions = function (applicant, event) {
            bypassUserSelect = true;
            testSessionService.getFluid('actions', { TestSittingId: applicant.TestSittingId() }).then(function (data) {
                bypassUserSelect = false;

                if (!data.length) {
                    data = [
                        {
                            Name: ko.Localization(
                                'Naati.Resources.CredentialRequestSummary.resources.NoActionsAvailable'),
                            Disabled: true
                        }
                    ];
                }

                applicant.Actions(data);
            });
        };

        vm.takeAction = function (applicantInfo, action) {
            bypassUserSelect = true;

            if (action.Disabled) {
                event.preventDefault();
                bypassUserSelect = false;
                return false;
            }

            var request = {
                ApplicationStatusId: applicantInfo.ApplicationStatusId(),
                ActionId: action.Id,
                ApplicationId: applicantInfo.ApplicationId(),
                CredentialRequestId: applicantInfo.CredentialRequestId(),
                CredentialTypeId: applicantInfo.CredentialTypeId(),
                ApplicationTypeId: applicantInfo.ApplicationTypeId()
            };

            applicationService.post(request, 'action').then(function (data) {
                bypassUserSelect = false;

                if (!checkAndShowMessages(data)) {
                    applicationService.getFluid('steps', request).then(function (steps) {
                        if (steps.length) {
                            return router.navigate('application-wizard/' + applicantInfo.ApplicationId() + '/' + action.Id + '/' + applicantInfo.CredentialRequestId());
                        }

                        applicationService.post(request, 'wizard').then(function () {
                            reloadData().then(function () {
                                toastr.success(ko.Localization('Naati.Resources.Application.resources.ActionTakenSuccessfully'));
                            });
                        });
                    });
                }
            });
        };


        function expandAll() {
            $('.fa-chevron-right').each(function (i, e) {
                var target = $(e);

                var tr = target.closest('tr');

                var dt = tr.closest('#' + 'testsitting-table').DataTable();
                var row = dt.row(tr);

                if (row.child.hide()) {
                    target.click();
                }
            });
        }

        function collapseAll() {
            $('.fa-chevron-down').each(function (i, e) {
                var target = $(e);

                var tr = target.closest('tr');

                var dt = tr.closest('#' + 'testsitting-table').DataTable();
                var row = dt.row(tr);

                if (row.child.isShown()) {
                    target.click();
                }
            });
        }

        function getTestMaterialByAttendenceId(ids) {
            var firstApplicant = vm.selectedApplicants()[0];

            var options = {
                TestSittingIds: ids,
                ShowAllMaterials: false,
                Skill: firstApplicant.Skill(),
                SkillId: firstApplicant.SkillId(),
                TestTasks: firstApplicant.SortedTestTasks,
            };

            $.extend(options, ko.toJS(serverModel));

            vm.createMaterialInstance.show(options).then(function (data) {
                reloadData();
            });
        }

        function enableDisableDataTableButtons() {
            var $table = $('#' + vm.tableDefinition.id);

            if (!$.fn.DataTable.isDataTable($table)) {
                setTimeout(enableDisableDataTableButtons, 100);
                return;
            }

            var buttons = $table.DataTable().buttons('*');

            if (!buttons.length) {
                setTimeout(enableDisableDataTableButtons, 100);
                return;
            }

            var disable = vm.testsessionApplicants().length === 0;
            if (disable) {
                buttons.disable();
            }
            else {
                buttons.enable();
            }
        }

        function showLines(testsessionApplicant, row) {

            var data = {
                personPhotoOptions: {
                    naatiNumber: testsessionApplicant.CustomerNo
                },
                testTasks: testsessionApplicant._testTasks
            };

            var detailsTemplate = ko.generateTemplate('testsitting-applicant-template', data, '<div>');
            row.child(detailsTemplate, 'table-row-separator').show();
        }

        function buildTestTasks(testsessionApplicant) {
            var testTasks = ko.observableArray([]);
            testsessionApplicant.TestTasks.subscribe(function () {
                updateTestTasks(testTasks, testsessionApplicant);
            });
            updateTestTasks(testTasks, testsessionApplicant);
            return testTasks;
        }

        function updateTestTasks(testTasks, testsessionApplicant) {
            ko.utils.arrayForEach(testTasks(), function (t) {
                t.TestMaterialIds([]);
            });

            ko.utils.arrayForEach(testsessionApplicant.TestTasks(), function (t) {
                var testTask = ko.utils.arrayFirst(testTasks(), function (tt) {
                    return t.TestComponentId() === tt.TestComponentId();
                });

                if (testTask) {
                    if (t.TestMaterialId()) {
                        testTask.TestMaterialIds.push(t.TestMaterialId());
                    }

                    testTask.dirtyFlag().reset();
                    return;
                }

                testTask = ko.viewmodel.fromModel(ko.toJS(t));
                testTask.TestsessionApplicant = testsessionApplicant;
                testTask.TestMaterialIds = ko.observableArray();
                if (t.TestMaterialId()) {
                    testTask.TestMaterialIds.push(t.TestMaterialId());
                }

                testTask.save = function () { return save(testTask); };
                testTask.remove = remove;

                testTasks.push(testTask);

                testTask.materialOptions = {
                    selectedOptions: testTask.TestMaterialIds,
                    multiple: true,
                    options: ko.observableArray()
                };

                cacheMaterials(t.TestComponentId(), testsessionApplicant.SkillId()).then(testTask.materialOptions.options);

                testTask.dirtyFlag = new ko.DirtyFlag(testTask.TestMaterialIds);
                testTask.tooltip = { trigger: 'hover', container: 'body' };
            });
        }

        function cacheMaterials(testComponentId, skillId) {
            var key = '{0}_{1}'.format(testComponentId, skillId);
            var materials = vm.materials[key];

            if (materials) {
                return materials.promise;
            }

            materials = Q.defer();
            vm.materials[key] = materials;

            var request = {
                TestComponentId: testComponentId,
                SkillId: skillId,
                IncludeSystemValueSkillTypes: false
            };

            testmaterialService.getFluid('fromtesttask', request).then(function (data) {
                var options = ko.utils.arrayMap(data, function (d) {
                    if (d.IsTestMaterialTypeSource) {
                        return {
                            value: d.Id,
                            text: '{0} {1} {2} {3} - {4} ({5})'.format(
                                d.Id,
                                d.Title,
                                d.Language,
                                d.CredentialType,
                                d.Domain,
                                ko.Localization('Naati.Resources.TestMaterial.resources.Source'))
                        };
                    }
                    else {
                        return {
                            value: d.Id,
                            text: '{0} {1} {2} {3} - {4}'.format(
                                d.Id,
                                d.Title,
                                d.Language,
                                d.CredentialType,
                                d.Domain)
                        };
                    }
                });

                materials.resolve(options);
            });

            return materials.promise;
        }

        function save(testTask, successMessage) {
            if (!successMessage) {
                successMessage = ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully');
            }

            var testsessionApplicant = testTask.TestsessionApplicant;
            var request = {
                TestSittingIds: [testsessionApplicant.AttendanceId()],
                TestComponentIds: {}
            };

            request.TestComponentIds[testTask.TestComponentId()] = testTask.TestMaterialIds();

            testmaterialService.post(request, 'assignMaterial').then(function (data) {
                reloadData().then(function () {
                    toastr.success(successMessage);
                });
            });
        }

        function remove(testTask) {
            messageService.remove().then(function (answer) {
                if (answer === 'yes') {
                    testTask.TestMaterialIds.removeAll();
                    save(testTask, ko.Localization('Naati.Resources.Shared.resources.DeletedSuccessfully'));
                }
            });
        }

        function checkAndShowMessages(messages) {
            var genericMessages = [];

            ko.utils.arrayForEach(messages, function (m) {
                if (!m.Field) {
                    genericMessages.push(m.Message);
                }
            });

            if (genericMessages.length) {
                toastr.error(genericMessages.join('<br /><br />'), null, {
                    closeButton: true,
                    timeOut: 0
                });
            }

            ko.utils.arrayForEach(messages, function (i) {
                if (!i.Field) {
                    return;
                }
                validator.setValidation(i.Field, false, i.Message);
            });

            return messages && messages.length;
        }

        function loadData() {
            loadtopsection();
            applicants();
        }

        function loadtopsection() {
            testSessionService.getFluid(serverModel.TestSessionId() + '/topsection').then(function (data) {
                ko.viewmodel.updateFromModel(serverModel, data);

                vm.isCompletedStatus(data.IsCompletedStatus);
                var strIsCompleted = data.IsCompletedStatus ? 'Completed' : 'To be sat';
                var heardingFistLine = 'TS' + data.Id + ', ' + data.Name + ' - ' + strIsCompleted;
                var heardingSecondLine = data.VenueAddress;
                var heardingSecondLineUrl = "https://www.google.com.au/maps/place/" + heardingSecondLine.replace('/', '%2F');
                vm.testSessionName(heardingFistLine);
                vm.testSessionAddress(heardingSecondLine);
                vm.testSessionAddressUrl(heardingSecondLineUrl);

                if (strIsCompleted === 'Completed') {
                    vm.txtMarkAsComplete('Reopen Test Session');
                } else {
                    vm.txtMarkAsComplete('Mark as Complete');
                }

            });
        }

        function reloadData() {
            loadtopsection();
            return getApplicantsByTestSitting().then(function (applicants) {
                ko.utils.arrayForEach(applicants, function (a) {
                    var app = ko.utils.arrayFirst(vm.testsessionApplicants(), function (b) {
                        return a.AttendanceId === b.AttendanceId();
                    });

                    if (!app) {
                        return;
                    }

                    delete a.Actions;
                    ko.viewmodel.updateFromModel(app, a);
                });
            });
        }

        function onlyUnique(value, index, self) {
            return self.indexOf(value) === index;
        }

        function applicants() {
            getApplicantsByTestSitting().then(function (apps) {
                var applicants = [];

                ko.utils.arrayForEach(apps,
                    function (a) {
                        var applicant = ko.viewmodel.fromModel(a);
                        applicant.SortedTestTasks = ko.pureComputed(function () {
                            var testTasks = applicant.TestTasks();
                            testTasks.sort(util.sortBy('TestComponentTypeLabel',
                                'TestComponentName',
                                'TestMaterialId'));
                            return testTasks;
                        });

                        applicant.TestTaskHtml = ko.pureComputed(function () {

                            var data = applicant.SortedTestTasks();
                            var testComponentNames = [];
                            data.forEach(function (el) {
                                testComponentNames.push(el.TestComponentName());
                            });

                            var uniqueTestComponentNames = testComponentNames.filter(onlyUnique);
                            var testTaskHtml = '<ul class="m-l-n-md">';

                            uniqueTestComponentNames.forEach(function (s) {

                                var taskrecords = data.filter(function (el) {
                                    return (el.TestComponentName() === s);
                                });

                                var testTask = '<li>';

                                var index = 0;
                                ko.utils.arrayForEach(taskrecords,
                                    function (item) {

                                        if (item.TestMaterialId() !== null) {

                                            var url = '#/test-material/' + item.TestMaterialId();

                                            var anchorWithLabel = '<a class="text-info" target="_blank" href ="' +
                                                url +
                                                '">' +
                                                item.TestComponentTypeLabel() +
                                                item.Label() +
                                                '- #' +
                                                item.TestMaterialId() +
                                                ' - ' +
                                                item.Domain() +
                                                '</a>';
                                            var anchor = '<a class="text-info" target="_blank" href ="' +
                                                url +
                                                '">#' +
                                                item.TestMaterialId() +
                                                '</a>';
                                            if (index === 0) {
                                                testTask += anchorWithLabel + (taskrecords.length > 1 ? ', ' : '');
                                            } else if (index !== (taskrecords.length - 1)) {
                                                testTask += anchor + ', ';
                                            } else if (index === (taskrecords.length - 1)) {
                                                testTask += anchor;
                                            }
                                        } else {
                                            testTask += item.TestComponentTypeLabel() + item.Label();
                                        }
                                        index++;

                                    });

                                testTask += '</li>';
                                testTaskHtml += testTask;
                            });

                            testTaskHtml += '</ul>';

                            return testTaskHtml;
                        });

                        applicant._testTasks = buildTestTasks(applicant);
                        applicants.push(applicant);
                    });
                vm.testsessionApplicants(applicants);
            });
        }

        function getApplicantsByTestSitting() {
            var defer = Q.defer();

            testSessionService.getFluid(serverModel.TestSessionId() + '/applicantsbytestsitting').then(function (data) {
                ko.utils.arrayForEach(data, function (item) {
                    item.Gender = getGender(item.Gender);
                    item.ShowAttendanceLink = item.Status === enums.CredentialRequestStatusTypes.TestSat || item.Status === enums.CredentialRequestStatusTypes.TestFailed;
                    item.StatusCss = util.getTestSittingStatusCss(item);
                    item.CredentialRequestStatusId = item.Status;
                    item.Status = util.getTestSittingStatus(item);
                    item.Actions = [];
                    item.CharacterTypeTraditional = item.LanguageCharacterType == 'Traditional Chinese';
                    item.CharacterTypeSimplified = item.LanguageCharacterType == 'Simplified Chinese';
                });

                defer.resolve(data);
            });

            return defer.promise;
        }

        function getGender(genderCode) {
            var gender = null;
            for (var g in enums.Genders) {
                if (enums.Genders[g] === genderCode) {
                    gender = g;
                    break;
                }
            }
            if (gender == null) return "";
            return ko.Localization('Naati.Resources.Shared.resources.' + gender);
        }

        function getStatus(statusModifiedDate, status, rejected) {
            var strStatus = 'N/A';
            var statuses = enums.CredentialRequestStatusTypes;

            if (status === statuses.TestSessionAccepted) {
                strStatus = "Confirmed on " + moment(statusModifiedDate).format(CONST.settings.shortDateDisplayFormat);;
            }

            if (status === statuses.CheckIn) {
                strStatus = "Checked In";
            }

            if (status === statuses.TestSat) {
                strStatus = "Sat";
            }

            if (rejected) {
                strStatus = "Session Rejected";
            }

            return strStatus;
        }

        function getStatusCss(status, rejected) {

            var strStatusCss = 'gray';
            var statuses = enums.CredentialRequestStatusTypes;

            if (status === statuses.TestSessionAccepted) {
                strStatusCss = 'info';
            }

            if (status === statuses.CheckIn) {
                strStatusCss = 'orange';
            }

            if (status === statuses.TestSat || status === statuses.TestFailed) {
                strStatusCss = 'success';
            }

            if (rejected) {
                strStatusCss = "gray";
            }
            return strStatusCss;
        }

        return vm;

    });