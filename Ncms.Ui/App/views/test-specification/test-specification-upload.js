define([
    'services/util',
    'services/testspecification-data-service',
    'services/screen/message-service'
], function (util, testSpecificationService, message) {
    var vm = {
        getInstance: getInstance
    };
    return vm;

    function getInstance(testSpecificationId) {
        var defer = null;
        var eventDefer = Q.defer();
        var vm = {
            Title: ko.observable().extend({ required: true, maxLength: 255 }),
            modalId: util.guid(),
            readyForUpload: ko.observable(false),
            hasFileToUpload: ko.observable(false),
            hasMessages: ko.observable(),
            fileUpload: {
                events: {
                    fileuploaddone: function () {

                    }
                },
                files: ko.observableArray(),
                url: testSpecificationService.url() + '/testSpecifications',
                formData: ko.observable({
                    id: testSpecificationId,
                    verifyOnly: true,
                    file: 'TestSpecifications.xlsx',
                    type: 'xlsx',
                    title: 'TestSpecifications'
                })
            },
            tableDefinition: {
                headerTemplate: 'validation-header-template',
                rowTemplate: 'validation-row-template'
            },
            validationErrors: ko.observableArray([]),
            validationMessages: ko.observableArray([]),
            uploadCompletedSuccessfully: ko.observable(false),
            event: eventDefer.promise,
        };

        vm.show = function () {
            defer = Q.defer();

            $('#' + vm.modalId).modal('show');

            return defer.promise;
        };

        vm.close = function () {
            $('#' + vm.modalId).modal('hide');
        };

        vm.verify = function () {

            this.fileUpload.formData({
                id: testSpecificationId,
                verifyOnly: true,
                file: 'TestSpecifications.xlsx',
                type: 'xlsx',
                title: 'TestSpecifications'
            });

            vm.validationErrors.removeAll();

            vm.uploadFiles();
        }

        vm.upload = function () {

            this.fileUpload.formData({
                id: testSpecificationId,
                verifyOnly: false,
                file: 'TestSpecifications.xlsx',
                type: 'xlsx',
                title: 'TestSpecifications'
            });

            message.confirm({
                title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                content: ko.Localization('Naati.Resources.SystemResources.resources.UploadConfirmMessage')
            })
                .then(
                    function (answer) {
                        if (answer === 'yes') {
                            vm.uploadFiles();
                            vm.uploadCompletedSuccessfully(true);
                            eventDefer.notify({
                                name: 'UploadCompletedSuccessfully',
                                data: vm.uploadCompletedSuccessfully()
                            });
                        }
                    });
        }

        vm.uploadFiles = function () {
            var files = vm.fileUpload.files();
            if (files.length) {
                files[0].submit()
                    .done(function (genericResponse) {
                       
                        genericResponse.messages.forEach(function (data) {
                            var type = 'info';
                            vm.validationErrors.push({ type, data });
                            vm.hasMessages(true);
                        });

                        vm.readyForUpload(genericResponse.errors.length == 0);
 
                        if (vm.readyForUpload() == true) {
                            toastr.success(genericResponse.data);
                        }

                        if (vm.readyForUpload() == false) {
                            toastr.error(genericResponse.data);
                        }

                        genericResponse.errors.forEach(function (data) {
                            var type = 'error';
                            vm.validationErrors.push({ type, data });
                            vm.hasMessages(true);
                        });

                    })
                    .error(function (jqXhr, textStatus, errorThrown) {
                        servercallbackprocessor.showError(jqXhr, textStatus, errorThrown);
                        vm.uploadCompletedSuccessfully(false);
                    });
            }
            else {
                saveRelated();
            }
        }

        vm.tableDefinition.dataTable = {
            source: vm.validationErrors
        };

        vm.fileUpload.files.subscribe(function (newValue) {
            vm.readyForUpload(false);
            vm.hasFileToUpload(true);
        });

        return vm;
    }

});