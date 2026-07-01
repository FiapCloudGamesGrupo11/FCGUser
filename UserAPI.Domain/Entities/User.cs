using UserAPI.Domain.Enums;

namespace UserAPI.Domain.Entities
{
    public class User : BaseEntity
    {
        public User() { }
        public User(string name, string lastName, string email, string password, Role role = Role.Common)
        {
            Id = Guid.NewGuid();
            Name = name;
            LastName = lastName;
            Email = email;
            Password = password;
            Role = role;
            Status = Status.Active;
        }

        public void ActiveUser() => Status = Status.Active;
        public void BlockUser() => Status = Status.Blocked;
        public void BanUser() => Status = Status.Banned;
        public void ChangeRoleToAdmin() => Role = Role.Admin;
        public void ChangeRoleToCommon() => Role = Role.Common;

        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; private set; }
        public Status Status { get; private set; }
    }
}
