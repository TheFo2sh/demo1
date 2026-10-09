using System.Collections.Concurrent;

namespace DiscoveryService.Backend.Accommodations;

/// <summary>Accommodations received from providers, kept for the lifetime of the running process.</summary>
public interface IAccommodationStore
{
    void Add(Accommodation accommodation);
    IReadOnlyCollection<Accommodation> GetAll();
}

/// <summary>
/// Process-local, non-durable <see cref="IAccommodationStore"/>: entries are lost on restart and
/// not shared between instances. Safe for concurrent writers; stores and returns copies.
/// </summary>
public sealed class InMemoryAccommodationStore : IAccommodationStore
{
    private readonly ConcurrentQueue<Accommodation> _accommodations = new();

    public void Add(Accommodation accommodation)
    {
        ArgumentNullException.ThrowIfNull(accommodation);
        _accommodations.Enqueue(accommodation.Copy());
    }

    public IReadOnlyCollection<Accommodation> GetAll() =>
        _accommodations.Select(a => a.Copy()).ToList();
}
