define([], function () {
    function ViewModel(params) {
        var self = this;

        params.component = self;

        self.test = params.test || {};
        self.personPhotoOptions = {
            naatiNumber: self.test.NaatiNumber
        };
    }

    return ViewModel;
});
