define([
    'services/examiner-data-service',
'services/panel-data-service'
], function (examinerService, panelService) {
    function ViewModel(params) {
        var self = this;
        params.component = self;

        self.panel = params.Panel || {};
        self.panel.selectedOptions = self.panel.selectedOptions || [];
        self.panel.selectedOptions = ko.observableArray(self.panel.selectedOptions);
        self.panel.options = ko.observableArray();
        self.panel.selectedOptions.subscribe(function () { self.loadOptions(); });
        self.panel.options.subscribe(function () { self.loadOptions(); })

        self.examiner = params.Examiner || {};
        self.examiner.options = ko.observableArray();
        self.examiner.selectedOptions = self.examiner.selectedOptions || [];
        self.examiner.selectedOptions = ko.observableArray(self.examiner.selectedOptions);

        self.examinerStatus = params.ExaminerStatus || {};
        self.submitted = $.extend(params.Submitted || {}, { allowNone: true });
        self.dueBy = $.extend(params.DueBy || {}, { allowNone: true });
 

        self.loadOptions = function () {
            var panels = self.panel.selectedOptions();
            if (!panels.length) {
                panels = ko.utils.arrayMap(self.panel.options(), function (item) {
                    return item.Id;
                });
            }
            if (!panels.length) {
                return;
            }
            panelService.post({ PanelIds: panels }, 'GetPanelMembershipLookup').then(self.examiner.options);
            
        };

        self.getJson = function () {
            return {
                Panel: self.panel.component.getJson(),
                Examiner: self.examiner.component.getJson(),
                ExaminerStatus: self.examinerStatus.component.getJson(),
                Submitted: self.submitted.component.getJson(),
                DueBy: self.dueBy.component.getJson(),
            };
        };

        self.examiner.selectedOptions.extend({
            required: {
                onlyIf: function () {
                    return self.panel.selectedOptions().length ? true : false;
                }
            }
        });

        var validation = ko.validatedObservable(self.examiner);
        self.validateFilter = function () {
            var isValid = validation.isValid();
            if (!isValid) {
                validation.errors.showAllMessages();
            }
            return isValid;
        };

        self.loadOptions();
    }

    return ViewModel;
});