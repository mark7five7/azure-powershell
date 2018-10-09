﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.Network
{
    using AutoMapper;
    using Microsoft.Azure.Commands.Network.Models;
    using Microsoft.Azure.Commands.ResourceManager.Common.Tags;
    using Microsoft.Azure.Management.Network;
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using System.Net;
    using MNM = Microsoft.Azure.Management.Network.Models;

    public class ExpressRouteConnectionBaseCmdlet : ExpressRouteGatewayBaseCmdlet
    {
        public IExpressRouteConnectionsOperations ExpressRouteConnectionClient
        {
            get
            {
                return NetworkClient.NetworkManagementClient.ExpressRouteConnections;
            }
        }

        public PSExpressRouteConnection ToPsExpressRouteConnection(Management.Network.Models.ExpressRouteConnection expressRouteConnection)
        {
            return NetworkResourceManagerProfile.Mapper.Map<PSExpressRouteConnection>(expressRouteConnection);
        }

        public PSExpressRouteConnection GetExpressRouteConnection(string resourceGroupName, string parentExpressRouteGatewayName, string name)
        {
            var expressRouteConnection = this.ExpressRouteConnectionClient.Get(resourceGroupName, parentExpressRouteGatewayName, name);
            return this.ToPsExpressRouteConnection(expressRouteConnection);
        }

        public List<PSExpressRouteConnection> ListExpressRouteConnections(string resourceGroupName, string parentExpressRouteGatewayName)
        {
            var expressRouteConnections = this.ExpressRouteConnectionClient.List(resourceGroupName, parentExpressRouteGatewayName);

            List<PSExpressRouteConnection> connectionsToReturn = new List<PSExpressRouteConnection>();
            if (expressRouteConnections != null)
            {
                foreach (MNM.ExpressRouteConnection connection in expressRouteConnections.Value)
                {
                    connectionsToReturn.Add(ToPsExpressRouteConnection(connection));
                }
            }

            return connectionsToReturn;
        }

        public bool IsExpressRouteConnectionPresent(string resourceGroupName, string parentExpressRouteGatewayName, string name)
        {
            try
            {
                this.GetExpressRouteConnection(resourceGroupName, parentExpressRouteGatewayName, name);
            }
            catch (Microsoft.Azure.Management.Network.Models.ErrorException exception)
            {
                if (exception.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    // Resource is not present
                    return false;
                }
            }

            return true;
        }
    }
}
