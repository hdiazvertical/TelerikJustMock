using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.BasicUsage.CreateMocksByExample
{
    /// <summary>
    /// The built-in feature for creating mocks by example saves time when it comes to tiresome set up of arrangements. 
    /// This functionality allows you to create mocks of a certain class (the system under test) 
    ///  and in the same time to arrange their behavior.
    /// For simple tests with few arrangements, this provides only marginal benefit. 
    ///  The real benefit comes with complex tests with multiple arrangements
    /// See http://www.telerik.com/help/justmock/basic-usage-create-mocks-by-example.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class CreateMocksByExample_Tests
    {
        [TestMethod]
        public void TestShowingTheMocksByExampleUsability()
        {
            // Create a mock of IInstallInfo with arranged behavior: 
            //  - the InstallPackages property, should return new List<IInstallPackage>. This list should contain: 
            //      - First item - IInstallPackage mock, arranged like so:
            //          - Installer.Name should return "blocked1",
            //          - Installer.BlockingCondition.Name should return "foo".
            //      - Second item - IInstallPackage mock, arranged like so:
            //          - Installer.Name should return "blocked2",
            //          - Installer.BlockingCondition.Name should return "bar".
            var installInfo = Mock.CreateLike<IInstallInfo>(
                me => me.InstallPackages == new List<IInstallPackage>
                    {
                        Mock.CreateLike<IInstallPackage>(pkg => pkg.Installer.Name == "blocked1" &&
                                                                pkg.Installer.BlockingCondition.Name == "foo"),
                        Mock.CreateLike<IInstallPackage>(pkg => pkg.Installer.Name == "blocked2" &&
                                                                pkg.Installer.BlockingCondition.Name == "bar"),
                    });

            // ASSERT
            Assert.AreEqual("blocked1", installInfo.InstallPackages[0].Installer.Name);
            Assert.AreEqual("foo", installInfo.InstallPackages[0].Installer.BlockingCondition.Name);
            Assert.AreEqual("blocked2", installInfo.InstallPackages[1].Installer.Name);
            Assert.AreEqual("bar", installInfo.InstallPackages[1].Installer.BlockingCondition.Name);
        }

        [TestMethod]
        public void ShouldExplainStepByStep()
        {
            // Create mock, whose Name property returns "blocked1". 
            //  The == operator here is equivalent to calling .Returns("blocked1") while arranging inst.Name.
            var first = Mock.CreateLike<IInstallerInfo>(inst => inst.Name == "blocked1");
            
            // ASSERT
            Assert.AreEqual("blocked1", first.Name);

            // Create inner mocks recursively and set Installer.Name to return "blocked1"
            var second = Mock.CreateLike<IInstallPackage>(pkg => pkg.Installer.Name == "blocked1");

            // ASSERT
            Assert.AreEqual("blocked1", second.Installer.Name);

            // Create inner mocks recursively and arrange several return values. 
            //  The && operator is used to make a list of several arrangements.
            var third = Mock.CreateLike<IInstallPackage>(
                pkg => pkg.Installer.Name == "blocked1" && pkg.Installer.BlockingCondition.Name == "foo");

            // ASSERT
            Assert.AreEqual("blocked1", third.Installer.Name);
            Assert.AreEqual("foo", third.Installer.BlockingCondition.Name);

            // Arrange a property to return a list of mocks. Mock.Create can be called recursively within the expression.
            var fourth = Mock.CreateLike<IInstallInfo>(
                me => me.InstallPackages == new List<IInstallPackage> { Mock.Create<IInstallPackage>() });

            // ASSERT
            Assert.AreEqual(1, fourth.InstallPackages.Count);
        }
    }
    
    #region SUT
        
    public class DetectionInfoBase
    {
        public string Name { get; set; }
    }
        
    public interface IInstallerInfo
    {
        string Name { get; set; }

        DetectionInfoBase BlockingCondition { get; set; }
    }
        
    public interface IInstallPackage
    {
        IInstallerInfo Installer { get; set; }
    }
        
    public interface IInstallInfo
    {
        List<IInstallPackage> InstallPackages { get; set; }
    }

    #endregion
}