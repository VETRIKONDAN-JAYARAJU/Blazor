using DataObjects;
using DataServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Dapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace PaymentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        private readonly IConfiguration _config;
        public BankAccountController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<List<BankAccount>>> SelectAll()
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryAsync<BankAccount>("SELECT bankaccountid, bank.bankname, accountno, name, nickname, qrcode.qrcodename, bankloginurl, loginid, password, province, state, type, recordstatus.status FROM bankaccount, bank, qrcode, recordstatus WHERE bankaccount.bankid = bank.bankid AND bankaccount.qrcodeid = qrcode.qrcodeid AND bankaccount.statusid = recordstatus.statusid order by bankaccountid");
            return Ok(result);
        }

        [HttpGet]
        [Route("get/{BankAccountId:int}")]
        public async Task<ActionResult<BankAccount>> SelectById(int BankAccountId)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryFirstAsync<BankAccount>("SELECT * FROM bankaccount WHERE bankaccountid = @key", new { BankAccountId });
            return Ok(result);
        }
    }
}
