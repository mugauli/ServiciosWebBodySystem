using ServiciosWebBodySystem.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiciosWebBodySystem.model
{
    public class RegistroViewModel
    {
        public int IdRegistro { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Sexo { get; set; }
        public Nullable<byte> IdEdad { get; set; }
        public string Empresa { get; set; }
        public string Cargo { get; set; }
        public Nullable<int> IdCiudad { get; set; }
        public Nullable<int> IdPais { get; set; }
        public string Email { get; set; }
        public String Fecha { get; set; }
        public Nullable<short> IdEstatus { get; set; }
        public PaseDTO Pase { get; set; } 
        public List<EventoFecha> EventosLts { get; set; }

    }
}