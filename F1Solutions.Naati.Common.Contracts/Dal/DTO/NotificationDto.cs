using Newtonsoft.Json;
using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int NotificationTypeId { get; set; }
        public virtual string Parameter { get; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
    }

    public class NotificationDto<T> : NotificationDto
    {
        public T Data { get; set; }

        public override string Parameter => JsonConvert.SerializeObject(Data);
    }
}
