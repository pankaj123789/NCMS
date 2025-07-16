using System;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate.Util;

namespace F1Solutions.Naati.Common.Dal
{
    public class PayrollQueryService : IPayrollQueryService
    {
        public GetProductSpecificationLookupListResponse GetProductSpecificationsForProductType(int productTypeId)
        {
            var query =
                from spec in NHibernateSession.Current.Query<ProductSpecification>()
                where spec.ProductCategory.ProductType.Id == productTypeId
                select new ProductSpecificationDetailsDto
                {
                    Code = spec.Code,
                    CostPerUnit = spec.CostPerUnit,
                    GlCode = spec.GLCode.Code,
                    Id = spec.Id,
                    Name = spec.Name,
                    Inactive = spec.Inactive
                };

            return new GetProductSpecificationLookupListResponse
            {
                ProductSpecificationDetails = query.ToArray()
            };
        }

        public GetProductSpecificationLookupListResponse GetProductSpecificationByCode(string code)
        {
            var list = NHibernateSession.Current
            .Query<ProductSpecification>()
            .Where(ps => ps.Code == code)
            .Select(ps => new ProductSpecificationDetailsDto
            {
                Id = ps.Id,
                Code = ps.Code,
                Name = ps.Name,
                Description = ps.Description,
                CostPerUnit = ps.CostPerUnit,
                GstApplies = ps.GSTApplies,
                GlCode = ps.GLCode.Code,
                Inactive = ps.Inactive,
                ProductCategoryId = ps.ProductCategory.Id
            })
            .ToList();

            return new GetProductSpecificationLookupListResponse
            {
                ProductSpecificationDetails = list
            };
        }

        public GetMarkingsForPayrollResponse GetMarkingsForPayroll(GetMarkingsForPayrollRequest request)
        {
            var payrollStatusIds = string.Join(",", request.PayrollStatuses.Select(x => (int)x));
            var naatiNumber = !string.IsNullOrEmpty(request.ExaminerNaatiNumber) ? $" and ExaminerNaatiNumber = {request.ExaminerNaatiNumber}" : string.Empty;

            var fromDate = request.From.HasValue ? $" and PayrollModifiedDate >= '{request.From.Value:yyyy-MM-dd}'" : string.Empty;
            var toDate = request.To.HasValue ? $" and PayrollModifiedDate <= '{request.To.Value:yyyy-MM-dd}'" : string.Empty;

            var sql = $"select * from vwMarkingsForPayroll where PayrollStatusId in ({payrollStatusIds}){naatiNumber}{fromDate}{toDate}";

            var results = NHibernateSession.Current.TransformSqlQueryDataRowResult<MarkingForPayrollDto>(sql);
            return new GetMarkingsForPayrollResponse
            {
                Markings = results
            };
        }

        public void ValidateMarkingForPayroll(ValidateMarkingForPayrollRequest request)
        {
            foreach (var jobExaminerId in request.JobExaminerIds)
            {
                var marking = NHibernateSession.Current.Get<JobExaminer>(jobExaminerId);
                var user = NHibernateSession.Current.Get<User>(request.UserId);

                marking.ValidatedDate = DateTime.Now;
                marking.ValidatedUser = user;

                NHibernateSession.Current.Save(marking);
            }
            NHibernateSession.Current.Flush();
        }

        public void UnvalidateMarkingForPayroll(int jobExaminerId)
        {
            var marking = NHibernateSession.Current.Get<JobExaminer>(jobExaminerId);
            if (marking == null)
            {
                return;
            }

            marking.ValidatedDate = null;
            marking.ValidatedUser = null;

            NHibernateSession.Current.Save(marking);
            NHibernateSession.Current.Flush();
        }

