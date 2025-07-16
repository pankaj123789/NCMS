define(['plugins/dialog', 'knockout'], function (dialog, ko) {

    var modal = function(moduleId)
    {
       this.moduleId = moduleId;
    };

    modal.prototype.ok = function()
    {
       dialog.close(this);
    };

    modal.show = function(moduleId)
    {
       return dialog.show(new modal(moduleId));
    };

    return modal;
});