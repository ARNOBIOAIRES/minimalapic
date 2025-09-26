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
    public class AdministradorService : IAdministradorService
    {
        private readonly AppDbContext _contexto;

        public AdministradorService(AppDbContext contexto)
        {
            _contexto = contexto;

        }

        public Administrador? Incluir (Administrador administrador)
        {
            _contexto.Administradores.Add(administrador);
            _contexto.SaveChanges();
            return administrador;
        }

        public List<Administrador> GetAll(int? page = 1)
       {
            var query = _contexto.Administradores.AsQueryable();

            int pageNumber = page ?? 1;

            return pageNumber > 0
                ? query.Skip((pageNumber - 1) * 10).Take(10).ToList()
                : query.ToList();
        }   

        public Administrador? Login(LoginDTO loginDTO)
        {
            var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha)
                .FirstOrDefault();
            return adm;
        }

        public Administrador? GetById(int id)
        {
            return _contexto.Administradores.Find(id);
        }
    }
}