using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Collections.Generic;

namespace Buran.Core.MvcLibrary.AdminPanel.Utils
{
    public class AuthCheckModel
    {
        public string ResourceName { get; set; }
        public List<string> Roles { get; set; }
    }

    public static class Operations
    {
        public static OperationAuthorizationRequirement Read = new OperationAuthorizationRequirement { Name = nameof(Read) };
        public static OperationAuthorizationRequirement Insert = new OperationAuthorizationRequirement { Name = nameof(Insert) };
        public static OperationAuthorizationRequirement Update = new OperationAuthorizationRequirement { Name = nameof(Update) };
        public static OperationAuthorizationRequirement Delete = new OperationAuthorizationRequirement { Name = nameof(Delete) };
    }
}
