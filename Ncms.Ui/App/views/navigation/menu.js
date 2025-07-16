define([
    'services/screen/form-route-service',
    'modules/enums',
    'services/security-data-service'
], function (formRoute, enums, securityService) {
    var vm = {
        menus: ko.observableArray([]),
        showAllItems: ko.observable(),
        route: function (menu) {
            return menu.route || '';
        },
        navigate: function (menu) {
            if (!menu.route && menu.click) {
                menu.click();
                return false;
            }

            return true;
        },
        hasMenus: function (menu) {
            return menu.menus && menu.menus.length;
        },

        activate: function () {
            vm.showAllItems(false);

            var defer = Q.defer();
            securityService.getFluid('showAllMenuItems').then(function (data) {

                if (data) {
                    var allMenuItems = ko.utils.arrayFilter(formRoute.list(), function (formRoute) {
                        return formRoute.showInMenu;
                    });
                    vm.menus(allMenuItems);
                    defer.resolve(true);
                    return;
                }

                var menus = ko.utils.arrayFilter(formRoute.list(), function (formRouteList) {
                    if (!formRouteList.showInMenu) {
                        return false;
                    }
                    if (formRouteList.menus) {
                        var subMenus = ko.utils.arrayFilter(formRouteList.menus, function (subMenu) {

                            var nounPermissions = currentUser.Permissions.find(n => n.Noun === subMenu.secNoun);
                            return nounPermissions && ((BigInt(nounPermissions.Permissions) & BigInt(subMenu.secVerb)) == BigInt(subMenu.secVerb));
                        });

                        formRouteList.menus = subMenus.slice();
                        return subMenus.length;
                    }

                    var nounPermissions = currentUser.Permissions.find(n => n.Noun === formRouteList.secNoun);
                    return nounPermissions && ((BigInt(nounPermissions.Permissions) & BigInt(formRouteList.secVerb)) == BigInt(formRouteList.secVerb));
                });

                vm.menus(menus);
                defer.resolve(true);
            });

            return defer.promise;
        },
        applyScroll: function () {
            OverlayScrollbars($('#aside')[0], {});
        },

    };

    return vm;
});
