using System.Collections.Generic;
using System.Linq;
using Ncms.Contracts.Models.Person;
using Ncms.Contracts.Models.System;

namespace Ncms.Contracts.Models.RolePlayer
{
    public class RolePlayerActionModel : SystemActionModel
    {
        public PersonBasicModel PersonDetails { get; set; }
        public IList<PersonNoteModel> PersonNotes { get; set; }
        public TestSessionRolePlayerModel TestSessionRolePlayer { get; set; }
      
    }

    public class RolePlayerUpdateWizardModel : SystemActionWizardModel
    {
        public virtual StepDataModel NotesStep => null;
       
        public override string PublicNotes => NotesStep?.Data?.PublicNote?.Value;

        public override string PrivateNotes => NotesStep?.Data?.PrivateNote?.Value;
    }

    public class AssignRolePlayersWizardModel : SystemActionWizardModel
    {
        public TestSessionRolePlayerAvailabilityModel RolePlayer { get;  set; }

        public virtual int TestSessionId => Data.TestSessionId;
        public int SkillId => SkillStep?.Data?.SkillId;
        public int TestSpecificationId => TestSpecificationStep?.Data?.TestSpecificationId;
        public virtual StepDataModel NotesStep => Steps.FirstOrDefault(x => x.Id == (int)AllocateRolePlayersWizardSteps.Notes);
        public StepDataModel SkillStep => Steps.FirstOrDefault(x => x.Id == (int)AllocateRolePlayersWizardSteps.Skill);
        public StepDataModel TestSpecificationStep => Steps.FirstOrDefault(x => x.Id == (int)AllocateRolePlayersWizardSteps.TestSpecification);
        public override string PublicNotes => NotesStep?.Data?.PublicNote?.Value;
        public override StepDataModel[] Steps => (StepDataModel[])Data.Steps.ToObject<StepDataModel[]>();

        public StepDataModel SendEmailStep => Steps.FirstOrDefault(x => x.Id == (int)AllocateRolePlayersWizardSteps.SendEmail);
        public override bool SendEmail => (bool)SendEmailStep.Data;
        public override string PrivateNotes => NotesStep?.Data?.PrivateNote?.Value;
    }
        

    public class RolePlayerBulkAssignmentWizard : AssignRolePlayersWizardModel
    {
        public StepDataModel AllocateRolePlayersStep => Steps.FirstOrDefault(x => x.Id == (int)AllocateRolePlayersWizardSteps.AllocateRolePlayers);
        public TestSessionRolePlayerAvailabilityModel[] RolePlayers => (TestSessionRolePlayerAvailabilityModel[])AllocateRolePlayersStep.Data.RolePlayers?.ToObject<TestSessionRolePlayerAvailabilityModel[]>() ?? new TestSessionRolePlayerAvailabilityModel[] { };

    }

    public class UpsertRolePlayerResultModel
    {
        public int TestSessionRolePLayerId { get; set; }
    }

    public class RolePlayerActionOutput : SytemActionOutput<UpsertRolePlayerResultModel, EmailMessageModel>
    {
    }

}
