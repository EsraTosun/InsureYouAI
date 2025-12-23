using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponents.BlogDetailViewComponents
{
    public class _BlogDetailNextAndPrevPostComponentPartial : ViewComponent
    {
        private readonly InsureContext _context;
        public _BlogDetailNextAndPrevPostComponentPartial(InsureContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke(int id)
        {
            var currentArticle = _context.Articles
                .FirstOrDefault(x => x.ArticleId == id);

            if (currentArticle == null)
                return View(); // güvenlik

            var prevArticle = _context.Articles
                .Where(x => x.ArticleId < id)
                .OrderByDescending(x => x.ArticleId)
                .Select(x => new { x.ArticleId, x.Title })
                .FirstOrDefault();

            var nextArticle = _context.Articles
                .Where(x => x.ArticleId > id)
                .OrderBy(x => x.ArticleId)
                .Select(x => new { x.ArticleId, x.Title })
                .FirstOrDefault();

            ViewBag.PrevArticle = prevArticle;
            ViewBag.NextArticle = nextArticle;

            return View();
        }
    }
}
