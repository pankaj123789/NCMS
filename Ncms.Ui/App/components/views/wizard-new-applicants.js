define([
        'modules/common',
        'modules/custom-validator',
],
    function (common, customValidator) {
        var functions = common.functions();

        function ViewModel(params) {
            var self = this;
            params.component = self;

            var defaultParams = {
                selectedTestSession: null,
                selectedIds: ko.observableArray(),
                applicants: ko.observableArray(),
                existingApplicants: ko.observableArray([]),
                venueCapacity: ko.observable(),
                allowSelfAssign: ko.observable(),
                fillSeats: true,
                isValidPromise: null,
                skillIds: ko.observableArray([])
            };

            $.extend(defaultParams, params);
            
            var serverModel = {
                CredentialRequestIds: defaultParams.selectedIds
            };

            var validator = customValidator.create(serverModel);

            self.skillIds = defaultParams.skillIds;
            self.selectedTestSession = defaultParams.selectedTestSession;
            self.applicants = defaultParams.applicants;
            self.existingApplicants = defaultParams.existingApplicants;
            self.venueCapacity = defaultParams.venueCapacity;
            self.allowSelfAssign = defaultParams.allowSelfAssign;
            self.fillSeats = defaultParams.fillSeats;
            self.readOnly = ko.observable(false);
            self.skillMatched = ko.observable(true);
            self.selectedIds = serverModel.CredentialRequestIds;
            self.tableDefinition = {
                searching: false,
                paging: false,
                oLanguage: { sInfoEmpty: '', sInfo: '' },
                columnDefs: [
                    {
                        orderable: false,
                        className: 'select-checkbox',
                        targets: 0
                    },
                    {
                        targets: 1,
                        render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateTimeDisplayFormat)
                    }
                ],
                order: [
                    [1, "asc"]
                ],
                select: {
                    style: 'multi+shift'
                },
                drawCallback: drawTable,
                events: {
                    select: selectTable,
                    deselect: selectTable,
                    'user-select': function (e) {
                        if (self.readOnly()) {
                            e.preventDefault();
                        }
                    },
                }
            };

            self.postData = function () {
                var json = { CredentialRequestIds: self.selectedIds() };
                return json;
            };

            self.availableSeats = function () {
                return self.venueCapacity() - self.existingApplicants().filter(function (a) { return !a.Rejected; }).length - self.selectedIds().length;
            };

            self.warnMessage = ko.pureComputed(function() {
                var available = self.availableSeats();
                var message = ko.Localization('Naati.Resources.CredentialRequestSummary.resources.AvailableSeats').format(Math.max(0, available));
                if (available < 0) {
                    message += " - " + ko.Localization('Naati.Resources.CredentialRequestSummary.resources.VenueOvercrowded');
                }

                if (self.applicants().length > 0) {
                    var applicant = self.applicants()[0];
                    var testSessionSkills = self.skillIds();

                    if (self.selectedTestSession && self.selectedTestSession.Skills) {
                        testSessionSkills = self.selectedTestSession.Skills()() || [];
                    }

                    var returnedData = $.grep(testSessionSkills, function(element, index) {
                        return (element.DirectionDisplayName === applicant.DirectionDisplayName &&
                            element.Language1Name === applicant.Language1 &&
                            element.Language2Name === applicant.Language2) || element === applicant.SkillId;
                    });

                    if (returnedData.length <= 0 && !self.allowSelfAssign()) {
                        self.skillMatched(false);
                        message += " - " +
                            ko.Localization('Naati.Resources.CredentialRequestSummary.resources.SkillNotMatched');
                    } else {
                        self.skillMatched(true);
                    }
                }
                return message;
            });

            self.isValid = function () {
                var defer = Q.defer();

                if (defaultParams.isValidPromise) {
                    defaultParams.isValidPromise().then(function (data) {
                        validator.reset();

                        ko.utils.arrayForEach(data.InvalidFields, function (i) {
                            validator.setValidation(i.FieldName, false, i.Message);
                        });

                        validator.isValid();

                        var isValid = !data.InvalidFields.length;
                        defer.resolve(isValid);
                        self.readOnly(isValid);
                    })
                }
                else {
                    defer.resolve(true);
                }

                return defer.promise;
            };

            load();

            function load() {
                self.readOnly(false);
                validator.reset();
                self.selectedIds([]);
            }

            var bypassSelect = false;
            function selectTable(e, dt) {
                if (bypassSelect) {
                    return;
                }

                self.selectedIds([]);
                var indexes = dt.rows('.selected').indexes();

                if (!indexes.length) {
                    return;
                }

                for (var i = 0; i < indexes.length; i++) {
                    self.selectedIds.push(self.applicants()[indexes[i]].CredentialRequestId);
                }
            }

            function drawTable() {
                bypassSelect = true;
                self.selectedIds([]);

                if (!self.fillSeats) {
                    bypassSelect = false;
                    return;
                }

                var $table = $(this).DataTable();
                var data = self.existingApplicants();
                var existingIds = ko.utils.arrayMap(data, function (item) { return item.CredentialRequestId; });
                  for (var i = 0; i < self.applicants().length; i++) {

                    if (self.availableSeats() <= 0) {
                        break;
                    }
                    //var requestId = self.applicants()[i].CredentialRequestId;
                    //if (!existingIds.includes(requestId)) {
                    //    $table.row(i).select();
                    //    self.selectedIds.push(requestId);
                    //}
                }

                bypassSelect = false;
            }
        }

        return ViewModel;
    });