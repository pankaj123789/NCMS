// Maps the files so Durandal knows where to find these.
require.config({
	urlArgs: 'v=' + samVersion,
	baseUrl: window.baseUrl + 'App',
    paths: {
        'text': window.baseUrl + 'Scripts/text',
		'durandal': window.baseUrl + 'Scripts/durandal',
		'plugins': window.baseUrl + 'Scripts/durandal/plugins',
		'transitions': window.baseUrl + 'Scripts/durandal/transitions',
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
    'plugins/dialog'], boot);
/*jshint -W020 */
durandalRouter = '';
durandalApp = '';
/*jshint +W020 */
function boot(app, viewLocator, system, router, dialog) {
    /*jshint -W020 */
    durandalRouter = router;
    durandalApp = app;
    /*jshint +W020 */
    dialog.MessageBox.setViewUrl('views/shared/confirmation-prompt');

    app.showModal = function (obj, activationData, context) {
        return dialog.show(obj, activationData, context);
    };
    app.showMessage = function (message, title, options) {
        return dialog.show(new dialog.MessageBox(message, title, options));
    };
    app.showYesNo = function (message, title) {
        return dialog.show(new dialog.MessageBox(message, title, [
                    { value: 'Yes', text: ko.Localization('Yes') },
                    { value: 'No', text: ko.Localization('No') }
        ]));
    };

    // Enable debug message to show in the console 
    system.debug(true);

    app.configurePlugins({
        router: true,
        dialog: true
    });

    app.start().then(function () {
        ko.validation.init({
            errorElementClass: 'has-error',
            errorMessageClass: 'help-block',
            decorateInputElement: true,
            grouping: {
                deep: true,
                observable: true
            }
        }, true);

        // When finding a viewmodel module, replace the viewmodel string 
        // with view to find it partner view.
        // [viewmodel]s/sessions --> [view]s/sessions.html
        // Defaults to viewmodels/views/views. 
        // Otherwise you can pass paths for modules, views, partials

        //viewLocator.useConvention = useConventionOveride;

        viewLocator.useConvention();

        //Show the app by setting the root view model for our application.
        app.setRoot('views/shell');
    });

    function useConventionOveride(modulesPath, viewsPath, areasPath) {

        this.convertModuleIdToViewId = function (moduleId) {
            return moduleId;
        };

        this.translateViewIdToArea = function (viewId, area) {
            return viewId;
        };
    }
}
