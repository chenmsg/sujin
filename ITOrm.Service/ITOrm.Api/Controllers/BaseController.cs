using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ITOrm.Api.Controllers
{
    public class BaseController:Controller
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }
    }
}