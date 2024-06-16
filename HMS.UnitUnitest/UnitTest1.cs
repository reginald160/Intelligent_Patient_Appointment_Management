using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Helpers;

namespace HMS.UnitUnitest
{
	[TestFixture]
	public class Tests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public async Task TestSequence()
		{
			var seq = new SequenceContractHelper();
			bool hasValues = await seq.SequenceHasValue();

			// Assert
		
			await new SequenceContractHelper().GenerateNextPatientNumberAsync(1);
			Assert.IsFalse(hasValues);
			Assert.Pass();
		}
	}
}