#if !NET40 && !NET35
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using NSubstitute;

using NUnit.Framework;

using SharpRaven.Data;
using SharpRaven.UnitTests.Utilities;

namespace SharpRaven.UnitTests.Data
{
    public class BackgroundRequesterFactoryTests
    {
        private class Fixture
        {
            public IRequesterFactory RequesterFactory { get; set; } = Substitute.For<IRequesterFactory>();
            public IProducerConsumerCollection<IRequester> Queue { get; set; } = Substitute.For<IProducerConsumerCollection<IRequester>>();
            public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
            public TimeSpan EmptyQueueDelay { get; set; } = TimeSpan.FromSeconds(1);
            public TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromSeconds(2);
            public int BoundedCapacity { get; set; } = 40;

            public BackgroundRequesterFactory GetSut() 
                => new BackgroundRequesterFactory(
                    RequesterFactory, 
                    Queue, 
                    CancellationTokenSource, 
                    EmptyQueueDelay, 
                    ShutdownTimeout,
                    BoundedCapacity);
        }

        [Test]
        public void Ctor_StartsTask()
        {
            var fixture = new Fixture();
            using (var factory = fixture.GetSut())
            {
                Assert.That(factory.QueueConsumer, Is.Not.Null);
                Assert.That(factory.QueueConsumer.Status, Is.Not.EqualTo(TaskStatus.Running));
            }
        }

        [Test]
        public void Ctor_ZeroItemQueue_ThrowsArgumentOutOfRange()
        {
            var fixture = new Fixture();
            fixture.BoundedCapacity = 0;
            Assert.Throws<ArgumentOutOfRangeException>(() => fixture.GetSut(), "At least 1 item must be allowed in the queue.");
        }

        [Test]
        public void Dispose_StopsTask()
        {
            var fixture = new Fixture();
            fixture.ShutdownTimeout = default;
            var factory = fixture.GetSut();
            factory.Dispose();

            Assert.That(factory.QueueConsumer.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        }


        [Test]
        public void Create_CancelledTaskAndNoShutdownTimeout_ConsumesNoEvents()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.ShutdownTimeout = default;
            fixture.CancellationTokenSource.Cancel();

            // Act
            var factory = fixture.GetSut();
            // Make sure task has finished
            Assert.True(factory.QueueConsumer.Wait(TimeSpan.FromSeconds(2)));
            factory.Dispose(); // no-op as task is already finished

            // Assert
            IRequester outRequester;
            fixture.Queue.DidNotReceive().TryTake(out outRequester);
            Assert.That(factory.QueueConsumer.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        }


        [Test]
        public void Create_RequesterInvoked()
        {
            // Arrange
            var fixture = new Fixture();
            var expectedFeedback = new SentryUserFeedback();
            var expectedDsn = new Dsn(TestHelper.DsnUri);

            var actualId = "not set";
            var requesterMock = Substitute.For<IRequester>();
            requesterMock
                .When(r => r.UseEventId(Arg.Any<string>()))
                .Do(info => actualId = info.Arg<string>());

            fixture.Queue = new ConcurrentQueue<IRequester>();
            fixture.RequesterFactory.Create(expectedFeedback, expectedDsn).Returns(requesterMock);

            var factory = fixture.GetSut();

            // Act
            var requester = factory.Create(expectedFeedback, expectedDsn);
            var id = requester.Request();

            factory.Dispose();

            // Assert
            requesterMock.Received(1).RequestAsync();
            Assert.That(id, Is.EqualTo(actualId));
            Assert.That(factory.QueueConsumer.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        }


        [Test]
        public void Create_RequestData_RequesterInvoked()
        {
            // Arrange
            var fixture = new Fixture();
            var requestDataMock = new RequestData(new JsonPacket("project"), null);
            var expectedDsn = new Dsn(TestHelper.DsnUri);
            var expectedTimeout = TimeSpan.Zero;
            const bool useCompression = true;

            var actualId = "not set";
            var requesterMock = Substitute.For<IRequester>();
            requesterMock
                .When(r => r.UseEventId(Arg.Any<string>()))
                .Do(info => actualId = info.Arg<string>());

            fixture.Queue = new ConcurrentQueue<IRequester>();
            fixture.RequesterFactory.Create(requestDataMock, expectedDsn, expectedTimeout, useCompression)
                .Returns(requesterMock);

            var factory = fixture.GetSut();

            // Act
            var requester = factory.Create(requestDataMock, expectedDsn, expectedTimeout, useCompression);
            var id = requester.Request();

            factory.Dispose();

            // Assert
            requesterMock.Received(1).RequestAsync();
            Assert.That(id, Is.EqualTo(actualId));
            Assert.That(factory.QueueConsumer.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        }


        [Test]
        public void Create_RequesterThrows_WorkerSuppresses()
        {
            // Arrange
            var fixture = new Fixture();
            var expectedFeedback = new SentryUserFeedback();
            var expectedDsn = new Dsn(TestHelper.DsnUri);

            var requesterMock = Substitute.For<IRequester>();
            requesterMock
                .When(e => e.RequestAsync())
                .Do(_ => throw new Exception("Sending to sentry failed."));

            fixture.Queue = new ConcurrentQueue<IRequester>();
            fixture.RequesterFactory.Create(expectedFeedback, expectedDsn).Returns(requesterMock);

            var factory = fixture.GetSut();

            // Act
            var requester = factory.Create(expectedFeedback, expectedDsn);
            requester.Request();

            factory.Dispose();

            // Assert
            requesterMock.Received(1).RequestAsync();
            Assert.That(factory.QueueConsumer.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        }
    }
}
#endif