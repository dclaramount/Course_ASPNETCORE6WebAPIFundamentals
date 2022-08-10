using System;
using CitiInfo.API.Entities;

namespace CitiInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<City?> GetCityASync(int cityId, bool includePointsOfInterest);
        Task<IEnumerable<PointOfInterest>> GetPointOfInterestsAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
    }
}

