define([
    'services/screen/date-service',
    'services/screen/message-service',
    'services/person-data-service',
    'services/naatientity-data-service',
    'services/person-data-service',
    'views/naati-entity/person-application',
    'views/naati-entity/person-exemptions',
    'views/naati-entity/person-qrcodes',
    'views/naati-entity/person-credentials',
    'views/naati-entity/person-role-plays',
    'views/naati-entity/contact-details',
    'views/naati-entity/transaction',
    'views/naati-entity/change-photo',
    'views/naati-entity/extend-certifications',
    'views/naati-entity/person-correspondence',
    'views/document/document-form',
    'views/shared/note',
    'views/application/application-new',
    'views/naati-entity/credential-applications',
    'plugins/router',
    'modules/shared-filters',
    'modules/enums',
    'views/naati-entity/deregister-mynaati-account',
    'views/naati-entity/deregister-mfa-account',
    'components/views/tab-lazy-load-controller'
],
    function (
        dateService,
        message,
        personDataService,
        naatientityDataService,
        personService,
        personApplication,
        personExemptions,
        personQrCodes,
        personCredentialRequests,
        personRolePlays,
        contactDetails,
        transaction,
        changePhoto,
        extendCertifications,
        correspondence,
        documentForm,
        notes,
        applicationNew,
        credentialApplications,
        router,
        sharedFilters,
        enums,
        deregisterMyNaatiAccount,
        deregisterMfaAccount,
        tabController) {

        var serverModel = {
            EntityId: ko.observable(),
            EntityTypeId: ko.observable(),
            ExpiryDate: ko.observable(),
            BirthDate: ko.observable().extend({
                dateLessThan: moment().format('l'),
                dateGreaterThan: moment('1/1/1753').format('l'),
            }),
            CountryOfBirth: ko.observable(),
            Deceased: ko.observable(),
            DoNotSendCorrespondence: ko.observable(),
            SendCorrespondence: ko.observable(),
            Gender: ko.observable(),
            GenderText: ko.observable(),
            Title: ko.observable(),
            Name: ko.observable(),
            GivenName: ko.observable(),
            Surname: ko.observable(),
            HasPhoto: ko.observable(),
            NaatiNumber: ko.observable(),
            PractitionerNumber: ko.observable(),
            PhotoDate: ko.observable(),
            BirthCountryId: ko.observable(),
            BirthCountry: ko.observable(),
            PersonId: ko.observable(),
            IsEportalActive: ko.observable(),
            WebAccountCreateDate: ko.observable(),
            ArchiveHistory: ko.observableArray(),
            EthicalCompetency: ko.observable(),
            InterculturalCompetency: ko.observable(),
            KnowledgeTest: ko.observable(),
            IsFormerPractitioner: ko.observable(),
            IsPractitioner: ko.observable(),
            IsApplicant: ko.observable(),
            IsFuturePractitioner: ko.observable(),
            IsExaminer: ko.observable(),
            IsRolePlayer: ko.observable(),
            IsRolePlayerAvailable: ko.observable(),
            MaxCertificationPeriodEndDate: ko.observable(),
            Types: ko.observableArray(),
            PanelName: ko.observable(),
            MyNaatiRegistrationDate: ko.observable(),
            LastmyNaatiLogin: ko.observable(),
            LastPasswordChangeDate: ko.observable(),
            IsAccountLocked: ko.observable(),
            UserName: ko.observable(),
            RolePlayerPanelName: ko.observable(),
            IsMfaEnabled: ko.observable('Disabled'),
            HasMfaAccount: ko.observable(),
            MfaExpireStartDate: ko.observable(),
            DisabledByNaati: ko.observable()
        };

        var cleanModel = ko.toJS(serverModel);

        serverModel.Types = ko.pureComputed(function () {
            var types = [];

            if (serverModel.IsFormerPractitioner()) {
                types.push({
                    name: ko.Localization('Naati.Resources.PersonType.resources.' + enums.PersonType.FormerPractitioner)
                });
            }

            if (serverModel.IsPractitioner()) {
                types.push({
                    name: ko.Localization('Naati.Resources.PersonType.resources.' + enums.PersonType.Practitioner)
                });
            }

            if (serverModel.IsFuturePractitioner()) {
                types.push({
                    name: ko.Localization('Naati.Resources.PersonType.resources.' + enums.PersonType.FuturePractitioner)
                });
            }

            if (serverModel.IsApplicant()) {
                types.push({
                    name: ko.Localization('Naati.Resources.PersonType.resources.' + enums.PersonType.Applicant)
                });
            }

            if (serverModel.IsExaminer()) {
                types.push({
                    name: ko.Localization('Naati.Resources.PersonType.resources.' + enums.PersonType.Examiner),
                    tooltip: serverModel.PanelName()
                });
            }

            if (serverModel.IsRolePlayer()) {
                types.push({
                    name: ko.Localization('Naati.Resources.PersonType.resources.' + enums.PersonType.RolePlayer),
                    tooltip: serverModel.RolePlayerPanelName()
                });
            }

            return types;
        });

        serverModel.DisplayName = ko.pureComputed(function () {
            var title = serverModel.Title() != null ? serverModel.Title() + ' ' : '';
            return $.trim(title + serverModel.GivenName() + ' ' + serverModel.Surname());
        });

        var validation = ko.validatedObservable(serverModel);
        var contactDetailsInstance = contactDetails.getInstance();
        var transactionInstance = transaction.getInstance();
        var notesInstance = notes.getInstance();
        var applicationNewInstance = applicationNew.getInstance();
        var credentialApplicationsInstance = credentialApplications.getInstance();
        var changePhotoInstance = changePhoto.getInstance(serverModel);
        var extendCertificationsInstance = extendCertifications.getInstance(serverModel);
        var deregisterMyNaatiAccountInstance = deregisterMyNaatiAccount.getInstance(serverModel);
        var deregisterMfaAccountInstance = deregisterMfaAccount.getInstance(serverModel);
        var correspondenceInstance = correspondence.getInstance(serverModel);

        var vm = {
            canActivate: canActivate,
            naatiNumber: ko.observable(),
            entityId: ko.observable(),
            person: serverModel,
            dirtyFlag: new ko.DirtyFlag([serverModel], false),
            isPerson: ko.observable(false),
            isRolePlayer: ko.observable(false),
            isRolePlayerAvailable: ko.observable(false),
            periods: ko.observableArray(),
            hasMyNaatiAccount: ko.observable(false),
            hasMfaAccount: ko.observable(false),
            canUpdateCertificationPeriods: ko.observable(false),
            canUpdateCertification: ko.observable(false),
            email: ko.observable()

        };

        vm.myNaatiStatus = ko.computed(function () {
            if (!vm.hasMyNaatiAccount()) {
                return ko.Localization('Naati.Resources.Person.resources.MyNaatiAccountNotRegistered');
            }
            if (serverModel.IsAccountLocked()) {
                return ko.Localization('Naati.Resources.Person.resources.MyNaatiAccountLocked');
            }
            return ko.Localization('Naati.Resources.Person.resources.MyNaatiAccountRegistered');
        });

        changePhotoInstance.canUpload = function () {
            if (!vm.dirtyFlag().isDirty()) {
                return true;
            }

            message.confirm({
                title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
                content: ko.Localization('Naati.Resources.Shared.resources.SaveBeforeAction')
            }).then(function (result) {
                if (result === 'yes') {
                    changePhotoInstance.upload();
                }
            });

            return false;
        };

        $.extend(notesInstance, {
            getNotesPromise: function (showAll) {
                return naatientityDataService.getFluid(vm.entityId() + '/notes/' + showAll);
            },
            postNotesPromise: function () {
                return naatientityDataService.post($.extend(notesInstance.parseNote(), { NaatiEntityId: vm.entityId() }), 'notes');
            },
            removeNotesPromise: function (note) {
                return naatientityDataService.remove('notes/{0}/{1}'.format(vm.entityId(), note.NoteId));
            }
        });

        var personApplicationInstance = personApplication.getInstance({
            isPerson: vm.isPerson,
            naatiNumber: vm.naatiNumber
        });

        var personExemptionsInstance = personExemptions.getInstance({
            naatiNumber: vm.naatiNumber
        });

        var personQrCodesInstance = personQrCodes.getInstance({
            naatiNumber: vm.naatiNumber
        });

        var personCredentialRequestsInstance = personCredentialRequests.getInstance({
            isPerson: vm.isPerson,
            naatiNumber: vm.naatiNumber,
            credentialApplicationsInstance: credentialApplicationsInstance,
            person: serverModel
        });

        var personCredentialExemptionsInstance = personCredentialRequests.getInstance({
            isPerson: vm.isPerson,
            naatiNumber: vm.naatiNumber,
            credentialApplicationsInstance: credentialApplicationsInstance,
            person: serverModel
        });

        personCredentialRequestsInstance.reload.subscribe(function () {
            loadEntity(vm.naatiNumber());
        });

        var personRolePlaysInstance = personRolePlays.getInstance({
            isPerson: vm.isPerson,
            naatiNumber: vm.naatiNumber,
            credentialApplicationsInstance: credentialApplicationsInstance,
            person: serverModel
        });

        personRolePlaysInstance.reload.subscribe(function () {
            loadEntity(vm.naatiNumber());
        });

        configDocumentForm();
        vm.isPerson.subscribe(processRequest);

        var compositionComplete = false,
            queryString = null;

        vm.activate = function (naatiNumber, query) {
            notesInstance.allowAttachments(true);
            notesInstance.enableShowAllNotes(true);
            queryString = query;
            if (compositionComplete) {
                processRequest();
            }
        };

        vm.compositionComplete = function () {
            compositionComplete = true;
            processRequest();
        };

        var tabs = tabController.generateTabs({
            tabs: [
                {
                    active: true,
                    id: 'contactDetails',
                    name: 'Naati.Resources.Person.resources.ContactDetails',
                    type: 'compose',
                    composition: {
                        model: contactDetailsInstance,
                        view: 'views/naati-entity/contact-details'
                    },
                    visible: vm.isPerson //&& showContactDetailsTab
                },
                {
                    id: 'applications',
                    name: 'Naati.Resources.Person.resources.Applications',
                    type: 'compose',
                    composition: {
                        model: personApplicationInstance,
                        view: 'views/naati-entity/person-application'
                    },
                    click: onTabClick,
                },
                {
                    id: 'credentials',
                    name: 'Naati.Resources.Person.resources.Credentials',
                    type: 'compose',
                    composition: {
                        view: 'views/naati-entity/person-credentials',
                        model: personCredentialRequestsInstance
                    },
                    click: onTabClick
                },
                {
                    id: 'exemptions',
                    icon: ko.pureComputed(function () {
                        if (transactionInstance.isLoading()) {
                            return 'fa fa-circle-notch animated spin infinite text-info';
                        }
                        return '';
                    }),
                    name: 'Naati.Resources.Person.resources.Exemptions',
                    type: 'compose',
                    composition: {
                        model: personExemptionsInstance,
                        view: 'views/naati-entity/person-exemptions'
                    },
                },
                {
                    id: 'qrcodes',
                    icon: ko.pureComputed(function () {
                        if (transactionInstance.isLoading()) {
                            return 'fa fa-circle-notch animated spin infinite text-info';
                        }
                        return '';
                    }),
                    name: 'Naati.Resources.Person.resources.Qrcodes',
                    type: 'compose',
                    composition: {
                        model: personQrCodesInstance,
                        view: 'views/naati-entity/person-qrcodes'
                    },
                },
                {
                    id: 'transactions',
                    roles: [{ noun: enums.SecNoun.Invoice, verb: enums.SecVerb.Read }],
                    icon: ko.pureComputed(function () {
                        if (transactionInstance.isLoading()) {
                            return 'fa fa-circle-notch animated spin infinite text-info';
                        }
                        return '';
                    }),
                    name: 'Naati.Resources.Person.resources.Transactions',
                    type: 'compose',
                    composition: {
                        model: transactionInstance,
                        view: 'views/naati-entity/transaction'
                    },
                },
                {
                    id: 'notes',
                    roles: [{ noun: enums.SecNoun.Notes, verb: enums.SecVerb.Read }],
                    name: 'Naati.Resources.Test.resources.Notes',
                    type: 'compose',
                    composition: {
                        model: notesInstance,
                        view: 'views/shared/note.html'
                    },
                    click: onTabClick,
                },
                {
                    id: 'correspondence',
                    icon: ko.pureComputed(function () {
                        if (correspondenceInstance.isLoading()) {
                            return 'fa fa-circle-notch animated spin infinite text-info';
                        }
                        return '';
                    }),
                    name: 'Naati.Resources.Person.resources.Correspondence',
                    type: 'compose',
                    composition: {
                        model: correspondenceInstance,
                        view: 'views/naati-entity/person-correspondence'
                    },
                    click: onTabClick,
                },
                {
                    id: 'documents',
                    name: 'Naati.Resources.Test.resources.Documents',
                    type: 'compose',
                    composition: {
                        model: vm.documentsInstance,
                        view: 'views/document/document-form'
                    },
                    click: onTabClick,
                },
                {
                    id: 'rolePlays',
                    roles: [{ noun: enums.SecNoun.RolePlayer, verb: enums.SecVerb.Read }],
                    name: 'Naati.Resources.Person.resources.RolePlays',
                    type: 'compose',
                    composition: {
                        model: personRolePlaysInstance,
                        view: 'views/naati-entity/person-role-plays'
                    },
                    click: onTabClick,
                    visible: (vm.isRolePlayer && vm.isRolePlayerAvailable)
                }
            ]
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

            if (tabId == 'transactions') {
                transactionInstance.load(serverModel.NaatiNumber());
            }

            if (tabId == 'correspondence') {
                correspondenceInstance.load(serverModel.NaatiNumber());
            }

            var currentUrl = window.location;
            var baseUrl = currentUrl.protocol + '//' + currentUrl.host + '/' + currentUrl.pathname.split('/')[1];
            var url = baseUrl + '#person/' + vm.naatiNumber() + '?tab=' + tabId;
            window.history.replaceState(null, document.title, url);

            function displaySkarline() {
                if ($('#' + tabId).is(':visible')) {
                    return $.sparkline_display_visible();
                }
                setTimeout(displaySkarline, 100);
            }

            displaySkarline();
        }

        var selectTabTimeout = null;
        function processRequest() {
            var tabId = null;
            var firstVisibleTab = ko.utils.arrayFirst(tabs(), function (t) {
                return ko.unwrap(t.visible) || typeof (t.visible) === 'undefined';
            });

            if (firstVisibleTab) {
                firstVisibleTab = firstVisibleTab.id;
            }

            if (queryString) {
                tabId = queryString['tab'];
            }
            else {
                tabId = firstVisibleTab;
            }

            if (!tabId) {
                tabId = firstVisibleTab;
            }

            if (selectTabTimeout) {
                clearTimeout(selectTabTimeout);
            }

            selectTabTimeout = setTimeout(function () {
                $('#personEdit>.nav-tabs [aria-controls]').each(function () {
                    var $this = $(this);
                    $this.parent().removeClass('active');
                    $($this.attr('href')).removeClass('active');
                });

                $('#personEdit .nav-tabs [aria-controls="' + tabId + '"]').tab('show');
                selectTabTimeout = null;
            }, 100);
        }

        function canActivate(naatiNumber) {
            vm.naatiNumber(naatiNumber);

            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }

            return loadEntity(naatiNumber);
        }

        currentUser.hasPermission(enums.SecNoun.Credential, enums.SecVerb.Update).then(vm.canUpdateCertification);
        currentUser.hasPermission(enums.SecNoun.CertificationPeriod, enums.SecVerb.Update).then(vm.canUpdateCertificationPeriods);

        extendVm();

        function extendVm() {
            var personDetailsHeadingFormat = ko.Localization('Naati.Resources.Test.resources.PersonDetailsHeadingFormat');
            var naatiEntityDetailsHeadingFormat = ko.Localization('Naati.Resources.Test.resources.NaatiEntityDetailsHeadingFormat');

            $.extend(vm,
                {
                    windowTitle: ko.pureComputed(
                        function () {
                            if (vm.isPerson()) {
                                return personDetailsHeadingFormat
                                    .format(vm.person.NaatiNumber(),
                                        vm.person.DisplayName(),
                                        vm.person.BirthDate()
                                            ? " - {0}"
                                                .format(moment(vm.person.BirthDate(), CONST.settings.shortDateDisplayFormat)
                                                    .format(CONST.settings.shortDateDisplayFormat))
                                            : "",
                                        vm.person.PractitionerNumber() != null
                                            ? " - Practitioner No {0}".format(vm.person.PractitionerNumber())
                                            : "",
                                        vm.person.Deceased()
                                            ? " (Deceased)"
                                            : "");

                            }

                            return naatiEntityDetailsHeadingFormat
                                .format(vm.person.NaatiNumber(),
                                    vm.person.Name());
                        }),

                    showChangeCertificationPeriods: ko.computed(function () {
                        return vm.periods().length;
                    }),

                    showExtendCertifications: function () {
                        return vm.canUpdateCertification() && (vm.person.IsPractitioner() || vm.person.IsFormerPractitioner());
                    },

                    detailsTitle: ko.pureComputed(
                        function () {
                            var format = ko.Localization('Naati.Resources.Test.resources.PersonDetailsHeadingFormat2');
                            return format.format(vm.person.NaatiNumber());
                        }),

                    tabOptions: {
                        tabContainerId: 'personEdit',
                        id: 'personEdit',
                        event: {
                            'shown.bs.tab': onTabClick
                        },
                        tabs: tabs
                    },

                    getUrl: function (url) {
                        if (!vm.person.NaatiNumber()) {
                            return '';
                        }

                        return url.format(vm.person.NaatiNumber());
                    },
                    archiveHistoryUrl: ko.pureComputed(function () {
                        if (vm.person.ArchiveHistory().length && vm.person.NaatiNumber()) {
                            return '#person/' + vm.person.NaatiNumber() + '/archivehistory';
                        }
                        return '';
                    }),
                    auditHistoryUrl: ko.pureComputed(function () {
                        if (vm.person.EntityId()) {
                            return '#person/' + vm.person.EntityId() + '-' + vm.person.NaatiNumber() + '-' + vm.person.PersonId() + '/audithistory';
                        }
                        return '';
                    }),
                    applicationNewOptions: {
                        view: 'views/application/application-new',
                        model: applicationNewInstance
                    },
                    deregisterMyNaatiOptions: {
                        view: 'views/naati-entity/deregister-mynaati-account',
                        model: deregisterMyNaatiAccountInstance
                    },
                    deregisterMfaOptions: {
                        view: 'views/naati-entity/deregister-mfa-account',
                        model: deregisterMfaAccountInstance
                    },
                    credentialApplicationsOptions: {
                        view: 'views/naati-entity/credential-applications',
                        model: credentialApplicationsInstance
                    },
                    credentialApplications: function () {
                        credentialApplicationsInstance.show();
                    },
                    newApplication: function () {
                        applicationNewInstance.show(vm.naatiNumber());
                    },
                    extendCertifications: function () {
                        extendCertificationsInstance.show().then(function () {
                            extendCertificationsInstance.close();
                            loadEntity(vm.naatiNumber());
                        });
                    },
                    deregisterMyNaatiAccount: function () {
                        deregisterMyNaatiAccountInstance.show(vm.person.UserName(), vm.person.DisplayName()).then(function (data) {
                            deregisterMyNaatiAccountInstance.close();
                            if (data) {
                                toastr.success(ko.Localization('Naati.Resources.Person.resources.MyNaatiAccountDeleted'));
                            }
                            loadEntity(vm.naatiNumber());
                        });
                    },
                    deregisterMfaAccount: function () {
                        deregisterMfaAccountInstance.show(vm.naatiNumber()).then(function (data) {
                            deregisterMfaAccountInstance.close();
                            if (data) {
                                toastr.success(ko.Localization('Naati.Resources.Person.resources.MyMfaAccountDeleted'));
                            }
                            loadEntity(vm.naatiNumber());
                        });
                    },
                    changePhotoOptions: {
                        view: 'views/naati-entity/change-photo',
                        model: changePhotoInstance
                    },
                    extendCertificationsOptions: {
                        view: 'views/naati-entity/extend-certifications',
                        model: extendCertificationsInstance
                    },
                    changePhoto: function () {
                        changePhotoInstance.show().then(function () {
                            vm.personPhotoOptions.component.resetThumbStyle();
                            changePhotoInstance.close();
                            loadEntity(vm.naatiNumber());
                        });
                    }
                });
        };

        vm.personPhotoOptions = {
            changePhoto: vm.changePhoto,
            person: vm.person
        };

        vm.close = function () {
            if (vm.dirtyFlag().isDirty()) {
                message.confirm({
                    title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                    content: ko.Localization('Naati.Resources.Shared.resources.ConfirmBeforeSaving')
                })
                    .then(
                        function (answer) {
                            if (answer === 'yes') {
                                router.navigate('person/');
                            }
                        });
            } else {
                router.navigate('person/');
            }
        };

        vm.unlock = function () {
            if (!vm.hasMyNaatiAccount()) {
                return;
            }

            var userName = vm.person.UserName();

            personDataService.post({ UserName: userName }, 'unlockMyNaatiUser').then(function (data) {
                if (data) {
                    vm.canActivate(vm.person.NaatiNumber()).then(function () {
                        vm.activate();
                        var message = ko.Localization('Naati.Resources.Person.resources.UnlockSucessful');
                        toastr.success(message);

                    });

                } else {
                    var message = ko.Localization('Naati.Resources.Person.resources.UnlockUnsuccesful');
                    toastr.error(message);
                }
            });
        }

        personApplicationInstance.newApplication = vm.newApplication;

        function loadEntity(naatiNumber) {
            return personDataService.getFluid('entity/' + naatiNumber).then(function (entity) {
                if (!entity) {
                    return false;
                }
                vm.entityId(entity.EntityId);
                vm.isPerson(entity.PersonId);
                vm.documentsInstance.relatedId(entity.PersonId);
                vm.documentsInstance.search();

                if (entity.PersonId) {
                    if (entity.PrimaryEmail.length !== 0) {
                        vm.email(entity.PrimaryEmail);
                        loadMyNaatiAccountDetails(entity.PrimaryEmail);
                    }
                    return loadPerson(naatiNumber);
                }
                else {
                    vm.naatiNumber(naatiNumber);
                    notesInstance.getNotes();

                    ko.viewmodel.updateFromModel(vm.person, $.extend(cleanModel, entity));

                    return true;
                }
            });
        }

        function loadPerson(naatiNumber) {
            vm.isPerson(true);
            vm.isRolePlayer(false);

            personApplicationInstance.load(naatiNumber);
            personCredentialRequestsInstance.load(naatiNumber);
            personRolePlaysInstance.load(naatiNumber);
            personExemptionsInstance.load(naatiNumber);
            personQrCodesInstance.load(naatiNumber);
            loadPeriods(naatiNumber);
            return personDataService.getFluid(naatiNumber)
                .then(
                    function (data) {
                        if (data) {
                            vm.naatiNumber(naatiNumber);
                            vm.isRolePlayer(data.IsRolePlayer);
                            vm.isRolePlayerAvailable(data.IsRolePlayerAvailable);
                            vm.person.IsMfaEnabled(data.MfaModeIsSet ? 'Enabled' : 'Disabled');
                            vm.person.HasMfaAccount(data.MfaModelIdSet);
                            vm.person.PractitionerNumber(data.PractitionerNumber);
                            notesInstance.getNotes();
                            vm.person.Gender(''); // hack for when loading for second time (after Cancel); required because select-component makes this an array, so we need to un-array it or the next line will fail
                            ko.viewmodel.updateFromModel(vm.person, data);
                            if (vm.person.BirthDate()) {
                                vm.person.BirthDate(moment(vm.person.BirthDate()).format(CONST.settings.shortDateDisplayFormat));
                            }
                            if (data.MfaExpireStartDate) {
                                vm.person.MfaExpireStartDate(new Date(data.MfaExpireStartDate).toLocaleDateString('en-AU', {
                                    day: '2-digit',
                                    month: '2-digit',
                                    year: 'numeric',
                                }));
                            }
                            vm.person.DisabledByNaati(data.AccessDisabledByNcms ? 'Yes' : 'No');
                            var genderOptions = sharedFilters.getFilter('Gender').componentOptions;

                            var genderText = '';
                            if (vm.person.Gender()) {
                                genderText = genderOptions.options.find(function (x) {
                                    return x.value === vm.person.Gender();
                                }).text;
                            }
                            vm.person.GenderText(genderText);

                            vm.person.SendCorrespondence(!vm.person.DoNotSendCorrespondence());
                            contactDetailsInstance.populate(data.EntityId, data.ContactDetails);

                            vm.dirtyFlag().reset();
                            return true;
                        }
                        return false;
                    },
                    function () {
                        return false;
                    });
        }

        function loadPeriods(naatiNumber) {
            var periodsRequest = {
                NaatiNumber: naatiNumber,
                CertificationPeriodStatus: [
                    enums.CertificationPeriodStatus.Expired,
                    enums.CertificationPeriodStatus.Current,
                    enums.CertificationPeriodStatus.Future
                ]
            };

            personService.getFluid('certificationperiods', periodsRequest).then(vm.periods);
        }

        function loadMyNaatiAccountDetails(username) {
            return personDataService.getFluid('mynaatidetails/' + username)
                .then(function (data) {
                    if (data && data.IsActive) {
                        vm.hasMyNaatiAccount(true);
                        serverModel.UserName(username);
                        serverModel.MyNaatiRegistrationDate(moment(data.CreationDate)
                            .format(CONST.settings.shortDateDisplayFormat));
                        serverModel.IsAccountLocked(data.IsLocked);
                        serverModel.LastmyNaatiLogin(moment(data.LastLoginDate)
                            .format(CONST.settings.shortDateTimeDisplayFormat));
                        serverModel.LastPasswordChangeDate(moment(data.LastPasswordChangedDate)
                            .format(CONST.settings.shortDateTimeDisplayFormat));
                    } else {
                        vm.hasMyNaatiAccount(false);
                        serverModel.MyNaatiRegistrationDate('N/A');
                        serverModel.IsAccountLocked(false);
                        serverModel.LastmyNaatiLogin('N/A');
                        serverModel.LastPasswordChangeDate('N/A');
                        serverModel.UserName('N/A');
                    }

                    vm.dirtyFlag().reset();
                });
        }

        function configDocumentForm() {

            vm.documentsInstance = documentForm.getInstance();
            personService.getFluid('documentTypesToUpload')
                .then(function (types) {
                    vm.documentsInstance.types(types);
                    vm.documentsInstance.showEportalDownload(false);
                    vm.documentsInstance.showMergeDocument(false);
                });
            vm.documentsInstance.parseFileName = function (fileName) {
                var tmp = fileName.FileName.split('.');
                return tmp.slice(0, tmp.length - 1).join('.');
            };

            vm.documentsInstance.fileUpload.url = personService.url() + '/documents/upload';
            vm.documentsInstance.fileUpload.formData = ko.computed(function () {
                return {
                    id: vm.documentsInstance.currentDocument.Id() || 0,
                    file: vm.documentsInstance.currentDocument.File(),
                    type: vm.documentsInstance.currentDocument.Type(),
                    personId: vm.documentsInstance.relatedId(),
                    title: vm.documentsInstance.currentDocument.Title(),
                    storedFileId: vm.documentsInstance.currentDocument.StoredFileId() || 0
                };
            });

            $.extend(vm.documentsInstance, {
                getDocumentsPromise: function () {
                    return personService.getFluid('documents/' + vm.documentsInstance.relatedId(), {
                        supressResponseMessages: true
                    });
                },
                postDocumentPromise: function () {
                    var data = ko.toJS(vm.documentsInstance.currentDocument);
                    data.RelatedId = vm.documentsInstance.relatedId();
                    return personService.post(data, 'documents');

                },
                removeDocumentPromise: function (document) {
                    return personService.remove('documents/' + document.StoredFileId);
                }
            });
        }

        return vm;
    });