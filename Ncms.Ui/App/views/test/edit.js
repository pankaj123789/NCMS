define([
    'services/examiner-data-service',
    'views/test/edit-tab/examiner',
    'views/test/edit-tab/examiner-files',
    'modules/common',
    'views/shared/note',
    'views/test/edit-tab/result',
    'views/test/edit-tab/paid-review',
    'views/test/edit-tab/history',
    'views/test/add-examiner',
    'views/test/edit-examiner',
    'views/test/edit-marks',
    'views/document/table',
    'views/document/document-form',
    'views/document/document-form-multi',
    'services/screen/date-service',
    'services/test-data-service',
    'services/testresult-data-service',
    'services/file-data-service',
    'services/screen/message-service',
    'modules/enums',
    'modules/custom-validator',
    'services/person-data-service',
    'services/application-data-service',
    'plugins/router',
    'views/test/edit-tab/confirmation',
    'components/views/tab-lazy-load-controller',
    'views/test/edit-tab/feedback'
], function (
    examinerService,
    examiner,
    examinerFiles,
    common,
    notes,
    result,
    paidReview,
    history,
    addExaminer,
    editExaminer,
    editMarks,
    documentTable,
    documentForm,
    documentFormMulti,
    dateService,
    testService,
    testresultService,
    fileService,
    messageService,
    enums,
    customValidator,
    personService,
    applicationService,
    router,
    saveResultConfirmation,
    tabController,
    feedback

) {
    var queryString;
    var commonFunctions = common.functions();

    var testServerModel = {
        TestAttendanceId: ko.observable(),
        TestSessionId: ko.observable(),
        ApplicationId: ko.observable(),
        ApplicationReference: ko.observable(),
        ApplicationTypeId: ko.observable(),
        CredentialTypeId: ko.observable(),
        SkillId: ko.observable(),
        ApplicationType: ko.observable(),
        CredentialTypeInternalName: ko.observable(),
        Skill: ko.observable(),
        ResultId: ko.observable(),
        CurrentJobId: ko.observable(),
        LastReviewTestResultId: ko.observable(),
        Language1Id: ko.observable(),
        Language2Id: ko.observable(),
        TestDate: ko.observable(),
        TestStatusTypeId: ko.observable(),
        TestStatus: ko.observable(),
        TestLocationId: ko.observable(),
        TestLocation: ko.observable(),
        VenueId: ko.observable(),
        Venue: ko.observable(),
        TestResultStatus: ko.observable(),
        TestResultStatusId: ko.observable(),
        PersonId: ko.observable(),
        NaatiNumber: ko.observable(),
        TestMaterialIds: ko.observableArray(),
        TestMaterialNames: ko.observableArray(),
        Actions: ko.observableArray(),
        ApplicationStatusTypeId: ko.observable(),
        CredentialRequestId: ko.observable(),
        CredentialRequestStatusTypeId: ko.observable(),
        Supplementary: ko.observable(),
        SupplementaryCredentialRequest: ko.observable(),
        AllowSupplementary: ko.observable(),
        HasDowngradePaths: ko.observable(),
        MarkingSchemaTypeId: ko.observable(),
        DefaultTestSpecification: ko.observable()
    }

    var personServerModel = {
        PersonId: ko.observable(),
        NaatiNumber: ko.observable(),
        PersonPractitionerNumber: ko.observable(),
        GivenName: ko.observable(),
        SurName: ko.observable(),
        BirthDate: ko.observable(),
        HasPhoto: ko.observable(),
        IsEportalActive: ko.observable(),
        EportalRegistrationDate: ko.observable(),

    }

    var validator = customValidator.create(testServerModel);
    var cleanTestModel = ko.toJS(testServerModel);
    var cleanPersonModel = ko.toJS(personServerModel);

    var vm = {
        selectAction: selectAction,
        actions: ko.observableArray(),
        test: testServerModel,
        person: personServerModel,
        testExaminers: ko.observableArray(),
        addExaminerInstance: addExaminer.getInstance(),
        confirmationInstance: saveResultConfirmation.getInstance(),
        editExaminerInstance: editExaminer.getInstance(),
        examinerInstance: examiner.getInstance(),
        examinerFilesInstance: examinerFiles.getInstance(),
        notesInstance: notes.getInstance(),
        generalDocumentsInstance: documentForm.getInstance(),
        editMarksInstance: editMarks.getInstance(),
        testAssetInstance: documentFormMulti.getInstance(),
        loadingTest: ko.observable(false),
        testExaminersCount: ko.observable(),
        isDirty: ko.observable(),
        materials: ko.pureComputed(function () {
            return vm.test.TestMaterialNames().join(', ');
        }),
        validation: validator,
        validate: function () {
            var isValid = vm.validation.isValid();

            vm.validation.errors.showAllMessages(!isValid);

            return isValid;
        },

        clearValidation: function () {
            vm.validation.errors.showAllMessages(false);
        },
        parseTest: function () {
            var test = vm.currentTest();

            return ko.toJS({
                AttendanceId: test.testId,
                JobId: test.jobId,
                TestDate: dateService.toPostDate(test.testDate()),
                Sat: test.testIsSat,
                TestVenueId: test.testEventVenueId,
                TestMaterialId: test.testMaterialId,
                DueDate: dateService.toPostDate(test.dueDate()),
                DueDateReview: dateService.toPostDate(test.dueDateReview())
            });
        },

        save: function () {

            if (!vm.tabOptions.component.validate()) {
                toastr.error(ko.Localization('Naati.Resources.Shared.resources.NotSaved'));
                return;
            }
            vm.tabOptions.component.save().done(
                function (resultsTab) {
                    loadTestData();
                    var allFulfilled = true;

                    resultsTab.forEach(function (result) {
                        allFulfilled &= result.state === 'fulfilled';
                    });

                    if (allFulfilled) {
                        toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                        vm.isDirty(false);
                    } else {
                        toastr.warning(ko.Localization('Naati.Resources.Shared.resources.PartiallySaved'));
                    }

                },
                function () {
                    loadTestData();
                    toastr.warning(ko.Localization('Naati.Resources.Shared.resources.PartiallySaved'));
                }
            );

        },

        hasJob: ko.pureComputed(function () {
            return vm.test.CurrentJobId && vm.test.CurrentJobId();
        }),
        testType: ko.pureComputed(function () {
            //TODO: DEFINE IF IT IS NEEDED
        })
    };

    $.extend(vm, {
        paidReviewInstance: paidReview.getInstance(vm.editMarksInstance, vm.confirmationInstance),
        resultInstance: result.getInstance(vm.editMarksInstance, vm.confirmationInstance),
        feedbackInstance: feedback.getInstance()
    });

    $.extend(vm, {
        passed: ko.pureComputed(function () {
            return vm.test.LastReviewTestResultId && vm.test.LastReviewTestResultId() ? vm.paidReviewInstance.passed() : vm.resultInstance.passed();
        }),
        failed: ko.pureComputed(function () {
            return vm.test.LastReviewTestResultId && vm.test.LastReviewTestResultId() ? vm.paidReviewInstance.failed() : vm.resultInstance.failed();
        })
    });

    $.extend(vm, {
        disableControls: ko.pureComputed(function () {

            return vm.loadingTest() || (vm.test.CredentialRequestStatusTypeId() !== enums.CredentialRequestStatusTypes.UnderPaidTestReview && vm.test.CredentialRequestStatusTypeId() !== enums.CredentialRequestStatusTypes.TestSat);
        }),
        unknownStatus: ko.pureComputed(function () {
            return vm.test.LastReviewTestResultId && vm.test.LastReviewTestResultId()
                ? vm.paidReviewInstance.testResult.ResultTypeId() === enums.TestResultType.NotKnown
                : vm.resultInstance.testResult.ResultTypeId() === enums.TestResultType.NotKnown;

        }),
        addExaminer: function () {

            var options = {
                test: vm.test
            }

            vm.addExaminerInstance
                .showAdd(options)
                .then(function () {
                    vm.addExaminerInstance.close();
                    loadTestData();
                });
        },
        editExaminer: function (data) {
            var options = {
                test: vm.test,
                jobExaminerId: data.JobExaminerId
            }
            vm.editExaminerInstance
                .showEdit(options)
                .then(function () {
                    loadTestData();
                });
        }
    });

    vm.disableControls.subscribe(vm.examinerInstance.disable);
    vm.testExaminers.subscribe(function () {
        vm.examinerInstance.loadExaminers(vm.test.TestAttendanceId(), vm.testExaminers, vm.test.ResultId() ? false : true);
        vm.examinerInstance.setNewDataTable(vm.test.CredentialTypeId(), vm.test.ResultId());
        if (vm.test && vm.test.ResultId && vm.test.ResultId()) {
            vm.resultInstance.load(vm.test, vm.test.ResultId(), vm.testExaminers);
        }
        if (vm.test.LastReviewTestResultId()) {
            vm.paidReviewInstance.load(vm.test, vm.test.LastReviewTestResultId(), vm.testExaminers, vm.resultInstance);
        }
    });

    vm.resultInstance.event.progress(resultProgress);
    vm.paidReviewInstance.event.progress(resultProgress);

    vm.test.TestAttendanceId.subscribe(function () {
        if (vm.test && vm.test.TestAttendanceId()) {
            vm.examinerFilesInstance.load(vm.test.TestAttendanceId());
        }
    });

    vm.examinerInstance.event.progress(function (progress) {
        if (progress.name === 'AddExaminer') {
            vm.addExaminer();
            return;
        }

        if (progress.name === 'EditExaminer') {
            vm.editExaminer(progress.data);
            return;
        }

        if (progress.name === 'ExaminerRemoved') {
            loadTestData();
        }

    });

    var compositionComplete = false;

    function processRequest() {
        var tabId = null;
        var addExaminer = null;

        if (queryString) {
            tabId = queryString['tab'];
            addExaminer = queryString['addExaminer'];
        }
        else {
            tabId = 'examiner';
        }

        if (!tabId) {
            tabId = 'examiner';
        }

        setTimeout(function () {
            $('.test-details .nav-tabs [aria-controls="' + tabId + '"]').tab('show');
        }, 0);

        if (tabId === 'examiner' && addExaminer && addExaminer.toLowerCase() === 'true') {
            vm.addExaminer();
        }
    }

    function loadTestData() {

        $('[href=#examiners]').trigger('click');

        var defer = Q.defer();
        vm.loadingTest(true);

        testService.getFluid(testServerModel.TestAttendanceId() + '/summary').then(function (data) {
            if (!data) {
                return defer.resolve(false);
            }

            ko.viewmodel.updateFromModel(testServerModel, data);

            vm.notesInstance.getNotes();
            vm.testAssetInstance.search();
            vm.generalDocumentsInstance.search();

            var feedbackTab = vm.tabOptions.tabs().find(function (element) {
                if (element.id == 'feedback') return true;
            });
            feedbackTab.icon('');

            vm.feedbackInstance.load(vm.test.TestAttendanceId(), feedbackTab.icon);

            personService.getFluid(testServerModel.NaatiNumber() + '/summary').then(function (data) {
                ko.viewmodel.updateFromModel(personServerModel, data);

                getExaminers().then(function (data) {
                    vm.testExaminers(data);
                    vm.testExaminersCount(data.length);
                    vm.loadingTest(false);
                    vm.isDirty(false);
                    defer.resolve(true);
                });
            });
        });

        return defer.promise;
    }

    function getExaminers() {
        var examinersDefer = Q.defer();
        examinerService.get({ request: JSON.stringify({ TestAttendanceIds: [vm.test.TestAttendanceId()], IncludeExaminerMarkings: true }) }).then(function (examinersdata) {
            examinersDefer.resolve(examinersdata);
        });
        return examinersDefer.promise;
    }

    function onTabClick(tabOption) {
        if (tabOption.disabled()) {
            return;
        }

        var testId = vm.test.TestAttendanceId();
        var tabId = tabOption.id;

        var currentUrl = window.location;
        var baseUrl = currentUrl.protocol + '//' + currentUrl.host + '/' + currentUrl.pathname.split('/')[1];
        var url = baseUrl + '#test/' + testId;

        url += '?tab=' + tabId;

        window.history.replaceState(null, document.title, url);
    }

    function resultProgress(progress) {
        if (progress.name === 'MarkSaved' || progress.name === 'reload' || progress.name === 'ResultSaved') {
            loadTestData();
        }
    }

    var supplementaryTestText = ko.Localization('Naati.Resources.Test.resources.SupplementaryTest');
    var personDetailsHeadingFormat = ko.Localization('Naati.Resources.Test.resources.PersonDetailsHeadingFormat');
    var registrationDate = ko.Localization('Naati.Resources.Shared.resources.RegistrationDate');
    var inactive = ko.Localization('Naati.Resources.Shared.resources.Inactive');
    var na = ko.Localization('Naati.Resources.Shared.resources.NotRegistered');
    var hasJobObservable = ko.pureComputed(function () {
        return !vm.test.CurrentJobId || !vm.test.CurrentJobId();
    });
    var testIsInvalidated = ko.pureComputed(function () {
        if (vm.test && vm.test.TestStatusTypeId()) {
            return vm.test.TestStatusTypeId() === enums.TestStatusType.TestInvalidated;
        }

        return false;
    });

    setupAssetDocument();
    setupTestDocument(vm.generalDocumentsInstance);

    testIsInvalidated.subscribe(function (value) {
        vm.examinerInstance.testIsInvalidated(value);
    });


    $.extend(vm, {
        canActivate: function (testId) {

            vm.notesInstance.allowAttachments(true);
            vm.testAssetInstance.resetfilecompnent();
            vm.testAssetInstance.cancel();

            queryString = {};

            testId = parseInt(testId);
            ko.viewmodel.updateFromModel(testServerModel, cleanTestModel);
            ko.viewmodel.updateFromModel(personServerModel, cleanPersonModel);
            testServerModel.TestAttendanceId(testId);

            if (!testId) {
                return false;
            }

            return loadTestData();
        },

        activate: function (testId, query) {

            queryString = query;

            if (compositionComplete) {
                processRequest();
            }
        },

        compositionComplete: function () {
            compositionComplete = true;
            processRequest();
        },

        personDetailsHeading: ko.pureComputed(function () {

            var personNaatiHearLink = '<a href="#/person/' +
                vm.person.NaatiNumber() +
                '?tab=contactDetails" target="_blank" class="text-info">' +
                vm.person.NaatiNumber() +
                '</a>&nbsp;';

            var title = personDetailsHeadingFormat.format(vm.person.GivenName() + ' ' + vm.person.SurName(),
                personNaatiHearLink + ' ',
                moment(vm.person.BirthDate()).format(CONST.settings.shortDateDisplayFormat));

            if (testServerModel.Supplementary()) {
                title += ' - ' + supplementaryTestText;
            }

            return title;
        }),

        personRegistrationStatus: ko.pureComputed(function () {
            var isEportalActive = vm.person.IsEportalActive();

            return isEportalActive != null
                ? isEportalActive
                    ? 'on'
                    : 'away'
                : 'busy';
        }),

        registrationStatusTitle: ko.pureComputed(function () {
            var isEportalActive = vm.person.IsEportalActive();

            return isEportalActive != null
                ? isEportalActive
                    ? registrationDate +
                    ': ' +
                    moment(vm.person.EportalRegistrationDate()).format(CONST.settings.shortDateDisplayFormat)
                    : inactive
                : na;
        }),

        testInvalidated: ko.computed(function () {
            return testIsInvalidated();
        }),

        tabOptions: {
            id: 'testEdit',
            tabs: tabController.generateTabs({
                tabs: [
                    {
                        active: ko.computed(function () {
                            if (testIsInvalidated()) {
                                if (vm.test.CurrentJobId() !== null && vm.testExaminersCount() === 0) {
                                    return false;
                                }
                            } else {
                                return true;
                            }
                        }),
                        id: 'examiner',
                        name: 'Naati.Resources.Test.resources.Examiner',
                        disabled: ko.computed(function () {
                            if (testIsInvalidated()) {
                                if (vm.test.CurrentJobId() !== null && vm.testExaminersCount() > 0) {
                                    return false;
                                } else {
                                    return true;
                                }
                            } else {
                                if (vm.test.CurrentJobId() !== null) {
                                    return false;
                                }
                            }

                            return false;
                        }),
                        type: 'compose',
                        composition: {
                            model: vm.examinerInstance,
                            view: 'views/test/edit-tab/examiner'
                        },
                        click: onTabClick
                    }, {
                        id: 'documents',
                        name: 'Naati.Resources.Test.resources.Documents',
                        roles: [{ noun: enums.SecNoun.Document, verb: enums.SecVerb.Read }],
                        active: ko.computed(function () {
                            if (testIsInvalidated()) {
                                if (vm.test.CurrentJobId() !== null && vm.testExaminersCount() === 0) {
                                    return true;
                                }
                            }
                        }),
                        disabled: ko.computed(function () {
                            if (testIsInvalidated()) {
                                if (vm.test.CurrentJobId() !== null && vm.testExaminersCount() > 0) {
                                    return false;
                                }
                            } else {
                                if (vm.test.CurrentJobId() !== null) {
                                    return false;
                                } else {
                                    return true;
                                }
                            }
                            return false;
                        }),
                        type: 'compose',
                        composition: {
                            model: vm.generalDocumentsInstance,
                            view: 'views/document/document-form'
                        },
                        click: onTabClick
                    }, {
                        id: 'results',
                        roles: [{ noun: enums.SecNoun.TestResult, verb: enums.SecVerb.Read }],
                        name: 'Naati.Resources.Test.resources.Results',
                        disabled: ko.computed(function () {
                            if (testIsInvalidated()) {
                                if (vm.test.CurrentJobId() !== null && vm.testExaminersCount() > 0) {
                                    return false;
                                } else {
                                    return true;
                                }
                            } else {
                                if (vm.test.CurrentJobId() !== null) {
                                    return false;
                                } else {
                                    return true;
                                }
                            }
                        }),
                        type: 'compose',
                        composition: {
                            model: vm.resultInstance,
                            view: 'views/test/edit-tab/result'
                        },
                        valid: vm.resultInstance.isValid,
                        validate: vm.resultInstance.validate,
                        clearValidation: vm.resultInstance.clearValidation,
                        save: vm.resultInstance.save,
                        click: onTabClick
                    }, {
                        id: 'assets',
                        active: ko.computed(function () {
                            return testIsInvalidated() && hasJobObservable();
                        }),
                        name: 'Naati.Resources.Test.resources.Assets',
                        roles: [{ noun: enums.SecNoun.Document, verb: enums.SecVerb.Read }],
                        type: 'compose',
                        composition: {
                            model: vm.testAssetInstance,
                            view: 'views/document/document-form-multi'
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
                        id: 'feedback',
                        name: 'Naati.Resources.Test.resources.Feedback',
                        roles: [{ noun: enums.SecNoun.Notes, verb: enums.SecVerb.Read }],
                        type: 'compose',
                        icon:  ko.observable(''),
                        composition: {
                            model: vm.feedbackInstance,
                            view: 'views/test/edit-tab/feedback.html'
                        },
                        click: onTabClick
                    },
                    {
                        id: 'paidReview',
                        name: 'Naati.Resources.Test.resources.PaidReview',
                        visible: ko.pureComputed(function () {
                            return vm.test.LastReviewTestResultId();
                        }),
                        type: 'compose',
                        composition: {
                            model: vm.paidReviewInstance,
                            view: 'views/test/edit-tab/paid-review'
                        },
                        valid: vm.paidReviewInstance.isValid,
                        validate: vm.paidReviewInstance.validate,
                        clearValidation: vm.paidReviewInstance.clearValidation,
                        save: vm.paidReviewInstance.save,
                        click: onTabClick
                    }, {
                        id: 'examinerFiles',
                        name: 'Naati.Resources.Test.resources.ExaminerFiles',
                        roles: [{ noun: enums.SecNoun.Document, verb: enums.SecVerb.Read }],
                        disabled: ko.computed(function () {
                            if (testIsInvalidated()) {
                                if (vm.test.CurrentJobId() !== null && vm.testExaminersCount() > 0) {
                                    return false;
                                } else {
                                    return true;
                                }
                            } else {
                                if (vm.test.CurrentJobId() !== null) {
                                    return false;
                                }
                            }
                            return false;
                        }),
                        type: 'compose',
                        composition: {
                            model: vm.examinerFilesInstance,
                            view: 'views/test/edit-tab/examiner-files'
                        },
                        click: onTabClick
                    }
                ]
            })
        },

        testHeaderOptions: {
            test: testServerModel
        }
    });

    vm.generalDocumentsInstance.types(['GeneralTestDocument']);

    vm.generalDocumentsInstance.tableDefinition.id = 'testGeneralDocuments';



    vm.close = function () {
        router.navigate('test/');
    };


    $.extend(vm.notesInstance, {
        getNotesPromise: function () {
            return testService.getFluid('notes/' + vm.test.TestAttendanceId());
        },
        postNotesPromise: function () {
            return testService.post($.extend(vm.notesInstance.parseNote(), {
                TestSittingId: vm.test.TestAttendanceId()
            }), 'notes');
        },
        removeNotesPromise: function (note) {
            return testService.remove('notes/' + note.NoteId + '/' + vm.test.TestAttendanceId());
        }
    });

    function highlightExaminerMarksRemovedOnAssets() {
        //TODO: check highlighting logic 
    }

    function setupAssetDocument() {
        vm.testAssetInstance.documents.subscribe(highlightExaminerMarksRemovedOnAssets);

        vm.testAssetInstance.types(['UnmarkedTestAsset', 'MarkedTestAsset', 'EnglishMarking', 'ReviewReport', 'TestMaterial', 'ProblemSheet', 'MedicalCertificate']);
        vm.testAssetInstance.showMergeDocument(false);
        vm.testAssetInstance.isTestAsset(true);
        vm.testAssetInstance.showEportalDownload(true);

        vm.testAssetInstance.parseFileName = function (options) {
            return commonFunctions.testAssetFileNameProcessor($.extend({}, options, { Test: vm.test }));
        };

        vm.testAssetInstance.tableDefinition.id = 'testAsset';

        vm.testAssetInstance.fileUpload.url = testresultService.url() + '/document/upload';
        vm.testAssetInstance.fileUpload.formData = ko.computed(function () {
            if (vm.testAssetInstance.inUploadMode()) {
                if (!vm.testAssetInstance.fileUpload.currentFile()) {
                    return null;
                }
                var foundDocument = ko.utils.arrayFirst(vm.testAssetInstance.currentDocuments(), function (item) {
                    return item.File() == vm.testAssetInstance.fileUpload.currentFile();
                });
                if (!foundDocument) {
                    return null;
                }
                return {
                    id: foundDocument.Id() || 0,
                    file: foundDocument.File(),
                    type: foundDocument.Type(),
                    testSittingId: vm.test.TestAttendanceId(),
                    title: foundDocument.Title(),
                    storedFileId: foundDocument.StoredFileId() || 0,

                    eportalDownload: foundDocument.EportalDownload()
                };
            }
            if (vm.testAssetInstance.inEditMode()) {
                return {
                    id: vm.testAssetInstance.currentDocument.Id() || 0,
                    file: vm.testAssetInstance.currentDocument.File(),
                    type: vm.testAssetInstance.currentDocument.Type(),
                    testSittingId: vm.test.TestAttendanceId(),
                    title: vm.testAssetInstance.currentDocument.Title(),
                    storedFileId: vm.testAssetInstance.currentDocument.StoredFileId() || 0,

                    eportalDownload: vm.testAssetInstance.currentDocument.EportalDownload()
                };
            }
            return null;
        });

        $.extend(vm.testAssetInstance, {
            getDocumentsPromise: function () {
                return testresultService.getFluid('documents/' + vm.test.TestAttendanceId());
            },
            postDocumentPromise: function () {
                var data = ko.toJS(vm.testAssetInstance.currentDocument);
                data.TestSittingId = vm.test.TestAttendanceId();
                data.eportalDownload = data.EportalDownload;
                return testresultService.post(data, 'document');
            },
            transformDocuments: function (data) {
                return ko.utils.arrayMap(data, function (d) {
                    var tmp = d.FileName.split('.');
                    d.FileType = tmp[tmp.length - 1];
                    d.UploadedByName = d.UploadedByPersonName || d.UploadedByUserName;
                    d.DocumentType = d.DocumentTypeDisplayName;
                    return d;
                });
            },
            removeDocumentPromise: function (document) {
                return fileService.remove(document.StoredFileId);
            }
        });
    }

    function selectAction(action, credentialRequest) {
        if (!vm.isDirty()) {
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

        if (validator.errors) {
            return validation.errors.showAllMessages(false);
        }
    }

    function takeAction(action) {
        var request = {
            ApplicationStatusId: testServerModel.ApplicationStatusTypeId(),
            ActionId: action.Id(),
            ApplicationId: testServerModel.ApplicationId(),
            CredentialRequestId: testServerModel.CredentialRequestId(),
            CredentialTypeId: testServerModel.CredentialTypeId(),
            ApplicationTypeId: testServerModel.ApplicationTypeId()
        };

        applicationService.post(request, 'action').then(function (data) {
            if (!checkAndShowMessages(data)) {
                applicationService.getFluid('steps', request).then(function (steps) {
                    if (steps.length) {
                        return router.navigate('application-wizard/' + testServerModel.ApplicationId() + '/' + action.Id() + '/' + testServerModel.CredentialRequestId());
                    }

                    applicationService.post(request, 'wizard').then(function () {
                        toastr.success(ko.Localization('Naati.Resources.Application.resources.ActionTakenSuccessfully'));
                        loadTestData().then(function (loaded) {
                            if (!loaded) {
                                router.navigate('test-session/test-sittings/' + testServerModel.TestSessionId());
                            }
                        });
                    });
                });
            }
        });
    }

    function setupTestDocument(documentInstance) {

        documentInstance.documents.subscribe(highlightExaminerMarksRemovedOnAssets);

        documentInstance.types(['GeneralTestDocument']);
        documentInstance.parseFileName = function (options) {
            return commonFunctions.testAssetFileNameProcessor($.extend({}, options, { Test: vm.test }));
        };

        documentInstance.tableDefinition.id = 'testDocument';

        documentInstance.fileUpload.url = testresultService.url() + '/document/upload';
        documentInstance.fileUpload.formData = ko.computed(function () {
            return {
                id: documentInstance.currentDocument.Id() || 0,
                file: documentInstance.currentDocument.File(),
                type: documentInstance.currentDocument.Type(),
                testSittingId: vm.test.TestAttendanceId(),
                title: documentInstance.currentDocument.Title(),
                storedFileId: documentInstance.currentDocument.StoredFileId() || 0,
                eportalDownload: documentInstance.eportalDownload()
            }
        });

        $.extend(documentInstance, {
            getDocumentsPromise: function () {
                return testresultService.getFluid('testDocuments/' + vm.test.TestAttendanceId());
            },
            postDocumentPromise: function () {
                var data = ko.toJS(documentInstance.currentDocument);
                data.TestSitingId = vm.test.TestAttendanceId();
                data.eportalDownload = documentInstance.eportalDownload();
                return testresultService.post(data, 'document');
            },
            transformDocuments: function (data) {
                return ko.utils.arrayMap(data, function (d) {
                    var tmp = d.FileName.split('.');
                    d.FileType = tmp[tmp.length - 1];
                    d.UploadedByName = d.UploadedByPersonName || d.UploadedByUserName;
                    d.DocumentType = d.DocumentTypeDisplayName;
                    return d;
                });
            },
            removeDocumentPromise: function (document) {
                return fileService.remove(document.StoredFileId);
            }
        });
    }
    return vm;
});

