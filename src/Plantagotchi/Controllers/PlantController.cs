using System;
using Microsoft.AspNetCore.Mvc;
using Plantagotchi.Dto;
using Plantagotchi.Models;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;


namespace Plantagotchi.Controllers
{   // our top level route
    [ApiController]
    [Route("plant")]

    public class PlantController : ControllerBase
    {

        /*
            * This is how it works. The thing making the call to the post request
            makes a plant dto

            * it oasses the plant dto into the post request

            * the post request is responsible for instantiating the plant object

            * it also saves info in json and returns plant object
        */
        //Create
        [HttpPost]
        public ActionResult<Plant> CreatePlant(CreatePlantDto plantDto)
        {
            
            var newPlant = new Plant(plantDto.Name);
            List<Plant> plants;
            string path = "Data/plants.json";

            // if json is empty create a new list of Plant, append to it, serialize and write it to json
            try
            {
                plants = JsonSerializer.Deserialize<List<Plant>>(System.IO.File.ReadAllText(path));
            }
            catch (JsonException) // check if this catch works
            {
                plants = new List<Plant>();
            }

            plants.Add(newPlant);



            // NOTE TO SELF  this overwrites the whole json file
            // if you decide to add more plants in the future you will
            // have to deserialize this and store it
            string json = JsonSerializer.Serialize(plants);
            System.IO.File.WriteAllText(path, json);

            return Ok(newPlant); // return object?? or just an ok??
        }

        // TODO change this to work with multiple plants
        // Read
        [HttpGet("{id}")]
        public ActionResult<Plant> GetPlantById(Guid id)
        {
            string path = "Data/plants.json";
            List<Plant> plants = JsonSerializer.Deserialize<List<Plant>>(System.IO.File.ReadAllText(path)); // json deserizlied and stored as a List
            Plant userPlant = null;

            foreach (Plant plant in plants)
            {
                if (plant.Id == id)
                {
                    userPlant = plant;
                }
            }

            if (userPlant == null)
            {
                return NoContent(); // plant not found get out
            }

            // lets try this
            DateTime timeNow = DateTime.UtcNow;

            int newLastWatered = (int)Math.Floor((timeNow - userPlant.LastWatered).TotalMinutes);
            int newLastSunLight = (int)Math.Floor((timeNow - userPlant.LastSunLight).TotalMinutes);


            // prod numbers / both system neglects ramps up damage over time
            int gracePeriod = 1440; // 1 day in minutes
            int maxTime = 7200; // 5 days in minutes
            double baseValue = 0.8;

            double e1 = Math.Max(0, newLastWatered - gracePeriod);
            double e2 = Math.Max(0, newLastSunLight - gracePeriod);

            double damagePercent = ((e1 / maxTime) + baseValue) * ((e2 / maxTime) + baseValue) - (baseValue * baseValue);
            double health = Math.Max(0, 100 - (damagePercent * 100));

            //store health
            userPlant.Health = (int)Math.Floor(health);


            // I should return the plant itself??
            return Ok(userPlant);
        }


        // Update water
        [HttpPut("{id}/water")]
        public ActionResult<Plant> WaterPlant(Guid id)
        {
            string path = "Data/plants.json";
            List<Plant> plants = JsonSerializer.Deserialize<List<Plant>>(System.IO.File.ReadAllText(path));

            foreach (Plant plant in plants)
            {
                if (plant.Id == id)
                {
                    plant.LastWatered = DateTime.UtcNow;
                    string json = JsonSerializer.Serialize(plants); // back into json
                    System.IO.File.WriteAllText(path, json);
                    return Ok(plant);
                }
            }

            return NotFound();
        }

        //update sunlight
        [HttpPut("{id}/sunlight")]
        public ActionResult<Plant> GiveSunLight(Guid id)
        {
            string path = "Data/plants.json";
            List<Plant> plants = JsonSerializer.Deserialize<List<Plant>>(System.IO.File.ReadAllText(path));

            foreach (Plant plant in plants)
            {
                if (plant.Id == id)
                {
                    plant.LastSunLight = DateTime.UtcNow;
                    string json = JsonSerializer.Serialize(plants); // back into json
                    System.IO.File.WriteAllText(path, json);
                    return Ok(plant);
                }
            }

            return NotFound();
        }

        // for both imedietaly after a get request
        [HttpPut("{id}/fullUpdate")]
        public ActionResult<Plant> FullUpdate(Guid id, Plant mainPlant)
        {
            string path = "Data/plants.json";
            List<Plant> plants = JsonSerializer.Deserialize<List<Plant>>(System.IO.File.ReadAllText(path));

            foreach (Plant plant in plants)
            {
                if (plant.Id == id)
                {
                    plant.LastSunLight = mainPlant.LastSunLight;
                    plant.LastWatered = mainPlant.LastWatered;
                    plant.Health = mainPlant.Health;
                    string json = JsonSerializer.Serialize(plants); // back into json
                    System.IO.File.WriteAllText(path, json);
                    return Ok(plant);
                }
            }

            return NotFound();
        }

        // Delete
        [HttpDelete("{id}")]
        public IActionResult DeletePlant(Guid id) //IactionResult used when you are just returning a status code
        {
            string path = "Data/plants.json";
            List<Plant> plants = JsonSerializer.Deserialize<List<Plant>>(System.IO.File.ReadAllText(path)); // this will return an array with lists inside, one loop needed [{}{}{}]

            foreach (Plant plant in plants)
            {
                if (plant.Id == id)
                {
                    plants.Remove(plant); // remove from array
                    string json = JsonSerializer.Serialize(plants); // back into json
                    System.IO.File.WriteAllText(path, json); // overwrite file 
                    return NoContent(); // server fufilled request but nothing to return
                }
            }
            return NotFound(); // this is a 404
        }

    }
}