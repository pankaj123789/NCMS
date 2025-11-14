using System.ComponentModel.DataAnnotations;

namespace Ncms.Contracts.Models.Person
{
    // DTO to carry deletion instructions from the API Controller to the Service Layer.
    public class DeletePersonRequestModel
    {
        [Required]
        public int PersonId { get; set; }

        // This will be populated by the API Controller using the logged-in Admin's ID/Username.
        public string DeletedBy { get; set; }
    }
}