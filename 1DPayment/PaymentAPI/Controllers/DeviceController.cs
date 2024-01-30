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
    public class DeviceController : ControllerBase
    {
        private readonly IConfiguration _config;
        public DeviceController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<List<Device>>> SelectAll()
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryAsync<Device>("SELECT deviceid, devicename, uuid, puid, devicekey, devicetype, recordstatus.status FROM device, recordstatus WHERE device.statusid = recordstatus.statusid ORDER BY deviceid");
            return Ok(result);
        }

        [HttpGet]
        [Route("get/{DeviceId:int}")]
        public async Task<ActionResult<Device>> SelectById(int DeviceId)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryFirstAsync<Device>("SELECT * FROM device WHERE deviceid = @key", new { Key = DeviceId });
            return Ok(result);
        }
    }
}