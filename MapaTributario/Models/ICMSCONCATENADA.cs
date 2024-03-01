namespace Mapa_Tributario.Models
{
    public class ICMSCONCATENADA
    {
        readonly Conexao conexao = new Conexao();
        public NCM_ICMS_Aliquota? Aliquota {  get; set; }
        public NCM_ICMS_AliquotaBENEFICIO_ISENCOES? Beneficios { get; set; }
        public NCM_ICMS_AliquotaBENEFICIO_ISENCOES? Isencoes { get; set; }
       // public NCM_ICMS_Aliquota4DIGITOS? digitos { get; set; }
        public RetornaExcessoesICMS_IPI_piscofins? Excessoes { get; set; }
        public string? Diferimento { get; set; }
        public string? Suspensao { get; set; }
        public string? NaoIncidencia { get; set; }
        public string? CreditoPresumido { get; set; }

       
    }
}
