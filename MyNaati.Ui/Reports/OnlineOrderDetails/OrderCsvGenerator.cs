//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using AutoMapper;
//using F1Solutions.Naati.Common.Dal.Domain.Portal;
//using MyNaati.Ui.LINQtoCSV;

//namespace MyNaati.Ui.Reports.OnlineOrderDetails
//{
//    public class OrderCsvGenerator : IOrderCsvGenerator
//    {
//        public byte[] GenerateOrderCsv(IEnumerable<Order> orders)
//        {
//            var orderCsvs = new List<OrderCsv>();

//            foreach(var order in orders)
//            {
//                foreach (var orderItem in order.OrderItems)
//                {
//                    var orderCsv = Mapper.Map<Order, OrderCsv>(order);

//                    orderCsv.Product = orderItem.Product;
//                    orderCsv.Skill = orderItem.Skill;
//                    orderCsv.Level = orderItem.Level;
//                    orderCsv.Direction = orderItem.Direction;
//                    orderCsv.Quantity = orderItem.Quantity;
//                    orderCsvs.Add(orderCsv);
//                }
//            }

//            var outPutFileDescrption = new CsvFileDescription
//                                           {
//                                               QuoteAllFields = false,
//                                               SeparatorChar = ',',
//                                               FirstLineHasColumnNames = true
//                                           };

//            var cc = new CsvContext();

//            using (var tStream = new StringWriter())
//            {
//                cc.Write(orderCsvs, tStream, outPutFileDescrption);
//                return Encoding.UTF8.GetBytes(tStream.GetStringBuilder().ToString());
//            }

//        }
//    }
//}