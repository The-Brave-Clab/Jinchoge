using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer.Entity.Events;

public class SpecialStageTransactionRetireEntity : BaseEntity<SpecialStageTransactionRetireEntity>
{
    public SpecialStageTransactionRetireEntity(
        Uri requestUri,
        string httpMethod,
        Dictionary<string, string> requestHeaders,
        byte[] requestBody,
        RouteConfig config)
        : base(requestUri, httpMethod, requestHeaders, requestBody, config)
    {
    }
    
    protected override Task ProcessRequest()
    {
        var player = GetPlayerFromCookies();
            
        Utils.Log("Path parameters:");
        foreach (var pathParameter in pathParameters)
        {
            Utils.Log($"\t{pathParameter.Key} = {pathParameter.Value}");
        }

        responseBody = Encoding.UTF8.GetBytes("{}");
        SetBasicResponseHeaders();

        return Task.CompletedTask;
    }
}