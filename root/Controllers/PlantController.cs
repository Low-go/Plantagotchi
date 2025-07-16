using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Plantagotchi.Dto;
using Plantagotchi.Models;
using Plantagotchi.Services;

namespace Plantagotchi.Controllers
{   
    // our top level route
    [ApiController]
    [Route("plant")]
    public class PlantController : ControllerBase
    {
        private readonly PlantService _plantService;

        // constructor to set up the service
        public PlantController()
        {
            _plantService = new PlantService();
        }

        /*
            * This is how it works. The thing making the call to the post request
            makes a plant dto

            * it passes the plant dto into the post request

            * the post request is responsible for instantiating the plant object

            * it also saves info in json and returns plant object
        */
        //Create
        [HttpPost]
        public ActionResult<Plant> CreatePlant(CreatePlantDto plantDto)
        {
            var newPlant = _plantService.CreatePlant(plantDto.Name);
            if (newPlant == null)
            {
                return BadRequest("Garden is full! Maximum 3 plants allowed.");
            }

            return Ok(newPlant); // return object?? or just an ok??
        }

        // read all of them
        [HttpGet]
        public ActionResult<List<Plant>> GetAllPlants()
        {
            var plants = _plantService.GetAllPlants();
            return Ok(plants);
        }


        // Read by id
        [HttpGet("{id}")]
        public ActionResult<Plant> GetPlantById(Guid id)
        {
            var userPlant = _plantService.GetPlantById(id);
            
            if (userPlant == null)
            {
                return NoContent(); // plant not found get out
            }

            // I should return the plant itself??
            return Ok(userPlant);
        }

        // Update water
        [HttpPut("{id}/water")]
        public ActionResult<Plant> WaterPlant(Guid id)
        {
            var plant = _plantService.WaterPlant(id);
            
            if (plant == null)
            {
                return NotFound();
            }

            if (!plant.IsAlive)
            {
                return BadRequest("Cannot Provide Water to Dead Plant!");
            }
            
            return Ok(plant);
        }

        //update sunlight
        [HttpPut("{id}/sunlight")]
        public ActionResult<Plant> GiveSunLight(Guid id)
        {
            var plant = _plantService.GiveSunlight(id);
            
            if (plant == null)
            {
                return NotFound();
            }

            if (!plant.IsAlive)
            {
                return BadRequest("Cannot Provide Sunlight to Dead Plant!");
            }
            
            return Ok(plant);
        }

        // for both immediately after a get request
        [HttpPut("{id}/fullUpdate")]
        public ActionResult<Plant> FullUpdate(Guid id, Plant mainPlant)
        {
            var plant = _plantService.FullUpdate(id, mainPlant);
            
            if (plant == null)
            {
                return NotFound();
            }
            
            return Ok(plant);
        }

        // Delete
        [HttpDelete("{id}")]
        public IActionResult DeletePlant(Guid id) //IactionResult used when you are just returning a status code
        {
            bool deleted = _plantService.DeletePlant(id);
            
            if (deleted)
            {
                return NoContent(); // server fulfilled request but nothing to return
            }
            
            return NotFound(); // this is a 404
        }
    }
}