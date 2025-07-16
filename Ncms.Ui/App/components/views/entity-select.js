define(['components/views/select-component', 'services/entity-data-service'], function (selectComponent, entityService) {
    function getDataField(data, field) {
        if (ko.isObservable(field)) {
            return data[field()];
        }

        if (typeof field === 'function') {
            return field(data);
        }

        return data[field];
    }

    function ViewModel(params) {
        var self = this;

        var defaultParams = {
            ajaxBehaviour: {
                cache: true
            },
            options: ko.observableArray([]),
            validateEntity: function (data) {
                return getDataField(data, self.valueField) && getDataField(data, self.textField);
            },
            dataCallback: null,
            mapOptions: null
        };

        params.component = self;

        $.extend(defaultParams, params);

        self.entity = defaultParams.entity;
        self.valueField = defaultParams.valueField;
        self.textField = defaultParams.textField;
        self.validateEntity = defaultParams.validateEntity;
        self.dataCallback = defaultParams.dataCallback;
        self.existingMapOptions = defaultParams.mapOptions;
        self.mapOptions = function (data) {
            if (!self.validateEntity(data)) {
                return null;
            }

            var mappedData = { value: getDataField(data, self.valueField), text: getDataField(data, self.textField) };

            if (self.existingMapOptions && typeof self.existingMapOptions === 'function') {
                mappedData = self.existingMapOptions(data, mappedData);
            }

            return mappedData;
        }

        // With 'call()' (or 'apply()') you can set the value of 'this', and invoke a function as a new method of an existing object
        // Which means 'this' within selectComponent, is now self, aka the current object 'this'
        // This is essentially making 'entity-select' inherit from 'select-component'
        selectComponent.call(self, defaultParams);

        self.loadOptions = function () {
            entityService.getFluid(self.entity, null, defaultParams.ajaxBehaviour).then(function(data) {
                if (self.dataCallback && typeof self.dataCallback === 'function') {
                    var callbackData = self.dataCallback(data);

                    if (Q.isPromise(callbackData)) {
                        callbackData.then(function(options) {
                            self.options($.map(options, self.mapOptions));
                        }).catch(function() {
                            self.options($.map(data, self.mapOptions));
                        });
                    } else {
                        self.options($.map(callbackData, self.mapOptions));
                    }
                } else {
                    self.options($.map(data, self.mapOptions));
                }
            });
        };

        self.loadOptions();
    }

    return ViewModel;
});
