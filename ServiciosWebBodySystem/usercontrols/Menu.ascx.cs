using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.NodeFactory;

namespace ServiciosWebBodySystem.usercontrols
{
    public partial class Menu : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    creaMenu();
                }
                catch (Exception ex)
                {
                    Literal1.Text += ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                }
            }

        }

        private void creaMenu()
        {
            Node node = new Node(1191);
            ltMenu.Text = GetChildrens(node, false);

        }

        private string GetChildrens(Node nodo, bool submenutitle)
        {
            String response = String.Empty;

            foreach (Node a in nodo.Children)
            {
                if (a.NodeTypeAlias == "Itemmenu")
                {
                    Literal1.Text += a.Name + " - Itemmenu - ";


                    response += @"<li class='nav-item popover-menu'>"

                                    + @"<a class='nav-link'>" + a.Name + "</a>"

                                    + @"<div style='display: none;' class='menuinter'>";
                    if (a.ChildrenAsList.Count == 0)
                        response += @"<div class='neew-submenu subMenuMG subMenuCorto'>";
                    else
                        response += @"<div class='neew-submenu subMenuMG'>";

                    if (a.GetProperty("imagen") != null)
                    {
                        response += @"<div class='submenu-logo'>"
                                        //+ @"<a class='navbar-brand'>"
                                        + (a.GetProperty("url") == null ? "<a class='navbar-brand'>" : "<a class='navbar-brand' href=' " + a.GetProperty("url").Value + "' target='_blank'>")
                                            + @"<img src='" + a.GetProperty("imagen").Value + "'  style='vertical-align:middle;'>"
                                        + @"</a>"
                                    + @"</div>";
                    }

                    response += @"<div class='col'>
                                    <div class='row'>"
                                      + GetChildrens(a, true)
                                + @"</div>
                                 </div>"
                            + @"</div>"
                      + @"</div>"
                  + @"</li>";

                }
                else if (a.NodeTypeAlias == "Submenutitle")
                {
                    Literal1.Text += a.Name + " - Submenutitle - ";

                    response += @"<div class='col-md-12 menuNeew'>
                                    <div class='col submenu-block'>"
                                        + a.Name
                                        + @"<i class='fa fa-chevron-right' aria-hidden='true'></i>
                                    </div>
                                    <div class='sub-menu'>
                                        <ul>" + GetChildrens(a, false) + @"</ul>
                                    </div>
                                </div>";



                }
                else if (a.NodeTypeAlias == "Itemsubmenu")
                {
                    if (submenutitle)
                    {
                        response += @"<div class='col-md-12 menuNeew'>
                                    <div class='col submenu-block'>"
                                      + a.Name
                                      + @"<i class='fa fa-chevron-right' aria-hidden='true'></i>
                                    </div>
                                    <div class='sub-menu'>
                                        <ul>" + GetChildrens(a, false) + @"</ul>
                                    </div>
                                </div>";
                    }
                    else
                    {
                        Literal1.Text += a.Name + " - Itemsubmenu - false -";
                        response += GetChildrens(a, true);
                    }


                }
                else if (a.NodeTypeAlias == "Eventodia")
                {
                    Literal1.Text += a.Name + " - Eventodia - ";
                    response += @" <li>"
                                   + @" <a href='" + a.NiceUrl + "'>" + a.Name + "</a>"
                              + @"</li>";

                }
                else
                {
                    Literal1.Text += " - | No se reconoce el DocumentType| - ";
                }
            }
            return response;
        }

    }
}