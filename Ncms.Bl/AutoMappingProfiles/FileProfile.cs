using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.File;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class FileProfile : Profile
    {
        public FileProfile()
        {
            CreateMap<CreateOrUpdateFileRequestModel, CreateOrUpdateFileRequest>()
                .ForMember(x => x.TokenToRemoveFromFilename, y => y.Ignore())
                .ForMember(x => x.UpdateStoredFileId, y => y.Ignore())
                .ForMember(x => x.UpdateFileName, y => y.Ignore())
                .ForMember(x => x.UploadedByPersonId, y => y.Ignore())
                .ForMember(x => x.UploadedDateTime, y => y.Ignore());

            CreateMap<CreateOrUpdateFileResponse, CreateOrUpdateFileResponseModel>();

            CreateMap<CreateOrUpdateFileRequestModel, FileMetadataRequest>()
                .ForMember(x => x.UpdateStoredFileId, y => y.Ignore())
                .ForMember(x => x.UpdateFileName, y => y.Ignore())
                .ForMember(x => x.UploadedByPersonId, y => y.Ignore())
                .ForMember(x => x.UploadedDateTime, y => y.Ignore());

            CreateMap<FileDto, FileResponseModel>()
                .ForMember(x => x.FileData, y => y.Ignore())
                .ForMember(x => x.FileType, y => y.Ignore());
        }
    }
}
