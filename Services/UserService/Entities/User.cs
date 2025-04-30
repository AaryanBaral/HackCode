namespace UserService.Entities
{
    public class User
    {
        public Guid Id {get; set;}
        public string UserName {get; set;}=string.Empty;
        public string Email {get; set;} =string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role {get; set;}
        public DateTime CreatedOn{get; set;} =DateTime.Now;
    }
     public enum UserRole
    {
        Admin,
        Moderator,
        User,
        Guest
    }
}