using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;

namespace Ncms.Contracts.Models.System
{
    public class StepDataModel
    {
        public int Id { get; set; }
        public dynamic Data { get; set; }
    }
    public enum SystemActionSource
    {
        Ncms = 1,
        MyNaati = 2,
    }

    public abstract class SystemActionWizardModel
    {
        public virtual SystemActionSource Source => SystemActionSource.Ncms;
        public virtual int ActionType { get; set; }
        public virtual dynamic Data { get; set; }
        public virtual StepDataModel[] Steps => (StepDataModel[])Data?.ToObject<StepDataModel[]>() ?? new StepDataModel[0];

        public virtual bool SendEmail => true;

        public virtual string PublicNotes => null;
        public virtual string PrivateNotes => null;
    }

    public abstract class SystemActionModel
    {

    }

    public abstract class SytemActionOutput<TUpsertType, TEmailMessageModelType>
        where TUpsertType : new()
        where TEmailMessageModelType : EmailMessageModel
    {
        public IList<TEmailMessageModelType> PendingEmails { get; set; }
        public GenericResponse<TUpsertType> UpsertResults { get; set; }
    }

}
