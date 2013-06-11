using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace C4InventorySerialization.Content
{
    public partial class DuplicateMacReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CreateGrid1();
        }

        protected void RebindGrid(object sender, EventArgs e)
        {
            CreateGrid1();
        }

        private void CreateGrid1()
        {

        }

    }
}