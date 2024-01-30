using DataObjects;
using Dapper;

namespace DataServices
{
    public class CommissionService
    {

        private readonly DBConnection connection;

        public CommissionService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<Commission>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Commission>("SELECT commissionid, merchant.merchantname, transactionid, transactionvalue, commissionearn, newbalance, transactiondate, commissiondate FROM commission, merchant WHERE commission.merchantid = merchant.merchantid");
            return result.ToList();
        }

        public bool IsCommissionCalculate(int key)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT COUNT(*) FROM commission WHERE merchantid = @key", new { key }).FirstOrDefault();
            return isExist;
        }

        public async Task<bool> Insert(Commission record)
        {
            var db = connection.Create();
            await db.ExecuteAsync(@"INSERT INTO commission (merchantid, transactionid, transactionvalue, commissionearn, newbalance, transactiondate) VALUES (@MerchantId, @TransactionId, @TransactionValue, @CommissionEarn, @NewBalance, @TransactionDate)", record);
            return true;
        }
    }
}