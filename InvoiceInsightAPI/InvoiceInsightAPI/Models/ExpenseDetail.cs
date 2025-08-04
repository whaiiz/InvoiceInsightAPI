namespace InvoiceInsightAPI.Models
{
    public class ExpenseDetail
    {
        public string Name { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public string UnitType { get; set; } = string.Empty;
    }
}
