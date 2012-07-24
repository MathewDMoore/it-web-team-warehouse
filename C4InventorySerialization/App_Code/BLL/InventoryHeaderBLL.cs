using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using C4SQL01TableAdapters;

[System.ComponentModel.DataObject]
public class InventoryHeaderBLL
{
    /*
    private InventoryTableAdapter _inventoryAdapter = null;
    protected InventoryTableAdapter Adapter
    {
        get
        {
            if (_inventoryAdapter == null)
                _inventoryAdapter = new InventoryTableAdapter();

            return _inventoryAdapter;
        }
    }

    [System.ComponentModel.DataObjectMethodAttribute
        (System.ComponentModel.DataObjectMethodType.Select, true)]
    public C4SQL01.InventoryDataTable GetInventory()
    {
        return Adapter.GetInventoryData();
    }

    [System.ComponentModel.DataObjectMethodAttribute
    (System.ComponentModel.DataObjectMethodType.Select, false)]
    public C4SQL01.InventoryDataTable GetDataByInventoryID(int InventoryID)
    {
        return Adapter.GetDataByInventoryID(InventoryID);
    }

    [System.ComponentModel.DataObjectMethodAttribute
    (System.ComponentModel.DataObjectMethodType.Update, true)]
    public bool UpdateInventory(int original_InventoryID, int Year)
    {
        C4SQL01.InventoryDataTable inventoryTable = Adapter.GetDataByInventoryID(original_InventoryID);
        if (inventoryTable.Count == 0)
            // no matching record found, return false
            return false;

        C4SQL01.InventoryRow invRow = inventoryTable[0];

        invRow.Year = Year;

        // Update the product record
        int rowsAffected = Adapter.Update(invRow);

        // Return true if precisely one row was updated,
        // otherwise false
        return rowsAffected == 1;
    }



    [System.ComponentModel.DataObjectMethodAttribute
        (System.ComponentModel.DataObjectMethodType.Insert, true)]
    public bool AddProduct(int Year)
    {
        // Create a new ProductRow instance
        C4SQL01.InventoryDataTable inventoryTable = new C4SQL01.InventoryDataTable();
        C4SQL01.InventoryRow invRow = inventoryTable.NewInventoryRow();

        invRow.Year = Year;

        // Add the new product
        inventoryTable.AddInventoryRow(invRow);
        int rowsAffected = Adapter.Update(invRow);

        // Return true if precisely one row was inserted,
        // otherwise false
        return rowsAffected == 1;
    }


    [System.ComponentModel.DataObjectMethodAttribute
    (System.ComponentModel.DataObjectMethodType.Delete, true)]
    public bool DeleteInventory(int original_InventoryID)
    {
        int rowsAffected = Adapter.Delete(original_InventoryID);

        // Return true if precisely one row was deleted,
        // otherwise false
        return rowsAffected == 1;
    }
    */

}