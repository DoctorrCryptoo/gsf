﻿//******************************************************************************************************
//  AuthenticationMiddleware.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  08/25/2017 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Hosting;
using GSF.Diagnostics;
using GSF.Security;
using Microsoft.Owin;
using Owin;

namespace GSF.Web.Security
{
    /// <summary>
    /// Middle-ware for configuring authentication using <see cref="ISecurityProvider"/> in the Owin pipeline.
    /// </summary>
    public class AuthenticationMiddleware : OwinMiddleware
    {
        private AuthenticationOptions Options { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="AuthenticationMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middle-ware object in the pipeline.</param>
        /// <param name="options">The options for authentication.</param>
        public AuthenticationMiddleware(OwinMiddleware next, AuthenticationOptions options)
            : base(next)
        {
            Options = options;
        }

        /// <inheritdoc/>
        public override async Task Invoke(IOwinContext context)
        {
            AuthenticationHandler handler = new(context, Options);
            handler.Authenticate();

            if (await handler.AuthorizeAsync())
                await Next.Invoke(context);
        }
    }

    /// <summary>
    /// Represents <see cref="IAppBuilder"/> extension functions for GSF authentication middle-ware.
    /// </summary>
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Inserts GSF role-based security authentication into the Owin pipeline.
        /// </summary>
        /// <param name="app">Target <see cref="IAppBuilder"/> instance.</param>
        /// <param name="options">Authentication options.</param>
        /// <returns><see cref="IAppBuilder"/> instance.</returns>
        public static IAppBuilder UseAuthentication(this IAppBuilder app, AuthenticationOptions options)
        {
            // Only self hosted Owin web server has access to HttpListener
            if (!HostingEnvironment.IsHosted)
            {
                HttpListener listener = (HttpListener)app.Properties["System.Net.HttpListener"];
                listener.AuthenticationSchemeSelectorDelegate = request => AuthenticationSchemeSelector(request, options);
                listener.Realm = options.Realm;
            }

            return app.Use<AuthenticationMiddleware>(options);
        }

        // Scheme selector
        private static AuthenticationSchemes AuthenticationSchemeSelector(HttpListenerRequest request, AuthenticationOptions options)
        {
            // Only change authentication scheme when requesting the authorization test page
            if (!request.Url.AbsolutePath.Equals(options.AuthTestPage))
            {
                // All other requests to web server are treated as anonymous so as to not establish any extra
                // expectations for the browser - the AuthenticationHandler fully manages the security
                return AuthenticationSchemes.Anonymous;
            }

            string scheme = request.QueryString.GetValues("scheme")?.FirstOrDefault();

            if (!Enum.TryParse(scheme, true, out AuthenticationSchemes authenticationScheme))
                return AuthenticationSchemes.Anonymous; // Unrecognized auth scheme

            AuthenticationSchemes selectedScheme = options.AuthenticationSchemes & authenticationScheme;
            
            Log.Publish(MessageLevel.Debug, "AuthenticationSchemeSelected", $"Authentication scheme selected for {request.Url.PathAndQuery}: {selectedScheme}");
            
            return selectedScheme;
        }

        private static readonly LogPublisher Log = Logger.CreatePublisher(typeof(AppBuilderExtensions), MessageClass.Framework);
    }
}
