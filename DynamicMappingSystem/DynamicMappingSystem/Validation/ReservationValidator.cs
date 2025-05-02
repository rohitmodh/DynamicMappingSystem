using DynamicMappingSystem.DataModels;
using FluentValidation;
using System.Reflection;

namespace DynamicMappingSystem.Validation
{
    public class ReservationValidator : AbstractValidator<Reservation>
    {
        public ReservationValidator()
        {
            RuleFor(r => r.Id).NotEmpty();
            RuleFor(r => r.CheckInDate).NotEqual(default(DateTime));
            RuleFor(r => r.CheckOutDate).GreaterThan(r => r.CheckInDate)
                .WithMessage("Checkout date must be after check-in date.");
        }
    }
}
