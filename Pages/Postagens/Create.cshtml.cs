using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MaoSolidaria.Models;
using MaoSolidaria.Data;

namespace MaoSolidaria.Pages.Postagens
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        public Postagem Postagem { get; set; } = new Postagem();

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(ApplicationDbContext context, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile ImagemPostagem)
        {
            if (!ModelState.IsValid)
                return Page();

            Postagem.DataCriacao = DateTime.Now;

            // Define o usuário logado como dono da postagem
            Postagem.UsuarioId = _userManager.GetUserId(User);

            if (ImagemPostagem != null && ImagemPostagem.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImagemPostagem.FileName)}";
                var pastaDestino = Path.Combine(_env.WebRootPath, "img", "Postagens");

                if (!Directory.Exists(pastaDestino))
                    Directory.CreateDirectory(pastaDestino);

                var filePath = Path.Combine(pastaDestino, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await ImagemPostagem.CopyToAsync(stream);

                Postagem.CaminhoImagem = $"/img/Postagens/{fileName}";
            }

            _context.Postagens.Add(Postagem);
            await _context.SaveChangesAsync();

            return RedirectToPage("/PaginaUser/PostagensUser");
        }
    }
}