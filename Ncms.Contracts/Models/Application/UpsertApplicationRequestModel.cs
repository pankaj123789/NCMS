using System.Collections.Generic;

namespace Ncms.Contracts.Models.Application
{
    public class UpsertApplicationRequestModel
    {
        public CredentialApplicationInfoModel ApplicationInfo { get; set; }
        public IList<CredentialApplicationSectionModel> Sections { get; set; }
        public IList<CredentialRequestModel> CredentialRequests { get; set; }
        public IList<NoteModel> Notes { get; set; }
        public IList<NoteModel> PersonNotes { get; set; }
        public IList<TestComponentModel> StandardTestComponents { get; set; }
        public IList<Test.TestComponentModel> RubricTestComponents { get; set; }
        public IList<PdActivityFieldModel> PdActivities { get; set; }
        public RecertificationModel Recertification { get; set; }
        public ProcessFeeModel ProcessFee { get; set; }
    }
}
