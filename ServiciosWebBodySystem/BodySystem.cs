using Newtonsoft.Json.Linq;
using ServiciosWebBodySystem.Datos;
using ServiciosWebBodySystem.DTO;
using ServiciosWebBodySystem.Helper;
using ServiciosWebBodySystem.model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Xml;
using System.Xml.Serialization;
using umbraco.cms.businesslogic.member;
using umbraco.NodeFactory;
using Umbraco.Web.WebApi;


namespace ServiciosWebBodySystem
{
    public class BodySystemController : UmbracoApiController
    {
        private CultureInfo ci = new CultureInfo("es-MX");
        private string debug = string.Empty;
        RegistroData _gdRegistro = new RegistroData();

        // GET api/<controller>

        [HttpGet]
        public string GetBanner(string hash)
        {


            var response = new BannerRegistroDTO();
            try
            {
                if (hash.Equals("prueba"))
                {
                    response.Title = @"Registro Trade Show B to B - 25 y 26  de Octubre";
                    response.Contenido = @"<p>Si eres Dueño, Director, Gerente u Operador de un Club, Gimnasio o Instalación Deportiva, éste es el evento y EXPO para ti! Aquí encontrarás todos los materiales y equipo necesario para la operación de tu espacio deportivo indoor y outdoor. Las mejores marcas Nacionales e Internacionales de la INDUSTRIA WELLNESS, exhibiendo las soluciones e innovaciones del sector. ¡Acceso Gratuito! Regístrate en Trade Show.</p>";
                    response.Video = @"<div><iframe src='https://www.youtube.com/embed/VCHDznhezUc?ecver=2' style='position:absolute;width:100%;height:100%;left:0' width='639' height='360' frameborder='0' allowfullscreen></iframe></div>";
                    var xml = @"<multi-url-picker>
                                  <url-picker mode='Media'>
                                    <new-window>False</new-window>
                                    <node-id>1171</node-id>
                                    <url>/media/1635/rombobosysystem.png</url>
                                    <link-title />
                                  </url-picker>
                                  <url-picker mode='Media'>
                                    <new-window>False</new-window>
                                    <node-id>1172</node-id>
                                    <url>/media/1636/rombocybex.png</url>
                                    <link-title />
                                  </url-picker>
                                </multi-url-picker>";

                    var listImg = new List<string>();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    XmlNodeList errorNodes = doc.DocumentElement.SelectNodes("/multi-url-picker/url-picker");

                    foreach (XmlNode pointCoord in doc.SelectNodes("/multi-url-picker/url-picker"))
                    {

                        XmlNode eNode = pointCoord.SelectSingleNode("url");

                        if (eNode != null)
                        {
                            listImg.Add(eNode.InnerText);
                        }
                    }

                    response.ListImages = listImg;


                }
                else
                {
                    Node node = new Node(1050);
                    //debug += node.Name;
                    //debug += node.Children.Count;
                    foreach (Node agen in node.Children)
                    {
                        //  debug += agen.Name;
                        var value = agen.GetProperty("hash");
                        if (value != null)
                            if (value.Value.Equals(hash))
                            {
                                // debug += value.Value;
                                response.Title = agen.Name;
                                response.Contenido = agen.GetProperty("contenido") == null ? "No se encontró contenido." : agen.GetProperty("contenido").Value.ToString();
                                response.Video = agen.GetProperty("urlVideo") == null ? "" : agen.GetProperty("urlVideo").Value.ToString();

                                if (agen.GetProperty("galeria") != null)
                                {


                                    var xml = agen.GetProperty("galeria").Value;

                                    //xml = xml.Replace("\u003c", "<").Replace("\u003e", ">");

                                    debug += "Galeria: " + xml;

                                    var listImg = new List<string>();
                                    XmlDocument doc = new XmlDocument();
                                    doc.LoadXml(xml);

                                    XmlNodeList errorNodes = doc.DocumentElement.SelectNodes("/multi-url-picker/url-picker");

                                    foreach (XmlNode pointCoord in doc.SelectNodes("/multi-url-picker/url-picker"))
                                    {

                                        XmlNode eNode = pointCoord.SelectSingleNode("url");

                                        if (eNode != null)
                                        {
                                            listImg.Add(eNode.InnerText);
                                        }
                                    }
                                    response.ListImages = listImg;
                                }


                                //  debug += "==|" + ( agen.GetProperty("galeria") == null ? "No galeria." : agen.GetProperty("galeria").Value.ToString()) + "|=="; 
                            }
                    }
                }

                return new JavaScriptSerializer().Serialize(new { success = true, debug = debug, result = response });

            }
            catch (Exception ex)
            {

                return new JavaScriptSerializer().Serialize(new { success = false, message = ex.Message, debug = debug });
            }


        }

