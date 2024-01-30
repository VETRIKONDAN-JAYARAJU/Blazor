using DataObjects;
using Dapper;

namespace DataServices
{
    public class RoleService : IGenericRepository<Role, int>
    {
        private readonly DBConnection connection;

        public RoleService(DBConnection dbconnection)   
        {
            connection = dbconnection;
        }

        public async Task<List<Role>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Role>("SELECT roleid, rolename, recordstatus.status FROM role, recordstatus WHERE role.statusid = recordstatus.statusid ORDER BY roleid");
            return result.ToList();
        }

        public async Task<List<Role>> SelectActive()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Role>("SELECT roleid, rolename FROM role WHERE statusid = 1 ORDER BY roleid");
            return result.ToList();
        }

        public async Task<Role> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Role>("SELECT * FROM role WHERE roleid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(Role record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM role WHERE rolename = Trim(@RoleName)", new { record.RoleName }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO role (rolename, createdby) VALUES (@RoleName, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> Update(Role record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM role WHERE rolename = Trim(@RoleName)", new { record.RoleName }).FirstOrDefault();

            if (isExist == true)
            {
                isExist = db.Query<bool>("SELECT count(*) FROM role WHERE rolename = Trim(@RoleName) AND statusid=@StatusId", new { record.RoleName, record.StatusId }).FirstOrDefault();
                if (isExist == true)
                {
                    return false;
                }
                else
                {
                    await db.ExecuteAsync(@"UPDATE role SET statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE roleid = @RoleId", record);
                    return true;
                }
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE role SET rolename=@RoleName, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE roleid = @RoleId", record);
                return true;
            }
        }

        public async Task<bool> Delete(int key)
        {
            var db = connection.Create();
            var rowsAffected = await db.ExecuteAsync("UPDATE role SET statusid = 3 WHERE roleid = @RoleId", new { RoleId = key });
            return (rowsAffected > 0);
        }
    }
}
