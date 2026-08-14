namespace IntCore.DTOs
{
    public class CreateUserDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string? Email { get; set; }
        public int? TenantId { get; set; }

        public string Password { get; set; } = default!;

        public string[] Roles { get; set; } = Array.Empty<string>();
    }
}
