using System.ComponentModel.DataAnnotations;

namespace SimpleCrudFrontend.Models
{
    public class Student
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public int StudentId { get; set; }
        public string WebId { get; set; }
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        [StringLength(7)]
        public string StudentSecurityNumber { get; set; }
        [Required]
        public DateTime? TimeIn { get; set; } = DateTime.Now;

    }
}
