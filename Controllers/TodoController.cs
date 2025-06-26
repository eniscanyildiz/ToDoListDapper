using Microsoft.AspNetCore.Mvc;

namespace TodoListProject.Controllers
{
    public class TodoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
} 