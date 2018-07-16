using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ServiciosWebBodySystem.Datos;
using ServiciosWebBodySystem.DTO;
using ServiciosWebBodySystem.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.member;
using umbraco.NodeFactory;
using Umbraco.Web.Mvc;

namespace ServiciosWebBodySystem
{

    public class ValidacionController : SurfaceController
    {

        // Url: /umbraco/surface/Validacion/[Method]
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

                var registro = _registroData.GetRegistroByfiltro(Registro.Value, Nombre, ApPaterno, ApMaterno, Email, Estado, Pase);


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


        #region Export Excel

        public System.Web.Mvc.PartialViewResult ExportWhiteListExcel(int? Registro, string Nombre, string ApPaterno, string ApMaterno, string Email, int Estado, int Pase)
        {
            Registro = Registro == null ? 0 : Registro.Value;
            var result = _registroData.GetRegistroByfiltro(Registro.Value, Nombre, ApPaterno, ApMaterno, Email, Estado, Pase);

            var excel = createExportAudit(result);

            setResponseFile(excel);

            return PartialView("_ExportWhiteListExcel", result);
        }
        private void setResponseFile(ExcelCustomHelper exH)
        {
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + exH.Filename);
            Response.ContentType = "application/xls";
            //Response.ContentType = "application/x-download";
            Response.ContentEncoding = Encoding.Default;
            Response.Charset = "UTF-8";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            exH.objList.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }
        private ExcelCustomHelper createExportAudit(List<RegistroDTO> caseList)
        {
            var grid = new GridView();
            var caseTable = new System.Data.DataTable("Case");
            caseTable.Columns.Add("ID", typeof(string));
            caseTable.Columns.Add("Nombre", typeof(string));
            caseTable.Columns.Add("A. Paterno", typeof(string));
            caseTable.Columns.Add("A. Materno", typeof(string));
            caseTable.Columns.Add("Sexo", typeof(string));
            caseTable.Columns.Add("Edad", typeof(string));
            caseTable.Columns.Add("Email", typeof(string));
            caseTable.Columns.Add("Telefono", typeof(string));

            caseTable.Columns.Add("Empresa", typeof(string));
            caseTable.Columns.Add("Cargo", typeof(string));
            caseTable.Columns.Add("Ciudad", typeof(string));
            caseTable.Columns.Add("Pais", typeof(string));
            caseTable.Columns.Add("Perfil", typeof(string));
            caseTable.Columns.Add("Pase", typeof(string));
            caseTable.Columns.Add("Servicios de interes", typeof(string));
            caseTable.Columns.Add("Eventos", typeof(string));
            caseTable.Columns.Add("Costo", typeof(string));
            caseTable.Columns.Add("Estado", typeof(string));


            //Case 
            //CreatedDate 
            //  SortCode 
            //  AccountNumber 
            //  CustomerName 
            //  Description 
            //  Status 


            foreach (var a in caseList)
            {
                caseTable.Rows.Add(a.IdRegistro.ToString(), a.Nombre, a.ApellidoPaterno, a.ApellidoMaterno, a.Sexo, a.Edad.Descripcion, a.Email, a.Telefono, a.Empresa, a.Cargo, a.Ciudad.Descripcion, a.Pais.Descripcion, a.StrPerfil, a.nombrePase, _registroData.GetCadenaServiciosInteres(a.ServiciosInteres), _registroData.GetCadenaEventos(a.RegistroEventos), a.Costo, a.ctStatusRegistro.Descripcion);
            }

            grid.DataSource = caseTable;
            grid.DataBind();

            // grid style
            grid.RowStyle.Height = 25;
            int j = 0;
            foreach (var r in caseList)
            {
                for (int i = 0; i < caseTable.Columns.Count; i++)
                {
                    if (j == 0)
                    {
                        grid.HeaderRow.Cells[i].BackColor = ColorTranslator.FromHtml("#ec1C49");
                        grid.HeaderRow.Cells[i].ForeColor = Color.White;
                        grid.HeaderRow.Cells[i].Font.Bold = true;
                        grid.HeaderRow.Cells[i].HorizontalAlign = HorizontalAlign.Center;
                    }

                    grid.Rows[j].Cells[i].BackColor = j % 2 == 0 ? Color.White : ColorTranslator.FromHtml("#EEEEEE");
                    grid.Rows[j].Cells[i].ForeColor = Color.Black;


                }

                j++;
            }

            string filename = String.Format("Registros.xls", DateTime.Now.ToString("dd_MM_yyyy hh-mm-ss"));
            ExcelCustomHelper ex = new ExcelCustomHelper(grid, "Registros", "Registros", filename);

            return ex;
        }

        #endregion

