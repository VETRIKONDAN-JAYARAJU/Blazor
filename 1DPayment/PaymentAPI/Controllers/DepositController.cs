using DataObjects;
using DataServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Dapper;
using System.Data.Common;

namespace PaymentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositController : ControllerBase
    {
        private readonly IConfiguration _config;

        public DepositController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<List<Deposit>>> SelectAll()
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryAsync<Deposit>("SELECT de.depositid, de.merchantid, me.merchantname, de.ordercorrespondenceid, de.membercorrespondenceid, de.referenceno, de.paymenttypeid, pt.paymenttype, de.bankid, bk.bankname, de.bankaccountid, ba.accountno, de.currencycode, de.amount, de.actualamount, ew.ewalletname, qc.qrcodename, de.transactioncode, ps.status FROM deposit de JOIN merchant me ON de.merchantid = me.merchantid JOIN paymenttype pt ON de.paymenttypeid = pt.paymenttypeid JOIN bank bk ON de.bankid = bk.bankid JOIN bankaccount ba ON de.bankaccountid = ba.bankaccountid LEFT JOIN ewallet ew ON de.ewalletid = ew.ewalletid LEFT JOIN qrcode qc ON de.qrcodeid = qc.qrcodeid JOIN paymentstatus ps ON de.statusid = ps.statusid ORDER BY de.depositid");
            return Ok(result);
        }

        [HttpGet]
        [Route("getbyid/{DepositId:int}")]
        public async Task<ActionResult<Deposit>> SelectById(int DepositId)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryFirstAsync<Deposit>("SELECT * FROM deposit WHERE depositid = @Key", new { Key = DepositId });
            return Ok(result);
        }

        [HttpGet]
        [Route("get/{accountno}/{paymentstatus}")]
        public async Task<ActionResult<Deposit>> SelectByAccountNo(string accountno, string paymentstatus, string? referenceno = null )
        {
            try
            {
                using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
                var result = await connection.QueryAsync<Deposit>("SELECT deposit.depositid, merchant.merchantname, deposit.ordercorrespondenceid, deposit.membercorrespondenceid, deposit.referenceno, paymenttype.paymenttype, bank.bankname, bankaccount.accountno, bankaccount.name, bankaccount.nickname, deposit.currencycode, deposit.amount, deposit.actualamount, ewallet.ewalletname, qrcode.qrcodename, deposit.transactioncode, paymentstatus.status FROM deposit JOIN merchant ON deposit.merchantid = merchant.merchantid JOIN paymenttype ON deposit.paymenttypeid = paymenttype.paymenttypeid JOIN bank ON deposit.bankid = bank.bankid JOIN bankaccount ON deposit.bankaccountid = bankaccount.bankaccountid LEFT JOIN ewallet ON deposit.ewalletid = ewallet.ewalletid LEFT JOIN qrcode ON deposit.qrcodeid = qrcode.qrcodeid JOIN paymentstatus ON deposit.statusid = paymentstatus.statusid WHERE bankaccount.accountno = @AcKey AND paymentstatus.status = @StatusKey AND deposit.referenceno LIKE @ReferenceNo", new { AcKey = accountno, StatusKey = paymentstatus, ReferenceNo = referenceno + "%" });
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Retrieving Data from the Server");
            }
        }


        [HttpPost]
        [Route("add")]
        public async Task<ActionResult<List<Deposit>>> Insert(Deposit record)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            await connection.ExecuteAsync(@"INSERT INTO deposit (merchantid,membercorrespondenceid,referenceno,paymenttypeid,bankid,bankaccountid,currencycode,amount,actualamount,ewalletid,qrcodeid,statusid,createdby) VALUES (@MerchantId,@MemberCorrespondenceId,@ReferenceNo,@PaymentTypeId,@BankId,@BankAccountId,@CurrencyCode,@Amount,@ActualAmount,@EWalletId,@QRCodeId,@StatusId,@CreatedBy)", record);
            
            record.BalanceAmount = connection.QuerySingleOrDefault<decimal>("SELECT SUM(actualamount) AS BalanceAmount FROM deposit WHERE merchantid = @MerchantId AND currencycode = @CurrencyCode", new { merchantid = record.MerchantId, currencycode = record.CurrencyCode });
            if(record.BalanceAmount > 0)
            {
                await connection.ExecuteAsync(@"INSERT INTO merchantbalance(merchantid, currencycode, balanceamount) VALUES (@MerchantId, @CurrencyCode, @BalanceAmount)", record);
            }

         
            return Ok(await SelectAllRecord(connection));
        }




        //[HttpPost]
        //[Route("update")]
        //public async Task<ActionResult<List<Deposit>>> Update(Deposit record)
        //{
        //    try
        //    {
        //        using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
        //        // await connection.ExecuteAsync(@"UPDATE deposit SET merchantid=@MerchantId, membercorrespondenceid=@MemberCorrespondenceId, referenceno=@ReferenceNo, paymenttypeid=@PaymentTypeId,bankid=@BankId,bankaccountid=@BankAccountId,currencycode=@CurrencyCode,amount=@Amount,actualamount=@ActualAmount,ewalletid=@EWalletId,qrcodeid=@QRCodeId,statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE depositid = @DepositId", record);

        //        await connection.ExecuteAsync(@"UPDATE deposit SET transactioncode=@TransactionCode, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE depositid=@DepositId", record);
        //        return Ok();
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, "Error Retrieving Data from the Server");
        //    }
        //}


        [HttpPost]
        [Route("update/{depositid}")]
        public async Task<ActionResult<List<Deposit>>> Update(Deposit record)
        {
            try
            {
                using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
                // await connection.ExecuteAsync(@"UPDATE deposit SET merchantid=@MerchantId, membercorrespondenceid=@MemberCorrespondenceId, referenceno=@ReferenceNo, paymenttypeid=@PaymentTypeId,bankid=@BankId,bankaccountid=@BankAccountId,currencycode=@CurrencyCode,amount=@Amount,actualamount=@ActualAmount,ewalletid=@EWalletId,qrcodeid=@QRCodeId,statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE depositid = @DepositId", record);

                await connection.ExecuteAsync(@"UPDATE deposit SET transactioncode=@TransactionCode, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE depositid=@DepositId", record);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Retrieving Data from the Server");
            }
        }



        [HttpDelete]
        [Route("delete")]
        public async Task<ActionResult<List<Deposit>>> Delete(int DepositId)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.ExecuteAsync("DELETE FROM deposit WHERE depositid = @Key", new { Key = DepositId });
            return Ok(await SelectAllRecord(connection));
        }

        private static async Task<IEnumerable<Deposit>> SelectAllRecord(MySqlConnection connection)
        {
            return await connection.QueryAsync<Deposit>("SELECT * FROM deposit");
        }

    }
}