        public CreatePayrollResponse CreatePayroll(CreatePayrollRequest request)
        {
            var response = new CreatePayrollResponse();

            var allMarkingsInAPayroll = NHibernateSession.Current.Query<JobExaminerPayroll>()
                .Select(x => x.JobExaminer.Id)
                .ToList();

            if (request.JobExaminerIds.Any(x => allMarkingsInAPayroll.Contains(x)))
            {
                response.ErrorMessage = "Pay run cannot be created. Some or all of these markings are already part of a pay run. Please refresh the screen.";
                response.Error = true;
            }
            else
            {
                var payroll = new Payroll
                {
                    ModifiedDate = DateTime.Now,
                    ModifiedUser = NHibernateSession.Current.Get<User>(request.UserId),
                    LegacyAccounting = request.LegacyAccounting,
                    PayrollStatus = NHibernateSession.Current.Load<PayrollStatus>((int)PayrollStatusName.InProgress)
                };

                foreach(var jobExaminerId in request.JobExaminerIds)
                {
                    payroll.AddJobExaminer(NHibernateSession.Current.Get<JobExaminer>(jobExaminerId));
                }

                NHibernateSession.Current.Save(payroll);
                NHibernateSession.Current.Flush();
            }

            return response;
        }

        public bool IsPayrollInProgress()
        {
            return NHibernateSession.Current.Query<Payroll>().Any(x => x.PayrollStatus.Name == PayrollStatusName.InProgress.ToString());
        }

        public AssignPayrollAccountingReferenceResponse AssignPayrollAccountingReference(AssignPayrollAccountingReferenceRequest request)
        {
            var response = new AssignPayrollAccountingReferenceResponse();

            var payroll = NHibernateSession.Current.Get<Payroll>(request.PayrollId);
            if (payroll.PayrollStatus.Name == PayrollStatusName.Complete.ToString())
            {
                response.ErrorMessage = "This payroll is complete and cannot be changed.";
                response.Error = true;

                return response;
            }

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                try
                {
                    foreach (var jobExaminerId in request.JobExaminerIds)
                    {
                        var jep = payroll.JobExaminerPayrolls.SingleOrDefault(x => x.JobExaminer.Id == jobExaminerId);
                        if (jep == null)
                        {
                            response.ErrorMessage = "Cannot find this marking in the payroll. It may have been removed.";
                            response.Error = true;

                            return response;
                        }

                        jep.AccountingReference = (request.AccountingReference ?? string.Empty).Trim();
                        NHibernateSession.Current.Save(jep);
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return response;
        }

        public void SetPayrollStatus(SetPayrollStatusRequest request)
        {
            var payroll = NHibernateSession.Current.Get<Payroll>(request.PayrollId);

            if (request.Status == PayrollStatusName.Complete && payroll.JobExaminerPayrolls.Any(x => string.IsNullOrEmpty(x.AccountingReference)))
            {
                throw new WebServiceException("Payroll cannot be completed until all examiners have accounting reference assigned.");
            }

            payroll.PayrollStatus = NHibernateSession.Current.Get<PayrollStatus>((int)request.Status);
            payroll.ModifiedDate = DateTime.Now;
            payroll.ModifiedUser = NHibernateSession.Current.Get<User>(request.UserId);

            NHibernateSession.Current.Save(payroll);
            NHibernateSession.Current.Flush();
        }

        public RemoveExaminerFromPayrollResponse RemoveExaminerFromPayroll(RemoveExaminerFromPayrollRequest request)
        {
            var response = new RemoveExaminerFromPayrollResponse();

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                try
                {
                    var payroll = NHibernateSession.Current.Get<Payroll>(request.PayrollId);
                    if (payroll.PayrollStatus.Name == PayrollStatusName.Complete.ToString())
                    {
                        response.ErrorMessage = "This payroll is complete and cannot be changed.";
                        response.Error = true;
                    }
                    else
                    {
                        payroll.RemoveJobExaminer(request.JobExaminerIds);
                        NHibernateSession.Current.Save(payroll);

                        // if there are no examiners left, delete the payrun so a new one can be created
                        if (!payroll.JobExaminerPayrolls.Any())
                        {
                            NHibernateSession.Current.Delete(payroll);
                        }

                        transaction.Commit();
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return response;
        }
    }
}
