namespace HMSPortal.Domain.Models
{
    public class Doctor : BaseIndividual
    {
        public string? DoctorCode { get; set; }
        public string? BackgroundHistory { get; set; }
        public string? Specialty { get; set; }
        public int YearsOfExperience { get; set; }
        public int Age { get; set; }
        public string? DoctorDetails { get; set; }
        public string? ImageUrl { get; set; }
		public ICollection<AppointmentModel> ? Appointments { get; set; }

	}
}
