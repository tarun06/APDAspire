using APDAspire.ContactServer.Controllers;
using APDAspire.Contact;
using APDAspire.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APDAspire.ContactTest
{
    public static class ConfigGenerator
    {
        public static List<ContactModel> GetAllContact()
        {
            return new List<ContactModel>()
            {
                new ContactModel(){FirstName="Tarun", LastName="kalal",
                    DOB = new DateTime(1995,3,6),
                    Contact_Id =Guid.NewGuid(),
                    EmailId = new List<string>(){"tarun.mewara06@gmail.com"}
                , PhoneNumber = new List<string>(){"+919738114549"}},

                new ContactModel(){FirstName="Mark", LastName="Smith",
                    DOB = new DateTime(2003,3,6),
                    Contact_Id =Guid.NewGuid(),
                    EmailId = new List<string>(){ "Mark.Smith@gmail.com" }
                , PhoneNumber = new List<string>(){"+449755777"} }

               };
        }

        public static List<ContactModel> GetContactByName(string firstName)
        {
           return GetAllContact().Where(x => x.FirstName.Contains(firstName)).ToList();
        }
    }
    

    [TestClass]
    public class ContactControllerTest
    {
        private Mock<IContact> mockContactStore;
        private ContactController contactController;

        [TestInitialize]
        public void ContactControllerTest_Init()
        {
            this.mockContactStore = new Mock<IContact>();
            this.contactController = new ContactController(this.mockContactStore.Object);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContactControllerTest_Init_NULL_Failed_Test()
        {
            this.contactController = new ContactController(null);
        }

        [TestMethod]
        public async Task ContactControllerTest_GetAll_Test()
        {
            var contacts = ConfigGenerator.GetAllContact();

            this.mockContactStore.Setup(x => x.GetAll()).ReturnsAsync(contacts);
            var result = await contactController.GetAll() as OkObjectResult;

            var outputCollection = result.Value as List<ContactModel>;

            CollectionAssert.AreEqual(contacts, outputCollection);
        }

        [TestMethod]
        public async Task ContactControllerTest_GetAll_Failed_Test()
        {
            string exceptionMessage = "Get ALL Error!";

            this.mockContactStore.Setup(x => x.GetAll()).Throws(new Exception(exceptionMessage));

            var response = await contactController.GetAll() as BadRequestResult;

            Assert.IsInstanceOfType(response, typeof(ActionResult));
            Assert.AreEqual(response.StatusCode , 400);
        }


        [TestMethod]
        public async Task ContactControllerTest_Update_Null_Failed_Test()
        {
            string exceptionMessage = "Update error!";

            this.mockContactStore.Setup(x => x.Update(null)).Throws(new Exception(exceptionMessage));

            var response = await contactController.Update(null) as BadRequestResult;

            Assert.IsInstanceOfType(response, typeof(ActionResult));
            Assert.AreEqual(response.StatusCode, 400);
        }

        [TestMethod]
        public async Task ContactControllerTest_Update_NullId_Failed_Test()
        {
            string exceptionMessage = "Contact error!";
            var contactModel = new ContactModel()
            {
                FirstName = "Mark",
                LastName = "Smith",
                DOB = new DateTime(2003, 3, 6),
                EmailId = new List<string>() { "Mark.Smith@gmail.com" },
                PhoneNumber = new List<string>() { "+449755777" }
            };

            this.mockContactStore.Setup(x => x.Update(contactModel)).Throws(new Exception(exceptionMessage));

            var response = await contactController.Update(contactModel) as BadRequestResult;

            Assert.IsInstanceOfType(response, typeof(ActionResult));
            Assert.AreEqual(response.StatusCode, 400);
        }

        [TestMethod]
        public async Task ContactControllerTest_Update_Succeed_Test()
        {
            Guid temp = Guid.NewGuid();
            var contactModel = new ContactModel()
            {
                FirstName = "Mark",
                LastName = "Smith",
                DOB = new DateTime(2003, 3, 6),
                Contact_Id = temp,
                EmailId = new List<string>() { "Mark.Smith@gmail.com" }
                 ,
                PhoneNumber = new List<string>() { "+449755777" }
            };

            this.mockContactStore.Setup(x => x.Update(contactModel)).ReturnsAsync(true);

            var response = await contactController.Update(contactModel) as OkObjectResult;
                 Assert.AreEqual(response.StatusCode, 200);
        }

        [TestMethod]
        public async Task ContactControllerTest_Create_Succeed_Test()
        {
            var contactModel = new ContactModel()
            {
                Contact_Id = Guid.NewGuid(),
                FirstName = "Mark",
                LastName = "Smith",
                DOB = new DateTime(2003, 3, 6),
                EmailId = new List<string>() { "Mark.Smith@gmail.com" },
                PhoneNumber = new List<string>() { "+449755777" }
            };

            this.mockContactStore.Setup(x => x.Create(contactModel)).ReturnsAsync(contactModel.Contact_Id);

            var response = await contactController.Add(contactModel) as OkObjectResult;
            Assert.AreEqual(response.StatusCode, 200);
            Assert.AreEqual(response.Value, contactModel.Contact_Id);

        }



        [TestMethod]
        public async Task ContactControllerTest_Create__Null_Failed_Test()
        {
            string exceptionMessage = "Create error!";

            this.mockContactStore.Setup(x => x.Create(null)).Throws(new Exception(exceptionMessage));

            var response = await contactController.Add(null) as BadRequestResult;

            Assert.IsInstanceOfType(response, typeof(ActionResult));
            Assert.AreEqual(response.StatusCode, 400);

        }

        [TestMethod]
        public async Task ContactControllerTest_Create_NullFirstName_Failed_Test()
        {
            string exceptionMessage = "First Name Not found error!";

            var contactModel = new ContactModel()
            {
                Contact_Id = Guid.NewGuid(),
                LastName = "Smith",
                DOB = new DateTime(2003, 3, 6),
                EmailId = new List<string>() { "Mark.Smith@gmail.com" },
                PhoneNumber = new List<string>() { "+449755777" }
            };

            this.mockContactStore.Setup(x => x.Create(contactModel)).Throws(new Exception(exceptionMessage));

            var response = await contactController.Add(contactModel) as BadRequestResult;
            Assert.AreEqual(response.StatusCode, 400);

        }


        [TestMethod]
        public async Task ContactControllerTest_Delete_Succeed_Test()
        {
            var tempGuid = Guid.NewGuid();
            this.mockContactStore.Setup(x => x.Delete(tempGuid)).ReturnsAsync(true);
            var response = await contactController.Delete(tempGuid.ToString()) as OkObjectResult;
            Assert.AreEqual(response.StatusCode, 200);
            Assert.AreEqual(response.Value, true);

        }

        [TestMethod]
        public async Task ContactControllerTest_Delete_Null_Failed_Test()
        {
            this.mockContactStore.Setup(x => x.Delete(Guid.Empty)).ReturnsAsync(null);
            var response = await contactController.Delete(Guid.Empty.ToString()) as BadRequestResult;
            Assert.AreEqual(response.StatusCode, 400);
        }
                

        [TestMethod]
        public async Task ContactControllerTest_GetByName_Succeed_Test()
        {
            string firstName = "Mark";
            var contacts = ConfigGenerator.GetContactByName(firstName);
            this.mockContactStore.Setup(x => x.GetByName(firstName)).ReturnsAsync(contacts);
            var result = await contactController.GetByName(firstName) as OkObjectResult;
            var outputCollection = result.Value as List<ContactModel>;
            CollectionAssert.AreEqual(contacts, outputCollection);
        }

        [TestMethod]
        public async Task ContactControllerTest_GetByName_NullFirstName_Failed_Test()
        {
            string exceptionMessage = "Contact Not found error!";
            this.mockContactStore.Setup(x => x.GetByName(null)).Throws(new Exception(exceptionMessage));
            var response = await contactController.GetByName(null) as BadRequestResult;
            Assert.AreEqual(response.StatusCode, 400);
        }

        [TestMethod]
        public async Task ContactControllerTest_GetByName_Success_NotFound()
        {
            string firstName = "Jhon";
            var contacts = ConfigGenerator.GetContactByName(firstName);
            this.mockContactStore.Setup(x => x.GetByName(firstName)).ReturnsAsync(contacts);
            var result = await contactController.GetByName(firstName) as NotFoundResult;
            Assert.AreEqual(result, null);
        }


    }
}
