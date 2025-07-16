define([
        'services/screen/date-service',
        'services/person-data-service',
        'plugins/router',
        'modules/shared-filters',
        'modules/common',
        'services/util',
        'services/screen/message-service',
        'services/institution-data-service',
    ],
    function (dateService, personDataService, router, sharedFilters, common, util, message, institutionService) {

        var serverModel = {
            EntityId: ko.observable(),
            Url: ko.observable().extend({ required: true }),
            IncludeInPd: ko.observable(),
        }

        var vm = {
            canActivate: canActivate,
            activate: activate,
            website: serverModel,
            validateInGoogle: ko.observable(true),
            save: save,
            tryClose: tryClose,
            windowTitle: ko.observable('Edit Website'),
            suburb: ko.observable(),
            editMode: ko.observable(),
            IsOrganisation: ko.observable(),
            gotoWebsite: gotoWebsite
        };
        vm.setIsOrganisationFlag = function (flag) {
            vm.IsOrganisation(flag);
        };

        vm.dirtyFlag = new ko.DirtyFlag([vm.website], false);

        vm.canSave = ko.computed(function() {
            return vm.dirtyFlag().isDirty();
        });

        var validation = ko.validatedObservable(vm.website);
        var httpRegex = new RegExp("^(http|https)://", "i");

        vm.formattedUrl = ko.computed(function formatUrl() {
            if (!vm.website.Url()) {
                return '';
            }

            if (!httpRegex.test(vm.website.Url())) {
                return 'http://' + vm.website.Url();
            }
            return vm.website.Url();
        });

        function canActivate(entityId) {
            return loadWebsite(entityId);
        }

        function activate() {
            //if (vm.editMode()) {
                edit();
            //} else {
            //    create(entityId);
            //}
        }


        function clear() {
            util.resetModel(serverModel);
        }

        //function create(entityId) {
        //    clear();
        //    serverModel.EntityId(entityId);
        //    vm.dirtyFlag().reset();
        //    validation.errors.showAllMessages(false);
        //    return true;
        //}

        function loadWebsite(entityId) {
            return (vm.IsOrganisation() ? institutionService : personDataService).getFluid(entityId + '/website')
                .then(
                    function(data) {
                        clear();
                        ko.viewmodel.updateFromModel(vm.website, data);
                        vm.editMode(true);
                        return true;
                    },
                    function() {
                        return false;
                    });
        }

        function gotoWebsite() {
            window.open(vm.formattedUrl(), '_blank');
        }

        function edit() {
            vm.dirtyFlag().reset();
            validation.errors.showAllMessages(false);
        }        

        function save() {
            var isValid = validation.isValid();
            validation.errors.showAllMessages(!isValid);

            if (isValid) {
                var json = ko.toJS(vm.website);
                (vm.IsOrganisation() ? institutionService : personDataService).post(json, 'website')
                    .then(
                        function() {
                            toastr.success('Website saved');
                            close();
                        });
            }
        }

        function tryClose() {
            if (vm.dirtyFlag().isDirty()) {
                message.confirm({
                    title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                    content: ko.Localization('Naati.Resources.Shared.resources.AreYouSureYouWantToCancel')
                })
                    .then(
                        function (answer) {
                            if (answer === 'yes') {
                                close();
                            }
                        });
            } else {
                close();
            }
        }

        function close() {
            router.navigateBack();
        };
     
        return vm;
    });
