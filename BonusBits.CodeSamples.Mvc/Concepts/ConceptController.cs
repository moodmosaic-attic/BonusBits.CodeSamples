using System.ComponentModel.Composition;
using System.Web.Mvc;

namespace Concepts
{
    [Export(typeof(IController)), ExportMetadata("controllerName", "Concept")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConceptController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Name = this.GetType().Assembly.FullName;

            return View("~/Extensions/Views/Concept/Index.cshtml");
        }
    }
}
