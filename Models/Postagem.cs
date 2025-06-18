using System.Collections.Generic;

namespace MaoSolidaria.Models
{
    public class Postagem
    {
        public int Id { get; set; }
        public string Texto { get; set; }
        public string? CaminhoImagem { get; set; }
        public DateTime DataCriacao { get; set; }
        public string UsuarioId { get; set; }
        public virtual ApplicationUser Usuario { get; set; }
        public virtual ICollection<Comentario> Comentarios { get; set; }

        public Postagem()
        {
            Comentarios = new List<Comentario>();
        }

    }
}