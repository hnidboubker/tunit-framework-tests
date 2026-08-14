namespace IntApplication.DTOs
{
    public class EditUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? Email { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public int? TenantId { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string[] Roles { get; set; } = Array.Empty<string>();
    }
}
