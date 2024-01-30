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
    public class DeviceBindController : ControllerBase
    {
        private readonly IConfiguration _config;

        public DeviceBindController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<List<DeviceBind>>> SelectAll()
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryAsync<DeviceBind>("SELECT devicebindid, device.devicename, merchant.merchantname, bank.bankname, accountno, recordstatus.status FROM devicebind, device, merchant, bank, recordstatus WHERE devicebind.deviceid = device.deviceid AND devicebind.merchantid = merchant.merchantid AND devicebind.bankid = bank.bankid AND devicebind.statusid = recordstatus.statusid ORDER BY devicebindid");
            return Ok(result);
        }

        [HttpGet]
        [Route("get/{DeviceBindId:int}")]
        public async Task<ActionResult<DeviceBind>> SelectById(int DeviceBindId)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryFirstAsync<DeviceBind>("SELECT * FROM devicebind WHERE devicebindid = @Key", new { Key = DeviceBindId });
            return Ok(result);
        }
    }
}
