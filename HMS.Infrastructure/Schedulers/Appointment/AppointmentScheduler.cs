using HMS.Infrastructure.Persistence.DataContext;
using HMSPortal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Schedulers.Appointment
{

    public class AppointmentScheduler
    {
        private readonly ApplicationDbContext _dbContext;
        private List<AppointmentModel> bookedAppointments;
        private  TimeSpan workDayStart = new TimeSpan(8, 0, 0); // 8:00 AM
        private  TimeSpan workDayEnd = new TimeSpan(17, 0, 0); // 5:00 PM
        private  TimeSpan regularAppointmentDuration = TimeSpan.FromMinutes(40);
        private  TimeSpan majorAppointmentDuration = TimeSpan.FromHours(2);
        private  TimeSpan slotDuration = TimeSpan.FromMinutes(30);


        public AppointmentScheduler(ApplicationDbContext dbContext)
        {
            bookedAppointments = new List<AppointmentModel>();
            _dbContext=dbContext;
        }

        public List<DateTime> GetAvailableAppointments(DateTime startDate, bool isMajorAppointment)
        {
            List<DateTime> availableSlots = new List<DateTime>();
            DateTime endDate = startDate.AddMonths(2);
            TimeSpan appointmentDuration = isMajorAppointment ? majorAppointmentDuration : regularAppointmentDuration;

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek >= DayOfWeek.Monday && date.DayOfWeek <= DayOfWeek.Friday)
                {
                    DateTime slotStart = date.Date + workDayStart;
                    while (slotStart + appointmentDuration <= date.Date + workDayEnd)
                    {
                        if (IsSlotAvailable(slotStart, slotStart + appointmentDuration))
                        {
                            availableSlots.Add(slotStart);
                        }
                        slotStart = slotStart.AddMinutes(30);
                    }
                }
            }

            return availableSlots;
        }

        public List<string> GetAvailableSlotsForDateToString(DateTime date)
        {
       
            bool isToday = date.Date == DateTime.Today;
            List<string> availableSlots = new List<string>();

            
            // Check if the date is a weekday
            if (date.DayOfWeek >= DayOfWeek.Monday && date.DayOfWeek <= DayOfWeek.Friday)
            {
                DateTime slotStart = date.Date + workDayStart;
                while (slotStart + slotDuration <= date.Date + workDayEnd)
                {
                    if (IsSlotAvailable(slotStart, slotStart + slotDuration))
                    {
                        DateTime slotEnd = slotStart + slotDuration;
                        string formattedSlot = $"{slotStart.ToString("h:mm tt")} - {slotEnd.ToString("h:mm tt")}";
                        bool difference = ((slotStart - DateTime.Now).TotalMinutes) > 20;
                        if (difference)
                        {
                            availableSlots.Add(formattedSlot);
                        }
                    }
                    slotStart = slotStart.Add(slotDuration);
                }
            }

            return availableSlots;
        }
        public List<DateTime> GetAvailableSlotsForDate(DateTime date)
        {
            List<DateTime> availableSlots = new List<DateTime>();

            // Check if the date is a weekday
            if (date.DayOfWeek >= DayOfWeek.Monday && date.DayOfWeek <= DayOfWeek.Friday)
            {
            
                DateTime slotStart = date.Date + DateTime.Now.TimeOfDay;
                while (slotStart + slotDuration <= date.Date + workDayEnd)
                {
                    if (IsSlotAvailable(slotStart, slotStart + slotDuration))
                    {
                        availableSlots.Add(slotStart);
                    }
                    slotStart = slotStart.Add(slotDuration);
                }
            }

            return availableSlots;
        }

        private bool IsSlotAvailable(DateTime start, DateTime end)
        {
            return !_dbContext.Appointments.Any(a =>
                (start >= a.StartTime && start < a.Endtime) ||
                (end > a.StartTime && end <= a.Endtime) ||
                (start <= a.StartTime && end >= a.Endtime));
        }




        public bool BookAppointment(DateTime startTime, bool isMajorAppointment)
        {
            TimeSpan duration = isMajorAppointment ? majorAppointmentDuration : regularAppointmentDuration;
            DateTime endTime = startTime + duration;

            if (IsSlotAvailable(startTime, endTime) &&
                startTime.TimeOfDay >= workDayStart &&
                endTime.TimeOfDay <= workDayEnd &&
                startTime.DayOfWeek >= DayOfWeek.Monday &&
                startTime.DayOfWeek <= DayOfWeek.Friday &&
                startTime.Date <= DateTime.Now.AddMonths(2))
            {
                bookedAppointments.Add(new AppointmentModel
                {
                    StartTime = startTime,
                    Endtime = endTime
                   // IsMajor = isMajorAppointment
                });
                return true;
            }
            return false;
        }

        public  DateClass ConvertToDateTimeRange(string timeSlot, DateTime appointmentDate)
        {
            // Split the time slot into start and end parts
            var times = timeSlot.Split('-');
            if (times.Length != 2)
            {
                throw new ArgumentException("Invalid time slot format. Expected format: 'hh:mmtt-hh:mmtt'.");
            }

            var startTime = DateTime.Parse(times[0].Trim());
            // Parse end time
            var endTime = DateTime.Parse(times[1].Trim());

            // Combine the parsed times with the appointment date
            var startDateTime = new DateTime(appointmentDate.Year, appointmentDate.Month, appointmentDate.Day,
                                             startTime.Hour, startTime.Minute, 0);
            var endDateTime = new DateTime(appointmentDate.Year, appointmentDate.Month, appointmentDate.Day,
                                           endTime.Hour, endTime.Minute, 0);

            return new DateClass
            {
                StartTime = startDateTime,
                StopTime = endDateTime,
            };
        }


        public class DateClass
        {
            public DateTime StartTime { get; set; }
            public DateTime StopTime { get; set; }
        }
    }
   

}
