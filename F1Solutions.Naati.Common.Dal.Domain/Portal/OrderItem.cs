using System;

namespace F1Solutions.Naati.Common.Dal.Domain.Portal
{
    public class OrderItem : EntityBase
    {
        public OrderItem()
        {

        }
        
        public virtual string Product { get; set; }
        public virtual string Skill { get; set; }
        public virtual string Level { get; set; }
        public virtual string Direction { get; set; }
        public virtual DateTime? Expiry { get; set; }

        public virtual int Quantity { get; set; }

        /// <summary>
        /// Includes GST if applicable.
        /// </summary>
        public virtual decimal Price { get; set; }
        public virtual bool GSTApplies { get; set; }
        public virtual OrderItemType OrderItemType { get; set; }
        public virtual int ProductSpecificationId { get; set; }
        public virtual int? AccreditationResultId { get; set; }

        public virtual decimal Total
        {
            get { return Quantity * Price; }
        }

        public virtual Order Order { get; set; }
    }

    public enum OrderItemType
    {
        RubberStamp,
        SelfInkingStamp,
        UnlaminatedCertificate,
        LaminatedCertificate,
        SingleIdCard,
        AccreditationIdCard,
        RecognitionIdCard,
        PDListingRegistration,
        PDListingRenewal,
    }
}
