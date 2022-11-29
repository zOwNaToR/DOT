using DataManager.Common.Abstractions;

namespace DataManager.Common.POCOs
{
    public class RefreshToken : IEntity
    {
        public Guid Id { get; set; }
        public string JwtId { get; set; } = "";
        public bool Used { get; set; }
        public bool Invalidated { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime ExpireDate { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
