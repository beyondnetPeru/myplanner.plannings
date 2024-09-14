﻿
using MyPlanner.Plannings.Api.Dtos.Plan;

namespace MyPlanner.Plannings.Api.UseCases.Plan.Command.ActivatePlan
{
    public class ActivatePlanController : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/plans/{planId}/activate", async ([AsParameters] PlanServices service, ActivatePlanDto planDto) =>
            {
                var request = new ActivatePlanRequest(planDto.PlanId, planDto.UserId);

                var result = await service.Mediator.Send(request);

                return result ? Results.Ok() : Results.BadRequest();

            });

        }
    }
}
