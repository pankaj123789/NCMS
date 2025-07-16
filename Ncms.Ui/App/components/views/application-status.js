define([
], function () {
    function ViewModel(params) {
        var self = this;
        params.component = self;

        self.applicationType = params.ApplicationType || {};
        self.applicationStatus = params.ApplicationStatus || {};
        self.applicationLineStatus = params.ApplicationLineStatus || {};

        self.getJson = function () {
            return {
                ApplicationType: self.applicationType.component.getJson(),
                ApplicationStatus: self.applicationStatus.component.getJson(),
                ApplicationLineStatus: self.applicationLineStatus.component.getJson()
            };  
        };
    }

    return ViewModel;
});