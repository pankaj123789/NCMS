define([
    'services/audit-data-service',
    'services/screen/message-service'
], function (auditService, message) {

    function getHeader(log) {
        var logHeader = ko.Localization('Naati.Resources.Shared.resources.LogHeader');
        var date = '<b>' + moment(log.DateTime).format('l') + '</b>';
        var userName = '<b>' + log.Username + '</b>';

        return logHeader.format(date, userName);
    }

    function getFormattedJson(json, separator, prefix) {
        var logs = [];

        for (var j in json) {
            var value = json[j];
            if (value && typeof (value) === 'object') {
                value = '<br />' + getFormattedJson(value, separator, prefix + '&nbsp;&nbsp;&nbsp;&nbsp;');
            }
            logs.push(prefix + '<b>' + j + '</b>' + separator + value);
        }

        return logs.join('<br />');
    }

    function getFormattedKeyValue(key, value, separator) {
        if ($.trim(key).toLowerCase() === 'input') {
            value = $.trim(value);

            if (value[0] === "'") {
                value = value.substr(1);
            }

            if (value[value.length - 1] === "'") {
                value = value.substr(0, value.length - 1);
            }

            return '<b>' + key + '</b>' + separator + '<br />' + getFormattedJson(JSON.parse(value), separator, '&nbsp;&nbsp;&nbsp;&nbsp;');
        }
        else {
            return '<b>' + key + '</b>' + separator + value;
        }
    }

    function formatLog(log) {
        if (!log) return;

        var tmp = log.split('Changed values:');

        if (tmp.length === 1) {
            tmp = tmp[0].split('New values:');
        }

        if (tmp.length < 2) {
            return log;
        }

        tmp = tmp[1].split(';');

        var newTmp = $.map(tmp, function (t) {
            var s = t.split('=');
            if (s.length > 1) {
                return getFormattedKeyValue(s[0], s[1], ': ');
            }

            s = t.split('changed from');
            if (s.length > 1) {
                return getFormattedKeyValue(s[0], s[1], ' changed from ');
            }

            return t;
        });

        return newTmp.join('<br/>');
    }

    function vm(params) {
        var self = this;

        var defaultParams = {
            logs: ko.observableArray([]),
            request: ko.observable(),
            showInModal: true
        };

        params.component = self;

        $.extend(defaultParams, params);
        self.formatLog = formatLog;
        self.getHeader = getHeader;
        self.logs = defaultParams.logs;
        self.showInModal = defaultParams.showInModal;
        self.request = defaultParams.request;

        self.show = function (request) {
            var defaultRequest = {
                PageSize: 1000,
                PageNumber: 1
            };

            $.extend(defaultRequest, request);

            self.logs([]);

            auditService.get(defaultRequest).then(function (data) {
                data = data.sort(function (left, right) {
                    return moment(left.DateTime).diff(moment(right.DateTime)) * -1;
                });

                ko.utils.arrayForEach(data, function (d) {
                    d.detail = function () {
                        message.alert({
                            title: getHeader(d),
                            content: formatLog(d.ChangeDetail),
                            css: 'modal-lg'
                        });
                    };
                });

                self.logs(data);
            });

            if (self.showInModal) {
                $('#auditLogModal').modal('show');
            }
        };

        self.tableDefinition = {
            headerTemplate: 'auditLog-header-template',
            rowTemplate: 'auditLog-row-template'
        };

        self.tableDefinition.dataTable = {
            source: self.logs,
            columnDefs: [
                {
                    targets: 3,
                    render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateTimeDisplayFormat)
                }
            ]
        };

        if (params.request && params.request()) {
            self.show(params.request());
        }

        self.request.subscribe(function (val) {
            self.show(val);
        });
    }

    return vm;
});
