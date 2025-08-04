namespace InvoiceInsightAPI.Models
{
    public class InvoiceData
    {
        public decimal TotalAmount { get; set; }

        public List<ExpenseDetail> Expenses { get; set; } = [];
    }
}
