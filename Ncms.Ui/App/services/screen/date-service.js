define([], function () {
    return {
        toPostDate: function (date) {
            return date ? moment(date, 'L').format('YYYY-MM-DD[T]HH:mm:ss[Z]') : '';
        },
        toUIDate: function (date) {
            return date ? moment(date).format('L') : '';
        },
        format: function (date, currentFormat, format) {
            return date ? moment(date, (currentFormat || CONST.settings.shortDateDisplayFormat)).format(format || CONST.settings.yearMonthDayFormat) : '';
        },
        today: function (format) {
            return moment().format(format || CONST.settings.yearMonthDayFormat);
        }
    };
});
