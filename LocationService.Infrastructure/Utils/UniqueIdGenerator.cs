using UUIDNext;

namespace LocationService.Infrastructure.Utils;

public static class UniqueIdGenerator
{
    public static string GenerateSequentialId()
    {
        return Uuid.NewSequential().ToString();
    }
}