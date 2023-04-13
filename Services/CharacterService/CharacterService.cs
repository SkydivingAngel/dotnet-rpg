using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.CharacterService.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper mapper;
        private static List<Character> characters = new List<Character>
        {
            new Character(),
            new Character{ Id= 1, Name = "Sam" }
        };

        public CharacterService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = mapper.Map<Character>(newCharacter);

            character.Id = characters.Max(c => c.Id) + 1;

            characters.Add(character);

            serviceResponse.Data = mapper.Map<List<GetCharacterDto>>(characters); // 1 Modo

            return serviceResponse;

            //return new ServiceResponse<List<Character>>{Data = characters};
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            serviceResponse.Data = characters.Select(c => mapper.Map<GetCharacterDto>(c)).ToList(); // 2 Modo

            return serviceResponse;

            //return new ServiceResponse<List<Character>>{Data = characters};
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

            var character = characters.FirstOrDefault(c => c.Id == id);

            serviceResponse.Data = mapper.Map<GetCharacterDto>(character);

            return serviceResponse;

            //if(character is not null)
                //return new ServiceResponse<Character>{Data = character};

            //throw new Exception("Character Not Found");
        }
    }
}