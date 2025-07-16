namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class ResultType : EntityBase, ILookupType
    {
        public virtual string Result { get; set; }

        public override string ToString()
        {
            return Result;
        }
    }
}
