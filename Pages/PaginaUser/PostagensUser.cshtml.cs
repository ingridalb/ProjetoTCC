using MaoSolidaria.Data;
using MaoSolidaria.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace MaoSolidaria.Pages.PaginaUser
{
    public class PostagensUserModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        // CONSTRUTOR
        public PostagensUserModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        public List<Postagem> Postagens { get; set; } = new();
        public ApplicationUser UsuarioLogado { get; set; }

        // GET
        public async Task<IActionResult> OnGetAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario is null) return RedirectToPage("/Account/Login");

            UsuarioLogado = usuario;

            Postagens = await _context.Postagens
                                      .Where(p => p.UsuarioId == usuario.Id)
                                      .Include(p => p.Usuario)
                                      .OrderByDescending(p => p.DataCriacao)
                                      .ToListAsync();

            return Page();
        }

        // CRIAR 
        [BindProperty] public Postagem NovaPostagem { get; set; } = new();
        [BindProperty] public IFormFile ImagemPostagem { get; set; }

        public async Task<IActionResult> OnPostCriarAsync()
        {
            if (ImagemPostagem is { Length: > 0 })
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImagemPostagem.FileName)}";
                var pasta = Path.Combine(_env.WebRootPath, "img", "postagens");
                Directory.CreateDirectory(pasta);

                var path = Path.Combine(pasta, fileName);
                await using var fs = new FileStream(path, FileMode.Create);
                await ImagemPostagem.CopyToAsync(fs);

                NovaPostagem.CaminhoImagem = $"/img/postagens/{fileName}";
            }

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return RedirectToPage("/Account/Login");

            NovaPostagem.UsuarioId = usuario.Id;
            NovaPostagem.DataCriacao = DateTime.Now;

            _context.Postagens.Add(NovaPostagem);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        // EDITAR 
        [BindProperty] public Postagem PostagemEditada { get; set; }
        [BindProperty] public IFormFile ImagemNova { get; set; }
        [BindProperty] public string ImagemAntiga { get; set; }

        public async Task<IActionResult> OnPostEditarAsync()
        {
            var postagem = await _context.Postagens.FindAsync(PostagemEditada.Id);
            if (postagem == null) return NotFound();

            postagem.Texto = PostagemEditada.Texto;

            if (ImagemNova is { Length: > 0 })
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImagemNova.FileName)}";
                var pasta = Path.Combine(_env.WebRootPath, "img", "postagens");
                Directory.CreateDirectory(pasta);

                var path = Path.Combine(pasta, fileName);
                await using var fs = new FileStream(path, FileMode.Create);
                await ImagemNova.CopyToAsync(fs);

                postagem.CaminhoImagem = $"/img/postagens/{fileName}";
            }
            else
            {
                postagem.CaminhoImagem = ImagemAntiga;      // mantém a antiga
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        // AJAX - Listar comentários
        public async Task<JsonResult> OnGetComentariosAsync(int id)
        {
            var lista = await _context.Comentarios
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

            return new JsonResult(lista);
        }

        // AJAX - Adicionar comentário
        [BindProperty] public string ComentarioTexto { get; set; }
        [BindProperty] public int ComentarioPostagemId { get; set; }

        public async Task<IActionResult> OnPostAdicionarComentarioAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null || string.IsNullOrWhiteSpace(ComentarioTexto))
                return new JsonResult(new { sucesso = false });

            _context.Comentarios.Add(new Comentario
            {
                Texto = ComentarioTexto,
                DataCriacao = DateTime.Now,
                UsuarioId = usuario.Id,
                PostagemId = ComentarioPostagemId
            });

            await _context.SaveChangesAsync();
            return new JsonResult(new { sucesso = true });
        }
    }
}
