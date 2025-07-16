define([
    'views/shell',
    'services/system-data-service'
], function (shell, systemService) {

    var serverModel = {
        Id: ko.observable(),
        Title: ko.observable().extend({ required: true, maxLength: 100 }),
        File: ko.observable().extend({ required: true }),
        StoredFileId: ko.observable(),
        UnmodifiedTitle: ko.observable(),
        Type: ko.observable()
    };

    var message = function () {
        this.data = ko.observable();
    };

    var vm = {
        title: shell.titleWithSmall,
        currentDocument: serverModel,
        inEditMode: ko.observable(false),
        fileUpload: {
            events: {
                fileuploaddone: function () {

                }
            },
            files: ko.observableArray(),
            url: systemService.url() + '/testSpecifications',
            formData: {
                id: 0,
                file: 'TestSpecifications.xlsx',
                type: 'xlsx',
                applicationId: 0,
                title: 'TestSpecifications',
                storedFileId: 0
            }
        },
        tableDefinition: {
            headerTemplate: 'document-header-template',
            rowTemplate: 'document-row-template'
        },
        validationErrors: ko.observableArray([]),
        validationMessages: ko.observableArray([]),
    };

    vm.download = function () {
        return systemService.getFluid('testSpecifications');
    }

    vm.upload = function () {
        var files = vm.fileUpload.files();
        if (files.length) {
            files[0].submit()
                .done(function (genericResponse) {
                    toastr.success(genericResponse.data);

                    genericResponse.messages.forEach(function (data) {
                        vm.validationMessages.push(new message().data(data));
                    });

                    genericResponse.errors.forEach(function (data) {
                        vm.validationErrors.push(new message().data(data));
                    });

                })
                .error(function (jqXhr, textStatus, errorThrown) {
                    servercallbackprocessor.showError(jqXhr, textStatus, errorThrown);
                });
        }
        else {
            saveRelated();
        }
    }

    vm.fileUpload.files.subscribe(function (newValue) {
        vm.inEditMode(newValue && newValue.length > 0);
        if (newValue && newValue.length > 0 && newValue[0].files && newValue[0].files.length > 0) {
            vm.currentDocument.File(newValue[0].files[0].name);
        }
    });

    vm.tableDefinition.dataTable = {
        source: vm.validationErrors
    };

    return vm;
    });