using Buran.Core.MvcLibrary.AdminPanel.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Buran.Core.Library.Utils;

namespace Buran.Core.MvcLibrary.AdminPanel.Controllers
{
    [Authorize]
    public class ResourceAuthController<T, Z, U> : EditController<T, Z>
         where T : class, new()
         where Z : class
         where U : class
    {
        private readonly IAuthorizationService _authorizationService;
        protected readonly UserManager<U> _userManager;

        public string AuthControllerName = string.Empty;

        public ResourceAuthController(bool popupEditor, Z context, IAuthorizationService authorizationService, UserManager<U> userManager)
            : base(popupEditor, context)
        {
            _authorizationService = authorizationService;
            _userManager = userManager;
        }


        protected bool CheckAuth(OperationAuthorizationRequirement req)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var checkModel = new AuthCheckModel
            {
                ResourceName = AuthControllerName.IsEmpty() ? GetType().Name : AuthControllerName,
                Roles = _userManager.GetRolesAsync(user).Result.ToList()
            };
            var authorizationResult = _authorizationService.AuthorizeAsync(User, checkModel, req).Result;
            return authorizationResult.Succeeded;
        }

        public override bool OnIndexAuthCheck()
        {
            return CheckAuth(Operations.Read);
        }

        public override bool OnShowAuthCheck()
        {
            return CheckAuth(Operations.Read);
        }

        public override bool OnCreateAuthCheck()
        {
            return CheckAuth(Operations.Insert);
        }

        public override bool OnEditAuthCheck()
        {
            return CheckAuth(Operations.Update);
        }

        public override bool OnDeleteAuthCheck()
        {
            return CheckAuth(Operations.Delete);
        }
    }
}
