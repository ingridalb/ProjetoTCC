using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MaoSolidaria.Models;
using MaoSolidaria.Data;

namespace MaoSolidaria.Pages.Chats
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
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
            var chat = await _context.Chats.FindAsync(Chat.Id);

            if (chat == null)
                return NotFound();

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
