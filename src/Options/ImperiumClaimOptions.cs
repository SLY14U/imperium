﻿namespace Oxide.Plugins
{
  using Newtonsoft.Json;
  using System.Collections.Generic;

  public partial class Imperium : RustPlugin
  {
    class ImperiumClaimOptions
    {
      [JsonProperty("enabled")]
      public bool Enabled;

      [JsonProperty("costs")]
      public List<int> Costs = new List<int>();

      [JsonProperty("minAreaNameLength")]
      public int MinAreaNameLength;

      [JsonProperty("minFactionMembers")]
      public int MinFactionMembers;

      public static ImperiumClaimOptions Default = new ImperiumClaimOptions {
        Enabled = true,
        Costs = new List<int> { 0, 100, 200, 300, 400, 500 },
        MinAreaNameLength = 3,
        MinFactionMembers = 3
      };
    }
  }
}
