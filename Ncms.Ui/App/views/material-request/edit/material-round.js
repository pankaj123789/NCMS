define([
    'modules/common',
    'modules/enums',
    'views/document/document-form',
    'views/material-request/wizard/round-links',
    'services/material-request-data-service',
    'modules/custom-validator',
    'plugins/router',
    'services/util'
],
    function (common, enums, documentForm, roundLinks, materialRequestService, customValidator, router, util) {
        return {
            getInstance: getInstance
        };

        function getInstance(options) {
            var defaultOptions = {
                DocumentTypes: ko.observableArray(),
                MaterialRound: null,
                TabActive: null,
                MaterialRequestId: ko.observable(),
                PanelId: ko.observable(),
                loadMaterialRequest: null
            };

            $.extend(defaultOptions, options);

            var serverModel = {
                MaterialRoundId: ko.observable(defaultOptions.MaterialRound.Id),
                DisplayName: ko.observable(defaultOptions.MaterialRound.DisplayName),
                RoundNumber: ko.observable(),
                CoordinatedByName: ko.observable(),
                StatusTypeId: ko.observable(),
                StatusTypeName: ko.observable(),
                DueDate: ko.observable(),
                RequestedDate: ko.observable(),
                StatusChangeDate: ko.observable(),
                SubmittedDate: ko.observable(),
                Members: ko.observableArray(),
                Links: ko.observableArray(),
                loadMaterialRequest: defaultOptions.loadMaterialRequest,
                MaxBillableHours: ko.observable()
            };

            serverModel.StatusClass = ko.computed(function () {
                return util.getMaterialRequestRoundStatusCss(serverModel.StatusTypeId());
            });

            var vm = {
                materialRound: serverModel,
                links: ko.observableArray(),
                actions: ko.observableArray(),
                applicationId: ko.observable(),
                selectAction: function () { }
            };

            var preventLinksBubble = false;
            var preventServerModelLinksBubble = false;

            vm.links.subscribe(function (changes) {
                if (preventLinksBubble) return;

                preventServerModelLinksBubble = true;
                ko.utils.arrayForEach(changes, function (change) {
                    if (change.status === 'added') {
                        var link = {
                            MaterialRequestRoundId: serverModel.MaterialRoundId(),
                            NaatiNumber: null,
                            NcmsAvailable: true,
                            Link: change.value
                        };
                        materialRequestService.post(link, 'link/' + serverModel.MaterialRoundId()).then(function (data) {
                            link.Id = data;
                            serverModel.Links.push(ko.viewmodel.fromModel(link));
                            toastr.success(ko.Localization('Naati.Resources.MaterialRequest.resources.LinkAdded'));
                        });
                    }
                    if (change.status === 'deleted') {
                        var link = ko.utils.arrayFirst(serverModel.Links(), function (l) {
                            return l.Link() == change.value;
                        });
                        materialRequestService.remove('link/' + link.Id()).then(function () {
                            serverModel.Links.remove(link);
                            toastr.success(ko.Localization('Naati.Resources.MaterialRequest.resources.LinkDeleted'));
                        });
                    }
                });
                preventServerModelLinksBubble = false;
            }, null, "arrayChange");

            serverModel.Links.subscribe(function () {
                if (preventServerModelLinksBubble) return;

                var links = ko.utils.arrayFilter(serverModel.Links(), function (l) {
                    return l.NcmsAvailable();
                });

                links = ko.utils.arrayMap(links, function (l) {
                    return l.Link();
                });
                preventLinksBubble = true;
                vm.links(links);
                preventLinksBubble = false;
            });

            vm.allowEdit = ko.computed(function () {
                return serverModel.StatusTypeId() === enums.MaterialRequestRoundStatusType.SentForDevelopment;
            });

            vm.roundLinksCompose = {
                view: 'views/material-request/wizard/round-links',
                model: roundLinks.getInstance({
                    allowEdit: vm.allowEdit,
                    links: vm.links,
                })
            };

            vm.humanizeDate = function (date) {
                return common.functions().humanizeDate(moment(date).toDate());
            };

            vm.selectAction = function (action) {
                selectAction(action);
            };

            var validator = customValidator.create(serverModel);

            var validation = ko.validatedObservable(serverModel);
            configDocumentForm();

            vm.documentCompose = {
                model: vm.documentsInstance,
                view: 'views/document/document-form'
            };

            init();

            function configDocumentForm() {
                vm.documentsInstance = documentForm.getInstance();
                vm.documentsInstance.eportalDownload(true);
                vm.documentsInstance.parseFileName = function (fileName) {
                    var tmp = fileName.FileName.split('.');
                    return tmp.slice(0, tmp.length - 1).join('.');
                };

                vm.documentsInstance.fileUpload.url = materialRequestService.url() + '/documents/upload';
                vm.documentsInstance.fileUpload.formData = ko.computed(function () {
                    return {
                        id: vm.documentsInstance.currentDocument.Id() || 0,
                        file: vm.documentsInstance.currentDocument.File(),
                        type: vm.documentsInstance.currentDocument.Type(),
                        materialRequestRoundId: serverModel.MaterialRoundId(),
                        title: vm.documentsInstance.currentDocument.Title(),
                        storedFileId: vm.documentsInstance.currentDocument.StoredFileId() || 0,
                        eportalDownload: vm.documentsInstance.eportalDownload()
                    };
                });

                defaultOptions.DocumentTypes.subscribe(function () {
                    vm.documentsInstance.types(defaultOptions.DocumentTypes());
                });

                $.extend(vm.documentsInstance,
                    {
                        getDocumentsPromise: function () {
                            if (vm.documentsInstance.relatedId() <= 0) {
                                return Promise.resolve([]);
                            }

                            return materialRequestService.getFluid('documents/' + serverModel.MaterialRoundId(),
                                {
                                    supressResponseMessages: true
                                });
                        },
                        postDocumentPromise: function () {
                            var data = ko.toJS(vm.documentsInstance.currentDocument);
                            data.RelatedId = serverModel.MaterialRoundId();
                            data.EportalDownload = vm.documentsInstance.eportalDownload();
                            return materialRequestService.post(data, 'documents');
                        },
                        removeDocumentPromise: function (document) {
                            return materialRequestService.remove('documents/' + document.AttachmentId);
                        },
                        transformDocuments: function (data) {
                            return ko.utils.arrayMap(data,
                                function (d) {
                                    d.AttachmentId = d.MaterialRequestRoundAttachmentId;
                                    d.MaterialRoundId = vm.materialRound.MaterialRoundId();
                                    return d;
                                });
                        },

                        downloadDocumentUrlFunc: function (document) {

                            return materialRequestService.url() +
                                '/roundDocument/download/' +
                                document.MaterialRoundId +
                                '/' +
                                document.StoredFileId;
                        }
                    });
            }

            function init() {
                vm.documentsInstance.search();
                materialRequestService.getFluid('round/' + serverModel.MaterialRoundId()).then(function (data) {
                    ko.viewmodel.updateFromModel(serverModel, data);
                    materialRequestService.getFluid('roundAction/' + serverModel.StatusTypeId()).then(vm.actions);

                    checkDocumentAccess();
                });

                vm.documentsInstance.types(defaultOptions.DocumentTypes());
                vm.documentsInstance.showMergeDocument(false);
            }

            function checkDocumentAccess() {
                if (serverModel.StatusTypeId()) {
                    var canEdit = vm.allowEdit();
                    vm.documentsInstance.allowCreate(canEdit);
                    vm.documentsInstance.allowEdit(canEdit);
                    vm.documentsInstance.allowRemove(canEdit);
                }
            }

            function resetValidation() {
                validator.reset();

                if (validation.errors) {
                    return validation.errors.showAllMessages(false);
                }
            };

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

            function selectAction(action, materialRound) {
                var request = {
                    ActionId: action.Id,
                    MaterialRequestId: defaultOptions.MaterialRequestId(),
                    MaterialRequestsRoundId: serverModel.MaterialRoundId()
                };

                materialRequestService.post(request, 'roundAction').then(function (data) {
                    if (!checkAndShowMessages(data)) {
                        materialRequestService.getFluid('roundSteps/' + request.MaterialRequestsRoundId + '/' + request.ActionId).then(function (steps) {
                            if (steps.length) {
                                return router.navigate('material-request-wizard/' + action.Id + '/' + defaultOptions.PanelId() + '/' + request.MaterialRequestId + '/' + serverModel.MaterialRoundId());
                            }

                            materialRequestService.post(request, 'wizard').then(function () {
                                toastr.success(ko.Localization('Naati.Resources.Application.resources.ActionTakenSuccessfully'));
                                vm.loadMaterialRequest();
                            });
                        });
                    }
                });
            }

            return vm;
        }


    });
