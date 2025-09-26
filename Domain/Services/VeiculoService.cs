using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using minimalApi.Domain.DTOs;
using minimalApi.Domain.Entities;
using minimalApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using minimalApi.Infraestrutura.Database;



namespace minimalApi.Domain.Services

{
    public class VeiculoService : IVeiculoService
    {
        private readonly AppDbContext _contexto;

        public VeiculoService(AppDbContext contexto)
        {
            _contexto = contexto;

        }

        public void AddVeiculo(Veiculo veiculo)
        {
            _contexto.Veiculos.Add(veiculo);
            _contexto.SaveChanges();
        }

        public void DeleteVeiculo(Veiculo veiculo)
        {
            _contexto.Veiculos.Remove(veiculo);
            _contexto.SaveChanges();
        }

        public List<Veiculo> GetAllVeiculos(int pageNumber = 1, string? nome = null, string? marca = null)
        {
            var query = _contexto.Veiculos.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(v => v.Nome.Contains(nome));
            }

            if (!string.IsNullOrEmpty(marca))
            {
                query = query.Where(v => v.Marca.Contains(marca));
            }

            return query.Skip((pageNumber - 1) * 10).Take(10).ToList();
        }

        public Veiculo? GetVeiculoById(int id)
        {
            return _contexto.Veiculos.Find(id);
        }

        public void UpdateVeiculo(Veiculo veiculo)
        {
            _contexto.Veiculos.Update(veiculo);
            _contexto.SaveChanges();
        }
    }
}