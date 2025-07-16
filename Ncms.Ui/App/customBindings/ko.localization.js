// See https://github.com/tfsjohan/Knockout-Localization-Binding for more details.

(function (ko) {

    var resources;

    function localization() {
		var baseUrl = window.baseUrl;
		var serviceName = 'api/resource'; // route to the same origin Web Api controller

        return getResource();

        function getResource(success) {
            var url = baseUrl + serviceName;
            return $.ajax({
                url: url,
                cache: false,
                async: false,
                type: "GET",
                data: {}
            });
        }
    }

    /* returns immediately regardless of whether we have our localisation dictionary loaded */
    /* args contains a list of localization strings to interpolate in */
    ko.Localization = function (key /* , args */) {
        if (!resources) {
            localization().then(function (response) {
                resources = response.data;
            });
        }

        return getLocalizedText(key);
    };

    ko.SetLanguage = function (language) {
        var date = new Date();
        date.setMonth(date.getMonth() + 6);
        document.cookie = "Culture=" + encodeURIComponent(language) + "; Expires=" + date.toUTCString();
        location.reload();
    };

    ko.Format = function (formatStr /* , args */) {
        if (arguments.length <= 1) {
            return formatStr;
        }

        for (var i = 0; i < arguments.length - 1; i++) {
            var reg = new RegExp("\\{" + i + "\\}", "gm");
            formatStr = formatStr.replace(reg, arguments[i + 1]);
        }
        return formatStr;
    };

    ko.NumberToWordsFromResource = function (number) {
        var numberResourceKey = "Number_" + number;
        var numberResourceValue = ko.Localization(numberResourceKey);
        return (numberResourceValue.indexOf('[') > -1 ? number.toString() : numberResourceValue);
    };

    var vm = {
    };

    try {
        ko.applyBindings();
    } catch (e) { }

    function updateLocalized(element, binding, callback, attribute) {
        if (resources == null) {
            localization().then(function (data) {
                resources = data;
                updateUiElement();
            });
        } else {
            updateUiElement();
        }

        function updateUiElement() {
            var text = getLocalizedText(binding);
            callback(text, attribute);
        }
    }

    // Accept syntax    restext: 'mytext' and restext: { key: 'mytext' } and restext: { key: 'mytext', fallback: 'defaultText' }
    ko.bindingHandlers.restext = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel, context) {
            var binding = ko.utils.unwrapObservable(valueAccessor());
            updateLocalized(element, binding, updateTextFunc);

            function updateTextFunc(text) {
                ko.bindingHandlers.text.update(element, function () { return text; }, allBindingsAccessor, viewModel, context);
            }
        }
    };

    // Accept syntax    reshtml: 'myhtml' and reshtml: { key: 'myhtml' } and reshtml: { key: 'myhtml', fallback: 'defaultHtml' }
    ko.bindingHandlers.reshtml = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel, context) {
            var binding = ko.utils.unwrapObservable(valueAccessor());
            updateLocalized(element, binding, updateHtmlFunc);

            function updateHtmlFunc(text) {
                ko.bindingHandlers.html.update(element, function () { return text; }, allBindingsAccessor, viewModel, context);
            }

        }
    };

    // Accept syntax    reshref: 'myHref' and reshref: { key: 'myHref' } and restext: { key: 'mytext', fallback: 'defaultHref' }
    ko.bindingHandlers.reshref = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel, context) {
            var binding = ko.utils.unwrapObservable(valueAccessor());
            updateLocalized(element, binding, updateHrefFunc);

            function updateHrefFunc(text) {
                ko.bindingHandlers.attr.update(element, function () { return { href: text }; }, allBindingsAccessor, viewModel, context);
            }
        }
    };

    // Accept syntax    ressrc: 'mySrc' or ressrc: { key: 'mySrc' }  or restext: { key: 'mytext', fallback: 'defaultSrc' }
    ko.bindingHandlers.ressrc = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel, context) {
            var binding = ko.utils.unwrapObservable(valueAccessor());
            updateLocalized(element, binding, updateSrcFunc);

            function updateSrcFunc(text) {
                ko.bindingHandlers.attr.update(element, function () { return { src: text }; }, allBindingsAccessor, viewModel, context);
            }
        }
    };

    // Accept ressrc: { color: 'myColor', font-size: 'myfontSize' }
    ko.bindingHandlers.resattr = {
        update: function (element, valueAccessor, allBindingsAccessor, viewModel, context) {
            var json = ko.utils.unwrapObservable(valueAccessor());
            for (var attr in json) {
                var updateSrcFunc = function (text, attribute) {
                    ko.bindingHandlers.attr.update(element, function () {
                        var x = {};
                        x[attribute] = text;
                        return x;
                    }, allBindingsAccessor, viewModel, context);
                };
                updateLocalized(element, json[attr], updateSrcFunc, attr);

            }
        }
    };

    function getLocalizedText(binding) {
        if (typeof resources === "undefined") {
            throw "ko.localizationbinding.getLocalizedText: resources object is not defined";
        }

        // transform arrays into a string with arguments
        if (binding instanceof Array) {
            binding = { key: binding[0], args: binding.slice(1) };
        }

        // Accept both restext: 'mytext' and restext: { key: 'mytext' }
        if (Object.prototype.toString.call(binding) === '[object String]') {
            binding = { key: binding };
        }

        var key = binding.key;

        var tmp = key.split('.');

        var innerKey = tmp.pop();
        var fileKey = tmp.join('.');

        var text = '';

        var file = resources[fileKey];
        if (file) {
            text = file[innerKey];
        }

        // CJM: fallback to '[key]' if we don't have the binding at the moment.
        text = text || binding.fallback || ("[" + key + "]");

        // Handle placeholders, where the localized text can be 'Hello #firstName!'. 
        // For parameterized text the binding will look like restext: { key: 'hello', params: { firstName: firstNameObservable } }
        if (binding.params) {
            for (var replaceKey in binding.params) {
                var replacement = binding.params[replaceKey];
                if (typeof replacement === "function") {
                    replacement = ko.utils.unwrapObservable(replacement());
                }
                text = text.replace("#" + replaceKey, replacement);
            }
        }

        // handle arguments, where the localised text can be 'Hello {0}!'
        // for argumented text the binding will look like restext: { key: 'hello', args: ['firstName'] }
        if (binding.args) {
            for (var i = 0; i < binding.args.length; i++) {
                binding.args[i] = resources[binding.args[i]] || binding.args[i];
            }

            binding.args.splice(0, 0, text);
            return ko.Format.apply(this, binding.args);
        }

        return text;
    }

})(ko);
