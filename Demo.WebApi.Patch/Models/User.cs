using DataAnnotationsExtensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Demo.WebApi.Patch.API.Models
{
    public class User : IValidatableObject
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Email]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Email) && string.IsNullOrWhiteSpace(this.Phone)) 
            {
                results.Add(new ValidationResult($"Either {nameof(this.Email)} or {nameof(this.Phone)} must be specified. Both cannot be null or empty."));
            }

            return results;
        }
    }
}
