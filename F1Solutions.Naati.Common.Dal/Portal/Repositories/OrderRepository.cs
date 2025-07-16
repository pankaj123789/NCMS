using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain.Portal;
using NHibernate.Linq;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    //public interface IOrderRepository : IRepository<Order>
    //{
    //    void UpdateWithFlush(Order order);
    //    IEnumerable<Order> GetPaidOnlineProductOrdersBetween(DateTime startDate, DateTime endDate);
    //    int AllocateOrderId();

    //    IEnumerable<PortalStatistic> GetPortalStats(DateTime startDate, DateTime endDate);
    //    Order FindForSam(int referenceNumber);
    //}

    //public class OrderRepository : Repository<Order>, IOrderRepository
    //{
    //    private readonly IOrderItemRepository mOrderItemRepository;

    //    public OrderRepository(ICustomSessionManager sessionManager, IOrderItemRepository orderItemRepository)
    //        : base(sessionManager)
    //    {
    //        mOrderItemRepository = orderItemRepository;
    //    }

    //    public void UpdateWithFlush(Order order)
    //    {
    //        //Orders should always be created by AllocateOrderId()
    //        var existingOrder = Get(order.Id);
    //        foreach (var item in existingOrder.OrderItems)
    //        {
    //            item.Order = null;
    //            //The passed items are the full list, but don't have IDs provided from the UI. Without this step, we'd duplicate them in the DB.
    //            Session.Delete(item);
    //        }

    //        existingOrder.OrderItems.Clear();

    //        existingOrder.CountryId = order.CountryId;
    //        existingOrder.DeliveryAddress = order.DeliveryAddress;
    //        existingOrder.DeliveryName = order.DeliveryName;
    //        existingOrder.ExternalPaymentTransactionID = order.ExternalPaymentTransactionID;
    //        existingOrder.NAATINumber = order.NAATINumber;
    //        existingOrder.OrderDate = order.OrderDate;
    //        existingOrder.SuburbId = order.SuburbId;

    //        base.SaveOrUpdate(existingOrder);

    //        foreach (var item in order.OrderItems)
    //        {
    //            item.Order = existingOrder;
    //            // ugly cludge due to nhibernate issue with saving uni-directional one-to-many associations
    //            mOrderItemRepository.SaveOrUpdate(item);
    //        }

    //        //Without this, the update doesn't happen very reliably for some reason.
    //        Session.Flush();
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="entity"></param>
    //    public override void SaveOrUpdate(Order entity)
    //    {
    //    }

    //    public IEnumerable<Order> GetPaidOnlineProductOrdersBetween(DateTime startDate, DateTime endDate)
    //    {
    //        var productTypes = new[]
    //        {
    //            OrderItemType.SingleIdCard,
    //            OrderItemType.AccreditationIdCard,
    //            OrderItemType.RecognitionIdCard,
    //            OrderItemType.LaminatedCertificate,
    //            OrderItemType.UnlaminatedCertificate,
    //            OrderItemType.RubberStamp,
    //            OrderItemType.SelfInkingStamp,
    //            OrderItemType.PDListingRegistration,
    //            OrderItemType.PDListingRenewal
    //        };

    //        return (from order in Session.Query<Order>()
    //                where order.OrderDate.Date >= startDate.Date
    //                      && order.OrderDate.Date <= endDate.Date
    //                      && order.NAATINumber != 0
    //                      && order.ExternalPaymentTransactionID != null
    //                      && order.ExternalPaymentTransactionID != ""
    //                      && order.OrderItems.All(i => productTypes.Contains(i.OrderItemType))
    //                select order).ToList();
    //    }

    //    public int AllocateOrderId()
    //    {
    //        var newOrder = new Order
    //        {
    //            OrderDate = DateTime.Now,
    //            NAATINumber = 0,
    //            SuburbId = 0,
    //            CountryId = 0
    //        };

    //        return (int)Session.Save(newOrder);
    //    }


    //    public IEnumerable<PortalStatistic> GetPortalStats(DateTime startDate, DateTime endDate)
    //    {
    //        var statsList = new List<PortalStatistic>();
    //        statsList.AddRange(GetOrderStats(startDate, endDate));
    //        statsList.AddRange(GetDownLoadStats(startDate, endDate));
    //        statsList.AddRange(GetApplicationStats(startDate, endDate));

    //        var statsGroup = (from s in statsList
    //                          group s by s.ProductService
    //                          into g
    //                          select new PortalStatistic(g.Key, g.Sum(s => s.PaymentCount), g.Sum(s => s.DownloadCount)))
    //            .ToList();

    //        //  Add any product types that are no represented in the data selected.
    //        var missingProducts = ProductType.List.Where(productType => !statsGroup.Any(g => g.ProductService == productType));

    //        statsGroup.AddRange(missingProducts.Select(productType => new PortalStatistic(productType, 0, 0)));
    //        statsGroup = statsGroup.OrderBy(s => s.ProductService).ToList();

    //        statsGroup.Add(new PortalStatistic("Total", statsGroup.Sum(s => s.PaymentCount), statsGroup.Sum(s => s.DownloadCount)));

    //        return statsGroup;
    //    }

    //    public Order FindForSam(int referenceNumber)
    //    {
    //        return Session.Query<Order>().Where(x => x.Id == referenceNumber).Fetch(x => x.OrderItems).SingleOrDefault();
    //    }

    //    private IEnumerable<PortalStatistic> GetOrderStats(DateTime startDate, DateTime endDate)
    //    {
    //        var orderQuery = from o in Session.Query<Order>()
    //                         where o.OrderDate.Date >= startDate.Date && o.OrderDate.Date <= endDate.Date
    //                         select o;

    //        return (from order in orderQuery.ToList() from item in order.OrderItems select new PortalStatistic(item.Product, 1, 0)).ToList();

    //    }

    //    private IEnumerable<PortalStatistic> GetDownLoadStats(DateTime startDate, DateTime endDate)
    //    {
    //        var downloadQuery = from d in Session.Query<Download>()
    //                            where d.DownloadDate.Date >= startDate.Date && d.DownloadDate.Date <= endDate.Date
    //                            select d;

    //        return (from download in downloadQuery.ToList() select new PortalStatistic(download.Product, 0, 1)).ToList();
    //    }

    //    private IEnumerable<PortalStatistic> GetApplicationStats(DateTime startDate, DateTime endDate)
    //    {
    //        var testingApplicationQuery = from d in Session.Query<Application>()
    //                                      where d.Date >= startDate.Date.Date && d.Date.Date <= endDate.Date
    //                                      && d.IsApplicationByTesting
    //                                      select d;
    //        var testingApplicationCount = testingApplicationQuery.Count();

    //        yield return new PortalStatistic("Application by Australian course", 0, testingApplicationCount);

    //        var courseApplicationQuery = from d in Session.Query<Application>()
    //                                     where d.Date >= startDate.Date.Date && d.Date.Date <= endDate.Date
    //                                     && d.IsApplicationByTesting == false
    //                                     select d;
    //        var courseApplicationCount = courseApplicationQuery.Count();

    //        yield return new PortalStatistic("Application by Testing", 0, courseApplicationCount);
    //    }
    //}
}
