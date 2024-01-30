using DataObjects;
using Dapper;

namespace DataServices
{
    public class DeviceService : IGenericRepository<Device, int>
    {
        private readonly DBConnection connection;

        public DeviceService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<List<Device>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Device>("SELECT deviceid, devicename, uuid, puid, devicekey, devicetype, recordstatus.status FROM device, recordstatus WHERE device.statusid = recordstatus.statusid ORDER BY deviceid");
            return result.ToList();
        }

        public async Task<List<Device>> SelectActive()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Device>("SELECT deviceid, devicename, devicekey, devicetype FROM device WHERE statusid = 1 ORDER BY deviceid");
            return result.ToList();
        }

        public async Task<Device> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Device>("SELECT * FROM device WHERE deviceid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(Device record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM device WHERE devicename = Trim(@DeviceName) or uuid = Trim(@UUID) or puid = Trim(@PUID) or devicekey = Trim(@DeviceKey) ", new { record.DeviceName, record.UUID, record.PUID, record.DeviceKey }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO device (devicename, uuid, puid, devicekey, devicetype, createdby) VALUES (@DeviceName, @UUID, @PUID, @DeviceKey, @DeviceType, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> Update(Device record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM device WHERE devicename = Trim(@DeviceName) AND uuid = Trim(@UUID) AND puid = Trim(@PUID) AND devicekey = Trim(@DeviceKey) AND devicetype = @DeviceType ", new { record.DeviceName, record.UUID, record.PUID, record.DeviceKey, record.DeviceType }).FirstOrDefault();

            if (isExist == true)
            {
                // isExist = db.Query<bool>("SELECT count(*) FROM device WHERE devicename = Trim(@DeviceName) AND statusid=@StatusId", new { record.DeviceName, record.StatusId }).FirstOrDefault();
                isExist = db.Query<bool>("SELECT count(*) FROM device WHERE devicename = Trim(@DeviceName) AND uuid = Trim(@UUID) AND puid = Trim(@PUID) AND devicekey = Trim(@DeviceKey) AND devicetype = @DeviceType AND statusid=@StatusId", new { record.DeviceName, record.UUID, record.PUID, record.DeviceKey, record.DeviceType, record.StatusId }).FirstOrDefault();

                if (isExist == true)
                {
                    return false;
                }
                else
                {
                    await db.ExecuteAsync(@"UPDATE device SET statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE deviceid = @DeviceId", record);
                    return true;
                }
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE device SET devicename=@DeviceName, uuid=@UUID, puid=@PUID, devicekey=@DeviceKey, devicetype=@DeviceType, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE deviceid = @DeviceId", record);

                // This Line is for Audit Log
                await db.ExecuteAsync(@"INSERT INTO device_auditlog (deviceid, devicename, uuid, puid, devicekey, devicetype, createdby) VALUES (@DeviceId, @DeviceName, @UUID, @PUID, @DeviceKey, @DeviceType, @CreatedBy)", record);

                return true;
            }
        }

        public async Task<bool> Delete(int key)
        {
            var db = connection.Create();
            var rowsAffected = await db.ExecuteAsync("UPDATE device SET statusid = 3 WHERE deviceid = @DeviceId", new { DeviceId = key });
            return (rowsAffected > 0);
        }
    }
}