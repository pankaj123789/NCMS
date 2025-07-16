define([
], function () {
    function ViewModel(params) {
        var self = this;
        var defaultParams = {
            PublicNote: ko.observable(),
            PrivateNote: ko.observable(),
            ShowPublicNote: ko.observable(true),
            ShowPrivateNote: ko.observable(true),
            Enable: ko.observable(true),
            PublicNotesHelpText: ko.observable("")
        };

        $.extend(defaultParams, params);

        self.PublicNote = defaultParams.PublicNote;
        self.PrivateNote = defaultParams.PrivateNote;
        self.ShowPublicNote = defaultParams.ShowPublicNote;
        self.ShowPrivateNote = defaultParams.ShowPrivateNote;
        self.Enable = defaultParams.Enable;
        self.PublicNotesHelpText = defaultParams.PublicNotesHelpText;
    }

    return ViewModel;
});