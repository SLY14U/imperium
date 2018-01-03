﻿namespace Oxide.Plugins
{
  public partial class RustFactions
  {
    class SelectingTaxChestInteraction : Interaction
    {
      public Faction Faction { get; private set; }

      public SelectingTaxChestInteraction(Faction faction)
      {
        Faction = faction;
      }

      public override bool TryComplete(HitInfo hit)
      {
        var container = hit.HitEntity as StorageContainer;

        if (container == null)
        {
          User.SendMessage(Messages.SelectingTaxChestFailedInvalidTarget);
          return false;
        }

        Core.Factions.SetTaxChest(Faction, container);
        User.SendMessage(Messages.SelectingTaxChestSucceeded, Faction.TaxRate, Faction.Id);

        return true;
      }
    }
  }
}