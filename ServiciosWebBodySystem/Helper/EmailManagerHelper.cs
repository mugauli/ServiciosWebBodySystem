using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using umbraco.NodeFactory;

namespace ServiciosWebBodySystem.Helper
{
    public class EmailManagerHelper
    {
        public EmailManagerHelper()
        {

        }

        /// <summary>
        /// Send mail without attachment
        /// </summary>
        /// <param name="From"> Remitente </param>
        /// <param name="To"> Destinatario </param>
        /// <param name="Subject"> Asunto </param>
        /// <param name="Body"> Cuerpo del correo </param>
        /// <param name="Attach"> URL documento adjunto </param>
        /// <returns></returns>
        public string SendMail(String From, String To, String Subject, String Body, String Attach, Byte[] imagenInterna, string path)
        {
            String Debug = string.Empty;
            string result = "false";
            try
            {
                String smtpServer = String.Empty;
                Int16 smtpPort = 0;
                Boolean smtpEnableSsl = false;
                Boolean smtpDefaultCredencials = false;
                String smtpUser = String.Empty;
                String smtpPass = String.Empty;

              
                #region Get Configurations
                if (ConfigurationManager.AppSettings["configuracionUmbraco"].Equals("1"))
                {
                    Node node = new Node(1050);

                    smtpServer = (node.GetProperty("servidor") != null ? node.GetProperty("servidor").Value : String.Empty);
                    smtpPort = Convert.ToInt16((node.GetProperty("puerto") != null ? node.GetProperty("puerto").Value : "0"));
                    smtpEnableSsl = (node.GetProperty("habilitarSsl") != null ? node.GetProperty("habilitarSsl").Value : "0").Equals("1");

                    smtpDefaultCredencials = (node.GetProperty("usarCredencialesPorDefault") != null ? node.GetProperty("usarCredencialesPorDefault").Value : "0").Equals("1");
                    smtpUser = smtpDefaultCredencials ? "" : (node.GetProperty("usuarioDeServidor") != null ? node.GetProperty("usuarioDeServidor").Value : string.Empty);
                    smtpPass = smtpDefaultCredencials ? "" : (node.GetProperty("passwordServidor") != null ? node.GetProperty("passwordServidor").Value : string.Empty);

                    Debug += "server:" + smtpServer + " - Port: " + smtpPort + " - SSL: " + smtpEnableSsl + " - SSLumbraco:" + node.GetProperty("habilitarSsl").Value + " - User:" + smtpUser + " - Pass:" + smtpPass;
                }
                else
                {


                    smtpServer = ConfigurationManager.AppSettings["smtpServer"];
                    smtpPort = Convert.ToInt16(ConfigurationManager.AppSettings["smtpPort"]);
                    smtpEnableSsl = ConfigurationManager.AppSettings["smtpEnableSsl"].Equals("1");

                    smtpDefaultCredencials = ConfigurationManager.AppSettings["smtpDefaultCredencials"].Equals("1");
                    smtpUser = smtpDefaultCredencials ? "" : ConfigurationManager.AppSettings["smtpUser"];
                    smtpPass = smtpDefaultCredencials ? "" : ConfigurationManager.AppSettings["smtpPass"];
                }
                #endregion

                MailMessage message = new MailMessage(From, To, Subject, Body);
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;

                if (Attach != "")
                {
                    Attachment File = new Attachment(Attach);
                    message.Attachments.Add(File);
                }

                #region Imagenes Inline

                var contentID = "Logo";

                if (!String.IsNullOrEmpty(path))
                {
                    var inlineLogo = new Attachment(path + @"\assets\logo4irhsa_color.png");
                    inlineLogo.ContentId = contentID;
                    inlineLogo.ContentDisposition.Inline = true;
                    inlineLogo.ContentDisposition.DispositionType = DispositionTypeNames.Inline;

                    message.IsBodyHtml = true;
                    message.Attachments.Add(inlineLogo);

                    if (imagenInterna != null)
                    {

                        MemoryStream streamBitmap = new MemoryStream(imagenInterna);
                        var imageToInline = new LinkedResource(streamBitmap, MediaTypeNames.Image.Jpeg);
                        imageToInline.ContentId = "MyImage";

                        AlternateView alternateView = AlternateView.CreateAlternateViewFromString(Body, null, System.Net.Mime.MediaTypeNames.Text.Html);

                        alternateView.LinkedResources.Add(imageToInline);
                        message.AlternateViews.Add(alternateView);


                    }
                }
                #endregion

                SmtpClient emailClient = new SmtpClient(smtpServer, smtpPort);
                emailClient.EnableSsl = smtpEnableSsl;


                if (!smtpDefaultCredencials)
                {
                    System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential(smtpUser, smtpPass);
                    emailClient.Credentials = SMTPUserInfo;
                }
                else
                {
                    emailClient.UseDefaultCredentials = smtpDefaultCredencials;
                }

                emailClient.Send(message);

                result = "true";
            }
            catch (Exception ex)
            {
                if (ConfigurationManager.AppSettings["debugEmail"].Equals("1"))
                {
                    result = ex.Message + "Debug:" + Debug;
                }
                else result = ex.Message;

                result += ex.InnerException != null ? ex.InnerException.Message : string.Empty; 
            }
            return result;
        }

        public static string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", string.Empty); sbText.Replace(" ", string.Empty);
            return sbText.ToString();
        }
    }
}