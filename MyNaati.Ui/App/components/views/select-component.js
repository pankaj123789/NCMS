define([], function () {
    function ViewModel(params) {
        var self = this;

        var defaultParams = {
            selectedOptions: [],
            options: ko.observableArray(),
            value: ko.observable(),
            disable: ko.observable(false),
            multiselect: ko.observable({}),
            attr: ko.observable({}),
            addChooseOption: true,
            multiple: true,
            optionsAfterRender: null,
            optionsText: 'text',
            optionsValue: 'value',
            afterRender: function () { },
            selectIfSingle: false
        };

        params.component = self;

        $.extend(true, defaultParams, params);

        self.disable = defaultParams.disable;

        self.selectedOptions = ko.isObservable(defaultParams.selectedOptions)
            ? defaultParams.selectedOptions
            : ko.observableArray(defaultParams.selectedOptions);
        self.options = defaultParams.options;
        self.value = defaultParams.value;
        self.multiselect = defaultParams.multiselect;
        self.attr = defaultParams.attr;
        self.optionsAfterRender = defaultParams.optionsAfterRender;
        self.optionsText = defaultParams.optionsText;
        self.optionsValue = defaultParams.optionsValue;
        self.selectIfSingle = defaultParams.selectIfSingle;
        self.customOptions = ko.computed(function() {
            var options = (typeof self.options === 'function' ? self.options() : self.options).slice();
            if (!defaultParams.multiple && defaultParams.addChooseOption) {
                addChooseOption(options);
            }
            return options;
        });

        if (defaultParams.multiple) {
            if (!self.attr()) {
                self.attr({ multiple: 'multiple' });
            } else if (!self.attr().multiple || self.attr().multiple !== 'multiple') {
                var attr = self.attr();
                attr.multiple = 'multiple';
                self.attr(attr);
            }
        } else {
            if (!params.selectedOptions) {
                self.selectedOptions = ko.computed({
                    read: function () {
                        return [ko.toJS(self.value)];
                    },
                    write: function () { }
                });
            }
        }

        self.getJson = function () {
            return ko.toJS({ selectedOptions: self.selectedOptions.slice() });
        }

        defaultParams.afterRender.apply(self);

        function addChooseOption(options) {
            if (options.length > 0 && options[0].value !== '') {
                var option = {};
                option[ko.toJS(self.optionsValue)] = '';
                option[ko.toJS(self.optionsText)] = 'Choose...';
                options.unshift(option);
            }
        }
    }

    return ViewModel;
});
