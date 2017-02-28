using System.Web.Mvc;

namespace Byui.Desire2Learn.FinancialAid.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult NoAuth()
        {
            return View();
        }

        public ActionResult NoCourses()
        {
            return View();
        }
        public ActionResult NoCourse()
        {
            return View();
        }
    }
}
