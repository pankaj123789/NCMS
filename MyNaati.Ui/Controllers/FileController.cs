using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using F1Solutions.Naati.Common.Bl.Extensions;
using MyNaati.Bl.BackOffice.Helpers;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Security;
using MyNaati.Ui.ViewModels.File;

namespace MyNaati.Ui.Controllers
{
    //public class FileController : Controller
    //{
    //    private IFileService mFileService;

    //    public FileController(IFileService fileSerice)
    //    {
    //        mFileService = fileSerice;
    //    }

    //    [HttpGet]
    //    [AuthorizeRoles(SystemRoles.ADMINISTRATORS)]
    //    public ActionResult UploadFile()
    //    {
    //        GetFileListingResponse response = mFileService.GetFileListing();
    //        FileListingModel model = new FileListingModel()
    //                                     {
    //                                         FileNames = response.FileNames
    //                                     };

    //        return View(model);
    //    }

    //    [HttpPost]
    //    [AuthorizeRoles(SystemRoles.ADMINISTRATORS)]
    //    public ActionResult UploadFile([Bind(Prefix = "FileUpload")]FileUploadModel uploadedFile)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            using (var reader = new BinaryReader(uploadedFile.File.InputStream))
    //            {
    //                var file = uploadedFile.FileName;
    //                var fileExtension = file.Split('.').Last();

    //                if (!ApplicationSettingsHelper.IncludedFileExtensionsList.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
    //                {
    //                    throw new Exception("Unsupported File Type");
    //                }

    //                var request = new CreateOrUpdateFileRequest()
    //                                  {
    //                                      FileName = uploadedFile.FileName,
    //                                      FileBytes = reader.ReadBytes(uploadedFile.File.ContentLength)
    //                                  };
    //                mFileService.CreateOrUpdateFile(request);
    //            }
    //        }

    //        GetFileListingResponse response = mFileService.GetFileListing();
    //        FileListingModel model = new FileListingModel()
    //        {
    //            FileNames = response.FileNames
    //        };

    //        return View(model);
    //    }

    //    [HttpGet]
    //    [AuthorizeRoles(SystemRoles.ADMINISTRATORS)]
    //    public ActionResult DownloadFile(string fileName)
    //    {
    //        GetFileRequest request = new GetFileRequest()
    //                                     {
    //                                         FileName = fileName
    //                                     };
    //        GetFileResponse response = mFileService.GetFile(request);
    //        return File(response.FileBytes, "application/octet-stream", fileName);
    //    }
    //}
}
