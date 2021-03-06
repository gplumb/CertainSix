﻿/*
 * Shipping Container Spoilage
 *
 * An API that tracks spoilage in shipping containers due to over heating for periods of time.
 *
 * OpenAPI spec version: 1.0.0
 * 
 * Generated by: https://openapi-generator.tech
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using ShippingContainer.Attributes;
using ShippingContainer.Helpers;
using ShippingContainer.Interfaces;
using ShippingContainer.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
            model.MaxTemperature = model.Temperatures.Max(x => x.Value);

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

            // The mean could be stored at container level and calculated here with a smaller number of data points, but
            // there is the potential for rounding issues, so I have favoured runtime accuracy here
            float meanTemp = 0;
            float maxTemp = 0;

            double containerCount = _repo.Containers.Where(x => x.TripId == trip.Id).Count();
            double spoiledContainerCount = 0;
            double spoiledProductCount = 0;

            if (containerCount > 0)
            {
                // These could be stored on their respective objects if performance of these queries were to become an issue
                // Note. If that is the case, ensure that all update queries use the .Updated field to avoid lost DB updates
                meanTemp = _repo.TemperatureRecords.Where(x => x.TripId.Equals(trip.Id)).Average(r => r.Value);

                maxTemp = _repo.Containers.Max(x => x.MaxTemperature);
                spoiledContainerCount = _repo.Containers.Where(x => x.TripId == trip.Id && x.IsSpoiled == true).Count();
                spoiledProductCount = _repo.Containers.Where(x => x.TripId == trip.Id && x.IsSpoiled == true).Select(x => x.ProductCount).Sum();
            }

            var result = new ViewModels.Trip()
            {
                Id = tripId,
                ContainerCount = containerCount,
                MaxTemperature = maxTemp,
                MeanTemperature = meanTemp,
                SpoiledContainerCount = spoiledContainerCount,
                SpoiledProductCount = spoiledProductCount
            };

            // .Updated timestamp is as good as anything to use here...
            var resourceVersion = Math.Abs(trip.Updated.GetHashCode());
            Response.Headers.Add("ETag", new StringValues(resourceVersion.ToString()));

            // Prepare controller output
            var output = new ObjectResult(result);

            // Explicitly add the status code as a concession to unit testing this action outside of MVC
            output.StatusCode = 200;
            return output;
        }
    }
}
