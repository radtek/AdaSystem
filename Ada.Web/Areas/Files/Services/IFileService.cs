using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Core;
using Files.Models;

namespace Files.Services
{
    public interface IFileService : IDependency
    {
        UploadResult FileSaveAs(UploadView view);

        UploadResult CropSaveAs(string fileUri, int maxWidth, int maxHeight, int cropWidth, int cropHeight, int X,
            int Y);

        UploadResult RemoteSaveAs(string sourceUri);
        void DeleteFile(string fileUri);
        byte[] ZipFiles(List<string> files, string dicName = null);
        byte[] ZipFiles(List<SelectListItem> files);
    }
}