
using System;
using System.Data.Common;

namespace Plantagotchi.Models
{
    public class Plant
    {
        //variables with getters and setters
        public Guid Id { get; set; }
        public String Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastWatered { get; set; }
        public DateTime LastSunLight { get; set;}
        public int Health { get; set;}
        public bool IsAlive { get; set;}
        // constructor
        public Plant(String name)
        {
            Id = Guid.NewGuid();
            Name = name;
            CreatedAt = DateTime.UtcNow;
            LastWatered = DateTime.UtcNow;
            LastSunLight = DateTime.UtcNow;
            Health = 100;
            IsAlive = true;
        }
    }
}