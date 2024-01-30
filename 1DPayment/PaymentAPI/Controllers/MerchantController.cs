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
    public class MerchantController : ControllerBase
    {
        private readonly IConfiguration _config;
        public MerchantController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<List<Merchant>>> SelectAll()
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryAsync<Merchant>("SELECT merchantid, merchantname, apiurl, bank.bankname, device.devicekey, uppermerchant, merchanttype, balance, commission, recordstatus.status FROM merchant, bank, device, recordstatus WHERE merchant.bankid = bank.bankid AND merchant.deviceid = device.deviceid AND merchant.statusid = recordstatus.statusid ORDER BY merchantid");
            return Ok(result);
        }

        [HttpGet]
        [Route("get/{MerchantId:int}")]
        public async Task<ActionResult<Merchant>> SelectById(int MerchantId)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryFirstAsync<Merchant>("SELECT * FROM merchant WHERE merchantid = @key", new { Key = MerchantId });
            return Ok(result);
        }

    }
}
