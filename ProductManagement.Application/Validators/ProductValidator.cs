using FluentValidation;
using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Validators;

public class ProductValidator : AbstractValidator<ProductRequestDto>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("El nombre es obligatorio.");
        RuleFor(p => p.Description).NotEmpty().WithMessage("La descripción es obligatoria.");
        RuleFor(p => p.Price).GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");
        RuleFor(p => p.Stock).GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");
        RuleFor(p => p.CategoryId).GreaterThan(0).WithMessage("La categoría es obligatoria.");
    }
}
