using Ncms.Contracts.Models.System;

namespace Ncms.Contracts.Models.TestMaterial
{
    public class TestMaterialActionModel : SystemActionModel
    {
    }

    public class TestMaterialWizardModel : SystemActionWizardModel
    {
        
    }

    public class TestMaterialActionOutput : SytemActionOutput<UpsertTestMaterialResultModel, EmailMessageModel>
    {
    }

    public class UpsertTestMaterialResultModel
    {
        //public int CredentialApplicationId { get; set; }
        //public IEnumerable<int> CredentialApplicationFieldDataIds { get; set; }
        //public IEnumerable<int> CredentialRequestIds { get; set; }
        //public IEnumerable<int> NoteIds { get; set; }
    }
}
