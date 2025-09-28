using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using minimalApi.Domain.DTOs;
using minimalApi.Domain.Entities;

namespace minimalApi.Domain.Interfaces;

    public interface IVeiculoService
    {
        Veiculo? GetVeiculoById(int id);
        List<Veiculo> GetAllVeiculos(int pageNumber = 1, string? nome = null, string? marca = null);
        void AddVeiculo(Veiculo veiculo);
        void UpdateVeiculo(Veiculo veiculo);
        void DeleteVeiculo(Veiculo veiculo);
    }
