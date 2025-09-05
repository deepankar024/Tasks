using FeedbackFormWebApp.Models;
using System.Web;

namespace FeedbackFormWebApp.Utils
{
    public static class AuthHelper
    {
        private const string UserSessionKey = "CurrentUser";

        public static User GetCurrentUser()
        {
            if (HttpContext.Current?.Session != null)
            {
                return HttpContext.Current.Session[UserSessionKey] as User;
            }
            return null;
        }

        public static void SetCurrentUser(User user)
        {
            if (HttpContext.Current?.Session != null)
            {
                HttpContext.Current.Session[UserSessionKey] = user;
            }
        }

        public static void ClearCurrentUser()
        {
            if (HttpContext.Current?.Session != null)
            {
                HttpContext.Current.Session.Remove(UserSessionKey);
            }
        }

        public static bool IsLoggedIn()
        {
            return GetCurrentUser() != null;
        }

        public static bool IsAdmin()
        {
            var user = GetCurrentUser();
            return user != null && user.Role == UserRoles.Admin;
        }

        public static bool IsUser()
        {
            var user = GetCurrentUser();
            return user != null && user.Role == UserRoles.User;
        }

        public static void RedirectToLogin()
        {
            if (HttpContext.Current?.Response != null)
            {
                HttpContext.Current.Response.Redirect("~/Login.aspx");
            }
        }

        public static void RedirectIfNotLoggedIn()
        {
            if (!IsLoggedIn())
            {
                RedirectToLogin();
            }
        }

        public static void RedirectIfNotAdmin()
        {
            if (!IsAdmin())
            {
                if (IsLoggedIn())
                {
                    HttpContext.Current.Response.Redirect("~/WebForm1.aspx");
                }
                else
                {
                    RedirectToLogin();
                }
            }
        }
    }
}
