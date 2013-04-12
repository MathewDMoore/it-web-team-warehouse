using System;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Management;
using System.Web.Services;
using System.Web.UI;
using Obout.Grid;
using System.Security.Principal;


namespace C4InventorySerialization.Content
{
    public partial class History : Page
    {
        private string _serverLocation = ConfigurationManager.AppSettings["ServerLocation"];

        private static readonly string ConnStr =
            ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;

        private string _searchParameter;
        protected void Page_Load()
        {
            if (!Page.IsPostBack)
            {
                var queryString = HttpUtility.ParseQueryString(Request.Url.Query);

                Search(queryString);
            }
        }

        public void Search(NameValueCollection searchParameter)
        {


            using (var sConn = new SqlConnection(ConnStr))
            {
                sConn.Open();
                var searchValue = string.Empty;
                SqlCommand sCmd = null;
                if (!string.IsNullOrEmpty(searchParameter["DeliveryNumber"]))
                {
                    sCmd = new SqlCommand("sp_Locate_History_by_DeliveryNumber", sConn);
                    searchValue = searchParameter["DeliveryNumber"];
                }
                if (!string.IsNullOrEmpty(searchParameter["IRNumber"]))
                {
                    sCmd = new SqlCommand("sp_Locate_History_by_IrNumber", sConn);
                    searchValue = searchParameter["IRNumber"];
                }
                if (!string.IsNullOrEmpty(searchParameter["SmartCode"]))
                {
                    sCmd = new SqlCommand("sp_Locate_History_by_SmartCode", sConn);
                    searchValue = searchParameter["SmartCode"];
                }
                if (!string.IsNullOrEmpty(searchParameter["MacId"]))
                {
                    sCmd = new SqlCommand("sp_Locate_History_by_MacId", sConn);
                    searchValue = searchParameter["MacId"];
                }
                if (!string.IsNullOrEmpty(searchParameter["UserName"]))
                {
                    sCmd = new SqlCommand("sp_Locate_History_by_UserName", sConn);
                    searchValue = searchParameter["UserName"];
                }

                if (sCmd != null)
                {   
                    sCmd.CommandType = CommandType.StoredProcedure;
                    sCmd.Parameters.Add("@SEARCHPARAMETER", SqlDbType.NVarChar);
                    sCmd.Parameters["@SEARCHPARAMETER"].Value = searchValue;

                    try
                    {
                        using (IDataReader reader1 = sCmd.ExecuteReader())
                        {
                            grid1.DataSource = reader1;
                            grid1.DataBind();
                        }
                        sConn.Close();
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
        }
    }
}