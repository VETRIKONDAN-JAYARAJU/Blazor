using DataObjects;
using DataServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Dapper;
using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;

namespace PaymentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayoutController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;

        public PayoutController(IConfiguration config, IWebHostEnvironment environment)
        {
            _config = config;
            _environment = environment;
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<List<Payout>>> SelectAll()
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            // var result = await connection.QueryAsync<Payout>("SELECT payout.payoutid, merchant.merchantname, bank.bankname, bankaccount.accountno, payout.ordercorrespondenceid, payout.membercorrespondenceid, payout.referenceno, paymenttype.paymenttype, payout.currencycode, payout.targetaccountno, payout.customername, payout.amount, payout.actualamount, payout.transactioncode, paymentstatus.status FROM payout JOIN merchant ON payout.merchantid = merchant.merchantid JOIN bank ON payout.bankid = bank.bankid JOIN bankaccount ON payout.bankaccountid = bankaccount.bankaccountid JOIN paymenttype ON payout.paymenttypeid = paymenttype.paymenttypeid JOIN paymentstatus ON payout.statusid = paymentstatus.statusid ORDER BY payout.payoutid");

            var result = await connection.QueryAsync<Payout>("SELECT po.payoutid, me.merchantname, bk.bankname, ba.accountno, dv.devicename,  po.ordercorrespondenceid, po.membercorrespondenceid, po.referenceno, pt.paymenttype, po.currencycode, po.targetaccountno, po.customername, po.amount, po.actualamount,  po.transactioncode, ps.status FROM payout po JOIN merchant me ON po.merchantid = me.merchantid JOIN bank bk ON po.bankid = bk.bankid  JOIN bankaccount ba ON po.bankaccountid = ba.bankaccountid JOIN devicebind db ON po.bankid = db.bankid JOIN device dv ON db.deviceid = dv.deviceid JOIN paymenttype pt ON po.paymenttypeid = pt.paymenttypeid JOIN paymentstatus ps ON po.statusid = ps.statusid ORDER BY po.payoutid");
            return Ok(result);
        }

        [HttpGet]
        [Route("getbyid/{PayoutId:int}")]
        public async Task<ActionResult<Payout>> SelectById(int PayoutId)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryFirstAsync<Payout>("SELECT * FROM payout WHERE payoutid = @Key", new { Key = PayoutId });
            return Ok(result);
        }

        [HttpGet]
        [Route("get/{accountno}/{paymentstatus}")]
        public async Task<ActionResult<Payout>> SelectByAccountNo(string accountno, string paymentstatus)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.QueryAsync<Payout>("SELECT payout.payoutid, merchant.merchantname, bank.bankname, bankaccount.accountno, bankaccount.name, bankaccount.nickname, payout.ordercorrespondenceid, payout.membercorrespondenceid, payout.referenceno, paymenttype.paymenttype, payout.currencycode, payout.targetaccountno, payout.customername, payout.amount, payout.actualamount, payout.transactioncode, paymentstatus.status FROM payout JOIN merchant ON payout.merchantid = merchant.merchantid JOIN bank ON payout.bankid = bank.bankid JOIN bankaccount ON payout.bankaccountid = bankaccount.bankaccountid JOIN paymenttype ON payout.paymenttypeid = paymenttype.paymenttypeid JOIN paymentstatus ON payout.statusid = paymentstatus.statusid WHERE bankaccount.accountno = @AcKey AND paymentstatus.status = @StatusKey", new { AcKey = accountno, StatusKey = paymentstatus });
            return Ok(result);
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult<List<Payout>>> Insert(Payout record)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            await connection.ExecuteAsync(@"INSERT INTO payout (merchantid,bankid,bankaccountid,membercorrespondenceid,referenceno,paymenttypeid,currencycode,targetaccountno,customername,amount,actualamount,statusid,createdby) VALUES (@MerchantId,@BankId,@BankAccountId,@MemberCorrespondenceId,@ReferenceNo,@PaymentTypeId,@CurrencyCode,@TargetAccountNo,@CustomerName,@Amount,@ActualAmount,@StatusId,@CreatedBy)", record);
            return Ok(await SelectAllRecord(connection));
        }

        //[HttpPost]
        //[Route("Update")]
        //public async Task<ActionResult<List<Payout>>> Update(Payout record)
        //{
        //    using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
        //    await connection.ExecuteAsync(@"UPDATE payout SET merchantid=@MerchantId,bankid=@BankId,bankaccountid=@BankAccountId,membercorrespondenceid=@MemberCorrespondenceId,referenceno=@ReferenceNo,paymenttypeid=@PaymentTypeId,currencycode=@CurrencyCode,targetaccountno=@TargetAccountNo, customerName=@CustomerName,amount=@Amount,actualamount=@ActualAmount,statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE payoutid = @PayoutId", record);
        //    return Ok();
        //}

        [HttpPost]
        [Route("update/{payoutid}")]
        public async Task<ActionResult<List<Payout>>> Update(Payout record)
        {
            try
            {
                using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
                await connection.ExecuteAsync(@"UPDATE payout SET transactioncode=@TransactionCode, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE payoutid=@PayoutId", record);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Retrieving Data from the Server");
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<ActionResult<List<Payout>>> Delete(int PayoutId)
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
            var result = await connection.ExecuteAsync("DELETE FROM payout WHERE payoutid = @Key", new { Key = PayoutId });
            return Ok(await SelectAllRecord(connection));
        }

        private static async Task<IEnumerable<Payout>> SelectAllRecord(MySqlConnection connection)
        {
            return await connection.QueryAsync<Payout>("SELECT * FROM payout");
        }


        //-------------------------------------------

        [HttpPost]
        [Route("uploadimage")]
        public async Task<IActionResult> UploadImage(IFormFile file, int payoutid)
        {
            try
            {
                var fileName = file.FileName;
                var fileSize = file.Length;
                var fileExtension = Path.GetExtension(fileName);

                if (file.Length > 0)
                {
                    if (fileSize > 1048576)
                    {
                        return StatusCode(StatusCodes.Status413PayloadTooLarge, new { message = "Your Uploaded File should be below 1MB" });
                    }
                    else if (fileExtension.Contains("png") || fileExtension.Contains("jpg"))
                    {
                        // combining GUID to create unique name before saving file
                        var uniqueFileName = $"{Guid.NewGuid().ToString()}{fileExtension}";

                        // getting full path inside wwwroot
                        var folderDirectory = $"{_environment.WebRootPath}\\payoutimages";
                        var filePath = Path.Combine(folderDirectory, uniqueFileName);

                        // copying file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                            stream.Dispose();
                        }

                        // Save into Database
                      //  using var connection = new MySqlConnection(_config.GetConnectionString("MariaDBConnection"));
                      //  await connection.ExecuteAsync(@"INSERT INTO payoutimage (payoutid,payoutimagepath) VALUES (@PayoutId,@FilePath)", new { PayoutId = payoutid, FilePath = uniqueFileName });

                        // return Ok();
                        return StatusCode(StatusCodes.Status200OK, new { message = "File Uploaded Successfully" });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status404NotFound, new { message = "Image format (png/jpg) only allowed" });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent, new { message = "Invalid File" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
