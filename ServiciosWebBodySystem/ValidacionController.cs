using ServiciosWebBodySystem.Datos;
using ServiciosWebBodySystem.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Security;
using umbraco.cms.businesslogic.member;
using umbraco.NodeFactory;
using Umbraco.Web.Mvc;

namespace ServiciosWebBodySystem
{

    public class ValidacionController : SurfaceController
    {
        private RegistroData _registroData = new RegistroData();
        private String debug = String.Empty;

        [AcceptVerbs("POST")]
        [HttpPost]
        public string Login(String user, String pass)
        {
            if (System.Web.Security.Membership.ValidateUser(user, pass))
            {
                FormsAuthentication.SetAuthCookie(user, true);
                return new JavaScriptSerializer().Serialize(new { success = true, message = "Prueba: " + user + " - " + pass });
            }

            return new JavaScriptSerializer().Serialize(new { success = false, message = "Prueba: " + user + " - " + pass });
        }

        [HttpPost]
        public string LoginOut()
        {
            try
            {
                FormsAuthentication.SignOut();
                return new JavaScriptSerializer().Serialize(new { success = true, message = "se ha cerrado la sessión" });
            }
            catch (Exception ex)
            {
                return new JavaScriptSerializer().Serialize(new { success = false, Message = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty), debug = debug });
            }

        }

        public string LoginActive()
        {
            var nombre = User.Identity.IsAuthenticated ? User.Identity.Name : String.Empty;
            return new JavaScriptSerializer().Serialize(new { success = User.Identity.IsAuthenticated, Name = nombre });

        }

        [Umbraco.Web.Mvc.MemberAuthorize(AllowType = "Validator")]
        public string Prueba()
        {
            return new JavaScriptSerializer().Serialize(new { success = true, message = "Prueba" });
        }

        [Umbraco.Web.Mvc.MemberAuthorize(AllowType = "NoValidator")]
        public string PruebaNoValidator()
        {
            return new JavaScriptSerializer().Serialize(new { success = true, message = "Prueba" });
        }

        [Umbraco.Web.Mvc.MemberAuthorize(AllowType = "Validator")]
        public string ObtenerNumeroRegistros()
        {
            var response = _registroData.GetNoRegistros();
            return new JavaScriptSerializer().Serialize(new { response });
        }

        [HttpPost]
        [Umbraco.Web.Mvc.MemberAuthorize(AllowType = "Validator")]
        public string GetRegistro(int? Registro, string Nombre, string ApPaterno, string ApMaterno, string Email, int Estado, int Pase)
        {
            try
            {

                Registro = Registro == null ? 0 : Registro.Value;

                var member = new MemberAccessException();

                var registro = _registroData.GetRegistroByfiltro(Registro.Value, Nombre, ApPaterno, ApMaterno, Email, Estado,Pase);


                return new JavaScriptSerializer().Serialize(new { success = true, registro = registro, count = registro.Count, debug = debug });

            }
            catch (Exception ex)
            {

                return new JavaScriptSerializer().Serialize(new { success = false, Message = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty), debug = debug });
            }
        }

        [HttpPost]
        [Umbraco.Web.Mvc.MemberAuthorize(AllowType = "Validator")]
        public string GetRegistrobyId(int Registro)
        {
            try
            {


                var registro = _registroData.GetRegistroById(Registro);

                return new JavaScriptSerializer().Serialize(new { success = true, registro = registro, debug = debug });

            }
            catch (Exception ex)
            {

                return new JavaScriptSerializer().Serialize(new { success = false, Message = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty), debug = debug });
            }
        }

        [HttpPost]
        [Umbraco.Web.Mvc.MemberAuthorize(AllowType = "Validator")]
        public string ValidacionRegistro(int IdRegistro, bool Validar)
        {
            try
            {
                var currentMem = Member.GetCurrentMember();
                var registro = _registroData.ValidarRegistro(IdRegistro, Validar, currentMem.Id, "");

                return new JavaScriptSerializer().Serialize(new { success = true, registro = registro, debug = debug });

            }
            catch (Exception ex)
            {

                return new JavaScriptSerializer().Serialize(new { success = false, Message = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty), debug = debug });
            }
        }

        [HttpPost]
        [Umbraco.Web.Mvc.MemberAuthorize(AllowType = "Validator")]
        public string GetCatalogPases()
        {
            try
            {


                Node _nodePases = new Node(1088);
                var pasesLts = new List<PaseDTO>();
                foreach (Node item in _nodePases.Children)
                {
                    debug += "---Item" + item.Id;
                    foreach (Node pase in item.Children)
                    {

                        debug += "---Pases" + pase.Id + "mostraRegistro" + (pase.GetProperty("mostrarEnRegistro") != null ? pase.GetProperty("mostrarEnRegistro").Value : "N/A");
                        if (pase.GetProperty("mostrarEnRegistro") != null)
                            if (pase.GetProperty("mostrarEnRegistro").Value == "1")
                            {
                                debug += "---Entro" + pase.Id + "mostraRegistro" + pase.GetProperty("mostrarEnRegistro").Value;
                                pasesLts.Add(new PaseDTO { nombre = pase.Name, IdPase = pase.Id });
                            }


                    }
                }


                return new JavaScriptSerializer().Serialize(new { success = true, pases = pasesLts, debug = debug });
            }
            catch (Exception ex)
            {

                return new JavaScriptSerializer().Serialize(new { success = false, message = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : "") });
            }
        }

        [HttpPost]
        [Umbraco.Web.Mvc.MemberAuthorize(AllowType = "Validator")]
        public string UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    int IdFactura = Convert.ToInt32(Request.Form["IdFactura"]);
                    var Descripcion = Request.Form["Descripcion"];

                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/Comprobantes/"), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return new JavaScriptSerializer().Serialize(new { success = true, message = "Archivo guardado con exito.", debug = debug });
                }
                catch (Exception ex)
                {
                    return new JavaScriptSerializer().Serialize(new { success = false, Message = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty), debug = debug });
                }
            }
            else
            {
                return new JavaScriptSerializer().Serialize(new { success = true, message = "No se ha seleccionado archivo.", debug = debug });
            }
        }


        [HttpPost]
        [Umbraco.Web.Mvc.MemberAuthorize(AllowType = "Validator")]
        public string UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    int IdFactura = Convert.ToInt32(Request.Form["IdFactura"]);
                    var Descripcion = Request.Form["Descripcion"];

                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/Comprobantes/"), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return new JavaScriptSerializer().Serialize(new { success = true, message = "Archivo guardado con exito.", debug = debug });
                }
                catch (Exception ex)
                {
                    return new JavaScriptSerializer().Serialize(new { success = false, Message = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty), debug = debug });
                }
            }
            else
            {
                return new JavaScriptSerializer().Serialize(new { success = true, message = "No se ha seleccionado archivo.", debug = debug });
            }
        }

    }
}