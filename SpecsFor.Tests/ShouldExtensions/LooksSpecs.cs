﻿using NSubstitute;
using NSubstitute.Exceptions;
using NUnit.Framework;
using SpecsFor.ShouldExtensions;

namespace SpecsFor.Tests.ShouldExtensions
{
	public class LooksSpecs : SpecsFor<LooksSpecs.TestObject>
	{
		#region Test Classes

		public class TestObject
		{
			public int ID { get; set; }
			public string Name { get; set; }
		}

		public interface ITestService
		{
			void DoStuff(TestObject obj);
		}

		#endregion

		protected override void InitializeClassUnderTest()
		{
			SUT = new TestObject {ID = 1, Name = "Test"};
		}

		[Test]
		public void nsubstitute_will_match_on_an_equivalent_object()
		{
			var mock = GetMockFor<ITestService>();

			mock.DoStuff(new TestObject {ID = 1, Name = "Test"});

			Assert.DoesNotThrow(() => mock.Received().DoStuff(Looks.Like(new TestObject {ID = 1, Name = "Test"})));
		}

		[Test]
		public void nsubstitute_will_not_match_on_a_nonequivalent_object()
		{
			var mock = GetMockFor<ITestService>();

			mock.DoStuff(new TestObject {ID = 3, Name = "Name"});

            Assert.Throws<ReceivedCallsException>(() => mock.Received().DoStuff(Looks.Like(new TestObject { ID = 1, Name = "Not Name" })));
		}

		[Test]
		public void nsubstitute_will_match_on_a_partial_object()
		{
			var mock = GetMockFor<ITestService>();

			mock.DoStuff(new TestObject {ID = 1, Name = "Test"});

			Assert.DoesNotThrow(() => mock.Received().DoStuff(Looks.LikePartialOf<TestObject>(new {ID = 1, Name = "Test"})));
		}

		[Test]
		public void nsubstitute_will_not_match_on_a_nonequivalent_partial_object()
		{
			var mock = GetMockFor<ITestService>();

			mock.DoStuff(new TestObject {ID = 3, Name = "Name"});

            Assert.Throws<ReceivedCallsException>(() => mock.Received().DoStuff(Looks.LikePartialOf<TestObject>(new { ID = 1, Name = "Not Name" })));
		}
	}
}