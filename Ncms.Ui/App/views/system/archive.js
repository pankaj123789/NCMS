define([
    'services/system-data-service',
    'views/shell'
], function (systemService, shell) {
    var vm = {
        fileUpload: {
            url: 'api/system/Archive',
            files: ko.observableArray()
        },
        title: shell.titleWithSmall,
        date: ko.observable(moment().format('D/MM/YYYY')),
        updateRecordsOnException: ko.observable(false),
        save: save,
        success: ko.observable(),
        updatedNaatiNumbers: ko.observable(),
        errorPerson: ko.observableArray(),
        errorMessage: ko.observable(),
        errorResponse: ko.observable(),
        innerException: ko.observable(),
        stackTrace: ko.observable(),
        waiting: ko.observable(false),
        displayRequest: ko.observable(true),
        displayResult: ko.observable(false),
        displayGeneralError: ko.observable(false),
        displayNaatiNumberError: ko.observable(false),
        tableComponent: {
            id: 'archiveTable',
            headerTemplate: 'archive-header-template',
            rowTemplate: 'archive-row-template'
        }
    };

    vm.fileUpload.formData = ko.computed(function() {
        return {
            date: moment(vm.date()).format('DD MMM YYYY'),
            updateRecordsOnException: vm.updateRecordsOnException()
        }
    });

    vm.additionalButtons = [
        {
            'class': 'btn btn-success',
            click: save,
            icon: ko.computed(function() {
                return (vm.fileUpload.files().length == 1 && vm.date() != null && !vm.waiting()) ? 'glyphicon glyphicon-ok' : 'glyphicon glyphicon-remove';
            }),
            resourceName: 'Naati.Resources.Shared.resources.Upload',
            enabled: ko.computed(function() {
                var vmDate = moment(vm.date());
                return (vm.fileUpload.files().length == 1 &&
                    vm.date() != null &&
                    !vm.waiting() &&
                    vmDate <= moment());
            })
        }
    ];

    vm.errorButtonClass = 'btn btn-danger';

    vm.successIcon = ko.computed(function() {
        return (vm.success()) ? 'glyphicon glyphicon-ok' : 'glyphicon glyphicon-remove';
    });

    function displayRequest() {
        vm.displayRequest(true);
        vm.displayResult(false);
        vm.displayGeneralError(false);
        vm.displayNaatiNumberError(false);
    }

    function displayResult() {
        vm.displayResult(true);
        vm.displayGeneralError(false);
        vm.displayNaatiNumberError(false);
    }

    function displayGeneralError() {
        vm.displayResult(true);
        vm.displayGeneralError(true);
        vm.displayNaatiNumberError(false);
    }

    function displayNaatiNumberError() {
        vm.displayResult(true);
        vm.displayGeneralError(false);
        vm.displayNaatiNumberError(true);
    }

    function save() {
        var files = vm.fileUpload.files();
        if (files.length) {
            for (var i = 0; i < files.length; i++) {
                vm.waiting(true);
                files[i].submit().done(function (data) {
                    vm.waiting(false);
                    if (data && data.data && data.data.Data) {
                        vm.success(data.data.Success);
                        vm.updatedNaatiNumbers(data.data.Data.UpdatedNaatiNumbers);
                        vm.errorPerson(data.data.Data.ErrorNaatiNumbers);
                        displayResult();
                        if (data.data.Data.ErrorMessage != null ||
                            data.data.Data.ErrorResponse != null) {
                            vm.errorMessage(data.data.Data.ErrorMessage);
                            vm.errorResponse(data.data.Data.ErrorResponse);
                            displayGeneralError();
                        }
                        if (data.data.Data.ErrorNaatiNumbers != null && data.data.Data.ErrorNaatiNumbers.length > 0) {
                            displayNaatiNumberError();
                        }
                    } else {
                        vm.success(false);
                    }
                }).fail(function (data) {
                    vm.waiting(false);
                    vm.success(false);
                    if (data && data.responseJSON && data.responseJSON.data) {
                        if (data.responseJSON.data.ErrorNaatiNumbers != null && data.responseJSON.data.ErrorNaatiNumbers != "Undefined" && data.responseJSON.data.ErrorNaatiNumbers.length > 0) {
                            displayNaatiNumberError();
                        } else {
                            displayGeneralError();
                        }
                        vm.errorMessage(data.responseJSON.data.ErrorDetails.ErrorMessage);
                        vm.errorResponse(data.responseJSON.data.ErrorDetails.ErrorResponse);
                        vm.updatedNaatiNumbers(data.responseJSON.data.UpdatedNaatiNumbers);
                        vm.errorPerson(data.responseJSON.data.ErrorNaatiNumbers);
                    } else if(data) {
                        vm.errorMessage(data);
                    }
                });
            }
        } else {
            toastr.error("No files selected.");
        }
    }

    $.extend(true, vm, {
        tableComponent: {
            dataTable: {
                source: vm.errorPerson
            }
        }
    });

    return vm;
});