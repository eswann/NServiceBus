namespace NServiceBus.TimeoutPersisters.NHibernate.Tests
{
    using System;
    using NUnit.Framework;
    using Timeout.Core;

    [TestFixture]
    public class When_removing_timeouts_from_the_storage : InMemoryDBFixture
    {
        [Test]
        public void Should_remove_timeouts_by_id()
        {
            var t1 = new TimeoutData();

            persister.Add(t1);

            var t2 = new TimeoutData();

            persister.Add(t2);

            var t = persister.GetAll();

            foreach (var timeoutData in t)
            {
                persister.Remove(timeoutData.Id);
            }

            using (var session = sessionFactory.OpenSession())
            {
                Assert.Null(session.Get<TimeoutEntity>(new Guid(t1.Id)));
                Assert.Null(session.Get<TimeoutEntity>(new Guid(t2.Id)));
            }
        }
    }
}