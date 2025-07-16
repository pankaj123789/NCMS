/* jshint boss:true*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'knockout', 'module'], factory);
    } else {
        factory(jQuery, ko);
    }
})(function ($, ko, module) {
    'use strict';

    ko.textCurrencyFormat = function (number, fractionDigits, decimalSeparator, thousandSeparator, format, negativeFormat, positiveFormat) {
        fractionDigits = isNaN(fractionDigits = Math.abs(fractionDigits)) ? 2 : fractionDigits;
        decimalSeparator = decimalSeparator == undefined ? "." : decimalSeparator;
        thousandSeparator = thousandSeparator == undefined ? "," : thousandSeparator;

        var isNegative = number < 0;
        var signal = isNegative ? "-" : "",
            i = String(parseInt(number = Math.abs(Number(number) || 0).toFixed(fractionDigits))),
            j = (j = i.length) > 3 ? j % 3 : 0;

        format = format || '{signal}{symbol}{value}';
        negativeFormat = negativeFormat || format;
        positiveFormat = positiveFormat || format;

        var applyFormat = format;
        if (isNegative) {
            applyFormat = negativeFormat;
        }
        else {
            applyFormat = positiveFormat;
        }

        return applyFormat.replace('{signal}', signal).replace('{symbol}', '$').replace('{value}', (j ? i.substr(0, j) + thousandSeparator : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousandSeparator) + (fractionDigits ? decimalSeparator + Math.abs(number - i).toFixed(fractionDigits).slice(2) : ""));
    };

    var bindingName = 'textCurrency';
    if (module && module.config() && module.config().name) {
        bindingName = module.config().name;
    }

    function update(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        return ko.bindingHandlers.text.update(element, function () {
            var accessor = ko.unwrap(valueAccessor());
            var value = 0;
            var format = '{signal}{symbol}{value}';
            var negativeFormat = format;
            var positiveFormat = format;

            if (!isNaN(parseFloat(accessor)) || !accessor) {
                value = accessor;
            }
            else {
                if ('format' in accessor) {
                    format = accessor.format;
                }

                negativeFormat = format;
                positiveFormat = format;

                if ('negativeFormat' in accessor) {
                    negativeFormat = accessor.negativeFormat;
                }
                if ('positiveFormat' in accessor) {
                    positiveFormat = accessor.positiveFormat;
                }

                value = ko.unwrap(accessor.value);
            }

            return ko.textCurrencyFormat(value, 2, '.', ',', format, negativeFormat, positiveFormat);
        });
    }

    return ko.bindingHandlers[bindingName] = {
        update: update
    };
});