define([
    'services/util',
    'services/screen/message-service'
],
    function (util, messageService) {
        function ViewModel(params) {
            var self = this;
            var preventProcessGroups = false;

            self.isWriter = params.isWriter || ko.observable(false);
            self.isDirty = params.isDirty || ko.observable(false);
            self.conditions = params.conditions || ko.observableArray();
            self.headerTemplate = params.headerTemplate;
            self.rowTemplate = params.rowTemplate;
            self.detailsRowTemplate = params.detailsRowTemplate;
            self.afterProcessGroups = params.afterProcessGroups;
            self.groupConditions = params.groupConditions || ko.observableArray();
            self.selectedConditions = params.selectedConditions || ko.observableArray();
            self.showIfEmpty = !!params.showIfEmpty;

            self.conditions.subscribe(bindConfigurations);

            bindConfigurations();

            function bindConfigurations() {
                if (preventProcessGroups) return;
                processGroups();
                self.isDirty(false);
            }

            function selectTable(e, dt) {
                if (!dt) return;

                var indexes = dt.rows('.selected').indexes();
                self.selectedConditions([]);

                if (!indexes.length) {
                    return;
                }

                for (var i = 0; i < indexes.length; i++) {
                    self.selectedConditions.push(self.groupConditions()[indexes[i]]);
                }
            }

            function enableDisableDataTableButtons() {
                var $table = $('#' + self.conditionOptions.id);

                if (!$.fn.DataTable.isDataTable($table)) {
                    setTimeout(enableDisableDataTableButtons, 100);
                    return;
                }

                var buttons = $table.DataTable().buttons;
                if (!buttons('*').length) {
                    setTimeout(enableDisableDataTableButtons, 100);
                    return;
                }

                var selectedConditions = self.selectedConditions();
                var groupButton = buttons(0);
                var ungroupButton = buttons(1);
                var hasGroup = ko.utils.arrayFirst(selectedConditions, function (s) {
                    return s.children && s.children.length;
                });

                groupButton.enable(selectedConditions.length >= 2);
                ungroupButton.enable(hasGroup != null);
            }

            function group(id, selectedConditions) {
                var hasConditions = !!selectedConditions;
                selectedConditions = selectedConditions || self.selectedConditions();
                id = id || util.guid();
                ko.utils.arrayForEach(selectedConditions, function (c) {
                    if (c.children && c.children.length) {
                        return group(id, c.children);
                    }
                    c.group(id);
                });

                if (!hasConditions) {
                    processGroups();
                }
            }

            self.selectedConditions.subscribe(enableDisableDataTableButtons);

            self.conditionOptions = {
                id: util.guid(),
                headerTemplate: self.headerTemplate,
                rowTemplate: self.rowTemplate,
                dataTable: {
                    source: self.groupConditions,
                    info: false,
                    ordering: false,
                    oLanguage: { sInfoEmpty: '', sInfo: '' },
                    select: {
                        style: 'multi'
                    },
                    events: {
                        select: selectTable,
                        deselect: selectTable,
                    },
                    buttons: {
                        dom: {
                            button: {
                                tag: 'label',
                                className: 'm-r-sm'
                            },
                            buttonLiner: {
                                tag: null
                            }
                        },
                        buttons: [{
                            text: '<span class="far fa-object-group m-r-xs"></span><span>' +
                                ko.Localization('Naati.Resources.RubricConfiguration.resources.GroupConditions') + '</span>',
                            className: 'btn btn-success',
                            action: function () {
                                group();
                            }
                        },
                        {
                            text: '<span class="far fa-object-ungroup m-r-xs"></span><span>' +
                                ko.Localization('Naati.Resources.RubricConfiguration.resources.UngroupConditions') + '</span>',
                            className: 'btn btn-success',
                            action: function () {
                                var selectedConditions = self.selectedConditions();
                                ko.utils.arrayForEach(selectedConditions, function (c) {
                                    if (!c.children || !c.children.length) return;

                                    ko.utils.arrayForEach(c.children, function (c) {
                                        c.group(null);
                                    });

                                    processGroups();
                                });
                            }
                        }]
                    },
                    initComplete: enableDisableDataTableButtons
                }
            };

            function processGroups() {
                self.isDirty(true);

                var conditions = self.conditions();
                if (!conditions || !conditions.length) {
                    self.groupConditions([]);
                    self.groupConditions(groupConditions);
                    return self.selectedConditions([]);
                }

                var groupConditions = [];
                var groups = [];

                ko.utils.arrayForEach(conditions, function (condition) {
                    if (!condition.group()) {
                        condition.concat = 'AND';
                        return groupConditions.push(condition);
                    }

                    if (groups.indexOf(condition.group()) >= 0) return;

                    condition.concat = 'AND';

                    var sameGroup = ko.utils.arrayFilter(conditions, function (c) {
                        var match = c.group() == condition.group();

                        if (match) {
                            c.concat = 'OR';
                        }

                        return match;
                    });

                    if (sameGroup.length == 1) {
                        condition.group(null);
                        return groupConditions.push(condition);
                    }

                    sameGroup[sameGroup.length - 1].concat = null;

                    var cluster = $.extend({}, sameGroup[0], {
                        showChildren: ko.observable(false),
                        concat: 'AND',
                        remove: function () {
                            messageService.remove().then(function (answer) {
                                if (answer !== 'yes') {
                                    return;
                                }

                                preventProcessGroups = true;
                                ko.utils.arrayForEach(sameGroup, function (condition) {
                                    self.conditions.remove(condition);
                                });

                                preventProcessGroups = false;
                                processGroups();
                            });
                        },
                        children: sameGroup,
                    });

                    cluster.toggleChildren = function (i, e) {
                        cluster.showChildren(!cluster.showChildren());

                        var target = $(e.target);
                        var tr = target.closest('tr');
                        var dt = tr.closest('#' + self.conditionOptions.id).DataTable();
                        var row = dt.row(tr);

                        if (row.child.isShown()) {
                            row.child.hide();
                        } else {
                            var detailsTemplate = ko.generateTemplate(self.detailsRowTemplate, i, '<div>');
                            row.child(detailsTemplate).show();
                        }
                    };

                    groupConditions.push(cluster);

                    groups.push(condition.group());
                });


                var groupIndex = 1;
                ko.utils.arrayForEach(groupConditions, function (gc) {
                    if (!gc.children || !gc.children.length) return;

                    ko.utils.arrayForEach(gc.children, function (c) {
                        c.group('OR' + groupIndex);
                    });

                    groupIndex++;
                });

                groupConditions[groupConditions.length - 1].concat = null;

                if (self.afterProcessGroups) {
                    self.afterProcessGroups({ groups: groupConditions });
                }

                self.groupConditions([]);
                self.groupConditions(groupConditions);
                self.selectedConditions([]);
            }

            params.component = self;
        }

        return ViewModel;
    });