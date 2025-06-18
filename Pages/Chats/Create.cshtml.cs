using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MaoSolidaria.Models;
using MaoSolidaria.Data;

namespace MaoSolidaria.Pages.Chats
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Chat Chat { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Chats.Add(Chat);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
