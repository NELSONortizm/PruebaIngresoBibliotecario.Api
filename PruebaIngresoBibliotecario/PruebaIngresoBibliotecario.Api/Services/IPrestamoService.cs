using PruebaIngresoBibliotecario.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PruebaIngresoBibliotecario.Api.Services
{
    public interface IPrestamoService
    {
        Task<Prestamo> CrearPrestamoAsync(Prestamo prestamo);
        Task<Prestamo?> ObtenerPrestamoPorIdAsync(Guid id);
        Task<bool> UsuarioTienePrestamoAsync(string identificacionUsuario);
        Task<List<Prestamo>> ObtenerTodosPrestamosAsync();
        Task<Prestamo> ActualizarPrestamoAsync(Guid id, Prestamo prestamo);
        Task<bool> EliminarPrestamoAsync(Guid id);
    }//
}
