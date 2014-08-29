using System;
using System.Collections.Generic;
using Firebase;

namespace C4InventorySerialization.Content
{
    public partial class ScanOrder1 : System.Web.UI.Page
    {
        public string Token { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var tokenGenerator = new TokenGenerator("JtY774uVRKPCHdqO0YDZvW2pt2yJiHMvRVQ7Vf4S");
            var arbitraryAuthPayload = new Dictionary<string, object>
                {
                    { "some", "arbitrary" },
                    { "data", "here" }
                };
             Token = tokenGenerator.CreateToken(arbitraryAuthPayload);
        }
    }
}