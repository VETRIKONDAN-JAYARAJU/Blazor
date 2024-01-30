using DataObjects;
using Dapper;

namespace DataServices
{
    public class UserService : IGenericRepository<User, int>
    {
        List<User> listUser = new List<User>();

        private readonly DBConnection connection;

        public UserService(DBConnection dbconnection)
        {
            connection = dbconnection;
        }

        // --------------------------------------------------------------------------


        // This Section for Authentication

        public List<User> SelectActiveUsersList()
        {
            var db = connection.Create();
            {
                listUser = db.Query<User>("SELECT userid, name, email, password, users.roleid, role.rolename FROM users, role WHERE users.roleid = role.roleid AND users.statusid = 1 ORDER BY userid").ToList();
                return listUser;
            }
        }

        public User? GetbyUsername(string email)
        {
            SelectActiveUsersList();
            return listUser.FirstOrDefault(x => x.Email == email);
        }

        public List<User> SelectEmailRole(int key)
        {
            List<User> result = new List<User>();
            var db = connection.Create();
            result = db.Query<User>("SELECT userid, name, email, role.rolename FROM users, role WHERE users.roleid = role.roleid AND userid = @key", new { key }).ToList();
            return result;
        }


        // --------------------------------------------------------------------------

        public async Task<List<User>> SelectAll()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<User>("SELECT userid, name, email, password, role.rolename, recordstatus.status FROM users, role, recordstatus WHERE users.roleid = role.roleid AND users.statusid = recordstatus.statusid ORDER BY userid");
            return result.ToList();
        }

        public async Task<List<User>> SelectActive()
        {
            var db = connection.Create();
            var result = await db.QueryAsync<User>("SELECT userid, name, email FROM users WHERE statusid = 1 ORDER BY userid");
            return result.ToList();
        }

        public async Task<User> SelectById(int key)
        {
            var db = connection.Create();
            var result = await db.QueryAsync<User>("SELECT * FROM users WHERE userid = @key", new { key });
            return result.First();
        }

        public async Task<bool> Insert(User record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM users WHERE email = Trim(@Email)", new { record.Email }).FirstOrDefault();
            if (isExist == true)
            {
                return false;
            }
            else
            {
                await db.ExecuteAsync(@"INSERT INTO users (name,email,password,roleid, createdby) VALUES (@Name, @Email, @Password, @RoleId, @CreatedBy)", record);
                return true;
            }
        }

        public async Task<bool> Update(User record)
        {
            var db = connection.Create();
            var isExist = db.Query<bool>("SELECT count(*) FROM users WHERE email = Trim(@Email)", new { record.Email }).FirstOrDefault();

            if (isExist == true)
            {
                isExist = db.Query<bool>("SELECT count(*) FROM users WHERE email = Trim(@Email) AND statusid=@StatusId", new { record.Email, record.StatusId }).FirstOrDefault();
                if (isExist == true)
                {
                    return false;
                }
                else
                {
                    await db.ExecuteAsync(@"UPDATE users SET statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE userid = @UserId", record);
                    return true;
                }
            }
            else
            {
                await db.ExecuteAsync(@"UPDATE users SET name=@Name, email=@Email, password=@Password, roleid=@RoleId, statusid=@StatusId, updatedon=@UpdatedOn, updatedby=@UpdatedBy WHERE userid = @UserId", record);
                return true;
            }
        }

        public async Task<bool> Delete(int key)
        {
            var db = connection.Create();
            var rowsAffected = await db.ExecuteAsync("UPDATE users SET statusid = 3 WHERE userid = @UserId", new { UserId = key });
            return (rowsAffected > 0);
        }


        public List<User> ReportUsers()
        {
            List<User> result = new List<User>();

            string query = @"SELECT users.userid, users.name, users.email, users.password, role.rolename, merchant.merchantname, recordstatus.status FROM users
					 JOIN role ON users.roleid = role.roleid LEFT JOIN merchant ON users.merchantid = merchant.merchantid
					 JOIN recordstatus ON users.statusid = recordstatus.statusid ORDER BY users.userid";

            var db = connection.Create();
            result = db.Query<User>(query).ToList();
            return result;
        }

    }
}