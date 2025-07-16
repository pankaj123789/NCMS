using System.Collections.Generic;
using Ncms.Contracts.Models.Person;
using Ncms.Contracts.Models.System;

namespace Ncms.Contracts.Models.Application
{
    public class CredentialApplicationDetailedModel : SystemActionModel
    {
        public PersonBasicModel ApplicantDetails { get; set; }
        public CredentialApplicationInfoModel ApplicationInfo { get; set; }
        public CredentialApplicationTypeModel ApplicationType { get; set; }
        public LookupTypeDetailedModel ApplicationStatus { get; set; }
        public IList<ApplicationNoteModel> Notes { get; set; }
        public IList<CredentialRequestModel> CredentialRequests { get; set; }
        public IList<CredentialApplicationFieldModel> Fields { get; set; }
        public IList<PersonNoteModel> PersonNotes { get; set; }
        public IList<PdActivityFieldModel> PdActivities { get; set; } = new List<PdActivityFieldModel>();
        public IList<CredentialWorkflowFeeModel> CredentialWorkflowFees { get; set; }
        public IList<TestComponentModel> StandardTestComponentModelsToUpdate { get; set; } = new List<TestComponentModel>();
        public IList<Test.TestComponentModel> RubricTestComponentModelsToUpdate { get; set; } = new List<Test.TestComponentModel>();
        public RecertificationModel Recertification { get; set; }
    }
}