using MaoSolidaria.Data;
using MaoSolidaria.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MaoSolidaria.Pages.PaginaUser
{
    public class TimelineUserModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public TimelineUserModel(ApplicationDbContext context,
                                  UserManager<ApplicationUser> userManager,
                                  IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        [BindProperty(SupportsGet = true)]
        public string Pesquisa { get; set; } = string.Empty;
        /* ------------------------------------------------------------------ */
        public ApplicationUser UsuarioLogado { get; set; } = null!;
        public List<Postagem> Postagens { get; set; } = new();

        /* GET -------------------------------------------------------------- */
        public async Task<IActionResult> OnGetAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario is null) return RedirectToPage("/Account/Login");

            UsuarioLogado = usuario;

            var query = _context.Postagens
    .Include(p => p.Usuario)
    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Pesquisa))
            {
                query = query.Where(p => p.Texto.ToLower().Contains(Pesquisa.ToLower()));
            }

            Postagens = await query
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return Page();
        }

        /* NOVA POSTAGEM ---------------------------------------------------- */
        [BindProperty] public Postagem NovaPostagem { get; set; } = new();
        [BindProperty] public IFormFile? ImagemPostagem { get; set; }

        public async Task<IActionResult> OnPostCriarAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario is null) return RedirectToPage("/Account/Login");

            if (string.IsNullOrWhiteSpace(NovaPostagem.Texto) &&
                (ImagemPostagem is null || ImagemPostagem.Length == 0))
            {
                ModelState.AddModelError(string.Empty, "Escreva algo ou selecione uma imagem.");
                await OnGetAsync();
                return Page();
            }

            /* Salvar imagem, se houver ------------------------------------ */
            if (ImagemPostagem is { Length: > 0 })
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImagemPostagem.FileName)}";
                var pastaDestino = Path.Combine(_env.WebRootPath, "img", "postagens");
                Directory.CreateDirectory(pastaDestino);

                var caminhoFisico = Path.Combine(pastaDestino, fileName);
                await using (var fs = new FileStream(caminhoFisico, FileMode.Create))
                {
                    await ImagemPostagem.CopyToAsync(fs);
                }

                NovaPostagem.CaminhoImagem = $"/img/postagens/{fileName}";
            }

            NovaPostagem.UsuarioId = usuario.Id;
            NovaPostagem.DataCriacao = DateTime.Now;

            _context.Postagens.Add(NovaPostagem);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        /* COMENTÁRIOS ------------------------------------------------------ */
        [BindProperty] public string ComentarioTexto { get; set; } = string.Empty;
        [BindProperty] public int ComentarioPostagemId { get; set; }

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
            if (usuario is null || string.IsNullOrWhiteSpace(ComentarioTexto))
            {
                return new JsonResult(new { sucesso = false });
            }

            var novo = new Comentario
            {
                Texto = ComentarioTexto,
                DataCriacao = DateTime.Now,
                UsuarioId = usuario.Id,
                PostagemId = ComentarioPostagemId
            };

            _context.Comentarios.Add(novo);
            await _context.SaveChangesAsync();

            return new JsonResult(new { sucesso = true });
        }
    }
}
