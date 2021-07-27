using DataAnnotationsExtensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Demo.WebApi.Patch.API.Models
{
    public class User : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Email]
        public string Email { get; set; }

        public int ApplicationId { get; set; }

        public string Username { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (ApplicationId == default && (this.Username == null || this.Username?.Length < 4)) 
            {
                results.Add(new ValidationResult($"{nameof(this.Username)} length must be greater than 4 when Application Id is 0."));
            }

            return results;
        }
    }
}
