define([
    'views/document/document-form',
    'services/noteattachment-data-service',
], function (documentTable, noteattachmentService) {
    var defer = null;

    var vm = {
        getInstance: getInstance
    };

    return vm;

    function getInstance() {
        var vm = {
            show: show,
            close: close,
            parseFileName: function () { },
            documentTableInstance: documentTable.getInstance()
        };

        var documentTypes = ['General', 'Identification'];

        vm.documentTableInstance.parseFileName = function (fileName) {
            var tmp = fileName.FileName.split('.');
            return tmp.slice(0, tmp.length - 1).join('.');
        };

        vm.documentTableInstance.types(documentTypes);
        vm.documentTableInstance.fileUpload.url = noteattachmentService.url() + '/upload';
        vm.documentTableInstance.fileUpload.formData = ko.computed(function() {
          return {
                id: vm.documentTableInstance.currentDocument.Id() || 0,
                file: vm.documentTableInstance.currentDocument.File(),
                type: vm.documentTableInstance.currentDocument.Type(),
                noteId: vm.documentTableInstance.relatedId(),
                title: vm.documentTableInstance.currentDocument.Title(),
                storedFileId: vm.documentTableInstance.currentDocument.StoredFileId() || 0
            };
        });

        $.extend(vm.documentTableInstance, {
            getDocumentsPromise: function () {
                return noteattachmentService.getFluid(vm.documentTableInstance.relatedId(), {
                    supressResponseMessages: true
                });
            },
            postDocumentPromise: function () {
                var data = ko.toJS(vm.documentTableInstance.currentDocument);
                data.RelatedId = vm.documentTableInstance.relatedId();
                data.testAsset = vm.documentTableInstance.typeOptions().length > 1;

                return noteattachmentService.post(data);
            },
            removeDocumentPromise: function (document) {
                return noteattachmentService.remove(document.StoredFileId);
            }
        });

        return vm;

        function show(note) {
            defer = Q.defer();
            $('#noteDocument').modal('show');

            vm.documentTableInstance.relatedId(note.NoteId);
            vm.documentTableInstance.search();

            var isOwner = note.UserId === currentUser.Id;
            vm.documentTableInstance.allowEdit(isOwner);
            vm.documentTableInstance.allowCreate(isOwner);
            vm.documentTableInstance.allowRemove(isOwner);

            return defer.promise;
        }

        function close() {
            $('#noteDocument').modal('hide');
        }
    }
});