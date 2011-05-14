#define INTEGRATION_TEST

using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate;
using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using Xunit;

using Environment = NHibernate.Cfg.Environment;

public sealed class DynamicEntityTest
{
    [Fact]
    public void SetPropertyWorks()
    {
        dynamic currency = new DynamicEntity("Currency");

        currency.IsEnabled = true;
        Assert.True(currency.IsEnabled);

        currency.IsEnabled = false;
        Assert.False(currency.IsEnabled);
    }

    [Fact]
    public void GetPropertyWorks()
    {
        dynamic currency = new DynamicEntity("Currency");

        currency.ExchangeRateUpdatedOn = DateTime.UtcNow;
        Assert.NotNull(currency.ExchangeRateUpdatedOn);
    }

    [Fact]
    public void GetNameWorks()
    {
        dynamic currency = new DynamicEntity("Currency");

        Assert.Equal("Currency", currency.Name);
    }

    [Fact]
    public void GetMapWorks()
    {
        dynamic currency = new DynamicEntity("Currency");

        Assert.Equal(typeof(Dictionary<String, Object>), currency.Map.GetType());
    }
}

#if INTEGRATION_TEST

public sealed class DynamicEntityNHibernateTest : InMemoryDatabaseTest
{
    public DynamicEntityNHibernateTest()
        : base(typeof(DynamicEntityNHibernateTest).Assembly) { }

    [Fact]
    public void NHibernateShouldBeAbleToPersistCurrency()
    {
        dynamic currency = new DynamicEntity("Currency");

        currency.ISOCode                   = "GBP";
        currency.EnglishName               = "United Kingdom Pound";
        currency.ExchangeRateEURtoCurrency = 0.87780;
        currency.ExchangeRateUpdatedOn     = DateTime.UtcNow;
        currency.IsEnabled                 = true;
        currency.Symbol                    = null;

        Object id;

        using (var tx = Session.BeginTransaction())
        {
            id = Session.Save(currency.Name, currency.Map);
            tx.Commit();

            Assert.NotNull(id);
        }

        Session.Clear();

        using (var tx = Session.BeginTransaction())
        {
            var loadedCurrency = Session.Load(currency.Name, id);
            tx.Commit();

            Assert.NotNull(loadedCurrency);
        }

        Session.Flush();
    }
}

#region InMemoryDatabaseTest

// http://ayende.com/Blog/archive/2009/04/28/nhibernate-unit-testing.aspx
public abstract class InMemoryDatabaseTest : IDisposable
{
    private readonly ISessionFactory m_sessionFactory;
    private readonly ISession m_session;

    private static Configuration s_cfg;

    public InMemoryDatabaseTest(Assembly assemblyContainingMapping)
    {
        if (s_cfg == null)
        {
            s_cfg = new Configuration()
                .SetProperty(Environment.ReleaseConnections, "on_close")
                .SetProperty(Environment.Dialect, typeof(SQLiteDialect).AssemblyQualifiedName)
                .SetProperty(Environment.ConnectionDriver, typeof(SQLite20Driver).AssemblyQualifiedName)
                .SetProperty(Environment.ConnectionString, "data source=:memory:")
                .SetProperty(Environment.ProxyFactoryFactoryClass, typeof(ProxyFactoryFactory).AssemblyQualifiedName)
                .SetProperty(Environment.ShowSql, "true")
                .AddAssembly(assemblyContainingMapping);

            m_sessionFactory = s_cfg.BuildSessionFactory();
        }

        m_session = m_sessionFactory.OpenSession().GetSession(EntityMode.Map);

        new SchemaExport(s_cfg).Execute(script: false, export: true, justDrop: false,
            connection: Session.Connection, exportOutput: Console.Out);
    }

    protected ISessionFactory SessionFactory
    {
        get
        {
            return m_sessionFactory;
        }
    }

    protected ISession Session
    {
        get
        {
            return m_session;
        }
    }

    public void Dispose()
    {
        Session.Dispose();
    }
}

#endregion

#endif
