using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;

namespace DevCookie
{
    public class DevAccessModule : Module
    {
        public static string SecretToken = null;
        public static int CookieExpiryInDays = 1;

        private readonly bool _useAsBlanketAuthFilter;

        // todo: add overload for specifying appSetting key that holds the token
        public DevAccessModule(string secretToken, int cookieExpiryInDays = 1, bool useAsBlanketAuthFilter = false)
        {
            SecretToken = secretToken;
            CookieExpiryInDays = cookieExpiryInDays;
            _useAsBlanketAuthFilter = useAsBlanketAuthFilter;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DevAccessChecker>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterFilterProvider();   // todo: end-to-end test! this line is required

            if (_useAsBlanketAuthFilter)
            {
                builder.Register(c => new DevCookieAuthorizeAttribute())
                    .AsAuthorizationFilterFor<Controller>()
                    .PropertiesAutowired()
                    .InstancePerLifetimeScope();

                // todo: add wiring for webapi controllers?
            }
        }
    }
}