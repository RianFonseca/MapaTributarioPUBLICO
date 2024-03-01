using Mapa_Tributario.Models;
using System.Data.SqlClient;
using teste0.Models.Excecoes;
using MaoaTributario.Models;
using MapaTributario.Models.Excecoes;

namespace Mapa_Tributario.DAO
{
    public class ExecaoDao
    {
        private readonly string connectionString;
        public ExecaoDao()
        {
            var sqlServerConnectionString = Environment.GetEnvironmentVariable("SQLServer");
            if (string.IsNullOrEmpty(sqlServerConnectionString))
            {
                Console.WriteLine("Error: Connection string 'SQLServer' is empty or null.");
                throw new InvalidOperationException("Failed to retrieve the 'SQLServer' from environment variables.");
            }
            this.connectionString = sqlServerConnectionString;
            if (string.IsNullOrEmpty(this.connectionString))
            {
                throw new InvalidOperationException("A variável de ambiente 'SQLServer' não foi definida.");
            }
        }
        public ListaFederal GetNcmFederal(string ncm)
        {
            var obj = new ModelConcatenada();
            var listaFederal = new ListaFederal();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    var ncmsubitem = GetIdSubitem(ncm, conn);
                    var ncmExecao = GetIdExcessao(ncmsubitem.IdSubitem, conn);
                    var idExcecao = GetIdExecao(ncmsubitem.IdSubitem, conn);
                    var ncmDescricao = GetNCMDescricao(ncm, conn);
                    var cstEx = GetCstEx(idExcecao, conn);
                    var idExPisCofins = GetPisconfinsEX(idExcecao, conn);
                    var ipi_ex = GetIPI_EX(idExcecao.ToString(), conn);
                    var piscofins = GetPiscofins(ncm, conn);

                    listaFederal.NcmSubitem = ncmsubitem;
                    listaFederal.NcmExcecao = ncmExecao;
                    listaFederal.Piscofins = piscofins;
                    listaFederal.ListaFederalExcecoes = new object[] { cstEx, idExPisCofins, ipi_ex };
                }
                finally
                {
                    conn.Close();
                }
            }

