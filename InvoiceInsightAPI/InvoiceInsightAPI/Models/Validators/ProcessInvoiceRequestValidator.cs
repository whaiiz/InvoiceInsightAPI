using FluentValidation;
using InvoiceInsightAPI.Models.Request;

namespace InvoiceInsightAPI.Models.Validators
{
    public class ProcessInvoiceRequestValidator : AbstractValidator<ProcessInvoiceRequest>
    {   public ProcessInvoiceRequestValidator()
        {
            RuleFor(x => x.FileName)
                .NotNull()
                .WithMessage("FileName is required.")
                .Must(fileName => fileName.Length > 0)
                .WithMessage("File cannot be empty.");
            
            RuleFor(x => x.ContentType)
                .NotNull()
                .WithMessage("ContentType is required.")
                .Must(contentType => contentType.Length > 0)
                .WithMessage("ContentType cannot be empty.")
                .Must(contentType => contentType is "image/jpeg" or "image/png")
                .WithMessage("File must be a  JPEG, or PNG.");

            RuleFor(x => x.Base64Content)
                .NotNull()
                .WithMessage("Base64Content is required.")
                .Must(file => file.Length > 0)
                .WithMessage("Base64Content cannot be empty.");
        }
    }
}
