using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimalApi.Domain.Entities
{
    public class Administrador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; } = default!;

        [Required]
        [StringLength(50)]
        public string Senha { get; set; } = default!;

        [StringLength(10)]
        public string Perfil { get; set; } = default!;
    }
}