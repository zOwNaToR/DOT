using Microsoft.AspNetCore.Identity;

namespace DataManager.Common.POCOs
{
    public class Role : IdentityRole<Guid>
    {
        public virtual ICollection<IdentityRoleClaim<Guid>> RoleClaims { get; set; }
        public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }
    }
}
