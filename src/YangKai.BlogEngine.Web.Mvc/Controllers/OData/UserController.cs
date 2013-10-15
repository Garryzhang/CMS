using System;
using System.Web.Http;
using System.Web.Http.OData;
using YangKai.BlogEngine.Domain;
using YangKai.BlogEngine.Service;

namespace YangKai.BlogEngine.Web.Mvc.Controllers.OData
{
    public class UserController : EntityController<User>
    {
        [HttpPost]
        public void Signin([FromODataUri] int key, ODataActionParameters parameters)
        {
            var username = (string)parameters["Username"];
            var password = (string)parameters["Password"];
            var isRemember = (bool)parameters["IsRemember"];

            var login = Proxy.Repository<User>().Exist(p => p.LoginName == username && p.Password == password);
            if (login)
            {
                var data = Proxy.Repository<User>().Get(p => p.LoginName == username);
                Current.User = new WebUser
                {
                    UserName = data.UserName,
                    LoginName = data.LoginName,
                    Password = data.Password,
                    Email = data.Email,
                    IsAdmin = true,
                    IsRemember = isRemember
                };
            }
            else
            {
                throw new Exception("Username or password error.");
            }
        }

        [HttpPost]
        public void Signout([FromODataUri] int key, ODataActionParameters parameters)
        {
            Current.User = null;
        }
    }
}