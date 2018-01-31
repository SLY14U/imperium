﻿namespace Oxide.Plugins
{
  using Network;

  public partial class Imperium : RustPlugin
  {
    void OnUserApprove(Connection connection)
    {
      Users.SetOriginalName(connection.userid.ToString(), connection.username);
    }

    void OnPlayerInit(BasePlayer player)
    {
      if (player == null) return;

      // If the player hasn't fully connected yet, try again in 2 seconds.
      if (player.IsReceivingSnapshot)
      {
        timer.In(2, () => OnPlayerInit(player));
        return;
      }

      Users.Add(player);
    }

    void OnPlayerDisconnected(BasePlayer player)
    {
      if (player != null)
        Users.Remove(player);
    }

    void OnHammerHit(BasePlayer player, HitInfo hit)
    {
      User user = Users.Get(player);

      if (user != null && user.CurrentInteraction != null)
        user.CompleteInteraction(hit);
    }

    object OnEntityTakeDamage(BaseCombatEntity entity, HitInfo hit)
    {
      if (!Options.EnableDefensiveBonuses)
        return null;

      if (entity == null || hit == null)
        return null;

      if (hit.damageTypes.Has(Rust.DamageType.Decay))
        return ScaleDamageForDecay(entity, hit);

      User user = Users.Get(hit.InitiatorPlayer);
      if (user == null)
        return null;

      return ScaleDamageForDefensiveBonus(entity, hit, user);
    }

    void OnEntityKill(BaseNetworkable entity)
    {
      // If a player dies in an area, remove them from the area.
      var player = entity as BasePlayer;
      if (player != null)
      {
        User user = Users.Get(player);
        if (user != null && user.CurrentArea != null)
          user.CurrentArea = null;
      }

      // If a claim TC is destroyed, remove the claim from the area.
      var cupboard = entity as BuildingPrivlidge;
      if (cupboard != null)
      {
        var area = Areas.GetByClaimCupboard(cupboard);
        if (area != null)
        {
          PrintToChat(Messages.AreaClaimLostCupboardDestroyedAnnouncement, area.FactionId, area.Id);
          Log($"{area.FactionId} lost their claim on {area.Id} because the tool cupboard was destroyed (hook function)");
          Areas.Unclaim(area);
        }
      }

      // If a tax chest is destroyed, remove it from the faction data.
      if (Options.EnableTaxation)
      {
        var container = entity as StorageContainer;
        if (container != null)
        {
          Faction faction = Factions.GetByTaxChest(container);
          if (faction != null)
          {
            Log($"{faction.Id}'s tax chest was destroyed (hook function)");
            faction.TaxChest = null;
          }
        }
      }
    }

    void OnDispenserGather(ResourceDispenser dispenser, BaseEntity entity, Item item)
    {
      ProcessTaxesIfApplicable(dispenser, entity, item);
      AwardBadlandsBonusIfApplicable(dispenser, entity, item);
    }

    void OnDispenserBonus(ResourceDispenser dispenser, BaseEntity entity, Item item)
    {
      ProcessTaxesIfApplicable(dispenser, entity, item);
      AwardBadlandsBonusIfApplicable(dispenser, entity, item);
    }

    void OnUserEnteredArea(Area area, User user)
    {
      Area previousArea = user.CurrentArea;

      user.CurrentArea = area;
      user.HudPanel.Refresh();

      if (previousArea == null)
        return;

      if (area.Type == AreaType.Badlands && previousArea.Type != AreaType.Badlands)
      {
        // The player has entered the badlands.
        user.SendChatMessage(Messages.EnteredBadlands);
      }
      else if (area.Type == AreaType.Wilderness && previousArea.Type != AreaType.Wilderness)
      {
        // The player has entered the wilderness.
        user.SendChatMessage(Messages.EnteredWilderness);
      }
      else if (area.Type == AreaType.Town && previousArea.Type != AreaType.Town)
      {
        // The player has entered a town.
        user.SendChatMessage(Messages.EnteredTown, area.Name, area.FactionId);
      }
      else if (area.IsClaimed && !previousArea.IsClaimed)
      {
        // The player has entered a faction's territory.
        user.SendChatMessage(Messages.EnteredClaimedArea, area.FactionId);
      }
      else if (area.IsClaimed && previousArea.IsClaimed && area.FactionId != previousArea.FactionId)
      {
        // The player has crossed a border between the territory of two factions.
        user.SendChatMessage(Messages.EnteredClaimedArea, area.FactionId);
      }
    }

    void OnFactionCreated(Faction faction)
    {
      Ui.RefreshForAllPlayers();
    }

    void OnFactionDisbanded(Faction faction)
    {
      Area[] areas = Instance.Areas.GetAllClaimedByFaction(faction);

      if (areas.Length > 0)
      {
        foreach (Area area in areas)
          PrintToChat(Messages.AreaClaimLostFactionDisbandedAnnouncement, area.FactionId, area.Id);

        Areas.Unclaim(areas);
      }

      Wars.EndAllWarsForEliminatedFactions();
      Ui.RefreshForAllPlayers();
    }

    void OnFactionTaxesChanged(Faction faction)
    {
      Ui.RefreshForAllPlayers();
    }

    void OnAreaChanged(Area area)
    {
      Wars.EndAllWarsForEliminatedFactions();
      Images.GenerateMapOverlayImage();
      Ui.RefreshForAllPlayers();
    }

    void OnDiplomacyChanged()
    {
      Ui.RefreshForAllPlayers();
    }
  }
}
