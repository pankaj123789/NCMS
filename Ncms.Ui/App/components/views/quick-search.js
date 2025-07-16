define([
    'components/views/auto-text-input',
    'modules/shared-filters',
    'plugins/router',
    'services/person-data-service',
], function (autoTextInput, sharedFilters, router, personService) {
    function ViewModel(params) {
        params = params || {};

        var self = this;
        params.component = self;

        var defaultParams = {
            attr: {
                'class': 'form-control input-sm bg-light no-border rounded padder',
            },
            resattr: {
                placeholder: 'Naati.Resources.Menu.resources.QuickSearch'
            },
            multiple: false,
            labelProp: 'NaatiNumberAndName',
            template: 'suggestionTmpl',
            delay: 700
        };

        defaultParams = $.extend({
            source: function (naatiNumber, callback) {
                if (!$.trim(naatiNumber)) {
                    return callback([]);
                }

                personService.getFluid('search', { term: naatiNumber }).then(function (data) {
                    $.each(data, function (i, d) {
                        d.BirthDateFormatted = moment(d.BirthDate).format(CONST.settings.shortDateDisplayFormat);
                        d.NaatiNumberAndName = '{0} - {1}'.format(d.NaatiNumber, d.Name);
                        d.Type = ko.Localization('Naati.Resources.Shared.resources.' + (d.PersonId ? 'Person' : 'Institution'));
                    });

                    callback(data);
                });
            },
            multiple: true,
            valueProp: 'NaatiNumber',
            labelProp: 'Name',
            resattr: {
                placeholder: 'Naati.Resources.Shared.resources.NAATINumber'
            },
        }, defaultParams);

        $.extend(defaultParams, params);

        autoTextInput.call(self, defaultParams);

        self.gotoPerson = function () {
            var value = self.dataValue();
            var naatiNumber = null;

            if (value) {
                naatiNumber = value.NaatiNumber;
            }
            else {
                naatiNumber = self.textValue();
                if (isNaN(parseInt(naatiNumber))) {
                    naatiNumber = null;
                }
            }

            if (naatiNumber) {
                if (value && value.InstitutionId != null)
                    router.navigate('organisation/' + naatiNumber);
                else if (value && value.PersonId != null)
                    router.navigate('person/' + naatiNumber);
            }
        };

        self.checkEnterPressed = function (data, e) {
            var keyCode = e.which || e.keyCode;
            if (keyCode == 13) {
                self.gotoPerson();
                return false;
            }
            return true;
        };

        self.dataValue.subscribe(function (newValue) {
            if (!newValue) return;
            self.gotoPerson();
        });
    }

    return ViewModel;
});
