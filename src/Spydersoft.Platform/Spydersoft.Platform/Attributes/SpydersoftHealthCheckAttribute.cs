using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spydersoft.Platform.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SpydersoftHealthCheckAttribute : Attribute
    {
        public SpydersoftHealthCheckAttribute(string name, HealthStatus failureStatus, string tags = "")
        {
            Name = name;
            FailureStatus = failureStatus;
            RawTags = tags;
            Tags = tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }
        public string Name { get; }

        public HealthStatus? FailureStatus { get; }

        public string RawTags { get; }

        public string[] Tags { get; }
    }
}
