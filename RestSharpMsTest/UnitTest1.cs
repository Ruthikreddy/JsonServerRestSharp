using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using RestSharpJson;
using System.Net;

namespace RestSharpMsTest
{
    [TestClass]
    public class RestSharpTestCases
    {
        RestClient client;
        /// <summary>
        /// Setups this instance for the client by giving url along with port.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            client = new RestClient("http://localhost:4000");
        }
        /// <summary>
        /// Gets the employee list in the form of irestresponse. 
        /// </summary>
        /// <returns>IRestResponse response</returns>
        private IRestResponse getEmployeeList()
        {
            //getting all the data from json server by giving table name and method.get
            RestRequest request = new RestRequest("/employees", Method.GET);
            //executing the request using client and saving the result in IrestResponse.
            IRestResponse response = client.Execute(request);
            return response;
        }
        /// <summary>
        /// Ons the calling get API return employee list.
        /// </summary>
        [TestMethod]
        public void onCallingGetApi_ReturnEmployeeList()
        {
            //gets the irest response from getemployee list method
            IRestResponse response = getEmployeeList();
            //assert for checking status code of get
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            //adding the data into list from irestresponse by using deserializing.
            List<Employee> dataResponse = JsonConvert.DeserializeObject<List<Employee>>(response.Content);
            //printing out the content for list of employee
            foreach (Employee employee in dataResponse)
            {
                Console.WriteLine("Id: " + employee.id + " Name: " + employee.name + " Salary: " + employee.salary);
            }
            //assert for checking count of no of element in list to be equal to data in jsonserver table.
            Assert.AreEqual(4, dataResponse.Count);
        }
        /// <summary>
        /// Givens the employee on post should return added employee. UC2
        /// </summary>
        [TestMethod]
        public void givenEmployee_OnPost_ShouldReturnAddedEmployee()
        {
            //adding multiple employees to table
            List<Employee> EmployeeList = new List<Employee>();
            EmployeeList.Add(new Employee { name = "Ishan", salary = "40000" });
            EmployeeList.Add(new Employee { name = "CounterNile", salary = "500000" });
            EmployeeList.ForEach(employeeData =>
            {
                //adding request to post(add) data
                RestRequest request = new RestRequest("/employees", Method.POST);
                JsonObject jObject = new JsonObject();
                jObject.Add("name", employeeData.name);
                jObject.Add("salary", employeeData.salary);
                //as parameters are passed as body hence "request body" call is made, in parameter type
                request.AddParameter("application/json", jObject, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                //assert
                //code will be 201 for posting data
                Assert.AreEqual(response.StatusCode,HttpStatusCode.Created);
                //derserializing object for assert and checking test case
                Employee dataResponse = JsonConvert.DeserializeObject<Employee>(response.Content);
                Assert.AreEqual(employeeData.name, dataResponse.name);
                Console.WriteLine(response.Content);
            });

        }
        /// <summary>
        /// Givens the employee on update should return updated employee. UC4
        /// </summary>
        [TestMethod]
        public void GivenEmployee_OnUpdate_ShouldReturnUpdatedEmployee()
        {
            //making a request for a particular employee to be updated
            RestRequest request = new RestRequest("employees/5", Method.PUT);
            JsonObject jobject = new JsonObject();
            jobject.Add("name", "Ishan");
            jobject.Add("salary",120000);
            request.AddParameter("application/json", jobject, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Assert.AreEqual(response.StatusCode,HttpStatusCode.OK);
            //deserializing content added in json file
            Employee dataResponse = JsonConvert.DeserializeObject<Employee>(response.Content);
            //asserting for salary
            Assert.AreEqual(dataResponse.salary, "120000");
            //writing content without deserializing from resopnse. 
            Console.WriteLine(response.Content);
        }
        [TestMethod]
        public void GivenEmployee_OnDelete_ShouldReturnSuccessStatus()
        {
            //request for deleting elements from json 
            RestRequest request = new RestRequest("employees/6", Method.DELETE);
            //executing request using rest client
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            //checking status codes.
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);
        }
    }
}
