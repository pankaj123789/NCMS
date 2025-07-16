define([
    'plugins/router',
    'views/shell',
    'modules/common',
    'modules/enums',
    'services/entity-data-service',
    'services/test-data-service',
    'services/examiner-data-service',
    'services/screen/date-service',
    'views/test/add-examiner',
    'views/test/update-due-date-modal',
    'views/document/document-modal'
], function (router, shell, common, enums, entityService, testService, examinerService, dateService, addExaminer, updateDueDateModal, documentModal) {
    var searchType = enums.SearchTypes.Test;
    var tableId = 'testTable';

    var vm = {
        title: shell.titleWithSmall,
        filters: [
            { id: 'NAATINumber' },
            { id: 'CredentialRequestType' },
            { id: 'Language' },
            { id: 'ApplicationType' },
            { id: 'PreferredTestLocation' },
            { id: 'AttendanceId' },
            { id: 'TestDateFromAndTo' },
            { id: 'TestStatusType' },
            { id: 'ExaminerStatusType' },
            { id: 'AllMarksReceived' },
            { id: 'Examiner' },
            { id: 'IsSupplementary' },
            { id: 'PendingToAssignPaidReviewers' },
            { id: 'ReadyToIssueResults' },
            { id: 'AllowIssue' }
        ],
        tableDefinition: {
            id: tableId,
            headerTemplate: 'test-header-template',
            rowTemplate: 'test-row-template'
        },
        searchType: searchType,
        searchTerm: ko.observable({}),
        tests: ko.observableArray([]),
        selectedTestIndices: ko.observableArray([]),
        updateDueDateModalInstance: updateDueDateModal.getInstance(),
        addExaminerInstance: addExaminer.getInstance(),
        documentModalInstance: documentModal.getInstance(),
        showOutOfDateAlert: ko.observable(false),
        idSearchOptions: {
            search: function (value) {
                var filter = JSON.stringify({ AttendanceIdIntList: [value] });
                testService.get({ request: { Skip: null, Take: 2, Filter: filter }, supressResponseMessages: true }).then(function (data) {
                    if (!data || !data.length) {
                        return toastr.success(ko.Localization('Naati.Resources.Shared.resources.NotFound').format('Test', value));
                    }
                    router.navigate('test/' + value);
                });
            }
        }
    };

    vm.documentModalInstance.documentTableInstance.types(['UnmarkedTestAsset', 'MarkedTestAsset', 'EnglishMarking', 'ReviewReport', 'TestMaterial', 'ProblemSheet', 'MedicalCertificate']);


    var commonFunctions = common.functions();
    var topics = common.topics();

    vm.tableDefinition.dataTable = {
        source: vm.tests,
        columnDefs: [
            { targets: 9, orderable: false },
            { targets: 5, render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateTimeDisplayFormat) }
        ],
        select: {
            style: 'multi+shift'
        },
        events: {
            select: function (e, dt, type, indices) {
                vm.selectedTestIndices(vm.selectedTestIndices().concat(ko.utils.arrayFilter(indices, function (index) {
                    return vm.selectedTestIndices.indexOf(index) === -1;
                })));
            },
            deselect: function (e, dt, type, indices) {
                vm.selectedTestIndices.remove(function (index) {
                    return indices.indexOf(index) !== -1;
                });
            }
        }
    };

    vm.selectedTests = ko.pureComputed(function () {
        return ko.utils.arrayMap($('#' + tableId).DataTable().rows(vm.selectedTestIndices()).data(), function (data) {
            return vm.tests()[data[12]];
        });
    });

    vm.selectedAccreditationDescriptions = ko.pureComputed(function () {
        var tests = vm.selectedTests();

        var accreditationDescriptions = [];
        for (var i = 0; i < tests.length; i++) {
            var t = tests[i];

            var added = $.grep(accreditationDescriptions, function (e) {
                return e.AccreditationDescription === t.AccreditationDescription;
            }).length > 0;

            if (!added) {
                accreditationDescriptions.push({
                    AccreditationDescription: '',
                    Language: {
                        Id: '',
                        Description: ''
                    },
                    Description: ''
                });
            }
        }

        return accreditationDescriptions;
    });

    var oldSelectedAccreditationDescriptionsValue;
    vm.selectedAccreditationDescriptions.subscribe(function (oldValue) {
        oldSelectedAccreditationDescriptionsValue = oldValue;
    }, null, 'beforeChange');

    vm.selectedAccreditationDescriptions.subscribe(function checkSingleLanguageAndLevel(newValue) {
        if ((oldSelectedAccreditationDescriptionsValue || []).length === 1 && newValue.length > 1) {
            toastr.remove();
            toastr.warning(ko.Localization('Naati.Resources.AddExaminer.resources.DifferentLanguageOrTestTypes'));
        }
    });

    function parseSearchTerm(searchTerm) {
        var json = {
            NaatiNumberIntList: searchTerm.NAATINumber ? searchTerm.NAATINumber.Data.NAATINumber : null,
            CredentialRequestTypeIntList: searchTerm.CredentialRequestType ? searchTerm.CredentialRequestType.Data.selectedOptions : null,
            LanguageIntList: searchTerm.Language ? searchTerm.Language.Data.Options : null,
            ApplicationTypeIntList: searchTerm.ApplicationType ? searchTerm.ApplicationType.Data.selectedOptions : null,
            TestLocationIntList: searchTerm.PreferredTestLocation ? searchTerm.PreferredTestLocation.Data.selectedOptions : null,
            AttendanceIdIntList: searchTerm.AttendanceId ? searchTerm.AttendanceId.Data.valueAsArray : null,
            TestDateFromString: searchTerm.TestDateFromAndTo ? searchTerm.TestDateFromAndTo.Data.From : null,
            TestDateToString: searchTerm.TestDateFromAndTo ? searchTerm.TestDateFromAndTo.Data.To : null,
            TestStatusTypeIntList: searchTerm.TestStatusType ? searchTerm.TestStatusType.Data.selectedOptions : null,
            ExaminerStatusTypeIntList: searchTerm.ExaminerStatusType ? searchTerm.ExaminerStatusType.Data.selectedOptions : null,
            AllMarksReceivedBoolean: searchTerm.AllMarksReceived ? searchTerm.AllMarksReceived.Data.checked : null,
            SupplementaryBoolean: searchTerm.IsSupplementary ? searchTerm.IsSupplementary.Data.checked : null,

            JobExminerMembershipIdIntList: searchTerm.Examiner ? searchTerm.Examiner.Data.Examiner : null,
            JobExaminerStatusIntList: searchTerm.Examiner ? searchTerm.Examiner.Data.Status : null,

            JobExaminerSubmittedFromString: searchTerm.Examiner ? searchTerm.Examiner.Data.Submitted.From !== 'Invalid date' ? searchTerm.Examiner.Data.Submitted.From : null : null,
            JobExaminerSubmittedToString: searchTerm.Examiner ? searchTerm.Examiner.Data.Submitted.To : null,

            JobExaminerDueByFromString: searchTerm.Examiner ? searchTerm.Examiner.Data.DueBy.From !== 'Invalid date' ? searchTerm.Examiner.Data.DueBy.From : null : null,
            JobExaminerDueByToString: searchTerm.Examiner ? searchTerm.Examiner.Data.DueBy.To : null,
            PendingToAssignPaidReviewersBoolean: searchTerm.PendingToAssignPaidReviewers ? searchTerm.PendingToAssignPaidReviewers.Data.checked : null,
            ReadyToIssueResultsBoolean: searchTerm.ReadyToIssueResults ? searchTerm.ReadyToIssueResults.Data.checked : null,
            AllowIssueBoolean: searchTerm.AllowIssue ? searchTerm.AllowIssue.Data.checked : null

        };

        $.extend(json, searchTerm.ApplicationStatus);

        return JSON.stringify(json);
    }
    vm.searchCallback = function () {
        vm.selectedTestIndices([]);
        testService.get({ request: { Skip: null, Take: 500, Filter: parseSearchTerm(vm.searchTerm()) } }).then(function (data) {
            vm.tests(data);
            vm.showOutOfDateAlert(false);
        });
    };

    function canAddCommonExaminers(tests) {

        if (!tests.length) {
            return false;
        }
        var firstTest = tests[0];
        if (firstTest.CredentialRequestStatusTypeId !== enums.CredentialRequestStatusTypes.TestSat
            && firstTest.CredentialRequestStatusTypeId !== enums.CredentialRequestStatusTypes.UnderPaidTestReview) {
            return false;
        }
        for (var i = 1; i < tests.length; i++) {
            var test = tests[i];
            if (firstTest.CredentialTypeId !== test.CredentialTypeId ||
                firstTest.SkillId !== test.SkillId ||
                firstTest.StatusTypeId !== test.StatusTypeId ||
                firstTest.CredentialRequestStatusTypeId !== test.CredentialRequestStatusTypeId) {
                return false;
            }
        }
        return true;
    }

    vm.addExaminer = function () {
        var tests = vm.selectedTests();
        if (!tests.length) {
            toastr.warning(ko.Localization('Naati.Resources.Shared.resources.NoRecordsWereSelected'));
            return;
        }

        if (!canAddCommonExaminers(tests)) {
            toastr.warning(ko.Localization('Naati.Resources.Shared.resources.InvalidSelectedTests'));
            return;
        }

        showExaminerModal();
    };

    vm.clearSelectList = function () {
        vm.selectedTestIndices([]);
        return true;
    };

    vm.uploadAssets = function (test) {
        vm.documentModalInstance.show(test);

        vm.documentModalInstance.parseFileName = function (options) {
            test.TestAttendanceId = test.AttendanceId;
            return commonFunctions.testAssetFileNameProcessor($.extend({}, options, { Test: test }));
        };
    };

    vm.updateDueDate = function () {
        var tests = vm.selectedTests();
        if (!tests || !tests.length) {
            toastr.warning(ko.Localization('Naati.Resources.Shared.resources.NoRecordsWereSelected'));
            return;
        }

        var jobs = $.map(tests, function (t) {
            return t.JobId;
        });

        if (jobs.length == 0) {
            toastr.warning(ko.Localization('Naati.Resources.Test.resources.CannotUpdateDueDate'));
        } else {
            if (!tests.some(function (test) {
                return test.JobId === '';
            })) {
                vm.updateDueDateModalInstance
                    .show(jobs)
                    .then(function (newDueDate) {
                        for (var i = 0; i < tests.length; i++) {
                            tests[i].JobDueDate = dateService.toPostDate(newDueDate);
                        }
                        vm.updateDueDateModalInstance.close();
                    });
            } else {
                toastr.error(ko.Localization('Naati.Resources.Test.resources.CannotAlterDueDateWithoutAMarker'));
            }
        }
    };

    vm.disableAddExaminers = ko.pureComputed(function () {

        return !vm.selectedTests().length;
    });

    vm.additionalButtons = [{
        'class': 'btn btn-success',
        click: vm.addExaminer,
        disable: vm.disableAddExaminers,
        icon: 'glyphicon glyphicon-plus',
        resourceName: 'Naati.Resources.Test.resources.AddExaminer',
        enableWithPermission: 'Examiner.Create'
    },
    {
        'class': 'btn btn-default',
        click: vm.updateDueDate,
        icon: 'glyphicon glyphicon-calendar',
        resourceName: 'Naati.Resources.Test.resources.UpdateDueDate',
        enableWithPermission: 'ExaminerMarks.Update'
    }];


    vm.showDetails = function (i, e) {
        var target = $(e.target);
        var tr = target.closest('tr');
        var dt = tr.closest('#' + vm.tableDefinition.id).DataTable();
        var row = dt.row(tr);

        if (row.child.isShown()) {
            target.removeClass('fa-chevron-down').addClass('fa-chevron-right');
            tr.removeClass('details');
            row.child.hide();
        }
        else {
            target.removeClass('fa-chevron-right').addClass('fa-chevron-down');
            tr.addClass('details');
            showExaminers(i, row);
        }
    };

    vm.activate = function () {
        $.fn.dataTable.moment(CONST.settings.shortDateDisplayFormat);
    };

    function getCleanTestServerModel() {
        var testServerModel = {
            TestAttendanceId: ko.observable(),
            ApplicationId: ko.observable(),
            ApplicationReference: ko.observable(),
            ApplicationTypeId: ko.observable(),
            CredentialTypeId: ko.observable(),
            SkillId: ko.observable(),
            ApplicationType: ko.observable(),
            CredentialTypeInternalName: ko.observable(),
            Skill: ko.observable(),
            ResultId: ko.observable(),
            CurrentJobId: ko.observable(),
            LastReviewTestResultId: ko.observable(),
            Language1Id: ko.observable(),
            Language2Id: ko.observable(),
            TestDate: ko.observable(),
            TestStatusTypeId: ko.observable(),
            TestStatus: ko.observable(),
            TestLocationId: ko.observable(),
            TestLocation: ko.observable(),
            VenueId: ko.observable(),
            Venue: ko.observable(),
            TestResultStatus: ko.observable(),
            TestResultStatusId: ko.observable(),
            PersonId: ko.observable(),
            NaatiNumber: ko.observable(),
            TestMaterialIds: ko.observableArray(),
            TestMaterialNames: ko.observableArray(),
            Actions: ko.observableArray(),
            ApplicationStatusTypeId: ko.observable(),
            CredentialRequestId: ko.observable(),
            CredentialRequestStatusTypeId: ko.observable(),
        }

        return testServerModel;
    }

    function showExaminerModal() {

        var defer = Q.defer;

        var firstItem = vm.selectedTests()[0];

        testService.getFluid(firstItem.AttendanceId + '/summary').then(function (data) {
            var testModel = getCleanTestServerModel();
            ko.viewmodel.updateFromModel(testModel, data);

            var options = {
                testAttendanceList: vm.selectedTests(),
                test: testModel
            };

            vm.addExaminerInstance.showAdd(options).then(function () {
                vm.searchCallback();
                vm.addExaminerInstance.close();
                defer.resolve();
            });
        });

        return defer.promise;
    }

    function showExaminers(test, row) {
        examinerService.get({ request: JSON.stringify({ TestAttendanceIds: [test.AttendanceId] }) }).then(function (data) {
            var rowTemplate = $('#test-examiner-row').html();
            var content = $('#test-row-detail').html();
            var rows = '';

            if (data.length)
                for (var i = 0; i < data.length; i++) {
                    var d = data[i];
                    rows += rowTemplate.format(
                        d.Examiner.PersonName + (moment(d.Examiner.MembershipEndDate).toDate() < new Date() ? ' (' + ko.Localization('Naati.Resources.Shared.resources.Inactive') + ')' : ''),
                        ko.Localization('Naati.Resources.ExaminerStatus.resources.' + d.ExaminerStatusName),
                        dateService.toUIDate(d.DateAllocated),
                        dateService.toUIDate(d.JobDueDate),
                        dateService.toUIDate(d.ExaminerReceivedDate)
                    );
                }

            content = content.format(
                ko.Localization('Naati.Resources.Test.resources.Marker'),
                ko.Localization('Naati.Resources.Test.resources.MarkerStatus'),
                ko.Localization('Naati.Resources.Test.resources.DateAllocated'),
                ko.Localization('Naati.Resources.Test.resources.DateDue'),
                ko.Localization('Naati.Resources.Test.resources.DateReceived'),
                rows
            );

            row.child(content).show();
        });
    }
    return vm;
});
