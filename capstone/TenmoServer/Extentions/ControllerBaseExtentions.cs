using Microsoft.AspNetCore.Mvc;

namespace TenmoServer.Extentions
{
    public static class ControllerBaseExtentions
    {
        public static bool CurrentUserIdEquals(this ControllerBase controller, int userId)
        {
            return controller.User.FindFirst("sub").Value == userId.ToString();
        }
    }
}
