using System.Text.Json.Serialization;

namespace Mapa_Tributario.Models
{
    public class NCM_ICMS_Aliquota4DIGITOS
    {
        public string? descricao { get; set; }
        public string? ncm { get; set; }
        public string? lista { get; set; }
        public string? categoria { get; set; }
        public string? mva { get; set; }
        public string? CEST {  get; set; }
        public string? signatarios { get; set; }
        public string? obs { get; set; }
        public MVA? CalculoMVA { get; set; }
        public Regra? RegrasMVA { get; set; }
        [JsonIgnore]
        public string? UF { get; set; }
        [JsonIgnore]
        public string? IdCategoria { get; set; }
    }
}
