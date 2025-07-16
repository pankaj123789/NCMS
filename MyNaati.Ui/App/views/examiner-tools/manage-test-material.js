define([
    'services/requester',
    'services/util'
],
    function (requester, util) {

        var validateDomain = function (val) {
            if (!vm || vm.showSaveParticipants()) {
                return true;
            }
            else {
                return val != ENUMS.TestMaterialDomain.Unspecified;
            }
        };

        var examinerToolsRequester = requester('ExaminerTools');
        var defaultLink = 'https://';
        var httpRegex = new RegExp("^(http|https)://", "i");

        var preventSave = false;
        var serverModel = {
            MaterialRequestId: ko.observable(),
            TestMaterialId: ko.observable(),
            Title: ko.observable(),
            Round: ko.observable(),
            RoundId: ko.observable(),
            RoundStatusId: ko.observable(),
            RoundNumber: ko.observable(),
            RoundStatusName: ko.observable(),
            CredentialTypeExternalName: ko.observable(),
            CredentialTypeId: ko.observable(),
            TestMaterialDomainId: ko.observable().extend({
                required: true,
                validation: {
                    validator: validateDomain,
                    message: 'Please select a valid domain.'
                }
            }),
            PanelId: ko.observable(),
            DueDate: ko.observable(),
            MaterialRequestCoordinatorLoadingPercentage: ko.observable(),
            ProductSpecificationCostPerUnit: ko.observable(),
            Members: ko.observableArray(),
            AvailableDocumentTypes: ko.observable(),
            MaterialRequestTaskTypes: ko.observable(),
            DocumentType: ko.observable(),
            MaxBillableHours: ko.observable(),
            TaskTypeName: ko.observable()
        };

        var vm = {
            materialRequest: serverModel,
            examinerToolsLink: window.baseUrl + "ExaminerTools",
            manageTestMaterialsLink: window.baseUrl + "ExaminerTools/ManageTestMaterials",
            homeLink: window.baseUrl,
            loaded: ko.observable(false),
            applicationId: ko.observable(),
            title: ko.observable('My Test Material Development Details'),
            files: ko.observableArray(),
            link: ko.observable().extend({ url: true }),
            links: ko.observableArray(),
            note: ko.observable(),
            notes: ko.observableArray(),
            allMembers: ko.observableArray(),
            previousMembers: ko.observableArray(),
            newParticipants: ko.observableArray(),
            domains: ko.observableArray(),
            submitted: ko.observable(false),
            processing: ko.observable(),
            isCoordinator: ko.observable(false),
            coordinator: ko.observable(),
            showSaveParticipants: ko.observable(false),
            dropzone: {
                url: examinerToolsRequester.url() + '/MaterialRequestRoundAttachment',
                previewTemplate: '\
                    <div class="row v-offset-xs-3">\
                        <div class="col-sm-2">\
                            <span class="preview"><img data-dz-thumbnail /></span>\
                        </div>\
                        <div class="col-sm-4">\
                            <p class="name" data-dz-name></p>\
                            <strong class="error text-danger" data-dz-errormessage></strong>\
                        </div>\
                        <div class="col-sm-6">\
                            <div class="row">\
                                <div class="col-sm-4">\
                                    <p class="size" data-dz-size></p>\
                                </div>\
                                <div class="col-sm-7">\
                                    <div class="progress progress-striped active" role="progressbar" aria-valuemin="0" aria-valuemax="100" aria-valuenow="0">\
                                        <div class="progress-bar progress-bar-success hide" style="width:0;" data-dz-uploadprogress></div>\
                                    </div>\
                                </div>\
                            </div>\
                        </div>\
                    </div>',
                previewsContainer: '#previews',
                params: {
                    id: null
                },
                parallelUploads: 20,
                init: function () {
                    this.on('processing', function () {
                        this.options.params.id = serverModel.RoundId();
                        this.options.params.type = serverModel.DocumentType();
                    });

                    this.on('queuecomplete', function () {
                        $('#previews .dz-complete').remove();
                        loadMaterialTable();
                        $('#addFileModal').modal('hide');
                    });
                }
            },
            disableTableFeatures: {
                paging: false,
                info: false,
                dom: "<'row'<'col-sm-12'B>>" +
                    "<'row v-offset-xs-1'<'col-sm-12'<'table-responsive't>>>" +
                    "<'row'<'col-sm-4'l><'col-sm-3'i><'col-sm-5'p>>"
            }
        };

        var validation = ko.validatedObservable(serverModel);
        var linkValidation = ko.validatedObservable([vm.link]);

        vm.readOnly = ko.computed(function () {
            return serverModel.RoundStatusId() != ENUMS.MaterialRequestRoundStatusType.SentForDevelopment;
        });

        vm.isFinalised = ko.computed(function () {
            return serverModel.RoundStatusId() == ENUMS.MaterialRequestRoundStatusType.Finalised;
        });

        vm.isMemberReadOnly = ko.computed(function () {
            return serverModel.RoundStatusId() !== ENUMS.MaterialRequestRoundStatusType.SentForDevelopment ||
                serverModel.RoundNumber() > 1;
        });

        vm.showNotesTab = ko.computed(function () {
            return !vm.readOnly() || vm.notes().length;
        });

        vm.maxTasks = ko.computed(function () {
            return serverModel.MaterialRequestTaskTypes() ? serverModel.MaterialRequestTaskTypes().length : 0;
        });

        ko.computed(function () {
            if (!vm.domains()) {
                return;
            }

            var domain = ko.utils.arrayFirst(vm.domains(), function (d) {
                return d.Id == serverModel.TestMaterialDomainId();
            });

            if (!domain) {
                serverModel.TestMaterialDomainId(ENUMS.TestMaterialDomain.Unspecified);
            }
        });

        vm.domainOptions = {
            value: serverModel.TestMaterialDomainId,
            multiple: false,
            options: vm.domains,
            disable: ko.computed(function () {
                return vm.readOnly() || !vm.isCoordinator();
            }),
            optionsValue: 'Id',
            optionsText: 'DisplayName'
        };

        vm.tab = ko.computed(function () {
            if (!vm.readOnly()) {
                return 'files';
            }
            if (vm.isCoordinator()) {
                return 'participants';
            }
            return 'notes';
        });

        vm.availableMembers = ko.computed(function () {
            return ko.utils.arrayFilter(vm.allMembers(), function (m) {
                var found = ko.utils.arrayFirst(serverModel.Members(), function (sm) {
                    return m.Id == sm.PanelMemberShipId();
                });
                return !found;
            });
        });

        vm.dueDateDisplay = ko.computed(function () {
            var dueDate = serverModel.DueDate();

            if (!dueDate) {
                return "";
            }

            return moment(new Date(parseInt(dueDate.substr(6)))).format();
        });

        vm.roundStatusDisplayName = ko.computed(function () {
            var status = serverModel.RoundStatusId();
            var roundStatusName = serverModel.RoundStatusName();

            if (status == ENUMS.MaterialRequestRoundStatusType.SentForDevelopment) {
                roundStatusName = "Awaiting Development";
            }
            else if (status == ENUMS.MaterialRequestRoundStatusType.AwaitingAproval) {
                roundStatusName = "Awaiting NAATI’s Approval";
            }

            return roundStatusName;
        });

        vm.addNote = function () {
            var req = {
                MaterialRequestId: serverModel.MaterialRequestId(),
                Note: vm.note()
            };

            examinerToolsRequester.post(req, 'materialrequestnote').then(function (data) {
                vm.note(null);
                loadNotes();
            });
        };

        vm.roundStatusCss = ko.computed(function () {
            var statusClass = "";
            var status = serverModel.RoundStatusId();

            if (status == ENUMS.MaterialRequestRoundStatusType.SentForDevelopment) {
                statusClass = "dark-yellow";
            }
            else if (status == ENUMS.MaterialRequestRoundStatusType.AwaitingAproval) {
                statusClass = "orange";
            }
            else if (status == ENUMS.MaterialRequestRoundStatusType.Rejected) {
                statusClass = "danger";
            }
            else if (status == ENUMS.MaterialRequestRoundStatusType.Approved) {
                statusClass = "success";
            }
            else if (status == ENUMS.MaterialRequestRoundStatusType.Cancelled) {
                statusClass = "purple";
            }

            return statusClass;
        });

        vm.productSpecificationCostPerUnit = ko.computed(function () {
            return ko.textCurrencyFormat(serverModel.ProductSpecificationCostPerUnit());
        });

        vm.totalAmount = ko.computed(function () {
            var totalAmount = 0;
            if (!serverModel.Members()) {
                return totalAmount;
            }

            ko.utils.arrayForEach(serverModel.Members(), function (m) {
                ko.utils.arrayForEach(m.Tasks(), function (t) {
                    totalAmount += (serverModel.ProductSpecificationCostPerUnit() || 0) * parseFloat(t.HoursSpent() || '0');
                });
            });

            return totalAmount;
        });

        vm.totalAmountCurrency = ko.computed(function () {
            return ko.textCurrencyFormat(vm.totalAmount());
        });

        vm.numberOfHours = ko.computed(function () {
            var numberOfHours = 0;
            if (!serverModel.Members()) {
                return numberOfHours;
            }

            ko.utils.arrayForEach(serverModel.Members(), function (m) {
                ko.utils.arrayForEach(m.Tasks(), function (t) {
                    numberOfHours += parseFloat(t.HoursSpent() || '0');
                });
            });

            return numberOfHours;
        });

        vm.coordinatorLoadingPercent = ko.computed(function () {
            return serverModel.MaterialRequestCoordinatorLoadingPercentage() + '% of ' + vm.totalAmountCurrency();
        });

        vm.coordinatorLoadingTotalAmount = ko.computed(function () {
            return ko.textCurrencyFormat(vm.totalAmount() * serverModel.MaterialRequestCoordinatorLoadingPercentage() / 100);
        });

        vm.enableSaveParticipant = ko.computed(function () {
            return vm.newParticipants() && vm.newParticipants().length;
        });

        vm.loaderOptions = {
            name: 'loader',
            params: {
                show: ko.observable(true),
                text: ko.observable('Loading material request...')
            }
        };

        vm.activate = function () {
            var pathArray = window.location.pathname.toUpperCase().split('/MANAGETESTMATERIAL/');
            var materialRequestId = pathArray[pathArray.length - 1];
            preventSave = true;
            examinerToolsRequester.getFluid('managetestmaterialinfo/' + materialRequestId).then(function (data) {
                vm.loaderOptions.params.show(false);
                var members = data.Members;
                delete data.Members;
                ko.viewmodel.updateFromModel(serverModel, data);
                serverModel.Members(ko.utils.arrayMap(members, function (m) {
                    return prepareMember(m);
                }));
                vm.previousMembers.removeAll();
                ko.utils.arrayForEach(serverModel.Members(),
                    function (serverMember) {
                        vm.previousMembers.push(serverMember);
                    });

                var isCoordinator = ko.utils.arrayFirst(serverModel.Members(), function (m) {
                    return m.MemberTypeId() == ENUMS.MaterialRequestPanelMembershipType.Coordinator && m.NaatiNumber() == window.currentUser.NaatiNumber;
                });

                vm.isCoordinator(isCoordinator);
                if (isCoordinator) {
                    vm.coordinator(isCoordinator.GivenName());
                }

                loadMaterialTable();
                loadNotes();
                loadLinks();
                examinerToolsRequester.getFluid('managetestmaterialmembers/' + serverModel.PanelId() + '/' + serverModel.CredentialTypeId() + '/' + serverModel.MaterialRequestId()).then(vm.allMembers);
                examinerToolsRequester.getFluid('materialrequestdomains/' + serverModel.CredentialTypeId()).then(vm.domains);
                clearValidation();
                preventSave = false;
            });
        };

        vm.addFile = function () {
            $('#addFileModal').modal('show');
        };

        vm.addParticipant = function () {
            vm.newParticipants([]);
            var $chosenContainer = $('.chosen-container');
            $chosenContainer.addClass('invisible');

            $('#addParticipantModal')
                .off('shown.bs.modal')
                .on('shown.bs.modal', function () {
                    var $chosen = $('.chosen');
                    $chosen.chosen();
                    $chosen.removeClass('invisible');
                    $chosen.parent().find('.chosen-choices').addClass('chosen-auto-height');
                    $chosen.trigger("chosen:updated");
                    $chosenContainer.removeClass('invisible');
                })
                .modal('show');
        };

        vm.deleteAttachment = function (attachment) {
            mbox.remove().then(function (answer) {
                if (answer == 'yes') {
                    examinerToolsRequester.remove(attachment.MaterialRequestRoundAttachmentId + '/' + serverModel.RoundId(), 'MaterialRequestRoundAttachment').then(function () {
                        loadMaterialTable();
                    });
                }
            });
        };

        vm.downloadAttachment = function (attachment) {
            var $downloadForm = $('#downloadForm');
            var action = examinerToolsRequester.url() + '/MaterialRequestRoundAttachment/' + attachment.MaterialRequestRoundAttachmentId + '/' + serverModel.RoundId();

            $downloadForm.attr('action', action);
            $downloadForm.submit();
        };

        vm.uploadAttachment = function () {
            vm.dropzone.component.processQueue();
            $('[data-dz-uploadprogress]').removeClass('hide');
        };

        vm.stackParticipant = function () {
            preventSave = true;
            var newMembers = ko.utils.arrayMap(vm.newParticipants(), function (m) {
                var member = ko.utils.arrayFirst(vm.availableMembers(), function (am) {
                    return am.Id == m;
                });

                var newMember = prepareMember({
                    PanelMemberShipId: m,
                    GivenName: member.Name,
                    MemberTypeId: ENUMS.MaterialRequestPanelMembershipType.Collaborator
                });

                return newMember;
            });

            vm.showSaveParticipants(newMembers.length > 0);
            var members = serverModel.Members();
            serverModel.Members([]);
            Array.prototype.push.apply(members, newMembers);
            serverModel.Members(members);

            $('#addParticipantModal').modal('hide');
            preventSave = false;
        };

        vm.commitNewParticipants = function () {
            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return;
            }

            //check if any members have been added or deleted
            var membersChanged = false;
            ko.utils.arrayForEach(vm.previousMembers(),
                function (previousMember) {
                    if (!ko.utils.arrayFirst(serverModel.Members(),
                        function (serverMember) {
                            return serverMember.Id === previousMember.Id;
                        })) {
                        membersChanged = true;
                    }
                });

            ko.utils.arrayForEach(serverModel.Members(),
                function (serverMember) {
                    if (!ko.utils.arrayFirst(vm.previousMembers(),
                        function (previousMember) {
                            return serverMember.Id === previousMember.Id;
                        })) {
                        membersChanged = true;
                    }
                });

            if (membersChanged) {
                mbox.confirm({
                    content: 'An email will be sent to the newly added or removed participant(s). Are you sure?',
                    title: 'Warning',
                    no: 'No, I will make other changes'
                }).then(
                    function (argument) {
                        if (argument != 'yes') {
                            return;
                        } else {
                            actualSaveMembers();
                        }
                    });
            } else {
                actualSaveMembers();
            }
        };

        vm.addTask = function (member) {
            preventSave = true;
            var newTask = prepareTask({
                MaterialRequestTaskTypeId: ko.observable(),
                HoursSpent: ko.observable(0),
            });
            addAvailableTasks(member, newTask);
            member.Tasks.push(newTask);
            newTask.MaterialRequestTaskTypeId(newTask.AvailableTasks()[0].Key);
            preventSave = false;

            saveMembers();
        };

        vm.removeTask = function (member, task) {
            preventSave = true;
            member.Tasks.remove(task);
            if (!member.Tasks().length) {
                var members = ko.observableArray(serverModel.Members());
                serverModel.Members([]);
                members.remove(member);
                serverModel.Members(members());
            }
            preventSave = false;
            saveMembers();
        };

        vm.submitForApproval = function () {
            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return;
            }
            if (serverModel.TestMaterialDomainId() == ENUMS.TestMaterialDomain.Unspecified) {
                alert("no");
                return; 
            }

            vm.processing(true);
            vm.loaderOptions.params.text('Submitting for approval...');
            vm.loaderOptions.params.show(true);

            examinerToolsRequester.post(null, 'materialrequestsubmitforapprovalvalidation/' + serverModel.MaterialRequestId() + '/' + serverModel.RoundId() + '/' + serverModel.TestMaterialDomainId()).then(function (data) {
                vm.loaderOptions.params.show(false);
                vm.processing(false);
                if (data) {
                    return mbox.confirm({
                        content: data,
                        title: 'Warning: Insufficient details for submission',
                        no: 'No',
                        yes: 'Yes'
                    }).then(function (argument) {
                        if (argument === 'yes') {
                            examinerToolsRequester.post(null,
                                'materialrequestsubmitforapproval/' +
                                serverModel.MaterialRequestId() +
                                '/' +
                                serverModel.RoundId() +
                                '/' +
                                serverModel.TestMaterialDomainId()).then(function (data) {
                                    if (data) {
                                        return mbox.alert({
                                            title: 'Failed to submit material development for approval',
                                            content: data
                                        });
                                    }
                                    vm.title('Material Request Submitted');
                                    vm.submitted(true);
                                });
                        }
                    });
                } else {
                    examinerToolsRequester.post(null,
                        'materialrequestsubmitforapproval/' +
                        serverModel.MaterialRequestId() +
                        '/' +
                        serverModel.RoundId() +
                        '/' +
                        serverModel.TestMaterialDomainId()).then(function (data) {
                            if (data) {
                                return mbox.alert({
                                    title: 'Failed to submit material development for approval',
                                    content: data
                                });
                            }
                            vm.title('Material Request Submitted');
                            vm.submitted(true);
                        });
                }
            });
        };

        vm.addLink = function () {
            if (!linkValidation.isValid()) {
                linkValidation.errors.showAllMessages();
                return;
            }

            if (!httpRegex.test(vm.link())) {
                vm.link('http://' + vm.link());
            }

            var req = {
                MaterialRequestRoundId: serverModel.RoundId(),
                Link: vm.link()
            };

            examinerToolsRequester.post(req, 'materialrequestlinks').then(function () {
                loadLinks();
                vm.link(null);
                clearLinkValidation();
            });
        };

        vm.removeLink = function (link) {
            mbox.remove().then(function (answer) {
                if (answer == 'yes') {
                    examinerToolsRequester.remove('materialrequestlinks/' + serverModel.RoundId() + '/' + link.Id).then(function () {
                        loadLinks();
                    });
                }
            });
        };

        vm.linkFocus = function () {
            if (!vm.link()) {
                vm.link(defaultLink);
                clearLinkValidation();
            }
        };

        vm.linkFocusOut = function () {
            if (vm.link() == defaultLink) {
                vm.link(null);
                clearLinkValidation();
            }
        };

        function actualSaveMembers() {
            vm.processing(true);
            vm.loaderOptions.params.text('Saving changes...');
            vm.loaderOptions.params.show(true);
            examinerToolsRequester
                .post(ko.toJS(serverModel),
                    'materialrequestupdatemembers/' +
                    serverModel.MaterialRequestId() +
                    '/' +
                    serverModel.RoundId() +
                    '/' +
                    (serverModel.TestMaterialDomainId() || 0)).then(function (data) {
                        vm.loaderOptions.params.show(false);
                        vm.processing(false);
                        if (data) {
                            return mbox.alert({ title: 'Failed to save participants', content: data });
                        }
                        mbox.alert({ title: 'Success', content: 'Participants saved' });
                        vm.showSaveParticipants(false);

                        vm.previousMembers.removeAll();
                        ko.utils.arrayForEach(serverModel.Members(),
                            function (serverMember) {
                                vm.previousMembers.push(serverMember);
                            });
                    });
            clearValidation();
        }

        function loadNotes() {
            examinerToolsRequester.getFluid('materialrequestnote/' + serverModel.MaterialRequestId()).then(function (data) {
                ko.utils.arrayForEach(data, function (d) {
                    var date = new Date(parseInt(d.CreatedDate.substr(6)));
                    d.DateHumanized = util.humanizeDate(date);
                });
                vm.notes(data);
            });
        }

        function addAvailableTasks(member, task) {
            var otherTasks = filterOtherTasks(member, task);
            task.AvailableTasks = ko.computed(function () {
                return ko.utils.arrayFilter(serverModel.MaterialRequestTaskTypes(), function (t) {
                    return !t.Key || ko.utils.arrayFirst(otherTasks(), function (otherTask) {
                        return t.Key === otherTask;
                    }) == null;
                });
            });
        }

        function filterOtherTasks(member, newTask) {
            return ko.computed(function () {
                return ko.utils.arrayMap(member.Tasks(), function (t) {
                    if (t === newTask) {
                        return null;
                    }
                    return t.MaterialRequestTaskTypeId();
                });
            });
        }

        function prepareMember(member) {
            if (!member.Tasks || !member.Tasks.length) {
                member.Tasks = [{
                    MaterialRequestTaskTypeId: null,
                    HoursSpent: 0
                }];
            }

            var newMember = ko.viewmodel.fromModel(member);
            newMember.IsCoordinator = ko.computed(function () {
                return newMember.MemberTypeId() == ENUMS.MaterialRequestPanelMembershipType.Coordinator;
            });

            ko.utils.arrayForEach(newMember.Tasks(), function (t) {
                prepareTask(t);
                addAvailableTasks(newMember, t);
                if (!t.MaterialRequestTaskTypeId()) {
                    t.MaterialRequestTaskTypeId(t.AvailableTasks()[0].Key);
                }
            });

            return newMember;
        }

        function prepareTask(task) {
            task.TotalAmount = ko.computed(function () {
                return ko.textCurrencyFormat(task.HoursSpent() * serverModel.ProductSpecificationCostPerUnit());
            });
            task.HoursSpent.subscribe(saveMembers);
            task.MaterialRequestTaskTypeId.subscribe(saveMembers);
            return task;
        }

        function loadMaterialTable() {
            examinerToolsRequester.getFluid('materialrequestroundattachments/' + serverModel.RoundId()).then(vm.files);
        }

        function loadLinks() {
            examinerToolsRequester.getFluid('materialrequestlinks/' + serverModel.RoundId()).then(vm.links);
        }

        function saveMembers() {
            if (preventSave) {
                return;
            }
            vm.showSaveParticipants(true);

        };

        function clearLinkValidation() {
            if (linkValidation.errors) {
                linkValidation.errors.showAllMessages(false);
            }
        }

        function clearValidation() {
            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }
        }

        return vm;
    }
);