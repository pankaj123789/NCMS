define(['components/views/select-component'], function (selectComponent) {

    function vm(params) {
        var self = this;

        var defaultParams = {
            valueField: 'value',
            textField: 'text',
            multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null },
            addChooseOption: false,
            options: ko.observableArray([
                {
                    value: 'M',
                    text: 'Male'
                },
                {
                    value: 'F',
                    text: 'Female'
                },
                {
                    value: 'X',
                    text: 'Unspecified'
                }
            ]),
            value: ko.observable(),
        };

        params.component = self;

        $.extend(defaultParams, params);

        // With 'call()' (or 'apply()') you can set the value of 'this', and invoke a function as a new method of an existing object
        // Which means 'this' within selectComponent, is now self, aka the current object 'this'
        // This is essentially making 'gender-select' inherit from 'select-component'
        selectComponent.call(self, defaultParams);
    }

    return vm;
});
