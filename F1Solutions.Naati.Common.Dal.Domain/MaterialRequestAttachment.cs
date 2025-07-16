namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class MaterialRequestRoundAttachment : EntityBase
    {
        public virtual  MaterialRequestRound MaterialRequestRound  { get; set;  }
        public virtual  StoredFile StoredFile { get; set;  }
        public virtual  string Description { get; set;  }

        public virtual bool ExaminersAvailable { get; set; }
        public virtual bool NcmsAvailable { get; set; }
    }
}
