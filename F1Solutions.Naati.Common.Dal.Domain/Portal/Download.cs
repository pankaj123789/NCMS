using System;

namespace F1Solutions.Naati.Common.Dal.Domain.Portal
{
   public  class Download : EntityBase
    {
       public virtual int NAATINumber { get; set; }
       public virtual DateTime DownloadDate { get; set; }
       public virtual string Product { get; set; }
       public virtual string ReferenceNumber { get; set; }
    }
}
