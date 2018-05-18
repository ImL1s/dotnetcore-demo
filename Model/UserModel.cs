using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnetcore_demo.Attributes;

namespace dotnetcore_demo.Model
{
    public class UserModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [RegularExpression(@"\w+")]
        [Required]
        public string Name { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [DataType(DataType.Date)]
        [AgeCheck(18, 120)]
        public DateTime BirthDate { get; set; }
    }
}