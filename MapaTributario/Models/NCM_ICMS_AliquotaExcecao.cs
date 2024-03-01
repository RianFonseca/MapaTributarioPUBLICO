namespace Mapa_Tributario.Models
{
    public class NCM_ICMS_AliquotaExcecao
    {
        public List<NCM_ICMS_AliquotaExcecao>? _Excecao2 {  get; set; }
        public int IdAliquota { get; set; }
        public int idExcecao { get; set; }
        public decimal aliquotas { get; set; }
        public string? notas { get; set; }
    }
}
