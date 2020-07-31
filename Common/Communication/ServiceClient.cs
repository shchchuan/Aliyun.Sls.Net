/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Aliyun.Sls;
using Aliyun.Sls.Utilities;
using Aliyun.Sls.Common;
using Aliyun.Sls.Common.Communication;
using Aliyun.Sls.Common.Handlers;
using Aliyun.Sls.Common.Utilities;

namespace Aliyun.Sls.Common.Communication
{
    /// <summary>
    /// The default implementation of <see cref="IServiceClient" />.
    /// </summary>
    internal abstract class ServiceClient : IServiceClient
    {

        #region Fields and Properties

        private ClientConfiguration _configuration;
        
        public ClientConfiguration Configuration
        {
            get { return _configuration; }
        }

        #endregion

        #region Constructors

        protected ServiceClient(ClientConfiguration configuration)
        {
            Debug.Assert(configuration != null);
            // Make a definsive copy to ensure the class is immutable.
            _configuration = (ClientConfiguration)configuration.Clone();
        }
        
        public static ServiceClient Create(ClientConfiguration configuration)
        {
            return new ServiceClientImpl(configuration);
        }

        #endregion
        
        #region IServiceClient Members

        public ServiceResponse Send(ServiceRequest request, ExecutionContext context)
        {
            Debug.Assert(request != null);
            SignRequest(request, context);
            ServiceResponse response = SendCore(request, context);
            HandleResponse(response, context.ResponseHandlers);
            return response;
        }

        #endregion

        protected abstract ServiceResponse SendCore(ServiceRequest request, ExecutionContext context);
        
        
        internal static void SignRequest(ServiceRequest request, ExecutionContext context)
        {
            if (context.Signer != null)
            {
                context.Signer.Sign(request, context.Credentials);
            }
        }
        
        protected static void HandleResponse(ServiceResponse response, IList<IResponseHandler> handlers)
        {
            foreach(IResponseHandler handler in handlers)
            {
                handler.Handle(response);
            }
        }

    }
}
