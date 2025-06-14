


using System;
using Microsoft.AspNetCore.Mvc;
using Plantagotchi.Dto;
using Plantagotchi.Models;
using System.Text.Json;
using System.IO;

namespace Plantagotchi.Controllers
{   // our top level route
    [ApiController]
    [Route("plant")]

    public class PlantController : ControllerBase
    {

        // [HttpGet("{id}")]
        // public Plant GetPlantById(Guid id)
        // {

        // }

        /*
            * This is how it works. The thing making the call to the post request
            makes a plant dto

            * it oasses the plant dto into the post request

            * the post request is responsible for instantiating the plant object

            * it also saves info in json and returns plant object
        */
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


            return Ok(newPlant);
        }
    }
}