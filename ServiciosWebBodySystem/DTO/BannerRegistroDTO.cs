using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiciosWebBodySystem.DTO
{
    public class BannerRegistroDTO
    {
        public string Title { get; set; }
        public string Contenido { get; set; }
        public string Video { get; set; }
        public List<String> ListImages { get; set; }
    }
}