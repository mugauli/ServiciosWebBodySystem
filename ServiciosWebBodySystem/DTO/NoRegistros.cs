using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiciosWebBodySystem.DTO
{
    public class NoRegistros
    {
        public int Validados { get; set; }
        public int Pendientes { get; set; }
        public int Vencidos { get; set; }
        public int Cancelados { get; set; }
    }
}