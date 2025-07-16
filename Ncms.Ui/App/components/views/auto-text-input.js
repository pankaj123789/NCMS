define(['components/views/text-input'], function (textInput) {
    function ViewModel(params) {
        var self = this;

        params.component = self;

        var defaultParams = {
            multiple: false,
            multiSeperator: ',',
            minLength: 1,
            dataValue: ko.observable(),
            values: ko.observableArray([]),
            dataValues: ko.observableArray([]),
            textValue: ko.observable(),
            event: {},
            source: [],
            change: null,
            close: null,
            create: null,
            focus: null,
            open: null,
            response: null,
            search: null,
            select: null
        };

        $.extend(defaultParams, params);

        textInput.call(self, defaultParams);

        self.multiple = defaultParams.multiple;
        self.multiSeperator = defaultParams.multiSeperator;
        self.minLength = defaultParams.minLength;
        self.useMinLength = true;
        self.dataValue = ko.isObservable(defaultParams.dataValue)
            ? defaultParams.dataValue
            : ko.observable(defaultParams.dataValue);
        self.values = null;
        self.dataValues = null;
        self.textValue = defaultParams.textValue;
        self.event = defaultParams.event;
        self.source = defaultParams.source;
        self.valueProp = defaultParams.valueProp;
        self.labelProp = defaultParams.labelProp;
        self.template = defaultParams.template;
        self.delay = defaultParams.delay;
        self.container = defaultParams.container;
        self.change = defaultParams.change;
        self.close = defaultParams.close;
        self.create = defaultParams.create;
        self.focus = defaultParams.focus;
        self.open = defaultParams.open;
        self.response = defaultParams.response;
        self.search = defaultParams.search;
        self.select = defaultParams.select;

        if (self.multiple) {
            self.useMinLength = false;
            self.values = defaultParams.values;
            self.dataValues = defaultParams.dataValues;

            self.regexSeparator = new RegExp(self.multiSeperator + '\s*');
            self.regexEndsWithSeparator = new RegExp(self.multiSeperator + '$');

            if (self.value()) {
                self.values(ko.toJS(self.value));
            }

            if (self.dataValue()) {
                self.dataValues(ko.toJS(self.dataValue));
            }

            var seperator = self.multiSeperator + ' ';

            if (!$.isArray(self.values())) {
                self.values([self.values()]);
            }

            if (!$.isArray(self.dataValues())) {
                self.dataValues([self.dataValues()]);
            }

            if (self.values().length > 0) {
                self.value(self.values().join(seperator) + seperator);
            }

            function cleanArray(actual) {
                var newArray = [];
                for (var i = 0; i < actual.length; i++) {
                    if (actual[i]) {
                        newArray.push(actual[i].trim());
                    }
                }

                return newArray;
            };

            self.split = function (val) {
                return cleanArray(val.split(self.regexSeparator));
            };

            self.extractTerms = function (term) {
                var currentTerms = self.split(term);
                var lastTerm = self.regexEndsWithSeparator.test(term) ? '' : currentTerms.pop() || '';

                return {
                    current: currentTerms,
                    last: lastTerm
                };
            };

            self.existingSource = self.source;
            self.existingFocus = self.focus;
            self.existingSelect = self.select;

            var sourceIsString = typeof self.source === 'string';
            var sourceIsFunction = typeof self.source === 'function' && !ko.isObservable(self.source);

            if (sourceIsString || sourceIsFunction) {
                if (sourceIsString) {
                    self.source = function (term, response) {
                        $.getJSON(self.existingSource, self.extractTerms(term).last, response);
                    };
                } else if (sourceIsFunction) {
                    self.source = function (term, response) {
                        var callback = function (data) {
                            response(data);
                        };

                        self.existingSource(self.extractTerms(term).last, callback);
                    };
                }

                self.existingSearch = self.search;

                self.search = function (event, ui) {
                    var terms = self.extractTerms(this.value);
                    var currentTerms = terms.current;
                    var lastTerm = terms.last;

                    self.values(currentTerms);
                    self.values.push(lastTerm);

                    var mappedValues = ko.utils.arrayMap(self.values(), function (val) {
                        return ko.utils.arrayFirst(self.dataValues(), function (dataVal) {
                            return dataVal[self.valueProp].toString() === val;
                        });
                    });

                    var filteredValues = ko.utils.arrayFilter(mappedValues, function (data) {
                        return data !== null;
                    });

                    self.dataValues(filteredValues);

                    if (lastTerm.length < self.minLength) {
                        event.preventDefault();
                    } else if (self.existingSearch) {
                        self.existingSearch(event, ui);
                    }
                };
            } else {
                self.source = function (term, response) {
                    var source = ko.isObservable(self.existingSource)
                        ? self.existingSource()
                        : self.existingSource;
                    var filter = new RegExp(self.extractTerms(term).last);
                    response(ko.utils.arrayFilter(source, function (item) {
                        return filter.test(item);
                    }));
                };
            }

            self.focus = function (event, ui) {
                if (self.existingFocus) {
                    self.existingFocus(event, ui);
                }

                event.preventDefault();
            };

            self.select = function (event, ui) {
                //self.values.push(self.value().toString());
                self.dataValues.push(self.dataValue());

                var seperator = self.multiSeperator + ' ';

                self.values.pop();
                self.values.push(ui.item.actual.toString());

                this.value = self.values().join(seperator) + seperator;

                if (self.existingSelect) {
                    self.existingSelect(Event, ui);
                }

                event.preventDefault();
            };
        }

        self.getJson = function () {
            var value, dataValue;

            if (self.multiple) {
                value = self.values();
                dataValue = ko.utils.arrayMap(self.dataValues(), function (val) {
                    var data = {};

                    data[self.valueProp] = val[self.valueProp];
                    data[self.labelProp] = val[self.labelProp];

                    return data;
                });
            } else {
                value = self.value();
                dataValue = {};

                dataValue[self.valueProp] = value;
                dataValue[self.labelProp] = self.dataValue() ? self.dataValue()[self.labelProp] : null;
            }

            return ko.toJS({ value: value, dataValue: dataValue });
        };

        self.auto = {
            value: self.value,
            dataValue: self.dataValue,
            source: self.source,
            valueProp: self.valueProp,
            labelProp: self.labelProp,
            template: self.template,
            options: {
                minLength: self.useMinLength ? self.minLength : null,
                change: self.change,
                close: self.close,
                create: self.create,
                focus: self.focus,
                open: self.open,
                response: self.response,
                search: self.search,
                select: self.select,
                delay: self.delay,
                container: self.container
            }
        };
    }

    return ViewModel;
});
