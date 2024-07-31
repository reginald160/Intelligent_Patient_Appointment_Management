namespace HMSPortal.Models
{
    public class Enums
    {
        public enum Gender
        {
            Male,Female, Others
        }

        public enum AuditType
        {
            None,
            Create,
            Update,
            Delete
        }

        public enum AppointmentStatus
        {
            UpComming,
            Completed,
            Cancelled,
            Ongoing
        }
    }
}
