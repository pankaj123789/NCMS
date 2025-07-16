define([
    'views/document/document-form',
    'services/logbook-data-service',
    'services/screen/date-service',
], function (documentTable, logbookService, dateService) {
    return {
        getInstance: getInstance
    };

    function getInstance() {
        
        var serverModel = {
            Id: ko.observable(),
            workPracticeUnit: ko.observable("Hours/Assignments"),
            date: ko.observable().extend({ required: true }),
            points: ko.observable().extend({ required: true, max: 99999999.9 }),
            description: ko.observable().extend({ required: true, maxLength: 200 }),
            credentialId: ko.observable(),
            naatiNumber: ko.observable(),
            certificationPeriodId: ko.observable()
        };

        var vm = {
            modalId: 'logbookCredentialEditModal',
            naatiNumber: ko.observable(),
            windowTitle: ko.observable(),
            buttonTitle: ko.observable(),
            documentTableInstance: documentTable.getInstance(),
            workPractice: serverModel,
            submit: submit,
            defer: null
        };

        var validation = ko.validatedObservable(serverModel);
        var cleanModel = ko.toJS(serverModel);

        var documentTypes = ['WorkPractice'];

        vm.documentTableInstance.parseFileName = function (fileName) {
            var tmp = fileName.FileName.split('.');
            return tmp.slice(0, tmp.length - 1).join('.');
        };

        vm.documentTableInstance.types(documentTypes);
        vm.documentTableInstance.fileUpload.url = logbookService.url() + '/upload';
        vm.documentTableInstance.fileUpload.formData = ko.computed(function () {
            return {
                id: vm.documentTableInstance.currentDocument.Id() || 0,
                file: vm.documentTableInstance.currentDocument.File(),
                type: vm.documentTableInstance.currentDocument.Type(),
                workPracticeId: vm.documentTableInstance.relatedId(),
                title: vm.documentTableInstance.currentDocument.Title(),
                storedFileId: vm.documentTableInstance.currentDocument.StoredFileId() || 0
            };
        });

        $.extend(vm.documentTableInstance, {
            getDocumentsPromise: function() {
                return logbookService.getFluid("workPracticeAttachment", {workPracticeId:vm.documentTableInstance.relatedId()}, {
                    supressResponseMessages: true
                });
            },
            postDocumentPromise: function () {
                
                var data = ko.toJS(vm.documentTableInstance.currentDocument);
                data.WorkPracticeId = vm.documentTableInstance.relatedId();
                data.Description = data.Title;
                return logbookService.post(data, "updateWorkPracticeAttachment");
            },
            removeDocumentPromise: function (document) {
                return logbookService.remove("deleteWorkPracticeAttachment/" + document.WorkPracticeAttachmentId);
            }
        });

        vm.show = function (item, credentialId, naatiNumber, certificationPeriodId, workPracticeUnits) {
            vm.defer = Q.defer();
            ko.viewmodel.updateFromModel(vm.workPractice, cleanModel);
            validation.errors.showAllMessages(false);
            serverModel.workPracticeUnit(workPracticeUnits);
            
            vm.documentTableInstance.documents([]);

            $('#logbookCredentialEditModal').modal('show');
            var title = item ? 'Edit Work Practice' : 'Add Work Practice';
            vm.windowTitle(title);
            vm.buttonTitle(item ? "Save" : "Add");
            
            vm.workPractice.credentialId(credentialId);
            vm.workPractice.naatiNumber(naatiNumber);
            vm.workPractice.certificationPeriodId(certificationPeriodId);
            if (item) {
                vm.workPractice.date(moment.utc(item.Date()).toDate());
                vm.workPractice.points(item.Points());
                vm.workPractice.description(item.Description());
                vm.workPractice.Id(item.Id());
                vm.documentTableInstance.relatedId(item.Id());
                vm.documentTableInstance.search();
            }

            return vm.defer.promise;
        };

        function submit() {
            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return;
            }

            var json = ko.toJS(vm.workPractice);
            json.date = dateService.toPostDate(json.date);
            logbookService.post(json, "updateWorkPractice").then(function(data) {
                close();
                vm.defer.resolve(true);
            });
        }

        function close() {
            $('#logbookCredentialEditModal').modal('hide');
            vm.defer.resolve(false);
        }
        return vm;
    }
});