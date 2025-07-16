define(['modules/shared-filters', 'services/usersearch-data-service', 'services/screen/message-service'], function (sharedFilters, usersearchService, messageService) {
    function getFilter(filterList, filterDefinition) {
        return ko.utils.arrayFirst(filterList, function (filter) {
            return filter.id === filterDefinition.id;
        });
    }

    function TrySaveSearchMessageConfig(searchName) {
        return {
            title: searchName,
            content: ko.Localization('Naati.Resources.Test.resources.SaveCurrentOrSaveNewOne').replace('{0}', searchName),
            yes: ko.Localization('Naati.Resources.Shared.resources.SaveCurrent'),
            no: ko.Localization('Naati.Resources.Shared.resources.NewOne')
        };
    }

    function ViewModel(params) {
        var self = this;

        function removeFilter(filterDefinition) {
            var filter = getFilter(self.currentFilters(), filterDefinition);

            if (filter) {
                self.currentFilters.remove(filter);
                self.controller.availableFiltersViewModel.add(filter.id);

                if (!self.currentFilters().length) {
                    self.clear();
                }
            }
        };

        function addFilter(filterDefinition, data) {
            var availableFilter = getFilter(self.availableFilters(), filterDefinition);

            if (!availableFilter) {
                return false;
            }

            var filter = $.extend(true, {}, availableFilter);

            filter.componentOptions.controller = self.controller;

            if (data) {
                $.extend(true, filter.componentOptions, data);
            }

            self.currentFilters.push(filter);

            return true;
        };

        var defaultParams = {
            title: null,
            filters: [],
            searchType: null,
            showOutOfDateAlert: ko.observable(false),
            searchTerm: ko.observable(),
            controller: {
                select: addFilter,
                remove: removeFilter
            },
            additionalButtons: [],
            resultTemplate: null,
            idSearchOptions: null
        };

        $.extend(true, defaultParams, params); // Perform deep extend in case params already has a controller object

        self.title = defaultParams.title;
        self.searchType = defaultParams.searchType;
        self.searchTerm = defaultParams.searchTerm;
        self.showOutOfDateAlert = defaultParams.showOutOfDateAlert;
        self.searchCallback = defaultParams.searchCallback;
        self.savedSearches = ko.observableArray();
        self.currentSearch = ko.observable(null);
        self.currentSearchId = ko.observable(null);
        self.newSearchName = ko.observable(null).extend({
            required: true, maxLength: 50,
            validation: {
                validator: function (val, someOtherVal) {
                    var grepped = $.grep(self.userSearches(), function (e, i) {
                        return e.SearchName == val;
                    });
                    return grepped.length == 0;
                },
                message: ko.Localization('Naati.Resources.Shared.resources.DuplicatedSearchName')
            }
        });

        self.idSearchOptions = defaultParams.idSearchOptions;
        self.idSearchValue = ko.observable();

        var searchValidation = ko.validatedObservable({ name: self.newSearchName });

        self.availableFilters = ko.observableArray(ko.utils.arrayMap(defaultParams.filters, function (filter) {
            return $.extend(true, {}, getFilter(sharedFilters.filterDefinitions(), filter), filter);
        }));

        self.currentFilters = ko.observableArray();
        self.controller = defaultParams.controller;
        self.additionalButtons = defaultParams.additionalButtons;
        self.resultTemplate = defaultParams.resultTemplate;

        self.availableFiltersComponent = {
            name: 'available-filters',
            params: {
                filters: self.availableFilters,
                controller: self.controller
            }
        };

        self.requiredFilters = ko.pureComputed(function () {
            return ko.utils.arrayFilter(self.availableFilters(), function (availableFilter) {
                return availableFilter.required;
            });
        });

        self.userCanSearch = ko.pureComputed(function () {
            var requiredFilters = self.requiredFilters();

            return ko.utils.arrayFilter(requiredFilters, function (requiredFilter) {
                return ko.utils.arrayFirst(self.currentFilters(), function (currentFilter) {
                    return currentFilter.id === requiredFilter.id;
                });
            }).length === requiredFilters.length;
        });

        self.userCanSaveSearch = ko.pureComputed(function () {
            return self.currentFilters().length > 0 && self.userCanSearch();
        });

        self.showButtons = ko.pureComputed(function () {
            return self.currentFilters().length > 0 || self.userCanSearch();
        });

        self.userSearches = ko.pureComputed(function () {
            return ko.utils.arrayFilter(self.savedSearches(), function (search) {
                return !self.currentSearch() || search.SearchId !== self.currentSearch().SearchId;
            });
        });

        self.loadUserSearches = function () {
            return usersearchService.get({ type: self.searchType }).then(function (data) {
                ko.utils.arrayForEach(data, function (filter) {
                    filter.Filters = ko.pureComputed(function () {
                        var filters = [];
                        var jsonFilters = ko.utils.parseJson(filter.CriteriaJson);
                        ko.utils.arrayForEach(jsonFilters, function (filterJson) {
                            var filter = getFilter(self.availableFilters(), { id: filterJson.filterId });
                            filters.push(ko.Localization(filter.name));
                        });
                        return filters;
                    });
                });
                self.savedSearches(data);
            });
        };

        self.find = function () {
            if (!self.validateFilters()) return;

            var jsonFilters = self.getJsonFilters();

            var formattedJsonFilters = ko.utils.arrayMap(jsonFilters, function (jsonFilter) {
                var filterId = jsonFilter.filterId;
                var filter = getFilter(self.availableFilters(), { id: filterId });

                return {
                    Id: filterId,
                    Data: filter.formatData ? filter.formatData(jsonFilter.filterData) : jsonFilter.filterData
                };
            });

            var searchTerm = {};

            ko.utils.arrayForEach(formattedJsonFilters, function (jsonFilter) {
                searchTerm[jsonFilter.Id] = { Data: jsonFilter.Data };
            });

            self.searchTerm(searchTerm);

            if (self.searchCallback && typeof self.searchCallback === 'function') {
                self.searchCallback(self.searchTerm());
            }
        }

        self.clear = function () {
            self.searchTerm({});
            self.currentSearch(null);
            self.currentFilters([]);
            self.controller.availableFiltersViewModel.clear();
        }

        self.tryLoadSearch = function (search) {
            if (self.currentSearch() || self.currentFilters().length > 0) {
                messageService.confirm({ title: ko.Localization('Naati.Resources.Test.resources.LoadSearch'), content: ko.Localization('Naati.Resources.Test.resources.LoadSearchConfirm') }).then(
                    function (answer) {
                        if (answer === 'yes') {
                            self.loadSearch(search);
                        }
                    });
            }
            else {
                self.loadSearch(search);
            }
        }

        self.checkEnterPressed = function (data, e) {
            var keyCode = e.which || e.keyCode;
            if (keyCode == 13) {
                self.gotoPerson();
                return false;
            }
            return true;
        };

        // Parse and replace any date values with formats accepted by the filter
        function parseDates(obj) {
            for (var node in obj) {
                if (typeof obj[node] == "object" && obj[node] !== null) {
                    parseDates(obj[node]);
                } else if (isDate(obj[node])) {
                    obj[node] = moment(obj[node]).format('DD/MM/YYYY');
                }
            }
        }

        function isDate(value) {
            var reISO = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)(?:Z|(\+|-)([\d|:]*))?$/;
            if (typeof value === 'string') {
                return reISO.exec(value);
            }
            return false;
        }

        self.loadSearch = function (search) {
            self.currentSearch(search);

            var jsonFilters = ko.utils.parseJson(self.currentSearch().CriteriaJson);

            parseDates(jsonFilters);

            ko.utils.arrayForEach(jsonFilters, function (filterJson) {
                self.controller.availableFiltersViewModel.select({ id: filterJson.filterId }, filterJson.filterData);
            });
        }

        self.tryRemoveSearch = function (search) {
            messageService.confirm({ title: ko.Localization('Naati.Resources.Shared.resources.AreYouSure'), content: ko.Localization('Naati.Resources.Shared.resources.RemoveSearchConfirmation') }).then(
                function (answer) {
                    if (answer == 'yes') {
                        self.removeSearch(search);
                    }
                });
        }

        self.removeSearch = function (search) {
            usersearchService.remove(search.SearchId).then(function () {
                if (self.currentSearch() && search.SearchId === self.currentSearch().SearchId) {
                    self.clear();
                }

                self.loadUserSearches();
            });
        }

        self.trySaveSearch = function () {
            searchValidation.errors.showAllMessages(false);

            if (self.currentSearch()) {
                var config = TrySaveSearchMessageConfig(self.currentSearch().SearchName);

                messageService.confirm(config).then(
                    function (answer) {
                        if (answer === 'yes') {
                            self.currentSearchId(self.currentSearch().SearchId);
                            self.newSearchName(self.currentSearch().SearchName);

                            self.saveSearch();
                        }
                        else {
                            $('#saveSearchModal').modal('show');
                        }
                    });
            }
            else {
                $('#saveSearchModal').modal('show');
            }
        }

        // Used to handle form submission, for when the user presses the enter key on the keyboard, instead of clicking the save button
        self.saveSearchSubmit = function () {
            self.saveSearch();

            return false;
        }

        self.saveSearch = function () {
            var isValid = searchValidation.isValid();
            searchValidation.errors.showAllMessages(!isValid);

            if (!isValid) return;

            var jsonFilters = self.getJsonFilters();

            var searchData = {
                SearchId: self.currentSearchId(),
                SearchName: self.newSearchName(),
                SearchType: self.searchType,
                CriteriaJson: JSON.stringify(jsonFilters)
            };

            usersearchService.post(searchData).then(function (savedSearch) {
                if (savedSearch) {
                    self.currentSearch(savedSearch);
                }

                self.loadUserSearches();
            });

            self.currentSearchId(null);
            self.newSearchName(null);

            $('#saveSearchModal').modal('hide');
        }

        // Validate the filters, for saving and finding
        self.validateFilters = function () {
            var valid = true;
            var filters = self.currentFilters();

            for (var i = 0; i < filters.length; i++) {
                var filter = filters[i];
                valid = valid && filter.componentOptions.controller.validateFilter[filter.id]();
            }

            return valid;
        };

        self.getJsonFilters = function () {
            var jsonFilters = [];
            var filters = self.currentFilters();

            for (var i = 0; i < filters.length; i++) {
                var filter = filters[i];
                jsonFilters.push({
                    filterId: filter.id,
                    filterData: filter.componentOptions.controller.getFilterJson[filter.id]()
                });
            }

            return jsonFilters;
        };

        self.checkEnterPressed = function (data, e) {
            var keyCode = e.which || e.keyCode;
            if (keyCode == 13) {
                self.idSearchClick();
                return false;
            }
            return true;
        };

        self.idSearchClick = function () {
            self.idSearchOptions.search(self.idSearchValue());
        };

        // Initial loading of the users saved search list
        self.loadUserSearches().then(function () {
            if (self.savedSearches().length === 0) {
                // Load any required filters by default, keep trying until the bindings resolve
                var interval = setInterval(function () {
                    if (self.controller.availableFiltersViewModel) {
                        clearInterval(interval);

                        ko.utils.arrayForEach(self.requiredFilters(), function (requiredFilter) {
                            self.controller.availableFiltersViewModel.select(requiredFilter);
                        });
                    }
                }, 10);
            }
        });
    };

    return ViewModel;
});
