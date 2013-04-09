using NSubstitute;
using NUnit.Framework;
using Should;
using SpecsFor.Demo.Domain;

namespace SpecsFor.Demo.PlainBDD
{
	public class OrderProcessorSpecs
	{
		public class given_the_item_is_available_when_processing_an_order : SpecsFor<OrderProcessor>
		{
			private OrderResult _result;

			protected override void Given()
			{
			    GetMockFor<IInventory>()
			        .IsQuantityAvailable("TestPart", 10)
			        .Returns(true);
			}

			protected override void When()
			{
				_result = SUT.Process(new Order { PartNumber = "TestPart", Quantity = 10 });
			}

			[Test]
			public void then_the_order_is_accepted()
			{
				_result.WasAccepted.ShouldBeTrue();	
			}

			[Test]
			public void then_it_checks_the_inventory()
			{
                GetMockFor<IInventory>().Received().IsQuantityAvailable("TestPart", 10);
			}

			[Test]
			public void then_it_raises_an_order_submitted_event()
			{
				GetMockFor<IPublisher>()
					.Received().Publish(Arg.Is<OrderSubmitted>(o => o.OrderNumber == _result.OrderNumber));
			}
		}

		public class given_the_item_is_not_available_when_processing_an_order : SpecsFor<OrderProcessor>
		{
			private OrderResult _result;

			protected override void Given()
			{
				GetMockFor<IInventory>()
					.IsQuantityAvailable("TestPart", 10)
					.Returns(false);
			}

			protected override void When()
			{
				_result = SUT.Process(new Order { PartNumber = "TestPart", Quantity = 10 });
			}

			[Test]
			public void then_the_order_is_rejected()
			{
				_result.WasAccepted.ShouldBeFalse();	
			}

			[Test]
			public void then_it_checks_the_inventory()
			{
                GetMockFor<IInventory>().Received().IsQuantityAvailable("TestPart", 10);
			}

			[Test]
			public void then_it_does_not_raise_an_order_submitted_event()
			{
				GetMockFor<IPublisher>()
					.DidNotReceive().Publish(Arg.Any<OrderSubmitted>());
			}
		}
	}
}