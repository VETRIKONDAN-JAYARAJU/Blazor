using DataObjects;
using Dapper;
using System.Xml.Linq;

namespace DataServices
{
    public class BankAccountService
    {
        private readonly DBConnection connection;

        public BankAccountService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<BankAccount>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<BankAccount>("SELECT bankaccountid, bank.bankname, accountno, name, nickname, qrcode.qrcodename, bankloginurl, loginid, password, province, state, type, balanceamount, currencycode, botstatus, recordstatus.status FROM bankaccount, bank, qrcode, recordstatus WHERE bankaccount.bankid = bank.bankid AND bankaccount.qrcodeid = qrcode.qrcodeid AND bankaccount.statusid = recordstatus.statusid order by bankaccountid");
            return result.ToList();
        }

        public async Task<List<BankAccount>> SelectActive(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<BankAccount>("SELECT bankaccountid, accountno FROM bankaccount WHERE statusid = 1 and bankid = @key", new { key });
            return result.ToList();
        }

        public async Task<BankAccount> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<BankAccount>("SELECT * FROM bankaccount WHERE bankaccountid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(BankAccount record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT COUNT(*) FROM bankaccount WHERE bankid = @BankId AND accountno = @AccountNo AND currencycode=@CurrencyCode", new { record.BankId, record.AccountNo, record.CurrencyCode }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO bankaccount (bankid, accountno, name, nickname, qrcodeid, bankloginurl, loginid, password, province, state, type, balanceamount, currencycode, createdby) VALUES (@BankId, @AccountNo, @Name, @NickName, @QRCodeId,  @BankLoginUrl, @LoginId, @Password, @Province, @State, @Type, @BalanceAmount, @CurrencyCode, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> UpdateBotStatus(BankAccount record)
        {
            var db = connection.Create();
            await db.ExecuteAsync("UPDATE bankaccount SET botstatus = @BotStatus WHERE bankaccountid = @BankAccountId", record);
            return true;
        }

        public async Task<bool> Update(BankAccount record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT COUNT(*) FROM bankaccount WHERE bankid = @BankId AND accountno = @AccountNo AND name=@Name AND nickname=@NickName AND qrcodeid=@QRCodeId AND bankloginurl=@BankLoginUrl AND loginid=@LoginId AND password=@Password AND province=@Province AND state=@State AND type=@Type AND balanceamount=@BalanceAmount AND currencycode = @CurrencyCode", new { record.BankId, record.AccountNo, record.Name, record.NickName, record.QRCodeId, record.BankLoginUrl, record.LoginId, record.Password, record.Province, record.State, record.Type, record.BalanceAmount, record.CurrencyCode }).FirstOrDefault();

            if (isExist == true)
            {
                isExist = db.Query<bool>("SELECT COUNT(*) FROM bankaccount WHERE bankid = @BankId AND accountno = @AccountNo AND statusid=@StatusId", new { record.BankId, record.AccountNo, record.StatusId }).FirstOrDefault();
                if (isExist == true)
                {
                    return false;
                }
                else
                {
                    await db.ExecuteAsync(@"UPDATE bankaccount SET statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE bankaccountid = @BankAccountId", record);
                    return true;
                }
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE bankaccount SET bankid=@BankId, accountno=@AccountNo, name=@Name, nickname=@NickName, qrcodeid=@QRCodeId, bankloginurl=@BankLoginUrl, loginid=@LoginId, password=@Password, province=@Province, state=@State, type=@Type, balanceamount=@BalanceAmount, currencycode=@CurrencyCode, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE bankaccountid = @BankAccountId", record);

                // This Line is for Audit Log
                await db.ExecuteAsync(@"INSERT INTO bankaccount_auditlog (bankaccountid, bankid, accountno, name, nickname, qrcodeid, bankloginurl, loginid, password, province, state, type, balanceamount, createdby) VALUES (@BankAccountId, @BankId, @AccountNo, @Name, @NickName, @QRCodeId,  @BankLoginUrl, @LoginId, @Password, @Province, @State, @Type, @BalanceAmount, @CreatedBy)", record);

                return true;
            }
        }

        public async Task<bool> Delete(int key)
        {
            var db = connection.Create();
            var rowsAffected = await db.ExecuteAsync("UPDATE bankaccount SET status = 3 WHERE bankaccountid = @BankAccountId", new { BankAccountId = key });
            return (rowsAffected > 0);
        }

        // This is for Settlement API Section

        public List<BankAccount> SelectBankAccountNo(int key)
        {
            List<BankAccount> result = new List<BankAccount>();

            var db = connection.Create();
            result = db.Query<BankAccount>("SELECT bankaccountid, accountno FROM bankaccount WHERE statusid = 1 and bankid = @key", new { key }).ToList();
            return result;
        }

        public List<BankAccount> SelectNickName(int key)
        {
            List<BankAccount> result = new List<BankAccount>();

            var db = connection.Create();
            result = db.Query<BankAccount>("SELECT name, nickname FROM bankaccount WHERE bankaccountid = @key", new { key }).ToList();
            return result;
        }
    }
}