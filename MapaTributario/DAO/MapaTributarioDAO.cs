using Mapa_Tributari;
using Mapa_Tributario.Models;
using System.Data.SqlClient;

namespace Mapa_Tributario.DAO
{
    public class MapaTributarioDAO
    {
        private readonly string connectionString;

        public MapaTributarioDAO()
        {
            var sqlServerConnectionString = Environment.GetEnvironmentVariable("SQLServer");

            if (string.IsNullOrEmpty(sqlServerConnectionString))
            {
                Console.WriteLine("Erro: A string de conexão 'SQLServer' está vazia ou nula.");
                throw new InvalidOperationException("Falha ao recuperar o 'SQLServer' das variáveis ​​de ambiente.");
            }
                this.connectionString = sqlServerConnectionString;
            if (string.IsNullOrEmpty(this.connectionString))
            {
                throw new InvalidOperationException("A variável de ambiente 'SQLServer' não foi definida.");
            }
        }

        readonly Conexao Sql = new Conexao();
        public bool VerificarExistenciaNCM(string ncm)
        {
            string queryString = "SELECT idSubItem FROM dbo.ncm_subitem WHERE subItem LIKE @ncm";
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    command.Parameters.AddWithValue("@ncm", ncm);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read()) return true;
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public MVA GerarMVACalculo(CalcMVA calc)
        {
            var mva = GerarMVA(calc.AliquotaInterna, calc.MVA);
            return mva;
        }
        public class ResultadoICMS
        {
            public ICMSCONCATENADA? ListaICMS { get; set; }
        }

        public ICMSCONCATENADA RetornaListaEstadual(VARIAS_NCMs_UF _NCMS)
        {
            var connString = Sql.conectar;
            var obj = new ICMSCONCATENADA();

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                try
                {
                    obj.Aliquota = RetornaNCM_ICMS_ALIQUOTAS(_NCMS.NCM, _NCMS.uf, conn);
                    obj.Beneficios = RetornaNCM_ICMS_BENEFICIOS(_NCMS.NCM, _NCMS.uf, conn);
                    obj.Isencoes = RetornaNCM_ICMS_ISENCOES(_NCMS.NCM, _NCMS.uf, conn);
                    obj.Excessoes = RetornaEXCESSOES(_NCMS.NCM, _NCMS.uf, conn);
                    obj.Diferimento = ICMS_Diferimento(_NCMS.NCM, _NCMS.uf, conn);
                    obj.Suspensao = ICMS_Suspensao(_NCMS.NCM, _NCMS.uf, conn);
                    obj.NaoIncidencia = ICMS_NaoIncidencia(_NCMS.NCM, _NCMS.uf, conn);
                    obj.CreditoPresumido = ICMS_CreditoPresumido(_NCMS.NCM, _NCMS.uf, conn);
                }
                finally
                {
                    conn.Close();
                }
            }

            return obj;
        }

        public Cst_piscofins RetornaCst_piscofins(string ncm, SqlConnection conn)
        {
            Cst_piscofins CSTpisconfins = new Cst_piscofins();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 cp.codigo, cp.descricao FROM ncm_cst_ncm cn, ncm_subitem si, ncm_cst_piscofins cp WHERE cn.idNCM = si.idSubItem AND cp.idCST = cn.idCst and si.subItem = @ncm", conn))
                {
                        cmd.Parameters.AddWithValue("@ncm", ncm);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Cst_piscofins _Cst_piscofins = new Cst_piscofins();

                            _Cst_piscofins = new Cst_piscofins();
                            _Cst_piscofins.codigo = (string)reader["codigo"];
                            _Cst_piscofins.descricao = (string)reader["descricao"];
                        }
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return CSTpisconfins;
        }

