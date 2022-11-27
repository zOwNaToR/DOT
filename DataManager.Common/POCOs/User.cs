using Microsoft.AspNetCore.Identity;

namespace DataManager.Common.POCOs
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; }
        public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }
    }
}