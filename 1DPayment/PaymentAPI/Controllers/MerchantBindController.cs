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
    public class MerchantBindController : ControllerBase
    {
        private readonly IConfiguration _config;
        public MerchantBindController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<List<MerchantBind>>> SelectAll()
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryAsync<MerchantBind>("SELECT merchantbindid, merchant.merchantname, merchantbind.bankid, bank.bankname, bankaccount.accountno, recordstatus.status FROM merchantbind, merchant, bank, bankaccount, recordstatus WHERE merchantbind.merchantid = merchant.merchantid AND merchantbind.bankid = bank.bankid AND merchantbind.bankaccountid = bankaccount.bankaccountid AND merchantbind.statusid = recordstatus.statusid GROUP BY merchant.merchantname, bank.bankname, bankaccount.accountno");
            return Ok(result);
        }

        [HttpGet]
        [Route("get/{MerchantBindId:int}")]
        public async Task<ActionResult<MerchantBind>> SelectById(int MerchantBindId)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryFirstAsync<MerchantBind>("SELECT * FROM merchantbind WHERE merchantbindid = @Key", new { Key = MerchantBindId });
            return Ok(result);
        }


    }
}
