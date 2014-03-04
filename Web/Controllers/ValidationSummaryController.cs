using System.Web.Mvc;

namespace Web.Controllers
{
    public class ValidationSummaryController : Controller
    {
        public ActionResult Index()
        {
            var model = new Book("Refactoring", "Fawler");
            ModelState.AddModelError("Title", "One");
            ModelState.AddModelError("Title", "Two");
            ModelState.AddModelError("Title", "Three");
            ModelState.AddModelError("Author", "Four");
            return View(model);
        }
    }

    public class Book
    {
        public Book(string title, string author)
        {
            Title = title;
            Author = author;
        }

        public string Title { get; set; }
        public string Author { get; set; }
    }
}