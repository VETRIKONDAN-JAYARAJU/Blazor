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
    public class BankController : ControllerBase
    {
        private readonly IConfiguration _config;
        public BankController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<List<Bank>>> SelectAll()
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryAsync<Bank>("SELECT bankid, bankname, recordstatus.status FROM bank, recordstatus WHERE bank.statusid = recordstatus.statusid ORDER BY bankid");
            return Ok(result);
        }

        [HttpGet]
        [Route("get/{BankId:int}")]
        public async Task<ActionResult<Bank>> SelectById(int BankId)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryFirstAsync<Bank>("SELECT * FROM bank WHERE bankid = @Key", new { Key = BankId });
            return Ok(result);
        }

    }
}
