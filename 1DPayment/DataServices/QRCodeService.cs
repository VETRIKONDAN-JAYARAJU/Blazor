using DataObjects;
using Dapper;

namespace DataServices
{
    public class QRCodeService : IGenericRepository<QRCode, int>
    {
        private readonly DBConnection connection;

        public QRCodeService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<QRCode>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<QRCode>("SELECT qrcodeid, qrcodename, filename, filepath, recordstatus.status FROM qrcode, recordstatus WHERE qrcode.statusid = recordstatus.statusid ORDER BY qrcodeid");
            return result.ToList();
        }

        public async Task<List<QRCode>> SelectActive()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<QRCode>("SELECT qrcodeid, qrcodename FROM qrcode WHERE statusid = 1 ORDER BY qrcodeid");
            return result.ToList();
        }

        public async Task<QRCode> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<QRCode>("SELECT * FROM qrcode WHERE qrcodeid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(QRCode record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM qrcode WHERE qrcodename = Trim(@QRCodeName)", new { record.QRCodeName }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO qrcode (qrcodename, filename, filepath, createdby) VALUES (@QRCodeName, @FileName, @FilePath, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> Update(QRCode record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM qrcode WHERE qrcodename = Trim(@QRCodeName)", new { record.QRCodeName }).FirstOrDefault();

            if (isExist == true)
            {
                isExist = db.Query<bool>("SELECT count(*) FROM qrcode WHERE qrcodename = Trim(@QRCodeName) AND statusid=@StatusId", new { record.QRCodeName, record.StatusId }).FirstOrDefault();
                if (isExist == true)
                {
                    return false;
                }
                else
                {
                    await db.ExecuteAsync(@"UPDATE qrcode SET statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE qrcodeid = @QRCodeId", record);
                    return true;
                }
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE qrcode SET qrcodename=@QRCodeName, filename=@FileName, filepath=@FilePath, statusid=@StatusId,  updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE qrcodeid = @QRCodeId", record);
                return true;
            }
        }

        public async Task<bool> Delete(int key)
        {
            var db = connection.Create();
            var rowsAffected = await db.ExecuteAsync("UPDATE qrcode SET statusid = 3 WHERE qrcodeid = @QRCodeId", new { QRCodeId = key });
            return (rowsAffected > 0);
        }
    }
}