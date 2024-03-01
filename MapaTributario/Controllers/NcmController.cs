using Microsoft.AspNetCore.Mvc;
using Mapa_Tributario.DAO;
using Mapa_Tributario.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace MapaTributario.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class NcmController : ControllerBase
    {
        private readonly MapaTributarioDAO mapaTributarioDAO;
        private readonly ExecaoDao execaoDao;

        public NcmController(IConfiguration configuration)
        {
            mapaTributarioDAO = new MapaTributarioDAO();
            execaoDao = new ExecaoDao();
        }

        [HttpPost]
        [Authorize]
        [EnableRateLimiting("fixed")]
        [Route("VerificarExistenciaNCM")]
        public IActionResult VerificarExistenciaNCM(string ncm)
        {
            bool existeNCM = mapaTributarioDAO.VerificarExistenciaNCM(ncm);

            if (existeNCM)
            {
                return Ok("NCM encontrada no banco de dados.");
            }
            else
            {
                return NotFound("NCM não encontrado no banco de dados.");
            }
        }
        [HttpPost]
        [Authorize]
        [EnableRateLimiting("fixed")]
        [Route("ListaFederalNCMS/")]
        public IActionResult GetListFederalNCMS(string ncm)
        {
            try
            {
                var ncmFederal = execaoDao.GetNcmFederal(ncm);
                var headers = Request.Headers;
                var body = Request.Body;
                
                return Ok(ncmFederal);
            }
            catch
            {
                return BadRequest("DefaultConnection");
            }
        }
        [HttpPost]
        [Authorize]
        [EnableRateLimiting("fixed")]
        [Route("ListaESTADUAL_NCMS_ICMS")]
        public IActionResult GetListaEstadualNCMS_ICMS(VARIAS_NCMs_UF _NCMS)
        {
            try
            {
                var headers = Request.Headers;
                var body = Request.Body;

                return Ok(mapaTributarioDAO.RetornaListaEstadual(_NCMS));
            }
            catch
            {
                return BadRequest("DefaultConnection");
            }
        }
    }
}

