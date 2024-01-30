using DataObjects;
using Dapper;

namespace DataServices
{
    public class PaymentStatusService : IGenericRepository<PaymentStatus, int>
    {
        private readonly DBConnection connection;

        public PaymentStatusService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<PaymentStatus>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<PaymentStatus>("SELECT * FROM paymentstatus ORDER BY statusid");
            return result.ToList();
        }

        public async Task<PaymentStatus> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<PaymentStatus>("SELECT * FROM paymentstatus WHERE statusid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(PaymentStatus record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM paymentstatus WHERE status = Trim(@Status)", new { record.Status }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO paymentstatus (status, createdby) VALUES (Trim(@Status),@CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> Update(PaymentStatus record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM paymentstatus WHERE status = Trim(@Status)", new { record.Status }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE paymentstatus SET status=@Status WHERE statusid = @StatusId", record);
                return true;
            }
        }

        public Task<List<PaymentStatus>> SelectActive()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int key)
        {
            throw new NotImplementedException();
        }
    }
}
