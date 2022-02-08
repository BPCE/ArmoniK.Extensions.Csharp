// This file is part of the ArmoniK project
// 
// Copyright (C) ANEO, 2021-2021. All rights reserved.
//   W. Kirschenmann   <wkirschenmann@aneo.fr>
//   J. Gurhem         <jgurhem@aneo.fr>
//   D. Dubuc          <ddubuc@aneo.fr>
//   L. Ziane Khodja   <lzianekhodja@aneo.fr>
//   F. Lemaitre       <flemaitre@aneo.fr>
//   S. Djebbar        <sdjebbar@aneo.fr>
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ArmoniK.Core.gRPC.V1;
using ArmoniK.DevelopmentKit.Common;
using ArmoniK.DevelopmentKit.SymphonyApi.Client.api;
using ArmoniK.DevelopmentKit.WorkerApi.Common;

using Google.Protobuf;

using Grpc.Net.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace ArmoniK.DevelopmentKit.SymphonyApi.Client
{
  /// <summary>
  ///   The main object to communicate with the control Plane from the client side
  ///   The class will connect to the control plane to createSession, SubmitTask,
  ///   Wait for result and get the result.
  ///   See an example in the project ArmoniK.Samples in the sub project
  ///   https://github.com/aneoconsulting/ArmoniK.Samples/tree/main/Samples/SymphonyLike
  ///   Samples.ArmoniK.Sample.SymphonyClient
  /// </summary>
  [MarkDownDoc]
  public class ArmonikSymphonyClient
  {
    private readonly  IConfigurationSection          controlPlanAddress_;
    internal readonly ILogger<ArmonikSymphonyClient> Logger;
    private ILoggerFactory LoggerFactory { get; set; }

    /// <summary>
    ///   The ctor with IConfiguration and optional TaskOptions
    /// </summary>
    /// <param name="configuration">IConfiguration to set Client Data information and Grpc EndPoint</param>
    /// <param name="loggerFactory">Factory to create logger in the client service</param>
    /// <param name="taskOptions">TaskOptions for any Session</param>
    public ArmonikSymphonyClient(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
      controlPlanAddress_ = configuration.GetSection(SectionControlPlan);
      LoggerFactory       = loggerFactory;
      Logger              = loggerFactory.CreateLogger<ArmonikSymphonyClient>();
    }

    private ClientService.ClientServiceClient ControlPlaneService { get; set; }

    /// <summary>
    ///   Returns the section key Grpc from appSettings.json
    /// </summary>
    public string SectionControlPlan { get; set; } = "Grpc";

    /// <summary>
    ///   Create the session to submit task
    /// </summary>
    /// <param name="taskOptions">Optional parameter to set TaskOptions during the Session creation</param>
    /// <returns>Returns the SessionService to submit, wait or get result</returns>
    public SessionService CreateSession(TaskOptions taskOptions = null)
    {
      ControlPlaneConnection();

      return new SessionService(LoggerFactory, ControlPlaneService, taskOptions);
    }

    /// <summary>
    ///   Open the session already created to submit task
    /// </summary>
    /// <param name="sessionId">The sessionId string which will opened</param>
    /// <param name="clientOptions">the customer taskOptions.Options send to the server by the client</param>
    /// <returns>Returns the SessionService to submit, wait or get result</returns>
    public SessionService OpenSession(SessionId sessionId, IDictionary<string, string> clientOptions)
    {
      ControlPlaneConnection();

      return new SessionService(LoggerFactory,
                                ControlPlaneService,
                                sessionId, clientOptions);
    }

    private void ControlPlaneConnection()
    {
      var channel = GrpcChannel.ForAddress(controlPlanAddress_["Endpoint"]);
      ControlPlaneService ??= new(channel);
    }
  }
}