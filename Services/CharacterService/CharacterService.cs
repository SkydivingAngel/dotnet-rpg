using System.Security.Claims;

namespace dotnet_rpg.CharacterService.Services
{
    public class CharacterService : ICharacterService
    {
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly DataContext context;
        private readonly IMapper mapper;

		private int GetUserId() => int.Parse(httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

		public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
            this.mapper = mapper;
        }
		
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = mapper.Map<Character>(newCharacter);

            character.User = await context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

            // Altro Modo
            // var character = new Character();
            // mapper.Map(newCharacter, character);

            context.Characters.Add(character);
			await context.SaveChangesAsync();
			
            //serviceResponse.Data = mapper.Map<List<GetCharacterDto>>(await context.Characters.ToListAsync()); // 1 Modo
			serviceResponse.Data = await context.Characters
            .Where(c => c.User!.Id == GetUserId())
            .Select(c => mapper.Map<GetCharacterDto>(c)).ToListAsync(); // 2 Modo
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
			var dbCharacters = await context.Characters
            .Where(c => c.User!.Id == GetUserId())
            .ToListAsync();
			
            serviceResponse.Data = dbCharacters.Select(c => mapper.Map<GetCharacterDto>(c)).ToList();

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

			var dbCharacter = await context.Characters
            .FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());

            if(dbCharacter is null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Character with Id '{id}' Not Found!"; 
            }
            else
            {
                serviceResponse.Data = mapper.Map<GetCharacterDto>(dbCharacter);
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

            var dbCharacter = await context.Characters
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == updateCharacter.Id);

            // if(dbCharacter is not null)
            // {
            //     dbCharacter.Name = updateCharacter.Name;
            //     dbCharacter.HitPoints = updateCharacter.HitPoints;
            //     dbCharacter.Strength = updateCharacter.Strength;
            //     dbCharacter.Defense = updateCharacter.Defense;
            //     dbCharacter.Intelligence = updateCharacter.Intelligence;
            //     dbCharacter.Class = updateCharacter.Class;

            //     await context.SaveChangesAsync();
            //     serviceResponse.Data = mapper.Map<GetCharacterDto>(dbCharacter);


            // }
            // else
            // {
            //     serviceResponse.Success = false;
            //     serviceResponse.Message = $"Character with Id '{updateCharacter.Id}' Not Found!";
            // }

            // Oppure try catch

            try
            {
                if(dbCharacter is null || dbCharacter.User!.Id != GetUserId())
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
                var dbCharacter = await context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());
				
                if(dbCharacter is null)
                    throw new Exception($"Character with Id '{id}' Not Found!");

                context.Characters.Remove(dbCharacter);
				await context.SaveChangesAsync();
				
                serviceResponse.Data = await context.Characters
                .Where(c => c.User!.Id == GetUserId())
                .Select(c => mapper.Map<GetCharacterDto>(c))
                .ToListAsync(); // 2 Modo 
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