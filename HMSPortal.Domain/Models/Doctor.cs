namespace HMSPortal.Domain.Models
{
    public class Doctor : BaseIndividual
    {
        public string? DoctorCode { get; set; }
        public string? BackgroundHistory { get; set; }
        public string? Speciality { get; set; }
        public int YearsOfExperience { get; set; }
    }
}
