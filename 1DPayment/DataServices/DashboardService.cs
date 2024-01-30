using DataObjects;
using Dapper;

namespace DataServices
{
    public class DashboardService
    {
        private readonly DBConnection connection;
        DateTime today = DateTime.Today;

        public DashboardService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        // First List Starts Here
        public int GetTransactionCount()
        {
            var db = connection.Create();
            int result = db.QuerySingleOrDefault<int>("SELECT COUNT(*) FROM deposit WHERE date_format(createdon, '%Y-%m-%d') = @today", new { today });
            return result;
        }

        public int GetSuccessTransactionCount()
        {
            var db = connection.Create();
            int result = db.QuerySingleOrDefault<int>("SELECT COUNT(*) FROM deposit WHERE statusid = 1 and date_format(createdon, '%Y-%m-%d') = @today", new { today });
            return result;
        }

        public int GetFailedTransactionCount()
        {
            var db = connection.Create();
            int result = db.QuerySingleOrDefault<int>("SELECT COUNT(*) FROM deposit WHERE statusid > 1 and date_format(createdon, '%Y-%m-%d') = @today", new { today });
            return result;
        }

        public decimal GetSuccessAmount()
        {
            var db = connection.Create();
            decimal result = db.QuerySingleOrDefault<decimal>("SELECT sum(amount) FROM deposit WHERE statusid = 1 and date_format(createdon, '%Y-%m-%d') = @today", new { today });
            return result;
        }

        // Second List Starts Here

        public int GetTotalTransactionCount()
        {
            var db = connection.Create();
            int result = db.QuerySingleOrDefault<int>("SELECT COUNT(*) FROM deposit");
            return result;
        }

        public int GetTotalSuccessTransactionCount()
        {
            var db = connection.Create();
            int result = db.QuerySingleOrDefault<int>("SELECT COUNT(*) FROM deposit WHERE statusid = 1");
            return result;
        }

        public decimal GetTotalSuccessAmount()
        {
            var db = connection.Create();
            decimal result = db.QuerySingleOrDefault<decimal>("SELECT SUM(amount) FROM deposit WHERE statusid = 1");
            return result;
        }

        // ----------------------------------------------------------------------

        public async Task<List<Deposit>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Deposit>("SELECT deposit.depositid, deposit.merchantid, merchant.merchantname, deposit.ordercorrespondenceid, deposit.membercorrespondenceid, deposit.referenceno, deposit.paymenttypeid, paymenttype.paymenttype, deposit.bankid, bank.bankname, deposit.bankaccountid, bankaccount.accountno, deposit.currencycode, deposit.amount, deposit.actualamount, ewallet.ewalletname, qrcode.qrcodename, deposit.transactioncode, paymentstatus.status FROM deposit JOIN merchant ON deposit.merchantid = merchant.merchantid JOIN paymenttype ON deposit.paymenttypeid = paymenttype.paymenttypeid JOIN bank ON deposit.bankid = bank.bankid JOIN bankaccount ON deposit.bankaccountid = bankaccount.bankaccountid LEFT JOIN ewallet ON deposit.ewalletid = ewallet.ewalletid LEFT JOIN qrcode ON deposit.qrcodeid = qrcode.qrcodeid JOIN paymentstatus ON deposit.statusid = paymentstatus.statusid order by deposit.depositid LIMIT 10");
            return result.ToList();
        }
    }
}