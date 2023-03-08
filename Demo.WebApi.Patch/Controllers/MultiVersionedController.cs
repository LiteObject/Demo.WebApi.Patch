using Microsoft.AspNetCore.Mvc;

namespace Demo.WebApi.Patch.API.Controllers;

/// <summary>
/// 
/// </summary>
/* [ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")] */
public class MultiVersionedController : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="apiVersion"></param>
    /// <returns></returns>
    [HttpGet]
    public string Get(ApiVersion apiVersion) => $"Controller = {GetType().Name}\nVersion = {apiVersion}";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="apiVersion"></param>
    /// <returns></returns>
    [HttpGet, MapToApiVersion("2.0")]
    public string GetV2(ApiVersion apiVersion) => $"Controller = {GetType().Name}\nVersion = {apiVersion}";
}
