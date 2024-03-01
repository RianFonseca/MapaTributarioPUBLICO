using System.ComponentModel.DataAnnotations;

namespace teste0.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Login é obrigatorio")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Senha é obrigatorio")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
    }
}
