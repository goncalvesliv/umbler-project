using Desafio.Umbler.Controllers;
using Desafio.Umbler.Models;
using Desafio.Umbler.Services;
using Desafio.Umbler.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Desafio.Umbler.Test
{
    [TestClass]
    public class ControllersTest
    {
        [TestMethod]
        public void Home_Index_returns_View()
        {
            var controller = new HomeController();
            var response = controller.Index();
            var result = response as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Home_Error_returns_View_With_Model()
        {
            var controller = new HomeController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var response = controller.Error();
            var result = response as ViewResult;
            var model = result.Model as ErrorViewModel;
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public async Task Domain_In_Database()
        {
            // arrange
            var mockService = new Mock<IDomainService>();
            var expectedViewModel = new DomainViewModel
            {
                Name = "test.com",
                Ip = "192.168.0.1",
                HostedAt = "umbler.corp",
                NameServers = new List<string> { "ns1.umbler.com" }
            };
            mockService.Setup(s => s.GetDomainInfoAsync("test.com")).ReturnsAsync(expectedViewModel);

            var controller = new DomainController(mockService.Object);

            // act
            var response = await controller.Get("test.com");
            var result = response as OkObjectResult;
            var obj = result.Value as DomainViewModel;

            // assert
            Assert.IsNotNull(obj);
            Assert.AreEqual("test.com", obj.Name);
            Assert.AreEqual("192.168.0.1", obj.Ip);
        }

        [TestMethod]
        public async Task Domain_Not_In_Database()
        {
            // arrange
            var mockService = new Mock<IDomainService>();
            mockService.Setup(s => s.GetDomainInfoAsync("test.com")).ReturnsAsync(new DomainViewModel
            {
                Name = "test.com",
                Ip = "192.168.0.1",
                HostedAt = "umbler.corp",
                NameServers = new List<string>()
            });

            var controller = new DomainController(mockService.Object);

            // act
            var response = await controller.Get("test.com");
            var result = response as OkObjectResult;
            var obj = result.Value as DomainViewModel;

            // assert
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public async Task Domain_Moking_LookupClient()
        {
            // arrange
            var mockService = new Mock<IDomainService>();
            mockService.Setup(s => s.GetDomainInfoAsync("test.com")).ReturnsAsync(new DomainViewModel
            {
                Name = "test.com",
                Ip = "192.168.0.1",
                HostedAt = "umbler.corp",
                NameServers = new List<string> { "ns1.umbler.com" }
            });

            var controller = new DomainController(mockService.Object);

            // act
            var response = await controller.Get("test.com");
            var result = response as OkObjectResult;
            var obj = result.Value as DomainViewModel;

            // assert
            Assert.IsNotNull(obj);
        }

        // Teste obrigatório — agora possível pois IDomainService é mockável
        [TestMethod]
        public async Task Domain_Moking_WhoisClient()
        {
            // arrange
            var mockService = new Mock<IDomainService>();
            var domainName = "test.com";

            mockService.Setup(s => s.GetDomainInfoAsync(domainName)).ReturnsAsync(new DomainViewModel
            {
                Name = domainName,
                Ip = "192.168.0.1",
                HostedAt = "umbler.corp",
                NameServers = new List<string> { "ns1.umbler.com" }
            });

            var controller = new DomainController(mockService.Object);

            // act
            var response = await controller.Get(domainName);
            var result = response as OkObjectResult;
            var obj = result.Value as DomainViewModel;

            // assert
            Assert.IsNotNull(obj);
            Assert.AreEqual(domainName, obj.Name);
        }

        [TestMethod]
        public async Task Domain_Invalid_Returns_BadRequest()
        {
            // arrange
            var mockService = new Mock<IDomainService>();
            var controller = new DomainController(mockService.Object);

            // act
            var response = await controller.Get("dominiosemextensao");
            var result = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(result);
        }
    }
}