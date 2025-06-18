using MaoSolidaria.Data;
using MaoSolidaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MaoSolidaria.Data;
using MaoSolidaria.Models;

namespace MaoSolidaria.Pages.Postagens
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
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
            if (!ModelState.IsValid) return Page();

            _context.Attach(Postagem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Postagens.Any(p => p.Id == Postagem.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("Index");
        }
    }
}
