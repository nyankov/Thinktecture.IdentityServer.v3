﻿/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license
 */

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Models;

namespace Thinktecture.IdentityServer.Core.Authentication
{
    public class LoginResult : IHttpActionResult
    {
        SignInMessage _message;
        HttpRequestMessage _request;
        private CoreSettings _settings;
        private InternalConfiguration _internalConfig;

        public LoginResult(SignInMessage message, HttpRequestMessage request, CoreSettings settings, InternalConfiguration internalConfig)
        {
            _message = message;
            _settings = settings;
            _request = request;
            _internalConfig = internalConfig;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Redirect);

            try
            {
                var sim = _message.Protect(60, _internalConfig.DataProtector);

                var urlHelper = _request.GetUrlHelper();
                var loginUrl = urlHelper.Route(Constants.RouteNames.Login, new { message = sim });
                var uri = new Uri(_request.RequestUri, loginUrl);

                response.Headers.Location = uri;
            }
            catch
            {
                response.Dispose();
                throw;
            }

            return response;
        }
    }
}