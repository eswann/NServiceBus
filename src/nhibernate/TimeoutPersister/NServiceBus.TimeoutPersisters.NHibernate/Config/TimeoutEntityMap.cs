namespace NServiceBus.TimeoutPersisters.NHibernate.Config
{
    using NHibernate;
    using global::NHibernate.Mapping.ByCode;
    using global::NHibernate.Mapping.ByCode.Conformist;

    public class TimeoutEntityMap : ClassMapping<TimeoutEntity>
    {
        public TimeoutEntityMap()
        {
            Id(x => x.Id, m => m.Generator(Generators.Assigned));
            Property(p => p.State);
            Property(p => p.CorrelationId);
            Property(p => p.Destination, pm => pm.Type<AddressUserType>());
            Property(p => p.SagaId, pm => pm.Index("SagaIdIdx"));
            Property(p => p.Time);
            Map(c => c.Headers,
                mpm =>
                    {
                        mpm.Table("TimeoutEntityHeader");
                        mpm.Cascade(Cascade.All);
                        mpm.Lazy(CollectionLazy.NoLazy);
                        mpm.Fetch(CollectionFetchMode.Subselect);
                        mpm.Key(km => km.Column("TimeoutEntityId"));
                    },
                mkr => mkr.Element(mkm => mkm.Column("Name")),
                cer => cer.Element(em => em.Column("Value")));
        }
    }
}