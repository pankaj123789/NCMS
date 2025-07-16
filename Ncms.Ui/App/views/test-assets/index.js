define([
    'views/shell',
    'modules/enums',
    'services/testasset-data-service',
    'services/screen/date-service',
    'services/util',
    'services/screen/message-service',
    'services/file-data-service'
],
    function(shell, enums, testassetDataService, dateService, util, messageService, fileService) {
        var vm = {
            title: shell.titleWithSmall,
            filters: [
                { id: 'NAATINumber' },
                { id: 'TestOffice' },
                { id: 'TestDateRange', name: 'Naati.Resources.TestAsset.resources.TestSatDateFromAndTo' },
                { id: 'JobID' },
                { id: 'UploadedByPerson' },
                { id: 'TestID' }
            ],
            tableDefinition: {
                id: 'testassetTable',
                headerTemplate: 'testasset-header-template',
                rowTemplate: 'testasset-row-template'
            },
            searchType: enums.SearchTypes.TestAsset,
            searchTerm: ko.observable({}),
            testassets: ko.observableArray([]),
            selectedTestIndices: ko.observableArray()
        };

        vm.downloadAsset = function (testasset) {
            return testassetDataService.url() +
                '/DownloadAsset?testAttendanceAssetId=' +
                testasset.TestAttendanceAssetId;
        };

        vm.documentType = function (type) {
            return ko.Localization('Naati.Resources.TestAsset.resources.DocumentType' + type);
        };

        vm.bulkDownload = function() {
            var assets = [];
            $.each(vm.selectedTests(),
                function(i, v) {
                    assets = assets + v.StoredFileId + '%2C';
                });

            if (assets.length > 0) {
                util.downloadFile('api/testasset/BulkDownloadAssets?storedFileIds=' + assets);
            }
        };

        vm.tryDelete = function (testasset) {
            messageService.remove()
                .then(function(answer) {
                    if (answer === 'yes') {
                        fileService.remove(testasset.StoredFileId).then(function () {
                            toastr.success(ko.Localization('Naati.Resources.Shared.resources.DeletedSuccessfully'));
                            getTestAssetList();
                        });
                    }
                });
        };

        vm.tableDefinition.dataTable = {
            source: vm.testassets,
            select: {
                style: 'multi+shift'
            },
            columnDefs: [
                { targets: 7, orderable: false },
                {
                    targets: 6,
                    render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
                }
            ],
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
            return ko.utils.arrayMap($('#' + vm.tableDefinition.id).DataTable().rows(vm.selectedTestIndices()).data(), function (data) {
                return vm.testassets()[data[9]];
            });
        });

        vm.hasResults = function() {
            return vm.testassets().length > 0;
        }

        vm.additionalButtons = [
            {
                'class': 'btn btn-success',
                click: vm.bulkDownload,
                disable: ko.pureComputed(function () {
                    return !vm.selectedTests().length;
                }),
                icon: 'glyphicon glyphicon-download',
                resourceName: 'Naati.Resources.Shared.resources.BulkDownload',
            }
        ];

        function parseSearchTerm(searchTerm) {
            return JSON.stringify({
                // todo: multiselect
                TestAttendanceId: searchTerm.TestID ? searchTerm.TestID.Data.valueAsArray : null,
                JobId: searchTerm.JobID ? searchTerm.JobID.Data.valueAsArray : null,
                NaatiNumber: searchTerm.NAATINumber ? searchTerm.NAATINumber.Data.NAATINumber : null,
                OfficeId: searchTerm.TestOffice ? searchTerm.TestOffice.Data.Options : null,
                UploadedByUserId: [],
                UploadedByPersonNaatiNo: searchTerm.UploadedByPerson ? searchTerm.UploadedByPerson.Data.NAATINumber : null,
                SatDateFrom: searchTerm.TestDateRange ? searchTerm.TestDateRange.Data.From : null,
                SatDateTo: searchTerm.TestDateRange ? searchTerm.TestDateRange.Data.To : null,
                TestAttendanceAssetType: ['UnmarkedTestAsset', 'MarkedTestAsset', 'EnglishMarking', 'ReviewReport', 'TestMaterial', 'ProblemSheet', 'MedicalCertificate'],
            });
        }

        function getTestAssetList() {
            vm.selectedTestIndices([]);
            testassetDataService.get({ request: parseSearchTerm(vm.searchTerm()) })
                .then(function(data) {
                    vm.testassets(data);
                });
        }

        vm.searchCallback = getTestAssetList;

        vm.activate = function() {
            $.fn.dataTable.moment(CONST.settings.shortDateDisplayFormat);
        }

        return vm;
    });
