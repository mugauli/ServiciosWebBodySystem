using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiciosWebBodySystem.model
{
    public class EventoAgenda
    {
        public int IdEvento { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string HoraFin { get; set; }
        public DateTime FechaHora { get; set; }
        public bool Agotado { get; set; }
    }
}