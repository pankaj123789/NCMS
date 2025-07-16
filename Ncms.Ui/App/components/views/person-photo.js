define([
    'services/personimage-data-service',
    'services/person-data-service',
], function (personImageService, personService) {
    function ViewModel(params) {
        var thumbWidth = 137;
        var thumbHeight = 177;
        var defaultAvatarSrc = ko.Localization('Naati.Resources.Test.resources.DefaultAvatarBase64');
        var registrationDateText = ko.Localization('Naati.Resources.Shared.resources.RegistrationDate');
        var inactiveText = ko.Localization('Naati.Resources.Shared.resources.Inactive');
        var notRegisteredText = ko.Localization('Naati.Resources.Shared.resources.NotRegistered');
        var idExpiry = ko.Localization('Naati.Resources.Shared.resources.IdExpiry');
        var tooltip = ko.Localization('Naati.Resources.NaatiEntity.resources.NewPhotoTooltip');

        var self = this;

        var serverModel = {
            NaatiNumber: ko.observable(),
            HasPhoto: ko.observable(false),
            IsEportalActive: ko.observable(false),
            WebAccountCreateDate: ko.observable(),
            PhotoDate: ko.observable(),
            ExpiryDate: ko.observable(),
        };

        var canChangePhoto = params.changePhoto;
        
        var defaultParams = {
            naatiNumber: ko.observable(),
            changePhoto: function () { return false; },
            canChangePhoto: canChangePhoto,
            person: serverModel
        };

        params.component = self;

        $.extend(true, defaultParams, params);

        self.naatiNumber = defaultParams.naatiNumber;
        self.person = defaultParams.person;
        self.changePhoto = defaultParams.changePhoto;
        self.canChangePhoto = defaultParams.canChangePhoto;

        self.thumbStyle = ko.observable();

        self.photoTooltip = {
            container: 'body',
            placement: 'bottom',
            trigger: 'hover',
            title: function () {
                if (!self.canChangePhoto) {
                    return false;
                }

                if (self.person.HasPhoto()) {
                    tooltip = ko.Localization('Naati.Resources.NaatiEntity.resources.PhotoUploadedTooltip').format(moment(self.person.PhotoDate()).format(CONST.settings.shortDateTimeDisplayFormat));
                }

                return tooltip
            }
        };

        self.idCardSrc = ko.pureComputed(function () {
            self.person.PhotoDate();//just to force url refresh
            if (self.person.NaatiNumber()) {
                return self.person.HasPhoto() ? personImageService.newUrl(self.person.NaatiNumber(), { Height: thumbHeight, Width: thumbWidth }) : defaultAvatarSrc;
            }
            return defaultAvatarSrc;
        });

        self.thumbLoad = function (data, event) {
            if ($(event.target).width() > thumbWidth) {
                return self.thumbStyle({ 'margin-left': ((($(event.target).width() - thumbWidth) / 2) * -1) + 'px' });
            }
            if ($(event.target).height() > thumbHeight) {
                return self.thumbStyle({ 'width': '100%' });
            }
        };

        self.personRegistrationStatus = ko.pureComputed(function () {
            var isEportalActive = self.person.IsEportalActive();

            return isEportalActive != null
                ? isEportalActive
                    ? 'on'
                    : 'away'
                : 'busy';
        });

        self.registrationStatusTitle = ko.pureComputed(function () {
            var isEportalActive = self.person.IsEportalActive();
            var registrationDate = self.person.WebAccountCreateDate();

            return isEportalActive != null
                ? isEportalActive
                ? registrationDateText +
                ': ' +
                moment(registrationDate).format(CONST.settings.shortDateDisplayFormat)
                : inactiveText
                : notRegisteredText;
        });

        self.resetThumbStyle = function () {
            self.thumbStyle({ 'margin-left': '0px', 'width': 'auto' });
        };

        ko.pureComputed(function () {
            var naatiNumber = ko.unwrap(self.naatiNumber);
            load(naatiNumber);
        })

        if (ko.isObservable(self.naatiNumber)) {
            self.naatiNumber.subscribe(init);
        }

        init();

        function init() {
            var naatiNumber = ko.unwrap(self.naatiNumber);
            if (naatiNumber) {
                load(naatiNumber);
            }
        }

        function load(naatiNumber) {
            personService.getFluid(naatiNumber).then(function (data) {
                ko.viewmodel.updateFromModel(self.person, data);
            });
        }
    }

    return ViewModel;
});