        [HttpGet]
        [HttpPost]
        public string GuardarRegistro(string json)
        {

            if (ConfigurationManager.AppSettings["debugEmail"].Equals("1"))
            {
                if (json.Contains("prueba"))
                    json = @"[{
                     'Nombre': 'nombre prueba',
                     'ApellidoPaterno': 'apellido prueba',
                     'ApellidoMaterno': 'apellido materno pueba',
                     'Sexo': 'F',
                     'Edad': 5,
                     'Empresa': 'Empresa prueba',
                     'Cargo': 'Cargao prueba',
                     'Ciudad': 8,
                     'Email': '" + json + @"@correo.com',
                     'Pais': 1,
                     'Perfil': 4,
                    'SubPerfil': 0,
                     'IdTipoPase': 8,
                     'Costo': '$2,500.00',
                      'Costo': '$2,500.00',
                     'ServiciosInteres': [1,2,6],
                     'EventosSeleccionados': [10,20,30]
                    }]";
            }

            var jArray = JArray.Parse(json);
            JObject a = JObject.Parse(jArray.First().ToString());

            var reg = new Registro
            {
                Nombre = ((JValue)a.SelectToken("Nombre")).Value.ToString(),
                ApellidoPaterno = ((JValue)a.SelectToken("ApellidoPaterno")).Value.ToString(),
                ApellidoMaterno = ((JValue)a.SelectToken("ApellidoMaterno")).Value.ToString(),
                Sexo = ((JValue)a.SelectToken("Sexo")).Value.ToString(),
                IdEdad = byte.Parse(((JValue)a.SelectToken("Edad")).Value.ToString().ToUpperInvariant()),
                Empresa = ((JValue)a.SelectToken("Empresa")).Value.ToString(),
                Cargo = ((JValue)a.SelectToken("Cargo")).Value.ToString(),
                IdCiudad = int.Parse(((JValue)a.SelectToken("Ciudad")).Value.ToString().ToUpperInvariant()),
                Email = ((JValue)a.SelectToken("Email")).Value.ToString(),
                Telefono = ((JValue)a.SelectToken("Telefono")).Value.ToString(),
                IdPais = int.Parse(((JValue)a.SelectToken("Pais")).Value.ToString().ToUpperInvariant()),

                Perfil = int.Parse(((JValue)a.SelectToken("Perfil")).Value.ToString().ToUpperInvariant()),
                IdTipoPase = int.Parse(((JValue)a.SelectToken("IdTipoPase")).Value.ToString().ToUpperInvariant()),
                NombrePase = ((JValue)a.SelectToken("NombrePase")).Value.ToString(),
                CostoPase = ((JValue)a.SelectToken("Costo")).Value.ToString()
            };

            var Costo = ((JValue)a.SelectToken("Costo")).Value.ToString();
            var NombrePase = ((JValue)a.SelectToken("NombrePase")).Value.ToString();

            #region Lista de servicios
            var ltsServicios = new List<int>();
            foreach (var us in ((JArray)a.SelectToken("ServiciosInteres")))
            {
                ltsServicios.Add(Convert.ToInt32(((JValue)us).Value));
            }
            #endregion

            #region Lista de Eventos
            var ltsEventos = new List<int>();
            var eventoAgotado = false;
            foreach (var us in ((JArray)a.SelectToken("EventosSeleccionados")))
            {
                eventoAgotado = EventoAgotado(Convert.ToInt32(((JValue)us).Value));
                ltsEventos.Add(Convert.ToInt32(((JValue)us).Value));
            }
            #endregion

            if (eventoAgotado) return "Uno o más eventos seleccionados ha excedido cupo límite para el evento." + debug;


            var response = _gdRegistro.GuardarRegistro(reg, ltsServicios, ltsEventos);

            int idRegistro = 0;

            if (response.Code != 0)
                return response.Message;

            #region Enviar Email
            var _EmailManagerHelper = new EmailManagerHelper();
            var _QCbarManager = new QCbarManager();


