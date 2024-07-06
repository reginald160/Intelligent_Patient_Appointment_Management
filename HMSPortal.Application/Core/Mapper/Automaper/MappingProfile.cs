using AutoMapper;
using HMSPortal.Application.ViewModels;
using HMSPortal.Application.ViewModels.Appointment;
using HMSPortal.Application.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Mapper.Automaper
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<GetDoctorViewModel, EditDoctorViewModel>();

    

            // Add other mappings here
        }
	}
}
