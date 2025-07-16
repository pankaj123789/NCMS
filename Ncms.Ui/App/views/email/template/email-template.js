define([
    'services/emailTemplate-data-service',
     'services/screen/message-service',
    'plugins/router',
    'modules/enums'
], function (emailTemplateService, message, router,enums ) {

    var emailTemplate = {
        Id: ko.observable(),
        Name: ko.observable().extend({ required: true, maxLength: 100 }),
        FromAddress: ko.observable().extend({ required: true, maxLength: 255, email: true }),
        Subject: ko.observable().extend({ required: true, maxLength: 255 }),
        Content: ko.observable(),
        Active: ko.observable(),
        Workflows: ko.observableArray()
    };

    var vm = {
        activate: activate,
        windowTitle: ko.observable('Email Templates'),
        emailModel: emailTemplate,
        save: save,
        exportFile: exportFile,
        tryClose: tryClose,
        wysiwygOptions: {
            value: emailTemplate.Content
        },
        tableDefinition: {
            searching: true,
            paging: true
        },
        formId: 'newPhotoForm'
    };
    var $form = null;
    var $file = null;

    var validation = ko.validatedObservable(vm.emailModel);
    vm.dirtyFlag = new ko.DirtyFlag([vm.emailModel], false);
    vm.canSave = ko.computed(function() {
        return vm.dirtyFlag().isDirty();
    });

    function exportFile() {
        var downloadLink = document.createElement('a');
        downloadLink.href = 'data:text/html;charset=utf-8,' + encodeURIComponent(emailTemplate.Content());
        downloadLink.download = emailTemplate.Name();
        document.body.appendChild(downloadLink);
        downloadLink.click();
        document.body.removeChild(downloadLink);
    }

    function save() {
        var isValid = validation.isValid();
        validation.errors.showAllMessages(!isValid);

        if (isValid) {
            var json = ko.toJS(vm.emailModel);
            emailTemplateService.post(json, 'SaveEmailTemplate')
                .then(
                    function () {
                        toastr.success('Email Template Saved.');
                        vm.dirtyFlag().reset();
                    });
        }
    }

    vm.selectFile = function() {
        $file[0].click();
    };

    function activate(id) {
        emailTemplateService.getFluid(id).then(function (data) {
            vm.windowTitle('Email Template ' + id + " - " + data.Name);

            data.Id = id;
            data.Workflows = data.WorkflowModels;
            data.Workflows.forEach(function(workflow) {
                workflow.TemplateDetails = workflow.EmailTemplateDetails.map(function(x) { return x.DisplayName; }).join(', ');
                workflow.ApplicationType = workflow.ApplicationTypes.map(function (x) { return x.DisplayName; }).join(', ');
                if (!workflow.ApplicationType) {
                    workflow.ApplicationType = '-';
                }
            });
            ko.viewmodel.updateFromModel(emailTemplate, data);
            vm.dirtyFlag().reset();

            $form = $('#' + vm.formId);
            $('[type=file]', $form).remove();
            $file = $('<input type="file" name="file" accept="text/html" />').appendTo($form);
            $file[0].addEventListener('change', getFile);
        });
    }

    function getFile(event) {
        if ('files' in event.target && event.target.files.length > 0) {
            readFileContent(event.target.files[0]).then(
                function(content) {
                    return emailTemplate.Content(content);
                });
        }
    }

    function readFileContent(file) {
        var reader = new FileReader();
        return new Promise(function(resolve, reject) {
            reader.onload = function(event) { return resolve(event.target.result); };
            reader.onerror = function(error) { return reject(error); };
            reader.readAsText(file);
        });
    }

    function tryClose() {
        if (vm.dirtyFlag().isDirty()) {
            message.confirm({
                title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                content: ko.Localization('Naati.Resources.Shared.resources.AreYouSureYouWantToCancel')
            })
                .then(
                    function (answer) {
                        if (answer === 'yes') {
                            close();
                        }
                    });
        } else {
            close();
        }
    }

    function close() {
        router.navigateBack();
    };
    return vm;
});