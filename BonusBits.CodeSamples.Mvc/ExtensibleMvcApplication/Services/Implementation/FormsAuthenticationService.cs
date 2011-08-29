using System;
using System.Web;
using System.Web.Security;

namespace ExtensibleMvcApplication.Services.Implementation
{
    internal sealed class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(String userName, Boolean createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            }

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);

            if (createPersistentCookie)
            {
                var timeout = TimeSpan.FromHours(2);

                var ticket = new FormsAuthenticationTicket(
                    userName,
                    true,
                    Convert.ToInt32(timeout.TotalSeconds)
                    );

                var cookie = new HttpCookie(
                    FormsAuthentication.FormsCookieName,
                    FormsAuthentication.Encrypt(ticket)
                    );

                cookie.Expires = DateTime.Now.Add(timeout);

                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}