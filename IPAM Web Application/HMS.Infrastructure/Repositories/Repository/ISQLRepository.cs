using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories.Repository
{
    public class ISQLRepository : ISQLServices
    {
        private string _connectionString;
        private readonly IConfiguration _configuration;

        public ISQLRepository(IConfiguration configuration)
        {
            _configuration=configuration;

            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new Exception(nameof(configuration));
        }

        private async Task<List<(long Id, string PatientCode, string FirstName)>> GetPatientsAsync(int userType)
        {
            string query = "SELECT Id, PatientCode, FirstName FROM Patients WHERE IsDeleted = 0";
            var patients = new List<(long Id, string PatientCode, string FirstName)>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            long id = reader.GetInt64(reader.GetOrdinal("Id"));
                            string patientCode = reader.GetString(reader.GetOrdinal("PatientCode"));
                            string firstName = reader.GetString(reader.GetOrdinal("FirstName"));

                            patients.Add((id, patientCode, firstName));
                        }
                    }
                }
            }

            return patients;
        }

    }
}
