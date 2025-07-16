define([
    'views/shell',
    'plugins/router',
    'modules/enums',
    'services/test-material-data-service',
    'services/job-data-service',
    'services/examiner-data-service',
    'views/test-material/request/edit/index',
    'services/screen/date-service'
], function(shell, router, enums, testMaterialService, jobService, examinerService, dateService) {
    var SearchType = enums.SearchTypes.TestMaterial;

    var vm = {
        title: shell.titleWithSmall,
        filters: [
            { id: 'NAATINumber' },
            { id: 'Language' },
            { id: 'Category' },
            { id: 'Level' },
            { id: 'Direction' },
            { id: 'DateSent' },
            { id: 'DateReceived' },
            { id: 'JobID' },
            { id: 'RequestStatus' }
        ],
        tableDefinition: {
            id: 'testMaterialTable',
            headerTemplate: 'testmaterialrequest-header-template',
            rowTemplate: 'testmaterialrequest-row-template'
        },
        searchType: SearchType,
        searchTerm: ko.observable({}),
        testMaterials: ko.observableArray([])
    };

    vm.tableDefinition.dataTable = {
        source: vm.testMaterials,
        columnDefs: [
            { targets: -1, orderable: false }
        ]
    };

    vm.searchCallback = function () {
        var json = parseTerm(vm.searchTerm());
        json.Type = SearchType;
        testMaterialService.getFluid('request', { request: JSON.stringify(json) }).then(function(data) {
            vm.testMaterials(data);
        });
    };

    vm.create = function () {
        router.navigate('materialreq');
    };

    vm.approve = function (data) {
        var jobId = data.JobID;
        jobService.post(null, jobId + '/approve').then(function (json) {
            toastr.success(ko.Localization('Naati.Resources.TestMaterial.resources.ApprovedSuccessfully'));
            vm.searchCallback();
        });
    };

    vm.additionalButtons = [{
        'class': 'btn btn-success',
        click: vm.create,
        icon: 'glyphicon glyphicon-plus',
        resourceName: 'Naati.Resources.Shared.resources.Create'
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
            showExaminers(row);
        }
    };

    vm.activate = function () {
        $.fn.dataTable.moment(CONST.settings.shortDateDisplayFormat);
    };

    function parseTerm(searchTerm) {
        return {
            NAATINumber: searchTerm.NAATINumber ? { Data: searchTerm.NAATINumber.Data } : null,
            Language: searchTerm.Language ? { Data: searchTerm.Language.Data } : null,
            Category: searchTerm.Category ? { Data: searchTerm.Category.Data } : null,
            Level: searchTerm.Level ? { Data: searchTerm.Level.Data } : null,
            Direction: searchTerm.Direction ? { Data: searchTerm.Direction.Data } : null,
            DateSent: searchTerm.DateSent ? { Data: searchTerm.DateSent.Data, Type: 'TestDateFromAndTo' } : null,
            DateReceived: searchTerm.DateReceived ? { Data: searchTerm.DateReceived.Data, Type: 'TestDateFromAndTo' } : null,
            JobID: searchTerm.JobID ? { Data: searchTerm.JobID.Data, Type: 'JobID' } : null,
            RequestStatus: searchTerm.RequestStatus ? { Data: searchTerm.RequestStatus.Data } : null
        };
    }

    function showExaminers(row) {
        examinerService.get({ request: JSON.stringify({ JobId: [row.data()[1]], ActiveExaminersOnly: false }) }).then(function (data) {
            var rowTemplate = $('#examinerRow').html();
            var content = $('#testRowDetail').html();
            var rows = '';

            if (data.length) {
                for (var i = 0; i < data.length; i++) {
                    var d = data[i];
                    rows += rowTemplate.format(
                        d.NAATINumber,
                        d.PersonName + (moment(d.EndDate).toDate() < new Date() ? ' (' + ko.Localization('Naati.Resources.Shared.resources.Inactive') + ')' : ''),
                        dateService.toUIDate(d.ExaminerSentDate),
                        dateService.toUIDate(d.ExaminerReceivedDate),
                        '$' + parseFloat(Math.round(d.ExaminerCost * 100) / 100).toFixed(2)
                    );
                }
            }

            content = content.format(
                ko.Localization('Naati.Resources.Shared.resources.NAATINumber'),
                ko.Localization('Naati.Resources.TestMaterial.resources.Examiner'),
                ko.Localization('Naati.Resources.TestMaterial.resources.Sent'),
                ko.Localization('Naati.Resources.TestMaterial.resources.Received'),
                ko.Localization('Naati.Resources.TestMaterial.resources.Cost'),
                rows
            );

            row.child(content).show();
        });
    }

    return vm;
});
