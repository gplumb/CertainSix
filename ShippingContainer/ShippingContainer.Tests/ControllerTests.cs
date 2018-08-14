using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShippingContainer.Controllers;
using ShippingContainer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace ShippingContainer.Tests
{
    /// <summary>
    /// DefaultApi Controller Tests
    /// </summary>
    public class ControllerTests
    {
        private IServiceProvider _provider;


        /// <summary>
        /// Constructor (test setup)
        /// </summary>
        public ControllerTests()
        {
            // Allow us to resolve dependencies outside of WebAPI/MVC for testing
            var services = new ServiceCollection();

            // Use in-memory EF provider
            services.AddDbContext<ShippingRepository>(options => options.UseInMemoryDatabase("ShippingContainer"));
            services.AddTransient<IShippingRepository, ShippingRepository>();
            _provider = services.BuildServiceProvider();
        }


        /// <summary>
        /// Mimick DI in a similar fashion to MVC and obtain a controller instance
        /// </summary>
        private DefaultApiController InitController(string method = "GET")
        {
            IShippingRepository repo = _provider.GetRequiredService<IShippingRepository>();
            var controller = new DefaultApiController(repo);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.HttpContext.Request.Method = method;
            return controller;
        }


        /// <summary>
        /// Exercises CreateTrip model validation
        /// </summary>
        [Fact]
        public void CreateTripValidation()
        {
            var controller = InitController("POST");

            // Invalid .Name and .SpoilDuration
            var data = new ViewModels.TripCreationDetails()
            {
                Name = null,
                SpoilDuration = 0,
                SpoilTemperature = 0,
            };

            controller.ValidateViewModel(data);
            Assert.Equal(2, controller.ModelState.ErrorCount);

            // .SpoilDuration is still invalid at this point
            data.Name = "Valid";
            controller.ValidateViewModel(data);
            Assert.Equal(1, controller.ModelState.ErrorCount);

            // All data is valid now
            data.SpoilDuration = 1;
            controller.ValidateViewModel(data);
            Assert.Equal(0, controller.ModelState.ErrorCount);
        }


        /// <summary>
        /// Exercises CreateTrip logic
        /// </summary>
        [Fact]
        public void CreateTripLogic()
        {
            // Positive test case
            var controller = InitController("POST");
            var data = new ViewModels.TripCreationDetails()
            {
                Name = "TestTrip",
                SpoilDuration = 5,
                SpoilTemperature = 10,
            };

            var tripId = CreateTrip(controller, data);

            // Ensure we can't replay
            controller = InitController("POST");
            var result = controller.CreateTrip(data) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }


        /// <summary>
        /// Helper that creates and validates a trip (returning its ID)
        /// </summary>
        private int CreateTrip(DefaultApiController controller, ViewModels.TripCreationDetails details)
        {
            // Test the result
            var result = controller.CreateTrip(details) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);

            // Check response headers
            Assert.True(controller.ControllerContext.HttpContext.Response.Headers.ContainsKey("location"));

            var location = controller.ControllerContext.HttpContext.Response.Headers["location"].ToString();
            var format = location.StartsWith("/trips/");
            var tripId = 0;

            Assert.True(format);
            Assert.True(int.TryParse(location.Replace("/trips/", ""), out tripId));
            Assert.True(tripId > 0);
            return tripId;
        }


        /// <summary>
        /// Exercises CreateContainer model validation
        /// </summary>
        [Fact]
        public void CreateContainerValidation()
        {
            var controller = InitController("POST");

            // Invalid payload
            var data = new ViewModels.ContainerCreationDetails()
            {
                Id = null,
                Measurements = null,
                ProductCount = -1
            };

            controller.ValidateViewModel(data);
            Assert.Equal(3, controller.ModelState.ErrorCount);

            // Invalid .Measurements and .ProductCount
            data.Id = "Valid1";
            controller.ValidateViewModel(data);
            Assert.Equal(2, controller.ModelState.ErrorCount);

            // Invalid .ProductCount
            data.Measurements = new List<ViewModels.TemperatureRecord>();
            controller.ValidateViewModel(data);
            Assert.Equal(1, controller.ModelState.ErrorCount);

            // Valid payload
            data.ProductCount = 10;
            controller.ValidateViewModel(data);
            Assert.Equal(0, controller.ModelState.ErrorCount);
        }


        /// <summary>
        /// Exercises CreateContainer logic
        /// </summary>
        [Fact]
        public void CreateContainerLogic()
        {
            // Positive test case (and tests)
            var tripId = CreateTripWithContainers("AnotherTestTrip");

            // Try and create a trip with bad trip ids 
            var data = new ViewModels.ContainerCreationDetails()
            {
                Id = "container1",
                Measurements = new List<ViewModels.TemperatureRecord>()
                {
                    new ViewModels.TemperatureRecord() { Time = DateTime.Now, Value =0 },
                    new ViewModels.TemperatureRecord() { Time = DateTime.Now.AddSeconds(1), Value = 1 },
                    new ViewModels.TemperatureRecord() { Time = DateTime.Now.AddSeconds(2), Value = 3 },
                },
                ProductCount = 10
            };

            var controller = InitController("POST");
            var result = controller.CreateContainer("I am not a number", data) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);

            controller = InitController("POST");
            result = controller.CreateContainer("100", data) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }


        /// <summary>
        /// Exercises CreateContainer logic
        /// </summary>
        [Fact]
        public void GetTripLogic()
        {
            var tripId = CreateTripWithContainers("FinalTrip").ToString();
            var controller = InitController("GET");

            // Positive test case
            var result = controller.TripsTripIdGet(tripId) as ObjectResult;
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);

            // Test expected output
            var trip = result.Value as ViewModels.Trip;
            Assert.Equal(tripId, trip.Id);
            Assert.Equal(2, trip.ContainerCount);
            Assert.Equal(30, trip.MaxTemperature);
            Assert.Equal(10.666666667f, trip.MeanTemperature);
            Assert.Equal(1, trip.SpoiledContainerCount);
            Assert.Equal(20, trip.SpoiledProductCount);

            // Fetch a non existent trip
            var badResult = controller.TripsTripIdGet("xxx") as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.NotFound, badResult.StatusCode);
        }


        /// <summary>
        /// Helper that creates and validates a trip, complete with containers (returning its ID)
        /// </summary>
        private int CreateTripWithContainers(string tripName)
        {
            var controller = InitController("POST");
            var tripId = CreateTrip(controller, new ViewModels.TripCreationDetails() { Name = tripName, SpoilDuration = 1, SpoilTemperature = 10, });
            var dateTime = DateTime.UtcNow;

            // Check that the trip's .Updated timestamp is updated by the controller
            // after we have added a container to it (tests field used for ETag functionality in GET /trip/x)
            IShippingRepository repo = _provider.GetRequiredService<IShippingRepository>();
            var trip = repo.Trips.SingleOrDefault(x => x.Id.Equals(tripId));
            Assert.NotNull(trip);
            var updatedTimeStamp = trip.Updated;

            // Create a container
            var data = new ViewModels.ContainerCreationDetails()
            {
                Id = "container1",
                Measurements = new List<ViewModels.TemperatureRecord>()
                {
                    new ViewModels.TemperatureRecord() { Time = DateTime.Now, Value =0 },
                    new ViewModels.TemperatureRecord() { Time = DateTime.Now.AddSeconds(1), Value = 1 },
                    new ViewModels.TemperatureRecord() { Time = DateTime.Now.AddSeconds(2), Value = 3 },
                },
                ProductCount = 10
            };

            // Test the result
            var result = controller.CreateContainer(tripId.ToString(), data) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);

            // Ensure we can't replay
            controller = InitController("POST");
            result = controller.CreateContainer(tripId.ToString(), data) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);

            // Ensure that .Updated is newer
            // Note: We need to re-initialize the repo at this point to avoid caching)
            repo = _provider.GetRequiredService<IShippingRepository>();
            trip = repo.Trips.SingleOrDefault(x => x.Id.Equals(tripId));
            Assert.NotNull(trip);
            Assert.True(trip.Updated > updatedTimeStamp);

            // Add another container to this trip
            data = new ViewModels.ContainerCreationDetails()
            {
                Id = "container2",
                Measurements = new List<ViewModels.TemperatureRecord>()
                {
                    new ViewModels.TemperatureRecord() { Time = DateTime.Now.AddSeconds(2), Value = 30 },
                    new ViewModels.TemperatureRecord() { Time = DateTime.Now, Value = 10 },
                    new ViewModels.TemperatureRecord() { Time = DateTime.Now.AddSeconds(1), Value = 20 },
                },
                ProductCount = 20
            };

            result = controller.CreateContainer(tripId.ToString(), data) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
            return tripId;
        }
    }
}
