using System;
using System.Web;

namespace DevCookie
{
    public interface IDevAccessChecker
    {
        bool UserHasDevAccess();
    }

    public class DevAccessChecker : IDevAccessChecker
    {
        // todo: setup option to allow these to be changed?
        internal const string CookieName = "devaccess";
        internal const string QueryStringName = "devaccess";

        private readonly HttpRequestBase _request;

        public DevAccessChecker(HttpRequestBase request)
        {
            _request = request;
        }

        public bool UserHasDevAccess()
        {
            if (DevAccessModule.SecretToken == null)
                throw new InvalidOperationException("DevCookie not setup. Please call DevCookieAccess.Setup() on startup.");
            // todo: make this exception text less magic string.

            return CookieIsValid(_request);
        }

        private static bool CookieIsValid(HttpRequestBase request)
        {
            var cookie = request.Cookies[CookieName];
            return cookie != null && cookie.Value == DevAccessModule.SecretToken;
        }

        internal static bool QueryStringIsValidAndCookieCreated(HttpRequestBase request, HttpResponseBase response)
        {
            var authToken = request.QueryString[QueryStringName];
            if (authToken != DevAccessModule.SecretToken)
                return false;

            var authCookie = new HttpCookie(CookieName, authToken) { Expires = DateTime.UtcNow.AddDays(DevAccessModule.CookieExpiryInDays) };
            if (request.IsSecureConnection)
                authCookie.Secure = true;       // todo: an integration test for this would be great. it's kinda important
            response.Cookies.Add(authCookie);
            return true;
        }
    }
}