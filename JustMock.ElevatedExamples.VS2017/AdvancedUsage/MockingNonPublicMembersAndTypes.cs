using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.AdvancedUsage.MockingNonPublicMembersAndTypes
{
	/// <summary>
	/// In elevated mode, you can use Telerik JustMock to mock non-public members and types. That is useful when you want to 
	/// isolate calls to non-public members.
	/// See http://www.telerik.com/help/justmock/advanced-usage-mocking-non-public-members-and-types.html for full 
	/// documentation of the feature.
	/// </summary>
	[TestClass]
	public class MockingNonPublicMembersAndTypes_Tests
	{
		[TestMethod]
		public void DoPublic_OnExecute_ShouldCallDoPrivate()
		{
			var isCalled = false;

			Foo foo = new Foo();

			// ARRANGE - When the non-public method DoPrivate() is called from the foo instance, it should set isCalled to true 
			//  instead instead of executing its original logic.
			Mock.NonPublic.Arrange(foo, "DoPrivate").DoInstead(() => isCalled = true);

			// ACT
			foo.DoPublic(); // DoPublic() should call DoPrivate().

			// ASSERT
			Assert.IsTrue(isCalled);
		}

		[TestMethod]
		public void Echo_OnExecute_ShouldReturnTheExpectationsForPrivateEcho()
		{
			var expected = 1;

			Foo foo = new Foo();

			// ARRANGE - When the non-public function PrivateEcho() is called from the foo instance, with any integer argument, 
			//  it should return expected integer.
			Mock.NonPublic.Arrange<int>(foo, "PrivateEcho", ArgExpr.IsAny<int>()).Returns(expected);

			// ACT
			int actual = foo.Echo(5);

			// ASSERT
			Assert.AreEqual(expected, actual);
		}


		[TestMethod]
		public void Echo_OnExecute_ShouldReturnTheExpectationUsingDynamicMock()
		{
			var expected = 1;

			Foo foo = new Foo();

			// ARRANGE - When the non-public function PrivateEcho() is called from the foo instance, with any integer argument, 
			//  it should return expected integer.
			var wrap = Mock.NonPublic.Wrap(foo);
			Mock.NonPublic.Arrange<int>(wrap.PrivateEcho(ArgExpr.IsAny<int>())).Returns(expected);

			// ACT
			int actual = foo.Echo(5);

			// ASSERT
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Execute_OnExecute_ShouldCallpExecuteWithCorrectArgument()
		{
			FooInternal foo = new FooInternal();

			// ARRANGE - That the non-public method pExecute() must be called from the foo instance, with argument equal to 10.
			Mock.NonPublic.Arrange(foo, "pExecute", 10).MustBeCalled();

			// ACT
			foo.Execute(10);

			// ASSERT
			Mock.Assert(foo);
		}

		[TestMethod]
		public void ShouldMockPrivateInterfaceImplementationMethod()
		{
			var expected = "dummy";

			// ARRANGE
			// Creating a mocked instance of the Bar class.
			var bar = Mock.Create<Bar>();

			// Arranging: When the Provider property of IManager interface is called through the bar instance, it should 
			//  return expected.
			Mock.Arrange(() => ((IManager)bar).Provider).Returns(expected);

			// ASSERT
			Assert.AreEqual(expected, ((IManager)bar).Provider);
		}

		[TestMethod]
		public void ShouldMockInternalVirtualMethod()
		{
			var isCalled = false;

			Foo foo = new Foo();

			// ARRANGE - When the internal foo.Do() method is called, it should set isCalled to true instead of executing 
			//  its original implementation.
			// NOTE: This arrange for internal type will be possible only if [InternalsVisibleTo] attribute is set as 
			//  described in this article: http://www.telerik.com/help/justmock/basic-usage-mock-internal-types-via-proxy.html
			Mock.Arrange(() => foo.Do()).DoInstead(() => isCalled = true);

			// ACT
			foo.Do();

			// ASSERT
			Assert.IsTrue(isCalled);
		}

		[TestMethod]
		public void ShouldMockInternalVirtualPropertyGET()
		{
			var expected = "ping";

			Foo foo = new Foo();

			// ARRANGE - When the internal foo.Value property is called, it should return expected string.
			// NOTE: This arrange for internal type will be possible only if [InternalsVisibleTo] attribute is set as 
			//  described in this article: http://www.telerik.com/help/justmock/basic-usage-mock-internal-types-via-proxy.html
			Mock.Arrange(() => foo.Value).Returns(expected);

			// ACT
			string actual = foo.Value;

			// ASSERT
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ShouldMockInternalVirtualPropertySET()
		{
			// ARRANGE
			// Creating a mocked instance of the Foo class.
			Foo foo = Mock.Create<Foo>();

			// Arranging: That the internal foo.Value must be set to "ping" during the test method.
			// NOTE: This arrange for internal type will be possible only if [InternalsVisibleTo] attribute is set as 
			//  described in this article: http://www.telerik.com/help/justmock/basic-usage-mock-internal-types-via-proxy.html
			Mock.ArrangeSet(() => foo.Value = "ping").MustBeCalled();

			// ACT
			foo.Value = "ping";

			// ASSERT
			Mock.Assert(foo);
		}

		[TestMethod]
		public void ShouldMockPrivateStaticPropertyGET()
		{
			var expected = 10;

			Foo foo = new Foo();

			// ARRANGE - When the private static PrivateStaticProperty property is called, it should return expected value.
			Mock.NonPublic.Arrange<int>(typeof(Foo), "PrivateStaticProperty").Returns(expected);

			// ACT
			int actual = foo.GetMyPrivateStaticProperty();

			// ASSERT
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ShouldMockPrivateStaticPropertyGETUsingDynamicMock()
		{
			var expected = 10;

			Foo foo = new Foo();

			// ARRANGE - When the private static PrivateStaticProperty property is called, it should return expected value.
			var wrap = Mock.NonPublic.WrapType(typeof(Foo));
			Mock.NonPublic.Arrange<int>(wrap.PrivateStaticProperty).Returns(expected);

			// ACT
			int actual = foo.GetMyPrivateStaticProperty();

			// ASSERT
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void ShouldMockPrivateStaticMethodFirstApproach()
		{
			// ARRANGE - In the static class FooInternalStatic an integer function with name "EchoPrivate" should throw an 
			//  Exception if called with an integer argument that equals 10.
			// NOTE: To interact with internal classes like this, you may need to add [InternalsVisibleTo] attribute inside 
			//  the AssemblyInfo.cs 
			Mock.NonPublic.Arrange<FooInternalStatic, int>("EchoPrivate", 10).Throws<Exception>();

			// ACT
			FooInternalStatic.Echo(10);
		}

		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void ShouldMockPrivateStaticMethodSecondApproach()
		{
			// ARRANGE - In an instance of type FooInternalStatic an integer function with name "EchoPrivate" should throw 
			//  an Exception if called with an integer argument that equals 10.
			// NOTE: To interact with internal classes like this, you may need to add [InternalsVisibleTo] attribute inside 
			//  the AssemblyInfo.cs 
			Mock.NonPublic.Arrange<int>(typeof(FooInternalStatic), "EchoPrivate", 10).Throws<Exception>();

			// ACT
			FooInternalStatic.Echo(10);
		}

		[TestMethod]
		[ExpectedException(typeof(AssertFailedException))]
		public void ShouldAssertPrivateStaticMethodFirstApproach()
		{
			// ARRANGE - In the static class FooInternalStatic an integer function with name "EchoPrivate" must be called 
			//  with an integer argument that equals 10.
			// NOTE: To interact with internal classes like this, you may need to add [InternalsVisibleTo] attribute inside 
			//  the AssemblyInfo.cs 
			Mock.NonPublic.Arrange<FooInternalStatic, int>("EchoPrivate", 10).MustBeCalled();

			// ASSERT - This will throw an AssertFailedException as the EchoPrivate method has never been called during the test.
			Mock.NonPublic.Assert<FooInternalStatic, int>("EchoPrivate", 10);
		}

		[TestMethod]
		public void ShouldAssertPrivateStaticMethodSecondApproach()
		{
			// ARRANGE - In the static class FooInternalStatic an integer function with name "EchoPrivate" must be called 
			//  with an integer argument that equals 10.
			// NOTE: To interact with internal classes like this, you may need to add [InternalsVisibleTo] attribute inside 
			//  the AssemblyInfo.cs 
			Mock.NonPublic.Arrange<int>(typeof(FooInternalStatic), "EchoPrivate", 10).MustBeCalled();

			// ACT
			FooInternalStatic.Echo(10);

			// ASSERT - This will assert that the EchoPrivate method has been called with the expected argument during the test.
			Mock.NonPublic.Assert<int>(typeof(FooInternalStatic), "EchoPrivate", 10);
		}

		[TestMethod]
		public void ShouldMockInternaldotNETClass()
		{
			var isCalled = false;

			// ARRANGE
			// We are about to mock System.Net.HttpRequestCreator(), which is a non-public class.
			string typeName = "System.Net.HttpRequestCreator";

			// Creating a mocked instance of the System.Net.HttpRequestCreator internal class.
			var httpRequestCreator = Mock.Create(typeName);

			// Arranging: When the non-public Create() method is called from the httpRequestCreator instance, with any Uri 
			//  argument, it should assign true to isCalled instead of executing its original logic.
			Mock.NonPublic.Arrange(httpRequestCreator, "Create", ArgExpr.IsAny<Uri>()).DoInstead(() => isCalled = true);

			// ACT
			System.Net.IWebRequestCreate iWebRequestCreate = (System.Net.IWebRequestCreate)httpRequestCreator;

			iWebRequestCreate.Create(new Uri("http://www.telerik.com"));

			// ASSERT
			Assert.IsTrue(isCalled);
		}

		[TestMethod]
		public void ShouldAssertOccrenceForNonPublicFunction()
		{
			// ARRANGE - Creating a mocked instance of the FooWithProtectedMembers class with Behavior.CallOriginal.
			var foo = Mock.Create<FooWithProtectedMembers>(Behavior.CallOriginal);

			// ASSERT - Asserting that the non-public integer function, IntValue() does not occur during the test.
			Mock.NonPublic.Assert<int>(foo, "IntValue", Occurs.Never());
		}

		[TestMethod]
		public void Init_OnExecute_ShouldCallLoad()
		{

			// ARRANGE
			// Creating a mocked instance of the FooWithProtectedMembers class with Behavior.CallOriginal.
			var foo = Mock.Create<FooWithProtectedMembers>(Behavior.CallOriginal);

			// Arranging: That the protected method Load() must be called inside the foo instance during the test.
			Mock.NonPublic.Arrange(foo, "Load").MustBeCalled();

			// ACT - The Init() method should call the protected Load() method.
			foo.Init();

			// ASSERT
			Mock.Assert(foo);
		}
	}

	#region SUT
	public class Foo
	{
		private void DoPrivate()
		{
			throw new NotImplementedException();
		}

		private void DoPrivate(int arg)
		{
			throw new NotImplementedException();
		}

		public void DoPublic()
		{
			DoPrivate();
		}

		public void Execute(int arg)
		{
			DoPrivate(arg);
		}

		private int PrivateEcho(int arg)
		{
			return arg;
		}

		public int Echo(int arg)
		{
			return PrivateEcho(arg);
		}

		internal virtual void Do()
		{
			throw new NotImplementedException();
		}

		internal virtual string Value
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private static int PrivateStaticProperty { get; set; }

		public int GetMyPrivateStaticProperty()
		{
			return PrivateStaticProperty;
		}
	}

	internal class FooInternal
	{
		private void pExecute(int arg1)
		{
			throw new NotImplementedException();
		}

		private void pExecute()
		{
			throw new NotImplementedException();
		}

		public void Execute(int arg1)
		{
			pExecute(arg1);
		}

		public void Execute()
		{
			pExecute();
		}
	}

	internal class FooInternalStatic
	{
		private static int EchoPrivate(int arg1)
		{
			throw new NotImplementedException();
		}

		public static int Echo(int arg1)
		{
			return EchoPrivate(arg1);
		}
	}

	public class FooWithProtectedMembers
	{
		protected virtual void Load()
		{
			throw new NotImplementedException();
		}

		protected virtual int IntValue
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual void Init()
		{
			Load();
		}
	}

	public interface IManager
	{
		object Provider { get; }
	}

	public class FooBase : IManager
	{
		object IManager.Provider
		{
			get { throw new NotImplementedException(); }
		}
	}

	public class Bar : FooBase
	{
		//...
	}
	#endregion
}