            string body = @"<head>
                            <style>
                     body {background-color: black;}
                    h1 {color: #ED1548;text-align: center;}
                    h3 {color: #ED1548;}
                    h2 {color: #ED1548;}
                    .imgCodeBar {width: 100%;}
                    .InfoPago {margin: auto;width: 70%;}
                    .datosBanco {color: #808080;}
                    .datosSede {color: #808080;}
                    .datosSede li {color: #808080;}
                    .contQRcode {width: 360px;margin: auto;}
                    .rojoTxt {color: #ED1548;margin: 40px;}
                     </style>
                    </head>
                    <body style='padding:5%'>
                        <header>
                            <img class='imagenLogo' src='cid:Logo' />
                        </header>
                        <h1>Registro Realizado.</h1>
                        <div class='mainContainer'>

                            <div class='imgContainer'  align='center'>
                                <div class='contQRcode'>
                                    <h3>Código de acceso:</h3>
                                    <img class='imgCodeBar' width='300' alt ='' src ='cid:MyImage'/>
                                </div>
                            </div>
                        </div>    
                        <div class='contInfo'  align='center'>
                            <div class='InfoPago'  align='left'>            
                                <h3>Pase:</h3>
                                <ul class='datosBanco'>
                                    <li><div class='titlePase'>Pase: </div><span class='rojoTxt'>" + NombrePase + @"</span>
                                    </li>
                                    <li><div class='titlePase'>Monto del Pase: </div><span class='rojoTxt'>" + Costo + @"</span></li>
                                </ul>
                            </div>
                        </div>";

            if (!Costo.ToUpper().Contains("GRATUITO"))
            {
                body += @"<div class='contInfo'  align='center'>
                            <div class='InfoPago'  align='left'>            
                                <h3>Eventos seleccionados:</h3>
                                <ul class='datosBanco'>";
                foreach (var evento in ltsEventos)
                {
                    body += GetInfoEvento(evento);
                }
                body += @"
                                </ul>
                            </div>
                        </div>";
            }

            body += @"<h2>Registro a partir de las 7:00 hrs</h2>
                    <h2>Sesiones y conferencias a partir de las 8:00 hrs</h2>";

            body += @"<div class='contInfo'  align='center'>
                            <div class='InfoPago'  align='left'>            
                                <h3>Sede:</h3>
                                <ul class='datosSede'>
                                    <li>
                                           <div class='titlePase'>World Trade Center CDMX</div><span class='rojoTxt'><a href='http://neew.mx/sede.aspx'>Visita el sitio Web</a></span>
                                    </li>                                    
                                </ul>
                            </div>
                        </div>";

            if (!Costo.ToUpper().Contains("GRATUITO"))
            {
                body += @"<div class='contInfo'  align='center'>
                            <div class='InfoPago'  align='left'>
                                <h3>Para validar su pase realice su pago con las siguientes opciones.</h3>
                                <h3>Formas de Pago:</h3>
                                <ul class='datosBanco'>
                                    <li>
                                        Vía Bancaria ya sea depósito o transferencia con los siguientes datos:
                                        <ul>
                                            <li>Banco: Scotiabank</li>
                                            <li>Cliente: MERFITMEX SAPI DE CV</li>
                                            <li>Cuenta: 00105306279</li>
                                            <li>CLABE: 044180001053062792</li>
                                        </ul>
                                    </li>
                                    <li>Vía tarjeta de crédito autorizando el cargo con el formato correspondiente, solicitar al mail evelyn.oviedo@neew.com.mx</li>
                                    <li>Cheque o efectivo entregado en nuestras oficinas, de lunes a viernes de 10:00 a 17:00hrs ubicadas en Lago Nargis no. 53, Colonia Granada, Miguel Hidalgo, México, CDMX, CP 11520</li>
                                </ul>
                            </div>
                        </div>";
                body += "<h3>UNA VEZ REALIZADO TU PAGO TE PEDIMOS ENVIAR COPIA DE TU COMPROBANTE  VÍA CORREO A <a href='mailto:wowfitness@neew.com.mx?Subject=Comprobante' target='_top'>wowfitness@neew.com.mx</a></h3>";
            }

            body += @"<h1>¡TE ESPERAMOS PARA CREAR JUNTOS EL MEJOR EVENTO WELLNESS EN MÉXICO!</h1>";

            body += @"</body>";



            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            var resp = _EmailManagerHelper.SendMail("registro@neew.mx", reg.Email, "Codigo QC para acceso", body, String.Empty, _QCbarManager.GenerarQCcode(idRegistro), mappedPath);




            #endregion

            return resp.ToString();
        }

        [HttpGet]
        [HttpPost]
        public string Contacto(string json)
        {
            if (ConfigurationManager.AppSettings["debugEmail"].Equals("1"))
            {
                if (json.Contains("prueba"))
                    json = @"[{
                        'Nombre': 'nombre',
                        'ApellidoPaterno': 'appellidopaterno',
                        'ApellidoMaterno': 'apellidopaterno',
                        'Correo': 'correo@correo.com',
                        'Correo2': 'correo@correo.com',
                        'Telefono': '555555555',
                        'Comentarios': 'Comentarios'
                    }]";
            }
            var jArray = JArray.Parse(json);
            JObject a = JObject.Parse(jArray.First().ToString());

            var reg = new Registro
            {
                Nombre = ((JValue)a.SelectToken("Nombre")).Value.ToString(),
                ApellidoPaterno = ((JValue)a.SelectToken("ApellidoPaterno")).Value.ToString(),
                ApellidoMaterno = ((JValue)a.SelectToken("ApellidoMaterno")).Value.ToString(),

            };





            #region Enviar Email

            Node _node = new Node(1064);

            var _EmailManagerHelper = new EmailManagerHelper();
            var _QCbarManager = new QCbarManager();


            string body = @"<head>
                <style>
                    body {background-color: black;}
                    h1 {color: #ED1548;text-align: center;}
                    h3 {color: #ED1548;}
                   .rojoTxt {color: #ED1548; margin: 40px;}
                </style>
            </head>
            <body style='padding:5%'>
                <header>
                    <img class='imagenLogo' src='cid:Logo' />
                </header>
                <h1>Mensaje de contacto recibido.</h1>    
                <div class='contInfo'  align='center'>
                    <div class='InfoPago'  align='left'>            
                        <h3>Contacto:</h3>
                        <ul class='datosBanco'>
                            <li><div class='titlePase'>Nombre: </div><span class='rojoTxt'>" + ((JValue)a.SelectToken("Nombre")).Value.ToString() + " " + ((JValue)a.SelectToken("ApellidoPaterno")).Value.ToString() + " " + ((JValue)a.SelectToken("ApellidoMaterno")).Value.ToString() + @"</span>
                            </li>
                            <li><div class='titlePase'>Correo electrónico: </div><span class='rojoTxt'>" + ((JValue)a.SelectToken("Correo")).Value.ToString() + @"</span></li>
                            <li><div class='titlePase'>Teléfono: </div><span class='rojoTxt'>" + ((JValue)a.SelectToken("Telefono")).Value.ToString() + @"</span></li>
                            <li><div class='titlePase'>Comentarios: </div><span class='rojoTxt'>" + ((JValue)a.SelectToken("Comentarios")).Value.ToString() + @"</span></li>

                        </ul>
                    </div>
                </div>
   
            </body>
                ";
            if (ConfigurationManager.AppSettings["debugEmail"].Equals("1"))
            {
                body += json;
            }

            if (_node.GetProperty("correoDeEnvioDeContacto") != null)
            {

                var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                var resp = _EmailManagerHelper.SendMail("ulises.munoz@grapesoft.com", _node.GetProperty("correoDeEnvioDeContacto").Value, "Mensaje de contacto.", body, String.Empty, null, mappedPath);
                return resp.ToString();
            }
            else
            {
                return "Correo de contacto invalido";
            }



            #endregion


        }

        [HttpPost]
        [HttpGet]
        public string GetEventos(int IdEvento, bool seminario, bool taller)
        {
            try
            {

                var MatrizEventos = new List<EventoFecha>();
                var Eventos = new List<EventoAgenda>();

                if (IdEvento == 0)
                {
                    Eventos.Add(new EventoAgenda { IdEvento = 1, Nombre = "Nombre 1", Fecha = DateTime.Now.ToString("dd MMMM ", ci).ToUpperInvariant(), Hora = DateTime.Now.ToString("HH:mm"), FechaHora = DateTime.Now.AddHours(1), Agotado = true, Descripcion = "Conferencia" });
                    Eventos.Add(new EventoAgenda { IdEvento = 2, Nombre = "Nombre 2", Fecha = DateTime.Now.ToString("dd MMMM ", ci).ToUpperInvariant(), Hora = DateTime.Now.ToString("HH:mm"), FechaHora = DateTime.Now.AddHours(-1), Agotado = true, Descripcion = "Conferencia" });
                    Eventos.Add(new EventoAgenda { IdEvento = 3, Nombre = "Nombre 3", Fecha = DateTime.Now.ToString("dd MMMM ", ci).ToUpperInvariant(), Hora = DateTime.Now.ToString("HH:mm"), FechaHora = DateTime.Now.AddHours(4), Agotado = false, Descripcion = "Conferencia" });
                    Eventos.Add(new EventoAgenda { IdEvento = 4, Nombre = "Nombre 4", Fecha = DateTime.Now.ToString("dd MMMM ", ci).ToUpperInvariant(), Hora = DateTime.Now.ToString("HH:mm"), FechaHora = DateTime.Now.AddHours(3), Agotado = true, Descripcion = "Seminario" });
                    Eventos.Add(new EventoAgenda { IdEvento = 5, Nombre = "Nombre 5", Fecha = DateTime.Now.ToString("dd MMMM ", ci).ToUpperInvariant(), Hora = DateTime.Now.ToString("HH:mm"), FechaHora = DateTime.Now.AddHours(2), Agotado = true, Descripcion = "Conferencia" });
                    Eventos.Add(new EventoAgenda { IdEvento = 6, Nombre = "Nombre 1", Fecha = DateTime.Now.AddDays(1).ToString("dd MMMM ", ci).ToUpperInvariant(), Hora = DateTime.Now.ToString("HH:mm"), FechaHora = DateTime.Now.AddHours(1), Agotado = true, Descripcion = "Conferencia" });
                    Eventos.Add(new EventoAgenda { IdEvento = 7, Nombre = "Nombre 2", Fecha = DateTime.Now.AddDays(1).ToString("dd MMMM ", ci).ToUpperInvariant(), Hora = DateTime.Now.ToString("HH:mm"), FechaHora = DateTime.Now.AddHours(-1), Agotado = true, Descripcion = "Conferencia" });
                    Eventos.Add(new EventoAgenda { IdEvento = 8, Nombre = "Nombre 3", Fecha = DateTime.Now.AddDays(1).ToString("dd MMMM ", ci).ToUpperInvariant(), Hora = DateTime.Now.ToString("HH:mm"), FechaHora = DateTime.Now.AddHours(4), Agotado = false, Descripcion = "Seminario" });
                    Eventos.Add(new EventoAgenda { IdEvento = 9, Nombre = "Nombre 4", Fecha = DateTime.Now.AddDays(1).ToString("dd MMMM ", ci).ToUpperInvariant(), Hora = DateTime.Now.ToString("HH:mm"), FechaHora = DateTime.Now.AddHours(3), Agotado = true, Descripcion = "Conferencia" });
                    Eventos.Add(new EventoAgenda { IdEvento = 10, Nombre = "Nombre 5", Fecha = DateTime.Now.AddDays(1).ToString("dd MMMM ", ci).ToUpperInvariant(), Hora = DateTime.Now.ToString("HH:mm"), FechaHora = DateTime.Now.AddHours(2), Agotado = true, Descripcion = "Conferencia" });
                    Eventos.Add(new EventoAgenda { IdEvento = 11, Nombre = "Nombre 1", Fecha = DateTime.Now.AddDays(2).ToString("dd MMMM ", ci).ToUpperInvariant(), Hora = DateTime.Now.ToString("HH:mm"), FechaHora = DateTime.Now.AddHours(1), Agotado = true, Descripcion = "Conferencia" });
                    Eventos.Add(new EventoAgenda { IdEvento = 12, Nombre = "Nombre 2", Fecha = DateTime.Now.AddDays(2).ToString("dd MMMM ", ci).ToUpperInvariant(), Hora = DateTime.Now.ToString("HH:mm"), FechaHora = DateTime.Now.AddHours(-1), Agotado = true, Descripcion = "Seminario" });
                    Eventos.Add(new EventoAgenda { IdEvento = 13, Nombre = "Nombre 3", Fecha = DateTime.Now.AddDays(2).ToString("dd MMMM ", ci).ToUpperInvariant(), Hora = DateTime.Now.ToString("HH:mm"), FechaHora = DateTime.Now.AddHours(4), Agotado = true, Descripcion = "Conferencia" });
                    Eventos.Add(new EventoAgenda { IdEvento = 14, Nombre = "Nombre 4", Fecha = DateTime.Now.AddDays(2).ToString("dd MMMM ", ci).ToUpperInvariant(), Hora = DateTime.Now.ToString("HH:mm"), FechaHora = DateTime.Now.AddHours(3), Agotado = false, Descripcion = "Conferencia" });
                    Eventos.Add(new EventoAgenda { IdEvento = 15, Nombre = "Nombre 5", Fecha = DateTime.Now.AddDays(2).ToString("dd MMMM ", ci).ToUpperInvariant(), Hora = DateTime.Now.ToString("HH:mm"), FechaHora = DateTime.Now.AddHours(2), Agotado = true, Descripcion = "Taller" });
                }
                else
                {
                    Eventos = getEventsUmb(IdEvento, seminario, taller);
                }
                Eventos = Eventos.OrderBy(x => x.FechaHora).ToList();

                String fechaAtual = String.Empty;
                List<String> obtenidas = new List<string>();

                foreach (var item in Eventos)
                {
                    if (!obtenidas.Contains(item.Fecha))
                    {
                        obtenidas.Add(item.Fecha);
                        MatrizEventos.Add(new EventoFecha { FechaEvento = item.Fecha, EventosLts = Eventos.Where(x => x.Fecha.Equals(item.Fecha)).ToList() });
                    }
                }

                return new JavaScriptSerializer().Serialize(new { success = true, MatrizEventos = MatrizEventos, debug = debug });

            }
            catch (Exception ex)
            {

                return new JavaScriptSerializer().Serialize(new { success = false, Message = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty), debug = debug });
            }
        }


        [HttpGet]
        [HttpPost]
        public string PruebaCorreo(string correoDestino)
        {
            #region Enviar Email
            var _EmailManagerHelper = new EmailManagerHelper();
            var _QCbarManager = new QCbarManager();


            string body = @"<head>
                            <style>
                     body {background-color: black;}
                    h1 {color: #ED1548;text-align: center;}
                    h3 {color: #ED1548;}
                    h2 {color: #ED1548;}
                    .imgCodeBar {width: 100%;}
                    .InfoPago {margin: auto;width: 70%;}
                    .datosBanco {color: #808080;}
                    .datosSede {color: #808080;}
                    .datosSede li {color: #808080;}
                    .contQRcode {width: 360px;margin: auto;}
                    .rojoTxt {color: #ED1548;margin: 40px;}
                     </style>
                    </head>
                    <body style='padding:5%'>
                        <header>
                            <img class='imagenLogo' src='cid:Logo' />
                        </header>
                        <h1>Registro Realizado.</h1>
                        <div class='mainContainer'>

                            <div class='imgContainer'  align='center'>
                                <div class='contQRcode'>
                                    <h3>Código de acceso:</h3>
                                    <img class='imgCodeBar' width='300' alt ='' src ='cid:MyImage'/>
                                </div>
                            </div>
                        </div>    
                        <div class='contInfo'  align='center'>
                            <div class='InfoPago'  align='left'>            
                                <h3>Pase:</h3>
                                <ul class='datosBanco'>
                                    <li><div class='titlePase'>Pase: </div><span class='rojoTxt'>prueba</span>
                                    </li>
                                    <li><div class='titlePase'>Monto del Pase: </div><span class='rojoTxt'>prueba</span></li>
                                </ul>
                            </div>
                        </div>";

            body += @"<h2>Registro a partir de las 7:00 hrs</h2>
                    <h2>Sesiones y conferencias a partir de las 8:00 hrs</h2>";

            body += @"<div class='contInfo'  align='center'>
                            <div class='InfoPago'  align='left'>            
                                <h3>Sede:</h3>
                                <ul class='datosSede'>
                                    <li>
                                           <div class='titlePase'>World Trade Center CDMX</div><span class='rojoTxt'><a href='http://neew.mx/sede.aspx'>Visita el sitio Web</a></span>
                                    </li>                                    
                                </ul>
                            </div>
                        </div>";

            body += @"<h1>¡TE ESPERAMOS PARA CREAR JUNTOS EL MEJOR EVENTO WELLNESS EN MÉXICO!</h1>";
            body += @"</body>";


            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            var resp = _EmailManagerHelper.SendMail("ulises.munoz@grapesoft.com", correoDestino, "Codigo QC para acceso", body, String.Empty, _QCbarManager.GenerarQCcode(10001), null);


            #endregion

            return resp.ToString();
        }

        [HttpGet]
        public string Prueba()
        {
            return "Prueba: " + DateTime.Now.ToString("dd MMMM yyyy hh:mm:ss");
        }

        public List<EventoAgenda> getEventsUmb(int IdEvento, bool seminario, bool taller)
        {

            var response = new List<EventoAgenda>();
            try
            {
                Node node = new Node(IdEvento);

                String agenda = node.GetProperty("agenda") != null ? node.GetProperty("agenda").Value : string.Empty;
                String[] agendas = agenda.Split(',');
                debug += "== AGENDA: " + agenda + " == ";
                foreach (var agen in agendas)
                {
                    int idNodeAgenda = Convert.ToInt32(agen);
                    debug += "OutAgen" + agen;
                    if (idNodeAgenda != 0)
                    {
                        debug += "InAgen" + agen;
                        Node nodeAgenda = new Node(idNodeAgenda);

                        foreach (Node ItemSubmenu in nodeAgenda.Children)
                        {
                            if (ItemSubmenu.NodeTypeAlias == "Eventodia")
                            {
                                foreach (Node EventoExpositor3 in ItemSubmenu.Children)
                                {
                                    #region RUTA 3
                                    debug += "||||====================================) RUTA 3 (=================================================||||";
                                    debug += "|==) EventoExpositor3: " + EventoExpositor3.Id + "|==) ";
                                    debug += "|==) Taller: " + (EventoExpositor3.GetProperty("esTaller") == null ? "No hay" : (EventoExpositor3.GetProperty("esTaller").Value.ToString())) + "|==) ";
                                    debug += "|==) Seminario: " + (EventoExpositor3.GetProperty("esSeminario") == null ? "No hay esSeminario" : (EventoExpositor3.GetProperty("esSeminario").Value.ToString())) + "|==) ";

                                    var isTaller = EventoExpositor3.GetProperty("esTaller") == null ? false : (EventoExpositor3.GetProperty("esTaller").Value.ToString() == "1");
                                    var isSeminario = EventoExpositor3.GetProperty("esSeminario") == null ? false : (EventoExpositor3.GetProperty("esSeminario").Value.ToString() == "1");

                                    if (!seminario && !taller || seminario && isSeminario || taller && isTaller)
                                    {
                                        debug += "(===)Entro: " + EventoExpositor3.Id + "(===)";
                                        debug += "(===)ESeminario: " + seminario.ToString() + "(===)";
                                        debug += "(===)EisSeminario: " + isSeminario.ToString() + "(===)";
                                        debug += "(===)ETalller: " + taller.ToString() + "(===)";
                                        debug += "(===)EisTaller: " + isTaller.ToString() + "(===)";


                                        if (EventoExpositor3.GetProperty("fechaYHora") != null && EventoExpositor3.GetProperty("fechaYHoraFin") != null)
                                        {
                                            //debug += "===>ConFecha===" + EventoExpositor3.Id + "-";

                                            DateTime eventFecha = DateTime.Parse(EventoExpositor3.GetProperty("fechaYHora").Value);
                                            DateTime eventFechaFin = DateTime.Parse(EventoExpositor3.GetProperty("fechaYHoraFin").Value);
                                            response.Add(new EventoAgenda
                                            {
                                                IdEvento = EventoExpositor3.Id,
                                                Nombre = EventoExpositor3.Name,
                                                Fecha = eventFecha.ToString("dd MMMM ", ci).ToUpperInvariant(),
                                                Hora = eventFecha.ToString("HH:mm"),
                                                HoraFin = eventFechaFin.ToString("HH:mm"),
                                                FechaHora = eventFecha,
                                                Agotado = EventoAgotado(EventoExpositor3.Id),
                                                Descripcion = nodeAgenda.Name
                                            });
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                debug += "ItemSubMEnu-|" + ItemSubmenu.Id + "|-";
                                foreach (Node EventoDia in ItemSubmenu.Children)
                                {

                                    debug += "EventoDia-|" + EventoDia.Id + "|-";
                                    foreach (Node EventoExpositor in EventoDia.Children)
                                    {
                                        debug += "EventoExpositor-|" + EventoExpositor.Id + "|-";
                                        if (EventoExpositor.NodeTypeAlias == "Eventodia")
                                        {
                                            foreach (Node EventoExpositor2 in EventoExpositor.Children)
                                            {
                                                debug += "||||====================================) RUTA 2 (=================================================||||";
                                                debug += "|==) EventoExpositor2: " + EventoExpositor2.Id + "|==) ";
                                                debug += "|==) EventoExpositor2 Type: " + EventoExpositor2.NodeTypeAlias + "|==) ";

                                                debug += "|==) Taller: " + (EventoExpositor2.GetProperty("esTaller") == null ? "No hay" : (EventoExpositor2.GetProperty("esTaller").Value.ToString())) + "|==) ";
                                                debug += "|==) Seminario: " + (EventoExpositor2.GetProperty("esSeminario") == null ? "No hay esSeminario" : (EventoExpositor2.GetProperty("esSeminario").Value.ToString())) + "|==) ";

                                                var isTaller = EventoExpositor2.GetProperty("esTaller") == null ? false : (EventoExpositor2.GetProperty("esTaller").Value.ToString() == "1");
                                                var isSeminario = EventoExpositor2.GetProperty("esSeminario") == null ? false : (EventoExpositor2.GetProperty("esSeminario").Value.ToString() == "1");

                                                if (!seminario && !taller || seminario && isSeminario || taller && isTaller)
                                                {

                                                    debug += "===>OTRO===" + EventoExpositor2.Id + "-";
                                                    if (EventoExpositor2.GetProperty("fechaYHora") != null && EventoExpositor2.GetProperty("fechaYHoraFin") != null)
                                                    {
                                                        debug += "===>ConFecha===" + EventoExpositor2.Id + "-";

                                                        DateTime eventFecha = DateTime.Parse(EventoExpositor2.GetProperty("fechaYHora").Value);
                                                        DateTime eventFechaFin = DateTime.Parse(EventoExpositor2.GetProperty("fechaYHoraFin").Value);
                                                        response.Add(new EventoAgenda
                                                        {
                                                            IdEvento = EventoExpositor2.Id,
                                                            Nombre = EventoExpositor2.Name,
                                                            Fecha = eventFecha.ToString("dd MMMM ", ci).ToUpperInvariant(),
                                                            Hora = eventFecha.ToString("HH:mm"),
                                                            HoraFin = eventFechaFin.ToString("HH:mm"),
                                                            FechaHora = eventFecha,
                                                            Agotado = EventoAgotado(EventoExpositor2.Id),
                                                            Descripcion = ItemSubmenu.Name + " - " + EventoDia.Name
                                                        });
                                                    }
                                                }

                                            }
                                        }
                                        else
                                        {
                                            debug += "||||====================================) RUTA 1 (=================================================||||";
                                            debug += "|==) EventoExpositor: " + EventoExpositor.Id + "|==) ";
                                            debug += "|==) EventoExpositor Type: " + EventoExpositor.NodeTypeAlias + "|==) ";

                                            debug += "|==) Taller: " + (EventoExpositor.GetProperty("esTaller") == null ? "No hay" : (EventoExpositor.GetProperty("esTaller").Value.ToString())) + "|==) ";
                                            debug += "|==) Seminario: " + (EventoExpositor.GetProperty("esSeminario") == null ? "No hay esSeminario" : (EventoExpositor.GetProperty("esSeminario").Value.ToString())) + "|==) ";

                                            var isTaller = EventoExpositor.GetProperty("esTaller") == null ? false : (EventoExpositor.GetProperty("esTaller").Value.ToString() == "1");
                                            var isSeminario = EventoExpositor.GetProperty("esSeminario") == null ? false : (EventoExpositor.GetProperty("esSeminario").Value.ToString() == "1");

                                            if (!seminario && !taller || seminario && isSeminario || taller && isTaller)
                                            {
                                                debug += EventoExpositor.Id + "-";
                                                if (EventoExpositor.GetProperty("fechaYHora") != null && EventoExpositor.GetProperty("fechaYHoraFin") != null)
                                                {

                                                    DateTime eventFecha = DateTime.Parse(EventoExpositor.GetProperty("fechaYHora").Value);
                                                    DateTime eventFechaFin = DateTime.Parse(EventoExpositor.GetProperty("fechaYHoraFin").Value);
                                                    response.Add(new EventoAgenda
                                                    {
                                                        IdEvento = EventoExpositor.Id,
                                                        Nombre = EventoExpositor.Name,
                                                        Fecha = eventFecha.ToString("dd MMMM ", ci).ToUpperInvariant(),
                                                        Hora = eventFecha.ToString("HH:mm"),
                                                        HoraFin = eventFechaFin.ToString("HH:mm"),
                                                        FechaHora = eventFecha,
                                                        Agotado = EventoAgotado(EventoExpositor.Id),
                                                        Descripcion = ItemSubmenu.Name
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }


            return response;

        }

        public bool EventoAgotado(int IdEvento)
        {
            var response = false;

            #region Obtener Cupo
            Node _Evento = new Node(IdEvento);

            int idSalon = _Evento.GetProperty("salon") != null ? Convert.ToInt32(_Evento.GetProperty("salon").Value) : 0;

            if (idSalon == 0) return true;

            Node _Salon = new Node(idSalon);

            int tmCupo = 0;
            int Cupo = _Salon.GetProperty("capacidad") != null ? (Int32.TryParse(_Salon.GetProperty("capacidad").Value, out tmCupo) ? tmCupo : 0) : 0;
            #endregion
            //debug += "SSSSSS==CUPO==|" + Cupo + "|==";
            //debug += "==EVENTO==|" + _Evento.Id + "|==";
            //debug += "==SALON==|" + _Salon.Id + "|==";
            //Liberar pases vencidos
            _gdRegistro.LiberarPasesVencidos();

            #region Obtener Ocupados
            var ocupados = _gdRegistro.ObtenerOcupados(IdEvento);
            //debug += "==OCUPADOS==|" + ocupados + "|==SSSSSS";
            #endregion
            response = ocupados >= Cupo;
            return response;
        }

        public string GetInfoEvento(int IdEvento)
        {
            Node _node = new Node(IdEvento);

            DateTime eventFecha = DateTime.Parse(_node.GetProperty("fechaYHora").Value);

            return @"<li><div class='titlePase'>" + _node.Name + " - " + eventFecha.ToString("dd MMMM ", ci).ToUpperInvariant() + " " + eventFecha.ToString("HH:mm") + @" hrs</div></li>";

        }

    }
}