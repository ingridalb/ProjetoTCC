using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MaoSolidaria.Models
{
    public class Usuario : IdentityUser
    {
        [Required]
        public string NomeCompleto { get; set; }

        public bool AceitouTermos { get; set; }

        public DateTime? DataAceiteTermos { get; set; }

        public ICollection<Postagem>? Postagens { get; set; }
        public ICollection<Comentario>? Comentarios { get; set; }
        public virtual ICollection<Chat> ChatsEnviados { get; set; }
        public virtual ICollection<Chat> ChatsRecebidos { get; set; }

        public Usuario()
        {
            Postagens = new List<Postagem>();
            Comentarios = new List<Comentario>();
            ChatsEnviados = new List<Chat>();
            ChatsRecebidos = new List<Chat>();
        }

    }
}
