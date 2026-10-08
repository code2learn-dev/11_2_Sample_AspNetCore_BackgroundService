using BackgroundServiceSample.WebMinimalApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BackgroundServiceSample.WebMinimalApi.Pages
{
    public class IndexModel : PageModel
    {
        private readonly RateDbContext _context;

        public IndexModel(RateDbContext context)
        {
            _context = context;
        }

        public IReadOnlyCollection<Currency> Currencies { get; set; }

        public void OnGet()
        {
            Currencies = [.. _context.Currencies.OrderByDescending(a => a.Id)];
        }
    }
}
