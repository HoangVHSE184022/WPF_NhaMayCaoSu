namespace WPF_NhaMayCaoSu.Repository.Models
{
    public class Role
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public ICollection<Account> Accounts { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
