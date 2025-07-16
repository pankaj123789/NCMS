using F1Solutions.Naati.Common.Contracts.Bl;

namespace MyNaati.Contracts.Portal
{
    
    public interface IOrderService : IInterceptableservice
    {
        /// <summary>
        /// Get all online product orders that are paid for (online) and were submitted between the two dates. Orders on the start or end date will be included.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        //
        //IEnumerable<Order> GetPaidOnlineProductOrdersBetween(DateTime startDate, DateTime endDate);

        //
        //UnpaidOrderSaveResponse SaveUnpaidOrder(UnpaidOrderSaveRequest request);

        //
        //OrderSubmissionResponse SubmitOrder(OrderSubmissionRequest request);

        
        PaymentResponse SubmitCreatePayment(PaymentRequest request);

        //
        //OrderDetailsResponse GetOrderDetails(OrderDetailsRequest request);

        //
        //int GetNextOrderId();

        //
        //IEnumerable<PortalStatistic> GetPortalStats(DateTime startDate, DateTime endDate);

        //
        //void CompleteFreePDListingOrder(PractitionerDirectoryUpdateListingRequest pdUpdate);
    }

    
    
}
