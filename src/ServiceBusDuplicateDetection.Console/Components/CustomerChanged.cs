using System;

namespace ServiceBusDuplicateDetection.ConsoleSample.Components
{
    // ReSharper disable once InconsistentNaming
    internal interface CustomerChanged
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}
