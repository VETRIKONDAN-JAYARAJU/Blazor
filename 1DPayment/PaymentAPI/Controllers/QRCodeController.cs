using DataObjects;
using DataServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Dapper;

namespace PaymentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRCodeController : ControllerBase
    {
        private readonly IConfiguration _config;
        public QRCodeController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<List<QRCode>>> SelectAll()
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryAsync<QRCode>("SELECT qrcodeid, qrcodename, filename, filepath, recordstatus.status FROM qrcode, recordstatus WHERE qrcode.statusid = recordstatus.statusid ORDER BY qrcodeid");
            return Ok(result);
        }

        [HttpGet]
        [Route("get/{QRCodeId:int}")]
        public async Task<ActionResult<QRCode>> SelectById(int QRCodeId)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryFirstAsync<QRCode>("SELECT * FROM qrcode WHERE qrcodeid = @Key", new { Key = QRCodeId });
            return Ok(result);
        }
    }
}
