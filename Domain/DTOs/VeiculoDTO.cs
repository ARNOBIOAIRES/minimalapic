using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimalApi.Domain.DTOs
{
    public record VeiculoDTO
    {
        public string Nome { get; init; }
        public string Marca { get; init; }
        public string Modelo { get; init; }
        public int Ano { get; init; }
    }
}