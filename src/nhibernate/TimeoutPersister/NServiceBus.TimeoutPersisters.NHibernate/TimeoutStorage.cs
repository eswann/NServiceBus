namespace NServiceBus.TimeoutPersisters.NHibernate
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Timeout.Core;
    using global::NHibernate;

    public class TimeoutStorage : IPersistTimeouts
    {
        private readonly ISessionFactory sessionFactory;

        public TimeoutStorage(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public IEnumerable<TimeoutData> GetAll()
        {
            using (var session = sessionFactory.OpenStatelessSession())
            using (var tx = session.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var timeoutEntities = session.QueryOver<TimeoutEntity>()
                    .List();

                tx.Commit();

                return timeoutEntities.Select(te => new TimeoutData
                                                        {
                                                            CorrelationId = te.CorrelationId,
                                                            Destination = te.Destination,
                                                            Id = te.Id.ToString(),
                                                            SagaId = te.SagaId,
                                                            State = te.State,
                                                            Time = te.Time,
                                                            Headers = new Dictionary<string, string>(te.Headers),
                                                        });
            }
        }

        public void Add(TimeoutData timeout)
        {
            var newId = Guid.NewGuid();

            using (var session = sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                session.Save(new TimeoutEntity
                {
                    Id = newId,
                    CorrelationId = timeout.CorrelationId,
                    Destination = timeout.Destination,
                    SagaId = timeout.SagaId,
                    State = timeout.State,
                    Time = timeout.Time,
                    Headers = timeout.Headers,
                });

                tx.Commit();
            }

            timeout.Id = newId.ToString();
        }

        public void Remove(string timeoutId)
        {
            using (var session = sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var timeoutEntity = session.Load<TimeoutEntity>(Guid.Parse(timeoutId));
                session.Delete(timeoutEntity);

                tx.Commit();
            }
        }

        public void ClearTimeoutsFor(Guid sagaId)
        {
            using (var session = sessionFactory.OpenStatelessSession())
            using (var tx = session.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var queryString = string.Format("delete {0} where SagaId = :sagaid",
                                        typeof(TimeoutEntity));
                session.CreateQuery(queryString)
                       .SetParameter("sagaid", sagaId)
                       .ExecuteUpdate();

                tx.Commit();
            }
        }
    }
}
