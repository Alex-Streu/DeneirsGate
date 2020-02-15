namespace DeneirsGate.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class User
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [StringLength(50)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(500)]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Picture { get; set; }

        public DateTime LastLogin { get; set; }
    }
}
