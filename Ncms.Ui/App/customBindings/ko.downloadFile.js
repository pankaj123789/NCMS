ko.bindingHandlers.downloadFile = {
    init: function (element, valueAccessor) {    

        function doDownload(value) {
            var callback = value.callback || null;
            var successMessage = ko.utils.unwrapObservable(value.successMessage || null);
            var errorMessage = ko.utils.unwrapObservable(value.errorMessage || null);
        
            var url = value.url || '';
            if (jQuery.type(url) == "function") {
                url = url();
            }

            if (!url) {
                return;
            }

            require(['services/util'],
                function(util) {
                    Promise.resolve(url).then(function (address) {
                        util.downloadFile(address, successMessage, errorMessage, callback);
                    });
                });
        }

        ko.applyBindingsToNode(element, {
            click: function () {
                var value = valueAccessor();
                var beforeDownload = value.beforeDownload || null;

                if (beforeDownload && typeof (beforeDownload) == 'function') {
                    var proceed = beforeDownload();
                    if (proceed === true) {
                        doDownload(value);
                    } else if (proceed !== false) {
                        proceed.then(function () { doDownload(value); return true; });
                    }
                }
                else {
                    doDownload(value);
                }
            }
        });
    }
};
