define([
    'services/dataset-data-service',
    'services/audit-data-service',
    'services/screen/message-service',
    'services/screen/date-service',
    'views/test-material/request/edit/examiner-modal',
    'views/document/table',
    'views/shell',
    'modules/shared-filters'
], function (datasetService, auditService, message, dateService, examinerModalView, documentTable, shell, sharedFilters) {
    var defer = null;
    var chairSuffix = ko.Localization('Naati.Resources.Roles.resources.Chair');

    var vm = {
        jobId: ko.observable(),
        primaryContactRequired: ko.observable(false),
        dataTable: {
            buttons: {
                dom: {
                    button: {
                        tag: 'button',
                        className: ''
                    },
                    buttonLiner: {
                        tag: null
                    }
                },
                buttons: [
                    {
                        text: ko.Localization('Naati.Resources.TestMaterial.resources.AddExaminer'),
                        className: 'btn btn-primary',
                        action: addExaminer
                    }
                ]
            },
            columnDefs: [
                { targets: -1, orderable: false }
            ]
        },
        formatLog: formatLog,
        approved: ko.observable(),
        formatCurrency: formatCurrency,
        direction: ko.observable(),
        dataset: getDataSet(),
        jobMaterial: getJobMaterial(),
        mode: ko.observable(),
        disableMasterSettings: ko.pureComputed(function () {
            return vm.mode() === 'edit';
        }),
        documentsInstance: documentTable.getInstance(),
        edit: edit,
        remove: remove,
        dateService: dateService,
        cost: cost,
        save: save,
        logs: ko.observableArray(),
        examinerModalInstance: examinerModalView.getInstance(),
        getPersonName: getPersonName
    };

    var compositionComplete = false;
    var queryString;
    $.extend(vm, {
        activate: function (jobId, query) {
            defer = Q.defer();

            vm.jobId(jobId || 0);
            queryString = query;

            if (compositionComplete) {
                processRequest();
            }

            return loadJob();
        },
        compositionComplete: function () {
            compositionComplete = true;
            processRequest();
        },
        tabOptions: {
            id: 'materialEdit',
            tabs: ko.observableArray([{
                active: true,
                id: 'examiners',
                name: 'Naati.Resources.Test.resources.Examiner',
                template: {
                    name: 'examiners-tab-template',
                    data: {
                        dataTable: vm.dataTable,
                        examiners: vm.dataset.JobExaminer
                    }
                },
                click: onTabClick
            }, {
                id: 'documents',
                name: 'Naati.Resources.Test.resources.Documents',
                type: 'compose',
                composition: {
                    model: vm.documentsInstance,
                    view: 'views/document/document-form'
                },
                visible: ko.pureComputed(function () {
                    return vm.mode() === 'edit';
                }),
                click: onTabClick
            }, {
                id: 'history',
                name: 'Naati.Resources.Test.resources.History',
                template: {
                    name: 'history-tab-template',
                    data: {
                        logs: vm.logs
                    }
                },
                visible: ko.pureComputed(function () {
                    return vm.mode() === 'edit';
                }),
                click: onTabClick
            }])
        },
        languageOptions: $.extend({
            disable: vm.disableMasterSettings,
            value: vm.jobMaterial.LanguageId,
            multiple: false
        }, sharedFilters.getFilter('Language').componentOptions),
        categoryOptions: $.extend({
            disable: vm.disableMasterSettings,
            value: vm.jobMaterial.AccreditationCategoryId,
            multiple: false
        }, sharedFilters.getFilter('Category').componentOptions),
        levelOptions: $.extend({
            disable: vm.disableMasterSettings,
            value: vm.jobMaterial.AccreditationLevelId,
            multiple: false
        }, sharedFilters.getFilter('Level').componentOptions),
        directionOptions: $.extend({
            disable: vm.disableMasterSettings,
            value: vm.direction,
            multiple: false
        }, sharedFilters.getFilter('Direction').componentOptions),
        job: getJob(),
        dirtyFlag: new ko.DirtyFlag([vm.dataset.JobExaminer], false),
        hasPrimaryContact: ko.pureComputed(function () {
            return ko.utils.arrayFirst(vm.dataset.JobExaminer(), function (e) {
                return e.PrimaryContact();
            }) != null;
        })
    });

    function onTabClick(tabOption) {
        var jobId = vm.jobId();
        var tabId = tabOption.id;

        var currentUrl = window.location;
        var baseUrl = currentUrl.protocol + '//' + currentUrl.host + '/' + currentUrl.pathname.split('/')[1];
        var url = baseUrl + '#materialreq';

        if (jobId) {
            url += '/' + jobId;
        }

        url += '?tab=' + tabId;

        window.history.replaceState(null, document.title, url);
    }

    function updatePrimaryContactRequired() {
        vm.primaryContactRequired(!vm.hasPrimaryContact() && vm.dataset.JobExaminer().length);
    }

    vm.hasPrimaryContact.subscribe(updatePrimaryContactRequired);
    vm.dataset.JobExaminer.subscribe(updatePrimaryContactRequired);

    vm.documentsInstance.types(['TestMaterial']);
    vm.documentsInstance.tableDefinition.id = 'materialDocuments';

    vm.canSave = ko.computed(function () {
        return vm.dirtyFlag().isDirty() && vm.hasPrimaryContact();
    });

    vm.direction.subscribe(function (newValue) {
        if (newValue === 'B')
            vm.jobMaterial.ToEnglish(null);
        else if (newValue === 'O')
            vm.jobMaterial.ToEnglish(0);
        else if (newValue === 'E')
            vm.jobMaterial.ToEnglish(1);
    });

    vm.approved.subscribe(function (newValue) {
        var userName = null,
            userId = null,
            toPayrollDate = null;

        if (newValue) {
            var user = shell.user();
            userName = user.Name;
            userId = user.Id;
            toPayrollDate = moment().format();
        }

        vm.job.SentToPayrollDate(toPayrollDate);
        vm.job.SentToPayrollUser(userName);
        vm.job.SentToPayrollUserId(userId);

        $.each(vm.dataset.JobExaminer(), function (i, examiner) {
            examiner.ExaminerToPayrollDate = toPayrollDate,
            examiner.ExaminerToPayrollUser = userName;
            examiner.ExaminerToPayrollUserID = userId;
        });
    });

    var jobValidation = ko.validatedObservable(vm.job);
    var jobMaterialValidation = ko.validatedObservable(vm.jobMaterial);

    return vm;

    function cost() {
        var cost = 0;
        $.each(vm.dataset.JobExaminer(), function (i, e) {
            cost += parseFloat(e.ExaminerCost());
        });
        return cost;
    }

    function formatCurrency(value) {
        if (!value) return '';

        return '$' + parseFloat(Math.round(value * 100) / 100).toFixed(2);
    }

    function getDataSet() {
        return {
            JobExaminer: ko.observableArray(),
            Job: ko.observableArray(),
            JobMaterial: ko.observableArray()
        };
    }

    function save() {
        vm.job.JobCost(cost());

        var jobValid = jobValidation.isValid();
        var jobMaterialValid = jobMaterialValidation.isValid();

        if (!jobValid) {
            jobValidation.errors.showAllMessages();
        }

        if (!jobMaterialValid) {
            jobMaterialValidation.errors.showAllMessages();
        }

            if (vm.dataset.JobExaminer().length === 0) {
                toastr.warning(ko.Localization('Naati.Resources.TestMaterial.resources.YouMustAddAtLeastOneExaminer'));
                return;
            }

            $.each(vm.dataset.JobExaminer(), function(key, value) {
                if (value.PrimaryContact() === null) {
                    value.PrimaryContact(true);
                }
            });

        if (!jobValid || !jobMaterialValid) {
            return;
        }

        var jobRequest = ko.toJS(vm.job);
        var jobMaterialRequest = ko.toJS(vm.jobMaterial);

        var job = vm.dataset.Job()[0];
        var jobMaterial = vm.dataset.JobMaterial()[0];

        jobRequest.JobLost = 0;
        jobRequest.Note = '';
        jobRequest.DueDate = dateService.toPostDate(jobRequest.DueDate);
        jobRequest.LanguageId = jobMaterialRequest.LanguageId;

        $.extend(jobMaterial, jobMaterialRequest);
        $.extend(job, jobRequest);

        var json = ko.toJS(vm.dataset);
        json.KeyAllocationConfiguration = [{
            DataSetName: 'JobMaterial',
            TableName: 'TestMaterial'
        }];

        datasetService.post(json, 'dsJob/' + vm.jobId()).then(function () {
            toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
            loadJob();
        });
    }

    function processRequest() {
        var tabId = null;

        if (queryString) {
            tabId = queryString['tab'];
        }
        else {
            tabId = 'examiners';
        }

        if (!tabId) {
            tabId = 'examiners';
        }

        setTimeout(function () {
            $('.test-material-details .nav-tabs [aria-controls="' + tabId + '"]').tab('show');
        }, 0);
    }

    function getPersonName(e) {
        var personName = e.PersonName();
        var suffix = [];

        if (moment(e.EndDate()) < moment()) {
            suffix.push(ko.Localization('Naati.Resources.Shared.resources.Inactive'));
        }

        if (e.IsChair()) {
            suffix.push(chairSuffix);
        }

        if (suffix.length) {
            personName += ' (' + suffix.join(' | ') + ')';
        }

        return personName;
    }

    function edit(e) {
        vm.examinerModalInstance.showEdit(e).then(function (e) {
            var addedExaminer = $.grep(vm.dataset.JobExaminer(), function (je) {
                return je.EntityId() === e.EntityId;
            });

            if (addedExaminer.length > 0) {
                ko.viewmodel.updateFromModel(addedExaminer[0], e);
            }
        });
    }

    function addExaminer() {
        if (!vm.jobMaterial.LanguageId()) {
            return toastr.warning(ko.Localization('Naati.Resources.TestMaterial.resources.SelectAnExaminer'));
        }

        vm.examinerModalInstance.showAdd(vm.jobMaterial.LanguageId()).then(function (e) {
            var addedExaminer = $.grep(vm.dataset.JobExaminer(), function (je) {
                return je.EntityId() === e.EntityId;
            });

            if (addedExaminer.length > 0) {
                return ko.viewmodel.updateFromModel(addedExaminer[0], e);
            }

            var examiners = vm.dataset.JobExaminer();
            vm.dataset.JobExaminer([]);

            var examiner = {
                DateAllocated: moment().format(),
                EndDate: null,
                EntityId: null,
                ExaminerCost: null,
                ExaminerPaperLost: false,
                ExaminerReceivedDate: null,
                ExaminerReceivedUser: null,
                ExaminerReceivedUserID: null,
                ExaminerSentDate: moment().format(),
                ExaminerSentUser: null,
                ExaminerSentUserID: null,
                ExaminerToPayrollDate: null,
                ExaminerToPayrollUser: null,
                ExaminerToPayrollUserID: null,
                JobExaminerID: 0,
                JobId: vm.jobId(),
                LetterRecipient: false,
                NAATINumber: null,
                NaatiNumberDisplay: null,
                PanelMembershipId: null,
                PersonName: null,
                ThirdExaminer: false,
                IsChair: false,
                PrimaryContact: false
            };

            if (vm.approved()) {
                var user = shell.user();

                examiner.ExaminerToPayrollDate = vm.job.SentToPayrollDate(),
                examiner.ExaminerToPayrollUser = user.Name;
                examiner.ExaminerToPayrollUserID = user.Id;
            }

            for (var p in e) {
                if (p in examiner) {
                    examiner[p] = e[p];
                }
            }

            examiners.push(ko.viewmodel.fromModel(examiner));
            vm.dataset.JobExaminer(examiners);
        });
    }

    function getJob() {
        return {
            SentToPayrollDate: ko.observable(),
            SentToPayrollUser: ko.observable(),
            SentToPayrollUserId: ko.observable(),
            DueDate: ko.observable().extend({
                required: {
                    onlyIf: function () { return !vm.disableMasterSettings(); }
                },
                dateGreaterThan: {
                    params: moment().add(1, 'days').format('l'),
                    message: ko.Localization('Naati.Resources.TestMaterial.resources.DateDueGreatThanValidation'),
                    onlyIf: function () { return !vm.disableMasterSettings(); }
                }
            }),
            JobCost: ko.observable().extend({ required: true })
        };
    }

    function loadJob() {
        vm.mode(vm.jobId() == 0 ? 'new' : 'edit');

        $('[href=#examiners]').trigger('click');

        ko.viewmodel.updateFromModel(vm.job, ko.toJS(getJob()));
        ko.viewmodel.updateFromModel(vm.jobMaterial, ko.toJS(getJobMaterial()));
        ko.viewmodel.updateFromModel(vm.dataset, ko.toJS(getDataSet()));

        jobValidation.errors.showAllMessages(false);
        jobMaterialValidation.errors.showAllMessages(false);

        if (vm.jobId() === 0) {
            datasetService.getFluid('Job/next').then(function (jobId) {
                datasetService.getFluid('TestMaterial/next').then(function (testMaterialId) {
                    vm.jobId(jobId);

                    var data = {
                        Job: [{
                            DueDate: null,
                            InitialJobId: null,
                            JobCategory: 0, //Setting
                            JobCost: null,
                            JobId: jobId,
                            JobLost: null,
                            JobType: null,
                            LanguageId: null,
                            Name: '',
                            Note: '',
                            NumberOfPapers: null,
                            ReceivedDate: null,
                            ReceivedUser: null,
                            ReceivedUserId: null,
                            ReviewFromJobId: null,
                            RowVersion: null,
                            SecondJobId: null,
                            SentDate: moment().format(),
                            SentToPanelMembershipId: null,
                            SentToPayrollDate: null,
                            SentToPayrollUser: null,
                            SentToPayrollUserId: null,
                            SentUser: null,
                            SentUserID: null,
                            SettingMaterialId: testMaterialId,
                            WorkshopId: null,
                            WorkshopName: null
                        }],
                        JobMaterial: [{
                            AccreditationCategoryId: null,
                            AccreditationLevelId: null,
                            Available: true,
                            Description: 'Description',
                            LanguageId: null,
                            Path: 'Path',
                            TestMaterialId: testMaterialId,
                            ToEnglish: null
                        }]
                    };

                    bindViewModel(data);
                });
            });
            vm.logs([]);
        }
        else {
            datasetService.getFluid('dsJob/' + vm.jobId()).then(bindViewModel);
            auditService.get({ RecordName: 'Job', RecordId: vm.jobId(), PageSize: 1000, PageNumber: 1 }).then(function (data) {
                vm.logs(data);
            });
        }
    }

    function formatLog(log) {
        if (!log) return;

        var tmp = log.split('Changed values:');

        if (tmp.length === 1) {
            tmp = tmp[0].split('New values:');
        }

        tmp = tmp[1].split(';');

        var newTmp = $.map(tmp, function (t) {
            var s = t.split('=');
            if (s.length > 1)
                return '<b>' + s[0] + ': </b>' + s[1];

            s = t.split('changed from');
            if (s.length > 1)
                return '<b>' + s[0] + '</b> changed from ' + s[1];

            return t;
        });

        return newTmp.join('<br/>');
    }

    function bindViewModel(data) {
        var job = data.Job[0];
        var jobMaterial = data.JobMaterial[0];

        var jobJson = {
            SentToPayrollDate: job.SentToPayrollDate,
            SentToPayrollUser: job.SentToPayrollUser,
            SentToPayrollUserId: job.SentToPayrollUserId,
            DueDate: dateService.toUIDate(job.DueDate),
            JobCost: job.JobCost
        };

        var jobJsonMaterial = {
            LanguageId: jobMaterial.LanguageId,
            AccreditationCategoryId: jobMaterial.AccreditationCategoryId,
            AccreditationLevelId: jobMaterial.AccreditationLevelId,
            ToEnglish: jobMaterial.ToEnglish,
            RequestStatus: jobMaterial.RequestStatus
        };

        if (!jobJsonMaterial.ToEnglish)
            vm.direction('B');
        else if (jobJsonMaterial.ToEnglish === 0)
            vm.direction('O');
        else if (jobJsonMaterial.ToEnglish === 1)
            vm.direction('E');

        vm.approved(jobJsonMaterial.RequestStatus === 'Approved');

        ko.viewmodel.updateFromModel(vm.job, jobJson);
        ko.viewmodel.updateFromModel(vm.jobMaterial, jobJsonMaterial);
        ko.viewmodel.updateFromModel(vm.dataset, data);

        jobValidation.errors.showAllMessages(false);
        jobMaterialValidation.errors.showAllMessages(false);

        vm.documentsInstance.relatedId(job.SettingMaterialId);
        vm.documentsInstance.search();

        vm.dirtyFlag().reset();
    }

    function getJobMaterial() {
        return {
            LanguageId: ko.observable().extend({ required: true }),
            AccreditationCategoryId: ko.observable().extend({ required: true }),
            AccreditationLevelId: ko.observable().extend({ required: true }),
            ToEnglish: ko.observable(),
            RequestStatus: ko.observable()
        };
    }

    function remove(examiner) {
        message.confirm({
            title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
            content: ko.Localization('Naati.Resources.Shared.resources.ThisRecordWillBeDeleted')
        })
            .then(
                function (answer) {
                    if (answer === 'yes') {
                        vm.dataset.JobExaminer.remove(function (e) {
                            return e.EntityId() === examiner.EntityId();
                        });
                    }
                });
    }
});
