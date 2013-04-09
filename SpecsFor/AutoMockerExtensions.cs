using System;
using System.Linq;
using StructureMap.AutoMocking;

namespace SpecsFor
{
	public static class AutoMockerExtensions
	{
		public static TMock GetMockFor<T, TMock>(this IAutoMocker<T> mocker) where TMock : class where T : class
		{
			return mocker.Get<TMock>();
		}

		public static TMock[] GetMockForEnumerableOf<T, TMock>(this IAutoMocker<T> mocker, int enumerableSize) where TMock : class where T : class
		{
			var existingMocks = mocker.Container.Model.InstancesOf<TMock>().ToArray();

			if (existingMocks.Length > 0)
			{
				if (existingMocks.Length != enumerableSize)
				{
					throw new InvalidOperationException("An IEnumerable for this type of mock has already been created with size " +
					                                    existingMocks.Length + ".");
				}

				return mocker.Container.GetAllInstances<TMock>().ToArray();
			}

			var mocks = mocker.CreateMockArrayFor<TMock>(enumerableSize);

			return mocks.ToArray();
		}
	}
}