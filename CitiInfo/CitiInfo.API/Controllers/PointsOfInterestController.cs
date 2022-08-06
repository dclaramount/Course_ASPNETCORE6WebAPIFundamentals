using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitiInfo.API.Models;
using CitiInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CitiInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly CitiesDataStore _citiesDataStore;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, CitiesDataStore citiesDataStore)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        } 

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            try
            {
                throw new ArgumentNullException();
                var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} wasnt found when accesing points of interest.");
                    return NotFound();
                }
                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex )
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);
                return StatusCode(500, "A problem happened while handling your request");
            }

        }

        // GET api/values/5
        [HttpGet("{pointofinterestid}", Name ="GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            //find point of itnerest
            var pointOfInterest = city.PointsOfInterest
                .FirstOrDefault(poi => poi.Id == pointOfInterestId);
            if (pointOfInterest==null)
            {
                return NotFound();
            }
            return Ok(pointOfInterest);
        }

        // POST api/values
        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestForCreatinoDto pointofInterest)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            //demo purposes - to be improved
            var maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointofInterest.Name,
                Description = pointofInterest.Description
            };
            city.PointsOfInterest.Add(finalPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, pointOfInterestId = finalPointOfInterest.Id }, finalPointOfInterest);
        }

        [HttpPut("{pointofinterestid}")]
        public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city==null)
            {
                return NotFound();
            }

            //find point of interest
            var pointofInterestFromStore = city.PointsOfInterest
                .FirstOrDefault(c => c.Id == pointOfInterestId);

            if (pointofInterestFromStore == null)
            {
                return NotFound();
            }

            pointofInterestFromStore.Name = pointOfInterest.Name;
            pointofInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{pointofinterestid}")]
        public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == pointOfInterestId);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{pointofinterestid}")]
        public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == pointOfInterestId);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }
            city.PointsOfInterest.Remove(pointOfInterestFromStore);
            _mailService.Send("Point of Interest Deleted," ,
                $"Point of Interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted");
            return NoContent();
        }
    }
}

