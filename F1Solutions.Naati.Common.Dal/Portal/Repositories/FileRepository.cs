using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain.Portal;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    //public interface IFileRepository : IRepository<File>
    //{
    //    File GetFileByName(string fileName);

    //    List<string> GetFileListing();

    //    void CreateOrUpdateFile(string fileName, byte[] fileBytes);
    //}

    //public class FileRepository : Repository<File>, IFileRepository
    //{
    //    public FileRepository(ICustomSessionManager sessionManager)
    //        : base(sessionManager)
    //    {
    //    }

    //    public File GetFileByName(string fileName)
    //    {
    //        return Session.Query<File>().SingleOrDefault(e => e.FileName == fileName);
    //    }

    //    public List<string> GetFileListing()
    //    {
    //        return Session.Query<File>().Select(e => e.FileName).ToList();
    //    }

    //    public void CreateOrUpdateFile(string fileName, byte[] fileBytes)
    //    {
    //        File file = GetFileByName(fileName);

    //        if (file != null)
    //        {
    //            Session.Delete(file);
    //            DeleteWithFlush(file);
    //        }

    //        file = new File()
    //        {
    //            FileName = fileName,
    //            FileBytes = fileBytes,
    //        };
            
    //        Session.SaveOrUpdate(file);
    //    }
    //}
}
