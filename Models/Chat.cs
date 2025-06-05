using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaoSolidaria.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public string ConteudoMensagem { get; set; }
        public DateTime DataEnvio { get; set; }
        public string RemetenteId { get; set; }
        public string DestinatarioId { get; set; }

        [ForeignKey("RemetenteId")]
        public virtual Usuario Remetente { get; set; }

        [ForeignKey("DestinatarioId")]
        public virtual Usuario Destinatario { get; set; }
    }
}