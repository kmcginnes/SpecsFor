using NSubstitute;
using NUnit.Framework;
using Should;
using SpecsFor.Demo.Domain;

namespace SpecsFor.Demo.BDD.Composition
{
	public class ValidatingOrderProcessorSpecs
	{
		public class when_processing_an_order : SpecsFor<ValidatingOrderProcessor>
		{
			private OrderResult _result;

			protected override void Given()
			{
				Given<the_item_is_available>();
				Given<the_item_is_valid>();

				base.Given();
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


		public class when_processing_an_order_with_a_negative_quantity : SpecsFor<ValidatingOrderProcessor>
		{
			private OrderResult _result;

			protected override void Given()
			{
				Given<the_item_is_available>();
				Given<the_item_is_not_valid>();

				base.Given();
			}

			protected override void When()
			{
				_result = SUT.Process(new Order{ PartNumber = "TestPart", Quantity = -1});
			}

			[Test]
			public void then_the_order_is_rejected()
			{
				_result.WasAccepted.ShouldBeFalse();
			}

			[Test]
			public void then_it_does_not_check_the_inventory()
			{
				GetMockFor<IInventory>()
                    .DidNotReceive().IsQuantityAvailable("TestPart", -1);
			}

			[Test]
			public void then_it_does_not_raise_an_order_submitted_event()
			{
				GetMockFor<IPublisher>()
                    .DidNotReceive().Publish(Arg.Any<OrderSubmitted>());
			}
		}

		public class the_item_is_available : IContext<ValidatingOrderProcessor>
		{
			public void Initialize(ISpecs<ValidatingOrderProcessor> state)
			{
				state.GetMockFor<IInventory>()
					.IsQuantityAvailable("TestPart", 10)
					.Returns(true);
			}
		}

		public class the_item_is_valid : IContext<ValidatingOrderProcessor>
		{
			public void Initialize(ISpecs<ValidatingOrderProcessor> state)
			{
				state.GetMockFor<IValidator<Order>>()
					.Validate(Arg.Any<Order>())
					.Returns(true);
			}
		}

		public class the_item_is_not_valid : IContext<ValidatingOrderProcessor>
		{
			public void Initialize(ISpecs<ValidatingOrderProcessor> state)
			{
				state.GetMockFor<IValidator<Order>>()
					.Validate(Arg.Any<Order>())
					.Returns(false);
			}
		}
	}
}