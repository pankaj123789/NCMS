define([], function () {
    var hourText = ko.Localization('Naati.Resources.Shared.resources.Hour');
    var hoursText = ko.Localization('Naati.Resources.Shared.resources.Hours');
    var minuteText = ko.Localization('Naati.Resources.Shared.resources.Minute');
    var minutesText = ko.Localization('Naati.Resources.Shared.resources.Minutes');
    var secondText = ko.Localization('Naati.Resources.Shared.resources.Second');
    var secondsText = ko.Localization('Naati.Resources.Shared.resources.Seconds');

    function getTimeParts(time) {
        var tmp = time.split(':');
        return {
            second: parseInt(tmp.pop()),
            minute: tmp.length > 0 ? parseInt(tmp.pop()) : 0,
            hour: tmp.length > 0 ? parseInt(tmp.pop()) : 0
        };
    }

    function getPartMessage(timePart, singularText, pluralText) {
        if (timePart == 0) {
            return '';
        }

        if (timePart == 1) {
            return timePart + ' ' + singularText;
        }

        return timePart + ' ' + pluralText;
    }

    function getRemainingMessage(timeParts) {
        return getPartMessage(timeParts.hour, hourText, hoursText) + ' ' +
            getPartMessage(timeParts.minute, minuteText, minutesText) + ' ' +
            getPartMessage(timeParts.second, secondText, secondsText);
    }

    function ViewModel(params) {
        var self = this;
        var selectOrDropFileText = ko.Localization('Naati.Resources.Shared.resources.SelectOrDropFile');
        var uploadLoadedMessage = ko.Localization('Naati.Resources.Shared.resources.UploadLoadedMessage');
        var uploadRemainingMessage = ko.Localization('Naati.Resources.Shared.resources.UploadRemainingMessage');

        var defaultParams = {
            events: {
                fileuploadprogress: function (e, data) {
                    self.uploadLoadedMessage(uploadLoadedMessage.format(ko.bytesToSize(data.loaded), ko.bytesToSize(data.total)));

                    self.percent(parseInt(data.loaded / data.total * 100));
                    self.loaded(data.loaded);
                    self.total(data.total);
                    self.bitrates.push(data.bitrate);
                },
                fileuploadstart: function (e, data) {
                    self.uploadLoadedMessage(uploadLoadedMessage.format(0, 0));
                    self.uploadRemainingMessage(uploadRemainingMessage.format("-", "-"));
                    self.total(0);
                    self.loaded(0);
                    self.bitrates([]);
                    self.percent(0);
                    self.isLoading(true);
                },
                fileuploadalways: function () {
                    self.isLoading(false);
                }
            },
            files: ko.observableArray()
        };

        defaultParams = $.extend(true, defaultParams, params);

        self.selected = ko.observable(false);
        self.uploadLoadedMessage = ko.observable();
        self.uploadRemainingMessage = ko.observable();
        self.total = ko.observable();
        self.loaded = ko.observable();
        self.bitrates = ko.observableArray();
        self.percent = ko.observable();
        self.isLoading = ko.observable();
        self.fileName = ko.observable();

        self.loaded.subscribe(function () {
            var bitrateAvg = 0;
            $.each(self.bitrates(), function (i, e) {
                bitrateAvg += e;
            });

            bitrateAvg = self.bitrates().length === 0 ? 0 : bitrateAvg / self.bitrates().length;

            var remaining = bitrateAvg === 0 ? 0 : (self.total() - self.loaded()) * 8 / bitrateAvg;
            remaining = moment.duration({ s: remaining }).format('hh:mm:ss');

            var timeParts = getTimeParts(remaining);
            var remainingMessage = getRemainingMessage(timeParts);

            self.uploadRemainingMessage(uploadRemainingMessage.format(remainingMessage, ko.bytesToSize(bitrateAvg).toLowerCase() + 'ps'));
        });

        self.fileupload = {
            fileUploadOptions: {
                url: defaultParams.url,
                drop: function (e, data) {
                    $.each(data.files, function (index, file) {
                        self.fileName(file.name);
                    });
                },
                change: function (e, data) {
                    $.each(data.files, function (index, file) {
                        self.fileName(file.name);
                    });
                },
                add: function (e, data) {
                    defaultParams.files([data]);
                },
                submit: function (e, data) {
                    data.formData = ko.isObservable(defaultParams.formData) || typeof defaultParams.formData === 'function' ? defaultParams.formData() : defaultParams.formData;
                }
            },
            events: defaultParams.events
        };
        self.reset = function () {
            self.fileName('');
            defaultParams.files([]);
            self.isLoading(false);
        };

        self.buttonLabel = function () {
            var fileName = self.fileName();
            return selectOrDropFileText + (fileName ? " (" + fileName + ")" : "");
        };

        self.fileName.subscribe(function (newValue) {
            if (newValue) {
                self.selected(true);
            }
            else {
                self.selected(false);
            }
        });

        params.component = self;
    }

    return ViewModel;
});
