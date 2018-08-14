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
            //TODO: Uncomment the next line to return response 201 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(201);

            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);


            throw new NotImplementedException();
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
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(Trip));

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

            string exampleJson = null;
            exampleJson = "{\r\n  \"maxTemperature\" : 6.0274563,\r\n  \"meanTemperature\" : 1.4658129,\r\n  \"spoiledContainerCount\" : 5.962133916683182377482808078639209270477294921875,\r\n  \"id\" : \"id\",\r\n  \"spoiledProductCount\" : 5.63737665663332876420099637471139430999755859375,\r\n  \"containerCount\" : 0.80082819046101150206595775671303272247314453125\r\n}";

            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<Trip>(exampleJson)
            : default(Trip);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }
    }
}
