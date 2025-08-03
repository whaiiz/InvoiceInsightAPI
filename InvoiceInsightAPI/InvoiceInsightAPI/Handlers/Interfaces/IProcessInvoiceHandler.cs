using InvoiceInsightAPI.Models.Request;
using InvoiceInsightAPI.Models.Response;

namespace InvoiceInsightAPI.Handlers.Interfaces
{
    public interface IProcessInvoiceHandler
    {
        Task<ApiResponse<ProcessInvoiceResponse>> ProcessInvoiceAsync(ProcessInvoiceRequest req);
    }
}
