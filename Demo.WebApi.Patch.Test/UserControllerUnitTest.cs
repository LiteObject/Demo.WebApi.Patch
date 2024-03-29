namespace Demo.WebApi.Patch.Test
{
    using Demo.WebApi.Patch.API.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public class UserControllerUnitTest
    {
        private readonly ITestOutputHelper _output;

        public UserControllerUnitTest(ITestOutputHelper output) => _output = output ?? throw new ArgumentNullException($"{nameof(output)} cannot be null.");

        [Theory]
        [InlineData("", "test@email.com", "214-000-0000")]        
        [InlineData("John Doe", "test", "*-*-0000")]
        [InlineData("John Doe John Doe John Doe John Doe John Doe John Doe", "test@email.com", "*-*-0000")]
        [InlineData("John Doe", null, null)]
        public void Validation_of_User_DTO_Should_Failc(string name, string email, string phone)
        {
            // ARRANGE            
            var user = new User { Id = 1, Name = name, Email = email, Phone = phone };            
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