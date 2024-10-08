﻿
using MyPlanner.Plannings.Api.UseCases.SizeModels.Command.CreateSizeModel;
using MyPlanner.Plannings.Domain.PlanAggregate;
using MyPlanner.Plannings.Domain.SizeModels;
using MyPlanner.Plannings.Domain.SizeModelTypes;
using MyPlanner.Plannings.Infrastructure.Database.Tables;
using MyPlanner.Shared.Domain.ValueObjects;
using MyPlanner.Shared.Infrastructure.Database;

namespace MyPlanner.Plannings.Api.Mappers
{
    public class SizeModelProfileMap : Profile
    {
        public SizeModelProfileMap()
        {
            CreateMap<Audit, AuditTable>().ConvertUsing(new AuditToAuditTableConvert());

            // SizeModel
            CreateMap<CreateSizeModelDto, CreateSizeModelCommand>();
            CreateMap<CreateSizeModelItemDto, CreateSizeModelItemRequest>();

            CreateMap<SizeModelProps, SizeModelTable>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.GetValue()))
                .ForMember(dest => dest.SizeModelTypeId, opt => opt.MapFrom(src => src.SizeModelTypeId.GetValue()))
                .ForMember(dest => dest.SizeModelTypeCode, opt => opt.MapFrom(src => src.SizeModelTypeCode.GetValue()))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.GetValue()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Id))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(new SizeModelItemPropsToTableResolver()));

            CreateMap<SizeModelTable, SizeModelDto>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.SizeModelTypeId, opt => opt.MapFrom(src => src.SizeModelTypeId))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)).AfterMap<SizeModelEnumAction>();

            CreateMap<SizeModelItemTable, SizeModelItemDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SizeModelId, opt => opt.MapFrom(src => src.SizeModelId))
                .ForMember(dest => dest.SizeModelName, opt => opt.MapFrom(src => src.SizeModel.Name))
                .ForMember(dest => dest.SizeModelTypeItemId, opt => opt.MapFrom(src => src.SizeModelTypeItemId))
                .ForMember(dest => dest.SizeModelTypeItemCode, opt => opt.MapFrom(src => src.SizeModelTypeItem.Code))
                .ForMember(dest => dest.FactorSelected, opt => opt.MapFrom(src => src.FactorSelected))
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.ProfileName))
                .ForMember(dest => dest.ProfileAvgRateSymbol, opt => opt.MapFrom(src => src.ProfileAvgRateSymbol))
                .ForMember(dest => dest.ProfileAvgRateValue, opt => opt.MapFrom(src => src.ProfileAvgRateValue))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.TotalCost, opt => opt.MapFrom(src => src.TotalCost))
                .ForMember(dest => dest.IsStandard, opt => opt.MapFrom(src => src.IsStandard))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)).AfterMap<SizeModelItemEnumAction>();


            CreateMap<SizeModelTable, SizeModel>()
            .ConstructUsing(src => SizeModel.Load(new SizeModelProps(
                IdValueObject.Create(src.Id),
                IdValueObject.Create(src.SizeModelTypeId),
                SizeModelTypeCode.Create(src.SizeModelTypeCode),
                Name.Create(src.Name),
                Audit.Load(new AuditProps()
                {
                    CreatedBy = src.Audit.CreatedBy,
                    CreatedAt = src.Audit.CreatedAt,
                    UpdatedBy = src.Audit.UpdatedBy,
                    UpdatedAt = src.Audit.UpdatedAt,
                    TimeSpan = src.Audit.TimeSpan
                }),
                Enumeration.FromValue<SizeModelStatus>(src.Status))));


            CreateMap<SizeModelItemTable, SizeModelItem>()
                .ConstructUsing(src => SizeModelItem.Load(
                    new SizeModelItemProps(
                        IdValueObject.Create(src.Id),
                        IdValueObject.Create(src.SizeModelId),
                        IdValueObject.Create(src.SizeModelTypeItemId),
                        SizeModelTypeItemCode.Create(src.SizeModelTypeItemCode),
                        Enumeration.FromValue<FactorsEnum>(src.FactorSelected),
                        SizeModelProfile.Create(
                            Domain.PlanAggregate.ProfileName.Create(src.ProfileName),
                            ProfileAvgRate.Create(
                                Enumeration.FromValue<CurrencySymbolEnum>(src.ProfileAvgRateSymbol),
                                src.ProfileAvgRateValue)),
                        SizeModelTypeQuantity.Create(src.Quantity),
                        SizeModelTotalCost.Create(src.TotalCost),
                        SizeModelItemIsStandard.Create(src.IsStandard),
                        Audit.Load(new AuditProps()
                        {
                            CreatedBy = src.Audit.CreatedBy,
                            CreatedAt = src.Audit.CreatedAt,
                            UpdatedBy = src.Audit.UpdatedBy,
                            UpdatedAt = src.Audit.UpdatedAt,
                            TimeSpan = src.Audit.TimeSpan
                        }),
                        Enumeration.FromValue<SizeModelItemStatus>(src.Status))));

        }
    }

    #region Actions

    internal class SizeModelItemEnumAction : IMappingAction<SizeModelItemTable, SizeModelItemDto>
    {
        public void Process(SizeModelItemTable source, SizeModelItemDto destination, ResolutionContext context)
        {
            destination.Status = Enumeration.FromValue<SizeModelItemStatus>(source.Status).Name.ToUpper();
        }
    }

    internal class SizeModelEnumAction : IMappingAction<SizeModelTable, SizeModelDto>
    {
        public void Process(SizeModelTable source, SizeModelDto destination, ResolutionContext context)
        {
            destination.Status = Enumeration.FromValue<SizeModelStatus>(source.Status).Name.ToUpper();
        }
    }

    #endregion

    #region Resolvers

    internal class SizeModelItemPropsToTableResolver : IValueResolver<SizeModelProps, SizeModelTable, ICollection<SizeModelItemTable>>
    {
        public ICollection<SizeModelItemTable> Resolve(SizeModelProps source, SizeModelTable destination, ICollection<SizeModelItemTable> destMember, ResolutionContext context)
        {
            var items = new List<SizeModelItemTable>();

            if (destination.Items == null)
            {
                destination.Items = new List<SizeModelItemTable>();
            }

            source.Items.ToList().ForEach(item =>
            {
                var sizeModelItemTable = new SizeModelItemTable()
                {
                    Id = item.GetPropsCopy().Id.GetValue(),
                    SizeModelId = item.GetPropsCopy().Id.GetValue(),
                    SizeModel = destination,
                    SizeModelTypeItemId = item.GetPropsCopy().SizeModelTypeItemId.GetValue(),
                    SizeModelTypeItemCode = item.GetPropsCopy().SizeModelTypeItemCode.GetValue(),
                    FactorSelected = item.GetPropsCopy().FactorSelected.Id,
                    ProfileName = item.GetPropsCopy().Profile.GetValue().ProfileName.GetValue(),
                    ProfileAvgRateSymbol = item.GetPropsCopy().Profile.GetValue().ProfileAvgRate.GetValue().Symbol.Id,
                    ProfileAvgRateValue = item.GetPropsCopy().Profile.GetValue().ProfileAvgRate.GetValue().Value,
                    Quantity = item.GetPropsCopy().Quantity.GetValue(),
                    TotalCost = item.GetPropsCopy().TotalCost.GetValue(),
                    IsStandard = item.GetPropsCopy().IsStandard.GetValue(),
                    Status = item.GetPropsCopy().Status.Id,
                    Audit = new AuditTable()
                    {
                        CreatedBy = item.GetPropsCopy().Audit.GetValue().CreatedBy,
                        CreatedAt = item.GetPropsCopy().Audit.GetValue().CreatedAt,
                        UpdatedBy = item.GetPropsCopy().Audit.GetValue().UpdatedBy!,
                        UpdatedAt = item.GetPropsCopy().Audit.GetValue().UpdatedAt,
                        TimeSpan = item.GetPropsCopy().Audit.GetValue().TimeSpan
                    }
                };

                destination.Items.Add(sizeModelItemTable);
            });

            return destination.Items;
        }
    }

    #endregion

    #region Converters


    internal class AuditToAuditTableConvert : ITypeConverter<Audit, AuditTable>
    {
        public AuditTable Convert(Audit source, AuditTable destination, ResolutionContext context)
        {
            var dto = new AuditTable
            {
                CreatedBy = source.GetValue().CreatedBy,
                CreatedAt = source.GetValue().CreatedAt,
                UpdatedBy = source.GetValue().UpdatedBy!,
                UpdatedAt = source.GetValue().UpdatedAt,
                TimeSpan = source.GetValue().TimeSpan
            };

            return dto;
        }
    }

    #endregion
}
