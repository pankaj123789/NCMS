using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.System;

namespace Ncms.Contracts
{
    public interface IMaterialRequestWizardLogicService
    {
        GenericResponse<IEnumerable<MaterialRequestWizardStep>> GetMaterialRequestSteps(int actionId, int materialRequestId,int panelId);
        GenericResponse<IEnumerable<MaterialRequestWizardStep>> GetMaterialRequestRoundSteps(int actionId, int materialRequestsRoundId);
        GenericResponse<IEnumerable<SystemActionNameModel>> GetValidMaterialRequestActions(int materialRequestStatusTypeId);
        GenericResponse<IEnumerable<SystemActionNameModel>> GetValidMaterialRequestRoundActions(int materialRequestRoundStatusTypeId);
        GenericResponse<NoteFieldRules> GetSystemNoteFieldRules(int actionId);
    }
}
