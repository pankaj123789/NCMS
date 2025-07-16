using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace F1Solutions.NAATI.ePortal.Web.ViewModels.Shared
{
    public interface IWizardModel
    {
        DeliveryDetailsEditModel DeliveryDetailsModel { get; set; }
        PaymentMethodEditModel PaymentMethodModel { get; set; }
        List<WizardStep> Steps { get; set; }
        Guid WizardId { get; set; }
    }
}