using HMSPortal.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSPortal.Models
{
    public class BaseIndividual : BaseEntity
    {

        public Guid? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; } 
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;   
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string HouseNumber { get; set; }= string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;

    }
}
