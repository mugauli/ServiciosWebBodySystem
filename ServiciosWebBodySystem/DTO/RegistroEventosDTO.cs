using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiciosWebBodySystem.DTO
{
    public class RegistroEventosDTO
    {
        public int IdRegistro { get; set; }
        public int IdEvento { get; set; }
        public bool Estado { get; set; }
        public string NombreEvento { get; set; }
    }
}