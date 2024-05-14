using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class CityRepository : ICityRepository
    {
        protected readonly ITrybeHotelContext _context;
        public CityRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 4. Refatore o endpoint GET /city
        public IEnumerable<CityDto> GetCities()
        {
            return _context.Cities.Select(c => new CityDto
            {
                cityId = c.CityId,
                name = c.Name
            }).ToList();
        }

        // 2. Refatore o endpoint POST /city
        public CityDto AddCity(City city)
        {
            try
            {
                _context.Cities.Add(city);
                _context.SaveChanges();
                var newCity = _context.Cities.First(c => c.CityId == city.CityId);
                return new CityDto {
                    cityId = newCity.CityId,
                    name = newCity.Name
                };
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        // 3. Desenvolva o endpoint PUT /city
        public CityDto UpdateCity(City city)
        {
            throw new NotImplementedException();
        }

    }
}