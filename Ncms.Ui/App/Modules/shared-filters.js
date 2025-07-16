define([
    'services/util',
	'modules/common',
	'modules/enums'
], function (util, common, enums) {
    function selectDataFormat(data) {
        return { Options: data.selectedOptions };
    }

    function arrayDataFormat(data) {
        var result = [];

        if (data.value.length > 0) {
            var items = data.value.split(/[\s,;]+/);
            result = stripNonIntegers(items);
        }
        return $.extend(data, { valueAsArray: result });
    }

    function stripNonIntegers(dataArray) {
        var result = [];

        $.each(dataArray,
            function(i, val) {
                if (parseInt(val) == val) {
                    result.push(val);
                }
            });
        return result;
    }


    function dateRangeFormat(data) {
        return {
            From: data.fromDateOptions.value,
            To: data.toDateOptions.value,
            FilterType: data.filterOptions.value
        };
    }

    var functions = common.functions();

    var vm = {
        getFilter: function(filterId) {
            return ko.utils.arrayFirst(vm.filterDefinitions(), function(filter) {
                return filter.id === filterId;
            });
        },
        filterDefinitions: function() {
            return [
                {
                    id: 'NAATINumber',
                    name: 'Naati.Resources.Shared.resources.NAATINumber',
                    component: 'text-input',
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.NAATINumber'
                        }
                    },
                    formatData: function (data) {
                        var val = $.isArray(data.value) ? data.value : ('0,' + (data.value || '')).split(/[\s,;]+/);
                        return { NAATINumber: stripNonIntegers(val) };
                    }
                },
                {
                    id: 'EmailTemplateName',
                    name: 'Naati.Resources.EmailMessage.resources.EmailTemplateName',
                    component: 'text-input',
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.EmailMessage.resources.EmailTemplateName'
                        }
                    },
                    formatData: arrayDataFormat
                },
                {
                    id: 'UploadedByPerson',
                    name: 'Naati.Resources.Shared.resources.UploadedBy',
                    component: 'auto-text-input',
                    componentOptions: {
                        source: functions.naatiNumberSearch, // TODO: support users in this one or add another search for users
                        multiple: true,
                        valueProp: 'NaatiNumber',
                        labelProp: 'Name',
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.UploadedBy'
                        }
                    },
                    formatData: function(data) {
                        return { NAATINumber: stripNonIntegers(data.value) };
                    }
                }, {
                    id: 'TestOffice',
                    name: 'Naati.Resources.Test.resources.TestOffice',
                    component: 'entity-select',
                    componentOptions: {
                        entity: 'office',
                        validateEntity: function(entity) { return entity.InstitutionId },
                        valueField: ko.observable('OfficeId'),
                        textField: function(entity) { return entity.Abbreviation || entity.InstitutionName; }
                    },
                    formatData: selectDataFormat
                }, {
                    id: 'FinanceOffice',
                    name: 'Naati.Resources.Finance.resources.Office',
                    component: 'finance-office',
                    componentOptions: {
                        Office: {
                            optionsValue: 'EntityId',
                            optionsText: 'Name'
                        },
                        EFTMachine: {
                            optionsValue: 'EntityId',
                            optionsText: 'Name'
                        }
                    },
                    filterClass: ko.observable('col-lg-12'),
                    showLabel: false,
                    formatData: function(data) {
                        return {
                            Office: selectDataFormat(data.Office).Options,
                            EFTMachine: selectDataFormat(data.EFTMachine).Options
                        };
                    }
                }, {
                    id: 'DateSent',
                    name: 'Naati.Resources.Test.resources.DateSent',
                    component: 'date-range',
                    componentOptions: {
                        filterOptions: {
                            value: enums.DateRanges.Today
                        }
                    },
                    filterClass: ko.observable('col-lg-6'),
                    formatData: dateRangeFormat
                }, {
                    id: 'DateReceived',
                    name: 'Naati.Resources.Test.resources.DateReceived',
                    component: 'date-range',
                    componentOptions: {
                        filterOptions: {
                            value: enums.DateRanges.Today
                        }
                    },
                    filterClass: ko.observable('col-lg-6'),
                    formatData: dateRangeFormat
                }, {
                    id: 'DateRequested',
                    name: 'Naati.Resources.EmailMessage.resources.DateRequested',
                    component: 'date-range',
                    componentOptions: {
                        filterOptions: {
                            value: enums.DateRanges.Today
                        }
                    },
                    filterClass: ko.observable('col-lg-6'),
                    formatData: dateRangeFormat
                },  {
                    id: 'Recipient',
                    name: 'Naati.Resources.EmailMessage.resources.Recipient',
                    component: 'text-input',
                    showLabel: true,
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.EmailMessage.resources.Recipient'
                        }
                    }
                }, {
                    id: 'RecipientEmail',
                    name: 'Naati.Resources.EmailMessage.resources.RecipientEmail',
                    component: 'text-input',
                    showLabel: true,
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.EmailMessage.resources.RecipientEmail'
                        }
                    },
                    formatData: arrayDataFormat
                }, {
                    id: 'EmailStatus',
                    name: 'Naati.Resources.EmailMessage.resources.Status',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('EmailStatus').then(this.options) },
                        multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                    }
                },  {
                    id: 'TestDateFromAndTo',
                    name: 'Naati.Resources.Test.resources.TestDateFromAndTo',
                    component: 'date-range',
                    componentOptions: {
                        filterOptions: {
                            options: [{
                                value: ko.observable(enums.DateRanges.Custom),
                                text: ko.Localization('Naati.Resources.Test.resources.Custom')
                            }]
                        }
                    },
                    formatData: dateRangeFormat
                }, {
                    id: 'TestDateFromAndTo2',
                    name: 'Naati.Resources.Test.resources.TestDateFromAndTo',
                    component: 'date-range',
                    componentOptions: {
                        filterOptions: {
                            options: [{
                                value: ko.observable(enums.DateRanges.Custom),
                                text: ko.Localization('Naati.Resources.Test.resources.Custom')
                            }]
                        }
                    },
                    formatData: dateRangeFormat
                }, {
                    id: 'Examiner',
                    name: 'Naati.Resources.Test.resources.Examiner',
                    component: 'examiner',
                    componentOptions: {
                        Panel: {
                            optionsValue: 'Id',
                            optionsText: 'DisplayName',
                            options: ko.observableArray(),
                            afterRender: function () { functions.getLookup('Panel').then(this.options) },
                            multiselect: { enableFiltering: true }
                        },
                        Examiner: {
                            optionsValue: 'Id',
                            optionsText: 'DisplayName',
                            options: ko.observableArray(),
                            multiselect: { enableFiltering: true }
                        },
                        ExaminerStatus: {
                            optionsValue: 'Id',
                            optionsText: 'DisplayName',
                            options: ko.observableArray(),
                            afterRender: function () { functions.getLookup('ExaminerStatusType').then(this.options) },
                            multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                        },
                        DueBy: {
                            filterOptions: {
                                value: enums.DateRanges.None
                            }
                        },
                        Submitted: {
                            filterOptions: {
                                value: enums.DateRanges.None
                            }
                        }
                    },
                    filterClass: ko.observable('col-lg-12'),
                    showLabel: false,
                    formatData: function(data) {
                        return {
                            Panel: selectDataFormat(data.Panel).Options,
                            Examiner: selectDataFormat(data.Examiner).Options,
                            Status: selectDataFormat(data.ExaminerStatus).Options,
                            DueBy: dateRangeFormat(data.DueBy),
                            Submitted: dateRangeFormat(data.Submitted),
                           
                        };
                    }
                },
                {
                    id: 'EndorsedQualifications',
                    name: 'Naati.Resources.Application.resources.EndorsedQualifications',
                    component: 'endorsed-qualifications',
                    componentOptions: {
                        Institution: {
                            optionsValue: 'Id',
                            optionsText: 'DisplayName',
                            options: ko.observableArray(),
                            afterRender: function () { functions.getLookup('EndorsedQualificationsInstitution').then(this.options) },
                            multiselect: { enableFiltering: true }
                        },
                        Qualification: {
                            optionsValue: 'Id',
                            optionsText: 'DisplayName',
                            options: ko.observableArray(),
                            afterRender: function () { functions.getLookup('EndorsedQualifications').then(this.options) },
                            multiselect: { enableFiltering: true }
                        },
                        Location: {
                            optionsValue: 'Id',
                            optionsText: 'DisplayName',
                            options: ko.observableArray(),
                            afterRender: function () { functions.getLookup('EndorsedQualificationsLocation').then(this.options) },
                            multiselect: { enableFiltering: true }
                        },
                        EndorsementPeriod: {
                            filterOptions: {
                                value: enums.DateRanges.None
                            }
                        },
                        EndorsedQualificationIds: {
                            optionsValue: 'Id',
                            optionsText: 'Id',
                            options: ko.observableArray(),
                            afterRender: function () { functions.getLookup('EndorsedQualificationIds').then(this.options) },
                            multiselect: { enableFiltering: true }
                        }
                    },
                    filterClass: ko.observable('col-lg-12'),
                    showLabel: false,
                    formatData: function (data) {
                        return {
                            Institution: selectDataFormat(data.Institution).Options,
                            Qualification: selectDataFormat(data.Qualification).Options,
                            Location: selectDataFormat(data.Location).Options,
                            EndorsementQualificationIds: selectDataFormat(data.EndorsementQualificationIds).Options,
                            EndorsementPeriod: dateRangeFormat(data.EndorsementPeriod)
                        };
                    }
                }

                , {
                    id: 'TestStatus',
                    name: 'Naati.Resources.Test.resources.TestStatus',
                    component: 'select-component',
                    componentOptions: {
                        options: functions.optionsValueFactory(enums.TestStatus.list, 'TestStatus')
                    },
                    formatData: selectDataFormat
                }, {
                    id: 'PaymentType',
                    name: 'Naati.Resources.Finance.resources.PaymentType',
                    component: 'select-component',
                    componentOptions: {
                        options: functions.optionsValueFactory(enums.PaymentTypes.list, 'Finance')
                    },
                    formatData: selectDataFormat
                }, {
                    id: 'ExaminerStatus',
                    name: 'Naati.Resources.Test.resources.ExaminerStatus',
                    component: 'select-component',
                    componentOptions: {
                        options: functions.optionsValueFactory(enums.ExaminerStatus.list, 'ExaminerStatus')
                    },
                    formatData: selectDataFormat
                }, {
                    id: 'RecipientNaatiUser',
                    name: 'Naati.Resources.EmailMessage.resources.RecipientNaatiUser',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('NaatiUser').then(this.options) },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },  
                {
                    id: 'AccountingOperationStatus',
                    name: 'Naati.Resources.Shared.resources.Status',
                    component: 'select-component',
                    componentOptions: {
                        options: functions.optionsValueFactory(enums.AccountingOperationStatus.list, 'AccountingOperationStatus')
                    },
                    formatData: selectDataFormat
                }, {
                    id: 'IncludeWithoutAssets',
                    name: 'Naati.Resources.Test.resources.IncludeWithoutAssets',
                    component: 'radio-component',
                    componentOptions: {
                        checked: true
                    }
                }, {
                    id: 'Language',
                    name: 'Naati.Resources.Shared.resources.Language',
                    component: 'entity-select',
                    componentOptions: {
                        entity: 'language',
                        valueField: ko.observable('LanguageId'),
						textField: ko.observable('Name'),
						dataCallback: function (data) {
							return data.sort(util.sortBy('Name'));
						}
                    },
                    formatData: selectDataFormat
                },

                {
                    id: 'Direction',
                    name: 'Naati.Resources.Test.resources.Direction',
                    component: 'select-component',
                    componentOptions: {
                        options: functions.optionsNameFactory(enums.Directions, 'Test', true)
                    },
                    formatData: selectDataFormat
                }, {
                    id: 'Level',
                    name: 'Naati.Resources.Test.resources.Level',
                    component: 'entity-select',
                    componentOptions: {
                        entity: 'accreditationlevel',
                        valueField: ko.observable('AccreditationLevelId'),
                        textField: ko.observable('WebDisplay')
                    },
                    formatData: selectDataFormat
                }, {
                    id: 'Category',
                    name: 'Naati.Resources.Test.resources.Category',
                    component: 'entity-select',
                    componentOptions: {
                        entity: 'accreditationcategory',
                        valueField: ko.observable('AccreditationCategoryId'),
                        textField: ko.observable('Description')
                    },
                    formatData: selectDataFormat
                }, {
                    id: 'MarkerAllocated',
                    name: 'Naati.Resources.Test.resources.MarkerAllocated',
                    component: 'radio-component',
                    componentOptions: {}
                }, {
                    id: 'AllMarksReceived',
                    name: 'Naati.Resources.Test.resources.AllMarksReceived',
                    component: 'radio-component',
                    componentOptions: {
                        checked: true
                    }
                },
                {
                    id: 'PendingToAssignPaidReviewers',
                    name: 'Naati.Resources.Test.resources.PendingToAssignPaidReviewers',
                    component: 'radio-component',
                    componentOptions: {
                        checked: true
                    }
                },
                {
                    id: 'IsSupplementary',
                    name: 'Naati.Resources.Test.resources.IsSupplementary',
                    component: 'radio-component',
                    componentOptions: {}
                },
                {
                    id: 'ShowInOnlineDirectory',
                    name: 'Naati.Resources.Shared.resources.ShowInOnlineDirectory',
                    component: 'radio-component',
                    componentOptions: {}
                },
                {
                    id: 'JobID',
                    name: 'Naati.Resources.Test.resources.JobID',
                    component: 'text-input',
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.Test.resources.JobID'
                        }
                    },
                    formatData: arrayDataFormat
                }, {
                    id: 'InvoiceNumber',
                    name: 'Naati.Resources.Finance.resources.InvoiceNo',
                    component: 'text-input',
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.Finance.resources.InvoiceNo'
                        }
                    },
                    formatData: arrayDataFormat
                }, {
                    id: 'DocumentName',
                    name: 'Naati.Resources.Document.resources.DocumentName',
                    component: 'text-input',
                    showLabel: false,
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.Document.resources.DocumentNamePlaceHolder'
                        }
                    }
                }, {
                    id: 'AttendanceId',
                    name: 'Naati.Resources.Test.resources.AttendanceId',
                    component: 'text-input',
                    componentOptions: {
                        multiple: true,
                        attr: {
                         
                        },
                        resattr: {
                            placeholder: 'Naati.Resources.Test.resources.AttendanceId'
                        }
                    },
                    formatData: arrayDataFormat
                },
                {
                    id: 'TestID',
                    name: 'Naati.Resources.Test.resources.AttendanceId',
                    component: 'text-input',
                    componentOptions: {
                        attr: {
                            maxlength: 9
                        },
                        resattr: {
                            placeholder: 'Naati.Resources.Test.resources.AttendanceId'
                        }
                    },
                    formatData: arrayDataFormat
                },
                {
                    id: 'TestVenue',
                    name: 'Naati.Resources.Test.resources.TestVenue',
                    component: 'entity-select',
                    componentOptions: {
                        entity: 'eventvenue',
                        valueField: ko.observable('EventVenueId'),
                        textField: ko.observable('Name')
                    },
                    formatData: selectDataFormat
                }, {
                    id: 'PanelType',
                    name: 'Naati.Resources.Panel.resources.PanelType',
                    component: 'entity-select',
                    componentOptions: {
                        entity: 'PanelType',
                        valueField: ko.observable('PanelTypeId'),
                        textField: ko.observable('Name')
                    },
                    formatData: selectDataFormat
                }, {
                    id: 'EndDate',
                    name: 'Naati.Resources.Shared.resources.EndDate',
                    component: 'date-input',
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.EndDate'
                        }
                    }
                }, {
                    id: 'TestDateRange',
                    name: 'Naati.Resources.Test.resources.TestDateFromAndTo',
                    component: 'date-range',
                    componentOptions: {
                        filterOptions: {
                            value: enums.DateRanges.Today
                        }
                    },
                    filterClass: ko.observable('col-lg-4'),
                    formatData: dateRangeFormat
                },
                {
                    id: 'Gender',
                    name: 'Naati.Resources.Person.resources.Gender',
                    component: 'select-component',
                    componentOptions: {
                        options: functions.optionsNameFactory(enums.Genders, 'Shared', true),
                        multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                    }
                }, {
                    id: 'RequestStatus',
                    name: 'Naati.Resources.TestMaterial.resources.RequestStatus',
                    component: 'select-component',
                    componentOptions: {
                        options: functions.optionsValueFactory(enums.RequestStatus.list, 'TestMaterial')
                    },
                    formatData: selectDataFormat
                }, {
                    id: 'IncludeDomainInvoices',
                    name: 'Naati.Resources.Finance.resources.IncludeDomainInvoices',
                    component: 'radio-component',
                    componentOptions: {
                        checked: true
                    }
                }, {
                    id: 'ContactNumber',
                    name: 'Naati.Resources.Shared.resources.ContactNumber',
                    component: 'text-input',
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.ContactNumber'
                        }
                    },
                    formatData: arrayDataFormat
                }, {
                    id: 'ApplicationStatus',
                    name: 'Naati.Resources.Shared.resources.ApplicationStatus',
                    component: 'applicationStatus',
                    componentOptions: {
                        ApplicationType: {
                            options: functions.optionsValueFactory(enums.ApplicationTypes.list, 'ApplicationTypes')
                        },
                        ApplicationStatus: {
                            options: functions.optionsValueFactory(enums.ApplicationStatusTypes.list, 'ApplicationStatuses')
                        },
                        ApplicationLineStatus: {
                            options: functions.optionsValueFactory(enums.ApplicationStatusTypes.list, 'ApplicationStatuses')
                        }
                    },
                    filterClass: ko.observable('col-lg-12'),
                    showLabel: false,
                    formatData: function(data) {
                        return {
                            ApplicationType: selectDataFormat(data.ApplicationType).Options,
                            ApplicationStatus: selectDataFormat(data.ApplicationStatus).Options,
                            ApplicationLineStatus: selectDataFormat(data.ApplicationLineStatus).Options
                        };
                    }
                }, {
                    id: 'Name',
                    name: 'Naati.Resources.Shared.resources.Name',
                    component: 'text-input',
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.Name'
                        }
                    },
                    formatData: arrayDataFormat
                },

                {
                    id: 'OrganisationName',
                    name: 'Naati.Resources.Shared.resources.OrganisationName',
                    component: 'text-input',
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.OrganisationName'
                        }
                    },
                    formatData: arrayDataFormat
                },

                {
                    id: 'Location',
                    name: 'Naati.Resources.Shared.resources.Location',
                    component: 'text-input',
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.Location'
                        }
                    },
                    formatData: arrayDataFormat
                },

                {
                    id: 'Qualification',
                    name: 'Naati.Resources.Shared.resources.Qualification',
                    component: 'text-input',
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.Qualification'
                        }
                    },
                    formatData: arrayDataFormat
                },

                {
                    id: 'Email',
                    name: 'Naati.Resources.Shared.resources.Email',
                    component: 'text-input',
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.Email'
                        }
                    },
                    formatData: arrayDataFormat
                },
                {
                    id: 'SessionName',
                    name: 'Naati.Resources.Shared.resources.SessionName',
                    component: 'text-input',
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.SessionName'
                        }
                    },
                    formatData: arrayDataFormat
                },
                {
                    id: 'ContactName',
                    name: 'Naati.Resources.Shared.resources.ContactName',
                    component: 'text-input',
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.ContactName'
                        }
                    },
                    formatData: arrayDataFormat
                },
                {
                    id: 'PractitionerNumber',
                    name: 'Naati.Resources.Shared.resources.PractitionerNumber',
                    component: 'text-input',
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.PractitionerNumber'
                        }
                    },
                    formatData: arrayDataFormat
                }, {
                    id: 'Nationality',
                    name: 'Naati.Resources.Shared.resources.Nationality',
                    component: 'text-input',
                    componentOptions: {
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.Nationality'
                        }
                    },
                    formatData: arrayDataFormat
                }, {
                    id: 'PersonName',
                    name: 'Naati.Resources.Shared.resources.PersonName',
                    component: 'text-input',
                    componentOptions: {
                        attr: {
                            maxlength: 50
                        },
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.PersonName'
                        }
                    }
                },
                {
                    id: 'GivenName',
                    name: 'Naati.Resources.Shared.resources.GivenName',
                    component: 'text-input',
                    componentOptions: {
                        attr: {
                            maxlength: 50
                        },
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.GivenName'
                        }
                    }
                },
                {
                    id: 'FamilyName',
                    name: 'Naati.Resources.Shared.resources.FamilyName',
                    component: 'text-input',
                    componentOptions: {
                        attr: {
                            maxlength: 50
                        },
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.FamilyName'
                        }
                    }
                },
                {
                    id: 'DateOfBirth',
                    name: 'Naati.Resources.Shared.resources.DateOfBirth',
                    component: 'date-range',
                    componentOptions: {
                        filterOptions: {
                            options: [{
                                value: ko.observable(enums.DateRanges.Custom),
                                text: ko.Localization('Naati.Resources.Test.resources.Custom')
                            }]
                        }
                    },
                    formatData: dateRangeFormat
                },
                {
                    id: 'CredentialIssueDate',
                    name: 'Naati.Resources.Shared.resources.CredentialIssueDate',
                    component: 'date-range',
                    componentOptions: {
                        filterOptions: {
                            options: [{
                                value: ko.observable(enums.DateRanges.Custom),
                                text: ko.Localization('Naati.Resources.Test.resources.Custom')
                            }]
                        }
                    },
                    formatData: dateRangeFormat
                },
                {
                    id: 'CredentialStartDate',
                    name: 'Naati.Resources.Shared.resources.CredentialStartDate',
                    component: 'date-range',
                    componentOptions: {
                        filterOptions: {
                            options: [{
                                value: ko.observable(enums.DateRanges.Custom),
                                text: ko.Localization('Naati.Resources.Test.resources.Custom')
                            }]
                        }
                    },
                    formatData: dateRangeFormat
                },
                {
                    id: 'CredentialEndDate',
                    name: 'Naati.Resources.Shared.resources.CredentialEndDate',
                    component: 'date-range',
                    componentOptions: {
                        filterOptions: {
                            options: [{
                                value: ko.observable(enums.DateRanges.Custom),
                                text: ko.Localization('Naati.Resources.Test.resources.Custom')
                            }]
                        }
                    },
                    formatData: dateRangeFormat
                },
                {
                    id: 'EndorsementDatePeriod',
                    name: 'Naati.Resources.Shared.resources.EndorsementPeriod',
                    component: 'date-range',
                    componentOptions: {
                        filterOptions: {
                            options: [{
                                value: ko.observable(enums.DateRanges.Custom),
                                text: ko.Localization('Naati.Resources.Test.resources.Custom')
                            }]
                        }
                    },
                    formatData: dateRangeFormat
                },
                {
                    id: 'CredentialRequestType',
                    name: 'Naati.Resources.Shared.resources.CredentialRequestType',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function() { functions.getLookup('CredentialType').then(this.options) },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'CredentialType',
                    name: 'Naati.Resources.Shared.resources.CredentialType',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('CredentialType').then(this.options) },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'TaskType',
                    name: 'Naati.Resources.Shared.resources.TaskType',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('TaskType').then(this.options) },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'Availability',
                    name: 'Naati.Resources.Shared.resources.Available',
                    component: 'radio-component',
                    componentOptions: {
                        checked: true
                    }
                },
                {
                    id: 'Title',
                    name: 'Naati.Resources.Shared.resources.Title',
                    component: 'text-input',
                    componentOptions: {
                        attr: {
                            maxlength: 255
                        },
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.Title'
                        }
                    }
                },
                {
                    id: 'TestMaterialStatus',
                    name: 'Naati.Resources.Shared.resources.Status',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('TestMaterialStatus').then(this.options) },
                        multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                    }
                },
                {
                    id: 'TestMaterialType',
                    name: 'Naati.Resources.Shared.resources.TestMaterialType',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('TestMaterialType').then(this.options) },
                        multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                    }
                },
                {
                    id: 'TestMaterialDomain',
                    name: 'Naati.Resources.TestMaterial.resources.TestMaterialDomain',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('TestMaterialDomain').then(this.options) },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'TestMaterialCredentialDomain',
                    name: 'Naati.Resources.Shared.resources.Credential',
                    component: 'group-component',
                    componentOptions: [
                        {
                            id: 'TestMaterialCredentialDomain-CredentialType',
                            name: 'Naati.Resources.Shared.resources.CredentialType',
                            component: 'select-component',
                            componentOptions: {
                                optionsValue: 'Id',
                                optionsText: 'DisplayName',
                                options: ko.observableArray(),
                                afterRender: function () { functions.getLookup('CredentialType').then(this.options); },
                                multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                            }
                        },
                        {
                            id: 'TestMaterialCredentialDomain-Domain',
                            name: 'Naati.Resources.TestMaterial.resources.TestMaterialDomain',
                            component: 'select-component',
                            componentOptions: {
                                optionsValue: 'Id',
                                optionsText: 'DisplayName',
                                options: ko.observableArray(),
                                dependant: true,
                                optionsLoader: functions.getCredentialTypeDomains,
                                multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                            }
                        }
                    ],
                    filterClass: ko.observable('col-lg-4'),
                    showLabel: false,
                    formatData: function (data) {
                        return {
                            Credential: { Data: data[0].data },
                            Domain: { Data: data[1].data }
                        };
                    }
                },
                {
                    id: 'TestSpecification',
                    name: 'Naati.Resources.Shared.resources.TestSpecification',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('TestSpecification').then(this.options) },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'TestStatusType',
                    name: 'Naati.Resources.Shared.resources.TestStatusType',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('TestStatusType').then(this.options) },
                        multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                    }
                },
                {
                    id: 'ExaminerStatusType',
                    name: 'Naati.Resources.Shared.resources.ExaminerStatusType',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('ExaminerStatusType').then(this.options) },
                        multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                    }
                },
                {
                    id: 'ApplicationType',
                    name: 'Naati.Resources.Shared.resources.ApplicationType',
                    component: 'select-component',
                    componentOptions: {
                        options: ko.observableArray(),
                        afterRender: function() { functions.getLookup('CredentialApplicationType').then(this.options) },
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'ActiveApplicationType',
                    name: 'Naati.Resources.Shared.resources.ApplicationType',
                    component: 'select-component',
                    helpText: ko.Localization('Naati.Resources.Application.resources.ApplicationTypeFilterTooltip'),
                    componentOptions: {
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('CredentialApplicationType').then(this.options) },
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                    }
                },
                {
                    id: 'PersonType',
                    name: 'Naati.Resources.Shared.resources.PersonType',
                    component: 'select-component',
                    componentOptions: {
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('PersonType').then(this.options) },
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                    }
                },
                {
                    id: 'State',
                    name: 'Naati.Resources.Shared.resources.State',
                    component: 'select-component',
                    componentOptions: {
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('State').then(this.options) },
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                    }
                },
                {
                    id: 'EnteredOffice',
                    name: 'Naati.Resources.Shared.resources.EnteredOffice',
                    component: 'select-component',
                    componentOptions: {
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('Office').then(this.options) },
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                    }
                },
                {
                    id: 'ApplicationOwner',
                    name: 'Naati.Resources.Shared.resources.ApplicationOwner',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function() { functions.getLookup('ApplicationOwner').then(this.options) },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'ApplicationReference',
                    name: 'Naati.Resources.Shared.resources.ApplicationReference',
                    component: 'text-input',
                    componentOptions: {
                        attr: {
                            maxlength: 50
                        },
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.ApplicationReference'
                        }
                    }
                },
                {
                    id: 'MaterialId',
                    name: 'Naati.Resources.Shared.resources.MaterialId',
                    component: 'text-input',
                    componentOptions: {
                        multiple: true,
                        attr: {
                        },
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.MaterialId'
                        }
                    },
                    formatData: arrayDataFormat
                },
                {
                    id: 'SourceTestMaterialId',
                    name: 'Naati.Resources.Shared.resources.SourceTestMaterialId',
                    component: 'text-input',
                    componentOptions: {
                        multiple: true,
                        attr: {
                        },
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.SourceTestMaterialId'
                        }
                    },
                    formatData: arrayDataFormat
                },
                {
                    id: 'EmailMessageId',
                    name: 'Naati.Resources.Shared.resources.EmailMessageId',
                    component: 'text-input',
                    componentOptions: {
                        multiple: true,
                        attr: {
                        },
                        resattr: {
                            placeholder: 'Naati.Resources.Shared.resources.EmailMessageId'
                        }
                    },
                    formatData: arrayDataFormat
                },
                {
                    id: 'ActiveApplication',
                    name: 'Naati.Resources.Shared.resources.ActiveApplication',
                    component: 'radio-component',
                    componentOptions: {
                        checked: true
                    }
                },
                {
                    id: 'AutoCreatedApplication',
                    name: 'Naati.Resources.Shared.resources.AutoCreatedApplication',
                    component: 'radio-component',
                    componentOptions: {
                        checked: true
                    }
                },
                {
                    id: 'AutoCreatedCredentialRequest',
                    name: 'Naati.Resources.Shared.resources.AutoCreatedCredentialRequest',
                    component: 'radio-component',
                    componentOptions: {
                        checked: true
                    }
                },
                {
                    id: 'ProductCardClaim',
                    name: 'Naati.Resources.Shared.resources.ProductCardClaim',
                    component: 'radio-component',
                    helpText: ko.Localization('Naati.Resources.Shared.resources.ProductCardClaimFilterTooltip'),
                    componentOptions: {
                        checked: true
                    }
                },
                {
                    id: 'CertificationCredentialType',
                    name: 'Naati.Resources.Shared.resources.CertificationCredentialType',
                    component: 'radio-component',
                    componentOptions: {
                        checked: true
                    }
                },
                {
                    id: 'ReadyToIssueResults',
                    name: 'Naati.Resources.Shared.resources.ReadyToIssueResults',
                    component: 'radio-component',
                    componentOptions: {
                        checked: true
                    }
                },
                {
                    id: 'AllowIssue',
                    name: 'Naati.Resources.Shared.resources.AllowIssue',
                    component: 'radio-component',
                    componentOptions: {
                        checked: true
                    }
                },
                {
                    id: 'TestTaskType',
                    name: 'Naati.Resources.Shared.resources.TestTaskType',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('TaskType').then(this.options); },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'TestMaterialRequestPanel',
                    name: 'Naati.Resources.Shared.resources.Panel',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('Panel').then(this.options); },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'TestMaterialType',
                    name: 'Naati.Resources.Shared.resources.TestMaterialType',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('TestMaterialType').then(this.options); },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'TestMaterialRequestStatus',
                    name: 'Naati.Resources.Shared.resources.TestMaterialRequestStatus',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () {
                            functions.getLookup('MaterialRequestStatus').then(this.options);
                        },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'RoundStatus',
                    name: 'Naati.Resources.Shared.resources.RoundStatus',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('MaterialRequestRoundStatus').then(this.options); },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'DueDate',
                    name: 'Naati.Resources.Shared.resources.DueDate',
                    component: 'date-range',
                    componentOptions: {
                        filterOptions: {
                            options: [{
                                value: ko.observable(enums.DateRanges.Custom),
                                text: ko.Localization('Naati.Resources.Test.resources.Custom')
                            }]
                        }
                    },
                    formatData: dateRangeFormat
                },
                {
                    id: 'Overdue',
                    name: 'Naati.Resources.Shared.resources.Overdue',
                    component: 'radio-component',
                    componentOptions: {
                        checked: true
                    }
                },
                {
                    id: 'OwnerNaatiUser',
                    name: 'Naati.Resources.Shared.resources.OwnerNaatiUser',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('NaatiUser').then(this.options) },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },  
                {
                    id: 'Credential',
                    name: 'Naati.Resources.Shared.resources.Credential',
                    component: 'group-component',
                    componentOptions: [
                        {
                            id: 'Credential-CredentialType',
                            name: 'Naati.Resources.Shared.resources.CredentialType',
                            component: 'select-component',
                            componentOptions: {
                                optionsValue: 'Id',
                                optionsText: 'DisplayName',
                                options: ko.observableArray(),
                                afterRender: function () { functions.getLookup('CredentialType').then(this.options); },
                                multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                            }
                        },
                        {
                            id: 'Credential-CredentialSkill',
                            name: 'Naati.Resources.Shared.resources.CredentialSkill',
                            component: 'select-component',
                            componentOptions: {
                                optionsValue: 'Id',
                                optionsText: 'DisplayName',
                                options: ko.observableArray(),
                                dependant: true,
                                optionsLoader: functions.getCredentialTypeSkills,
                                multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                            }
                        }
                    ],
                    filterClass: ko.observable('col-lg-4'),
                    showLabel: false,
                    formatData: function (data) {
                        return {
                            Credential: { Data: data[0].data },
                            CredentialSkill: { Data: data[1].data }
                        };
                    }
                },
                {
                    id: 'MaterialRequest',
                    name: 'Naati.Resources.Shared.resources.MaterialRequest',
                    component: 'group-component',
                    componentOptions: [
                        {
                            id: 'MaterialRequest-Name',
                            name: 'Naati.Resources.MaterialRequest.resources.MaterialRequestName',
                            component: 'text-input',
                            componentOptions: {
                                attr: {
                                    maxlength: 50
                                },
                                resattr: {
                                    placeholder: 'Naati.Resources.MaterialRequest.resources.MaterialRequestName'
                                }
                            }
                        },
                        {
                            id: 'MaterialRequest-Status',
                            name: 'Naati.Resources.MaterialRequest.resources.Status',
                            component: 'select-component',
                            componentOptions: {
                                optionsValue: 'Id',
                                optionsText: 'DisplayName',
                                options: ko.observableArray(),
                                afterRender: function () { functions.getLookup('MaterialRequestStatus').then(this.options) },
                                multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                            }
                        },
                        {
                            id: 'MaterialRequest-CredentialType',
                            name: 'Naati.Resources.MaterialRequest.resources.RequestCredentialType',
                            component: 'select-component',
                            componentOptions: {
                                optionsValue: 'Id',
                                optionsText: 'DisplayName',
                                options: ko.observableArray(),
                                afterRender: function () { functions.getLookup('CredentialType').then(this.options) },
                                multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                            }
                        },
                        {
                            id: 'MaterialRequest-TaskType',
                            name: 'Naati.Resources.Shared.resources.TaskType',
                            component: 'select-component',
                            componentOptions: {
                                optionsValue: 'Id',
                                optionsText: 'DisplayName',
                                options: ko.observableArray(),
                                dependant: true,
                                optionsLoader: functions.getCredentialTypeTaskTypes,
                                multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                            }
                        },
                        
                    ],
                    filterClass: ko.observable('col-lg-12'),
                    showLabel: true,
                    formatData: function (data) {
                        return {
                            Title: { Data: data[0].data },
                            Status: { Data: data[1].data },
                            CredentialType: { Data: data[2].data },
                            TaskType: { Data: data[3].data },
                        };
                    }
                },
                {
                    id: 'CredentialTestSession',
                    name: 'Naati.Resources.Shared.resources.Credential',
                    component: 'group-component',
                    componentOptions: [
                        {
                            id: 'Credential-CredentialType-TestSession',
                            name: 'Naati.Resources.Shared.resources.CredentialType',
                            component: 'select-component',
                            componentOptions: {
                                optionsValue: 'Id',
                                optionsText: 'DisplayName',
                                options: ko.observableArray(),
                                afterRender: function() { functions.getLookup('CredentialTypeTestSession').then(this.options) },
                                multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                            }
                        },
                        {
                            id: 'Credential-CredentialSkill-TestSession',
                            name: 'Naati.Resources.Shared.resources.CredentialSkill',
                            component: 'select-component',
                            componentOptions: {
                                optionsValue: 'Id',
                                optionsText: 'DisplayName',
                                options: ko.observableArray(),
                                dependant: true,
                                optionsLoader: functions.getCredentialTypeSkillsTestSession,
                                multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                            }
                        }
                    ],
                    filterClass: ko.observable('col-lg-4'),
                    showLabel: false,
                    formatData: function(data) {
                        return {
                            Credential: { Data: data[0].data },
                            CredentialSkill: { Data: data[1].data }
                        };
                    }
                },
                {
                    id: 'IncludeCompletedSessions',
                    name: 'Naati.Resources.Shared.resources.IncludeCompletedSessions',
                    component: 'radio-component',
                    componentOptions: {
                        checked: true
                    }
                },
                {
                    id: 'CredentialStatusType',
                    name: 'Naati.Resources.Shared.resources.CredentialStatusType',
                    component: 'select-component',
                    componentOptions: {
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('CredentialStatusType').then(this.options) },
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                    }
                },
                {
                    id: 'CredentialRequestStatusType',
                    name: 'Naati.Resources.Shared.resources.CredentialRequestStatus',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function() {
                             functions.getLookup('CredentialRequestStatusType').then(function(data) {
                                 return $.grep(data,
                                     function(e) {
                                         return e.DisplayName !== "Deleted";
                                     });
                             }).then(this.options);
                        },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'CredentialRequestStatusTypeForSummary',
                    name: 'Naati.Resources.Shared.resources.CredentialRequestStatus',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () {
                            functions.getLookup('CredentialRequestStatusType').then(function (data) {
                                return $.grep(data,
                                    function (e) {
                                        return e.DisplayName !== "Deleted" && e.DisplayName !== "Draft" && e.DisplayName !== "Cancelled";
                                    });
                            }).then(this.options);
                        },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'ApplicationStatusType',
                    name: 'Naati.Resources.Shared.resources.ApplicationStatus',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function() {
                            functions.getLookup('CredentialApplicationStatusType').then(function(data) {
                                return $.grep(data,
                                    function(e) {
                                        return e.DisplayName !== "Deleted";
                                    });
                            }).then(this.options);
                        },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'PreferredTestLocation',
                    name: 'Naati.Resources.Shared.resources.PreferredTestLocation',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('TestLocation').then(this.options) },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                    }
                },
                {
                    id: 'TestLocation',
                    name: 'Naati.Resources.Shared.resources.PreferredTestLocation',
                    component: 'group-component',
                    componentOptions: [
                        {
                            id:'TestLocation-PreferredTestLocation',
                            name: 'Naati.Resources.Shared.resources.PreferredTestLocation',
                            component: 'select-component',
                            componentOptions: {
                                optionsValue: 'Id',
                                optionsText: 'DisplayName',
                                options: ko.observableArray(),
                                afterRender: function () { functions.getLookup('TestLocation').then(this.options) },
                                multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                            }
                        },
                        {
                            id: 'TestLocation-VenuesShowingInactive',
                            name: 'Naati.Resources.Shared.resources.Venue',
                            component: 'select-component',
                            componentOptions: {
                                optionsValue: 'Id',
                                optionsText: 'DisplayName',
                                options: ko.observableArray(),
                                dependant: true,
                                optionsLoader: functions.getVenuesShowingInactive,
                                multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                            }
                        }

                    ],
                    filterClass: ko.observable('col-lg-4'),
                    showLabel: false,
                    formatData: function(data) {
                        return {
                            PreferredTestLocation: { Data: data[0].data },
                            TestVenue: { Data: data[1].data }
                        };
                    }
                },
                {
                    id: 'IntendedTestSession',
                    name: 'Naati.Resources.Shared.resources.IntendedTestSession',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('IntendedTestSession').then(this.options) },
                        multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
                    }
				},
				{
					id: 'Sponsor',
					name: 'Naati.Resources.NaatiEntity.resources.Sponsor',
					component: 'select-component',
					componentOptions: {
						optionsValue: 'Id',
						optionsText: 'DisplayName',
						options: ko.observableArray(),
						afterRender: function () { functions.getLookup('Institution').then(this.options) },
						multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: null }
					}
                },
                {
                    id: 'Organisation',
                    name: 'Naati.Resources.Shared.resources.Organisation',
                    component: 'select-component',
                    componentOptions: {
                        optionsValue: 'Id',
                        optionsText: 'DisplayName',
                        options: ko.observableArray(),
                        afterRender: function () { functions.getLookup('Institution').then(this.options) }
                    }
                },
				{
					id: 'LanguageGroup',
					name: 'Naati.Resources.Language.resources.Group',
					component: 'select-component',
					componentOptions: {
						optionsValue: 'Id',
						optionsText: 'DisplayName',
						options: ko.observableArray(),
						afterRender: function () { functions.getLookup('LanguageGroup').then(this.options) },
						multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
					}
                },
				{
					id: 'SkillType',
					name: 'Naati.Resources.Skill.resources.SkillType',
					component: 'select-component',
					componentOptions: {
						optionsValue: 'Id',
						optionsText: 'DisplayName',
						options: ko.observableArray(),
						afterRender: function () { functions.getLookup('SkillType').then(this.options) },
						multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
					}
				},
				{
					id: 'DirectionType',
					name: 'Naati.Resources.Shared.resources.Direction',
					component: 'select-component',
					componentOptions: {
						optionsValue: 'Id',
						optionsText: 'DisplayName',
						options: ko.observableArray(),
						afterRender: function () { functions.getLookup('DirectionType').then(this.options) },
						multiselect: { enableFiltering: false, enableCaseInsensitiveFiltering: null }
					}
				},
				{
                    id: 'SystemActionType',
					name: 'Naati.Resources.Shared.resources.Action',
					component: 'select-component',
					componentOptions: {
						optionsValue: 'Id',
						optionsText: 'DisplayName',
						options: ko.observableArray(),
						afterRender: function () { functions.getLookup('SystemActionType').then(this.options) },
                        multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
					}
                },
                {
                    id: 'Deceased',
                    name: 'Naati.Resources.Shared.resources.Deceased',
                    component: 'radio-component',
                    componentOptions: {
                        checked: true
                    }
                },
                {
                    id: 'RolePlayer',
                    name: 'Naati.Resources.Shared.resources.RolePlayer',
                    component: 'group-component',
                    componentOptions: [
                        {
                            id: 'RolePlayer-CredentialType',
                            name: 'Naati.Resources.Shared.resources.CredentialType',
                            component: 'select-component',
                            componentOptions: {
                                optionsValue: 'Id',
                                optionsText: 'DisplayName',
                                options: ko.observableArray(),
                                afterRender: function () { functions.getLookup('CredentialType').then(this.options) },
                                multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                            }
                        },
                        {
                            id: 'RolePlayer-Panel',
                            name: 'Naati.Resources.Shared.resources.Panel',
                            component: 'select-component',
                            componentOptions: {
                                optionsValue: 'Id',
                                optionsText: 'DisplayName',
                                options: ko.observableArray(),
                                dependant: true,
                                optionsLoader: function () { return functions.getLookup('Panel')},
                                multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                            }
                        }
                    ],
                    filterClass: ko.observable('col-lg-4'),
                    showLabel: true,
                    formatData: function (data) {
                        return {
                            CredentialType: { Data: data[0].data },
                            Panel: { Data: data[1].data }
                        };
                    }
                },
                {
                    id: 'PersonExaminer',
                    name: 'Naati.Resources.Shared.resources.Examiner',
                    component: 'group-component',
                    componentOptions: [
                        {
                            id: 'Examiner-CredentialType',
                            name: 'Naati.Resources.Shared.resources.CredentialType',
                            component: 'select-component',
                            componentOptions: {
                                optionsValue: 'Id',
                                optionsText: 'DisplayName',
                                options: ko.observableArray(),
                                afterRender: function () { functions.getLookup('CredentialType').then(this.options) },
                                multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                            }
                        },
                        {
                            id: 'Examiner-Panel',
                            name: 'Naati.Resources.Shared.resources.Panel',
                            component: 'select-component',
                            componentOptions: {
                                optionsValue: 'Id',
                                optionsText: 'DisplayName',
                                options: ko.observableArray(),
                                dependant: true,
                                optionsLoader: function () { return functions.getLookup('Panel') },
                                multiselect: { enableFiltering: true, enableCaseInsensitiveFiltering: true }
                            }
                        }
                    ],
                    filterClass: ko.observable('col-lg-4'),
                    showLabel: true,
                    formatData: function (data) {
                        return {
                            CredentialType: { Data: data[0].data },
                            Panel: { Data: data[1].data }
                        };
                    }
                },
                {
                    id: 'NewCandidatesOnly',
                    name: 'Naati.Resources.Test.resources.NewCandidatesOnly',
                    component: 'radio-component',
                    componentOptions: {}
                },
                {
                    id: 'IsActive',
                    name: 'Naati.Resources.Test.resources.IsActive',
                    component: 'radio-component',
                    componentOptions: {}
                },
            ];
        }
    };

    return vm;
});
