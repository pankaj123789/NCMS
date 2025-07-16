using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;
using MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation;

namespace MyNaati.Ui.ViewModels.ApplicationByCourseWizard
{
    /// <summary>
    /// This model interprets and decorates the data on the source model as 
    /// necessary for our application form view to be as dumb as possible.
    /// </summary>
    public class ApplicationByCourseFormModel : ApplicationFormModel
    {
        private WizardModel mSourceModel;
        public ApplicationByCourseFormModel(WizardModel sourceModel, ILookupProvider lookupProvider)
            : base (sourceModel, lookupProvider)
        {
            mSourceModel = sourceModel;
        }
    }
}