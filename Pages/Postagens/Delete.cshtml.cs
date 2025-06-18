using MaoSolidaria.Data;
using MaoSolidaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaoSolidaria.Pages.Postagens
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Postagem Postagem { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Postagem = await _context.Postagens.FindAsync(id);

            if (Postagem == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var postagem = await _context.Postagens.FindAsync(Postagem.Id);

            if (postagem != null)
            {
                _context.Postagens.Remove(postagem);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
