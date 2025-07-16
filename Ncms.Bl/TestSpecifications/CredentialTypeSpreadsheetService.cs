using ClosedXML.Excel;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Contracts.Models.TestSpecification;
using System.Collections.Generic;
using System.Linq;


namespace Ncms.Bl.TestSpecifications
{
    public static class CredentialTypeSpreadsheetService
    {
        public static GenericResponse<IList<CredentialType>> ReadCredentialTypes(XLWorkbook workbook, GenericResponse<IList<CredentialType>> credentialTypesResult)
        {
            var worksheet = workbook.Worksheet($"{Constants.CredentialTypes} - Read Only");

            var i = 2;
            var keepGoing = true;
            while (keepGoing)
            {
                //this might change if first column doesnt have to have data
                var data = worksheet.Cell(i, 1).GetValue<string>();
                if (string.IsNullOrEmpty(data))
                {
                    keepGoing = false;
                    break;
                }
                ((List<CredentialType>)credentialTypesResult.Data).Add(new CredentialType()
                {
                    InternalName = worksheet.Cell(i, 1).GetValue<string>(),
                });
                i++;
            }
            return credentialTypesResult;
        }

        public static bool WriteCredentialTypes(XLWorkbook document, IList<CredentialType> credentialTypes)
        {
            var worksheet = document.Worksheets.Add($"{Constants.CredentialTypes} - Read Only");
            worksheet.Protect();

            var i = 2;
            foreach(var result in credentialTypes)
            {
                worksheet.Cell(i, 1).Value = result.InternalName;
                worksheet.Cell(i, 2).Value = result.ExternalName;
                worksheet.Cell(i, 3).Value = result.DisplayOrder;
                worksheet.Cell(i, 4).Value = result.Simultaneous;
                worksheet.Cell(i, 5).Value = result.SkillTypeId;
                worksheet.Cell(i, 6).Value = result.Certification;
                if (result.DefaultExpiry.HasValue)
                {
                    worksheet.Cell(i, 7).Value = result.DefaultExpiry;
                }
                worksheet.Cell(i, 8).Value = result.AllowBackdating;
                worksheet.Cell(i, 9).Value = result.TestSessionBookingAvailabilityWeeks;
                worksheet.Cell(i, 10).Value = result.TestSessionBookingClosedWeeks;
                worksheet.Cell(i, 11).Value = result.TestSessionBookingRejectHours;
                worksheet.Cell(i, 12).Value = result.AllowAvailabilityNotice;

                i++;
            }

            foreach(var result in credentialTypes)
            {
                worksheet.Cell(1, 1).Value = nameof(result.InternalName);
                worksheet.Cell(1, 1).Style.Font.SetBold();
                worksheet.Cell(1, 2).Value = nameof(result.ExternalName);
                worksheet.Cell(1, 2).Style.Font.SetBold();
                worksheet.Cell(1, 3).Value = nameof(result.DisplayOrder);
                worksheet.Cell(1, 3).Style.Font.SetBold();
                worksheet.Cell(1, 4).Value = nameof(result.Simultaneous);
                worksheet.Cell(1, 4).Style.Font.SetBold();
                worksheet.Cell(1, 5).Value = nameof(result.SkillTypeId);
                worksheet.Cell(1, 5).Style.Font.SetBold();
                worksheet.Cell(1, 6).Value = nameof(result.Certification);
                worksheet.Cell(1, 6).Style.Font.SetBold();
                worksheet.Cell(1, 7).Value = nameof(result.DefaultExpiry);
                worksheet.Cell(1, 7).Style.Font.SetBold();
                worksheet.Cell(1, 8).Value = nameof(result.AllowBackdating);
                worksheet.Cell(1, 8).Style.Font.SetBold();
                worksheet.Cell(1, 9).Value = nameof(result.TestSessionBookingAvailabilityWeeks);
                worksheet.Cell(1, 9).Style.Font.SetBold();
                worksheet.Cell(1, 10).Value = nameof(result.TestSessionBookingClosedWeeks);
                worksheet.Cell(1, 10).Style.Font.SetBold();
                worksheet.Cell(1, 11).Value = nameof(result.TestSessionBookingRejectHours);
                worksheet.Cell(1, 11).Style.Font.SetBold();
                worksheet.Cell(1, 12).Value = nameof(result.AllowAvailabilityNotice);
                worksheet.Cell(1, 12).Style.Font.SetBold();
            }

            worksheet.Columns().AdjustToContents();
            worksheet.Rows().AdjustToContents();

            return true;
        }
    }

}
