using MaoSolidaria.Data;
using MaoSolidaria.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaoSolidaria.Pages.Postagens
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Postagem> Postagens { get; set; } = new List<Postagem>();

        public async Task OnGetAsync()
        {
            Postagens = await _context.Postagens
                .Include(p => p.Usuario)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();
        }
    }
}
