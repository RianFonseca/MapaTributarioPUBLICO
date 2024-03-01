namespace Mapa_Tributario.Models
{
    public class NCM_ICMS_Aliquota
    {
        public int idAliquota {  get; set; }
        public int idSubItem { get; set; }
        public decimal aliquotas { get; set; }
        public int idEstado { get; set; }
        public string? notas { get; set; }
    }
}
