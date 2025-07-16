define(['services/navigation-data-service', 'services/screen/form-route-service'], function (navigationService, formRoute) {
    var vm = {
        menus: ko.observableArray([]),
        searches: ko.observableArray([
            { link: '#', text: 'Tests sat today' },
            { link: '#', text: 'All marks received' },
        ]),
        favourites: ko.observableArray([]),
        recents: ko.observableArray([
            { id: 0, link: '#', text: '33455 Mary Klesons' },
            { id: 1, link: '#', text: '98874 Canberra City' },
            { id: 2, link: '#', text: '10455 Paraprofessional Person' },
        ]),
        quickSearch: ko.observable(),
        showAllItems: ko.observable(),
        css: css,
        hasColumn1Result: hasColumn1Result,
        hasColumn2Result: hasColumn2Result,
        hasResult: hasResult,
        toggleRibbons: toggleRibbons,
        activate: activate
    };

    vm.filteredMenus = filteredMenus;
    vm.filteredFavourites = filteredFavourites;
    vm.filteredRecents = filteredRecents;
    vm.filteredSearches = filteredSearches

    return vm;

    function activate(activationData) {
        vm.menuContext = activationData.menuContext;
        vm.quickSearch = activationData.quickSearch || ko.observable('');
        vm.quickSearch.subscribe(search);

        vm.showAllItems(false);

        navigationService.get().then(function (data) {
            var mapped = $.map(data, function (d, i) {
                var menu = formRoute.get(d);
                var result = $.extend({ Name: d }, menu);
                result.route = "#" + result.route;

                if (i % 2 == 0)
                    vm.favourites.push(result);

                return result;
            });

            vm.menus(mapped);
        });
    }

    function hasColumn1Result() {
        return filteredRecents().length > 0 || filteredSearches().length > 0;
    }

    function hasColumn2Result() {
        return filteredMenus().length > 0 || filteredFavourites().length > 0;
    }

    function hasResult() {
        return hasColumn1Result() || hasColumn2Result();
    }

    function css() {
        var baseCss = "dropdown-menu ";
        if (hasColumn1Result() && hasColumn2Result()) {
            return baseCss + "fullsize";
        }
        else if (hasColumn1Result()) {
            return baseCss + "only-first";
        }
        else if (hasColumn2Result()) {
            return baseCss + "only-second";
        }

        return baseCss + "only-second";
    }

    function search(newValue) {
        if (!newValue)
            return;

        $("#startMenu")
            .off('hide.bs.dropdown')
            .on('hide.bs.dropdown', function () {
                vm.quickSearch('');
            })
            .addClass('open');
    }

    function toggleRibbons() {
        vm.showAllItems(!vm.showAllItems());
    }

    function filteredMenus() {
        return filter(vm.menus(), 'title');
    }

    function filteredFavourites() {
        return filter(vm.favourites(), 'title');
    }

    function filteredRecents() {
        return filter(vm.recents(), 'text');
    }

    function filteredSearches() {
        console.log(vm.quickSearch().toLowerCase());
        return filter(vm.searches(), 'text');
    }

    function filter(list, property) {
        var items = $.grep(list, function (l) {
            return l[property].toLowerCase().indexOf(vm.quickSearch().toLowerCase()) > -1;
        });
        return items;
    }
});