using DataObjects;
using Dapper;

namespace DataServices
{
    public class DepositService
    {
        public readonly DBConnection connection;

        public DepositService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<Deposit>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Deposit>("SELECT * FROM deposit");
            return result.ToList();
        }


        // Select record using MerchantId for Commission Calaculation

        public List<Deposit> GetSuccessDepositDetails(int key)
        {
            List<Deposit> results = new List<Deposit>();

            var db = connection.Create();
            results = db.Query<Deposit>("SELECT depositid, merchantid, currencycode, amount, createdon FROM Deposit WHERE  statusid = 1 AND merchantid = @key", new { key }).ToList();
            return results;
        }




       // var result = await connection.QueryAsync<Payout>("SELECT po.payoutid, me.merchantname, bk.bankname, ba.accountno, dv.devicename,  po.ordercorrespondenceid, po.membercorrespondenceid, po.referenceno, pt.paymenttype, po.currencycode, po.targetaccountno, po.customername, po.amount, po.actualamount,  po.transactioncode, ps.status FROM payout po JOIN merchant me ON po.merchantid = me.merchantid JOIN bank bk ON po.bankid = bk.bankid  JOIN bankaccount ba ON po.bankaccountid = ba.bankaccountid JOIN devicebind db ON po.bankid = db.bankid JOIN device dv ON db.deviceid = dv.deviceid JOIN paymenttype pt ON po.paymenttypeid = pt.paymenttypeid JOIN paymentstatus ps ON po.statusid = ps.statusid ORDER BY po.payoutid");

        //public Task<bool> Delete(int key)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> Insert(Deposit record)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<List<Deposit>> SelectActive()
        //{
        //    throw new NotImplementedException();
        //}



        //public Task<Deposit> SelectById(int key)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> Update(Deposit record)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
