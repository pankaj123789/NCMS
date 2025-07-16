define([
    'durandal/system',
    'plugins/router',
    'durandal/app',
    'services/security-data-service',
    'services/system-data-service',
    'services/screen/form-route-service',
    'services/messenger-service',
    'modules/enums'
], function (system, router, app, securityService, systemService, formRoute, messenger, enums) {
    moment.locale("en-au");
    $.datepicker.setDefaults({
        dateFormat: 'dd/mm/yy',
        altFormat: 'dd/mm/yy',
        firstDay: 1
    });

    $.fn.dataTable.render.nullableMoment = function (data, type, row) {
        if (data) {
            return moment(data, moment.ISO_8601).format(CONST.settings.shortDateDisplayFormat);
        }

        return '';
    };

    $.fn.dataTable.render.nullableDate = function (format) {
        return function (data, type, full) {
            if (!data) {
                return ''
            }

            if (type == 'display')
                return moment(data, moment.ISO_8601).format(format);
            else
                return data;
        };
    };

    if (!String.prototype.format) {
        String.prototype.format = function () {
            var args = arguments;
            return this.replace(/{(\d+)}/g, function (match, number) {
                return typeof args[number] != 'undefined'
                    ? (args[number] === 0 ? '0' : args[number]) || ''
                    : ''
                    ;
            });
        };
    }

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

    $(document).on('show.bs.dropdown hidden.bs.dropdown', '.dropdown', function () {
        $('body').toggleClass('dropdown-open');
    });

    var $window = $(window);
    $window.resize(function () {
        vm.currentWidth($window.width());
    });

    /*jshint -W020 */
    urlEncodingsDeferred = Q.defer();
    urlEncodings = urlEncodingsDeferred.promise;
    /*jshint +W020 */
    var title = ko.observable('');
    var titleSmall = ko.observable('');

    var vm = {
        hideHeader: ko.observable(false),
        hideMenu: ko.observable(false),
        user: ko.observable({ Name: "..." }),
        router: router,
        currentWidth: ko.observable($window.width()),
        title: title,
        titleSmall: titleSmall,
        quickSearch: ko.observable(''),
        titleWithSmall: ko.computed(function () {
            return (title() || '') + " <small>" + (titleSmall() || '') + "</small>";
        }, this),
        productName: ko.observable(),
        databaseName: ko.observable(),
        environment: ko.observable(),
        activate: activate,
        search: search,
        overridePermissions: ko.observable(),
        notifications: messenger.messages,
        compositionComplete: function () {
        }
    };

    vm.notificationsBadge = ko.computed(function () {
        return vm.notifications().length > 9 ? '9+' : vm.notifications().length;
    });

    vm.notificationClick = function (notification) {
        messenger.click(notification);
    };

    vm.notificationDeleteClick = function (notification) {
        event.preventDefault();
        event.stopPropagation();
        messenger.deleteClick(notification);
        return false;
    };

    return vm;

    function activate() {
        var defer = Q.defer();

        securityService.getFluid('ShowAllMenuItems').then(function (data) {
            vm.overridePermissions(data);
            var result = boot();
            defer.resolve(result);

        });

        return defer.promise;
    }

    function boot() {
        messenger.connect();

        // set a currentUser global for, you know, knowing who the current user is and stuff.
        currentUser = securityService.get();
        currentUser.hasPermissionSync = userHasPermissionSync;
        currentUser.hasPermission = userHasPermission;


        systemService.getFluid('basicinfo')
            .then(function (data) {
                vm.environment(data.Environment.toUpperCase().startsWith('PROD') ? '' : data.Environment);
                vm.productName(data.Name + ' ' + data.Version.split('.', 3).join('.'));
                if (data.Debug) {
                    vm.databaseName(data["Database name"]);
                }
            });

        router.guardRoute = function (instance, instruction) {
            if (instruction.config.name) {
                return userHasPermission(instruction.config.secNoun, instruction.config.secVerb)
                    .then(
                        function (data) {
                            if (!data) {
                                toastr.error(ko
                                    .Localization('Naati.Resources.SystemResources.resources.ScreenAccessDenied'));
                                return false;
                            }
                            return true;
                        },
                        function () {
                            toastr.error(ko.Localization('Naati.Resources.SystemResources.resources.GeneralError'));
                            return false;
                        });
            }

            return true;
        };

        router.on('router:navigation:complete',
            function (instance, instruction, router) {
                vm.title(instruction.config.title);
                vm.titleSmall(instruction.config.titleSmall);
                vm.title = vm.title;
            });

        // note: order is important here for default routes (we keep going until we find a route the user can access)
        var routes = [];
        mapRoutes(routes, formRoute.list());

        var routerActivationPromise = router
            .map(routes)
            .buildNavigationModel();

        if (!currentUser) {
            window.location.href = "/Errors/AccessDenied";
            return;
        }

        vm.user(currentUser);

        router.activate();

        if (!router.activeInstruction()) {
            router.navigate('home');
        }

        var bootPromise = Q.all([routerActivationPromise]);

        return bootPromise;
    }

    function userHasPermission(noun, verb) {
        if ($.isArray(noun)) {
            if (verb) {
                throw new 'To validate many permission you have to use the first argument as Array({ noun, verb }).';
            }

            var permissions = noun;
            for (var i = 0; i < permissions.length; i++) {
                var permission = permissions[i];
                if (validateUserPermission(permission.noun, permission.verb)) {
                    return Promise.resolve(true);
                }
            }
            //got to here. Must be false
            return Promise.resolve(false);
        }

        return Promise.resolve(validateUserPermission(noun, verb));
    }

    function userHasPermissionSync(noun, verb) {
        if ($.isArray(noun)) {
            if (verb) {
                throw new 'To validate many permission you have to use the first argument as Array({ noun, verb }).';
            }

            var permissions = noun;
            for (var i = 0; i < permissions.length; i++) {
                var permission = permissions[i];
                if (validateUserPermission(permission.noun, permission.verb)) {
                    return true;
                }
            }
            //got to here. Must be false
            return false;
        }

        return validateUserPermission(noun, verb);
    }

    function validateUserPermission(noun, verb) {
        var verbVal = verb;
        var nounVal = noun;



        try {
            if (vm.overridePermissions()) {
                return true;
            }

            if (!$.isNumeric(verb)) {
                verbVal = enums.SecVerb[verb];
            }

            if (verbVal === undefined) {
                throw 'Invalid security verb: ' + verb;
            }

            if (!$.isNumeric(nounVal)) {
                nounVal = enums.SecNoun[noun];
            }

            if (nounVal === undefined) {
                throw 'Invalid security noun: ' + noun;
            }

            var nounPermissions = currentUser.Permissions.find(n => n.Noun === nounVal);
            return (nounPermissions && ((BigInt(nounPermissions.Permissions) & BigInt(verbVal)) == BigInt(verbVal))) || false;
        } catch (e) {
            console.error(e);
            return false;
        }
    }

    function mapRoutes(routes, menus) {
        for (var i = 0; i < menus.length; i++) {
            var m = menus[i];

            if (m.menus) {
                mapRoutes(routes, m.menus);
            }

            if (!m.moduleId && !m.route)
                continue;

            routes.push(m);
        }
    }

    function search() {
        if (Boolean(this.quickSearch())) {
            router.navigate('person/' + this.quickSearch());
        }

        this.quickSearch('');
    }

});