        public NCMDescricaoIdExcessao GetIdExcessao(int idSubItem, SqlConnection conn)
        {
            var connString = Sql.conectar;
            NCMDescricaoIdExcessao ncmDescricaoIdExcessao = null;
            try
            {    
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT idExcecao, descricao, ipi, idSubitem FROM dbo.ncm_excecao WHERE idSubitem = @idSubItem";
                        cmd.Parameters.AddWithValue("@idSubItem", idSubItem);
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            ncmDescricaoIdExcessao = new NCMDescricaoIdExcessao();
                            ncmDescricaoIdExcessao.idExcessao = (int)reader["idExcecao"];
                            ncmDescricaoIdExcessao.descricao = (string)reader["descricao"];
                        }
                    }      
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error: " + ex.Message);
            }

            return ncmDescricaoIdExcessao;
        }
       
        public Piscofins RetornaPiscofins(string ncm, SqlConnection conn)
        {
            Piscofins piscofins = new Piscofins();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 pc.piscofins FROM ncm_subitem sub, ncm_piscofins pc WHERE  sub.idSubItem = pc.idSubitem and sub.subItem = @ncm", conn))
                {
                    cmd.Parameters.AddWithValue("@ncm", ncm);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Piscofins piscofin = new Piscofins();
                        piscofins = new Piscofins();
                        piscofins.piscofins = (string)reader["piscofins"];
                    }
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return piscofins;
        }
        
        public NCM_ICMS_Aliquota RetornaNCM_ICMS_ALIQUOTAS(string ncm, string uf, SqlConnection conn)
        {  
            NCM_ICMS_Aliquota aliquota = new NCM_ICMS_Aliquota();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT na.* FROM ncm_subitem ns, ncm_estados ne, ncm_aliquotas na WHERE ns.idSubItem = na.idSubItem AND ne.idEstado = na.idEstado AND ns.subItem = @ncm AND ne.uf = @uf", conn))
                {
                    cmd.Parameters.AddWithValue("@ncm", ncm);
                    cmd.Parameters.AddWithValue("@uf", uf);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        NCM_ICMS_Aliquota Aliq = new NCM_ICMS_Aliquota();
                        aliquota.idAliquota = (int)reader["idAliquota"];
                        aliquota.idSubItem = (int)reader["idSubItem"];
                        aliquota.aliquotas = (decimal)reader["aliquotas"];
                        aliquota.idEstado = (int)reader["idEstado"];
                        aliquota.notas = (string)reader["notas"];
                    }
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return aliquota;
        }
        public NCM_ICMS_AliquotaBENEFICIO_ISENCOES RetornaNCM_ICMS_BENEFICIOS(string ncm, string uf, SqlConnection conn)
        {
            NCM_ICMS_AliquotaBENEFICIO_ISENCOES lstRetornaNCMICMSALIQ = new NCM_ICMS_AliquotaBENEFICIO_ISENCOES();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT nb.* FROM ncm_subitem ns, ncm_estados ne, ncm_beneficios nb WHERE ns.idSubItem = nb.idSubItem AND ne.idEstado = nb.idEstado AND ns.subItem = @ncm AND ne.uf = @uf", conn))
                {       
                    cmd.Parameters.AddWithValue("@ncm", ncm);
                    cmd.Parameters.AddWithValue("@uf", uf);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        lstRetornaNCMICMSALIQ.idAliquota = (int)reader["idAliquota"];
                        lstRetornaNCMICMSALIQ.idSubItem = (int)reader["idSubItem"];
                        lstRetornaNCMICMSALIQ.idEstado = (int)reader["idEstado"];
                        lstRetornaNCMICMSALIQ.notas = (string)reader["notas"];
                    }
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return lstRetornaNCMICMSALIQ;
        }
        public NCM_ICMS_AliquotaBENEFICIO_ISENCOES RetornaNCM_ICMS_ISENCOES(string ncm, string uf, SqlConnection conn)
        {
            var connString = Sql.conectar;
            NCM_ICMS_AliquotaBENEFICIO_ISENCOES lstRetornaNCMICMSALIQ = new();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT nb.* FROM ncm_subitem ns, ncm_estados ne, ncm_isencoes nb WHERE ns.idSubItem = nb.idSubItem AND ne.idEstado = nb.idEstado AND ns.subItem = @ncm AND ne.uf = @uf", conn))
                {
                        cmd.Connection = conn;
                        cmd.Parameters.AddWithValue("@ncm", ncm);
                        cmd.Parameters.AddWithValue("@uf", uf);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            lstRetornaNCMICMSALIQ.idAliquota = (int)reader["idAliquota"];
                            lstRetornaNCMICMSALIQ.idSubItem = (int)reader["idSubItem"];
                            lstRetornaNCMICMSALIQ.idEstado = (int)reader["idEstado"];
                            lstRetornaNCMICMSALIQ.notas = (string)reader["notas"];                        
                        }
                }             
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return lstRetornaNCMICMSALIQ;
        }
        
        public MVA GerarMVA(decimal aliquotaInterna, decimal valorMVA = 0)
        {
            var mva = new MVA();
            if (valorMVA == 0)
            {
                return mva;
            }
            decimal[] aliquotaInterestadual = { 4, 7, 12 };
            var resultadoMVA = new List<decimal>();
            foreach (var aliq in aliquotaInterestadual)
            {
                var calc = (valorMVA + 100) * (100 - aliq) / (100 - aliquotaInterna) - 100;
                if (calc < valorMVA)
                {
                    calc = valorMVA;
                }

                resultadoMVA.Add(calc);
            }
            mva.ValorMVA = valorMVA;
            mva.MVA_4 = Math.Round(resultadoMVA[0], 2);
            mva.MVA_7 = Math.Round(resultadoMVA[1], 2);
            mva.MVA_12 = Math.Round(resultadoMVA[2], 2);
            return mva;
        }
        public decimal CalcularMVA(decimal mva, decimal aliquotaInterna, decimal aliquotaInterestadual)
        {
            var calc = (mva + 100) * (100 - aliquotaInterestadual) / (100 - aliquotaInterna) - 100;
            if (calc < mva)
            {
                calc = mva;
            }

            return calc;
        }
        public Regra GerarRegrasMVA(string estado, int idCategoria)
        {
            var lstRegras = new Regra();
            try
            {
                using (var conn = new SqlConnection(Sql.conectar))
                {
                    using (var cmd = new SqlCommand("EXECUTE mapaTributario_buscarRegraMVA @idCategoria, @estado"))
                    {
                        cmd.Parameters.AddWithValue("@idCategoria", idCategoria);
                        cmd.Parameters.AddWithValue("@estado", estado);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var regra = new Regra()
                                {
                                    Texto = reader["texto"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch
            {               
                return lstRegras;
            }

            return lstRegras;
        }

        public RetornaExcessoesICMS_IPI_piscofins RetornaEXCESSOES(string ncm, string uf, SqlConnection conn)
        {
            var connString = Sql.conectar;
            RetornaExcessoesICMS_IPI_piscofins lstRetornaNCMICMSALIQ = new RetornaExcessoesICMS_IPI_piscofins();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ne.descricao, ne.ipi, ep.piscofins, ea.aliquotas, ea.notas, replace(replace(replace((SELECT cp.codigo + '-' + cp.descricao FROM ncm_cst_ncm_ex ce, ncm_cst_piscofins cp WHERE ce.idCst = cp.idCST AND ce.idEx = ne.idExcecao FOR XML PATH ('cst')),'</cst><cst>','<br>'),'<cst>',''),'</cst>','') AS 'CST' FROM ncm_subitem sb, ncm_excecao ne, ncm_excecao_piscofins ep, ncm_aliquotas_ex ea, ncm_estados e WHERE sb.idSubItem = ne.idSubitem AND ne.idExcecao = ep.idExcecao AND ne.idExcecao = ea.idExcecao AND ea.idEstado = e.idEstado AND e.uf = @uf AND sb.subItem = @ncm", conn))
                {
                        cmd.Connection = conn;
                        cmd.Parameters.AddWithValue("@ncm", ncm);
                        cmd.Parameters.AddWithValue("@uf", uf);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            RetornaExcessoesICMS_IPI_piscofins Aliq = new RetornaExcessoesICMS_IPI_piscofins();
                            lstRetornaNCMICMSALIQ.descricao = (string)reader["descricao"];
                            lstRetornaNCMICMSALIQ.ipi = (string)reader["ipi"];
                            lstRetornaNCMICMSALIQ.piscofins = (string)reader["piscofins"];
                            lstRetornaNCMICMSALIQ.aliquotas = (decimal)reader["aliquotas"];
                            lstRetornaNCMICMSALIQ.notas = (string)reader["notas"];
                            lstRetornaNCMICMSALIQ.CST = (string)reader["CST"];
                        }
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return lstRetornaNCMICMSALIQ;
        }
        public List<EstadoSignatarios> RetornaNCM_EstadosSignatarios(string ncm, string uf)
        {
            var connString = Sql.conectar;
            List<EstadoSignatarios> lstRetornaEstadosSig = new List<EstadoSignatarios>();
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        ncm = "%" + ncm + "%";
                        cmd.CommandText = "SELECT * FROM substituicaoTributaria_cest stc, substituicaoTributaria_produtos_cest stcid,  substituicaoTributaria_MVA stm, substituicaoTributaria_produtos stp, substituicaoTributaria_listas_produtos listaProd, substituicaoTributaria_produtos_descricoes stpd, substituicaoTributaria_listas stl, substituicaoTributaria_UF sUF WHERE stl.idLista = stm.idLista AND stm.idProduto = stp.idProduto AND stp.idProduto = stcid.idProduto AND stc.idCest = stcid.idCest AND stp.idDescricao = stpd.idDescricao AND stp.idProduto = listaProd.idProduto AND stp.ncm like @ncm AND stl.idUF = sUF.idUF AND sUF.uf = @uf AND stm.mva <> 0 ORDER BY CAST(stpd.descricao AS NVARCHAR(5))";
                        cmd.Parameters.AddWithValue("@ncm", ncm);
                        cmd.Parameters.AddWithValue("@uf", uf);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EstadoSignatarios Aliq = new EstadoSignatarios();
                            Aliq.idCategoria = (int)reader["idCategoria"];
                            Aliq.idUF = (string)reader["idUF"];
                            lstRetornaEstadosSig.Add(Aliq);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return lstRetornaEstadosSig;
        }
        public List<EstSigObs> RetornaNCM_EstadosSignatariosOBS(int idCategoria, string iduf)
        {
            var connString = Sql.conectar;
            List<EstSigObs> lstRetornaEstadosSig = new List<EstSigObs>();
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT idObs, idCategoria, iduf, obs FROM dbo.substituicaotributaria_AplicacaoOBS WHERE iduf = @iduf and idCategoria = @idCategoria";
                        cmd.Parameters.AddWithValue("@iduf", iduf);
                        cmd.Parameters.AddWithValue("@idCategoria", idCategoria);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EstSigObs Aliq = new EstSigObs();

                            Aliq.idObs = (int)reader["idCategoria"];
                            Aliq.iduf = (int)reader["iduf"];
                            Aliq.obs = (string)reader["obs"];
                            lstRetornaEstadosSig.Add(Aliq);
                        }

                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return lstRetornaEstadosSig;
        }
        public List<DestinoEstSig> RetornaNCM_DestinoEstadosSignatarios(int idCategoria, string uf)
        {
            var connString = Sql.conectar;
            List<DestinoEstSig> lstRetornaEstadosSig = new List<DestinoEstSig>();
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT destino FROM substituicaotributaria_categorias stc, substituicaotributaria_Aplicacao stma WHERE stc.idCategoria = @idCategoria AND stma.origem = @uf AND stc.idCategoria = stma.idCategoria";
                        cmd.Parameters.AddWithValue("@uf", uf);
                        cmd.Parameters.AddWithValue("@idCategoria", idCategoria);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            DestinoEstSig Aliq = new DestinoEstSig();
                            Aliq.destino = (string)reader["destino"];
                            lstRetornaEstadosSig.Add(Aliq);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return lstRetornaEstadosSig;
        }
        public List<NCM_ICMS_AliquotaIdExcessao> RetornaIdExcessaoICMS(int idSubItem)
        {
            var connString = Sql.conectar;
            List<NCM_ICMS_AliquotaIdExcessao> lstRetornaNCMDesc = new List<NCM_ICMS_AliquotaIdExcessao>();
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT idExcecao, descricao, ipi, idSubitem FROM dbo.ncm_excecao WHERE idSubitem = @idSubItem";
                        cmd.Parameters.AddWithValue("@idSubItem", idSubItem);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            NCM_ICMS_AliquotaIdExcessao desc = new NCM_ICMS_AliquotaIdExcessao();

                            desc.idExcecao = (int)reader["idExcecao"];
                            desc.descricao = (string)reader["descricao"];
                            desc.idSubItem = (int)reader["idSubItem"];
                            lstRetornaNCMDesc.Add(desc);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return lstRetornaNCMDesc;
        }
        public List<NCM_ICMS_AliquotaExcecao> RetornaIdExcessaoICMSFinal(int idExcecao, string uf)
        {
            var connString = Sql.conectar;
            List<NCM_ICMS_AliquotaExcecao> lstRetornaNCMDesc = new List<NCM_ICMS_AliquotaExcecao>();
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT * FROM ncm_aliquotas_ex naX, ncm_estados nEs WHERE idExcecao = @idExcecao and naX.idEstado = nEs.idEstado AND nEs.uf = @uf";
                        cmd.Parameters.AddWithValue("@idExcecao", idExcecao);
                        cmd.Parameters.AddWithValue("@uf", uf);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            NCM_ICMS_AliquotaExcecao desc = new NCM_ICMS_AliquotaExcecao();
                            desc.IdAliquota = (int)reader["idAliquota"];
                            desc.idExcecao = (int)reader["idExcecao"];
                            desc.aliquotas = (decimal)reader["aliquotas"];
                            desc.notas = (string)reader["notas"];
                            lstRetornaNCMDesc.Add(desc);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return lstRetornaNCMDesc;
        }
        public string ICMS_Diferimento(string ncm, string uf, SqlConnection conn)
        {
            string diferimento = "";
            var connString = Sql.conectar;
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT nb.notas FROM ncm_estados ne, ncm_diferimento nb WHERE ne.idEstado = nb.idEstado AND nb.ncm = @ncm AND ne.uf = @uf", conn))
                {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT nb.notas FROM ncm_estados ne, ncm_diferimento nb WHERE ne.idEstado = nb.idEstado AND nb.ncm = @ncm AND ne.uf = @uf";
                        cmd.Parameters.AddWithValue("@ncm", ncm);
                        cmd.Parameters.AddWithValue("@uf", uf);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            diferimento = (string)reader["notas"];
                        }
                        else
                        {
                            diferimento = "Nenhum dispositivo legal cadastrado!";
                        }
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return diferimento;
                    }
        public string ICMS_Suspensao(string ncm, string uf, SqlConnection conn)
        {
            string diferimento = "";
            var connString = Sql.conectar;
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT nb.notas FROM ncm_estados ne, ncm_suspensao nb WHERE ne.idEstado = nb.idEstado AND nb.ncm = @ncm AND ne.uf = @uf", conn))
                {                                     
                        cmd.Parameters.AddWithValue("@ncm", ncm);
                        cmd.Parameters.AddWithValue("@uf", uf);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            diferimento = (string)reader["notas"];
                        }
                        else
                        {
                            diferimento = "Nenhum dispositivo legal cadastrado!";
                        }
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return diferimento;
        }
        public string ICMS_NaoIncidencia(string ncm, string uf, SqlConnection conn)
        {
            string diferimento = "";
            var connString = Sql.conectar;
            try
            {         
                    using (SqlCommand cmd = new SqlCommand("SELECT nb.notas FROM ncm_estados ne, ncm_naoIncidencia nb WHERE ne.idEstado = nb.idEstado AND nb.ncm = @ncm AND ne.uf = @uf", conn))
                    {
                        cmd.Connection = conn;

                        cmd.CommandText = "SELECT nb.notas FROM ncm_estados ne, ncm_naoIncidencia nb WHERE ne.idEstado = nb.idEstado AND nb.ncm = @ncm AND ne.uf = @uf";
                        cmd.Parameters.AddWithValue("@ncm", ncm);
                        cmd.Parameters.AddWithValue("@uf", uf);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            diferimento = (string)reader["notas"];
                        }
                        else
                        {
                            diferimento = "Nenhum dispositivo legal cadastrado!";
                        }
                    }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return diferimento;
        }
        public string ICMS_CreditoPresumido(string ncm, string uf, SqlConnection conn)
        {
            string diferimento = "";
            var connString = Sql.conectar;
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT nb.notas FROM ncm_estados ne, ncm_creditoPresumido nb WHERE ne.idEstado = nb.idEstado AND nb.ncm = @ncm AND ne.uf = @uf", conn))
                {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT nb.notas FROM ncm_estados ne, ncm_creditoPresumido nb WHERE ne.idEstado = nb.idEstado AND nb.ncm = @ncm AND ne.uf = @uf";
                        cmd.Parameters.AddWithValue("@ncm", ncm);
                        cmd.Parameters.AddWithValue("@uf", uf);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            diferimento = (string)reader["notas"];
                        }
                        else
                        {
                            diferimento = "Nenhum dispositivo legal cadastrado!";
                        }
                }               
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return diferimento;
        }
    }
}
