using Microsoft.AspNetCore.Mvc.RazorPages;
using MaoSolidaria.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaoSolidaria.Data;

namespace MaoSolidaria.Pages.PaginaUser
{
    public class TimelineUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TimelineUserModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public ApplicationUser UsuarioLogado { get; set; } = null!;
        public List<Postagem> Postagens { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Pega o usuário logado
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            UsuarioLogado = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (UsuarioLogado == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            // Carrega as postagens (ordenadas por data decrescente)
            Postagens = await _context.Postagens
                .Include(p => p.Usuario) // Inclui dados do autor
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return Page();
        }

        // Para criar uma nova postagem (caso o formulário seja adicionado de verdade no futuro)
        [BindProperty]
        public string TextoPostagem { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToPage("/Account/Login");

            if (string.IsNullOrWhiteSpace(TextoPostagem))
            {
                ModelState.AddModelError(string.Empty, "Texto da postagem é obrigatório.");
                return await OnGetAsync();
            }

            var novaPostagem = new Postagem
            {
                UsuarioId = userId,
                Texto = TextoPostagem,
                DataCriacao = DateTime.Now
            };

            _context.Postagens.Add(novaPostagem);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
