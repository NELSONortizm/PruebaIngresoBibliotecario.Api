
using Microsoft.EntityFrameworkCore;
using PruebaIngresoBibliotecario.Api.Models;
using PruebaIngresoBibliotecario.Api;
using System.Threading.Tasks;
using System;
using PruebaIngresoBibliotecario.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace PruebaIngresoBibliotecario.Api.Services
{
    public class PrestamoService : IPrestamoService
    {
        private readonly PersistenceContext _context;

        //constructor
        public PrestamoService(PersistenceContext context)
        {
            _context = context;
        }
        public async Task<Prestamo> CrearPrestamoAsync(Prestamo prestamo)
        {


            // Verificar si el usuario invitado ya tiene un préstamo activo
            if (prestamo.TipoUsuario == 3 && await UsuarioTienePrestamoAsync(prestamo.IdentificacionUsuario))
            {
                throw new InvalidOperationException(
                    $"El usuario con identificacion {prestamo.IdentificacionUsuario} ya tiene un libro prestado por lo cual no se le puede realizar otro prestamo");
            }

            // Verificar si el usuario ya tiene un préstamo con un tipo de usuario diferente
            var usuarioExistente = await _context.Prestamos
                .Where(p => p.IdentificacionUsuario == prestamo.IdentificacionUsuario)
                .FirstOrDefaultAsync();

            if (usuarioExistente != null && usuarioExistente.TipoUsuario != prestamo.TipoUsuario)
            {
                throw new InvalidOperationException($"El usuario con identificacion {prestamo.IdentificacionUsuario} ya tiene un tipo de usuario registrado como {usuarioExistente.TipoUsuario}. No se puede registrar otro tipo.");
            }

            //verificar si ya existe un prestamo con el mismo id
            //var prestamoExistente = await _context.Prestamos.Where(p => p.Id == prestamo.Id).FirstOrDefaultAsync();
            //if (prestamoExistente != null)
            //{
            //    // Si ya existe un préstamo con ese ID, devolver un error 404
            //    throw new InvalidOperationException($"El prestamo con id {prestamo.Id} no existe");
            //}

            prestamo.FechaMaximaDevolucion = CalcularFechaMaximaDevolucion(prestamo.TipoUsuario);

            _context.Prestamos.Add(prestamo);
            await _context.SaveChangesAsync();
            return prestamo;
        }//CrearPrestamoAsync

        public async Task<Prestamo> ObtenerPrestamoPorIdAsync(Guid id)
        {
            return await _context.Prestamos.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> UsuarioTienePrestamoAsync(string identificacionUsuario)
        {
            return await _context.Prestamos.AnyAsync(p => p.IdentificacionUsuario == identificacionUsuario);
        }//


        private DateTime CalcularFechaMaximaDevolucion(int tipoUsuario)
        {
            int dias = tipoUsuario switch
            {
                1 => 10,
                2 => 8,
                3 => 7,
                _ => throw new ArgumentException("Tipo de usuario inválido")
            };

            DateTime fecha = DateTime.Now;
            int diasAgregados = 0;

            while (diasAgregados < dias)
            {
                fecha = fecha.AddDays(1);
                if (fecha.DayOfWeek != DayOfWeek.Saturday && fecha.DayOfWeek != DayOfWeek.Sunday)
                {
                    diasAgregados++;
                }
            }

            return fecha;
        }//CalcularFechaMaximaDevolucion

        public async Task<List<Prestamo>> ObtenerTodosPrestamosAsync()
        {
            return await _context.Prestamos.ToListAsync();
        }//ObtenerTodosPrestamosAsync

        public async Task<Prestamo> ActualizarPrestamoAsync(Guid id, Prestamo prestamo)
        {
            var prestamoExistente = await _context.Prestamos.FindAsync(id);
            if (prestamoExistente == null)
            {
                throw new KeyNotFoundException($"El préstamo con ID {id} no existe.");
            }

            prestamoExistente.Isbn = prestamo.Isbn;
            prestamoExistente.IdentificacionUsuario = prestamo.IdentificacionUsuario;
            prestamoExistente.TipoUsuario = prestamo.TipoUsuario;
            prestamoExistente.FechaMaximaDevolucion = prestamo.FechaMaximaDevolucion;

            await _context.CommitAsync();
            return prestamoExistente;
        }//ActualizarPrestamoAsync


        public async Task<bool> EliminarPrestamoAsync(Guid id)
        {
            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo == null)
            {
                throw new KeyNotFoundException($"El préstamo con ID {id} no existe.");
            }

            _context.Prestamos.Remove(prestamo);
            await _context.CommitAsync();
            return true;
        }//EliminarPrestamoAsync

    }//class

}
