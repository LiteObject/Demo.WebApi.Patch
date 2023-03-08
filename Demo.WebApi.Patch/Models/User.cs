using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;

namespace Demo.WebApi.Patch.API.Models
{
    /// <summary>
    /// The user DTO
    /// </summary>
    public class User : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user name
        /// </summary>
        [Required]
        [StringLength(25)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user email
        /// </summary>
        [Email]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user phone number
        /// </summary>
        [Phone]
        public string Phone { get; set; }

        /// <summary>
        /// Validates user DTO
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
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
