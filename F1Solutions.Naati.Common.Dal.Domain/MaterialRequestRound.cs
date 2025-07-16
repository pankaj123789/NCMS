using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class MaterialRequestRound : EntityBase
    {


        public virtual MaterialRequest MaterialRequest { get; set; }
        public virtual DateTime DueDate { get; set; }

        public virtual int RoundNumber { get; set; }
        public virtual DateTime? SubmittedDate { get; set; }
        public virtual MaterialRequestRoundStatusType MaterialRequestRoundStatusType { get; set; }
        public virtual DateTime RequestedDate { get; set; }
        public virtual DateTime StatusChangeDate { get; set; }
        public virtual User ModifiedUser { get; set; }

       
    }
}
