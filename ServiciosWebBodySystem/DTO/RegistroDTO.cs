using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiciosWebBodySystem.DTO
{
    public class RegistroDTO
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
        public Nullable<int> Perfil { get; set; }
        public Nullable<int> IdTipoPase { get; set; }
        public string SubPErfil { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public Nullable<short> IdEstatus { get; set; }
       
        public virtual ICollection<ServiciosInteresDTO> ServiciosInteres { get; set; }
        public virtual ICollection<RegistroEventosDTO> RegistroEventos { get; set; }
        public virtual ctStatusRegistroDTO ctStatusRegistro { get; set; }
        public virtual String nombrePase { get; set; }
    }
}