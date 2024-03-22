using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllTPlayer_App.Models;

namespace AllTPlayer_App.Data
{
    public class UsersDB
    {
        readonly SQLiteAsyncConnection db;

        public UsersDB(string connection)
        {
            db = new SQLiteAsyncConnection(connection);

            db.CreateTableAsync<User>().Wait();
        }

        public Task<List<User>> GetUsersAsync()
        {
            return db.Table<User>().ToListAsync();
        }

        public Task<int> DeleteUsersAsync()
        {
            return db.DeleteAllAsync<User>();
        }

        public Task<int> SaveUserAsync(User user)
        {
            if (user.Id != 0)
            {
                return db.UpdateAsync(user);
            }
            else
            {
                return db.InsertAsync(user);
            }
        }



        // Future features ------------------------------------
        public Task<User> GetUserAsync(int id)
        {
            return db.Table<User>().Where(i=> i.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> DeleteUserAsync(User user) 
        {
            return db.DeleteAsync(user);
        }


    }
}
