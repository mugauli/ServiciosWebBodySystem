using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace ServiciosWebBodySystem.Helper
{
    public class ExcelCustomHelper
    {
        public string title { get; set; }
        public GridView objList { get; set; }
        public string Filename { get; set; }
        public string Name { get; set; }


        public ExcelCustomHelper(GridView _objList, string _title, string _Name, string _filename)
        {
            this.objList = _objList;
            this.title = _title;
            this.Filename = _filename;
            this.Name = _Name;
        }

    }
}