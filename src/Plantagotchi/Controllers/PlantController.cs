


using System;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Plantagotchi.Models;

namespace Plantagotchi.Controllers
{   // our top level route
    [ApiController]
    [Route("plant")]

    public class PlantController
    {

        [HttpGet("{id}")]
        public Plant GetPlantById(Guid id)
        {
            
        }


        [HttpPost("{id}")] // i think??? do we need an id
        public ActionResult<Plant> CreatePlant(Plant plant)
        {
            // return CreatedAtActionResult(nameof(GetPlantById), new {id = plant.id}, plant);

            Plant newPlant = new Plant();
            return Ok(plant);
        }
    }
}