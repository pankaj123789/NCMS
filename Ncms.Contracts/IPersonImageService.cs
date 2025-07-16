using Ncms.Contracts.Models.Common;
using Ncms.Contracts.Models.Person;

namespace Ncms.Contracts
{
    public interface IPersonImageService
    {
        FileModel GetPersonImage(int naatiNumber, GetImageRequestModel model);
        void UpdatePhoto(UpdatePhotoRequestModel model);
    }
}
