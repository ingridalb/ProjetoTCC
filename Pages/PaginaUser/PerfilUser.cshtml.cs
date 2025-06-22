using MaoSolidaria.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaoSolidaria.Pages.PaginaUser
{
    public class PerfilUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PerfilUserModel(
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public ApplicationUser Input { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return NotFound();

            Input = new ApplicationUser
            {
                NomeCompleto = usuario.NomeCompleto,
                Email = usuario.Email,
                PhoneNumber = usuario.PhoneNumber,
                CaminhoImagem = usuario.CaminhoImagem
            };

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(IFormFile ImagemPerfil)
        {
            if (!ModelState.IsValid)
                return Page();

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return NotFound();

            usuario.NomeCompleto = Input.NomeCompleto;
            usuario.Email = Input.Email;
            usuario.PhoneNumber = Input.PhoneNumber;

            // Upload da imagem
            if (ImagemPerfil != null && ImagemPerfil.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{ImagemPerfil.FileName}";
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "usuarios", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImagemPerfil.CopyToAsync(stream);
                }

                usuario.CaminhoImagem = $"/usuarios/{fileName}";
            }

            var result = await _userManager.UpdateAsync(usuario);
            if (!result.Succeeded)
            {
                foreach (var erro in result.Errors)
                    ModelState.AddModelError(string.Empty, erro.Description);

                return Page();
            }

            return RedirectToPage();
        }
    }
}
