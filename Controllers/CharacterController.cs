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
        private ICharacterService characterService;

        public CharacterController(ICharacterService characterService)
        {
            this.characterService = characterService;
        }

        [HttpGet("GetAll")]
        public ActionResult<List<Character>> GetAll()
        {
            return Ok(characterService.GetAllCharacters());
        }

        [HttpGet("GetSingle")]
        public ActionResult<Character> GetSingle(int id)
        {
            return Ok(characterService.GetCharacterById(id));
        }

        
        [HttpPost("AddCharacter")]
        public ActionResult<List<Character>> AddCharacter(Character newCharacter)
        {
            return Ok(characterService.AddCharacter(newCharacter));
        }
    }
}