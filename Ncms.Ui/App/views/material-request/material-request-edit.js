define([
    'plugins/router',
    'modules/custom-validator',
    'modules/enums',
    'services/screen/message-service',
    'services/material-request-data-service',
    'views/material-request/edit/material-round',
    'views/material-request/edit/material-correspondence',
    'views/material-request/edit/material-participants',
    'views/shared/note',
    'services/util',
    'services/screen/date-service',
],
    function (router, customValidator, enums, message, materialRequestService, materialRounds, materialCorrespondence, participants, notes, util, dateService) {
        var compositionComplete = false;
        var queryString;

        var serverModel = {
            MaterialRequestId: ko.observable(),
            PanelId: ko.observable(),
            StatusTypeId: ko.observable(),
            StatusTypeDisplayName: ko.observable(),
            CreatedDate: ko.observable(),
            EnteredOffice: ko.observable(),
            PanelName: ko.observable(),
            TypeLabel: ko.observable(),
            SourceTestMaterial: ko.observable(),
            OutputTestMaterial: ko.observable(),
            OwnedByUserName: ko.observable(),
            ProductSpecificationDisplayName: ko.observable(),
            SpecificationCode: ko.observable(),
            CoordinatedByName: ko.observable(),
            MaxBillableHours: ko.observable(),
            ProductSpecificationCostPerUnit: ko.observable(),
            Members: ko.observableArray(),
            RequestTitle: ko.observable(),
            Domain: ko.observable(),
            CreatedByUserName: ko.observable(),
        };

        var validator = customValidator.create(serverModel);

        var emptyModel = ko.toJS(serverModel);

        serverModel.StatusClass = ko.computed(function () {
            return util.getMaterialRequestStatusCss(serverModel.StatusTypeId());
        });

        var vm = {
            materialRequest: serverModel,
            materialRounds: ko.observableArray(),
            actions: ko.observableArray(),
            tabId: ko.observable(),
            participantsInstance: participants.getInstance(serverModel),
            notesInstance: notes.getInstance(),
            publicNotesInstance: notes.getInstance(),
            materialCorrespondenceInstance: materialCorrespondence.getInstance(),
            documentTypes: ko.observableArray(),
            tabOptions: {
                id: 'materialRequestEdit',
                tabs: ko.observableArray()
            }
        };

        vm.sourceTestMaterial = ko.computed(function () {
            return serverModel.SourceTestMaterial() || {};
        });

        vm.outputTestMaterial = ko.computed(function () {
            return serverModel.OutputTestMaterial() || {};
        });

        vm.materialRounds.subscribe(materialRoundsSubscribe);

        var dirtyFlag = new ko.DirtyFlag([serverModel], false);

        vm.close = function () {
            if (dirtyFlag().isDirty()) {
                message.confirm({
                    title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                    content: ko.Localization('Naati.Resources.Shared.resources.ConfirmBeforeSaving')
                })
                    .then(
                        function (answer) {
                            if (answer === 'yes') {
                                router.navigate('test-material-request-search');
                            }
                        });
            } else {
                router.navigate('test-material-request-search');
            }
        };

        var validation = ko.validatedObservable(serverModel);

        $.extend(vm, {
            title: ko.computed(function () {
                return ko.Localization('Naati.Resources.MaterialRequest.resources.MaterialRequestTitle').format(serverModel.MaterialRequestId());
            }),
            subtitle: ko.computed(function () {
                return ko.Localization('Naati.Resources.MaterialRequest.resources.EditingMaterialRequestTitle').format(serverModel.OwnedByUserName() + ' (' + serverModel.EnteredOffice() + ')');
            })
        });

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
                MaterialRequest: ko.toJS(vm.materialRequest)
            };

            materialRequestService.post(request, 'update')
                .then(function () {
                    dirtyFlag().reset();
                    defer.resolve('fulfilled');
                    toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                });

            return defer.promise;
        };

        vm.compositionComplete = function () {
            compositionComplete = true;
            processRequest();
        };

        vm.canActivate = function (materialRequestId, query) {
            queryString = query || {};
            materialRequestId = parseInt(materialRequestId);

            ko.viewmodel.updateFromModel(serverModel, emptyModel);
            serverModel.MaterialRequestId(materialRequestId);

            return loadMaterialRequest();
        };

        vm.activate = function () {
            if (compositionComplete) {
                processRequest();
            }
        };

        vm.selectAction = function (action) {
            selectAction(action);
        };

        vm.canSave = ko.pureComputed(function () {
            return dirtyFlag().isDirty();
        });

        $.extend(vm.notesInstance, {
            getNotesPromise: function () {
                return materialRequestService.getFluid(serverModel.MaterialRequestId() + '/notes');
            },
            postNotesPromise: function () {
                return materialRequestService.post($.extend(vm.notesInstance.parseNote(), { MaterialRequestId: serverModel.MaterialRequestId(), Highlight: true }), 'notes');
            },
            removeNotesPromise: function (note) {
                return materialRequestService.remove(note.NoteId + '/' + serverModel.MaterialRequestId() + '/notes');
            }
        });

        $.extend(vm.publicNotesInstance, {
            getNotesPromise: function () {
                return materialRequestService.getFluid(serverModel.MaterialRequestId() + '/publicNotes');
            },
            postNotesPromise: function () {
                return materialRequestService.post($.extend(vm.publicNotesInstance.parseNote(), { MaterialRequestId: serverModel.MaterialRequestId(), Highlight: true }), 'publicNotes');
            },
            removeNotesPromise: function (note) {
                return materialRequestService.remove(note.NoteId + '/' + serverModel.MaterialRequestId() + '/publicNotes');
            }
        });

        function selectAction(action, materialRound) {
            if (!isDirty()) {
                return takeAction(action, materialRound);
            }

            message.confirm({
                title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
                content: ko.Localization('Naati.Resources.Shared.resources.SaveBeforeAction')
            }).then(function (result) {
                if (result === 'yes') {
                    vm.save().then(function (saveResult) {
                        if (saveResult === 'fulfilled') {
                            takeAction(action, materialRound);
                        }
                    });
                }
            });
        };

        function takeAction(action, materialRound) {
            var request = {
                ActionId: action.Id,
                MaterialRequestId: serverModel.MaterialRequestId(),
                MaterialRoundId: materialRound ? materialRound.Id() : 0
            };

            materialRequestService.post(request, 'action').then(function (data) {
                if (!checkAndShowMessages(data)) {
                    materialRequestService.getFluid('steps/' + request.MaterialRequestId + '/' + request.ActionId + '/' + serverModel.PanelId()).then(function (steps) {
                        if (steps.length) {
                            return router.navigate('material-request-wizard/' + action.Id + '/' + serverModel.PanelId() + '/' + serverModel.MaterialRequestId() + '/' + (materialRound ? '/' + materialRound.Id() : ''));
                        }

                        materialRequestService.post(request, 'wizard').then(function () {
                            toastr.success(ko.Localization('Naati.Resources.Application.resources.ActionTakenSuccessfully'));
                            loadMaterialRequest();
                        });
                    });
                }
            });
        }

        function isDirty() {
            return dirtyFlag().isDirty();
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

        function checkTabActive(tabId) {
            return vm.tabId() == tabId;
        }

        function onTabClick(tabOption) {
            if (tabOption.disabled()) {
                return;
            }

            var tabId = tabOption.id;

            var currentUrl = window.location;
            var baseUrl = currentUrl.protocol + '//' + currentUrl.host + '/' + currentUrl.pathname.split('/')[1];
            var url = baseUrl + '#material-request/' + serverModel.MaterialRequestId();

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
            var tabId = null;
            var firstTab = ko.utils.arrayFirst(vm.tabOptions.tabs(), function (tab) {
                return !('disabled' in tab) || !tab.disabled();
            });

            if (!$.isEmptyObject(queryString)) {
                tabId = queryString['tab'];
            }
            else if (firstTab) {
                tabId = firstTab.id;
            }

            vm.tabId(tabId);
            var maxAttempt = 10;
            var attempt = 0;

            var interval = setInterval(function () {
                var $tab = $('.nav-tabs:visible:first [aria-controls="' + tabId + '"]');
                try { $tab.tab('show'); } catch (e) { }
                if ($tab.length > 0 || maxAttempt == attempt) {
                    clearInterval(interval);
                }
                attempt++;
            }, 500);
        }

        function loadMaterialRequest() {
            materialRequestService.getFluid('rounds/' + serverModel.MaterialRequestId()).then(vm.materialRounds);
            vm.materialCorrespondenceInstance.load(serverModel.MaterialRequestId());
            vm.notesInstance.getNotes();
            vm.publicNotesInstance.getNotes();
            vm.participantsInstance.load();


            materialRequestService.getFluid('allAvailableDocumentTypes').then(function (data) {
                var types = ko.utils.arrayMap(data, function (d) {
                    return d.Name;
                });
                vm.documentTypes(types);
            });

            return materialRequestService.getFluid(serverModel.MaterialRequestId()).then(function (data) {

                ko.utils.arrayForEach(data.Members,
                    function (member) {
                        member.TotalCost = data.ProductSpecificationCostPerUnit * member.TotalHours;
                        member.DisplayApprovedDate = dateService.toUIDate((member.PayRoll || {}).ApprovedDate);
                        member.DisplayPaymentDate = dateService.toUIDate((member.PayRoll || {}).PaymentDate);
                    });
                ko.viewmodel.updateFromModel(serverModel, data);

                materialRequestService.getFluid('action/' + serverModel.StatusTypeId()).then(vm.actions);

                resetValidation();
                dirtyFlag().reset();

                return true;
            });
        }

        function materialRoundsSubscribe() {
            var i = 0;

            var tabs = ko.utils.arrayMap(vm.materialRounds().reverse(), function (mr) {
                var id = 'round' + mr.Id;
                i++;
                return {
                    id: id,
                    active: checkTabActive(id),
                    label: mr.DisplayName,
                    type: 'compose',
                    composition: {
                        model: materialRounds.getInstance({
                            MaterialRound: mr,
                            MaterialRequestId: serverModel.MaterialRequestId,
                            PanelId: serverModel.PanelId,
                            DocumentTypes: vm.documentTypes,
                            loadMaterialRequest: loadMaterialRequest
                        }),
                        view: 'views/material-request/edit/material-round'
                    },
                    click: onTabClick
                }
            });

            tabs.push({
                id: 'materialParticipants',
                active: false,
                name: 'Naati.Resources.MaterialRequest.resources.Participants',
                type: 'compose',
                composition: {
                    model: vm.participantsInstance,
                    view: 'views/material-request/edit/material-participants'
                },
                click: onTabClick
            },
                {
                    id: 'materialNotes',
                    active: false,
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
                    id: 'publicNotes',
                    active: false,
                    name: 'Naati.Resources.MaterialRequest.resources.PanelNotes',
                    roles: [{ noun: enums.SecNoun.Notes, verb: enums.SecVerb.Read }],
                    type: 'compose',
                    composition: {
                        model: vm.publicNotesInstance,
                        view: 'views/shared/note.html'
                    },
                    click: onTabClick
                });


            tabs.push({
                id: 'correspondence',
                active: false,
                name: 'Naati.Resources.EmailMessage.resources.SystemEmails',
                type: 'compose',
                composition: {
                    model: vm.materialCorrespondenceInstance,
                    view: 'views/material-request/edit/material-correspondence'
                },
                click: onTabClick
            });

            vm.tabOptions.tabs(tabs);
            processRequest();
        }

        return vm;

    });
