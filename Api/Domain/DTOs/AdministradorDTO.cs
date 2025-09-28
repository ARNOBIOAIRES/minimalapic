using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimalApi.Domain.Enums;

namespace minimalApi.Domain.DTOs
{
    public class AdministradorDTO
    {
        public string Email { get; set; } = default!;
        public string Senha { get; set; } = default!;
        public Perfis? Perfil { get; set; } = default!;
    }
}