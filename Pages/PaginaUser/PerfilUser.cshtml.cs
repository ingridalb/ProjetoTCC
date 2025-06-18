using MaoSolidaria.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaoSolidaria.Pages.PaginaUser
{
    public class PerfilUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PerfilUserModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImagemPerfil.FileName)}";
                var pastaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "usuarios");
                if (!Directory.Exists(pastaDestino))
                {
                    Directory.CreateDirectory(pastaDestino);
                }

                var filePath = Path.Combine(pastaDestino, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImagemPerfil.CopyToAsync(stream);
                }

                usuario.CaminhoImagem = $"/img/usuarios/{fileName}";
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
