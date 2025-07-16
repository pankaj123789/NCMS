define([
    'services/file-data-service',
    'services/testasset-data-service',
    'views/document/document-form'
], function (fileService, testassetService, documentForm) {
    var vm = {
        getInstance: getInstance
    };

    return vm;

    function getInstance() {
        var vm = documentForm.getInstance();

        vm.fileUpload.url = fileService.url() + '/upload';
        vm.fileUpload.formData = ko.computed(function () {
            return {
                id: vm.currentDocument.Id() || 0,
                file: vm.currentDocument.File(),
                type: vm.currentDocument.Type(),
                relatedId: vm.relatedId(),
                title: vm.currentDocument.Title(),
                storedFileId: vm.currentDocument.StoredFileId() || 0,
                testAsset: vm.typeOptions().length > 1,
                eportalDownload: vm.eportalDownload()
            }
        });

        $.extend(vm, {
            getDocumentsPromise: function () {
                var request = {
                    TestAttendanceAssetType: vm.types(),
                };

                var types = vm.types();

                if (types.length === 1 && types[0] === 'TestMaterial') {
                    request.TestMaterialId = [vm.relatedId()];
                }
                else {
                    request.TestAttendanceId = [vm.relatedId()];
                }

                return testassetService.get({
                    request: JSON.stringify(request),
                    supressResponseMessages: true
                });
            },
            postDocumentPromise: function () {
                var data = ko.toJS(vm.currentDocument);
                data.RelatedId = vm.relatedId();
                data.testAsset = vm.typeOptions().length > 1;
                if (data.testAsset) {
                    data.eportalDownload = vm.eportalDownload();
                }

                return fileService.post(data);
            },
            removeDocumentPromise: function (document) {
                return fileService.remove(document.StoredFileId);
            }
        });

        return vm;
    }
});
