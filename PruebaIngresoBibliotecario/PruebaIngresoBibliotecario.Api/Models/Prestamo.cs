using System;

namespace PruebaIngresoBibliotecario.Api.Models
{
    public class Prestamo
    {
        public Guid Id { get; set; }
        public string Isbn { get; set; } = string.Empty;
        public string IdentificacionUsuario { get; set; } = string.Empty;
        public int TipoUsuario { get; set; }
        public DateTime FechaMaximaDevolucion {  get; set; }

    }//
}//
