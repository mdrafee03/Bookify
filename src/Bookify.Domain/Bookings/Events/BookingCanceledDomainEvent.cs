using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Bookings.Events;

public sealed record BookingCanceledDomainEvent(Guid BookingId) : IDomainEvent;
