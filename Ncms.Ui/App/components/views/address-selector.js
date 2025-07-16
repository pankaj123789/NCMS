define(['components/views/auto-text-input', 'services/address-data-service'], function (autoTextInput, addressService) {
    function ViewModel(params) {
        var self = this;

        var defaultParams = {
            source: function (address, callback) {
                addressService.get({ address: address }).then(function (data) {
                    var mapped = $.map(data.Addresses, function (a) {
                        return { value: JSON.stringify(a), label: a.FullAddress };
                    });
                    callback(mapped);
                });
            },
            multiple: false,
            valueProp: 'value',
            labelProp: 'label',
            resattr: {
                placeholder: 'Naati.Resources.Shared.resources.AddressPlaceholder'
            }
        };

        params.component = self;

        $.extend(defaultParams, params);

        self.source = defaultParams.source;
        self.multiple = defaultParams.valueField;
        self.valueProp = defaultParams.textField;
        self.labelProp = defaultParams.validateEntity;
        self.resattr = defaultParams.dataCallback;
        
        autoTextInput.call(self, defaultParams);
    }

    return ViewModel;
});
