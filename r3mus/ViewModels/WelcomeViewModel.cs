using r3mus.Models;
using System.Collections.Generic;

namespace r3mus.ViewModels
{
    public class WelcomeViewModel
    {
        public List<ApiInfo> Apis { get; set; }

        public LatestNew LatestInternalNewsItem { get; set; }
    }
}
