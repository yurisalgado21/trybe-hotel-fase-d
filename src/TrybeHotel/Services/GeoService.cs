using System.Net.Http;
using TrybeHotel.Dto;
using TrybeHotel.Repository;

namespace TrybeHotel.Services
{
    public class GeoService : IGeoService
    {
        private readonly HttpClient _client;
        public GeoService(HttpClient client)
        {
            _client = client;
        }

        // 11. Desenvolva o endpoint GET /geo/status
        public async Task<object> GetGeoStatus()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://nominatim.openstreetmap.org/status.php?format=json");
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Headers.Add("User-Agent", "aspnet-user-agent");

            var response = await _client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<object>();
                return result.Result!;
            }

            return default(Object)!;
        }
        
        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<GeoDtoResponse> GetGeoLocation(GeoDto geoDto)
        {
            var url = $"https://nominatim.openstreetmap.org/search?street={geoDto.Address}&city={geoDto.City}&country=Brazil&state={geoDto.State}&format=json&limit=1";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Headers.Add("User-Agent", "aspnet-user-agent");

            var response = await _client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<List<GeoDtoResponse>>();
                return new GeoDtoResponse{
                    lat = result.Result![0].lat,
                    lon = result.Result![0].lon
                };
            }

            return default(GeoDtoResponse)!;
        }

        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<List<GeoDtoHotelResponse>> GetHotelsByGeo(GeoDto geoDto, IHotelRepository repository)
        {
            try
            {
                var geoResponseLocation = await GetGeoLocation(geoDto);
                var allHotels = repository.GetHotels();
                var hotelsResponse = new List<GeoDtoHotelResponse>();

                foreach (var item in allHotels)
                {
                    var hotelDto = new GeoDto {
                        Address = item.address,
                        City = item.cityName,
                        State = item.state
                        };
                    var hotelLocation = await GetGeoLocation(hotelDto);

                    var distance = CalculateDistance(geoResponseLocation.lat!, geoResponseLocation.lon!, hotelLocation.lat!, hotelLocation.lon!);

                    var hotelResponse = new GeoDtoHotelResponse {
                        HotelId = item.hotelId,
                        Name = item.name,
                        Address = item.address,
                        CityName = item.cityName,
                        State = item.state,
                        Distance = distance
                    };

                    hotelsResponse.Add(hotelResponse);
                }

                return hotelsResponse;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public int CalculateDistance (string latitudeOrigin, string longitudeOrigin, string latitudeDestiny, string longitudeDestiny) {
            double latOrigin = double.Parse(latitudeOrigin.Replace('.',','));
            double lonOrigin = double.Parse(longitudeOrigin.Replace('.',','));
            double latDestiny = double.Parse(latitudeDestiny.Replace('.',','));
            double lonDestiny = double.Parse(longitudeDestiny.Replace('.',','));
            double R = 6371;
            double dLat = radiano(latDestiny - latOrigin);
            double dLon = radiano(lonDestiny - lonOrigin);
            double a = Math.Sin(dLat/2) * Math.Sin(dLat/2) + Math.Cos(radiano(latOrigin)) * Math.Cos(radiano(latDestiny)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
            double distance = R * c;
            return int.Parse(Math.Round(distance,0).ToString());
        }

        public double radiano(double degree) {
            return degree * Math.PI / 180;
        }

    }
}