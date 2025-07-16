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
using System.Diagnostics;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;

namespace F1Solutions.Naati.Common.Dal.NHibernate.Configuration
{
    /// <summary>
    /// Invoked by Global.asax.cx, or wherever you can to initialize NHibernate, to guarentee that 
    /// NHibernate is only initialized once while working in IIS 7 integrated mode.  But note 
    /// that this is not web specific, although that is the realm that prompted its creation.
    /// 
    /// In a web context, it should be invoked from Application_BeginRequest with the NHibernateSession.Init()
    /// function being passed as a parameter to InitializeNHiberate()
    /// </summary>
    public class NHibernateInitializer
    {
        private static readonly object syncLock = new object();
        private static NHibernateInitializer instance;

        protected NHibernateInitializer() { }

        private bool nHibernateSessionIsLoaded = false;

        public static NHibernateInitializer Instance()
        {
            if (instance == null)
            {
                lock (syncLock)
                {
                    if (instance == null)
                    {
                        instance = new NHibernateInitializer();
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// This is the method which should be given the call to intialize the NHibernateSession; e.g.,
        /// NHibernateInitializer.Instance().InitializeNHibernateOnce(() => InitializeNHibernateSession());
        /// where InitializeNHibernateSession() is a method which calls NHibernateSession.Init()
        /// </summary>
        /// <param name="initMethod"></param>
        public void InitializeNHibernateOnce(Action initMethod)
        {
            lock (syncLock)
            {
                if (!nHibernateSessionIsLoaded)
                {
                    initMethod();
                    nHibernateSessionIsLoaded = true;
                }
            }
        }
    }

    public class CustomMsSql2000Dialect : MsSql2012Dialect
    {
        public CustomMsSql2000Dialect()
        {
            Debugger.Break();
            RegisterFunction("contains", new StandardSQLFunction("contains", null));
        }
    }
}
