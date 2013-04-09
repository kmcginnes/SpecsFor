using NSubstitute;
using NUnit.Framework;
using Should;
using SpecsFor.Demo.Domain;

namespace SpecsFor.Demo.OldSchoolTests
{
	public class OrderProcessorSpecs : SpecsFor<OrderProcessor>
	{
		[Test]
		public void Order_submitted_successfully_Tests()
		{
			GetMockFor<IInventory>()
				.IsQuantityAvailable("TestPart", 10)
				.Returns(true);

			var result = SUT.Process(new Order {PartNumber = "TestPart", Quantity = 10});

			result.WasAccepted.ShouldBeTrue();

			GetMockFor<IInventory>().Received().IsQuantityAvailable("TestPart", 10);

			GetMockFor<IPublisher>()
				.Received().Publish(Arg.Is<OrderSubmitted>(o => o.OrderNumber == result.OrderNumber));
		}

		[Test]
		public void Order_is_rejected_Tests()
		{
			GetMockFor<IInventory>()
				.IsQuantityAvailable("TestPart", 10)
				.Returns(false);

			var result = SUT.Process(new Order {PartNumber = "TestPart", Quantity = 10});

			result.WasAccepted.ShouldBeFalse();

            GetMockFor<IInventory>().Received().IsQuantityAvailable("TestPart", 10);

			GetMockFor<IPublisher>()
				.DidNotReceive().Publish(Arg.Any<OrderSubmitted>());
		}
	}
}