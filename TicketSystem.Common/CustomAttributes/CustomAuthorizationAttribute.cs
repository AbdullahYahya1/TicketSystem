
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly UserType[] _UserTypes;
        public CustomAuthorizeAttribute(UserType[] UserTypes)
        {
            _UserTypes = UserTypes;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var user = context.HttpContext.User;
                var UserType = (UserType)Enum.Parse(typeof(UserType), user.FindFirst("UserType")?.Value, true);

                if (!_UserTypes.Contains(UserType))
                {
                    context.Result = new ForbidResult();
                }
            }
            catch (Exception Ex)
            {
                context.Result = new ForbidResult(Ex.Message);

            }

        }
    }
}
