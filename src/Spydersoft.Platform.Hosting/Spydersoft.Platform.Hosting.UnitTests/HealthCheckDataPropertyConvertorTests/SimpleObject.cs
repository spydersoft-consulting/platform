using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spydersoft.Platform.Hosting.UnitTests.HealthCheckDataPropertyConvertorTests;
public class SimpleObject
{
    public string Name { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.Now;

    public int Age { get; set; } = 0;
}