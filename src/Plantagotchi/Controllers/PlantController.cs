using System;
using Microsoft.AspNetCore.Mvc;
using Plantagotchi.Dto;
using Plantagotchi.Models;
using System.Text.Json;
using System.Collections.Generic;


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
            // return CreatedAtActionResult(nameof(GetPlantById), new {id = plant.id}, plant);

            var newPlant = new Plant(plantDto.Name);
            var jsonPayLoad = JsonSerializer.Serialize(newPlant);
            Console.WriteLine(jsonPayLoad);


            // NOTE TO SELF  this overwrites the whole json file
            // if you decide to add more plants in the future you will
            // have to deserialize this and store it
            string path = "Data/plants.json";
            System.IO.File.WriteAllText(path, jsonPayLoad);


            return Ok(newPlant); // return object?? or just an ok??
        }

        // TODO change this to work with multiple plants
        // Read
        [HttpGet("{id}")]
        public ActionResult<Plant> GetPlantById(Guid id)
        {
            string path = "Data/plants.json";
            Plant userPlant = JsonSerializer.Deserialize<Plant>(System.IO.File.ReadAllText(path)); // json deserizlied and stored as a class/object

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


        // Update placeholder 
        //
        //

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