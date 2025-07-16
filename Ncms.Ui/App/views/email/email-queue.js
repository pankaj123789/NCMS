define([
    'views/shell',
    'modules/enums',
    'services/screen/date-service',
    'services/finance-data-service',
    'services/screen/message-service',
    'services/email-data-service',
    'views/email/email'
], function (shell, enums, dateService, financeService, messageService, emailService, emailTemplate) {
    var tableId = 'emailQueueTable';

    var vm = {
        searchComponentOptions: {
            title: shell.titleWithSmall,
            filters: [
                {id: 'DateRequested'},
                {id: 'EmailStatus'},
                {id: 'Recipient'},
                {id: 'RecipientEmail'},
                {id: 'NAATINumber'},
                {id: 'EmailMessageId'},
                { id: 'RecipientNaatiUser'}
            ],
            searchType: enums.SearchTypes.EmailQueue,
            tableDefinition: {
                id: tableId,
                headerTemplate: 'email-queue-header-template',
                rowTemplate: 'email-queue-row-template'
            }
        },
        emailTemplateInstance: emailTemplate.getInstance(),
        viewEmail: viewEmail,
        resendEmail: sendEmail,
        entityEmail: ko.observable(),
        searchType: enums.SearchTypes.EmailQueue,
        searchTerm: ko.observable({}),
        queue: ko.observableArray([]),
        getQueue: function () {
            emailService.get({ request: { Skip: null, Take: 500, Filter: parseSearchTerm(vm.searchTerm()) } }).then(function (data) {
                vm.queue(data);
            });
        },
        largeTooltip: '<div class="tooltip tooltip-xl" role="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>'
    };

    function parseSearchTerm(searchTerm) {
        var json = {
            NaatiNumberIntList: searchTerm.NAATINumber ? searchTerm.NAATINumber.Data.NAATINumber : null,
            EmailMessageIdIntList: searchTerm.EmailMessageId ? searchTerm.EmailMessageId.Data.valueAsArray : null,
            DateRequestedFromString: searchTerm.DateRequested ? searchTerm.DateRequested.Data.From : null,
            DateRequestedToString: searchTerm.DateRequested ? searchTerm.DateRequested.Data.To : null,
            EmailStatusIntList: searchTerm.EmailStatus ? searchTerm.EmailStatus.Data.selectedOptions : null,
            RecipientUserIntList: searchTerm.RecipientNaatiUser ? searchTerm.RecipientNaatiUser.Data.selectedOptions : null,
            RecipientString: searchTerm.Recipient ? searchTerm.Recipient.Data.value : null,
            RecipientEmailString: searchTerm.RecipientEmail ? searchTerm.RecipientEmail.Data.value : null
            };
        
        return JSON.stringify(json);
    }

    function viewEmail(data) {
        showEmailTemplate(data);
    }

    function sendEmail(data) {
        messageService.confirm({ title: ko.Localization('Naati.Resources.EmailMessage.resources.ResendEmail'), content: ko.Localization('Naati.Resources.EmailMessage.resources.ResendConfirmation') }).then(
            function (answer) {
                if (answer === 'yes') {
                    emailService.post({ emailMessageId: data.EmailMessageId }, "sendEmail").then(function() {
                        toastr.success(ko.Localization('Naati.Resources.EmailMessage.resources.EmailSent'));
                        vm.getQueue();
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

    $.extend(true, vm, {
        searchComponentOptions: {
            searchCallback: vm.getQueue,
            searchTerm: vm.searchTerm,
            tableDefinition: {
                dataTable: {
                    source: vm.queue,
                    order: [
                        [0, "desc"]
                    ],
                    columnDefs: [
                        { targets: -1, orderable: false },
                        { targets: [1, 5], render: $.fn.dataTable.render.nullableDate(CONST.settings.shortDateTimeDisplayFormat) }
                    ]
                }
            }
        }
    });

    return vm;
});
