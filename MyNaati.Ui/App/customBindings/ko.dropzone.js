ko.bindingHandlers.dropzone = {
    update: function (element, valueAcessor, allBindingsAccessor) {
        var value = ko.unwrap(valueAcessor());
        var options = {
            autoProcessQueue: false,
            maxFilesize: 2048,
            thumbnailWidth: 80,
            thumbnailHeight: 80,
        };

        options = $.extend({}, options, value);
        value.component = new Dropzone(element, options);
    }
}