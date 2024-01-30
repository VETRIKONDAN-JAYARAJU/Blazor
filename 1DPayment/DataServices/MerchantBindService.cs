using DataObjects;
using Dapper;

namespace DataServices
{
    public class MerchantBindService
    {
        private readonly DBConnection connection;

        public MerchantBindService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<MerchantBind>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<MerchantBind>("SELECT merchantbindid, merchant.merchantname, merchantbind.bankid, bank.bankname, bankaccount.accountno, recordstatus.status FROM merchantbind, merchant, bank, bankaccount, recordstatus WHERE merchantbind.merchantid = merchant.merchantid AND merchantbind.bankid = bank.bankid AND merchantbind.bankaccountid = bankaccount.bankaccountid AND merchantbind.statusid = recordstatus.statusid GROUP BY merchant.merchantname, bank.bankname, bankaccount.accountno");
            return result.ToList();
        }

        public async Task<List<MerchantBind>> SelectActive()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<MerchantBind>("SELECT * FROM merchantbind WHERE statusid = 1 ORDER BY merchantbindid");
            return result.ToList();
        }

        public async Task<MerchantBind> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<MerchantBind>("SELECT * FROM merchantbind WHERE merchantbindid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(MerchantBind record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT COUNT(*) FROM merchantbind WHERE merchantid = @MerchantId AND bankid = @BankId AND bankaccountid = @BankAccountId", new { record.MerchantId, record.BankId, record.BankAccountId }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO merchantbind (merchantid, bankid, bankaccountid, createdby) VALUES (@MerchantId, @BankId, @BankAccountId, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> Update(MerchantBind record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM merchantbind WHERE merchantid = @MerchantId AND bankid = @BankId AND bankaccountid = @BankAccountId", new { record.MerchantId, record.BankId, record.BankAccountId }).FirstOrDefault();

            if (isExist == true)
            {
                isExist = db.Query<bool>("SELECT count(*) FROM merchantbind WHERE merchantid = @MerchantId AND bankid = @BankId AND bankaccountid = @BankAccountId AND statusid=@StatusId", new { record.MerchantId, record.BankId, record.BankAccountId, record.StatusId }).FirstOrDefault();
                if (isExist == true)
                {
                    return false;
                }
                else
                {
                    await db.ExecuteAsync(@"UPDATE merchantbind SET statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE merchantbindid = @MerchantBindId", record);
                    return true;
                }
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE merchantbind SET merchantid=@MerchantId, bankid=@BankId, bankaccountid=@BankAccountId, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE merchantbindid = @MerchantBindId", record);

                // This Line is for Audit Log
                await db.ExecuteAsync(@"INSERT INTO merchantbind_auditlog (merchantbindid, merchantid, bankid, bankaccountid, createdby) VALUES (@MerchantBindId, @MerchantId, @BankId, @BankAccountId, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> Delete(int key)
        {
            var db = connection.Create();
            var rowsAffected = await db.ExecuteAsync("UPDATE merchantbind SET statusid = 3 WHERE merchantbindid = @MerchantBindId", new { MerchantBindId = key });
            return (rowsAffected > 0);
        }

        // ****************************************************************************************************************************************************************************
        // This is for Settlement API Section

        public async Task<List<MerchantBind>> SelectMerchantName()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<MerchantBind>("SELECT distinct merchantbind.merchantid, merchant.merchantname FROM merchantbind, merchant WHERE merchantbind.merchantid = merchant.merchantid AND merchantbind.statusid = 1");
            return result.ToList();
        }

        public List<MerchantBind> SelectMerchantBankName(int key)
        {
            List<MerchantBind> result = new List<MerchantBind>();
            var db = connection.Create();
            result = db.Query<MerchantBind>("SELECT DISTINCT merchantbind.bankid, bank.bankname FROM merchantbind, bank WHERE merchantbind.bankid = bank.bankid AND merchantbind.statusid = 1 AND merchantbind.merchantid = @key", new { key }).ToList();
            return result;
        }

        public List<MerchantBind> SelectMerchantBankAccountNo(int key, int bid)
        {
            List<MerchantBind> result = new List<MerchantBind>();
            var db = connection.Create();
            result = db.Query<MerchantBind>("SELECT merchantbind.bankaccountid, bankaccount.accountno FROM merchantbind, bankaccount WHERE merchantbind.bankaccountid = bankaccount.bankaccountid AND merchantbind.merchantid = @key AND merchantbind.bankid = @bid ", new { key, bid }).ToList();
            return result;
        }



    }
}