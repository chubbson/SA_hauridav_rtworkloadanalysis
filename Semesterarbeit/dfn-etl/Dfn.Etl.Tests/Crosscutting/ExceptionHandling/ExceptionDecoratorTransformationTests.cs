using Dfn.Etl.Core;
using Dfn.Etl.Crosscutting.ExceptionHandling;
using Dfn.Etl.Crosscutting.Logging;
using Moq;
using NUnit.Framework;

namespace Dfn.Etl.Tests.Crosscutting.ExceptionHandling
{
    [TestFixture]
    public class ExceptionDecoratorTransformationTests
    {
        private ILogAgent m_LogAgent;
        private ICancelNetwork m_CancelNetwork;

        [SetUp]
        public void Setup()
        {
            Mock<ILogAgent> mockLog = new Mock<ILogAgent>();
            m_LogAgent = mockLog.Object;

            Mock<ICancelNetwork> mockCancel = new Mock<ICancelNetwork>();
            m_CancelNetwork = mockCancel.Object;
        }

        [Test]
        public void Transform_on_broken_dataflow_message_returns_broken_dataflow_message()
        {
            var mockTransformation = new Mock<ITransformation<int, string>>();
            var underTest = new ExceptionDecoratorTransformation<int, string>(mockTransformation.Object, m_LogAgent, m_CancelNetwork);

            var rV = underTest.Transform(new BrokenDataflowMessage<int>(null, null));
            
            mockTransformation.Verify(transformation => transformation.Transform(It.IsAny<int>()), Times.Never());
            Assert.That(rV, Is.TypeOf<BrokenDataflowMessage<string>>());
        }
         
    }
}