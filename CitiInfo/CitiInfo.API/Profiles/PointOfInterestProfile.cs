﻿using System;
using AutoMapper;

namespace CitiInfo.API.Profiles
{
    public class PointOfInterestProfile :Profile
    {
        public PointOfInterestProfile()
        {
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
            CreateMap<Models.PointOfInterestForCreatinoDto, Entities.PointOfInterest>();
        }
    }
}
