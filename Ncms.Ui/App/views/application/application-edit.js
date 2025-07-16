define([
    'plugins/router',
    'modules/common',
    'modules/enums',
    'modules/custom-validator',
    'services/screen/date-service',
    'services/screen/message-service',
    'services/person-data-service',
    'services/application-data-service',
    'views/application/application-parts/application-applicant',
    'views/application/application-parts/application-header',
    'views/application/application-parts/application-credential-request',
    'views/application/application-parts/prerequisite-applications',
    'views/document/document-form',
    'views/shared/note',
    'views/application/application-correspondence/application-correspondence',
    'services/institution-data-service',
    'components/views/tab-lazy-load-controller'
],
    function (router, common, enums, customValidator, dateService, message, personService, applicationService, applicant,
        applicationHeader, credentialRequests, prerequisiteApplications, documentForm, notes, applicationCorrespondence, institutionService,
        tabController) {
        var compositionComplete = false;
        var queryString;

        var functions = common.functions();

        // note, some of the members of the serverModel aren't used, but we need them because they make a 
        // round-trip from the server and back again (this keeps back-end code simpler)
        var serverModel = {
            ApplicationId: ko.observable(0),
            NaatiNumber: ko.observable(),
            ApplicationTypeId: ko.observable(0),
            EnteredDate: ko.observable(),
            ReceivingOfficeId: ko.observable(),
            PreferredTestLocationId: ko.observable(),
            StatusChangeUserId: ko.observable(),
            StatusChangeDate: ko.observable(),
            OwnedByUserId: ko.observable(),
            ApplicationStatusTypeId: ko.observable(),
            ApplicationReference: ko.observable(),
            ApplicationOwner: ko.observable(),
            ApplicationStatus: ko.observable(),
            EnteredUserName: ko.observable(),
            SponsorInstitutionContactPersonId: ko.observable(),
            SponsorInstitutionNaatiNumber: ko.observable(),
            ShowNonPreferredTestLocationInfo: ko.observable(true),
            ErrorNonPreferredTestLocationInfo: ko.observable(false),
            AutoCreated: ko.observable(false)
        };

        serverModel.SponsorInstitutionContactPersonId.extend({
            required: {
                onlyIf: ko.pureComputed(function () {
                    return serverModel.SponsorInstitutionNaatiNumber();
                })
            }
        });

        var validator = customValidator.create(serverModel);

        var cleanModel = ko.toJS(serverModel);

        var vm = {
            application: serverModel,
            credentialRequests: ko.observableArray(),
            locations: ko.observableArray(),
            sponsorContacts: ko.observableArray(),
            offices: ko.observableArray(),
            actions: ko.observableArray(),
            applicationTypeName: ko.observable(),
            applicantInstance: applicant.getInstance(),
            applicationHeaderInstance: applicationHeader.getInstance(),
            credentialRequestsInstance: credentialRequests.getInstance(),
            notesInstance: notes.getInstance(),
            applicationCorrespondenceInstance: applicationCorrespondence.getInstance(),
            prerequisiteApplicationsInstance: prerequisiteApplications.getInstance(),
            isAuthorisedRoleUser: ko.observable(false),
            institutions: ko.observableArray(),
            isLock: ko.observable(false),
            tabId: ko.observable(),
            tabOptions: {
                tabs: ko.observableArray()
            }
        };

        vm.credentialRequestOptions = {
            name: 'credential-request',
            params: {
                credentialRequests: vm.credentialRequests
            }
        };

        vm.applicationHeaderInstance.sections.subscribe(function (newValue) {
            if (newValue && newValue.length) {
                if (!vm.tabId()) {
                    vm.tabId('application');
                }
            }
            else if (vm.tabId() == 'application') {
                vm.tabId('credentialRequests');
            }
        });

        serverModel.ApplicationId.subscribe(function (newValue) {
            vm.documentsInstance.relatedId(newValue);
            vm.documentsInstance.search();
        });

        serverModel.SponsorInstitutionNaatiNumber.subscribe(function (newValue) {
            if (newValue > 0) {
                institutionService.getFluid('contactPersonByNaatiNo/' + newValue).then(function (data) {
                    vm.sponsorContacts(data);
                });
            } else {
                vm.application.SponsorInstitutionContactPersonId(null);
                vm.sponsorContacts([]);
            }
        });

        var dirtyFlag = new ko.DirtyFlag([serverModel], false);
        configDocumentForm();
        vm.credentialRequestsInstance.selectAction = selectAction;

        vm.hasCredentialsRequest = ko.computed(function () {
            return serverModel.ApplicationId() && vm.credentialRequestsInstance.credentialRequests() && vm.credentialRequestsInstance.credentialRequests().length;
        });

        vm.close = function () {
            if (dirtyFlag().isDirty()) {
                message.confirm({
                        title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                        content: ko.Localization('Naati.Resources.Shared.resources.ConfirmBeforeSaving')
                    })
                    .then(
                        function (answer) {
                            if (answer === 'yes') {
                                router.navigate('application/');
                            }
                        });
            } else {
                router.navigate('application/');
            }
        };

        //vm.naatiNumberOptions = {
        //    source: function(naatiNumber, callback) {
        //        if (!$.trim(naatiNumber)) {
        //            return callback([]);
        //        }

        //        personService.getFluid('search', { term: naatiNumber, type: enums.EntitySearchType.Institution }).then(callback);
        //    },
        //    multiple: false,
        //    value: vm.application.SponsorInstitutionNaatiNumber,
        //    textValue: serverModel.SponsorInstitutionNaatiNumber,
        //    valueProp: 'NaatiNumber',
        //    labelProp: 'Name',
        //    resattr: {
        //        placeholder: 'Naati.Resources.Application.resources.SponsorOrganisation'
        //    }
        //};

        var validation = ko.validatedObservable(serverModel);

        $.extend(vm, {
            title: ko.computed(function () {
                return '{0} - {1}'.format(vm.applicationTypeName(), serverModel.ApplicationReference());
            }),
            subtitle: ko.computed(function () {
                if (!serverModel.ApplicationId()) {
                    return ko.Localization('Naati.Resources.Application.resources.AddingApplicationTitle');
                }

                return '{0} - {1} {2} {3} - {4}'.format(
                    ko.Localization('Naati.Resources.Application.resources.EditingApplicationTitle'),
                    vm.applicantInstance.applicant.GivenName(),
                    vm.applicantInstance.applicant.OtherNames(),
                    vm.applicantInstance.applicant.FamilyName(),
                    vm.applicantInstance.applicant.NaatiNumber());
            }),
            sponsorContactOptions: {
                value: serverModel.SponsorInstitutionContactPersonId,
                multiple: false,
                options: vm.sponsorContacts,
                optionsValue: 'ContactPersonId',
                optionsText: 'Name',
                disable: ko.computed(function () {
                    if (vm.isLock())
                        return true;
                    return (vm.sponsorContacts().length <= 0 || vm.application.SponsorInstitutionNaatiNumber() <= 0);
                })
            },
            testLocationOptions: {
                value: serverModel.PreferredTestLocationId,
                multiple: false,
                options: vm.locations,
                optionsValue: 'Id',
                optionsText: 'DisplayName',
                afterRender: function () { functions.getLookup('TestLocation').then(vm.locations); },
                disable: ko.computed(function () { return vm.isLock(); })
            },
            enteredOfficeOptions: {
                value: serverModel.ReceivingOfficeId,
                multiple: false,
                options: vm.offices,
                optionsValue: 'Id',
                optionsText: 'DisplayName',
                afterRender: function () { functions.getLookup('Office').then(vm.offices); },
                addChooseOption: false,
                disable: ko.computed(function () {
                    if (vm.isLock())
                        return true;
                    return !vm.isAuthorisedRoleUser();
                })


            },
            sponsorOrganisationOptions: {
                value: serverModel.SponsorInstitutionNaatiNumber,
                multiple: false,
                options: vm.institutions,
                optionsValue: 'Id',
                optionsText: 'DisplayName',
                afterRender: function () { functions.getLookup('Institution').then(vm.institutions); },
                disable: ko.computed(function () {
                    if (vm.isLock())
                        return true;
                    return !vm.isAuthorisedRoleUser();
                })
            }

        });


        vm.institutions.subscribe(function (options) { setIfSingle(options, serverModel.SponsorInstitutionNaatiNumber); });

        function setIfSingle(options, observable) {
            if (options.length === 1) {
                observable(options[0].Id);
            }
        }

        var disabledApplicationTab = ko.computed(function () {
            return !vm.applicationHeaderInstance.sections() || !vm.applicationHeaderInstance.sections().length;
        });

        disabledApplicationTab.subscribe(processRequest);

        var disabledPrerequisiteApplicationsTab = ko.computed(function () {
            return !vm.prerequisiteApplicationsInstance.applications() || !vm.prerequisiteApplicationsInstance.applications().length;
        });

        disabledPrerequisiteApplicationsTab.subscribe(processRequest);

        vm.tabOptions = {
            id: 'applicationEdit',
            tabs: tabController.generateTabs({
                tabs: [
                    {
                        id: 'application',
                        disabled: disabledApplicationTab,
                        label: ko.computed(function () {
                            return '{0} {1}'.format(vm.applicationTypeName(),
                                ko.Localization('Naati.Resources.Shared.resources.Application'));
                        }),
                        type: 'compose',
                        composition: {
                            model: vm.applicationHeaderInstance,
                            view: 'views/application/application-parts/application-header'
                        },
                        click: onTabClick,
                        validate: vm.applicationHeaderInstance.validate,
                        clearValidation: vm.applicationHeaderInstance.clearValidation,
                        valid: vm.applicationHeaderInstance.valid
                    }, {
                        id: 'credentialRequests',
                        name: 'Naati.Resources.Application.resources.CredentialRequests',
                        type: 'compose',
                        composition: {
                            model: vm.credentialRequestsInstance,
                            view: 'views/application/application-parts/application-credential-request'
                        },
                        click: onTabClick
                    },
                    {
                        id: 'prerequisiteApplications',
                        //disabled: disabledPrerequisiteApplicationsTab,
                        name: 'Naati.Resources.Application.resources.PrerequisiteApplications',
                        type: 'compose',
                        composition: {
                            model: vm.prerequisiteApplicationsInstance,
                            view: 'views/application/application-parts/prerequisite-applications'
                        },
                        click: onTabClick
                    },{
                        id: 'documents',
                        name: 'Naati.Resources.Test.resources.Documents',
                        type: 'compose',
                        composition: {
                            model: vm.documentsInstance,
                            view: 'views/document/document-form'
                        },
                        click: onTabClick
                    }, {
                        id: 'notes',
                        name: 'Naati.Resources.Test.resources.Notes',
                        roles: [{ noun: enums.SecNoun.Notes, verb: enums.SecVerb.Read }],
                        type: 'compose',
                        composition: {
                            model: vm.notesInstance,
                            view: 'views/shared/note.html'
                        },
                        click: onTabClick
                    },
                    {
                        id: 'correspondence',
                        name: 'Naati.Resources.EmailMessage.resources.SystemEmails',
                        type: 'compose',
                        composition: {
                            model: vm.applicationCorrespondenceInstance,
                            view: 'views/application/application-correspondence/application-correspondence'
                        },
                        click: onTabClick
                    },
                ],
                activeTab: vm.tabId
            })
        };

        vm.save = function () {
            var defer = Q.defer();

            if (!vm.tabOptions.component.validate()) {
                return defer.promise;
            }

            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return defer.promise;
            }

            var request = {
                ApplicationInfo: ko.toJS(vm.application),
                Sections: ko.toJS(vm.applicationHeaderInstance.sections),
                CredentialRequests: ko.toJS(vm.credentialRequestsInstance.credentialRequests),
                Notes: [],
                PersonNotes: []
            };

            $.each(request.Sections, function (i, section) {
                ko.utils.arrayForEach(section.Fields, function (field) {
                    if ((field.DataTypeId === enums.ApplicationFieldTypes.Date || field.DataTypeId === enums.ApplicationFieldTypes.EndorsedQualificationStartDate || field.DataTypeId === enums.ApplicationFieldTypes.EndorsedQualificationEndDate) && field.Value) {

                        var date = dateService.toPostDate(field.Value);
                        field.Value = date;
                    }
                });
            });

            applicationService.post(request, 'update')
                .then(function () {
                    dirtyFlag().reset();
                    vm.applicationHeaderInstance.resetDirtyFlag();
                    vm.credentialRequestsInstance.resetDirtyFlag();
                    defer.resolve('fulfilled');
                    toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                });

            return defer.promise;
        };

        vm.compositionComplete = function () {
            compositionComplete = true;
            processRequest();
        };

        vm.canActivate = function (applicationId, naatiNumber, query) {
            queryString = query || {};
            applicationId = parseInt(applicationId);

            ko.viewmodel.updateFromModel(serverModel, cleanModel);
            if (queryString['applicationType']) {
                serverModel.ApplicationTypeId(queryString['applicationType']);
                vm.applicationHeaderInstance.load(serverModel.ApplicationTypeId());
            }

            vm.credentialRequestsInstance.load(applicationId);

            serverModel.ApplicationId(applicationId);

            if (!applicationId) {
                vm.applicantInstance.add(naatiNumber);
                return true;
            }

            currentUser.hasPermission(enums.SecNoun.Application, enums.SecVerb.Update).then(vm.isAuthorisedRoleUser);

            return loadApplication();
        };

        function loadIsLock() {
            currentUser.hasPermission(enums.SecNoun.Application, enums.SecVerb.Manage)
                .then(function(hasAdminPermission) {
                    var isLock = !hasAdminPermission
                        ? vm.application.ApplicationStatusTypeId() === enums.ApplicationStatusTypes.Completed
                        : false;
                    vm.isLock(isLock);
                });
        }

        vm.activate = function (applicationId, naatiNumber, query) {
            if (compositionComplete) {
                processRequest();
            }

            vm.notesInstance.allowAttachments(true);
            vm.documentsInstance.currentDocumentType('');

            applicationService.getFluid('documentTypes/' + serverModel.ApplicationId())
                .then(function (types) {
                    vm.documentsInstance.types(types);
                    vm.documentsInstance.showEportalDownload(false);
                    vm.documentsInstance.showMergeDocument(false);
                });
        };

        vm.selectAction = function (action) {
            selectAction(action);
        };

        vm.statusClass = function () {
            var statuses = enums.ApplicationStatusTypes;
            var status = serverModel.ApplicationStatusTypeId();

            if (status === statuses.Draft) {
                return 'label-gray';
            }

            if (status === statuses.ProcessingSubmission || status === statuses.ProcessingApplicationInvoice) {
                return 'label-dark-gray';
            }
            if (status === statuses.Entered) {
                return 'label-info';
            }
            if (status === statuses.BeingChecked) {
                return 'label-orange';
            }
            if (status === statuses.Rejected) {
                return 'label-danger';
            }
            if (status === statuses.InProgress) {
                return 'label-orange';
            }
            if (status === statuses.Completed) {
                return 'label-success';
            }
            if (status === statuses.AwaitingAssessmentPayment || status === statuses.AwaitingApplicationPayment) {
                return 'label-dark-yellow';
            }
            else {
                return 'label-success';
            }
        };

        $.extend(vm.notesInstance, {
            getNotesPromise: function () {
                return applicationService.getFluid(serverModel.ApplicationId() + '/notes');
            },
            postNotesPromise: function () {
                return applicationService.post($.extend(vm.notesInstance.parseNote(), { ApplicationId: serverModel.ApplicationId(), Highlight: true }), 'notes');
            },
            removeNotesPromise: function (note) {
                return applicationService.remove(note.NoteId + '/' + serverModel.ApplicationId() + '/notes');
            }
        });

        vm.canSave = ko.pureComputed(function () {
            return dirtyFlag().isDirty() || vm.applicationHeaderInstance.isDirty() || vm.credentialRequestsInstance.isDirty();
        });

        function selectAction(action, credentialRequest) {
            if (!isDirty()) {
                return takeAction(action, credentialRequest);
            }

            message.confirm({
                title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
                content: ko.Localization('Naati.Resources.Shared.resources.SaveBeforeAction')
            }).then(function (result) {
                if (result === 'yes') {
                    vm.save().then(function (saveResult) {
                        if (saveResult === 'fulfilled') {
                            takeAction(action, credentialRequest);
                        }
                    });
                }
            });
        };

        function takeAction(action, credentialRequest) {
            var request = {
                ApplicationStatusId: serverModel.ApplicationStatusTypeId(),
                ActionId: action.Id,
                ApplicationId: serverModel.ApplicationId(),
                CredentialRequestId: credentialRequest ? credentialRequest.Id() : 0,
                CredentialTypeId: credentialRequest ? credentialRequest.CredentialType.Id() : 0,
                ApplicationTypeId: serverModel.ApplicationTypeId(),
            };

            applicationService.post(request, 'action').then(function (data) {
                if (!checkAndShowMessages(data)) {
                    applicationService.getFluid('steps', request).then(function (steps) {
                        if (steps.length) {
                            return router.navigate('application-wizard/' + serverModel.ApplicationId() + '/' + action.Id + (credentialRequest ? '/' + credentialRequest.Id() : ''));
                        }

                        applicationService.post(request, 'wizard').then(function () {
                            toastr.success(ko.Localization('Naati.Resources.Application.resources.ActionTakenSuccessfully'));
                            var applicationId = serverModel.ApplicationId();
                            vm.credentialRequestsInstance.load(applicationId);
                            vm.applicationCorrespondenceInstance.load(applicationId);
                            loadApplication();
                        });
                    });
                }
            });
        }

        function isDirty() {
            return dirtyFlag().isDirty()
                || vm.applicantInstance.isDirty()
                || vm.applicationHeaderInstance.isDirty()
                || vm.credentialRequestsInstance.isDirty()
                || vm.documentsInstance.isDirty()
                || vm.notesInstance.isDirty();
        }

        function checkAndShowMessages(messages) {
            var genericMessages = [];

            ko.utils.arrayForEach(messages, function (m) {
                if (!m.Field) {
                    genericMessages.push(m.Message);
                }
            });

            if (genericMessages.length) {
                toastr.error(genericMessages.join('<br /><br />'), null, {
                    closeButton: true,
                    timeOut: 0
                });
            }

            resetValidation();

            ko.utils.arrayForEach(messages, function (i) {
                if (!i.Field) {
                    return;
                }
                validator.setValidation(i.Field, false, i.Message);
            });

            validator.isValid();

            return messages && messages.length;
        }

        function resetValidation() {
            validator.reset();

            if (validation.errors) {
                return validation.errors.showAllMessages(false);
            }
        };

        function onTabClick(tabOption) {
            if (tabOption.disabled()) {
                return;
            }

            var tabId = tabOption.id;

            var currentUrl = window.location;
            var baseUrl = currentUrl.protocol + '//' + currentUrl.host + '/' + currentUrl.pathname.split('/')[1];
            var url = baseUrl + '#application/' + serverModel.ApplicationId();

            queryString['tab'] = tabId;
            vm.tabId(tabId);

            var query = [];
            for (var q in queryString) {
                query.push('{0}={1}'.format(q, queryString[q]));
            }

            url += '?' + query.join('&');

            window.history.replaceState(null, document.title, url);
        }

        function processRequest() {
            if (!$.isEmptyObject(queryString)) {
                vm.tabId(queryString['tab']);
            }
        }

        function loadApplication() {
            vm.notesInstance.getNotes();

            applicationService.getFluid(serverModel.ApplicationId() + '/credentialotherrequests').then(function (data) {
                var credentialRequests = ko.viewmodel.fromModel(data);
                vm.credentialRequests(credentialRequests());
            });

            return applicationService.get({ id: serverModel.ApplicationId() }).then(function (data) {
                vm.applicationTypeName(data.ApplicationTypeName);

                data.SponsorInstitutionContactPersonId = data.SponsorInstitutionContactPersonId || null;
                data.SponsorInstitutionNaatiNumber = data.SponsorInstitutionNaatiNumber || null;

                ko.viewmodel.updateFromModel(serverModel, data);
                vm.applicantInstance.add(serverModel.NaatiNumber());
                vm.applicationCorrespondenceInstance.load(serverModel.ApplicationId(), serverModel.NaatiNumber());
                vm.applicationHeaderInstance.load(data.ApplicationId);
                applicationService.getFluid('action/' + serverModel.ApplicationStatusTypeId()).then(vm.actions);
                vm.prerequisiteApplicationsInstance.load(serverModel.ApplicationId());

                loadIsLock();
                ko.computed(function() {
                    vm.credentialRequestsInstance.isEnable(!vm.isLock());
                });

                resetValidation();
                dirtyFlag().reset();
                return true;
            });
        }

        function configDocumentForm() {

            vm.documentsInstance = documentForm.getInstance();
            vm.documentsInstance.parseFileName = function (fileName) {
                var tmp = fileName.FileName.split('.');
                return tmp.slice(0, tmp.length - 1).join('.');
            };

            vm.documentsInstance.fileUpload.url = applicationService.url() + '/documents/upload';
            vm.documentsInstance.fileUpload.formData = ko.computed(function () {
                return {
                    id: vm.documentsInstance.currentDocument.Id() || 0,
                    file: vm.documentsInstance.currentDocument.File(),
                    type: vm.documentsInstance.currentDocument.Type(),
                    applicationId: vm.documentsInstance.relatedId(),
                    title: vm.documentsInstance.currentDocument.Title(),
                    storedFileId: vm.documentsInstance.currentDocument.StoredFileId() || 0
                };
            });

            $.extend(vm.documentsInstance, {
                getDocumentsPromise: function () {
                    if (vm.documentsInstance.relatedId() <= 0) {
                        return Promise.resolve([]);
                    }

                    return applicationService.getFluid('documents/' + vm.documentsInstance.relatedId(), {
                        supressResponseMessages: true
                    });
                },
                postDocumentPromise: function () {
                    var data = ko.toJS(vm.documentsInstance.currentDocument);
                    data.RelatedId = vm.documentsInstance.relatedId();
                    return applicationService.post(data, 'documents');
                },
                removeDocumentPromise: function (document) {
                    return applicationService.remove('documents/' + document.StoredFileId);
                }
            });
        }

        vm.ApplicationIsAutoCreated = function () {
            var autoCreated = serverModel.AutoCreated;

            return autoCreated;
        }

        return vm;

    });
