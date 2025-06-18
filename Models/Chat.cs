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
        public virtual ApplicationUser Remetente { get; set; }

        [ForeignKey("DestinatarioId")]
        public virtual ApplicationUser Destinatario { get; set; }
    }
}