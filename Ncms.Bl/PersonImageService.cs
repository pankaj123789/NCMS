using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts;
using Ncms.Contracts.Models.Common;
using Ncms.Contracts.Models.Person;

namespace Ncms.Bl
{
    public class PersonImageService : IPersonImageService
    {
        private readonly IPersonQueryService _personQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public PersonImageService(IPersonQueryService personQueryService, IAutoMapperHelper autoMapperHelper)
        {
            _personQueryService = personQueryService;
            _autoMapperHelper = autoMapperHelper;
        }
        public FileModel GetPersonImage(int naatiNumber, GetImageRequestModel model)
        {
            var request = new GetPersonDetailsRequest
            {
                NaatiNumber = naatiNumber
            };

            GetPersonImageResponse response = null;

            try
            {
                response = _personQueryService.GetPersonImage(request);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }
            if (response.PersonImageData == null || response.PersonImageData.Length == 0)
            {
                return new FileModel
                {
                    FileData = new MemoryStream(),
                    FileName = $"{naatiNumber}.png",
                    FileType = FileType.Png
                };

            }

            Stream stream;
            if ((model.Height.HasValue || model.Width.HasValue) && response.PersonImageData?.Length > 0)
            {
                stream = ResizeImage(response.PersonImageData, model);
            }
            else
            {
                stream = new MemoryStream(response.PersonImageData);
            }

            return new FileModel
            {
                FileData = stream,
                FileName = $"{naatiNumber}.png",
                FileType = FileType.Png
            };
        }

        public void UpdatePhoto(UpdatePhotoRequestModel model)
        {
            var request = _autoMapperHelper.Mapper.Map<UpdatePhotoDto>(model);

            _personQueryService.UpdatePhoto(request);
        }

        private Stream ResizeImage(byte[] buffer, GetImageRequestModel model)
        {
            var path = ConfigurationManager.AppSettings["tempFilePath"];
            var fileName = Path.Combine(path, Guid.NewGuid().ToString());
            File.WriteAllBytes(fileName, buffer);
            Stream stream;

            try
            {
                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (var image = Image.FromStream(fileStream))
                    {
                        if (image.Size.Height < model.Height.GetValueOrDefault() || image.Size.Width < model.Width.GetValueOrDefault())
                        {
                            stream = new MemoryStream(buffer);
                            return stream;
                        }
                        else
                        {
                            stream = ResizeImage(model, image);
                            return stream;
                        }
                    }
                }
            }
            catch (OutOfMemoryException)
            {
                LoggingHelper.LogError($"Error resizing Person Image.");
                throw new UserFriendlySamException($"Could not resize Person Image.");
            }
            catch(Exception ex)
            {
                LoggingHelper.LogError($"Error resizing Person Image: {ex.Message}");
                throw new UserFriendlySamException($"Error resizing Person Image: {ex.Message}.");
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        private Stream ResizeImage(GetImageRequestModel model, Image image)
        {
            double heightPercent = (double)(model.Width ?? image.Size.Width) / image.Size.Width;
            double widthPercent = (double)(model.Height ?? image.Size.Height) / image.Size.Height;
            var finalPercent = heightPercent > widthPercent ? heightPercent : widthPercent;

            var newWidth = (int)(image.Size.Width * finalPercent);
            var newHeight = (int)(image.Size.Height * finalPercent);

            using (var thumb = image.GetThumbnailImage(newWidth, newHeight, null, new IntPtr()))
            {
                return GetImageStream(thumb);
            }
        }

        private Stream GetImageStream(Image thumb)
        {
            var ms = new MemoryStream();
            thumb.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            return ms;
        }
    }
}
