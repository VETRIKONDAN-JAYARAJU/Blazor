using DataObjects;
using Dapper;

namespace DataServices
{
    public class BankService : IGenericRepository<Bank, int>
    {
        private readonly DBConnection connection;

        public BankService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<Bank>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Bank>("SELECT bankid, bankname, recordstatus.status FROM bank, recordstatus WHERE bank.statusid = recordstatus.statusid ORDER BY bankid");
            return result.ToList();
        }

        public async Task<List<Bank>> SelectActive()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Bank>("SELECT bankid, bankname FROM bank WHERE statusid = 1 ORDER BY bankid");
            return result.ToList();
        }

        public async Task<Bank> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Bank>("SELECT * FROM bank WHERE bankid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(Bank record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM bank WHERE bankname = Trim(@BankName)", new { record.BankName }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO bank (bankname, createdby) VALUES (@BankName, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> Update(Bank record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM bank WHERE bankname = Trim(@BankName)", new { record.BankName }).FirstOrDefault();

            if (isExist == true)
            {
                isExist = db.Query<bool>("SELECT count(*) FROM bank WHERE bankname = Trim(@BankName) AND statusid=@StatusId", new { record.BankName, record.StatusId }).FirstOrDefault();
                if (isExist == true)
                {
                    return false;
                }
                else
                {
                    await db.ExecuteAsync(@"UPDATE bank SET statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE bankid = @BankId", record);
                    return true;
                }
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE bank SET bankname=@BankName, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE bankid = @BankId", record);
                return true;
            }
        }

        public async Task<bool> Delete(int key)
        {
            var db = connection.Create();
            var rowsAffected = await db.ExecuteAsync("UPDATE bank SET statusid = 3 WHERE bankid = @BankId", new { BankId = key });
            return (rowsAffected > 0);
        }
    }
}