using Microsoft.AspNetCore.Http;

namespace TONBRAINS.TONOPS.WebApp.Models.ModuleVersions
{
    public class UploadVersionModel
    {

        public string Id
        {
            get;
            set;
        }

        public IFormFile VersionFile
        {
            get;
            set;
        }

    }

}
