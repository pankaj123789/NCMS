define([
    'services/file-data-service',
    'services/examiner-data-service',
    'services/screen/message-service',
    'services/screen/date-service',
    'services/testresult-data-service'
], function (fileService, examinerService, message, dateService, testResultService) {
    var vm = {
        getInstance: getInstance
    };

    return vm;

    function getInstance() {
        var eventDefer = Q.defer();

        var vm = {
            event: eventDefer.promise,
            datatable: {
                source: ko.observableArray([]),
                columnDefs: [
                    { targets: 6, render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateTimeDisplayFormat) }
                ],
                order: [[0, "asc"], [2, "asc"], [1, "asc"]]
            }
        };

        vm.load = function (testAttendanceId) {
            testResultService.getFluid('examinerDocuments/' + testAttendanceId).then(function (data) {
                ko.utils.arrayForEach(data, function (d) {
                    d.ExaminerMarksRemoved = d.ExaminerMarksRemoved || false;
                    d.AvailableForExaminersDisplayValue = d.EportalDownload ? 'Yes' : 'No';
                    d.MergeDocumentDisplayValue = d.MergeDocument ? 'Yes' : 'No';
                });
                
                data = transformDocuments(data);

                vm.datatable.source(data);
            });
        };

        vm.download = function (document) {
            return fileService.getFluid('downloadWithToken/' + document.StoredFileId);
        };

        return vm;

        function transformDocuments(data) {
            return ko.utils.arrayMap(data,
                function(d) {
                    var tmp = d.FileName.split('.');
                    d.FileType = tmp[tmp.length - 1];
                    d.UploadedByName = d.UploadedByPersonName || d.UploadedByUserName;
                    d.DocumentType = d.DocumentTypeDisplayName;
                    return d;
                });
        }
    }
});
