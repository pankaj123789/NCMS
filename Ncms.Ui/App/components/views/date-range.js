define(['modules/enums', 'services/screen/date-service'], function (enums, dateService) {
    function ViewModel(params) {
        var self = this;

        self.disableDates = ko.pureComputed(function () {
            return self.filterOptions.value() !== enums.DateRanges.Custom;
        });

        var dateRangeOptions = [];

        for (var option in enums.DateRanges) {
            if (option == 'None' && !params.allowNone) continue;

            if (option !== 'list' && enums.DateRanges.hasOwnProperty(option)) {
                dateRangeOptions.push({ value: enums.DateRanges[option], text: ko.Localization('Naati.Resources.Test.resources.' + option) });
            }
        }

        if (params.filterOptions && params.filterOptions.value) {
            if (!params.filterOptions.selectedOptions) {
                params.filterOptions.selectedOptions = ko.observableArray([]);
            }

            if (!~params.filterOptions.selectedOptions.indexOf(params.filterOptions.value)) {
                params.filterOptions.selectedOptions.push(params.filterOptions.value);
            }
        }

        var fromRequired = function () {
            if (params.fromDateOptions === undefined)
                return false;
            return (params.fromDateOptions.value.rules ? params.fromDateOptions.value.rules()[0].rule : '') === 'required';
        }

        var toRequired = function () {
            if (params.toDateOptions === undefined)
                return false;
            return (params.toDateOptions.value.rules ? params.toDateOptions.value.rules()[0].rule : '') === 'required';
        }

        var dateRangeFilterOption = function () {
            if (params.dateRangeFilterOption === undefined)
                return enums.DateRanges.Custom;
            return enums.DateRanges.ThisMonth;
        }

        var defaultParams = {
            filterOptions: {
                value: ko.observable(dateRangeFilterOption()),
                options: ko.observableArray(dateRangeOptions),
                multiple: false,
                addChooseOption: false
            },
            fromDateOptions: {
                disable: self.disableDates,
                require: fromRequired(),
                resattr: {
                    placeholder: 'Naati.Resources.Shared.resources.From'
                }
            },
            toDateOptions: {
                disable: self.disableDates,
                require: toRequired(),
                resattr: {
                    placeholder: 'Naati.Resources.Shared.resources.To'
                }
            }
        };

        params.component = self;

        $.extend(true, defaultParams, params);

        self.filterOptions = defaultParams.filterOptions;
        self.fromDateOptions = defaultParams.fromDateOptions;
        self.toDateOptions = defaultParams.toDateOptions;

        if (!ko.isObservable(self.filterOptions.value)) {
            self.filterOptions.value = ko.observable(self.filterOptions.value);
        }

        self.updateDates = function (filterType) {
            var from = '';
            var to = '';

            switch (filterType) {
                case enums.DateRanges.None:
                    from = null;
                    to = null;
                    break;
                case enums.DateRanges.Today:
                    var today = moment().toDate();
                    from = today;
                    to = today;
                    break;
                case enums.DateRanges.Yesterday:
                    var yesterday = moment().subtract(1, 'days').toDate();
                    from = yesterday;
                    to = yesterday;
                    break;
                case enums.DateRanges.LastWeek:
                    from = moment().startOf('week').add(-7, 'days').toDate();
                    to = moment(from).add(6, 'days').toDate();
                    break;
                case enums.DateRanges.ThisWeek:
                    from = moment().startOf('week').toDate();
                    to = moment(from).add(6, 'days').toDate();
                    break;
                case enums.DateRanges.ThisMonth:
                    from = moment().startOf('month').toDate();
                    to = moment().endOf('month').toDate();
                    break;
                case enums.DateRanges.ThisYear:
                    from = moment().startOf('year').toDate();
                    to = moment().endOf('year').toDate();
                    break;
                case enums.DateRanges.Custom:
                    from = convertDate(self.fromDateOptions.component.value());
                    to = convertDate(self.toDateOptions.component.value());
                    break;
            }
            
            self.fromDateOptions.component.value(dateService.toUIDate(from));
            self.toDateOptions.component.value(dateService.toUIDate(to));
        };

        if (self.filterOptions.value !== enums.DateRanges.Custom) {
            var interval = setInterval(function () {
                if (self.fromDateOptions.component && self.toDateOptions.component && self.filterOptions.component) {
                    clearInterval(interval);
                    self.updateDates(self.filterOptions.value());
                }
            }, 10);
        }

        self.showFilter = ko.pureComputed(function () {
            var options = ko.toJS(self.filterOptions.options);
            return options.length > 1;
        });

        self.filterOptionsSubscription = self.filterOptions.value.subscribe(self.updateDates);

        var options = ko.toJS(self.filterOptions.options);
        if (options.length === 1) {
            self.filterOptions.value(ko.toJS(options[0].value));
        }

        self.dispose = function () {
            self.filterOptionsSubscription.dispose();
        }

        self.getJson = function() {
            return ko.toJS({
                fromDateOptions: {
                    value: dateService.toPostDate(self.fromDateOptions.component.value())
                },
                toDateOptions: {
                    value: dateService.toPostDate(self.toDateOptions.component.value())
                },
                filterOptions: {
                    value: self.filterOptions.component.value()
                }
            });
        };

        function convertDate(value){
            if (typeof value == "string" && moment(value,'DD/MM/YYYY') !== "Invalid date") {
                return dateService.toPostDate(value);
            }
            return value;
        }
    }


    return ViewModel;
});
