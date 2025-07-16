define([
    'modules/common',
    'services/util',
    'services/application-data-service',
    'services/person-data-service',
    'services/screen/message-service',
],
    function(common, util, applicationService, personService, messageService) {
        return {
            getInstance: getInstance
        };

        function getInstance(person) {
            var vm = {
                naatiNumber: ko.observable(),
                isLoading: ko.observable(false),
                person: person,
                emails: ko.observableArray(),
                selectedEmail: ko.observable(),
                emailContent: ko.observable(),
                resizableOptions: {
                    handles: "w, e",
                    minWidth: 550,
                    maxWidth: 1000
                },
                tableDefinition: {
                    id: util.guid(),
                    info: false,
                    oLanguage: { sInfoEmpty: '', sInfo: '' },
                    select: {
                        style: 'single'
                    },
                    order: [],
                    events: {
                        select: selectTable,
                        deselect: selectTable
                    },
                    initComplete: enableDisableDataTableButtons,
                    buttons: {
                        dom: {
                            button: {
                                tag: 'label',
                                className: ''
                            },
                            buttonLiner: {
                                tag: null
                            }
                        },
                        buttons: [{
                            text: '<span class="fa fa-envelope m-r-xs"></span><span>' +
                                ko.Localization('Naati.Resources.Person.resources.ViewEmail') + '</span>',
                            className: 'btn btn-success',
                            action: function(e, dt, node, config) {
                                if (vm.isLoading()) {
                                    messageService.alert({
                                        title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
                                        content: ko.Localization('Naati.Resources.EmailMessage.resources.WarningAttachmentDownloading')
                                    });
                                    return;
                                }

                                var removeElement = ["iframe", "p.form-control-static.m-t-n-sm:last"];
                                var html =
                                        '<html><head>' +
                                        '<link rel="stylesheet" href="/Content/bootstrap.css" />' +
                                        ' <link rel="stylesheet" href="/Content/site.css" />' +
                                        ' <link rel="stylesheet" href="/Content/additional.css" />' +
                                        ' <style> #divtagdefaultwrapper{margin:-10px 8px;font-size:11pt !important} body{padding: 15px !important; } p {display: block;-webkit-margin-before: 1em;-webkit-margin-after: 1em;-webkit-margin-start: 0px;-webkit-margin-end: 0px;} .m-t-n-sm {margin-top: -10px;} .form-control-static.m-t-n-sm,label{  font-family: "Source Sans Pro", "Helvetica Neue", Helvetica, Arial, sans-serif;font-size: 12px;-webkit-font-smoothing: antialiased;line-height: 1.42857143;color: #58666e;}</style></head><body>' +
                                        $('.email-preview').html() +
                                        $('iframe[name="emailContent"]').contents().find("html").html() +
                                        "</body></html>";

                                util.windowOpen({
                                    target: '_blank',
                                    replace: null
                                }, html, removeElement);

                            }
                        }]
                    }
                }
            };

            vm.selectedEmail.subscribe(enableDisableDataTableButtons);

            vm.humanizeDate = function(date) {
                return common.functions().humanizeDate(date);
            };

            vm.emailsPreviewOptions = {
                loaded: ko.observable(true),
                emails: ko.pureComputed(function() {
                    if (vm.selectedEmail()) {
                        return [vm.selectedEmail()];
                    }

                    return [];
                })
            };

            vm.load = function(naatiNumber) {
                if (vm.naatiNumber() != naatiNumber) {
                    vm.naatiNumber(naatiNumber);
                    loadEmails();
                }
            }

            function loadEmails() {
                vm.isLoading(true);
                vm.selectedEmail(null);
                if (vm.person.NaatiNumber()) {
                    personService.getFluid(vm.person.NaatiNumber() + '/emails').then(function(data) {
                        ko.utils.arrayForEach(data, function(email) {
                            email.Attachments = [];
                            email.Bcc = null;
                            email.EmailMessageId = null;
                            email.RecipientEmail = null;
                            email.Body = null;
                            email.LastSendResult = null;
                            email.Cc = null;
                            email.CreatedDate = null;
                            email.CreatedUserId = null;
                            email.EmailContent = null;
                            email.RecipientEntityId = null;
                            email.To = null;
                        });
                        vm.emails(ko.viewmodel.fromModel(data)());
                        vm.isLoading(false);
                    });
                }
            }

            function enableDisableDataTableButtons() {
                var $table = $('#' + vm.tableDefinition.id);

                if (!$.fn.DataTable.isDataTable($table)) {
                    setTimeout(enableDisableDataTableButtons, 100);
                    return;
                }

                var buttons = $table.DataTable().buttons('*');
                if (!buttons.length) {
                    setTimeout(enableDisableDataTableButtons, 100);
                    return;
                }

                $table.width('100%');

                var disable = !vm.selectedEmail();
                if (disable) {
                    buttons.disable();
                }
                else {
                    buttons.enable();
                }
            };

            function selectTable(e, dt) {
                vm.selectedEmail(null);
                var indexes = dt.rows('.selected').indexes();

                if (!indexes.length) {
                    return;
                }

                var index = indexes[0];
                loadEmail(vm.emails()[index]);
            }

            function loadEmail(emailObservable) {
                var email = ko.toJS(emailObservable);

                if (!email) {
                    return;
                }
           
                var emailContent = email.EmailContent;
                if (emailContent) {
                    vm.selectedEmail(emailObservable);
                }

                if (!emailObservable.isLoaded && !vm.isLoading() && emailObservable.Attachments().length <= 0) {
                    vm.isLoading(true);
                    var promises = [];
                    promises.push(personService.post({ GraphEmailId: email.GraphEmailMessageId, NaatiNumber: person.NaatiNumber(), IsInlineAttachment: false, MailBox: email.MailBox }, 'graphEmailAttachments'));
                    promises.push(personService.post({ GraphEmailId: email.GraphEmailMessageId, MailBox: email.MailBox }, 'graphEmailDetails'));
                    Promise.all(promises).then(function(results) {
                        var emailDetails = results[1];
                        ko.viewmodel.updateFromModel(emailObservable, emailDetails);
                    
                        var data = results[0];
                        var attachments = [];
                        ko.utils.arrayForEach(data,
                            function(d) {
                                if (d.IsInline == false) {
                                    var blob = base64ToBlob(d.GraphAttachmentBytes, d.FileType);
                                    d.DownloadFileName = d.FileName;
                                    d.DownloadUrl = window.URL.createObjectURL(blob);
                                    attachments.push(d);
                                } else {
                                    var replaceSrc = "cid:" + d.ContentId;
                                    var replaceString = "data:" + d.FileType + ";base64," + d.GraphAttachmentBytes;
                                    emailObservable.EmailContent(emailObservable.EmailContent()
                                        .replace(new RegExp(replaceSrc, 'g'), replaceString));

                                }
                            });

                        emailObservable.HasAttachments(attachments.length > 0);

                        vm.selectedEmail(emailObservable);
                        emailObservable.isLoaded = true;
                        emailObservable.Attachments(attachments);
                        vm.isLoading(false);
                    });
                }
            }

            function base64ToBlob(base64, mimetype, slicesize) {
                if (!window.atob || !window.Uint8Array) {
                    // The current browser doesn't have the atob function. Cannot continue
                    return null;
                }

                mimetype = mimetype || '';
                slicesize = slicesize || 512;
                var bytechars = atob(base64);
                var bytearrays = [];
                for (var offset = 0; offset < bytechars.length; offset += slicesize) {
                    var slice = bytechars.slice(offset, offset + slicesize);
                    var bytenums = new Array(slice.length);
                    for (var i = 0; i < slice.length; i++) {
                        bytenums[i] = slice.charCodeAt(i);
                    }
                    var bytearray = new Uint8Array(bytenums);
                    bytearrays[bytearrays.length] = bytearray;
                }
                return new Blob(bytearrays, { type: mimetype });
            };
            return vm;
        }
    });
