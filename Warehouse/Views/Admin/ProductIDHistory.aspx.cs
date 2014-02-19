using System;
using System.Linq;

namespace C4InventorySerialization.Admin
{
    public partial class ProductIDHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CreateGrid();
            }
        }

        protected void RebindGrid(object sender, EventArgs e)
        {
            CreateGrid();
        }

        protected void CreateGrid()
        {
            var db = new SerializationDataContext();
            var query = from p in db.MPI_Histories
                        select new
                                   {
                                       p.DATE,
                                       p.TYPE,
                                       p.PRODUCTID,
                                       p.ITEMCODE,
                                       p.COLOR,
                                       p.SMARTCODEONLY,
                                       p.NOSERIALIZATION,
                                       p.USERNAME,
                                   };

           // PIHGrid.DataSource = query;
//            PIHGrid.DataBind();
        }
    }
}
