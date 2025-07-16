using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain.Portal
{
   public class Order : EntityBase
    {
       public virtual int NAATINumber { get; set; }
       public virtual DateTime OrderDate { get; set; }
       public virtual string DeliveryName { get; set; }
       public virtual string DeliveryAddress { get; set; }
       public virtual int? SuburbId { get; set; }
       public virtual int CountryId { get; set; }
       public virtual string ExternalPaymentTransactionID { get; set; }
       public virtual IList<OrderItem> OrderItems { get; set; }

       public virtual decimal Total
       {
           get { return OrderItems.Sum(x => x.Total); }
       }
    }
}
