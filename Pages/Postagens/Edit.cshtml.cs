using MaoSolidaria.Data;
using MaoSolidaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;


namespace MaoSolidaria.Pages.Postagens
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EditModel(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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

        [BindProperty]
        public IFormFile ImagemPostagem { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var postagemExistente = await _context.Postagens.FindAsync(Postagem.Id);
            if (postagemExistente == null) return NotFound();

            postagemExistente.Texto = Postagem.Texto;

            if (ImagemPostagem is { Length: > 0 })
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImagemPostagem.FileName)}";
                var pastaDestino = Path.Combine(_env.WebRootPath, "img", "postagens");
                Directory.CreateDirectory(pastaDestino);

                var caminhoFisico = Path.Combine(pastaDestino, fileName);
                await using var stream = new FileStream(caminhoFisico, FileMode.Create);
                await ImagemPostagem.CopyToAsync(stream);
                postagemExistente.CaminhoImagem = $"/img/postagens/{fileName}";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/PaginaUser/PostagensUser");
        }
    }
}
