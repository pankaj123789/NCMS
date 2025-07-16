define([
    'services/util'
], function (util) {
    var BaseTemplate = '\
<div class="modal fade" tabindex="-1" role="dialog" data-backdrop="static">\
    <div class="modal-dialog {CSS}">\
        <div class="modal-content">\
                <div class="modal-header">\
                    <h4 class="modal-title">{TITLE}</h4>\
                </div>\
                <div class="modal-body"></div>\
                <div class="modal-footer">\
                </div>\
        </div><!-- /.modal-content -->\
    </div><!-- /.modal-dialog -->\
</div>';

    return {
        remove: remove,
        custom: custom,
        confirm: confirm,
        alert: alert
    };

    function remove(settings) {
        var options = {
            title: ko.Localization('Naati.Resources.Shared.resources.AreYouSure'),
            content: ko.Localization('Naati.Resources.Shared.resources.ThisRecordWillBeDeleted'),
            yes: '<span class="glyphicon glyphicon-trash"></span><span>' +
                (settings
                    ? settings.yesText
                    ? settings.yesText
                    : ko.Localization('Naati.Resources.Shared.resources.Delete')
                    : ko.Localization('Naati.Resources.Shared.resources.Delete')) +
                '</span>',
            no: ko.Localization('Naati.Resources.Shared.resources.Cancel')
        };

        $.extend(options, settings);

        var buttons = {
            buttons: [
                {
                    text: options.no,
                    className: 'btn btn-default',
                    argument: 'no'
                },
                {
                    text: options.yes,
                    className: 'btn btn-danger',
                    argument: 'yes'
                }
            ]
        };

        $.extend(buttons, options);

        return custom(buttons);
    }

    function confirm(settings) {
        var options = {
            title: '',
            content: '',
            yes: ko.Localization('Naati.Resources.Shared.resources.Yes'),
            no: ko.Localization('Naati.Resources.Shared.resources.No')
        };

        $.extend(options, settings);

        var buttons = {
            buttons: [
                {
                    text: options.yes,
                    className: 'btn btn-primary',
                    argument: 'yes'
                },
                {
                    text: options.no,
                    className: 'btn btn-default',
                    argument: 'no'
                }
            ]
        };

        $.extend(buttons, options);

        return custom(buttons);
    }

    function alert(settings) {
        var options = {
            title: '',
            content: '',
        };

        var buttons = {
            buttons: [
                {
                    text: 'OK',
                    className: 'btn btn-primary',
                    argument: 'ok'
                }
            ]
        };

        $.extend(buttons, options, settings);

        return custom(buttons);
    }

    function custom(settings) {
        var options = {
            css: '',
            title: '',
            content: '',
            buttons: [],
            template: null,
            data: null
        };

        $.extend(options, settings);

        var defer = Q.defer();

        var tmpl = BaseTemplate
            .replace('{CSS}', options.css)
            .replace('{TITLE}', options.title);

        var $tmpl = $(tmpl);

        $tmpl
            .appendTo('body')
            .on('hidden.bs.modal', function (e) {
                $(this).remove();
            })
            .modal('show');

        if (options.content) {
            $('.modal-body', $tmpl).html(options.content);
        }
        else if (options.template) {
            ko.applyBindingsToNode($('.modal-body', $tmpl)[0], {
                template: options.template
            });
        }


        for (var i = 0; i < options.buttons.length; i++) {
            var button = options.buttons[i];
            var $button = $('<button type="button" class="' + button.className + '" data-argument="' + button.argument + '">' + button.text + '</button>');

            if (button.click) {
				(function (button) {
					$button.click(function () {
						if (button.click() !== false) {
							$tmpl.modal('hide');
						}
					});
				})(button);
            }
            else {
                $button.click(function () {
                    defer.resolve($(this).data('argument'));
                    $tmpl.modal('hide');
                });
            }
            $('.modal-footer', $tmpl).append($button);
        }

        if (!options.title) {
            $('.modal-header', $tmpl).hide();
        }

        return defer.promise;
    }
});
