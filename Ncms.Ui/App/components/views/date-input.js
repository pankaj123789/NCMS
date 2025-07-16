define([
    'components/views/text-input'
], function (textInput) {
    function ViewModel(params) {
        textInput.call(this, params);
    }

    return ViewModel;
});
