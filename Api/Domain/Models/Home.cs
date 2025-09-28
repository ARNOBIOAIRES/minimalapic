using System;

namespace minimalApi.Domain.Models
{
    public struct Home 
    {
        public string Documentation { get => "Bem-vindo à API de Veiculos"; }
        public string Swagger { get => "/swagger"; }
    }
}