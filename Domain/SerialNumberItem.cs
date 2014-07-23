using System;

namespace Domain
{
    public class SerialNumberItem
    {
        public int Id { get; set; }
        public string DocType { get; set; }
        public DateTime Date { get; set; }
        public int DocNum { get; set; }
        public int SerialNum { get; set; }
        public int DocEntry { get; set; }
        public DateTime DocDate { get; set; }
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string AltText { get; set; }
        public int Quantity { get; set; }
        public string ProductId { get; set; }
        public string Color { get; set; }
        public bool NoSerialization { get; set; }
        public bool SmartCodeOnly { get; set; }
        public string SerialCode { get; set; }
        public bool Verified { get; set; }
        public string Username { get; set; }
        public string MacId { get; set; }
        public string RealItemCode { get; set; }
        public string ProductGroup { get; set; }
        public string ProductFamily { get; set; }
        public string ProductPillar { get; set; }
        public string ReturnedByUser { get; set; }
	}
}