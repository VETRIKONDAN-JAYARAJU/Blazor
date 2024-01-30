using DataObjects;
using Dapper;

namespace DataServices
{
    public class MenuService
    {
        private readonly DBConnection connection;

        public MenuService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        public List<Menu> GetMenus(int[] menuIds)
        {
            var db = connection.Create();
            string sql = "SELECT * FROM menu WHERE menuid IN @menuIds";
            var result = db.Query<Menu>(sql, new { menuIds }).ToList();
            return result;
        }


        public async Task<List<Menu>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Menu>("SELECT * FROM menu ORDER BY menuid");
            return result.ToList();
        }

        public async Task<List<Menu>> SelectActive()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Menu>("SELECT * FROM menu WHERE menustatus = true ORDER BY menuid");
            return result.ToList();
        }

        public async Task<Menu> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<Menu>("SELECT * FROM Menu WHERE menuid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(Menu record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM menu WHERE menuname = Trim(@MenuName) or pagename = Trim(@PageName) ", new { record.MenuName, record.PageName }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO menu (menuname, pagename, iconname, createdby) VALUES (@MenuName, @PageName, @IconName, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> Update(Menu record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM menu WHERE menuname = Trim(@MenuName)", new { record.MenuName }).FirstOrDefault();

            if (isExist == true)
            {
                isExist = db.Query<bool>("SELECT count(*) FROM menu WHERE menuname = Trim(@MenuName) AND menustatus=@MenuStatus", new { record.MenuName, record.MenuStatus }).FirstOrDefault(); 
                if (isExist == true)
                {
                    return false;
                }
                else
                {
                    await db.ExecuteAsync(@"UPDATE menu SET menustatus = @MenuStatus, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE menuid = @MenuId", record);
                    return true;
                }
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE menu SET menuname=@MenuName, pagename=@PageName, iconname=@IconName, menustatus=@MenuStatus, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE menuid = @MenuId", record);
                return true;
            }
        }

        public async Task<bool> Delete(int key)
        {
            var db = connection.Create();
            var rowsAffected = await db.ExecuteAsync("UPDATE menu SET menustatus = false WHERE menuid = @MenuId", new { MenuId = key });
            return (rowsAffected > 0);
        }
    }
}