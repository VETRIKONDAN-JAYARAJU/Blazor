using DataObjects;
using Dapper;

namespace DataServices
{
    public class CurrencyService : IGenericRepository<Currency, int>
    {
        private readonly DBConnection connection;

        public CurrencyService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<Currency>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Currency>("SELECT currencyid, currencyname, currencycode, recordstatus.status FROM currency, recordstatus WHERE currency.statusid = recordstatus.statusid ORDER BY currencyid");
            return result.ToList();
        }

        public async Task<List<Currency>> SelectActive()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Currency>("SELECT currencyid, currencycode, CurrencyName FROM currency WHERE statusid = 1 ORDER BY currencyid");
            return result.ToList();
        }

        public async Task<Currency> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Currency>("SELECT * FROM currency WHERE currencyid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(Currency record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM currency WHERE currencyname = Trim(@CurrencyName) or currencycode = @CurrencyCode", new { record.CurrencyName, record.CurrencyCode }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO currency (currencyname, currencycode, createdby) VALUES (@CurrencyName, @CurrencyCode, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> Update(Currency record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM currency WHERE currencyname = Trim(@CurrencyName) AND currencycode = Trim(@CurrencyCode)", new { record.CurrencyName, record.CurrencyCode }).FirstOrDefault();

            if (isExist == true)
            {
                isExist = db.Query<bool>("SELECT count(*) FROM currency WHERE currencyname = Trim(@CurrencyName) AND currencycode = Trim(@CurrencyCode) AND statusid=@StatusId", new { record.CurrencyName, record.CurrencyCode, record.StatusId }).FirstOrDefault();
                if (isExist == true)
                {
                    return false;
                }
                else
                {
                    await db.ExecuteAsync(@"UPDATE currency SET statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE currencyid = @CurrencyId", record);
                    return true;
                }
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE currency SET currencyname=@CurrencyName, currencycode=@CurrencyCode, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE currencyid = @CurrencyId", record);
                return true;
            }
        }

        public async Task<bool> Delete(int key)
        {
            var db = connection.Create();
            var rowsAffected = await db.ExecuteAsync("UPDATE currency SET statusid = 3 WHERE currencyid = @CurrencyId", new { CurrencyId = key });
            return (rowsAffected > 0);
        }
    }
}