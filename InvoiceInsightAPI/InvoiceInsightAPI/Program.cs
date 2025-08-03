using InvoiceInsightAPI.Handlers;
using InvoiceInsightAPI.Handlers.Interfaces;
using InvoiceInsightAPI.Models.Request;
using InvoiceInsightAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProcessInvoiceHandler, ProcessInvoiceHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("ProcessInvoice", async (
        IProcessInvoiceHandler processInvoiceHandler, 
        [FromBody] ProcessInvoiceRequest req) =>
{ 
    var result = await processInvoiceHandler.ProcessInvoiceAsync(req);
    return result;
})
.Produces<ApiResponse<ProcessInvoiceResponse>>(200)
.Produces<ApiResponse<ProcessInvoiceResponse>>(400)
.Produces<ApiResponse<ProcessInvoiceResponse>>(500);

app.UseHttpsRedirection();
app.Run();
