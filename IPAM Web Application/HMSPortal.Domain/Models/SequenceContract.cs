using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Domain.Models
{
	public class SequenceContract
	{

			[Key]
			public Guid Id { get; set; }
			public long PatientCount { get; set; }
			public long AdminCount { get; set; }
			public long DoctorCount { get; set; }

		
	}
}
