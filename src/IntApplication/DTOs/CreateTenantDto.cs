namespace IntApplication.DTOs
{
    public class CreateTenantDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class EditTenantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CreateTenantWithUserAdminDto
    {
        public string TenantName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = string.Empty;
    }

    public class EditTenantWithUserAdminDto
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;

        public int? UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = string.Empty;
    }
}
