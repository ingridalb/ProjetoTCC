using MaoSolidaria.Data;
using MaoSolidaria.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MaoSolidaria.Pages.PaginaUser
{
    public class ChatUserModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatUserModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<ApplicationUser> Usuarios { get; set; } = new();
        public List<Chat> Mensagens { get; set; } = new();

        [BindProperty]
        public string TextoMensagem { get; set; }

        [BindProperty(SupportsGet = true)]
        public string DestinatarioId { get; set; }

        public ApplicationUser Destinatario { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var usuarioLogado = await _userManager.GetUserAsync(User);
            if (usuarioLogado == null)
                return RedirectToPage("/Account/Login");

            // Lista de usuários com quem posso conversar
            Usuarios = await _context.Users
                .Where(u => u.Id != usuarioLogado.Id)
                .ToListAsync();

            if (!string.IsNullOrEmpty(DestinatarioId))
            {
                // Conversa entre os dois usuários
                Mensagens = await _context.Chats
                    .Where(c =>
                        (c.RemetenteId == usuarioLogado.Id && c.DestinatarioId == DestinatarioId) ||
                        (c.RemetenteId == DestinatarioId && c.DestinatarioId == usuarioLogado.Id))
                    .Include(c => c.Remetente)
                    .Include(c => c.Destinatario)
                    .OrderBy(c => c.DataEnvio)
                    .ToListAsync();

                Destinatario = await _context.Users.FindAsync(DestinatarioId);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var usuarioLogado = await _userManager.GetUserAsync(User);
            if (usuarioLogado == null || string.IsNullOrEmpty(DestinatarioId))
                return RedirectToPage();

            var novaMensagem = new Chat
            {
                ConteudoMensagem = TextoMensagem,
                RemetenteId = usuarioLogado.Id,
                DestinatarioId = DestinatarioId,
                DataEnvio = DateTime.Now
            };

            _context.Chats.Add(novaMensagem);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { DestinatarioId = DestinatarioId });
        }
    }
}
