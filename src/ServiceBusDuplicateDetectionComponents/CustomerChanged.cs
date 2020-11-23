using System;

namespace ServiceBusDuplicateDetectionComponents
{
    // ReSharper disable once InconsistentNaming
    public interface CustomerChanged
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}
