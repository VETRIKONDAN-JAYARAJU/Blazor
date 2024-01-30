using DataObjects;
using Dapper;

namespace DataServices
{
    public class PaymentMethodService :IGenericRepository<PaymentMethod, int>
    {
        private readonly DBConnection connection;

        public PaymentMethodService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<PaymentMethod>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<PaymentMethod>("SELECT paymenttypeid, paymenttype, recordstatus.status FROM paymenttype, recordstatus WHERE paymenttype.statusid = recordstatus.statusid ORDER BY paymenttypeid");
            return result.ToList();
        }

        public async Task<List<PaymentMethod>> SelectActive()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<PaymentMethod>("SELECT paymenttypeid, paymenttype FROM paymenttype WHERE statusid = 1 ORDER BY paymenttypeid");
            return result.ToList();
        }

        public async Task<PaymentMethod> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<PaymentMethod>("SELECT * FROM paymenttype WHERE paymenttypeid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(PaymentMethod record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM paymenttype WHERE paymenttype = Trim(@PaymentType)", new { record.PaymentType }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO paymenttype (paymenttype, createdby) VALUES (@PaymentType, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> Update(PaymentMethod record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM paymenttype WHERE paymenttype = Trim(@PaymentType)", new { record.PaymentType }).FirstOrDefault();

            if (isExist == true)
            {
                isExist = db.Query<bool>("SELECT count(*) FROM paymenttype WHERE paymenttype = Trim(@PaymentType) AND statusid=@StatusId", new { record.PaymentType, record.StatusId }).FirstOrDefault();
                if (isExist == true)
                {
                    return false;
                }
                else
                {
                    await db.ExecuteAsync(@"UPDATE paymenttype SET statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE paymenttypeid = @PaymentTypeId", record);
                    return true;
                }
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE paymenttype SET paymenttype=@PaymentType, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE paymenttypeid = @PaymentTypeId", record);
                return true;
            }
        }

        public async Task<bool> Delete(int key)
        {
            var db = connection.Create();
            var rowsAffected = await db.ExecuteAsync("UPDATE paymenttype SET statusid = 3 WHERE paymenttypeid = @PaymentTypeId", new { PaymentTypeId = key });
            return (rowsAffected > 0);
        }

        // For Payout API
        public async Task<List<PaymentMethod>> SelectIB()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<PaymentMethod>("SELECT paymenttypeid, paymenttype FROM paymenttype WHERE paymenttypeid = 1");
            return result.ToList();
        }
    }
}