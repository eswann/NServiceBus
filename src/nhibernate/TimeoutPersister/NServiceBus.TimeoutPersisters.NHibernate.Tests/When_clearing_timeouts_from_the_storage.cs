using System;
using System.Collections.Generic;
using NServiceBus.Timeout.Core;
using NUnit.Framework;

namespace NServiceBus.TimeoutPersisters.NHibernate.Tests
{
    [TestFixture]
    public class When_clearing_timeouts_from_the_storage : InMemoryDBFixture
    {
        [Test]
        public void Should_clear_timeouts_by_sagaid()
        {
            var sagaId = Guid.NewGuid();

            var t1 = new TimeoutData { Time = DateTime.Now.AddYears(1), SagaId = sagaId, Headers = new Dictionary<string, string> { { "Header1", "Value1" } } };

            persister.Add(t1);

            var t2 = new TimeoutData { Time = DateTime.Now.AddYears(1), SagaId = sagaId, Headers = new Dictionary<string, string> { { "Header1", "Value1" } } };

            persister.Add(t2);

            persister.ClearTimeoutsFor(sagaId);

            using (var session = sessionFactory.OpenSession())
            {
                Assert.Null(session.Get<TimeoutEntity>(new Guid(t1.Id)));
                Assert.Null(session.Get<TimeoutEntity>(new Guid(t2.Id)));
            }
        }
    }
}