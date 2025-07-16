using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Plantagotchi.Models;
using Plantagotchi.Dto;

namespace Plantagotchi.Services
{
    public class PlantService
    {
        private readonly string _filePath = "Data/plants.json";

        // Create a new plant and save it to json
        public Plant CreatePlant(string name)
        {
            var newPlant = new Plant(name);
            List<Plant> plants = LoadPlantsFromFile();

            if (plants.Count >= 3)
            {
                return null; // return something else later, no more than 3 plants
            }

            plants.Add(newPlant);
            SavePlantsToFile(plants);

            return newPlant; // return the new plant object
        }

        // Get all plants
        public List<Plant> GetAllPlants()
        {
            // need to autocalulcate health
            List<Plant> plants = LoadPlantsFromFile();

            if (plants.Count == 0) // no plants to show
            {
                return plants; // empty list
            }

            foreach (Plant plant in plants)
            {
                CalculateAndUpdateHealth(plant); // update plants health
            }


            SavePlantsToFile(plants);
            return plants;

        }

        // Get a plant by ID and calculate its current health
        public Plant GetPlantById(Guid id)
        {
            List<Plant> plants = LoadPlantsFromFile();
            Plant userPlant = null;

            // find the plant were looking for
            foreach (Plant plant in plants)
            {
                if (plant.Id == id)
                {
                    userPlant = plant;
                    break; // found it, get out of here
                }
            }

            if (userPlant == null)
            {
                return null; // plant not found, controller will handle the 404
            }

            //calculate health based on time since last care
            CalculateAndUpdateHealth(userPlant);

            //save the updated health back to file
            SavePlantsToFile(plants);

            return userPlant;
        }

        // Water a plant (updates LastWatered timestamp)
        public Plant WaterPlant(Guid id)
        {
            List<Plant> plants = LoadPlantsFromFile();

            foreach (Plant plant in plants)
            {
                if (plant.Id == id)
                {
                    if (!plant.IsAlive)
                    {
                        return plant; // plant is dead
                    }
                    
                    plant.LastWatered = DateTime.UtcNow;
                    SavePlantsToFile(plants); // save changes back to json
                    return plant; // return the updated plant
                }
            }

            return null; // plant not found, controller will handle 404
        }

        // Give sunlight to a plant (updates ---> LastSunLight timestamp)
        public Plant GiveSunlight(Guid id)
        {
            List<Plant> plants = LoadPlantsFromFile();

            foreach (Plant plant in plants)
            {
                if (plant.Id == id)
                {

                    if (!plant.IsAlive)
                    {
                        return plant; // plant is dead
                    }
                    plant.LastSunLight = DateTime.UtcNow;
                    SavePlantsToFile(plants); // save changes back to json
                    return plant; //return the updated plant
                }
            }

            return null; //plant not found, controller will handle 404
        }

        // Full update for both water and sunlight (used right after a get request)
        public Plant FullUpdate(Guid id, Plant updatedPlant)
        {
            List<Plant> plants = LoadPlantsFromFile();

            foreach (Plant plant in plants)
            {
                if (plant.Id == id)
                {
                    // update the timestamps and health from the provided plant object
                    plant.LastSunLight = updatedPlant.LastSunLight;
                    plant.LastWatered = updatedPlant.LastWatered;
                    plant.Health = updatedPlant.Health;
                    plant.IsAlive = updatedPlant.IsAlive; // also update alive status
                    
                    SavePlantsToFile(plants); // save changes back to json
                    return plant; // return the updated plant
                }
            }

            return null; // plant not found, controller will handle 404
        }

        // Delete a plant from the json file
        public bool DeletePlant(Guid id)
        {
            List<Plant> plants = LoadPlantsFromFile();

            foreach (Plant plant in plants)
            {
                if (plant.Id == id)
                {
                    plants.Remove(plant); // remove from array
                    SavePlantsToFile(plants); // overwrite file with updated list
                    return true; // successfully deleted
                }
            }

            return false; // plant not found, nothing to delete
        }

        // Private helper methods these do the actual file operations

        // Load all plants from the json file
        private List<Plant> LoadPlantsFromFile()
        {
            try
            {
                return JsonSerializer.Deserialize<List<Plant>>(File.ReadAllText(_filePath));
            }
            catch (JsonException)
            {
                // if json is corrupted or empty, return empty list
                return new List<Plant>();
            }
            catch (FileNotFoundException)
            {
                // if file doesn't exist, return empty list
                return new List<Plant>();
            }
        }

        // Save all plants back to the json file
        private void SavePlantsToFile(List<Plant> plants)
        {
            string json = JsonSerializer.Serialize(plants);
            File.WriteAllText(_filePath, json);
        }

        // Calculate health based on time since last water/sunlight
        private void CalculateAndUpdateHealth(Plant plant)
        {
            // lets try this
            DateTime timeNow = DateTime.UtcNow;

            int newLastWatered = (int)Math.Floor((timeNow - plant.LastWatered).TotalMinutes);
            int newLastSunLight = (int)Math.Floor((timeNow - plant.LastSunLight).TotalMinutes);

            // prod numbers / both system neglects ramps up damage over time
            int gracePeriod = 1440; // 1 day in minutes
            int maxTime = 7200; // 5 days in minutes
            double baseValue = 0.8;

            double e1 = Math.Max(0, newLastWatered - gracePeriod);
            double e2 = Math.Max(0, newLastSunLight - gracePeriod);

            double damagePercent = ((e1 / maxTime) + baseValue) * ((e2 / maxTime) + baseValue) - (baseValue * baseValue);
            double health = Math.Max(0, 100 - (damagePercent * 100));

            // store health and update alive status
            plant.Health = (int)Math.Floor(health);
            plant.IsAlive = plant.Health > 0; // plant dies if health reaches 0
        }
    }
}