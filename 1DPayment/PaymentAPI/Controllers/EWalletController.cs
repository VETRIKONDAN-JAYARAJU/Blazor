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
    public class EWalletController : ControllerBase
    {
        private readonly IConfiguration _config;
        public EWalletController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<List<EWallet>>> SelectAll()
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryAsync<EWallet>("SELECT ewalletid, ewalletno, ewalletname, qrcode.qrcodename, recordstatus.status FROM ewallet, qrcode, recordstatus WHERE ewallet.qrcodeid = qrcode.qrcodeid AND ewallet.statusid=recordstatus.statusid ORDER BY ewalletid");
            return Ok(result);
        }

        [HttpGet]
        [Route("get/{EWalletId:int}")]
        public async Task<ActionResult<EWallet>> SelectById(int EWalletId)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryFirstAsync<EWallet>("SELECT * FROM ewallet WHERE ewalletid = @key", new { Key = EWalletId });
            return Ok(result);
        }

    }
}
