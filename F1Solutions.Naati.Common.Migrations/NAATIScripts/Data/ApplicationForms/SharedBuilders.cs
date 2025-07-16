using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms.Base;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms
{
    public class SharedBuilders
    {
        internal IEnumerable<IFormUserTypeBuilder> UserTypes { get; }
        internal IEnumerable<IFormActionTypeBuilder> ActionTypes { get; }
        internal IEnumerable<IFormAnswerTypeBuilder> AnswerTypes { get; }
        
        public SharedBuilders()
        {
            UserTypes = new List<IFormUserTypeBuilder>
            {
                FormUserTypeBuilder.Create("AnonymousUser", "Anonymous User"),
                FormUserTypeBuilder.Create("LoggedInUser", "Logged In User"),
                FormUserTypeBuilder.Create("PractitionerUser", "Practitioner User"),
                FormUserTypeBuilder.Create("RecertificationUser", "Recertification User"),
                FormUserTypeBuilder.Create("NonPractitionerUser", "Non Practitioner User"),
            };

            ActionTypes = new List<IFormActionTypeBuilder>
            {
                FormActionTypeBuilder.Create("Delete", "Delete"),
                FormActionTypeBuilder.Create("Redirect", "Redirect"),
                FormActionTypeBuilder.Create("Submit", "Submit"),
            };

            AnswerTypes = new List<IFormAnswerTypeBuilder>
            {
                FormAnswerTypeBuilder.Create("RadioOptions", "Radio Options", true),
                FormAnswerTypeBuilder.Create("CheckOptions", "Check Option", true),
                FormAnswerTypeBuilder.Create("Input", "Input", allowText: true),
                FormAnswerTypeBuilder.Create("Date", "Date", allowText: true),
                FormAnswerTypeBuilder.Create("PersonVerification", "Person Verification"),
                FormAnswerTypeBuilder.Create("PersonDetails", "Your Details"),
                FormAnswerTypeBuilder.Create("LanguageSelector", "Language Selector"),
                FormAnswerTypeBuilder.Create("CredentialSelector", "Credential Selector"),
                FormAnswerTypeBuilder.Create("TestLocation", "Test Location"),
                FormAnswerTypeBuilder.Create("ProductSelector", "Product Selector"),
                FormAnswerTypeBuilder.Create("DocumentUpload", "Document Upload"),
                FormAnswerTypeBuilder.Create("PersonPhoto", "Person Photo"),
                FormAnswerTypeBuilder.Create("CountrySelector", "Country Selector"),
                FormAnswerTypeBuilder.Create("Email", "Email", allowText: true),
                FormAnswerTypeBuilder.Create("TestSessions", "Test Sessions"),
                FormAnswerTypeBuilder.Create("CredentialSelectorUpgradeAndSameLevel", "Credential Selector"),
                FormAnswerTypeBuilder.Create("Fees", "Fees", allowText: true),
                FormAnswerTypeBuilder.Create("RecertificationCredentialSelector", "Recertification Selector"),
                FormAnswerTypeBuilder.Create("EndorsedQualification", "Endorsed Qualification", allowText: true),
                FormAnswerTypeBuilder.Create("EndorsedQualificationInstitution", "Endorsed Qualification Institution", allowText: true),
                FormAnswerTypeBuilder.Create("EndorsedQualificationLocation", "Endorsed Qualification Location", allowText: true),
            };
        }

        public IEnumerable<IFormBuilder> GetBuilders()
        {
            return UserTypes.Cast<IFormBuilder>().Concat(ActionTypes).Concat(AnswerTypes);
        }

        public string AplicationsUrl(string url)
        {
            return $"[[ExternalUrl_Applications_URL]]//{url}";
        }

        public string NaatiUrl(string url)
        {
            return $"[[ExternalUrl_NAATI_URL]]/{url}";
        }


    }
}
