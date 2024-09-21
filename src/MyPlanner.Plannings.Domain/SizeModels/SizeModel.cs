﻿using BeyondNet.Ddd;
using BeyondNet.Ddd.Interfaces;
using BeyondNet.Ddd.ValueObjects;
using MyPlanner.Plannings.Domain.SizeModelTypes;
using MyPlanner.Plannings.Shared.Domain.ValueObjects;

namespace MyPlanner.Plannings.Domain.SizeModels
{
    public class SizeModelProps : IProps
    {
        public IdValueObject Id { get; private set; }
        public SizeModelType SizeModelType { get; private set; } // TShirt
        public Name Name { get; private set; } // TShirt size for Squads
        public ICollection<SizeModelItem> Items { get; private set; }
        public Audit Audit { get; set; }
        public SizeModelStatus Status { get; private set; } = SizeModelStatus.Active;

        public SizeModelProps(IdValueObject id, SizeModelType sizeModelType, Name name, UserId userId)
        {
            Id = id;
            SizeModelType = sizeModelType;
            Name = name;
            Items = new List<SizeModelItem>();
            Audit = Audit.Create(userId.GetValue());
            Status = SizeModelStatus.Active;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class SizeModel : Entity<SizeModel, SizeModelProps>
    {
        private SizeModel(SizeModelProps props) : base(props)
        {
        }

        public static SizeModel Create(IdValueObject id, SizeModelType sizeModelType, Name name, UserId userId)
        {
            return new SizeModel(new SizeModelProps(id, sizeModelType, name, userId));
        }

        public void AddItem(SizeModelItem sizeModelItem, UserId userId)
        {
            if (GetPropsCopy().Items.Any(x => x.GetPropsCopy().Id == sizeModelItem.GetPropsCopy().Id))
            {
                AddBrokenRule("SizeModel", "SizeModelItem already exists");
                return;
            }

            GetProps().Items.Add(sizeModelItem);
            GetProps().Audit.Update(userId.GetValue());
        }

        public void RemoveItem(SizeModelItem sizeModelItem, UserId userId)
        {
            if (!GetPropsCopy().Items.Any(x => x.GetPropsCopy().Id == sizeModelItem.GetPropsCopy().Id))
            {
                AddBrokenRule("SizeModel", "SizeModelItem does not exist");
                return;
            }

            GetProps().Items.Remove(sizeModelItem);
            GetProps().Audit.Update(userId.GetValue());
        }

        public void ChangeName(Name name, UserId userId)
        {
            GetProps().Name.SetValue(name.GetValue());
            GetProps().Audit.Update(userId.GetValue());
        }

        public void Activate(UserId userId)
        {
            if (GetProps().Status == SizeModelStatus.Active)
            {
                AddBrokenRule("SizeModel", "SizeModel is already active");
                return;
            }

            GetProps().Status.SetValue<SizeModelStatus>(SizeModelStatus.Active.Id);
            GetProps().Audit.Update(userId.GetValue());
        }

        public void Deactivate(UserId userId)
        {
            var count = GetProps().Items.Where(x => x.GetPropsCopy().Status == SizeModelItemStatus.Active).Count();

            if (count > 0)
            {
                AddBrokenRule("SizeModel", "SizeModel has active SizeModelItems");
                return;
            }

            GetProps().Status.SetValue<SizeModelStatus>(SizeModelStatus.Inactive.Id);
            GetProps().Audit.Update(userId.GetValue());
        }

        public void Delete(UserId userId)
        {
            if (GetProps().Status == SizeModelStatus.Delete)
            {
                AddBrokenRule("SizeModel", "SizeModel is already deleted");
                return;
            }

            foreach (var item in GetPropsCopy().Items)
            {
                if (item.GetPropsCopy().Status == SizeModelItemStatus.Active)
                {
                    AddBrokenRule("SizeModel", "SizeModel has active SizeModelItems");
                    return;
                }
            }

            GetProps().Status.SetValue<SizeModelStatus>(SizeModelStatus.Delete.Id);
            GetProps().Audit.Update(userId.GetValue());
        }
    }

    public class SizeModelStatus : Enumeration
    {
        public static SizeModelStatus Delete = new SizeModelStatus(-1, "Delete");
        public static SizeModelStatus Active = new SizeModelStatus(1, "Active");
        public static SizeModelStatus Inactive = new SizeModelStatus(0, "Inactive");

        public SizeModelStatus(int id, string name) : base(id, name)
        {
        }
    }
}
