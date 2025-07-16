using System;

namespace F1Solutions.Naati.Common.Dal.Domain.Portal
{
   public class Application : EntityBase
    {
       public virtual int? NAATINumber { get; set; }
       public virtual string Data { get; set; }
       public virtual DateTime Date { get; set; }
       public virtual bool IsApplicationByTesting { get; set; }
    }
}
