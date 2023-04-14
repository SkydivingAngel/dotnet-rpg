namespace dotnet_rpg.CharacterService.Services
{
    public class CharacterService : ICharacterService
    {
		private readonly DataContext context;
        private readonly IMapper mapper;
		
        public CharacterService(IMapper mapper, DataContext context)
        {
            this.mapper = mapper;
			this.context = context;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = mapper.Map<Character>(newCharacter);

            context.Characters.Add(character);
			await context.SaveChangesAsync();
			
            //serviceResponse.Data = mapper.Map<List<GetCharacterDto>>(characters); // 1 Modo
			serviceResponse.Data = await context.Characters.Select(c => mapper.Map<GetCharacterDto>(c)).ToListAsync(); // 2 Modo
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
			var dbCharacters = await context.Characters.ToListAsync();
			
            serviceResponse.Data = dbCharacters.Select(c => mapper.Map<GetCharacterDto>(c)).ToList(); // 2 Modo

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

			var dbCharacter = await context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            serviceResponse.Data = mapper.Map<GetCharacterDto>(dbCharacter);

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

            // var character = characters.FirstOrDefault(c => c.Id == updateCharacter.Id);

            // if(character is not null)
            // {
            //     character.Name = updateCharacter.Name;
            //     character.HitPoints = updateCharacter.HitPoints;
            //     character.Strength = updateCharacter.Strength;
            //     character.Defense = updateCharacter.Defense;
            //     character.Intelligence = updateCharacter.Intelligence;
            //     character.Class = updateCharacter.Class;

            //     serviceResponse.Data = mapper.Map<GetCharacterDto>(character); // 1 Modo          
            // }
            // else
            // {
            //     serviceResponse.Success = false;
            //     serviceResponse.Message = $"Character with Id '{updateCharacter.Id}' Not Found!";
            // }

            // Oppure try catch

            try
            {
                //var character = characters.FirstOrDefault(c => c.Id == updateCharacter.Id);
                var dbCharacter = await context.Characters.FirstOrDefaultAsync(c => c.Id == updateCharacter.Id);
				
				
                if(dbCharacter is null)
                    throw new Exception($"Character with Id '{updateCharacter.Id}' Not Found!");

                //mapper.Map(updateCharacter, dbCharacter);

                // Oppure

                dbCharacter.Name = updateCharacter.Name;
                dbCharacter.HitPoints = updateCharacter.HitPoints;
                dbCharacter.Strength = updateCharacter.Strength;
                dbCharacter.Defense = updateCharacter.Defense;
                dbCharacter.Intelligence = updateCharacter.Intelligence;
                dbCharacter.Class = updateCharacter.Class;

				await context.SaveChangesAsync();
				
                serviceResponse.Data = mapper.Map<GetCharacterDto>(dbCharacter); 
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                var dbCharacter = await context.Characters.FirstOrDefaultAsync(c => c.Id == id);
				
                if(dbCharacter is null)
                    throw new Exception($"Character with Id '{id}' Not Found!");

                context.Characters.Remove(dbCharacter);
				await context.SaveChangesAsync();
				
                serviceResponse.Data = await context.Characters.Select(c => mapper.Map<GetCharacterDto>(c)).ToListAsync(); // 2 Modo 
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
    }
}