             return listaFederal;
        }
        public int GetIdExecao(int idSubItem, SqlConnection conn)
        {
            int idExcecao = 0;
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT idExcecao, descricao, ipi, idSubitem, ipi_dc10979 FROM dbo.ncm_excecao WHERE idSubitem = @idSubitem", conn))
                {
                    cmd.Parameters.AddWithValue("@idSubitem", idSubItem);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        idExcecao = (int)reader["idExcecao"];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetIdExcessao: " + ex.Message);
            }

            return idExcecao;
        }

        public bool GetNCM(string ncm, SqlConnection conn)
        {
            bool _bool = false;
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT idSubItem, subItem, descricao FROM dbo.ncm_subitem WHERE subItem LIKE @ncm", conn))
                {
                    cmd.Parameters.AddWithValue("@ncm", ncm);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        _bool = true;
                    }
                    else
                    {
                        _bool = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return _bool;
        }

        public Ncm_Subitem GetIdSubitem(string ncm, SqlConnection conn)
        {
            Ncm_Subitem idSubItem = new Ncm_Subitem();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT idSubItem, subItem, descricao, idItem, idsubPosicao, idPosicao, idCapitulo FROM dbo.ncm_subitem WHERE subItem = @ncm", conn))
                {
                    cmd.Parameters.AddWithValue("@ncm", ncm);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        idSubItem.IdSubitem = (int)reader["idSubitem"];
                        idSubItem.SubItem = (string)reader["subItem"];
                        idSubItem.Descricao = (string)reader["descricao"];
                        idSubItem.IdItem = (int)reader["idItem"];
                        idSubItem.IdSubPosicao = (int)reader["idsubPosicao"];
                        idSubItem.IdPosicao = (int)reader["idPosicao"];
                        idSubItem.IdCapitulo = (int)reader["idCapitulo"];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetIdSubitem: " + ex.Message);
            }

            return idSubItem;
        }

        public NcmExcecao GetIdExcessao(int idSubItem, SqlConnection conn)
        {
            var ncmExcecao = new NcmExcecao();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT idExcecao, descricao, ipi, idSubitem FROM dbo.ncm_excecao WHERE idSubitem = @idSubItem", conn))
                {
                    cmd.Parameters.AddWithValue("@idSubItem", idSubItem);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        ncmExcecao.IdExcecao = (int)reader["idExcecao"];
                        ncmExcecao.Descricao = (string)reader["descricao"];
                        ncmExcecao.Ipi = (string)reader["ipi"];
                        ncmExcecao.IdSubItem = (int)reader["idSubitem"];
                        ncmExcecao.Ipi_dc = (string)reader["ipi_dc10979"];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetIdExcessao: " + ex.Message);
            }

            return ncmExcecao;
        }

        public CstNcmExcecao GetCstEx(int idExcecao, SqlConnection conn)
        {
            var _CST_ex = new CstNcmExcecao();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT idCstNcm, idCst, idEx FROM dbo.ncm_cst_ncm_ex WHERE idEx = @idExcecao", conn))
                {
                    cmd.Parameters.AddWithValue("@idExcecao", idExcecao);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        _CST_ex.IdCstNcmExcecao = (int)reader["idCstNcm"];
                        _CST_ex.IdCST = (int)reader["idCst"];
                        _CST_ex.IdEx = (int)reader["idEx"];
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error in GetCstEx: " + ex.Message);
            }

            return _CST_ex;
        }

        public PisCofinsExcecao GetPisconfinsEX(int idExcecao, SqlConnection conn)
        {
             PisCofinsExcecao piscofinsExcecao = new PisCofinsExcecao();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT idExPisCos, piscofins, idExcecao FROM dbo.ncm_excecao_piscofins WHERE idExcecao = @idExcecao", conn))
                {
                    cmd.Parameters.AddWithValue("@idExcecao", idExcecao);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        piscofinsExcecao.IdExPisCosfins = (int)reader["idExPisCos"];
                        piscofinsExcecao.TextoHtmlPisCofins = (string)reader["piscofins"];
                        piscofinsExcecao.IdExcecao = (int)reader["idExcecao"];         
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetPisconfinsEX: " + ex.Message);
            }

            return piscofinsExcecao;
        }
        public NCMDescricao GetNCMDescricao(string ncm, SqlConnection conn)
        {
            NCMDescricao desc = new NCMDescricao();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT idSubItem, subItem, descricao FROM dbo.ncm_subitem WHERE subItem LIKE @ncm", conn))
                {
                    ncm = "%" + ncm + "%";
                    cmd.Parameters.AddWithValue("@ncm", ncm);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        desc = new NCMDescricao();
                        desc.idSubItem = (int)reader["idSubItem"];
                        desc.subItem = (string)reader["subItem"];
                        desc.descricao = (string)reader["descricao"];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetNCMDescricao: " + ex.Message);
            }

            return desc;
        }

        public NCMDescricaoIdExcessao GetIdDescricaoIdExcecao(int idSubItem, SqlConnection conn)
        {
            NCMDescricaoIdExcessao desc = new NCMDescricaoIdExcessao();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT idExcecao, descricao, ipi, idSubitem, ipi_dc10979 FROM dbo.ncm_excecao WHERE idSubitem = @idSubItem", conn))
                {
                    cmd.Parameters.AddWithValue("@idSubItem", idSubItem);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        desc.idExcessao = (int)reader["idExcecao"];
                        desc.descricao = (string)reader["descricao"];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetIdDescricaoIdExcecao: " + ex.Message);
            }

            return desc;
        }

        public Ipi GetIPI(string ipi, SqlConnection conn)
        {
            Ipi _ipi = new Ipi();
            try
            {
                using (SqlCommand cmd = new SqlCommand("EXEC monitoramento_buscaripi @ipi", conn))
                {
                    cmd.Parameters.AddWithValue("@ipi", ipi);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        _ipi.ipi = (string)reader["ipi"];
                    }
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return _ipi;
        }

        public IPI_EX GetIPI_EX(string idExcecao, SqlConnection conn)
        {
            IPI_EX _ipiEX = new IPI_EX();
            try
            {
                using (SqlCommand cmd = new SqlCommand("EXEC monitoramento_buscaripi_ex @idExcecao", conn))
                {
                    cmd.Parameters.AddWithValue("@idExcecao", idExcecao);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        _ipiEX.IdExcecao = (int)reader["idExcecao"];
                        _ipiEX.ipi_ex = (string)reader["ipi"];
                        _ipiEX.descricao = (string)reader["descricao"];
                    }
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return _ipiEX;
        }
        public Cst_piscofins GetCstPisconfins(string ncm, SqlConnection conn)
        {
            Cst_piscofins _cst_piscofins = new Cst_piscofins();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT cp.codigo, cp.descricao FROM ncm_cst_ncm cn, ncm_subitem si, ncm_cst_piscofins cp WHERE cn.idNCM = si.idSubItem AND cp.idCST = cn.idCst and si.subItem = @ncm", conn))
                {
                    cmd.Parameters.AddWithValue("@ncm", ncm);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        _cst_piscofins.codigo = (string)reader["idExcecao"];
                        _cst_piscofins.descricao = (string)reader["descricao"];
                    }
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }

            return _cst_piscofins;
        }
        public CSTPiscofinsEX GetCstPisconfinsEX(int idEx, SqlConnection conn)
        {
            CSTPiscofinsEX _CSTPISCOFINEX = new CSTPiscofinsEX();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT t2.* FROM dbo.ncm_cst_ncm_ex t1, ncm_cst_piscofins t2 WHERE t1.idEx = @idEx AND t1.idCst = t2.idCST", conn))
                {
                    cmd.Parameters.AddWithValue("@idEx", idEx);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        _CSTPISCOFINEX.idCST = (int)reader["idExPisCos"];
                        _CSTPISCOFINEX.codigo = (string)reader["piscofins"];
                        _CSTPISCOFINEX.descricao = (string)reader["idExcecao"];
                    }
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }
            return _CSTPISCOFINEX;
        }
        public Piscofins GetPiscofins(string ncm, SqlConnection conn)
        {
            Piscofins _piscofins = new Piscofins();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT pc.piscofins FROM ncm_subitem sub, ncm_piscofins pc WHERE  sub.idSubItem = pc.idSubitem and sub.subItem = @ncm", conn))
                {
                    cmd.Parameters.AddWithValue("@ncm", ncm);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        _piscofins.piscofins = (string)reader["piscofins"];
                    }
                }
            }
            catch (Exception ex)
            {
                string teste = ex.Message;
            }
            return _piscofins;
        }
    }
}
    