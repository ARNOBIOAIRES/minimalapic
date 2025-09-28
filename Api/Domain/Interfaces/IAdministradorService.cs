using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimalApi.Domain.DTOs;
using minimalApi.Domain.Entities;

namespace minimalApi.Domain.Interfaces;

public interface IAdministradorService
{
  Administrador? Login(LoginDTO loginDTO);

  Administrador? Incluir(Administrador administrador);

   Administrador? GetById(int id);
  
  List<Administrador> GetAll(int? page);
  }
