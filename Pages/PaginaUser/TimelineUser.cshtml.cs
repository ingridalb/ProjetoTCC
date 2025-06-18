using Microsoft.AspNetCore.Mvc.RazorPages;
using MaoSolidaria.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaoSolidaria.Data;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Hosting;

namespace MaoSolidaria.Pages.PaginaUser
{
    public class TimelineUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TimelineUserModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _context = context;
            _env = env;
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

        [BindProperty] public string TextoPostagem { get; set; } = string.Empty;
        [BindProperty] public IFormFile ImagemPostagem { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (userId is null) return RedirectToPage("/Account/Login");

            if (string.IsNullOrWhiteSpace(TextoPostagem) && (ImagemPostagem == null || ImagemPostagem.Length == 0))
            {
                ModelState.AddModelError(string.Empty, "Escreva algo ou escolha uma imagem.");
                await OnGetAsync();
                return Page();
            }

            string? caminhoImagem = null;
            if (ImagemPostagem is { Length: > 0 })
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImagemPostagem.FileName)}";
                var pastaDestino = Path.Combine(_env.WebRootPath, "img", "postagens");
                Directory.CreateDirectory(pastaDestino);
                var caminhoFisico = Path.Combine(pastaDestino, fileName);
                await using var fs = new FileStream(caminhoFisico, FileMode.Create);
                await ImagemPostagem.CopyToAsync(fs);

                caminhoImagem = $"/img/postagens/{fileName}";
            }

            var nova = new Postagem
            {
                UsuarioId = userId,
                Texto = TextoPostagem,
                CaminhoImagem = caminhoImagem,
                DataCriacao = DateTime.Now
            };
            _context.Postagens.Add(nova);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        [BindProperty]
        public string ComentarioTexto { get; set; }

        [BindProperty]
        public int ComentarioPostagemId { get; set; }

        public async Task<JsonResult> OnGetComentariosAsync(int id)
        {
            var comentarios = await _context.Comentarios
                .Where(c => c.PostagemId == id)
                .Include(c => c.Usuario)
                .OrderBy(c => c.DataCriacao)
                .Select(c => new
                {
                    nome = c.Usuario.NomeCompleto,
                    imagem = c.Usuario.CaminhoImagem,
                    texto = c.Texto,
                    data = c.DataCriacao.ToString("dd/MM/yyyy HH:mm")
                })
                .ToListAsync();

            return new JsonResult(comentarios);
        }

        public async Task<IActionResult> OnPostAdicionarComentarioAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null || string.IsNullOrWhiteSpace(ComentarioTexto))
            {
                return new JsonResult(new { sucesso = false });
            }

            var novoComentario = new Comentario
            {
                Texto = ComentarioTexto,
                DataCriacao = DateTime.Now,
                UsuarioId = usuario.Id,
                PostagemId = ComentarioPostagemId
            };

            _context.Comentarios.Add(novoComentario);
            await _context.SaveChangesAsync();

            return new JsonResult(new { sucesso = true });
        }
    }
}
