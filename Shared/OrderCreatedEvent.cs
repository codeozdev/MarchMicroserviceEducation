using System.Collections.Immutable;

namespace Shared;

public record OrderCreatedEvent(string OrderCode, string UserId, ImmutableList<OrderItem> Items);

public record OrderItem(string ProductCode, int Quantity, decimal Price);