using FeedbackFormWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FeedbackFormWebApp.Services
{
    public class UserRepository : IUserRepository
    {
        public User GetById(int id)
        {
            using (var db = new FeedbackDbContext())
            {
                return db.Users.FirstOrDefault(u => u.Id == id);
            }
        }

        public User GetByEmail(string email)
        {
            using (var db = new FeedbackDbContext())
            {
                return db.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            }
        }

        public void Add(User user)
        {
            using (var db = new FeedbackDbContext())
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        public void Update(User user)
        {
            using (var db = new FeedbackDbContext())
            {
                var existing = db.Users.Find(user.Id);
                if (existing != null)
                {
                    existing.FullName = user.FullName;
                    existing.Email = user.Email;
                    existing.Role = user.Role;
                    existing.LastLogin = user.LastLogin;
                    // Don't update password hash unless explicitly changed
                    db.SaveChanges();
                }
            }
        }

        public bool EmailExists(string email)
        {
            using (var db = new FeedbackDbContext())
            {
                return db.Users.Any(u => u.Email.ToLower() == email.ToLower());
            }
        }

        public List<User> GetAll()
        {
            using (var db = new FeedbackDbContext())
            {
                return db.Users.OrderBy(u => u.FullName).ToList();
            }
        }

        public User ValidateUser(string email, string password)
        {
            var user = GetByEmail(email);
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                // Update last login
                var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                user.LastLogin = DateTime.SpecifyKind(
                    TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone),
                    DateTimeKind.Unspecified);
                Update(user);
                return user;
            }
            return null;
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            string hashOfInput = HashPassword(password);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hash) == 0;
        }
    }
}
