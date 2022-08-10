using System;
using CitiInfo.API.Models;
using CitiInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CitiInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities()
        {
            var cityEntities = await _cityInfoRepository.GetCitiesAsync();
            var result = new List<CityWithoutPointsOfInterestDto>();
            foreach(var cityEntity in cityEntities)
            {
                result.Add(new CityWithoutPointsOfInterestDto()
                {
                    Id = cityEntity.Id,
                    Description = cityEntity.Description,
                    Name = cityEntity.Name
                });
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCity(int id)
        {
            //var cityToReturn = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == id);
            //if(cityToReturn == null)
            //{
            //    return NotFound();
            //}
            return Ok();
        }
    }
}

