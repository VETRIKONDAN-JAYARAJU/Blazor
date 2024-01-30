using DataObjects;
using Dapper;

namespace DataServices
{
    public class ReportService
    {
        private readonly DBConnection connection;

        public ReportService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        // Fee Bind Report Section
        public List<FeeBind> FeeBindReport()
        {
            List<FeeBind> result = new List<FeeBind>();

            string query = @"SELECT feebindid, merchant.merchantname, paymenttype.paymenttype, currencycode, feerate, feebindtype, minamount, maxamount, limitamount FROM feebind, merchant, paymenttype WHERE feebind.merchantid = merchant.merchantid AND feebind.paymenttypeid = paymenttype.paymenttypeid ORDER BY feebindid";

            var db = connection.Create();
            result = db.Query<FeeBind>(query).ToList();
            return result;
        }



        // Deposit Report Section
        public List<Deposit> DepositReport()
        {
            List<Deposit> result = new List<Deposit>();

            string query = @"SELECT de.depositid, de.merchantid, me.merchantname, de.ordercorrespondenceid, de.membercorrespondenceid, de.referenceno, de.paymenttypeid, pt.paymenttype, de.bankid, bk.bankname, de.bankaccountid, ba.accountno, de.currencycode, de.amount, de.actualamount, ew.ewalletname, qc.qrcodename, de.transactioncode, ps.status FROM deposit de JOIN merchant me ON de.merchantid = me.merchantid JOIN paymenttype pt ON de.paymenttypeid = pt.paymenttypeid JOIN bank bk ON de.bankid = bk.bankid JOIN bankaccount ba ON de.bankaccountid = ba.bankaccountid LEFT JOIN ewallet ew ON de.ewalletid = ew.ewalletid LEFT JOIN qrcode qc ON de.qrcodeid = qc.qrcodeid JOIN paymentstatus ps ON de.statusid = ps.statusid ORDER BY de.depositid";

            var db = connection.Create();
            result = db.Query<Deposit>(query).ToList();
            return result;
        }


        // Payout Report Section
        public List<Payout> PayoutReport()
        {
            List<Payout> result = new List<Payout>();

            string query = @"SELECT po.payoutid, me.merchantname, bk.bankname, ba.accountno, dv.devicename,  po.ordercorrespondenceid, po.membercorrespondenceid, po.referenceno, pt.paymenttype, po.currencycode, po.targetaccountno, po.customername, po.amount, po.actualamount,  po.transactioncode, ps.status FROM payout po JOIN merchant me ON po.merchantid = me.merchantid JOIN bank bk ON po.bankid = bk.bankid  JOIN bankaccount ba ON po.bankaccountid = ba.bankaccountid JOIN devicebind db ON po.bankid = db.bankid JOIN device dv ON db.deviceid = dv.deviceid JOIN paymenttype pt ON po.paymenttypeid = pt.paymenttypeid JOIN paymentstatus ps ON po.statusid = ps.statusid ORDER BY po.payoutid";

            var db = connection.Create();
            result = db.Query<Payout>(query).ToList();
            return result;
        }

        // Settlement Report Section
        public List<Settlement> SettlementReport()
        {
            List<Settlement> result = new List<Settlement>();

            string query = @"SELECT se.settlementid, se.merchantid, me.merchantname, se.bankid, ba.bankname, se.bankaccountid, bac.accountno, bac.name, bac.nickname, se.sourcemerchantid, me1.merchantname as sourcemerchantname, se.sourcebankid, ba1.bankname as sourcebankname, se.sourcebankaccountid, bac1.accountno as sourceaccountno, se.currencycode, se.amount, se.referenceno, se.remarks, se.transactioncode, se.statusid, ps.status FROM settlement se JOIN merchant me ON se.merchantid = me.merchantid JOIN bank ba ON se.bankid = ba.bankid JOIN bankaccount bac ON se.bankaccountid = bac.bankaccountid JOIN merchant me1 ON se.sourcemerchantid = me1.merchantid JOIN bank ba1 ON se.sourcebankid = ba1.bankid JOIN bankaccount bac1 ON se.sourcebankaccountid = bac1.bankaccountid JOIN paymentstatus ps ON se.statusid = ps.statusid";

            var db = connection.Create();
            result = db.Query<Settlement>(query).ToList();
            return result;
        }

    }
}