        #region Imprimir y app
        //[HttpPost]
        public System.Web.Mvc.JsonResult Search(int? Registro, string Nombre, string ApPaterno, string ApMaterno, string Email, string Empresa)
        {
            Registro = Registro == null ? 0 : Registro.Value;
            var result = _registroData.GetRegistroByfiltro2(Registro.Value, Nombre, ApPaterno, ApMaterno, Email, Empresa);

            return Json(new { success = true, result = result }, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        public System.Web.Mvc.JsonResult SearchByIdQR(int Registro)
        {

            var result = _registroData.GetRegistroByfiltro(Registro, String.Empty, String.Empty, String.Empty, String.Empty, 0, 0);

            if (result.Count < 1)
                return Json(new { success = false }, System.Web.Mvc.JsonRequestBehavior.AllowGet);
            else
                return Json(new { success = true, result = result.FirstOrDefault() }, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        }
        public System.Web.Mvc.JsonResult ExistsRegistro(int Registro, string key)
        {

            var result = _registroData.ExistsRegistro(Registro, key);

            return Json(new { success = true, result = result }, System.Web.Mvc.JsonRequestBehavior.AllowGet);

        }
        public System.Web.Mvc.JsonResult SaveRegistro(int Registro, string key)
        {

            var result = _registroData.SaveRegistro(Registro, key);

            return Json(new { success = true, result = result }, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        }
        public System.Web.Mvc.JsonResult GetRegistros(string key)
        {

            var result = _registroData.GetRegistroBykey(key);
            return Json(new { success = true, result = result }, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        }
        public System.Web.Mvc.JsonResult SendRegistro(string key, string email)
        {

            var resp = new EmailManagerHelper().SendMail("reportes@neew.mx", email, "Reporte de visitantes.", GetBodyEmail(key), String.Empty, null, string.Empty);


            return Json(new { success = true, result = !resp.Equals("false") }, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        }
        public System.Web.Mvc.PartialViewResult DescargarExcel(string key)
        {
            var result = _registroData.GetRegistroBykey(key);

            var excel = createExportAuditApp(result);

            setResponseFile(excel);

            return PartialView("_ExportWhiteListExcel", result);
        }

        private ExcelCustomHelper createExportAuditApp(List<RegistroDTO> caseList)
        {
            var grid = new GridView();
            var caseTable = new System.Data.DataTable("Case");
            caseTable.Columns.Add("Nombre", typeof(string));
            caseTable.Columns.Add("A. Paterno", typeof(string));
            caseTable.Columns.Add("A. Materno", typeof(string));
            caseTable.Columns.Add("Sexo", typeof(string));
            caseTable.Columns.Add("Edad", typeof(string));
            caseTable.Columns.Add("Email", typeof(string));
            caseTable.Columns.Add("Telefono", typeof(string));

            caseTable.Columns.Add("Empresa", typeof(string));
            caseTable.Columns.Add("Cargo", typeof(string));
            caseTable.Columns.Add("Ciudad", typeof(string));
            caseTable.Columns.Add("Pais", typeof(string));
            caseTable.Columns.Add("Perfil", typeof(string));
            caseTable.Columns.Add("Pase", typeof(string));

            //Case 
            //CreatedDate 
            //  SortCode 
            //  AccountNumber 
            //  CustomerName 
            //  Description 
            //  Status 


            foreach (var a in caseList)
            {
                caseTable.Rows.Add(a.Nombre, a.ApellidoPaterno, a.ApellidoMaterno, a.Sexo, a.Edad.Descripcion, a.Email, a.Telefono, a.Empresa, a.Cargo, a.Ciudad.Descripcion, a.Pais.Descripcion, a.StrPerfil, a.nombrePase);
            }

            grid.DataSource = caseTable;
            grid.DataBind();

            // grid style
            grid.RowStyle.Height = 25;
            int j = 0;
            foreach (var r in caseList)
            {
                for (int i = 0; i < caseTable.Columns.Count; i++)
                {
                    if (j == 0)
                    {
                        grid.HeaderRow.Cells[i].BackColor = ColorTranslator.FromHtml("#ec1C49");
                        grid.HeaderRow.Cells[i].ForeColor = Color.White;
                        grid.HeaderRow.Cells[i].Font.Bold = true;
                        grid.HeaderRow.Cells[i].HorizontalAlign = HorizontalAlign.Center;
                    }

                    grid.Rows[j].Cells[i].BackColor = j % 2 == 0 ? Color.White : ColorTranslator.FromHtml("#EEEEEE");
                    grid.Rows[j].Cells[i].ForeColor = Color.Black;


                }

                j++;
            }

            string filename = String.Format("Registros.xls", DateTime.Now.ToString("dd_MM_yyyy hh-mm-ss"));
            ExcelCustomHelper ex = new ExcelCustomHelper(grid, "Registros", "Registros", filename);

            return ex;
        }

        private string GetBodyEmail(string key)
        {

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
                        <h1>Lista de visitantes registrados.</h1>";

            body += @"<div class='contInfo'  align='center'>
                            <div class='InfoPago'  align='left'>            
                                <h3>Sede:</h3>
                                <ul class='datosSede'>
                                    <li>
                                           <span class='rojoTxt'><a href='http://grapesoft-001-site23.ctempurl.com/umbraco/surface/Validacion/DescargarExcel?key=" + key + @"'>Descagar archivo</a></span>
                                    </li>                                    
                                </ul>
                            </div>
                        </div>";

            body += @"<h2>No responda a ésta cuenta de correo, no está supervisada.</h2>";

            body += @"</body>";
            return body;

        }

        #endregion
    }
}