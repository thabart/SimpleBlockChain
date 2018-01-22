using Microsoft.AspNetCore.Mvc;

namespace EheathBlockChain.OpenId.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("Bienvenue sur SimpleIdentityServer");
        }
    }
}
