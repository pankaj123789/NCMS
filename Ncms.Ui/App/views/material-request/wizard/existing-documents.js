define([
    'modules/enums',
    'services/file-data-service'
], function (enums, fileService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            wizardService: null,
            materialRequest: null,
            outputMaterial:null,
            action: null,
            materialRound: null,
            stepId: null,
            nextStepAction: null,
        };

        $.extend(defaultParams, params);

        var vm = {
            materialRequest: defaultParams.materialRequest,
            sections: ko.observableArray([]),
            selectedDocuments: ko.observableArray(),
            action: defaultParams.action,
            outputMaterial: defaultParams.outputMaterial,
            nextStepAction: defaultParams.nextStepAction,
            downloadDocumentUrlFunc: null,
            enableMergeOption: ko.observable()
        };
       
        vm.load = function () {
           
            defaultParams.wizardService.getFluid('wizard/showMergeOption/' +vm.action().Id +'/' +vm.outputMaterial.TestMaterialTypeId()).then(function(data) {
                vm.enableMergeOption(data);

                defaultParams.wizardService.getFluid('wizard/existingdocuments/' + vm.materialRequest.MaterialRequestId() + '/' + (vm.materialRequest.SourceTestMaterialId() || 0) + '/' + vm.action().Id + '/' + vm.outputMaterial.TestMaterialTypeId()).then(function (data) {
                    var sections = {};
                    ko.utils.arrayForEach(data, function (d) {
                        var section = sections[d.SectionName];
                        if (!section) {
                            section = [];
                            sections[d.SectionName] = section;
                        }
                        ko.utils.arrayForEach(d.Documents, function (doc) {
                            doc.Selected = false;
                            doc.MergeDocument = vm.enableMergeOption() ? doc.MergeDocument : false;
                        });
                        section.push.apply(section, d.Documents);
                    });

                    var results = [];
                    for (var section in sections) {
                        var documents = ko.viewmodel.fromModel(sections[section]);
                        results.push(createSection(section, documents));
                    }
                    vm.sections(results);

                    if (!vm.sections().length) {
                        vm.nextStepAction();
                    }
                });
            });
        };

        vm.download = function (document) {
            return vm.downloadDocumentUrlFunc ? vm.downloadDocumentUrlFunc(document) : fileService.url() + '/download/' + document.Id();
        };

        vm.postData = function () {
            var documents = [];
            var sections = ko.toJS(vm.sections());
            ko.utils.arrayForEach(sections, function (s) {
                ko.utils.arrayForEach(s.documents, function (d) {
                    if (d.Selected) {
                        documents.push(d);
                    }
                });
            });
            return {
                Documents: documents
            };
        };

        function createSection(section, documents) {
            return {
                Name: section,
                documents: documents,
                tableDefinition: {
                    headerTemplate: 'documents-header-template',
                    rowTemplate: 'documents-row-template',
                    dataTable: {
                        source: documents,
                        select: {
                            style: 'multi+shift'
                        },
                        columnDefs: [
                            { orderable: false, className: 'select-checkbox', targets: 0 },
                            { targets: -1, orderable: false },
                            { orderData: 1, targets: 1 }
                        ],
                        events: {
                            select: function (e, dt) {
                                if (!dt) {
                                    return;
                                }
                                selectTable(dt, documents);
                            },
                            deselect: function (e, dt) {
                                if (!dt) {
                                    return;
                                }
                                selectTable(dt, documents);
                            }
                        }
                    }
                }
            };
        }

        function selectTable(dt, documents) {
            ko.utils.arrayForEach(documents(), function (d) {
                d.Selected(false);
            });

            var indexes = dt.rows('.selected').indexes();

            if (!indexes.length) {
                return;
            }

            for (var i = 0; i < indexes.length; i++) {
                var document = documents()[indexes[i]];
                document.Selected(true);
            }
        }

        return vm;
    }
});