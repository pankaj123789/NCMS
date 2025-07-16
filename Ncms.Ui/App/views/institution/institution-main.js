define([
    'services/institution-data-service',
    'services/screen/date-service',
    'services/screen/message-service',
    'services/naatientity-data-service',
    'views/naati-entity/person-application',
    'views/naati-entity/contact-details',
    'views/naati-entity/transaction',
    'views/document/document-form',
    'views/shared/note',
    'plugins/router',
    'modules/shared-filters',
    'modules/enums',
    'views/institution/institution-contact-person',
    'views/naati-entity/address-edit',
    'views/naati-entity/phone-edit',
    'views/naati-entity/email-edit',
    'components/views/tab-lazy-load-controller',
    'views/naati-entity/website-edit',
],
    function (institutionService,
        dateService, message,
        naatientityDataService,
        personApplication,
        contactDetails,
        transaction,
        documentForm,
        notes,
        router,
        sharedFilters,
        enums, contactPerson, addressInstance, phoneInstance, emailInstance,
        tabController, websiteInstance) {

        var serverModel = {
            EntityId: ko.observable(),
            EntityTypeId: ko.observable(),
            Title: ko.observable(),
            IsEportalActive: ko.observable(),
            WebAccountCreateDate: ko.observable(),
            ArchiveHistory: ko.observableArray(),
            NaatiNumber: ko.observable(),
            AbbreviatedName: ko.observable(),
            Name: ko.observable()
        };

        var cleanModel = ko.toJS(serverModel);

        var validation = ko.validatedObservable(serverModel);
        var contactDetailsInstance = contactDetails.getInstance();
        var transactionInstance = transaction.getInstance();
        var notesInstance = notes.getInstance();
        var contactPersonInstance = contactPerson.getInstance();

        var vm = {
            canActivate: canActivate,
            naatiNumber: ko.observable(),
            entityId: ko.observable(),
            institution: serverModel,
            dirtyFlag: new ko.DirtyFlag([serverModel], false),
            isInstitution: ko.observable(false),
            thumbStyle: ko.observable(),
            institutionId: ko.observable(),
            contactDetails: ko.observable()
        };

        $.extend(notesInstance, {
            getNotesPromise: function () {
                return naatientityDataService.getFluid(vm.entityId() + '/notes');
            },
            postNotesPromise: function () {
                return naatientityDataService.post($.extend(notesInstance.parseNote(), { NaatiEntityId: vm.entityId() }), 'notes');
            },
            removeNotesPromise: function (note) {
                return naatientityDataService.remove('notes/{0}/{1}'.format(vm.entityId(), note.NoteId));
            }
        });

        var personApplicationInstance = personApplication.getInstance({
            isPerson: !vm.isInstitution,
            naatiNumber: vm.naatiNumber
        });

        vm.isInstitution.subscribe(processRequest);

        var compositionComplete = false,
            queryString = null;

        vm.activate = function (naatiNumber, query) {
            notesInstance.allowAttachments(true);
            queryString = query;
            if (compositionComplete) {
                processRequest();
            }
        };

        vm.compositionComplete = function () {
            compositionComplete = true;
            processRequest();
        };

        var activeTab = ko.observable();
        var tabs = tabController.generateTabs({
            tabs: [
                {
                    active: true,
                    id: 'contactPersons',
                    roles: [{ noun: enums.SecNoun.Organisation, verb: enums.SecVerb.Read }],
                    name: 'Naati.Resources.Institution.resources.ContactPersons',
                    type: 'compose',
                    composition: {
                        model: contactPersonInstance,
                        view: 'views/institution/institution-contact-person'
                    },
                    click: onTabClick
                },
                {
                    id: 'contactDetails',
                    name: 'Naati.Resources.Person.resources.ContactDetails',
                    roles: [{ noun: enums.SecNoun.Organisation, verb: enums.SecVerb.Read }],
                    type: 'compose',
                    composition: {
                        model: contactDetailsInstance,
                        view: 'views/naati-entity/contact-details'
                    },
                    visible: true,
                    click: onTabClick
                },
                {
                    id: 'applications',
                    name: 'Naati.Resources.Person.resources.Applications',
                    roles: [{ noun: enums.SecNoun.Application, verb: enums.SecVerb.Read }],
                    type: 'compose',
                    composition: {
                        model: personApplicationInstance,
                        view: 'views/naati-entity/person-application'
                    },
                    click: onTabClick
                },
                {
                    id: 'transactions',
                    name: 'Naati.Resources.Person.resources.Transactions',
                    roles: [{ noun: enums.SecNoun.Invoice, verb: enums.SecVerb.Read }],
                    type: 'compose',
                    composition: {
                        model: transactionInstance,
                        view: 'views/naati-entity/transaction'
                    },
                    click: onTabClick
                },
                {
                    id: 'notes',
                    name: 'Naati.Resources.Test.resources.Notes',
                    roles: [{ noun: enums.SecNoun.Notes, verb: enums.SecVerb.Read }],
                    type: 'compose',
                    composition: {
                        model: notesInstance,
                        view: 'views/shared/note.html'
                    },
                    click: onTabClick
                }
            ],
            activeTab: activeTab
        });

        function onTabClick(viewModel, ev) {
            var $tab = $(ev.target);
            if (!$tab.is('a')) return;

            var tabId = $tab.attr('href').substr(1);

            var tab = ko.utils.arrayFirst(tabs(), function (t) {
                return t.id == tabId;
            });

            if (tab == null) {
                return;
            }

            var currentUrl = window.location;
            var baseUrl = currentUrl.protocol + '//' + currentUrl.host + '/' + currentUrl.pathname.split('/')[1];
            var url = baseUrl + '#organisation/' + vm.naatiNumber() + '?tab=' + tabId;
            window.history.replaceState(null, document.title, url);

            function displaySkarline() {
                if ($('#' + tabId).is(':visible')) {
                    return $.sparkline_display_visible();
                }
                setTimeout(displaySkarline, 100);
            }

            displaySkarline();
        }

        function processRequest() {
            if (!queryString) {
                return;
            }

            var tabId = queryString['tab'];
            if (!tabId) {
                return;
            }

            activeTab(tabId);
        }

        function canActivate(naatiNumber) {
            vm.naatiNumber(naatiNumber);

            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }

            return loadEntity(naatiNumber);
        }

        extendVm();

        function extendVm() {
            var personDetailsHeadingFormat = ko.Localization('Naati.Resources.Test.resources.PersonDetailsHeadingFormat');
            var naatiEntityDetailsHeadingFormat = ko.Localization('Naati.Resources.Test.resources.NaatiEntityDetailsHeadingFormat');

            $.extend(vm,
                {
                    windowTitle: ko.pureComputed(
                        function () {
                            if (vm.isInstitution()) {
                                return personDetailsHeadingFormat
                                    .format(vm.institution.NaatiNumber(),
                                        vm.institution.Name());
                            }

                            return naatiEntityDetailsHeadingFormat
                                .format(vm.institution.NaatiNumber(),
                                    vm.institution.Name());
                        }),

                    tabOptions: {
                        tabContainerId: 'institutionEdit',
                        id: 'institutionEdit',
                        event: {
                            'shown.bs.tab': onTabClick
                        },
                        tabs: tabs
                    },
                    auditHistoryUrl: ko.pureComputed(function () {
                        if (vm.institution.EntityId()) {
                            return '#organisation/' +
                                vm.institution.EntityId() +
                                '-' +
                                vm.institution.NaatiNumber() +
                                '-' +
                                vm.institutionId() +
                                '/audithistory';
                        }
                        return '';
                    }),
                    getUrl: function (url) {
                        if (!vm.institution.NaatiNumber()) {
                            return '';
                        }

                        return url.format(vm.institution.NaatiNumber());
                    }
                });
        }

        function loadEntity(naatiNumber) {
            return institutionService.getFluid(naatiNumber).then(function (entity) {
                vm.entityId(entity.EntityId);
                vm.institutionId(entity.InstitutionId);
                vm.isInstitution(true);
                vm.contactDetails(entity.ContactDetails);
                if (entity.InstitutionId) {
                    transactionInstance.load(naatiNumber);
                    personApplicationInstance.load(naatiNumber);
                    contactDetailsInstance.populate(vm.entityId(), entity.ContactDetails, true);
                    addressInstance.setIsOrganisationFlag(true);
                    phoneInstance.setIsOrganisationFlag(true);
                    emailInstance.setIsOrganisationFlag(true);
                    websiteInstance.setIsOrganisationFlag(true);
                    contactPersonInstance.load(entity.InstitutionId, entity.EntityId);
                } 

                vm.naatiNumber(naatiNumber);
                notesInstance.getNotes();

                ko.viewmodel.updateFromModel(vm.institution, $.extend(cleanModel, entity));
                return true;
            });
        }
        return vm;
    });