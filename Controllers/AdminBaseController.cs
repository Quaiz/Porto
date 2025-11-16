using System.Web.Mvc;

namespace Porto.Controllers
{
    /// <summary>
    /// Base controller cho tất cả Admin controllers
    /// Tự động kiểm tra quyền Admin trước khi thực thi action
    /// </summary>
    public abstract class AdminBaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra đăng nhập và quyền Admin
            if (Session["Username"] == null || Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                TempData["Error"] = "Bạn không có quyền truy cập trang quản trị";
                filterContext.Result = RedirectToAction("Login", "Account");
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        // Allow child controllers to override Dispose
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}

