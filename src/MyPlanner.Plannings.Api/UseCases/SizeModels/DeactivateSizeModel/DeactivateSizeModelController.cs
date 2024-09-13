﻿using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyPlanner.Plannings.Api.Dtos.SizeModel;

namespace MyPlanner.Plannings.Api.UseCases.SizeModels.DeactivateSizeModel
{
    public class DeactivateSizeModelController(IMediator mediator) : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/sizemodels/{sizeModelId}/deactivate", async ([FromHeader(Name = "x-requestid")] Guid requestId, DeactivateSizeModelDto activateSizeModelDto) =>
            {
                var request = new DeactivateSizeModelRequest(activateSizeModelDto.SizeModelId, activateSizeModelDto.UserId);

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
