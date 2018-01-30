﻿namespace Oxide.Plugins
{
  using Oxide.Core;

  public partial class Imperium : RustPlugin
  {
    static class Api
    {
      public static void HandleFactionCreated(Faction faction)
      {
        Interface.Call("OnFactionCreated", faction);
      }

      public static void HandleFactionDisbanded(Faction faction)
      {
        Interface.Call("OnFactionDisbanded", faction);
      }

      public static void HandlePlayerJoinedFaction(Faction faction, User user)
      {
        Interface.Call("OnPlayerJoinedFaction", faction, user);
      }

      public static void HandlePlayerLeftFaction(Faction faction, User user)
      {
        Interface.Call("OnPlayerLeftFaction", faction, user);
      }

      public static void HandlePlayerInvitedToFaction(Faction faction, User user)
      {
        Interface.Call("OnPlayerInvitedToFaction", faction, user);
      }

      public static void HandlePlayerUninvitedFromFaction(Faction faction, User user)
      {
        Interface.Call("OnPlayerUninvitedFromFaction", faction, user);
      }

      public static void HandlePlayerPromoted(Faction faction, User user)
      {
        Interface.Call("OnPlayerPromoted", faction, user);
      }

      public static void HandlePlayerDemoted(Faction faction, User user)
      {
        Interface.Call("OnPlayerDemoted", faction, user);
      }
    }
  }
}