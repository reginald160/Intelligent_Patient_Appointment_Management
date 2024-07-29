using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Helpers
{
	public class SequenceContractHelper
	{
		private readonly string _connectionString = ConfigurationHelper.GetConnectionString("DefaultConnection");
		public  async Task<long> GenerateNextPatientNumberAsync(int userType)
		{
			long lastPatientNumber = await GetLastSequenceNumberAsync(userType);
			long newPatientNumber = lastPatientNumber + 1;
			
			return newPatientNumber;
		}

		private async Task<long> GetLastSequenceNumberAsync(int userType)
		{
			var dataColoumn = GetCharacterForInteger(userType);

			//user type 1 is patience, 2 is doctor and 3 is Admin


			long lastPatientNumber = 0;
			string query = $"SELECT TOP 1 {dataColoumn} FROM SequenceContract";

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();
				using (SqlCommand command = new SqlCommand(query, connection))
				{
					//command.Parameters.AddWithValue("@dataColumn", dataColoumn);
					var result = await command.ExecuteScalarAsync();
					if (result != null && result.ToString() != "0")
					{
						lastPatientNumber = Convert.ToInt64(result);
					}
				}
			}

			return lastPatientNumber;
		}

		public async Task UpdateSequence(long sequenceNumber, int userType)
		{
			if(!await SequenceHasValue()) 
			{
				await SaveNewSequenceAsync(userType);
			}
			else
			{
				await UpdateSequenceAsync(userType, sequenceNumber);
			}
		}
		private async Task SaveNewSequenceAsync( int column)
		{
			var columnName = GetCharacterForInteger(column);

			string query = $"INSERT INTO SequenceContract (Id,{columnName}) VALUES (@Id, 1)";

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();
				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@Id", Guid.NewGuid());
					await command.ExecuteNonQueryAsync();
				}
			}
		}

		private async Task UpdateSequenceAsync(int number, long newvalue)
		{
			var column = GetCharacterForInteger(number);
			string query = $"UPDATE SequenceContract SET {column} = @newvalue";

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();
				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@newvalue", newvalue);
					await command.ExecuteNonQueryAsync();
				}
			}
		}
		private static string GetCharacterForInteger(int number)
		{
			switch (number)
			{
				case 1:
					return "PatientCount";
				case 2:
					return "AdminCount";
				case 3:
					return "DoctorCount";
				default:
					throw new ArgumentException("Invalid input. Please provide an integer between 1 and 3.");
			}
		}
		public async Task<bool> SequenceHasValue()
		{
			string query = "SELECT COUNT(*) FROM SequenceContract";
			int count = 0;

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();
				using (SqlCommand command = new SqlCommand(query, connection))
				{
					count = (int)await command.ExecuteScalarAsync();
				}
			}

			return count > 0;
		}
	}
}
