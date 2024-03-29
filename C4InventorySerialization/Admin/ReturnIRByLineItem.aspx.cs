﻿using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace C4InventorySerialization.Admin
{
    public partial class ReturnIRByLineItem : System.Web.UI.Page
    {
        /*
                int CountDOCNUM = 0;
        */
        string LineNum;
        private string Username;

        public void ProcessReturn()
        {

            if (Context.Request.QueryString["LineNum"] != null)
            {

                LineNum = Request.QueryString["LineNum"];
                Username = User.Identity.Name;
                string connStr = ConfigurationManager.ConnectionStrings["InventoryConnectionString"].ConnectionString;
                var qString = LineNum.Split(' ').Where(x => !string.IsNullOrEmpty(x));

                qString.ToList().ForEach(s =>
                    {

                    using (SqlConnection sConn = new SqlConnection(connStr))
                    {
                        sConn.Open();
                        SqlCommand sCmd = new SqlCommand("sp_ReturnIRByLineItem", sConn);
                        sCmd.CommandType = CommandType.StoredProcedure;
                        sCmd.Parameters.Add("@ID", SqlDbType.Int);
                        sCmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
                        sCmd.Parameters["@USERNAME"].Value = Username;
                        sCmd.Parameters["@ID"].Value = s;
                        IDataReader reader1 = sCmd.ExecuteReader();
                    }
                    }
                        );
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                ProcessReturn();
            }

        }
    }

}

