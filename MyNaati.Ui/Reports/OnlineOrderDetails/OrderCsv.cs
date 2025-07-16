using System;
using MyNaati.Ui.LINQtoCSV;

namespace MyNaati.Ui.Reports.OnlineOrderDetails
{
    public class OrderCsv
    {
        [CsvColumn(FieldIndex = 1, Name= "Customer number")]
        public int NAATINumber { get; set; }
        [CsvColumn(FieldIndex = 2, Name= "Order Reference Number")]
        public int OrderId { get; set; }
        [CsvColumn(FieldIndex = 3, Name="Order Date", OutputFormat="dd MMM yyyy hh:mm tt")]
        public DateTime OrderDate { get; set; }
        [CsvColumn(FieldIndex = 5, Name = "Delivery Name")]
        public string DeliveryName { get; set; }
        [CsvColumn(FieldIndex = 6, Name = "Delivery Address")]
        public string DeliveryAddress { get; set; }
        [CsvColumn(FieldIndex = 7, Name = "Suburb")]
        public string Suburb { get; set; }
        [CsvColumn(FieldIndex = 8, Name = "Country")]
        public string Country { get; set; }
        [CsvColumn(FieldIndex = 4, Name = "Order Total ($AUD)")]
        public decimal Total { get; set; }

        [CsvColumn(FieldIndex = 9, Name = "Product")]
        public string Product { get; set; }
        [CsvColumn(FieldIndex = 10, Name = "Skill")]
        public string Skill { get; set; }
        [CsvColumn(FieldIndex = 11, Name = "Level")]
        public string Level { get; set; }
        [CsvColumn(FieldIndex = 12, Name = "Direction")]
        public string Direction { get; set; }
        [CsvColumn(FieldIndex = 13, Name = "Quantity")]
        public int Quantity { get; set; }
    }
}