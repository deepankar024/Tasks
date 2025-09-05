using FeedbackFormWebApp.Models;
using System.Collections.Generic;

namespace FeedbackFormWebApp.Services
{
    public interface IUserRepository
    {
        User GetById(int id);
        User GetByEmail(string email);
        void Add(User user);
        void Update(User user);
        bool EmailExists(string email);
        List<User> GetAll();
        User ValidateUser(string email, string password);
    }
}
