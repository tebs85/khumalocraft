using Microsoft.AspNetCore.Identity;

namespace KhumaloCraft.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
