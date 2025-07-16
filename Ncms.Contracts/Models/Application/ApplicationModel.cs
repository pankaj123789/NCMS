using System;

namespace Ncms.Contracts.Models.Application
{
    public class ApplicationModel
    {
        public string Reference { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public int NaatiNumber { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string Owner { get; set; }
    }
}
