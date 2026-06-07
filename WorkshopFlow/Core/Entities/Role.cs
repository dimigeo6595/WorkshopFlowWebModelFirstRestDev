using System;
using System.Collections.Generic;

namespace WorkshopFlow.Core.Entities;

public class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<User> Users { get; set; } = new HashSet<User>();

    public ICollection<Capability> Capabilities { get; set; } = new HashSet<Capability>();
}
