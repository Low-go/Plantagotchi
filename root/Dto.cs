using System;


namespace Plantagotchi.Dto
{

    public record PlantDto(Guid Id, string Name, DateTime CreatedAt, DateTime LastWatered, DateTime LastSunLight, int Health);
    public record CreatePlantDto(string Name);
}