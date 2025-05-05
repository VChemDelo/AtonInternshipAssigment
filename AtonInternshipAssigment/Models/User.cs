using System.ComponentModel.DataAnnotations;

namespace AtonInternshipAssigment.Models
{
    public class User
    {
        public Guid Guid { get; set; } = Guid.NewGuid();

        [Required]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Логин должен содержать только латинские буквы и цифры.")]
        public string Login { get; set; } = null!;

        [Required]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Пароль должен содержать только латинские буквы и цифры.")]
        public string Password { get; set; } = null!;

        [Required]
        [RegularExpression("^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Имя должно содержать только латинские или русские буквы.")]
        public string Name { get; set; } = null!;

        [Range(0, 2, ErrorMessage = "Пол должен быть 0, 1 или 2.")]
        public int Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public bool Admin { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? RevokedOn { get; set; }

        public string RevokedBy { get; set; }
    }
}
