#region Licence
/*
New BSD License for S#arp Architecture from Codai, Inc.
 
Copyright (c) 2009, Codai, Inc.
All rights reserved.
 
Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:
 
    * Redistributions of source code must retain the above copyright notice,
      this list of conditions and the following disclaimer.
 
    * Redistributions in binary form must reproduce the above copyright notice,
      this list of conditions and the following disclaimer in the documentation
      and/or other materials provided with the distribution.
 
    * Neither the name of Codai, Inc., nor the names of its
      contributors may be used to endorse or promote products derived from this
      software without specific prior written permission.
 
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using F1Solutions.Naati.Common.Dal.NHibernate.SharpArchitecture;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Context;

namespace F1Solutions.Naati.Common.Dal.NHibernate.Configuration
{
    public static class NHibernateSession
	{
		#region Init() overloads

		public static global::NHibernate.Cfg.Configuration Init(ISessionStorage storage, string[] mappingAssemblies)
		{
			return Init(storage, mappingAssemblies, null, null, null, null, null);
		}

		public static global::NHibernate.Cfg.Configuration Init(ISessionStorage storage, string[] mappingAssemblies, string cfgFile)
		{
			return Init(storage, mappingAssemblies, null, cfgFile, null, null, null);
		}

		public static global::NHibernate.Cfg.Configuration Init(ISessionStorage storage, string[] mappingAssemblies, IDictionary<string, string> cfgProperties)
		{
			return Init(storage, mappingAssemblies, null, null, cfgProperties, null, null);
		}

		public static global::NHibernate.Cfg.Configuration Init(ISessionStorage storage, string[] mappingAssemblies, string cfgFile, string validatorCfgFile)
		{
			return Init(storage, mappingAssemblies, null, cfgFile, null, validatorCfgFile, null);
		}

		public static global::NHibernate.Cfg.Configuration Init(ISessionStorage storage, string[] mappingAssemblies, AutoPersistenceModel autoPersistenceModel)
		{
			return Init(storage, mappingAssemblies, autoPersistenceModel, null, null, null, null);
		}

		public static global::NHibernate.Cfg.Configuration Init(ISessionStorage storage, string[] mappingAssemblies, AutoPersistenceModel autoPersistenceModel, string cfgFile)
		{
			return Init(storage, mappingAssemblies, autoPersistenceModel, cfgFile, null, null, null);
		}

		public static global::NHibernate.Cfg.Configuration Init(ISessionStorage storage, string[] mappingAssemblies, AutoPersistenceModel autoPersistenceModel, IDictionary<string, string> cfgProperties)
		{
			return Init(storage, mappingAssemblies, autoPersistenceModel, null, cfgProperties, null, null);
		}

		public static global::NHibernate.Cfg.Configuration Init(
			ISessionStorage storage, 
			string[] mappingAssemblies,
			AutoPersistenceModel autoPersistenceModel,
			string cfgFile,
			string validatorCfgFile)
		{
			return Init(storage, mappingAssemblies, autoPersistenceModel, cfgFile, null, validatorCfgFile, null);
		}

		public static global::NHibernate.Cfg.Configuration Init(
			ISessionStorage storage, 
			string[] mappingAssemblies,
			AutoPersistenceModel autoPersistenceModel,
			string cfgFile,
			IDictionary<string, string> cfgProperties,
			string validatorCfgFile,
            string connectionString)
		{
			return Init(storage, mappingAssemblies, autoPersistenceModel, cfgFile, cfgProperties, validatorCfgFile, null, connectionString);
		}

		public static global::NHibernate.Cfg.Configuration Init(
			ISessionStorage storage,
			string[] mappingAssemblies,
			AutoPersistenceModel autoPersistenceModel,
			string cfgFile,
			IDictionary<string, string> cfgProperties,
			string validatorCfgFile,
			IPersistenceConfigurer persistenceConfigurer,
            string connectionString)
		{
			InitStorage(storage);
			return AddConfiguration(DefaultFactoryKey, mappingAssemblies, autoPersistenceModel, cfgFile, cfgProperties, validatorCfgFile, persistenceConfigurer, connectionString);
		}

		#endregion

		public static void InitStorage(ISessionStorage storage)
		{
			Check.Require(storage != null, "storage mechanism was null but must be provided");
			//Check.Require(Storage == null, "A storage mechanism has already been configured for this application");
			Storage = storage;
		}

		public static global::NHibernate.Cfg.Configuration AddConfiguration(
			string factoryKey,
			string[] mappingAssemblies,
			AutoPersistenceModel autoPersistenceModel,
			string cfgFile,
			IDictionary<string, string> cfgProperties,
			string validatorCfgFile,
			IPersistenceConfigurer persistenceConfigurer,
            string connectionString)
		{
			global::NHibernate.Cfg.Configuration cfg = ConfigureNHibernate(cfgFile, cfgProperties);			

			Check.Require(!sessionFactories.ContainsKey(factoryKey),
				"A session factory has already been configured with the key of " + factoryKey);

			sessionFactories.Add(
				factoryKey,
				CreateSessionFactoryFor(mappingAssemblies, autoPersistenceModel, cfg, persistenceConfigurer, connectionString));

			return cfg;
		}

        /// <summary>
        /// Used to get the current NHibernate session if you're communicating with a single database.
        /// When communicating with multiple databases, invoke <see cref="Current" /> instead.
        /// </summary>
        public static ISession Current
		{
			get
			{
				Check.Require(!IsConfiguredForMultipleDatabases(),
					"The NHibernateSession.Current property may " +
					"only be invoked if you only have one NHibernate session factory; i.e., you're " +
					"only communicating with one database.  Since you're configured communications " +
					"with multiple databases, you should instead call CurrentFor(string factoryKey)");

				return CurrentFor(DefaultFactoryKey);
			}
		}

        public static void RegisterInterceptor(IInterceptor interceptor)
		{
			Check.Require(interceptor != null, "interceptor may not be null");

			RegisteredInterceptor = interceptor;
		}

		public static bool IsConfiguredForMultipleDatabases()
		{
			return sessionFactories.Count > 1;
		}

		/// <summary>
		/// Used to get the current NHibernate session associated with a factory key; i.e., the key 
		/// associated with an NHibernate session factory for a specific database.
		/// 
		/// If you're only communicating with one database, you should call <see cref="Current" /> instead,
		/// although you're certainly welcome to call this if you have the factory key available.
		/// </summary>
		public static ISession CurrentFor(string factoryKey)
		{
			Check.Require(!string.IsNullOrEmpty(factoryKey), "factoryKey may not be null or empty");
			Check.Require(Storage != null, "An ISessionStorage has not been configured");
			Check.Require(sessionFactories.ContainsKey(factoryKey), "An ISessionFactory does not exist with a factory key of " + factoryKey);

		    var sessionFactory = sessionFactories[factoryKey];

		    if (!CurrentSessionContext.HasBind(sessionFactory))
		    {
		        ISession session;
		        if (RegisteredInterceptor != null)
		        {
		            session = sessionFactories[factoryKey].WithOptions().Interceptor(RegisteredInterceptor).OpenSession();
		        }
		        else
		        {
		            session = sessionFactories[factoryKey].OpenSession();
		        }

		        CurrentSessionContext.Bind(session);
		        return session;
		    }
		    else
		    {
		        return sessionFactory.GetCurrentSession();
            }

		
   //         ISession session = Storage.GetSessionForKey(factoryKey);

			//if (session == null)
			//{
			//	if (RegisteredInterceptor != null)
			//	{
			//		session = sessionFactories[factoryKey].WithOptions().Interceptor(RegisteredInterceptor).OpenSession();
			//	}
			//	else
			//	{
			//		session = sessionFactories[factoryKey].OpenSession();
			//	}

			//	Storage.SetSessionForKey(factoryKey, session);
			//}

			//return session;
		}

		/// <summary>
		/// This method is used by application-specific session storage implementations
		/// and unit tests. Its job is to walk thru existing cached sessions and Close() each one.
		/// </summary>
		public static void CloseAllSessions()
		{
		    foreach (var sessionFactory in sessionFactories.Values)
		    {
		        if (CurrentSessionContext.HasBind(sessionFactory))
		        {
                    var session = CurrentSessionContext.Unbind(sessionFactory);
                    session.Flush();
                    session.Dispose();
		        }
            }
            //if (Storage != null)
            //{
            //    foreach (ISession session in Storage.GetAllSessions())
            //    {
            //        if (session.IsOpen)
            //            session.Close();
            //    }
            //}
        }

		/// <summary>
		/// To facilitate unit testing, this method will reset this object back to its
		/// original state before it was configured.
		/// </summary>
		public static void Reset()
		{
			foreach (ISession session in Storage.GetAllSessions())
			{
				session.Dispose();
			}

			sessionFactories.Clear();

			Storage = null;
			RegisteredInterceptor = null;
		}

		/// <summary>
		/// Return an ISessionFactory based on the specified factoryKey.
		/// </summary>
		public static ISessionFactory GetSessionFactoryFor(string factoryKey)
		{
            if (!sessionFactories.ContainsKey(factoryKey))
                return null;

			return sessionFactories[factoryKey];
		}

        public static void RemoveSessionFactoryFor(string factoryKey) {
            if (GetSessionFactoryFor(factoryKey) != null) {
                sessionFactories.Remove(factoryKey);
            }
        }

		/// <summary>
		/// Returns the default ISessionFactory using the DefaultFactoryKey.
		/// </summary>
		public static ISessionFactory GetDefaultSessionFactory()
		{
			return GetSessionFactoryFor(DefaultFactoryKey);
		}

		/// <summary>
		/// The default factory key used if only one database is being communicated with.
		/// </summary>
		public static readonly string DefaultFactoryKey = "nhibernate.current_session";

        /// <summary>
        /// An application-specific implementation of ISessionStorage must be setup either thru
        /// <see cref="InitStorage" /> or one of the <see cref="Init" /> overloads. 
        /// </summary>
        public static ISessionStorage Storage { get; set; }

		private static ISessionFactory CreateSessionFactoryFor(
			string[] mappingAssemblies,
			AutoPersistenceModel autoPersistenceModel,
			global::NHibernate.Cfg.Configuration cfg,
			IPersistenceConfigurer persistenceConfigurer,
            string connectionString)
		{
            FluentConfiguration fluentConfiguration = Fluently.Configure(cfg).CurrentSessionContext<WebSessionContext>();
            //    .ExposeConfiguration(c =>
            //{
            //    var listerner = new AuditingListener();

            //    c.EventListeners.PreInsertEventListeners = c.EventListeners.PreInsertEventListeners.Concat(new[] { listerner }).ToArray();
            //    c.EventListeners.PostInsertEventListeners = c.EventListeners.PostInsertEventListeners.Concat(new[] { listerner }).ToArray();
            //    //c.EventListeners.PreDeleteEventListeners = c.EventListeners.PreDeleteEventListeners.Concat(new[] { listerner }).ToArray();
            //    //c.EventListeners.PreUpdateEventListeners = c.EventListeners.PreUpdateEventListeners.Concat(new[] { listerner }).ToArray();
            //    c.EventListeners.PostUpdateEventListeners = c.EventListeners.PostUpdateEventListeners.Concat(new[] { listerner }).ToArray();
            //    c.EventListeners.PostDeleteEventListeners = c.EventListeners.PostDeleteEventListeners.Concat(new[] { listerner }).ToArray();
            //});

            if (persistenceConfigurer != null)
		    {
		        fluentConfiguration.Database(persistenceConfigurer);
		    }
		    else
		    {
		        fluentConfiguration = fluentConfiguration.Database( 
		            MsSqlConfiguration.MsSql2012.ConnectionString(c => c.Is(connectionString)));

		    }

			fluentConfiguration.Mappings(m =>
			{
				foreach (var mappingAssembly in mappingAssemblies)
				{
					var assembly = Assembly.LoadFrom(MakeLoadReadyAssemblyName(mappingAssembly));

					m.HbmMappings.AddFromAssembly(assembly);
					m.FluentMappings.AddFromAssembly(assembly)
						.Conventions.AddAssembly(assembly);
				}

				if (autoPersistenceModel != null)
				{
					m.AutoMappings.Add(autoPersistenceModel);
				}
			});

			return fluentConfiguration.BuildSessionFactory();
		}

		private static string MakeLoadReadyAssemblyName(string assemblyName)
		{
			return (assemblyName.IndexOf(".dll") == -1)
				? assemblyName.Trim() + ".dll"
				: assemblyName.Trim();
		}

		private static global::NHibernate.Cfg.Configuration ConfigureNHibernate(string cfgFile, IDictionary<string, string> cfgProperties)
		{
			global::NHibernate.Cfg.Configuration cfg = new global::NHibernate.Cfg.Configuration();

			if (cfgProperties != null)
				cfg.AddProperties(cfgProperties);

            if (string.IsNullOrEmpty(cfgFile) == false)
                return cfg.Configure(cfgFile);

            if (File.Exists("Hibernate.cfg.xml"))
                return cfg.Configure();

            return cfg;
		}


		private static IInterceptor RegisteredInterceptor;

		/// <summary>
		/// Maintains a dictionary of NHibernate session factories, one per database.  The key is 
		/// the "factory key" used to look up the associated database, and used to decorate respective
		/// repositories.  If only one database is being used, this dictionary contains a single
		/// factory with a key of <see cref="DefaultFactoryKey" />.
		/// </summary>
		private static IDictionary<string, ISessionFactory> sessionFactories = new ConcurrentDictionary<string, ISessionFactory>();
    }
}
