﻿using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyPlanner.Plannings.Api.Dtos.SizeModel;

namespace MyPlanner.Plannings.Api.UseCases.SizeModels.ChangeSizeModelName
{
    public class ChangeSizeModelNameController(IMediator mediator) : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/sizemodels/{sizeModelId}/name", async ([FromHeader(Name = "x-requestid")] Guid requestId, [FromBody] ChangeSizeModelNameDto changeSizeModelNameDto) =>
            {
                var request = new ChangeSizeModelNameRequest(changeSizeModelNameDto.SizeModelId, changeSizeModelNameDto.Name, changeSizeModelNameDto.UserId);

                var result = await mediator.Send(request);

                if (!result)
                {
                    Results.BadRequest();
                }

                return Results.Ok(result);

            }).WithTags(Tags.SizeModels);
        }
    }
}
