using System;
using System.Linq;
using F1Solutions.Naati.Sam.ServiceContracts.Services;
using NAATI.Domain;
using NAATI.WebService.NHibernate.DataAccess;
using NHibernate.Linq;
using SharpArch.Data.NHibernate;
using Invoice = Xero.Api.Core.Model.Invoice;

namespace F1Solutions.NAATI.SAM.WebService.ExposedServices.Finance
{
    public class CreateOfficeInvoiceCompletionOperation : CreateInvoiceCompletionOperation
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int NaatiNumber { get; set; }
        public int CorrespondenceCategoryId { get; set; }

        protected override void PerformOperation(Invoice invoice)
        {
            var session = NHibernateSession.Current;
            var order = session.Get<Order>(OrderId);
            var entity = session.Query<NaatiEntity>().First(x => x.NaatiNumber == NaatiNumber);

            var correspondence = new CreateCorrespondenceDto
            {
                EntityId = entity.Id,
                CreatedBy = UserId,
                Note = $"{order.ReferenceNumber} / {invoice.Number} has been created",
                Path = string.Empty,
                SendDate = DateTime.Now,
                CategoryId = CorrespondenceCategoryId
            };

            var createCorrespondenceRequest = new CreateCorrespondenceRequest
            {
                Correspondence = correspondence
            };

            var correspondenceService = new CorrespondenceService();
            correspondenceService.CreateCorrespondence(createCorrespondenceRequest);

            order.InvoiceNumber = invoice.Number;

            session.Save(order);
            session.Flush();
        }
    }
}
