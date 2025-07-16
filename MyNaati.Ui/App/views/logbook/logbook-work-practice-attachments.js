define(['services/requester'],
    function (requester) {
        return {
            getInstance: getInstance
        };

        function getInstance(options) {
            var logbook = new requester('logbook');

            var vm = {
                attachments: ko.observableArray(),
                workPracticeId: options.workPracticeId,
                tableDefinition: {
                    searching: false,
                    paging: false,
                    oLanguage: { sInfoEmpty: '', sInfo: '' }
                },
                fileEvent: {
                    filebatchuploadcomplete: filebatchuploadcomplete
                },
                editable: options.editable
            };

            vm.fileOptions = {
                uploadUrl: window.baseUrl + 'logbook/createorupdateworkpracticeattachment',
                deleteUrl: window.baseUrl + 'logbook/deleteworkpracticeattachment',
                maxFilePreviewSize: 10240,
                maxFileSize: 10240,
                msgSizeTooLarge: 'The selected file exceeded the size limit of 10MB. Please try again with a smaller file.',
                fileActionSettings: {
                    showUpload: false,
                    showZoom: false
                },
                uploadExtraData: function () {
                    return {
                        WorkPracticeId: vm.workPracticeId(),
                        WorkPracticeAttachmentId: 0,
                    }
                },
                browseOnZoneClick: true,
                showRemove: false,
                showCancel: false,
                showClose: false,
                showUpload: false,
                showBrowse: false,
                showCaption: false,
                dropZoneTitle: 'Click or drag a file',
                dropZoneClickTitle: '',
                allowedFileExtensions: ['jpeg', 'jpg', 'png', 'pdf', 'doc', 'docx', 'xls', 'xlsx'],
                layoutTemplates: {
                    main2: '{preview}\n{remove}\n{cancel}\n{upload}\n{browse}\n'
                },
                previewTemplates: {
                    other: '<div class="file-preview-frame {frameClass}" id="{previewId}" data-fileindex="{fileindex}" data-template="{template}">\n' +
                        '   <div class="kv-file-content">' +
                        '           <div class="text-muted"><div class="restricted-extention">Only "jpeg, jpg, png, pdf, doc, docx, xls, xlsx" files are supported</div></div>\n' +
                        '   </div>\n' +
                        '</div>\n'

                },
             
            };

            vm.load = function () {
                $(vm.fileOptions.element).fileinput('clear');

                if (!vm.workPracticeId()) {
                    return vm.attachments([]);
                }

                logbook.getFluid('getworkpracticeattachments', { WorkPracticeId: vm.workPracticeId() }).then(function (data) {
                    vm.attachments(data);
                });
            };

            vm.attachmentDownloadUrl = function (attachment) {
                return window.baseUrl + 'logbook/downloadworkpracticeattachment/' + attachment.StoredFile.Id;
            };

            vm.removeAttachment = function (attachment) {
                mbox.remove({ title: 'Remove attachment', content: 'Are you sure that you want to remove this attachment?' }).then(function (answer) {
                    if (answer === 'yes') {
                        logbook.remove(attachment.Id, 'deleteworkpracticeattachment').then(function () {
                            vm.load();
                            toastr.success('Attachment removed.');
                        });
                    }
                });
            };

            var defer = null;
            vm.upload = function () {
                defer = Q.defer();
                $(vm.fileOptions.element).fileinput('upload');
                return defer.promise;
            };

            function filebatchuploadcomplete() {
                defer.resolve();
            }

            return vm;
        }
    });