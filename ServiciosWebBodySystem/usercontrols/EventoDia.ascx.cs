using System;
using System.Collections.Generic;
using System.Configuration;
using umbraco.cms.businesslogic.media;
using umbraco.NodeFactory;

namespace ServiciosWebBodySystem.usercontrols
{
    public partial class EventoDia : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                try
                {

                    ltEventos.Text = AgregarRegiones(AgregarEventos());
                }
                catch (Exception ex)
                {
                    ltEventos.Text = ConfigurationManager.AppSettings["debugEmail"].Equals("1") ? (ex.Message + " - " + (ex.InnerException != null ? ex.InnerException.Message : string.Empty)) : string.Empty;
                }
            }

        }

        public List<string> AgregarEventos()
        {
            try
            {
                Node nodeCurrent = Node.GetCurrent();
                String response = String.Empty;


                var EventosLts = new List<string>();
                bool color = true;
                foreach (Node node in nodeCurrent.Children)
                {
                    var detalle = node.GetProperty("detalle") != null ? node.GetProperty("detalle").Value == "1" : false;

                    //mediaId is the ID returned from your document property...

                    String divCNT = string.Empty;
                    //detalle
                    if(detalle) divCNT += @"<a href ='" + node.NiceUrl + "'>";
                    divCNT += @"<div class='item-evento'>"
                                     + @"<div class='foto-evento'>
                                        <img  class='gallery-item imgGrayscale' src='" + (node.GetProperty("fotografia") != null ? node.GetProperty("fotografia").Value : "") + "' width='200px'>"
                                     + @"</div>";
                    if (node.GetProperty("imagenNat") != null)
                    {
                        string url = "/assets/flag.png";
                        Media file = new Media(Convert.ToInt32(node.GetProperty("imagenNat").Value));
                        url = file.getProperty("umbracoFile").Value.ToString();

                        divCNT += "<div class='bandera-expositor'>";
                        divCNT += "<img src='" + url + "'>";
                        divCNT += "</div>";
                    }
                    divCNT += @"<div class='info-evento clip-evento fuscia'>";


                    divCNT += @"<div class='arrow'>&nbsp;</div>
                                        <p class='hora'>" + (node.GetProperty("hora") != null ? node.GetProperty("hora").Value : "") + "</p>"
                    + @"<p class='expositor'>" + (node.GetProperty("nombre") != null ? node.GetProperty("nombre").Value : "") + "</p>"
                    + @"<p class='titulo'>" + node.Name + "</p>"
                    + @"</div>
                      </div>";
                    if(detalle) divCNT += @"</a>";
                    EventosLts.Add(divCNT);
                }

                return EventosLts;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public string AgregarRegiones(List<string> eventos)
        {

            String response = String.Empty;

            if (eventos.Count > 0)
            {
                response += @"<div id='renglon-1' class='csReglon'>";
                response += eventos[0];
                response += eventos.Count > 1 ? eventos[1].Replace("fuscia", "cyan") : "";
                response += eventos.Count > 2 ? eventos[2] : "";
                response += @"</div>";
            }

            if (eventos.Count > 3)
            {
                response += @"<div id='renglon-2' class='csReglon'>";
                response += eventos[3].Replace("fuscia", "cyan");
                response += eventos.Count > 4 ? eventos[4] : "";
                response += eventos.Count > 5 ? eventos[5].Replace("fuscia", "cyan") : "";
                response += @"</div>";
            }

            if (eventos.Count > 6)
            {
                var contador = 0;
                var contadorRenglon = 3;
                bool par = true;
                for (int i = 6; i < eventos.Count; i++)
                {
                    if (contador == 0)
                    {
                        response += @"<div id='renglon-" + contadorRenglon + "' class='csReglon'>";
                        contadorRenglon++;
                    }
                    if (par)
                    {
                        if (contador == 0 || contador == 2)
                        {
                            response += eventos[i].Replace("fuscia", "cyan"); ;
                        }
                        else
                        {
                            response += eventos[i];
                        }

                    }
                    else
                    {
                        if (contador == 1 || contador == 3)
                        {
                            response += eventos[i].Replace("fuscia", "cyan"); ;
                        }
                        else
                        {
                            response += eventos[i];
                        }
                    }

                    contador++;
                    if (contador == 4)
                    {
                        response += @"</div>";
                        contador = 0;
                        par = !par;
                    }
                }
            }




            return response;

        }
    }
}