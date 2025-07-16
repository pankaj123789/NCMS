define([
    'durandal/app',
    'plugins/router'],
    function (app, router) {
        var routerActivationPromise = router
            .map([
            {
                name: 'logbookIndex',
                title: 'Logbook',
                route: 'dashboard',
                moduleId: 'views/logbook/logbook-dashboard'
            }, {
                name: 'professionalDevelopment',
                title: 'Professional Development',
                route: 'professional-development(/:certificationPeriodId)',
                moduleId: 'views/logbook/logbook-professional-development'
            }, {
                name: 'workPractice',
                title: 'Work Practice',
                route: 'credential/:credentialId(/:certificationPeriodId)',
                moduleId: 'views/logbook/logbook-credential'
            }, ])
            .buildNavigationModel()
            .activate();

        document.title = '';
        app.title = document.title;
        app.breadCrumbs = ko.observableArray();

        var vm = {
            app: app,
            showLoader: ko.observable(true),
            activate: function () {
                return routerActivationPromise;
            }
        };

        var loaderTimeout = null;
        router.on('router:navigation:processing', function () {
            loaderTimeout = setTimeout(function () {
                loaderTimeout = null;
                vm.showLoader(true);
            }, 400);
        });

        router.on('router:navigation:composition-complete', function () {
            if (loaderTimeout) {
                clearTimeout(loaderTimeout);
            }

            vm.showLoader(false);
        });

        vm.showLoader.subscribe(function (show) {
            var $logbookLoader = $("#logbookLoader");

            if (show) {
                return $logbookLoader.show();
            }

            $logbookLoader.hide();
        });

        if (!router.activeInstruction()) {
            router.navigate('dashboard');
        }

        return vm;
    }
);