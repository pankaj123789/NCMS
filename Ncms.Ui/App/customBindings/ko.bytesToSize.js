ko.bytesToSize = function (bytes) {
    if (bytes == 0) return '0 Byte';
    var k = 1024;
    var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
    var i = Math.floor(Math.log(bytes) / Math.log(k));
    return (bytes / Math.pow(k, i)).toPrecision(3) + ' ' + sizes[i];
};

ko.bindingHandlers.bytesToSize = {
    update: function (element, valueAcessor, allBindingsAccessor) {
        return ko.bindingHandlers.text.update(element, function () {
            var value = ko.unwrap(valueAcessor());
            return ko.bytesToSize(value);
        });
    }
}