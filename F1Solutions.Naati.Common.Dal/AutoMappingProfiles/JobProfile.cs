using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class JobProfile : Profile
    {
        public JobProfile()
        {
            CreateMap<Job, Job>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.JobExaminers, opt => opt.Ignore());

            CreateMap<JobExaminer, JobExaminer>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }
    }
}
