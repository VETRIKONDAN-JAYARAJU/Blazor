using DataObjects;
using Dapper;

namespace DataServices
{
    public class UserMenuService
    {
        private readonly DBConnection connection;

        public UserMenuService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public async Task<UserMenu> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<UserMenu>("SELECT * FROM usermenu WHERE userid = @key", new { key });
            return result.First();
        }

        public async Task<UserMenu> SelectByRoleId(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<UserMenu>("SELECT * FROM usermenu WHERE roleid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(UserMenu record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM usermenu WHERE roleid = @RoleId ", new { record.RoleId }).FirstOrDefault();
            if (isExist == true)
            {
                await db.ExecuteAsync(@"UPDATE usermenu SET menuids=@MenuIds, updatedon=@UpdatedOn, updatedby=@CreatedBy WHERE roleid=@RoleId", record);
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO usermenu (roleid, menuids, createdby) VALUES (@RoleId, @MenuIds, @CreatedBy)", record);
                return true;
            }
        }
    }
}
