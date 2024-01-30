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
    public class SettlementController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SettlementController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<List<Settlement>>> SelectAll()
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryAsync<Settlement>("SELECT se.settlementid, se.merchantid, me.merchantname, se.bankid, ba.bankname, se.bankaccountid, bac.accountno, bac.name, bac.nickname, se.sourcemerchantid, me1.merchantname as sourcemerchantname, se.sourcebankid, ba1.bankname as sourcebankname, se.sourcebankaccountid, bac1.accountno as sourceaccountno, se.currencycode, se.amount, se.referenceno, se.remarks, se.transactioncode, se.statusid, ps.status FROM settlement se JOIN merchant me ON se.merchantid = me.merchantid JOIN bank ba ON se.bankid = ba.bankid JOIN bankaccount bac ON se.bankaccountid = bac.bankaccountid JOIN merchant me1 ON se.sourcemerchantid = me1.merchantid JOIN bank ba1 ON se.sourcebankid = ba1.bankid JOIN bankaccount bac1 ON se.sourcebankaccountid = bac1.bankaccountid JOIN paymentstatus ps ON se.statusid = ps.statusid");
            return Ok(result);
        }

        [HttpGet]
        [Route("getbyid/{SettlementId:int}")]
        public async Task<ActionResult<Settlement>> SelectById(int SettlementId)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryFirstAsync<Settlement>("SELECT se.settlementid, se.merchantid, me.merchantname, se.bankid, ba.bankname, se.bankaccountid, bac.accountno, bac.name, bac.nickname, se.sourcemerchantid, me1.merchantname as sourcemerchantname, se.sourcebankid, ba1.bankname as sourcebankname, se.sourcebankaccountid, bac1.accountno as sourceaccountno, se.currencycode, se.amount, se.referenceno, se.remarks, se.transactioncode, se.statusid, ps.status FROM settlement se JOIN merchant me ON se.merchantid = me.merchantid JOIN bank ba ON se.bankid = ba.bankid JOIN bankaccount bac ON se.bankaccountid = bac.bankaccountid JOIN merchant me1 ON se.sourcemerchantid = me1.merchantid JOIN bank ba1 ON se.sourcebankid = ba1.bankid JOIN bankaccount bac1 ON se.sourcebankaccountid = bac1.bankaccountid JOIN paymentstatus ps ON se.statusid = ps.statusid WHERE settlementid = @Key", new { Key = SettlementId });
            return Ok(result);
        }

        [HttpGet]
        [Route("get/{accountno}/{paymentstatus}")]
        public async Task<ActionResult<Settlement>> SelectByAccountNo(string accountno, string paymentstatus)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryAsync<Settlement>("SELECT se.settlementid, se.merchantid, me.merchantname, se.bankid, ba.bankname, se.bankaccountid, bac.accountno, bac.name, bac.nickname, se.sourcemerchantid, me1.merchantname as sourcemerchantname, se.sourcebankid, ba1.bankname as sourcebankname, se.sourcebankaccountid, bac1.accountno as sourceaccountno, se.currencycode, se.amount, se.referenceno, se.remarks, se.transactioncode, se.statusid, ps.status FROM settlement se JOIN merchant me ON se.merchantid = me.merchantid JOIN bank ba ON se.bankid = ba.bankid JOIN bankaccount bac ON se.bankaccountid = bac.bankaccountid JOIN merchant me1 ON se.sourcemerchantid = me1.merchantid JOIN bank ba1 ON se.sourcebankid = ba1.bankid JOIN bankaccount bac1 ON se.sourcebankaccountid = bac1.bankaccountid JOIN paymentstatus ps ON se.statusid = ps.statusid WHERE bac1.accountno = @AcKey AND ps.status = @StatusKey", new { AcKey = accountno, StatusKey = paymentstatus });
            return Ok(result);
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult<List<Settlement>>> Insert(Settlement record)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            await connection.ExecuteAsync(@"INSERT INTO settlement (merchantid,bankid,bankaccountid,sourcemerchantid,sourcebankid,sourcebankaccountid,currencycode,amount,referenceno,remarks,transactioncode,statusid,createdby) VALUES (@MerchantId,@BankId,@BankAccountId,@SourceMerchantId,@SourceBankId,@SourceBankAccountId,@CurrencyCode,@Amount,@ReferenceNo,@Remarks,@TransactionCode,@StatusId,@CreatedBy)", record);
            return Ok(await SelectAllRecord(connection));
        }

        //[HttpPost]
        //[Route("update")]
        //public async Task<ActionResult<List<Settlement>>> Update(Settlement record)
        //{
        //    using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
        //    await connection.ExecuteAsync(@"UPDATE settlement SET merchantid=@MerchantId,bankid=@BankId,bankaccountid=@BankAccountId,currencycode=@CurrencyCode,amount=@Amount,remarks=@Remarks,referenceno=@ReferenceNo,statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE settlementid = @SettlementId", record);
        //    return Ok();
        //}

        [HttpPost]
        [Route("update/{settlementid}")]
        public async Task<ActionResult<List<Settlement>>> Update(Settlement record)
        {
            try
            {
                using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
                await connection.ExecuteAsync(@"UPDATE settlement SET transactioncode=@TransactionCode, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE settlementid = @SettlementId", record);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Retrieving Data from the Server");
            }
        }


        [HttpDelete]
        [Route("delete")]
        public async Task<ActionResult<List<Settlement>>> Delete(int SettlementId)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.ExecuteAsync("DELETE FROM settlement WHERE settlementid = @Key", new { Key = SettlementId });
            return Ok(await SelectAllRecord(connection));
        }

        private static async Task<IEnumerable<Settlement>> SelectAllRecord(MySqlConnection connection)
        {
            return await connection.QueryAsync<Settlement>("SELECT * FROM settlement");
        }

    }
}