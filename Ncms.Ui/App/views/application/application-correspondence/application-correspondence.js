define([
    'plugins/router',
    'modules/common',
    'modules/enums',
    'services/application-data-service',
    'views/email/email',
    'services/screen/message-service'
],
function(router, common, enums, applicationService, emailTemplate, messageService) {
    return {
        getInstance: getInstance
    };

    function getInstance() {
        var vm = {
            emails: ko.observableArray(),
            applicationId: ko.observable(),
            viewEmail: viewEmail,
            tableDefinition: {
                id: 'applicationCorrespondenceTable',
                headerTemplate: 'application-correspondence-header-template',
                rowTemplate: 'application-correspondence-row-template',
            },
            sendEmail: sendEmail,
            emailTemplateInstance: emailTemplate.getInstance(),
            entityEmail: ko.observable()
        };
        if (vm.tableDefinition.dataTable == undefined) {
            vm.tableDefinition.dataTable = {
                source: vm.emails,
                columnDefs: [
                    {
                        targets: 0,
                        render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.longDateTimeDisplayFormat)
                    },
                    {
                        targets: 3,
                        render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.longDateTimeDisplayFormat)
                    },
                    {
                        targets: -1,
                        orderable: false
                    },
                ],
                order: [
                    [0, "desc"]
                ]
            };
        }

         vm.load = function(applicationId, naatiNumber) {
            vm.applicationId(applicationId);
            if (naatiNumber) {
                applicationService.getFluid('applicant/' + naatiNumber).then(function(data) {
                    vm.entityEmail(data.PrimaryEmail);
                });
            }

             applicationService.getFluid("listEmail/" + vm.applicationId()).then(function (data) {
                 vm.emails(data);
            });
        };

        function viewEmail(data) {
            showEmailTemplate(data);
        }

        function sendEmail(data) {
            messageService.confirm({ title: ko.Localization('Naati.Resources.EmailMessage.resources.ResendEmail'), content: ko.Localization('Naati.Resources.EmailMessage.resources.ResendConfirmation') }).then(
                   function(answer) {
                       if (answer === 'yes') {
                           applicationService.post({ emailMessageId: data.EmailMessageId }, "sendEmail").then(function() {
                               if (vm.entityEmail() !== data.RecipientEmail) {
                                   toastr.success(ko.Localization('Naati.Resources.EmailMessage.resources.EmailSentWithUpdatedRecipient'));
                               }
                               else {
                                   toastr.success(ko.Localization('Naati.Resources.EmailMessage.resources.EmailSent'));
                               }
                               vm.load(vm.applicationId());
                           });
                       }
                   });
        }

        function showEmailTemplate(data) {

            var template = 'text!views/email/email.html';
            loadIfNever(template).then(function(isFirstLoad) {
                if (isFirstLoad) {
                    var $div = $('<div></div>');
                    $('body').append($div);
                    ko.applyBindingsToNode($div[0],
                    {
                        template: {
                            name: 'emailTemplate',
                            data: vm.emailTemplateInstance
                        }
                    });
                }

                vm.emailTemplateInstance.show(data.EmailMessageId);
            });
        }

        var templateLoaded = [];
        function loadIfNever(template) {
            var defer = Q.defer();

            var isLoaded = ko.utils.arrayFirst(templateLoaded, function(t) {
                return t === template;
            });

            if (!isLoaded) {
                require([template], function(t) {
                    var $t = $(t);
                    $('body').append($t);
                    templateLoaded.push(template);
                    defer.resolve(true);
                });
            }
            else {
                defer.resolve();
            }

            return defer.promise;
        }

        return vm;
    }
});
