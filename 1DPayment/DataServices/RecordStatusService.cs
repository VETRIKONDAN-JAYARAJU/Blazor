using DataObjects;
using Dapper;

namespace DataServices
{
    public class RecordStatusService : IGenericRepository<RecordStatus, int>
    {
        private readonly DBConnection connection;

        public RecordStatusService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<RecordStatus>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<RecordStatus>("SELECT * FROM recordstatus ORDER BY statusid");
            return result.ToList();
        }

        public async Task<RecordStatus> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<RecordStatus>("SELECT * FROM recordstatus WHERE statusid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(RecordStatus record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM recordstatus WHERE status = Trim(@Status)", new { record.Status }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO recordstatus (status, createdby) VALUES (@Status, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> Update(RecordStatus record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM recordstatus WHERE status = Trim(@Status)", new { record.Status }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE recordstatus SET status=@Status WHERE statusid = @StatusId", record);
                return true;
            }
        }

        public Task<List<RecordStatus>> SelectActive()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int key)
        {
            throw new NotImplementedException();
        }
    }
}
