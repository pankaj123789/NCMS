define([
], function () {
    function ViewModel(params) {
        var self = this;
        var defaultParams = {
            subject: ko.observable(),
            body: ko.observable(),
            enable: ko.observable(true),
        };

        $.extend(defaultParams, params);

        self.emailSubject = defaultParams.subject;
        self.emailBody = defaultParams.body;

        self.wysiwygOptions = {
            value: self.emailBody
        };
    }

    return ViewModel;
});