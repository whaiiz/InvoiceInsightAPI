using System.Net;

namespace InvoiceInsightAPI.Models.Response
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }

        public T? Result { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public List<string> ErrorMessages { get; set; } = [];
    }
}