﻿namespace Oxide.Plugins
{
  public partial class Imperium
  {
    void OnTownDisbandCommand(User user)
    {
      Faction faction = Factions.GetByMember(user);

      if (!EnsureUserCanManageTowns(user, faction))
        return;

      Town town = Areas.GetTownByMayor(user);
      if (town == null)
      {
        user.SendChatMessage(Messages.NotMayorOfTown);
        return;
      }

      PrintToChat(Messages.TownDisbandedAnnouncement, faction.Id, town.Name);
      Log($"{Util.Format(user)} disbanded the town faction {town.Name}");

      foreach (Area area in town.Areas)
        Areas.RemoveFromTown(area);
    }
  }
}
