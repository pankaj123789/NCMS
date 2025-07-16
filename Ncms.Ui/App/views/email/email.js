define([
    'plugins/router',
    'modules/common',
    'modules/enums',
    'services/application-data-service',
    'services/file-data-service',
],
    function (router, common, enums, applicationService, fileService) {
        return {
            getInstance: getInstance
        };

        function getInstance() {
            var email = {
                From: ko.observable(),
                RecipientEmail: ko.observable(),
                Subject: ko.observable(),
                Body: ko.observable(),
                Attachments: ko.observableArray(),
                CreatedDate: ko.observable()
            };
            email.Body.subscribe(setEmailContent);


            var vm = {
                emailModel: email,
                applicationId: ko.observable(),
                frameHeight: ko.observable(),
                tableDefinition: {
                    id: 'correspondenceTable',
                    headerTemplate: 'correspondence-header-template',
                    rowTemplate: 'correspondence-row-template',
                },
                attachmentsTableDefinition: {
                    searching: false,
                    paging: false,
                    oLanguage: { sInfoEmpty: '', sInfo: '' }
                },
                emailContentId: 'emailContent',
                modalId: 'emailModal'
            };

            var cleanModel = ko.toJS(email);

            vm.show = function (emailMessageId) {
                $('#' + vm.modalId).modal('show');
                load(emailMessageId);
            };

            vm.tableDefinition.dataTable = {
                source: vm.emails,
                columnDefs: [
                    {
                        targets: 0,
                        render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
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

            function load(emailMessageId) {

                var defer = Q.defer();
                ko.viewmodel.updateFromModel(vm.emailModel, cleanModel);

                applicationService.getFluid("email/" + emailMessageId).then(function (data) {
                    ko.utils.arrayForEach(data.Attachments, function (a) {
                        a.DownloadUrl = fileService.url() + '/download/' + a.StoredFileId;
                    });

                    ko.viewmodel.updateFromModel(vm.emailModel, data);
                    vm.emailModel.CreatedDate(moment(data.CreatedDate).format("DD/MM/YYYY h:mm a"));
                    defer.resolve(data);

                });

                return defer.promise;
            };

            function setEmailContent(html) {

                setTimeout(function () {
                    var $iframe = $('#{0}'.format(vm.emailContentId));
                    var iframe = $iframe[0];

                    iframe.contentWindow.document.open();
                    iframe.contentWindow.document.write(html);
                    iframe.contentWindow.document.close();

                    $iframe.load(function () {
                        var height = $iframe.contents().find("html").height();
                        vm.frameHeight('{0}px'.format(height));
                    });
                }, 100);

            }

            return vm;
        }
    });
