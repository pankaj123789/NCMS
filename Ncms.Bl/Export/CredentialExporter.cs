using System;
using System.Collections.Generic;
using System.Linq;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.Export
{
    public class CredentialExporter : SearchResultsExporter
    {
        public CredentialExporter(IEnumerable<CredentialExportModel> credentials)
        {
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }
            _credentials = credentials;
        }

        private readonly IEnumerable<CredentialExportModel> _credentials;
        private object[][][] _data;

        protected override string TemplateFileName => "CredentialExport.xltx";

        protected override string[] Criteria => new string[0];

        protected override object[][][] Data => _data ?? (_data = new[]
        {
            _credentials.Select(x => new object[]
            {
                x.NaatiNumber,
                x.PractitionerNumber,
                x.Title,
                x.GivenName,
                x.FamilyName,
                x.Address,
                x.Suburb,
                x.State,
                x.Postcode,
                x.Country,
                x.PrimaryEmail,
                x.CredentialId,
                x.ApplicationType,
                x.InternalName,
                x.ExternalName,
                x.Language1,
                x.Language1Code,
                x.Language1Group,
                x.Language2,
                x.Language2Code,
                x.Language2Group,
                x.DirectionDisplayName,
                x.StartDate.ToString("dd/MM/yyyy"),
                x.EndDate.ToString("dd/MM/yyyy"),
                x.Status,
                x.StatusChangeDate.ToString("dd/MM/yyyy"),
                x.ExportedDate.ToString("dd/MM/yyyy H:mm")
            }).ToArray()
        });
    }
}
