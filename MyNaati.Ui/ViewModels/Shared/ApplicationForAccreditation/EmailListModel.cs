using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation
{
    public class EmailListModel
    {
     

        public EmailListModel()
        {
            EmailForCreate = new EmailModel();
            EmailForEdit = new EmailModel();
            CurrentEmails = new List<EmailModel>();
        }

        public EmailModel EmailForCreate { get; set; }
        public EmailModel EmailForEdit { get; set; }

        [AtLeastOnePreferredEmail(ErrorMessage = "You must have one preferred email address.")]
        public IList<EmailModel> CurrentEmails { get; set; }

        public string ValidateEmailUniqueness(EmailModel editModel)
        {
           // if (CurrentEmails.Any(p => p.Id != editModel.Id && p.ContactTypeId == editModel.ContactTypeId))
            if (CurrentEmails.Any(p => p.Id != editModel.Id))
            {
                //Get names from lists instead of lookupProvider so they're sure to match up.
               
                return string.Format("You may only have one active email address per contact type. Edit the {0} email address, or select a different contact type.",
                  //  contactType);
                    string.Empty);
            }

            return null;
        }

        public string EnsureOnlyOnePreferred(EmailModel favouredEmail)
        {
            if (favouredEmail.IsPreferred == false)
            {
                favouredEmail = this.CurrentEmails.FirstOrDefault(e => e.IsPreferred && e.Id != favouredEmail.Id);
            }

            if (favouredEmail == null)
            {
                return "You must have one preferred email address. Tip: First mark another email address as preferred.";
            }

            // There can be only one!
            foreach (var email in this.CurrentEmails.Where(e => e.Id != favouredEmail.Id))
            {
                email.IsPreferred = false;
            }

            return null;
        }
    }

    public class EmailModel
    {
        private ILookupProvider mLookupProvider;

        public EmailModel()
        {
            mLookupProvider = ServiceLocator.Resolve<ILookupProvider>();
        }

        public int Id { get; set; }
        
        [DisplayName("Email")]
        [Required(ErrorMessage = "The email address does not have a valid email address structure.")]
        [RegularExpression(@"^([a-zA-Z0-9_'+*$%\^&!\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9:]{2,4})+$",
                           ErrorMessage = "The email address does not have a valid email address structure.")]
        [StringLength(200, ErrorMessage = "Email address must be at most 200 characters long.")]
        public string Email { get; set; }

        [DisplayName("Preferred email")]
        public bool IsPreferred { get; set; }
   
    }

    public class AtLeastOnePreferredEmail : MinimumItemCountAttribute
    {
        public AtLeastOnePreferredEmail()
            : base(1, (e => ((EmailModel)e).IsPreferred), false, false)
        {
        }
    }
}
