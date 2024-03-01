using MaoaTributario.Models;
using teste0.Models.Excecoes;

namespace Mapa_Tributario.Models
{
    public class ModelConcatenada
    {
        readonly Conexao conexao = new Conexao();
        public NCMDescricao? _NCMDesc {  get; set; }
        public NcmExcecao? _NCMDescIdExcessao { get; set; }
        public  Cst_piscofins? _cst_piscofins { get; set; }
        public  Piscofins? _piscofins { get; set; }
        public  Ipi? _ipi {  get; set; }
        public IPI_EX? _ipiEX { get; set; }
        public PisCofinsExcecao? _pcEX { get; set; }
        public CSTEX? _CSTEX { get; set; }
        public CSTPiscofinsEX? _CSTPISCOFINEX { get; set; }
    }
}
