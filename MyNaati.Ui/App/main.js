// Maps the files so Durandal knows where to find these.
require.config({
	urlArgs: "v=" + window.version,
    paths: {
        'text': '../Scripts/text',
        'durandal': '../Scripts/durandal',
        'plugins': '../Scripts/durandal/plugins',
        'transitions': '../Scripts/durandal/transitions',
        'main-dist': 'main'
    }
});

// Durandal 2.x assumes no global libraries. It will ship expecting 
// Knockout and jQuery to be defined with requirejs. .NET 
// templates by default will set them up as standard script
// libs and then register them with require as follows: 
define('jquery', function () { return jQuery; });
define('knockout', ko);

define([
    'durandal/app',
    'durandal/viewLocator',
    'durandal/system',
    'plugins/router',
    'services/message',
    'services/constant',
    'services/enums'], boot);
/*jshint -W020 */
durandalRouter = '';
durandalApp = '';
/*jshint +W020 */
function boot(app, viewLocator, system, router) {
    /*jshint -W020 */
    moment.locale("en-au");
    Date.prototype.toJSON = function () { return moment(this).format('YYYY-MM-DD[T]HH:mm:ss[Z]'); }
    window.baseUrl = $('script[data-main]').prop('src').replace('Scripts/require.js', '');
    durandalRouter = router;
    durandalApp = app;

    $(document)
        .on('show.bs.modal',
            '.modal',
            function () {
                var $self = $(this);
                var zIndex = 1040 + (10 * $('.modal:visible').length);
                $self.css('z-index', zIndex);
                setTimeout(function () {
                    $self.data('bs.modal').$backdrop.css('z-index', zIndex - 1);
                }, 0);
            });

    $(document).on('hidden.bs.modal', '.modal', function () {
        $('.modal:visible').length && $(document.body).addClass('modal-open');
    });
    /*jshint +W020 */

    // Enable debug message to show in the console 
    system.debug(true);

    app.configurePlugins({
        router: true,
    });

    $.datepicker.setDefaults({
        dateFormat: 'dd/mm/yy'
    });

    app.title = document.title;
    app.start().then(function () {
        ko.validation.init({
            errorElementClass: 'has-error',
            errorMessageClass: 'help-block',
            decorateInputElement: true
        }, true);

        // When finding a viewmodel module, replace the viewmodel string 
        // with view to find it partner view.
        // [viewmodel]s/sessions --> [view]s/sessions.html
        // Defaults to viewmodels/views/views. 
        // Otherwise you can pass paths for modules, views, partials
        viewLocator.useConvention();
        app.setRoot($('#applicationHost').attr('data-app-root'));
    });
}
