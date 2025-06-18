using MaoSolidaria.Data;
using MaoSolidaria.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Hosting;

namespace MaoSolidaria.Pages.PaginaUser
{
    public class PostagensUserModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostagensUserModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Postagem> Postagens { get; set; } = new();

        public ApplicationUser UsuarioLogado { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return RedirectToPage("/Account/Login");
            }

            UsuarioLogado = usuario;

            Postagens = await _context.Postagens
                .Where(p => p.UsuarioId == usuario.Id)
                .Include(p => p.Usuario)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            return Page();
        }

        [BindProperty]
        public Postagem NovaPostagem { get; set; } = new();

        [BindProperty]
        public IFormFile ImagemPostagem { get; set; }

        public async Task<IActionResult> OnPostCriarAsync()
        {
            if (ImagemPostagem != null && ImagemPostagem.Length > 0)
            {
                var fileName = Path.GetFileName(ImagemPostagem.FileName);
                var pasta = Path.Combine("wwwroot", "img", "postagens");
                Directory.CreateDirectory(pasta);
                var caminho = Path.Combine(pasta, fileName);

                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    await ImagemPostagem.CopyToAsync(stream);
                }

                NovaPostagem.CaminhoImagem = "/img/postagens/" + fileName;
            }

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
            {
                return RedirectToPage("/Account/Login");
            }

            NovaPostagem.UsuarioId = usuario.Id;
            NovaPostagem.DataCriacao = DateTime.Now;

            _context.Postagens.Add(NovaPostagem);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        [BindProperty]
        public Postagem PostagemEditada { get; set; }

        public async Task<IActionResult> OnPostEditarAsync()
        {
            var postagem = await _context.Postagens.FindAsync(PostagemEditada.Id);

            if (postagem == null)
            {
                return NotFound();
            }

            postagem.Texto = PostagemEditada.Texto;
            postagem.CaminhoImagem = PostagemEditada.CaminhoImagem;

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

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

        [BindProperty]
        public string ComentarioTexto { get; set; }

        [BindProperty]
        public int ComentarioPostagemId { get; set; }

        public async Task<IActionResult> OnPostAdicionarComentarioAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null || string.IsNullOrWhiteSpace(ComentarioTexto))
            {
                return new JsonResult(new { sucesso = false, mensagem = "Erro ao comentar." });
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
