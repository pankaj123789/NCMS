//#region Licence
///*
//New BSD License for S#arp Architecture from Codai, Inc.
 
//Copyright (c) 2009, Codai, Inc.
//All rights reserved.
 
//Redistribution and use in source and binary forms, with or without modification,
//are permitted provided that the following conditions are met:
 
//    * Redistributions of source code must retain the above copyright notice,
//      this list of conditions and the following disclaimer.
 
//    * Redistributions in binary form must reproduce the above copyright notice,
//      this list of conditions and the following disclaimer in the documentation
//      and/or other materials provided with the distribution.
 
//    * Neither the name of Codai, Inc., nor the names of its
//      contributors may be used to endorse or promote products derived from this
//      software without specific prior written permission.
 
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
//ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
//ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//*/
//#endregion

//using System;
//using System.ServiceModel;
//using Castle.Core;
//using Castle.Core.Configuration;
//using Castle.MicroKernel;

//namespace F1Solutions.NAATI.ePortal.Web.CastleWindsor
//{
//    /// <summary>
//    ///     This facility may be registered within your web application to automatically look for and close
//    ///     WCF connections.  This eliminates all the redundant code for closing the connection and aborting
//    ///     if any appropriate exceptions are encountered.  See documentation for setting up and using this
//    ///     Castle facility.
//    /// </summary>
//    public class WcfSessionFacility : IFacility
//    {
//        public const string ManageWcfSessionsKey = "ManageWcfSessions";

//        public void Init(IKernel kernel, IConfiguration facilityConfig)
//        {
//            kernel.ComponentDestroyed += KernelComponentDestroyed;

//        }

//        public void Terminate()
//        {
//        }

//        private static void KernelComponentDestroyed(ComponentModel model, object instance)
//        {
//            var shouldManage = false;
//            var value = model.ExtendedProperties[ManageWcfSessionsKey];

//            if (value != null)
//            {
//                shouldManage = (bool)value;
//            }

//            if (!shouldManage)
//            {
//                return;
//            }

//            var communicationObject = instance as ICommunicationObject;

//            if (communicationObject != null)
//            {
//                try
//                {
//                    if (communicationObject.State != CommunicationState.Faulted)
//                    {
//                        communicationObject.Close();
//                    }
//                    else
//                    {
//                        communicationObject.Abort();
//                    }
//                }
//                catch (CommunicationException)
//                {
//                    communicationObject.Abort();
//                }
//                catch (TimeoutException)
//                {
//                    communicationObject.Abort();
//                }
//                catch (Exception)
//                {
//                    communicationObject.Abort();
//                    throw;
//                }
//            }
//        }
//    }
//}