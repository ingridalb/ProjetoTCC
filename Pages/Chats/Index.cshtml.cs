using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MaoSolidaria.Models;
using MaoSolidaria.Data;

namespace MaoSolidaria.Pages.Chats
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Chat> Mensagens { get; set; }

        public async Task OnGetAsync()
        {
            Mensagens = await _context.Chats.ToListAsync();
        }
    }
}
