define([
    'services/application-data-service',
    'services/util'
], function (applicationService, util) {
    function ViewModel(params) {
        var self = this;

        var defaultParams = {
            emails: ko.observable(),
            loaded: ko.observable(true)
        };

        $.extend(defaultParams, params);

        self.emails = defaultParams.emails;
        self.loaded = defaultParams.loaded;
        self.computedEmails = ko.pureComputed(function () {
            return ko.utils.arrayMap(self.emails(), function (data) {
                data.Attachments = data.Attachments || ko.observableArray([]);
                return $.extend({
                    ContentId: ko.observable(util.guid()),
                    FrameHeight: ko.observable("0px"),
                    FormattedCreatedDate: data.CreatedDate ? moment(data.CreatedDate()).format("DD/MM/YYYY h:mm a") : null
                }, data);
            });
        });

        self.setEmailContent = function (element, data) {
            var html = data.EmailContent();
            var $iframe = $('#{0}'.format(data.ContentId()));
            var iframe = $iframe[0];

            iframe.contentWindow.document.open();
            iframe.contentWindow.document.write(html);
            iframe.contentWindow.document.close();

            function setHeight() {
                try {
                    var height = $iframe.contents().find("html").height();
                    data.FrameHeight('{0}px'.format(height));
                }
                catch (err) { }
            }

            setHeight();
            $iframe.load(setHeight);
        };
    }

    return ViewModel;
});