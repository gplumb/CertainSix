using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using Xunit;

using ShippingContainer.Controllers;
using ShippingContainer.Interfaces;

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

            // Test the result
            var result = controller.CreateTrip(data) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);

            // Check response headers
            Assert.True(controller.ControllerContext.HttpContext.Response.Headers.ContainsKey("location"));

            var location = controller.ControllerContext.HttpContext.Response.Headers["location"].ToString();
            var format = location.StartsWith("/trips/");
            var tripId = 0;

            Assert.True(format);
            Assert.True(int.TryParse(location.Replace("/trips/", ""), out tripId));
            Assert.True(tripId > 0);

            // Ensure we can't replay
            controller = InitController("POST");
            result = controller.CreateTrip(data) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }
    }
}
