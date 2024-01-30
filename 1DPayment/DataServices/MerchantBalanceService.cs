using DataObjects;
using Dapper;

namespace DataServices
{
    public class MerchantBalanceService
    {
        private readonly DBConnection connection;

        public MerchantBalanceService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<MerchantBalances>> SelectMerchantBalance(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<MerchantBalances>("SELECT * from MerchantBalance WHERE merchantid = @key", new { key });
            return result.ToList();
        }
    }
}
