using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace BabyBrother.Services.Implementations
{
    public class WindowsResourceService : IResourceService
    {
        private readonly ResourceLoader _stringResourceLoader;

        public WindowsResourceService(ResourceLoader stringResourceLoader)
        {
            _stringResourceLoader = stringResourceLoader;
        }

        public string GetString(string id)
        {
            return _stringResourceLoader.GetString(id);
        }
    }
}
