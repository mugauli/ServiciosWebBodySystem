using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiciosWebBodySystem.DTO
{
    public class MethodResponseDTO<T>
    {
        /// <summary>
        ///Lista de Códigos de Error
        /// Modelo de Datos                  -100
        /// Controlador                      -400
        /// </summary>
        public int Code { get; set; }

        public string Message { get; set; }

        public string InternalMessage { get; set; }

        public T Result { get; set; }
    }
}