﻿
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HMSPortal.Application.ViewModels
{
	public class AddDoctorViewModel
	{
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Phone { get; set; } = string.Empty;
		public string PostalCode { get; set; } = string.Empty;
		public string HouseNumber { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public DateTime DateOfBirth { get; set; }
		public string Gender { get; set; } = string.Empty;

		public string? DoctorCode { get; set; }
		public string? BackgroundHistory { get; set; }
		public string? Specialty { get; set; }
		public int YearsOfExperience { get; set; }
		public int Age { get; set; }
		public string? DoctorDetails { get; set; }
		[JsonIgnore]
		public IFormFile? Image { get; set; }
		public string? Password { get; set; }
		public string? ImageUrl { get; set; }
	}
}
