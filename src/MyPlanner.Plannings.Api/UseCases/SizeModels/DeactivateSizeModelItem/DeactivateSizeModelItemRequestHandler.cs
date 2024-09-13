﻿using MediatR;
using MyPlanner.Plannings.Domain.SizeModels;
using MyPlanner.Plannings.Shared.Domain.ValueObjects;

namespace MyPlanner.Plannings.Api.UseCases.SizeModels.DeactivateSizeModelItem
{
    public class DeactivateSizeModelItemRequestHandler : IRequestHandler<DeactivateSizeModelItemRequest, bool>
    {
        private readonly ISizeModelRepository sizeModelRepository;
        private readonly ILogger<DeactivateSizeModelItemRequestHandler> logger;

        public DeactivateSizeModelItemRequestHandler(ISizeModelRepository sizeModelRepository, ILogger<DeactivateSizeModelItemRequestHandler> logger)
        {
            this.sizeModelRepository = sizeModelRepository;
            this.logger = logger;
        }

        public async Task<bool> Handle(DeactivateSizeModelItemRequest request, CancellationToken cancellationToken)
        {
            var sizeModelItem = await sizeModelRepository.GetItem(request.SizeModelId, request.SizeModelItemId);

            sizeModelItem.Deactivate(UserId.Create(request.UserId));

            if (!sizeModelItem.IsValid())
            {
                logger.LogError($"SizeModelItem is not valid. Errors:{sizeModelItem.GetBrokenRules()}");
                return false;
            }

            await sizeModelRepository.DeactiveItem(request.SizeModelItemId);

            await sizeModelRepository.UnitOfWork.SaveEntitiesAsync(this, cancellationToken);

            return true;
        }
    }
}
