using System.Collections.Generic;
using System.Linq;

namespace Demo.WebApi.Patch.API.Services
{
    public static class UserService
    {
        private static List<Models.User> Users { get; }

        static UserService()
        {
            Users = new List<Models.User>
            {
                new Models.User { Id = 1, Name = "Test User 1", Email = "test1@email.com", Phone = "214-000-0000" },
                new Models.User { Id = 2, Name = "Test User 2", Email = "test2@email.com", Phone = "972-000-0000" },
            };
        }

        public static List<Models.User> GetAll()
        {
            return Users;
        }

        public static Models.User? GetById(int id)
        {
            return Users.FirstOrDefault(u => u.Id == id);
        }

        public static void Add(Models.User newUser)
        {
            var currentMaxId = Users.OrderByDescending(u => u.Id)?.FirstOrDefault()?.Id ?? 0;
            newUser.Id = currentMaxId + 1;

            Users.Add(newUser);
        }

        public static void Delete(int id)
        {
            var user = GetById(id);

            if (user is not null)
            {
                Users.Remove(user);
            }
        }

        public static void Update(Models.User updatedUser)
        {
            var index = Users.FindIndex(u => u.Id == updatedUser.Id);

            if (index != -1)
            {
                Users[index] = updatedUser;
            }
        }
    }
}
