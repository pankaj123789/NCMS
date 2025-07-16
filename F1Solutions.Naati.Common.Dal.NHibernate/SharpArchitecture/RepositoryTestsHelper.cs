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
using System.Configuration;
using System.Data.Common;
using System.Reflection;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using FluentNHibernate.Automapping;
using NHibernate.Tool.hbm2ddl;

namespace F1Solutions.Naati.Common.Dal.NHibernate.SharpArchitecture
{
    /// <summary>
    /// Provides helper methods for consolidating duplicated code from test fixture base classes.
    /// </summary>
    public class RepositoryTestsHelper
    {
        public static void InitializeDatabase() {
            global::NHibernate.Cfg.Configuration cfg = InitializeNHibernateSession();
            DbConnection connection = NHibernateSession.Current.Connection;
            new SchemaExport(cfg).Execute(false, true, false, connection, null);
        }

        public static global::NHibernate.Cfg.Configuration InitializeNHibernateSession() {
            string[] mappingAssemblies = GetMappingAssemblies();
            AutoPersistenceModel autoPersistenceModel = GetAutoPersistenceModel(mappingAssemblies);
            return NHibernateSession.Init(new SimpleSessionStorage(), mappingAssemblies, autoPersistenceModel);
        }

		public static void Shutdown()
		{
			NHibernateSession.CloseAllSessions();
			NHibernateSession.Reset();
		}

		public static void FlushSessionAndEvict(object instance)
		{
			// Commits any changes up to this point to the database
			NHibernateSession.Current.Flush();

			// Evicts the instance from the current session so that it can be loaded during testing;
			// this gives the test a clean slate, if you will, to work with
			NHibernateSession.Current.Evict(instance);
		}

        public static string[] GetMappingAssemblies() {
            string mappingAssembliesSetting = ConfigurationManager.AppSettings["nhibernate.mapping.assembly"];

            Check.Require(!string.IsNullOrEmpty(mappingAssembliesSetting),
                "Please add an AppSetting to your app.config for 'nhibernate.mapping.assembly.' This setting " +
                "takes a comma delimited list of assemblies containing NHibernate mapping files. Including '.dll' " +
                "at the end of each is optional.");

            return mappingAssembliesSetting.Split(',');
        }

        public static AutoPersistenceModel GetAutoPersistenceModel(string[] assemblies) {
            foreach (var asmName in assemblies) {
                Assembly asm = Assembly.Load(asmName);
                Type[] asmTypes = asm.GetTypes();

                foreach (Type asmType in asmTypes) {
                    if (typeof(IAutoPersistenceModelGenerator).IsAssignableFrom(asmType)) {
                        IAutoPersistenceModelGenerator generator = Activator.CreateInstance(asmType) as IAutoPersistenceModelGenerator;
                        return generator.Generate();
                    }
                }
            }

            return null;
        }
	}
}