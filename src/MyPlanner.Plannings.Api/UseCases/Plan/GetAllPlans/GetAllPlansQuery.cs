﻿using MediatR;
using MyPlanner.Plannings.Api.Dtos.Plan;
using MyPlanner.Plannings.Shared.Application.Dtos;

namespace MyPlanner.Plannings.Api.UseCases.Plan.GetAllPlans
{
    public class GetAllPlansQuery : IRequest<IEnumerable<PlanDto>>
    {
        public GetAllPlansQuery(PaginationDto pagination)
        {
            Pagination = pagination;
        }

        public PaginationDto Pagination { get; }
    }
}
