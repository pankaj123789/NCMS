using System;
using System.Collections.Generic;
using System.Linq;
using Ncms.Contracts;
using Ncms.Contracts.Models.Accounting;

namespace Ncms.Bl.Export
{
	public class EndOfPeriodSummaryModel
	{
		public string SummaryItem { get; set; }
		public int Count { get; set; }
		public decimal Amount { get; set; }
	}

	public class EndOfPeriodExporter : SearchResultsExporter
	{
		public EndOfPeriodExporter(EndOfPeriodRequest searchRequest,
			IEnumerable<PaymentModel> payments,
			IEnumerable<EndOfPeriodSummaryModel> summary,
			IAccountingService accountingService)
		{
			if (searchRequest == null)
			{
				throw new ArgumentNullException(nameof(searchRequest));
			}

			if (payments == null)
			{
				throw new ArgumentNullException(nameof(payments));
			}

			_searchRequest = searchRequest;
			_payments = payments;
			_summary = summary;
			_accountingService = accountingService;
		}

		private readonly IEnumerable<PaymentModel> _payments;
		private readonly IEnumerable<EndOfPeriodSummaryModel> _summary;
		private readonly EndOfPeriodRequest _searchRequest;
		private object[][][] _data;
		private string[] _criteria;
		private readonly IAccountingService _accountingService;
		private const string AbsentCriterionValue = "All";

		private string FormatOffices(int[] officeIds)
		{
			return officeIds == null || !officeIds.Any()
				? AbsentCriterionValue
				: string.Join(", ", _accountingService.GetOffices().Data.Where(x => officeIds.Contains(x.Id)).Select(x => x.Abbreviation));
		}

		private string FormatEftMachines(int[] machineIds)
		{
			return machineIds == null || !machineIds.Any()
				? AbsentCriterionValue
				: string.Join(", ", _accountingService.GetEftMachines().Data.Where(x => machineIds.Contains(x.Id)).Select(x => x.TerminalNumber));
		}

		private static string FormatIncludeLegacyInvoices()
		{
			return "Yes";
		}

		private static string FormatStringList(string[] values)
		{
			var list = AbsentCriterionValue;
			if (values != null && values.Any())
			{
				list = string.Join(", ", values);
			}

			return list;
		}

		private static string FormatDateRange(DateTime? from, DateTime? to)
		{
			if (!from.HasValue)
			{
				return AbsentCriterionValue;
			}

			var format = "From {0:dd/MM/yyyy}";
			if (to.HasValue)
			{
				format += " to {1:dd/MM/yyyy}";
			}

			return string.Format(format, from, to);
		}

		protected override string TemplateFileName => "EndOfPeriodExport.xltx";

		protected override string[] Criteria => _criteria ?? (_criteria = new List<string>
		{
			$"Date Range: {FormatDateRange(_searchRequest.DateCreatedFrom, _searchRequest.DateCreatedTo)}",
			$"Office(s): {FormatOffices(_searchRequest.Office)}",
			$"EFT Machine(s): {FormatEftMachines(_searchRequest.EftMachine)}",
			$"Invoice Number(s): {FormatStringList(_searchRequest.InvoiceNumber)}",
			$"Payment Type(s): {FormatStringList(_searchRequest.PaymentType)}",
			$"Include Pre-Xero Invoices: {FormatIncludeLegacyInvoices()}"
		}.ToArray());

		protected override object[][][] Data => _data ?? (_data = new[]
		{
			_payments.Select(x => new object[]
			{
				x.Office,
				x.InvoiceNumber,
				x.NaatiNumber,
				x.Customer,
				x.PaymentType,
				x.Amount,
				x.DatePaid,
				x.PaymentAccount,
				x.Reference,
				x.BSB,
				x.ChequeNumber,
				x.BankName,
				x.EftMachine
			}).ToArray(),
			_summary.Select(x => new object[]
			{
				x.SummaryItem,
				x.Count,
				x.Amount
			}).ToArray()
		});
	}
}
