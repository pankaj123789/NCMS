define([
    'services/material-request-data-service',
    'views/email/email',
    'services/screen/message-service'
],
    function (materialRequestService, emailTemplate, messageService) {
        return {
            getInstance: getInstance
        };

        function getInstance() {
            var vm = {
                emails: ko.observableArray(),
                materialRequestId: ko.observable(),
                viewEmail: viewEmail,
                tableDefinition: {
                    headerTemplate: 'material-correspondence-header-template',
                    rowTemplate: 'material-correspondence-row-template',
                },
                sendEmail: sendEmail,
                emailTemplateInstance: emailTemplate.getInstance()
            };

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

            vm.load = function (materialRequestId) {
                vm.materialRequestId(materialRequestId);
                materialRequestService.getFluid("listEmail/" + vm.materialRequestId()).then(function (data) {
                    vm.emails(data);
                });
            };

            function viewEmail(data) {
                showEmailTemplate(data);
            }

            function sendEmail(data) {
                messageService.confirm({ title: ko.Localization('Naati.Resources.EmailMessage.resources.ResendEmail'), content: ko.Localization('Naati.Resources.EmailMessage.resources.ResendConfirmation') }).then(
                    function (answer) {
                        if (answer === 'yes') {
                            materialRequestService.post({ emailMessageId: data.EmailMessageId }, "sendEmail").then(function () {
                                toastr.success(ko.Localization('Naati.Resources.EmailMessage.resources.EmailSent'));
                                vm.load(vm.materialRequestId());
                            });
                        }
                    });
            }

            function showEmailTemplate(data) {

                var template = 'text!views/email/email.html';
                loadIfNever(template).then(function (isFirstLoad) {
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

                var isLoaded = ko.utils.arrayFirst(templateLoaded, function (t) {
                    return t === template;
                });

                if (!isLoaded) {
                    require([template], function (t) {
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
