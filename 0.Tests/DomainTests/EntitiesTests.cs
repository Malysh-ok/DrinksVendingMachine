using App.Infrastructure.Authorization;
using Domain.Entities.Enums;
using Infrastructure.AppComponents.AppExceptions;
using Infrastructure.BaseExtensions.ValueTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.DomainTests;

[TestClass]
public class EntitiesTests
{
    [TestMethod]
    public void TmpTest()
    {
        var dt = DateTime.Now.AddMinutes(10);
        // dt = new DateTime(2023, 7, 12, 22, 0, 0);
        
        var jwt = LoginManager.CreateJwt(dt);
        var strJwt = LoginManager.SerializeJwt(jwt);
  
        // var accessToken = @"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQWRtaW4iLCJleHAiOjE2ODkxODg0MDAsImlzcyI6IkRyaW5rc1ZlbmRpbmdNYWNoaW5lU2VydmVyIiwiYXVkIjoiRHJpbmtzVmVuZGluZ01hY2hpbmVDbGllbnQifQ.CssYROuvQUuZTsclijMKDHL7viiJySQk6YA0qlNFJpo";
        var isValitToken = LoginManager.IsValidJwtStr(strJwt);
        var jwtStr = LoginManager.SerializeJwt(null);
    }
}