using Microsoft.AspNetCore.Mvc;
using PruebaIngresoBibliotecario.Api.Services;
using System.Threading.Tasks;
using PruebaIngresoBibliotecario.Api.Models;
using System;
using System.Collections.Generic;


namespace PruebaIngresoBibliotecario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrestamoController : ControllerBase
    {
        public enum TipoUsuarioPrestamo
        {
            AFILIADO = 1,
            EMPLEADO = 2,
            INVITADO = 3
        }

        private readonly IPrestamoService _prestamoService;

        public PrestamoController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }//constructor

        //POST /api/prestamo
        [HttpPost]
        public async Task<IActionResult> CrearPrestamo([FromBody] Prestamo prestamo)
        {
            // Validación de que el TipoUsuario sea válido
            if (!Enum.IsDefined(typeof(TipoUsuarioPrestamo), prestamo.TipoUsuario))
            {
                return BadRequest(new { mensaje = "Tipo de usuario inválido" });
            }

            //validacion de que el ISBN sea GUID VALIDO
            if (!Guid.TryParse(prestamo.Isbn, out _))
            {
                return BadRequest(new { mensaje = "EL isbn debe ser un GUID valido" });
            }

            // Validación de que la IdentificacionUsuario no sea nula o vacía
            if (string.IsNullOrWhiteSpace(prestamo.IdentificacionUsuario))
            {
                return BadRequest(new { mensaje = "La IdentificacionUsuario no puede estar vacía." });
            }

            try
            {
                var nuevoPrestamo = await _prestamoService.CrearPrestamoAsync(prestamo);
                return Ok(new
                {
                    id = nuevoPrestamo.Id,
                    fechaMaximaDevolucion = nuevoPrestamo.FechaMaximaDevolucion.ToString("MM/dd/yyyy"),
                });
            }
            catch (InvalidOperationException ex)
            {
                // Captura la excepción y retorna un error 400 con el mensaje de la excepción
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                // Log error si es necesario
                return StatusCode(500, "Ocurrió un error en el servidor.");
            }


        }//CrearPrestamo

        //GET /api/prestamo
        [HttpGet]
        public async Task<IActionResult> ObtenerTodosPrestamos()
        {
            var prestamos = await _prestamoService.ObtenerTodosPrestamosAsync();
            return Ok(prestamos);
        }//ObtenerTodosPrestamos


        // GET /api/prestamo/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPrestamo(Guid id)
        {
            var prestamo = await _prestamoService.ObtenerPrestamoPorIdAsync(id);
            if (prestamo == null)
            {
                return NotFound(new { mensaje = $"El prestamo con id {id} no existe" });
            }
            return Ok(prestamo);
        }//ObtenerPrestamo

        // PUT /api/prestamo/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarPrestamo(Guid id, [FromBody] Prestamo prestamo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var prestamoActualizado = await _prestamoService.ActualizarPrestamoAsync(id, prestamo);
                return Ok(prestamoActualizado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }//

        // DELETE /api/prestamo/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPrestamo(Guid id)
        {
            try
            {
                var resultado = await _prestamoService.EliminarPrestamoAsync(id);
                return Ok(new { mensaje = "El préstamo ha sido eliminado correctamente." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }



    }//Controller
}
