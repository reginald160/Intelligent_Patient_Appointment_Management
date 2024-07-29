using HMSPortal.Domain.Models.Contract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Domain.Models
{
	public class AdminModel : AuditableEntity
	{
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Phone { get; set; } = string.Empty;

		public DateTime DateOfBirth { get; set; }
		public string Gender { get; set; } = string.Empty;
		[Required]
		[DataType(DataType.EmailAddress)]
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
		public string? ImageUrl { get; set; }

		public string? UserId { get; set; }
		public string Role { get; set; }
	}
}
