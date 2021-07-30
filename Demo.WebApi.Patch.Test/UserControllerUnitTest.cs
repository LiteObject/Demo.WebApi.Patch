using Demo.WebApi.Patch.API.Models;
using Demo.WebApi.Patch.Controllers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Demo.WebApi.Patch.Test
{
    public class UserControllerUnitTest
    {
        private readonly ITestOutputHelper _output;

        public UserControllerUnitTest(ITestOutputHelper output) => _output = output ?? throw new ArgumentNullException($"{nameof(output)} cannot be null.");

        [Fact]
        public async Task Validation_of_User_DTO_Should_Fail()
        {
            // ARRANGE            
            var user = new User { Id = 1, Name = "1234567890 1234567890" };            
            var validationResultList = new List<ValidationResult>();          
            
            // ACT            
            var result = Validator.TryValidateObject(user, new ValidationContext(user), validationResultList, true);
            validationResultList.ForEach(v => _output.WriteLine(v.ErrorMessage));

            // ASSERT
            Assert.False(result);
            Assert.NotEmpty(validationResultList);            
        }
    }
}
