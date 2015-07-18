using r3mus.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace r3mus.Controllers
{
    public class IntelController : Controller
    {
        public enum Map
        {
            Delve,
            Querious
        }

        //
        // GET: /Intel/
        public ActionResult GetIntelMap_PNG(Map map)
        {
            var tmpMap = Image.FromFile(HostingEnvironment.MapPath(string.Format("~/Images/{0}_Tac_Map.png", map.ToString())));

            var vm = new IntelViewModel() { Title = map.ToString() };

            using (var stream = new MemoryStream())
            {
                tmpMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                vm.Map = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(stream.ToArray()));
            }

            return View(vm);
        }
	}
}