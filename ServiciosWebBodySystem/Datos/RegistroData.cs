using ServiciosWebBodySystem.DTO;
using ServiciosWebBodySystem.model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using umbraco.NodeFactory;

namespace ServiciosWebBodySystem.Datos
{
    public class RegistroData
    {
        private CultureInfo ci = new CultureInfo("es-MX");
        public MethodResponseDTO<int> GuardarRegistro(Registro registro, List<int> Intereses, List<int> Eventos)
        {

            try
            {
                var response = new MethodResponseDTO<int> { Code = 0 };

                using (var context = new BodySystemDBEntities())
                {

                    if (context.Registro.Where(x => x.Email == registro.Email && (x.IdEstatus == 1 || x.IdEstatus == 2)).ToList().Count == 0)
                    {
                        registro.Fecha = DateTime.Now;
                        registro.IdEstatus = 1;
                        context.Registro.Add(registro);
                        context.SaveChanges();


                        foreach (var a in Intereses)
                        {
                            context.ServiciosInteres.Add(new ServiciosInteres { IdRegistro = registro.IdRegistro, IdServiciosInteres = a });
                            context.SaveChanges();

                        }
                        foreach (var a in Eventos)
                        {
                            context.RegistroEventos.Add(new RegistroEventos { IdRegistro = registro.IdRegistro, IdEvento = a, Estado = true });
                            context.SaveChanges();

                        }

                        response.Result = registro.IdRegistro;
                    }
                    else
                    {
                        response.Code = -800;
                        response.Message = "Ya se ha registrado el correo con otro usuario.";
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                return new MethodResponseDTO<int> { Code = -100, Message = ex.Message };
                throw;
            }


        }

        public MethodResponseDTO<NoRegistros> GetNoRegistros()
        {

            try
            {
                var response = new MethodResponseDTO<NoRegistros> { Code = 0 };

                using (var context = new BodySystemDBEntities())
                {
                    var NoRegistros = new NoRegistros();

                    var registros = context.Registro.ToList();

                    NoRegistros.Validados = registros.Where(x => x.IdEstatus == 2).Count();
                    NoRegistros.Pendientes = registros.Where(x => x.IdEstatus == 1).Count();
                    NoRegistros.Vencidos = registros.Where(x => x.IdEstatus == 3).Count();
                    NoRegistros.Cancelados = registros.Where(x => x.IdEstatus == 4).Count();

                    response.Result = NoRegistros;

                }
                return response;
            }
            catch (Exception ex)
            {
                return new MethodResponseDTO<NoRegistros> { Code = -100, Message = ex.Message };
                throw;
            }


        }

        public void LiberarPasesVencidos()
        {
            try
            {

                using (var context = new BodySystemDBEntities())
                {
                    #region obtener Registros vencidos
                    var fechaVencimiento = DateTime.Now.AddDays(-5);
                    var registrosVencidos = context.Registro.Where(x => x.Fecha < fechaVencimiento && x.IdEstatus == 1);

                    foreach (var item in registrosVencidos)
                    {
                        item.IdEstatus = 3;
                        var regEvento = context.RegistroEventos.Where(x => x.IdRegistro == item.IdRegistro);
                        foreach (var evento in regEvento)
                        {
                            evento.Estado = false;

                        }
                    }
                    #endregion
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public int ObtenerOcupados(int IdEvento)
        {
            try
            {
                using (var context = new BodySystemDBEntities())
                {
                    return context.RegistroEventos.Where(x => x.IdEvento == IdEvento && x.Estado == true).Count();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Registro"></param>
        /// <param name="Nombre"></param>
        /// <param name="ApPaterno"></param>
        /// <param name="ApMaterno"></param>
        /// <param name="Email"></param>
        /// <param name="Estado"></param>
        /// <param name="Pase"></param>
        /// <returns></returns>
        public List<RegistroDTO> GetRegistroByfiltro(int Registro, string Nombre, string ApPaterno, string ApMaterno, string Email, int Estado, int Pase)
        {
            var registroLts = new List<Registro>();
            try
            {
                using (var context = new BodySystemDBEntities())
                {
                    var resp = MapeoRegistro(context.Registro.Where(x => (x.IdRegistro == Registro || Registro == 0) &&
                                                                          (x.Email.Contains(Email) || String.IsNullOrEmpty(Email)) &&
                                                                          (x.Nombre.Contains(Nombre) || String.IsNullOrEmpty(Nombre)) &&
                                                                          (x.ApellidoPaterno.Contains(ApPaterno) || String.IsNullOrEmpty(ApPaterno)) &&
                                                                          (x.ApellidoMaterno.Contains(ApMaterno) || String.IsNullOrEmpty(ApMaterno)) &&
                                                                          (x.IdEstatus == Estado || Estado == 0) &&
                                                                          (x.IdTipoPase == Pase || Pase == 0)
                                                       ).ToList());
                    return resp;
                }

            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public RegistroViewModel GetRegistroById(int Registro)
        {
            var registroLts = new List<Registro>();
            try
            {
                using (var context = new BodySystemDBEntities())
                {
                    var resp = MapeoRegistroViewModel(context.Registro.Where(x => (x.IdRegistro == Registro)).ToList());
                    return resp;
                }

            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public bool ValidarRegistro(int IdRegistro, bool Validar, int IdMember, string UrlComprobante)
        {
            var registroLts = new List<Registro>();
            try
            {
                using (var context = new BodySystemDBEntities())
                {
                    var resp = context.Registro.Where(x => x.IdRegistro == IdRegistro).FirstOrDefault();
                    resp.IdEstatus = (short)(Validar ? 2 : 4);

                    context.HistoricoRegistro.Add(new HistoricoRegistro
                    {
                        IdRegistro = IdRegistro,
                        IdMember = IdMember,
                        Fecha = DateTime.Now,
                        UrlComprobante = UrlComprobante,
                        IdEstatus = (short)(Validar ? 2 : 4)
                    });
                    context.SaveChanges();
                    return true;
                }

            }
            catch (Exception ex)
            {

                return false;
            }

        }
        public RegistroViewModel MapeoRegistroViewModel(List<Registro> registroLts)
        {
            var reg = new RegistroViewModel();
            var registro = registroLts.FirstOrDefault();


            reg.IdRegistro = registro.IdRegistro;
            reg.Nombre = registro.Nombre;
            reg.ApellidoPaterno = registro.ApellidoPaterno;
            reg.ApellidoMaterno = registro.ApellidoMaterno;
            reg.Sexo = registro.Sexo;
            reg.IdEdad = registro.IdEdad;
            reg.Empresa = registro.Empresa;
            reg.Cargo = registro.Cargo;
            reg.IdCiudad = registro.IdCiudad;
            reg.IdPais = registro.IdPais;
            reg.Email = registro.Email;
            reg.Fecha = registro.Fecha.Value.ToString("dd MMMM yyyy HH:mm", ci).ToUpperInvariant() + " hrs"; ;
            reg.IdEstatus = registro.IdEstatus;

            reg.Pase = new PaseDTO { nombre = registro.NombrePase, costo = registro.CostoPase };
            reg.EventosLts = GetEventosSeleccionadosUMB(registro.RegistroEventos.ToList());





            return reg;
        }
        public List<RegistroDTO> MapeoRegistro(List<Registro> registro)
        {
            var response = new List<RegistroDTO>();

            foreach (var rg in registro)
            {
                var reg = new RegistroDTO
                {
                    IdRegistro = rg.IdRegistro,
                    Nombre = rg.Nombre,
                    ApellidoPaterno = rg.ApellidoPaterno,
                    ApellidoMaterno = rg.ApellidoMaterno,
                    Sexo = rg.Sexo,
                    IdEdad = rg.IdEdad,
                    Empresa = rg.Empresa,
                    Cargo = rg.Cargo,
                    IdCiudad = rg.IdCiudad,
                    IdPais = rg.IdPais,
                    Perfil = rg.Perfil,
                    StrPerfil = GetPerfil(rg.Perfil.Value),
                    IdTipoPase = rg.IdTipoPase,
                    SubPErfil = rg.SubPErfil,
                    Email = rg.Email,
                    Fecha = rg.Fecha,
                    IdEstatus = rg.IdEstatus,
                    nombrePase = rg.NombrePase,
                    Telefono = rg.Telefono,
                    Costo = rg.CostoPase,
                    ServiciosInteres = MapeServiciosInteres(rg.ServiciosInteres),
                    RegistroEventos = MapeRegistroEventos(rg.RegistroEventos),
                    ctStatusRegistro = new ctStatusRegistroDTO { Id = rg.ctStatusRegistro.Id, Descripcion = rg.ctStatusRegistro.Descripcion },
                    Pais = new PaisDTO { IdPais = rg.Pais.IdPais, Descripcion = rg.Pais.Descripcion },
                    Edad = new EdadDTO { IdEdad = rg.Edad.IdEdad, Descripcion = rg.Edad.Descripcion },
                    Ciudad = (rg.Ciudad == null ? new CiudadDTO { IdCiudad = 0, Descripcion = "", IdPais = rg.Pais.IdPais } : new CiudadDTO { IdCiudad = rg.Ciudad.IdCiudad, Descripcion = rg.Ciudad.Descripcion, IdPais = rg.Ciudad.IdPais }),


                };

                response.Add(reg);
            }
            return response;
        }

        public List<ServiciosInteresDTO> MapeServiciosInteres(ICollection<ServiciosInteres> ServiciosInteres)
        {
            var response = new List<ServiciosInteresDTO>();

            foreach (var rg in ServiciosInteres)
            {
                var reg = new ServiciosInteresDTO
                {
                    IdServiciosInteres = rg.IdServiciosInteres,
                    IdRegistro = rg.IdRegistro,
                    NombreServicio = GetSeviciosInteres(rg.IdServiciosInteres)
                };

                response.Add(reg);
            }

            return response;
        }

        public List<RegistroEventosDTO> MapeRegistroEventos(ICollection<RegistroEventos> RegistroEventos)
        {
            var response = new List<RegistroEventosDTO>();
            foreach (var rg in RegistroEventos)
            {
                var reg = new RegistroEventosDTO
                {
                    IdEvento = rg.IdEvento,
                    IdRegistro = rg.IdRegistro,
                    Estado = rg.Estado,
                    NombreEvento = GetEvento(rg.IdEvento)
                };

                response.Add(reg);
            }
            return response;
        }

        public PaseDTO GetPase(int IdPase)
        {


            var response = new PaseDTO();

            return response;
        }

        public List<EventoFecha> GetEventosSeleccionadosUMB(List<RegistroEventos> EventosLts)
        {

            var EventoAgendaLts = new List<EventoAgenda>();
            var response = new List<EventoFecha>();
            try
            {
                foreach (var Evento in EventosLts)
                {

                    Node node = new Node(Evento.IdEvento);
                    DateTime eventFecha = DateTime.Parse(node.GetProperty("fechaYHora").Value);
                    DateTime eventFechaFin = DateTime.Parse(node.GetProperty("fechaYHoraFin").Value);
                    string descripcion = string.Empty;
                    var parent = node.Parent.Parent.Parent;
                    if (parent.NodeTypeAlias == "Eventodia")
                    {
                        descripcion = parent.Parent.Name + " - " + parent.Name;
                    }
                    else
                    {
                        descripcion = parent.Name;
                    }

                    EventoAgendaLts.Add(new EventoAgenda
                    {
                        IdEvento = node.Id,
                        Nombre = node.Name,
                        Fecha = eventFecha.ToString("dd MMMM ", ci).ToUpperInvariant(),
                        Hora = eventFecha.ToString("HH:mm"),
                        HoraFin = eventFechaFin.ToString("HH:mm"),
                        FechaHora = eventFecha,
                        Descripcion = descripcion
                    });

                }



                EventoAgendaLts = EventoAgendaLts.OrderBy(x => x.FechaHora).ToList();

                String fechaAtual = String.Empty;
                List<String> obtenidas = new List<string>();

                foreach (var item in EventoAgendaLts)
                {
                    if (!obtenidas.Contains(item.Fecha))
                    {
                        obtenidas.Add(item.Fecha);
                        response.Add(new EventoFecha { FechaEvento = item.Fecha, EventosLts = EventoAgendaLts.Where(x => x.Fecha.Equals(item.Fecha)).ToList() });
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }


            return response;

        }

        public string GetSeviciosInteres(int Id)
        {

            switch (Id)
            {
                case 1:
                    return "Accesorios";

                case 2:
                    return "Audio";

                case 3:
                    return "Capacitación";

                case 4:
                    return "Electroestimulación";

                case 5:
                    return "Franquicias";

                case 6:
                    return "Mobiliario";

                case 7:
                    return "Servicio y soporte técnico";

                case 8:
                    return "Software";

                case 9:
                    return "Superficies Deportivas";

                case 10:
                    return "Suplementos";

                case 12:
                    return "Uniformes";

                case 13:
                    return "Venta de Equipo Nuevo";

                case 14:
                    return "Venta de Equipo Seminuevo";

                default:
                    return "Otros";



            }


        }

        public string GetEvento(int Id)
        {
            try
            {
                Node node = new Node(Id);

                return node.Name;
            }
            catch (Exception ex)
            {
                return "Evento inactivo";
            }
        }

        public string GetPerfil(int Id)
        {

            switch (Id)
            {
                case 1: return "Proveedor de la industria";
                case 2: return "Emprendedor / Inversionista";
                case 3: return "Director / Gerente";
                case 4: return "Coordinador";
                case 5: return "Entrenador / Instructor";
                case 6: return "Usuario";
                default: return "";
            }

        }

        public string GetCadenaEventos(List<RegistroEventosDTO> eventos)
        {
            StringBuilder result = new StringBuilder();

            foreach (var item in eventos)
            {
                
                result.Append(item.NombreEvento);
                result.Append(" | ");
            }

            return result.ToString();
        }

        public string GetCadenaServiciosInteres(List<ServiciosInteresDTO> ServiciosInteres)
        {
            StringBuilder result = new StringBuilder();

            foreach (var item in ServiciosInteres)
            {

                result.Append(item.NombreServicio);
                result.Append(" | ");
            }

            return result.ToString();
        }
    }
}