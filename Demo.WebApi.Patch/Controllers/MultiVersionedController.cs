using Microsoft.AspNetCore.Mvc;

namespace Demo.WebApi.Patch.API.Controllers;

/* [ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")] */
public class MultiVersionedController : ControllerBase
{
    [HttpGet]
    public string Get(ApiVersion apiVersion) => $"Controller = {GetType().Name}\nVersion = {apiVersion}";

    [HttpGet, MapToApiVersion("2.0")]
    public string GetV2(ApiVersion apiVersion) => $"Controller = {GetType().Name}\nVersion = {apiVersion}";
}
