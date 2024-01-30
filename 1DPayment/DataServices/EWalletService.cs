using DataObjects;
using Dapper;

namespace DataServices
{
    public class EWalletService
    {
        private readonly DBConnection connection;

        public EWalletService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<EWallet>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<EWallet>("SELECT ewalletid, ewalletno, ewalletname, qrcode.qrcodename, recordstatus.status FROM ewallet, qrcode, recordstatus WHERE ewallet.qrcodeid = qrcode.qrcodeid AND ewallet.statusid=recordstatus.statusid ORDER BY ewalletid");
            return result.ToList();
        }

        public async Task<List<EWallet>> SelectActive()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<EWallet>("SELECT ewalletid, ewalletno, ewalletname FROM ewallet WHERE statusid = 1 ORDER BY ewalletid");
            return result.ToList();
        }

        public async Task<EWallet> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<EWallet>("SELECT * FROM ewallet WHERE ewalletid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(EWallet record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM ewallet WHERE ewalletno = Trim(@EWalletNo) or ewalletname = Trim(@EWalletName)", new { record.EWalletNo, record.EWalletName }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO ewallet (ewalletno, ewalletname, qrcodeid, createdby) VALUES (@EWalletNo, @EWalletName, @QRCodeId, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> Update(EWallet record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM ewallet WHERE ewalletno = Trim(@EWalletNo) AND ewalletname = Trim(@EWalletName) AND qrcodeid=@QRCodeId", new { record.EWalletNo, record.EWalletName, record.QRCodeId }).FirstOrDefault(); 

            if (isExist == true)
            {
                isExist = db.Query<bool>("SELECT count(*) FROM ewallet WHERE ewalletno = Trim(@EWalletNo) AND ewalletname = Trim(@EWalletName) AND statusid=@StatusId", new { record.EWalletNo, record.EWalletName, record.StatusId }).FirstOrDefault();
                if (isExist == true)
                {
                    return false;
                }
                else
                {
                    await db.ExecuteAsync(@"UPDATE ewallet SET statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE ewalletid = @EWalletId", record);
                    return true;
                }
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE ewallet SET ewalletno=@EWalletNo, ewalletname=@EWalletName, qrcodeid=@QRCodeId, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE ewalletid = @EWalletId", record);

                // This Line is for Audit Log
                await db.ExecuteAsync(@"INSERT INTO ewallet_auditlog (ewalletid, ewalletno, ewalletname, qrcodeid, createdby) VALUES (@EWalletId, @EWalletNo, @EWalletName, @QRCodeId, @CreatedBy)", record);

                return true;
            }
        }


        public async Task<bool> Delete(int key)
        {
            var db = connection.Create();
            var rowsAffected = await db.ExecuteAsync("UPDATE ewallet SET statusid = 3 WHERE ewalletid = @EWalletId", new { EWalletId = key });
            return (rowsAffected > 0);
        }
    }
}