using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiciosWebBodySystem.model
{
    public class EventoFecha
    {
        public String FechaEvento { get; set; }
        public List<EventoAgenda> EventosLts { get; set; }
    }
}