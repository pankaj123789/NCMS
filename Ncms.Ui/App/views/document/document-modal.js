define([
    'views/document/document-form',
    'services/testresult-data-service',
    'services/file-data-service',
], function (documentForm, testresultService, fileService) {
    var defer = null;

    var vm = {
        getInstance: getInstance
    };

    return vm;

    function getInstance() {
        var vm = {
            test: ko.observable({}),
            show: show,
            close: close,
            parseFileName: null,
            documentTableInstance: documentForm.getInstance()
        };

        vm.title = ko.pureComputed(function () {
            return '{0} - {1} {2} - {3} - {4}'.format(
                ko.Localization('Naati.Resources.Test.resources.Assets'),
                ko.Localization('Naati.Resources.Test.resources.Attendance'),
                vm.test().AttendanceId,
                vm.test().Skill,
                vm.test().CredentialTypeInternalName)
        });

        setupDocument();

        return vm;

        function show(test) {
            vm.test(test);

            defer = Q.defer();
            $('#documentModal').modal('show');

            vm.documentTableInstance.search();
            
            return defer.promise;
        }

        function close() {
            $('#documentModal').modal('hide');
        }

        function setupDocument(documentInstance) {
            var documentInstance = vm.documentTableInstance;

            documentInstance.parseFileName = function () { return vm.parseFileName.apply(vm, arguments); };

            documentInstance.fileUpload.url = testresultService.url() + '/document/upload';
            documentInstance.fileUpload.formData = ko.computed(function () {
                return {
                    id: documentInstance.currentDocument.Id() || 0,
                    file: documentInstance.currentDocument.File(),
                    type: documentInstance.currentDocument.Type(),
                    testSittingId: vm.test().AttendanceId,
                    title: documentInstance.currentDocument.Title(),
                    storedFileId: documentInstance.currentDocument.StoredFileId() || 0,
                    eportalDownload: documentInstance.eportalDownload()
                }
            });

            $.extend(documentInstance, {
                getDocumentsPromise: function () {
                    return testresultService.getFluid('documents/' + vm.test().AttendanceId);
                },
                postDocumentPromise: function () {
                    var data = ko.toJS(documentInstance.currentDocument);
                    data.TestSitingId = vm.test().AttendanceId;
                    data.eportalDownload = documentInstance.eportalDownload();
                    return testresultService.post(data);
                },
                transformDocuments: function (data) {
                    return ko.utils.arrayMap(data, function (d) {
                        var tmp = d.FileName.split('.');
                        d.FileType = tmp[tmp.length - 1];
                        d.UploadedByName = d.UploadedByPersonName || d.UploadedByUserName;
                        d.DocumentType = d.DocumentTypeDisplayName;
                        return d;
                    });
                },
                removeDocumentPromise: function (document) {
                    return fileService.remove(document.StoredFileId);
                }
            });
        }
    }
});