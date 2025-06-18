using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MaoSolidaria.Models;
using MaoSolidaria.Data;

namespace MaoSolidaria.Pages.Chats
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Chat Chat { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Chat = await _context.Chats.FindAsync(id);

            if (Chat == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Attach(Chat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Chats.Any(e => e.Id == Chat.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("Index");
        }
    }
}
