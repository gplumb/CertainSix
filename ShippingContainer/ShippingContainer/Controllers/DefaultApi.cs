﻿/*
 * Shipping Container Spoilage
 *
 * An API that tracks spoilage in shipping containers due to over heating for periods of time.
 *
 * OpenAPI spec version: 1.0.0
 * 
 * Generated by: https://openapi-generator.tech
 */

using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

using ShippingContainer.Attributes;
using ShippingContainer.ViewModels;
using ShippingContainer.Interfaces;
using System.Linq;
using ShippingContainer.Helpers;

namespace ShippingContainer.Controllers
{
    /// <summary>
    /// DefaultApiController
    /// </summary>
    public class DefaultApiController : Controller
    {
        readonly IShippingRepository _repo;


        /// <summary>
        /// Constructor
        /// </summary>
        public DefaultApiController(IShippingRepository repository)
        {
            _repo = repository;
        }


        /// <summary>
        /// Adds a container record to a trip.
        /// </summary>
        /// <param name="tripId">The trip id.</param>
        /// <param name="containerCreationDetails">The container resource to create.</param>
        /// <response code="201">Successful container creation.</response>
        /// <response code="400">Invalid creation details.</response>
        /// <response code="404">Trip not found</response>
        [HttpPost]
        [Route("/trips/{tripId}/containers")]
        [ValidateModelState]
        public virtual IActionResult CreateContainer([FromRoute][Required]string tripId, [FromBody]ContainerCreationDetails containerCreationDetails)
        {
            var trip = GetTripById(tripId);

            if (trip == null)
            {
                return StatusCode(404);
            }

            // Has the container already been created?
            var container = _repo.Containers.FirstOrDefault(x => x.ContainerId.Equals(containerCreationDetails.Id));

            if (container != null)
            {
                return StatusCode(400);
            }

            // No, then let's create it
            var model = new Models.Container()
            {
                ContainerId = containerCreationDetails.Id,
                TripId = trip.Id,
                ProductCount = containerCreationDetails.ProductCount,

                // Use projection to convert from view model to DTO
                // Note. We don't need to populate ContainerID as EF will take care of referential integrity for us
                Temperatures = containerCreationDetails.Measurements.Select(tr => new Models.TemperatureRecord()
                {
                    Time = tr.Time,
                    Value = tr.Value,
                    TripId = trip.Id
                }).ToList()
            };

            model.IsSpoiled = SpoilageHelpers.IsSpoiled(model.Temperatures, trip.SpoilTemperature, trip.SpoilDuration);

            // TODO: Work out spoilage counts now and Maximum
            // TODO: Work out mean now or later?

            trip.Updated = DateTime.UtcNow;
            _repo.Containers.Add(model);
            _repo.SaveChanges();
            return StatusCode(201);
        }


        /// <summary>
        /// Attempt to fetch a trip for the given id
        /// </summary>
        private Models.Trip GetTripById(string tripId)
        {
            // We're using numbers in our datastore, so convert the parameter into an integer
            int tid = 0;
            if (!int.TryParse(tripId, out tid))
            {
                return null;
            }

            return _repo.Trips.FirstOrDefault(x => x.Id.Equals(tid));
        }


        /// <summary>
        /// Create a new trip.
        /// </summary>
        /// <param name="tripCreationDetails">The trip resource to create.</param>
        /// <response code="201">Successful trip creation.</response>
        /// <response code="400">Invalid creation details.</response>
        [HttpPost]
        [Route("/trips")]
        [ValidateModelState]
        public virtual IActionResult CreateTrip([FromBody]TripCreationDetails tripCreationDetails)
        {
            var trip = _repo.Trips.FirstOrDefault(x => x.Name.Equals(tripCreationDetails.Name));

            if (trip != null)
            {
                return StatusCode(400);
            }

            trip = new Models.Trip()
            {
                Name = tripCreationDetails.Name,
                SpoilDuration = tripCreationDetails.SpoilDuration,
                SpoilTemperature = tripCreationDetails.SpoilTemperature
            };

            _repo.Trips.Add(trip);
            _repo.SaveChanges();

            Response.Headers.Add("Location", new Microsoft.Extensions.Primitives.StringValues($"/trips/{trip.Id}"));
            return StatusCode(201);
        }


        /// <summary>
        /// Gets a trip by id.
        /// </summary>
        /// <param name="tripId">The id of the trip to return.</param>
        /// <response code="200">Successful operation</response>
        /// <response code="404">Trip not found</response>
        [HttpGet]
        [Route("/trips/{tripId}")]
        [ValidateModelState]
        public virtual IActionResult TripsTripIdGet([FromRoute][Required]string tripId)
        {
            var trip = GetTripById(tripId);

            if (trip == null)
            {
                return StatusCode(404);
            }

            // First-cut, this can be calculated more efficiently
            float meanTemp = _repo.TemperatureRecords.Where(x => x.TripId.Equals(trip.Id)).Average(r => r.Value);
            float maxTemp = _repo.TemperatureRecords.Where(x => x.TripId.Equals(trip.Id)).Max(r => r.Value);
            float containerCount = _repo.Containers.Where(x => x.TripId == trip.Id).Count();

            // TODO: ETag header for resource version
            // TODO: spoiled container count
            // TODO: spoiled product count

            var result = new ViewModels.Trip()
            {
                Id = tripId,
                ContainerCount = containerCount,
                MaxTemperature = maxTemp,
                MeanTemperature = meanTemp
            };

            return new ObjectResult(result);
        }
    }
}
