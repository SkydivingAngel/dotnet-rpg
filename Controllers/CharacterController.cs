using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        //private static Character knight = new Character();
        
        private static List<Character> characters = new List<Character>
        {
            new Character(),
            new Character{ Name = "Sam" }
        };

        [HttpGet("GetAll")]
        public ActionResult<List<Character>> GetAll()
        {
            return Ok(characters);
        }

        [HttpGet("GetSingle")]
        public ActionResult<Character> GetSingle()
        {
            return Ok(characters[0]);
        }
    }
}