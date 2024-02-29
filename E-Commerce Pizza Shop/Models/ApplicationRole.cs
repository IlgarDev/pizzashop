using AspNetCore.Identity.MongoDbCore.Models;

namespace PizzaApp.Models
{
    public class ApplicationRole : MongoIdentityRole
    {
        public ApplicationRole() : base()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
        }
    }
}
