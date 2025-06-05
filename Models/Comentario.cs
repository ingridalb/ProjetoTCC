using System.Collections.Generic;

namespace MaoSolidaria.Models
{
    public class Comentario
    {
        public int Id { get; set; }
        public string Texto { get; set; }
        public DateTime DataCriacao { get; set; }
        public int PostagemId { get; set; }
        public virtual Postagem Postagem { get; set; }
        public string UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}