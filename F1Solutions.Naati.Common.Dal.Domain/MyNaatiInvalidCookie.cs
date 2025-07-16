using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class MyNaatiInvalidCookie : EntityBase, IInvalidCookie
    {
        public virtual string Cookie { get; set; }
        public virtual DateTime ExpiryDate { get; set; }
    }
}
