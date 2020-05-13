//******************************************************************************************************
//  FtpSessionDisconnected.cs - Gbtc
//
//  Copyright � 2012, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  05/22/2003 - J. Ritchie Carroll
//       Generated original version of source code.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/17/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

#region [ Contributor License Agreements ]

//*******************************************************************************************************
//
//   Code based on the following project:
//        http://www.codeproject.com/KB/IP/net_ftp_upload.aspx
//  
//   Copyright Alex Kwok & Uwe Keim 
//
//   The Code Project Open License (CPOL):
//        http://www.codeproject.com/info/cpol10.aspx
//
//*******************************************************************************************************

#endregion

using System;
using System.Net;

namespace GSF.Net.Ftp
{
    internal class FtpSessionDisconnected : IFtpSessionState
    {
        #region [ Members ]

        // Fields
        private FtpClient m_host;
        private readonly bool m_caseInsensitive;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        internal FtpSessionDisconnected(FtpClient h, bool caseInsensitive)
        {
            Passive = true;
            Timeout = 30000;
            Port = 21;
            m_host = h;
            m_caseInsensitive = caseInsensitive;
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="FtpSessionDisconnected"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~FtpSessionDisconnected()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        public string Server { get; set; }

        public int Port { get; set; }

        public int Timeout { get; set; }

        public bool Passive { get; set; }

        public IPAddress ActiveAddress { get; set; }

        public int MinActivePort { get; set; }

        public int MaxActivePort { get; set; }

        public FtpDirectory CurrentDirectory
        {
            get => throw new InvalidOperationException();
            set => throw new InvalidOperationException();
        }

        public FtpControlChannel ControlChannel => throw new InvalidOperationException();

        public bool IsBusy => throw new InvalidOperationException();

        public FtpDirectory RootDirectory => throw new InvalidOperationException();

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases all the resources used by the <see cref="FtpSessionDisconnected"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="FtpSessionDisconnected"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            try
            {
                if (disposing)
                    m_host = null;
            }
            finally
            {
                m_disposed = true;  // Prevent duplicate dispose.
            }
        }

        public void Connect(string userName, string password)
        {
            FtpControlChannel ctrl = new FtpControlChannel(m_host)
            {
                Server = Server,
                Port = Port,
                Timeout = Timeout,
                Passive = Passive,
                ActiveAddress = ActiveAddress
            };

            if (MinActivePort > 0 && MaxActivePort > 0)
                ctrl.DataChannelPortRange = new Range<int>(MinActivePort, MaxActivePort);

            ctrl.Connect();

            try
            {
                ctrl.Command($"USER {userName}");

                if (ctrl.LastResponse.Code == FtpResponse.UserAcceptedWaitingPass)
                    ctrl.Command($"PASS {password}");

                if (ctrl.LastResponse.Code != FtpResponse.UserLoggedIn)
                    throw new FtpAuthenticationException("Failed to login.", ctrl.LastResponse);

                m_host.State = new FtpSessionConnected(m_host, ctrl, m_caseInsensitive);
                ((FtpSessionConnected)m_host.State).InitRootDirectory();
            }
            catch
            {
                ctrl.Close();
                throw;
            }
        }


        public void AbortTransfer()
        {
            throw new InvalidOperationException();
        }

        public void Close()
        {
            // Nothing to do - don't want to throw an error...
        }

        #endregion
    }
}