﻿

namespace MyPlanner.Plannings.Api.UseCases.SizeModelTypes.Commands.CreateSizeModelType
{
    public class CreateSizeModelTypeEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/sizemodeltypes/", async ([FromHeader(Name = "x-requestid")] Guid requestId,
                                                   [AsParameters] SizeModelTypeService service,
                                                   [FromBody] CreateSizeModelTypeDto createSizeModelTypeDto)=>
            {
                var request = service.Mapper.Map<CreateSizeModelTypeCommand>(createSizeModelTypeDto);

                var result = await service.Mediator.Send(request);

                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);

            }).WithTags(Tags.SizeModelTypes);
        }
    }
}
