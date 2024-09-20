﻿using Microsoft.AspNetCore.Mvc;
using MyPlanner.Plannings.Api.Dtos.SizeModelType;

namespace MyPlanner.Plannings.Api.UseCases.SizeModelTypes.Commands.ChangeCodeSizeModelType
{
    public class ChangeCodeSizeModelTypeController : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/sizemodeltypes/{sizeModelTypeId}/changecode", async ([FromHeader(Name = "x-requestid")] Guid requestId,
                                                                            [AsParameters] SizeModelTypeService service,
                                                                            string sizeModelTypeId,
                                                                            [FromBody] ChangeCodeSizeModelTypeDto changeCodeSizeModelTypeDto) =>
            {
                var request = new ChangeCodeSizeModelTypeRequest(sizeModelTypeId, changeCodeSizeModelTypeDto.Code, changeCodeSizeModelTypeDto.UserId);

                var result = await service.Mediator.Send(request);

                {
                    Results.BadRequest();
                }

                return Results.Ok(result);



            }).WithTags(Tags.SizeModelTypes);

        }
    }
}
