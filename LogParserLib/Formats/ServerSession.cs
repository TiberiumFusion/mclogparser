using com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats
{
    // A server session, including events
    public class ServerSession
    {
        [JsonProperty(Order = 0)] public long E_ID { get; set; } // E_ for "filled in by the Exporter". Index that this object occupies in the Json export catalog.
        [JsonIgnore] public ExporterOptions E_Options; // Awareness of export options for the ShouldSerialize conditionals


        //================| Serializable fields |================//

        [JsonProperty(Order = 101)] public TimeRange Range = new TimeRange();
            public bool ShouldSerializeRange() { return E_Options.ServerSession_IncludeRange; }


        [JsonProperty(Order = 401)] public List<GameEvent> AllGameEvents = new List<GameEvent>(); // All GameEvents that happened during this session
            public bool ShouldSerializeAllGameEvents() { return E_Options.ServerSession_IncludeAllGameEvents && !E_Options.ServerSession_UseCatalogIDsForAllGameEvents; }
        [JsonProperty(Order = 402)] public List<long> AllGameEventIDs = new List<long>(); // Export-friendly version of the above field
            public bool ShouldSerializeAllGameEventIDs() { return E_Options.ServerSession_IncludeAllGameEvents && E_Options.ServerSession_UseCatalogIDsForAllGameEvents; }

        [JsonProperty(Order = 411)] public List<GameEvent> BootGameEvents = new List<GameEvent>(); // GameEvents that only occurred during the boot phase. Be aware that players can connect and do things before a server finishes booting!
            public bool ShouldSerializeBootGameEvents() { return E_Options.ServerSession_IncludeBootGameEvents && !E_Options.ServerSession_UseCatalogIDsForBootGameEvents; }
        [JsonProperty(Order = 412)] public List<long> BootGameEventIDs = new List<long>(); // Export-friendly version of the above field
            public bool ShouldSerializeBootGameEventIDs() { return E_Options.ServerSession_IncludeBootGameEvents && E_Options.ServerSession_UseCatalogIDsForBootGameEvents; }

        [JsonProperty(Order = 421)] public List<GameEvent> RunningGameEvents = new List<GameEvent>(); // GameEvents that only occurred between the boot and shutdown phases
            public bool ShouldSerializeRunningGameEvents() { return E_Options.ServerSession_IncludeRunningGameEvents && !E_Options.ServerSession_UseCatalogIDsForRunningGameEvents; }
        [JsonProperty(Order = 422)] public List<long> RunningGameEventIDs = new List<long>(); // Export-friendly version of the above field
            public bool ShouldSerializeRunningGameEventIDs() { return E_Options.ServerSession_IncludeRunningGameEvents && E_Options.ServerSession_UseCatalogIDsForRunningGameEvents; }

        [JsonProperty(Order = 431)] public List<GameEvent> ShutdownGameEvents = new List<GameEvent>(); // GameEvents that only occurred during the shutdown phase. Be aware that players can do things before a server ultimately stops!
            public bool ShouldSerializeShutdownGameEvents() { return E_Options.ServerSession_IncludeShutdownGameEvents && !E_Options.ServerSession_UseCatalogIDsForShutdownGameEvents; }
        [JsonProperty(Order = 432)] public List<long> ShutdownGameEventIDs = new List<long>(); // Export-friendly version of the above field
            public bool ShouldSerializeShutdownGameEventIDs() { return E_Options.ServerSession_IncludeShutdownGameEvents && E_Options.ServerSession_UseCatalogIDsForShutdownGameEvents; }

        [JsonProperty(Order = 701)] public List<DecoratedLog> SourceLogFiles = new List<DecoratedLog>(); // The physical log files which encompass the events of this ServerSession
            public bool ShouldSerializeSourceLogFiles() { return E_Options.ServerSession_IncludeSourceLogFiles && !E_Options.ServerSession_UseCatalogIDsForSourceLogFiles; }
        [JsonProperty(Order = 702)] public List<long> SourceLogFileIDs = new List<long>(); // Export-friendly version of the above field
            public bool ShouldSerializeSourceLogFileIDs() { return E_Options.ServerSession_IncludeSourceLogFiles && E_Options.ServerSession_UseCatalogIDsForSourceLogFiles; }

        [JsonProperty(Order = 105)] public bool EndedNormally = false; // False if ended by a crash, early termination (such Ctrl+C), not accepting EULA, etc.
            public bool ShouldSerializeEndedNormally() { return E_Options.ServerSession_IncludeEndedNormally; }

        [JsonProperty(Order = 106)] public string ShutdownReason = ""; // Shutdown reason (not something provided by logs), inferred from existence or lack of ServerStoppingEvents
            public bool ShouldSerializeShutdownReason() { return E_Options.ServerSession_IncludeShutdownReason; }


        // Specific GameEvent collections for simpler lookups
        [JsonProperty(Order = 501)] public List<GameEvent> PlayerChatEvents = new List<GameEvent>(); // All chat events, including /tell commands, for all players during this session
            public bool ShouldSerializePlayerChatEvents() { return E_Options.ServerSession_IncludePlayerChatEvents && !E_Options.ServerSession_UseCatalogIDsForPlayerChatEvents; }
        [JsonProperty(Order = 502)] public List<long> PlayerChatEventIDs = new List<long>(); // Export-friendly version of the above field
            public bool ShouldSerializePlayerChatEventIDs() { return E_Options.ServerSession_IncludePlayerChatEvents && E_Options.ServerSession_UseCatalogIDsForPlayerChatEvents; }

        [JsonProperty(Order = 511)] public List<GameEvent> PlayerDeathEvents = new List<GameEvent>(); // All player deaths during this session
            public bool ShouldSerializePlayerDeathEventss() { return E_Options.ServerSession_IncludePlayerDeathEvents && !E_Options.ServerSession_UseCatalogIDsForPlayerDeathEvents; }
        [JsonProperty(Order = 512)] public List<long> PlayerDeathEventIDs = new List<long>(); // Export-friendly version of the above field
            public bool ShouldSerializePlayerDeathEventIDs() { return E_Options.ServerSession_IncludePlayerDeathEvents && E_Options.ServerSession_UseCatalogIDsForPlayerDeathEvents; }

        [JsonProperty(Order = 521)] public List<GameEvent> PlayerCommandEvents = new List<GameEvent>(); // All of player commands run this session
            public bool ShouldSerializePlayerCommandEvents() { return E_Options.ServerSession_IncludePlayerCommandEvents && !E_Options.ServerSession_UseCatalogIDsForPlayerCommandEvents; }
        [JsonProperty(Order = 522)] public List<long> PlayerCommandEventIDs = new List<long>(); // Export-friendly version of the above field
            public bool ShouldSerializePlayerCommandEventIDs() { return E_Options.ServerSession_IncludePlayerCommandEvents && E_Options.ServerSession_UseCatalogIDsForPlayerCommandEvents; }


        [JsonProperty(Order = 201)] public string ServerMinecraftVersion;
            public bool ShouldSerializeServerMinecraftVersion() { return E_Options.ServerSession_IncludeServerMinecraftVersion; }

        [JsonProperty(Order = 202)] public string ServerDefaultGametype;
            public bool ShouldSerializeServerDefaultGametype() { return E_Options.ServerSession_IncludeServerDefaultGametype; }

        [JsonProperty(Order = 203)] public string ServerAddress;
            public bool ShouldSerializeServerAddress() { return E_Options.ServerSession_IncludeServerAddress; }

        [JsonProperty(Order = 204)] public string ServerIP;
            public bool ShouldSerializeServerIP() { return E_Options.ServerSession_IncludeServerIP; }

        [JsonProperty(Order = 205)] public string ServerPort;
            public bool ShouldSerializeServerPort() { return E_Options.ServerSession_IncludeServerPort; }

        [JsonProperty(Order = 206)] public string ServerFlavor;
            public bool ShouldSerializeServerFlavor() { return E_Options.ServerSession_IncludeServerFlavor; }

        [JsonProperty(Order = 207)] public string ServerFlavorVersion;
            public bool ShouldSerializeServerFlavorVersion() { return E_Options.ServerSession_IncludeServerFlavorVersion; }

        [JsonProperty(Order = 208)] public string ServerFlavorAPIVersion;
            public bool ShouldSerializeServerFlavorAPIVersion() { return E_Options.ServerSession_IncludeServerFlavorAPIVersion; }

        [JsonProperty(Order = 104)] public string ServerBootTime;
            public bool ShouldSerializeServerBootTime() { return E_Options.ServerSession_IncludeServerBootTime; }

        [JsonProperty(Order = 103)] public DateTime TimeOfBootDone;
            public bool ShouldSerializeTimeOfBootDone() { return E_Options.ServerSession_IncludeTimeOfBootDone; }

        [JsonProperty(Order = 301)] public List<ServerLevel> ServerLoadedLevels = new List<ServerLevel>();
            public bool ShouldSerializeServerLoadedLevels() { return E_Options.ServerSession_IncludeServerLoadedLevels; }

        [JsonProperty(Order = 601)] public Dictionary<string, List<PlayerSession>> ConcurrentPlayerSessions = new Dictionary<string, List<PlayerSession>>(); // UUID -> all of that player's session during this serversession
            public bool ShouldSerializeConcurrentPlayerSessions() { return E_Options.ServerSession_IncludeConcurrentPlayerSessions && !E_Options.ServerSession_UseCatalogIDsForConcurrentPlayerSessions; }
        [JsonProperty(Order = 602)] public Dictionary<string, List<long>> ConcurrentPlayerSessionIDs = new Dictionary<string, List<long>>(); // Export-friendly version of the above field
            public bool ShouldSerializeConcurrentPlayerSessionIDs() { return E_Options.ServerSession_IncludeConcurrentPlayerSessions && E_Options.ServerSession_UseCatalogIDsForConcurrentPlayerSessions; }


        //////////////////////////////////////////// CTOR ////////////////////////////////////////////
        public ServerSession()
        {
            
        }
        
        // Input to this method requires a GameEvent index that is always at the initial ServerStartEvent
        public void PopulateFromGameEvents(AnalyzedData analyzedData, int index)
        {
            List<GameEvent> gameEvents = analyzedData.GameEvents;
            // To reiterate, the very first GameEvent (specified by the index param) must be the initial ServerStartEvent
            Range.Start = gameEvents[index].Source.Time;
            ServerMinecraftVersion = (gameEvents[index] as ServerStartEvent).ServerMinecraftVersion;

            bool foundStart = false;
            bool foundBootDone = false;
            bool foundShutdownBegan = false;    // These 4 flags constitute a server shutdown, in order
            bool foundSavingPlayers = false;    // There is no definitive shutdown message to logs
            bool foundSavingWorlds = false;     // The server simply terminates after the last world is saved
            int worldsSavedCount = 0;           // This counter checks the number of worlds saved against the number loaded during the boot
            List<GameEvent> lastOrgList = BootGameEvents;
            for (; index < gameEvents.Count; index++)
            {
                GameEvent ge = gameEvents[index];

                // GameEvent organization
                AllGameEvents.Add(ge);
                if (!foundBootDone)
                    lastOrgList = BootGameEvents;
                else
                {
                    if (foundShutdownBegan)
                        lastOrgList = ShutdownGameEvents;
                    else
                        lastOrgList = RunningGameEvents;
                }
                lastOrgList.Add(ge);

                ///// Server start event
                // First event of a server session. Finding two of these without first finding a shutdown event indicates a crash or early termination
                ServerStartEvent startGE = ge as ServerStartEvent;
                if (startGE != null)
                {
                    if (foundStart) // Finding a second server start unexpectedly, before finding a complete series of shutdown events
                    {
                        lastOrgList.Remove(ge);

                        ShutdownReason = "Unexpected";

                        // This ends the server session
                        Range.End = ge.Source.Time;
                        EndedNormally = false;
                        break;
                    }
                    else
                        foundStart = true;
                }
                
                ///// Server gametype event
                // Tells the default gametype (gamemode)
                ServerGametypeEvent gametypeGE = ge as ServerGametypeEvent;
                if (gametypeGE != null)
                {
                    ServerDefaultGametype = gametypeGE.DefaultGametype;
                }

                ///// Server address event
                // Tells the server's IP and port
                ServerAddressEvent addressGE = ge as ServerAddressEvent;
                if (addressGE != null)
                {
                    ServerAddress = addressGE.ServerAddress;
                    ServerIP = addressGE.ServerIP;
                    ServerPort = addressGE.ServerPort;
                }

                ///// Server version event
                // Tells what flavor of minecraft is used (i.e. CraftBukkit, Spigot, etc.)
                ServerVersionEvent versionGE = ge as ServerVersionEvent;
                if (versionGE != null)
                {
                    ServerFlavor = versionGE.ServerFlavor;
                    ServerFlavorVersion = versionGE.FlavorVersion;
                    ServerFlavorAPIVersion = versionGE.FlavorAPIVersion;
                }

                ///// Server level load event
                // This is the only way to gather what worlds the server loads (and thus count how it needs to save for a successful shutdown)
                ServerLevelLoadEvent loadLevelGE = ge as ServerLevelLoadEvent;
                if (loadLevelGE != null)
                {
                    ServerLoadedLevels.Add(new ServerLevel(loadLevelGE.LevelName, loadLevelGE.LevelSeed));
                }

                ///// Server boot done event
                // Indicates the end of the boot phase and the beginning of normal running phase
                ServerBootDoneEvent bootdoneGE = ge as ServerBootDoneEvent;
                if (bootdoneGE != null)
                {
                    ServerBootTime = bootdoneGE.BootTime;
                    TimeOfBootDone = bootdoneGE.Source.Time;
                    foundBootDone = true;
                }
                
                ///// Server stopping event
                // First event of the shutdown process (something invoked /stop)
                ServerStoppingEvent stoppingGE = ge as ServerStoppingEvent;
                if (stoppingGE != null)
                {
                    if (!foundBootDone)
                        EndedNormally = false;

                    foundShutdownBegan = true;
                }

                ///// Saving players event
                // Occurs after the ServerStoppingEvent
                SavingPlayersEvent savingPlayerGE = ge as SavingPlayersEvent;
                if (savingPlayerGE != null)
                {
                    foundSavingPlayers = true;
                }

                ///// Saving worlds event
                // Occurs after the SavingPlayersEvent
                SavingWorldsEvent savingWorldsGE = ge as SavingWorldsEvent;
                if (savingWorldsGE != null)
                {
                    foundSavingWorlds = true;
                }

                ///// Saving chunks event
                // Occurs after the SavingWorldsEvent. Each level fires one of these.
                // This is used to check for a successful shutdown by the number of levels saved vs the number of levels loaded during boot
                // Annoyingly, the world names used during loading and saving are different, hence the use of simple a counting check
                SavingChunksEvent savingChunksGE = ge as SavingChunksEvent;
                if (savingChunksGE != null)
                {
                    worldsSavedCount++;

                    if (   worldsSavedCount == ServerLoadedLevels.Count
                        && foundShutdownBegan
                        && foundSavingPlayers
                        && foundSavingWorlds)
                    {
                        ShutdownReason = "Server stopped normally";

                        // This ends the server session
                        Range.End = ge.Source.Time;
                        EndedNormally = true;
                        break;
                    }
                }
                
                ///// Player join event
                // Tells the UUIDs of all players who were present during this session
                PlayerJoinEvent playerjoinGE = ge as PlayerJoinEvent;
                if (playerjoinGE != null)
                {
                    PlayerSession psession = analyzedData.FindPlayerSessionFor(playerjoinGE);
                    if (psession != null)
                    {
                        if (!ConcurrentPlayerSessions.ContainsKey(playerjoinGE.Player.UUID))
                            ConcurrentPlayerSessions[playerjoinGE.Player.UUID] = new List<PlayerSession>();
                        ConcurrentPlayerSessions[playerjoinGE.Player.UUID].Add(psession);

                    }
                }

                ///// Player chat event
                PlayerChatEvent playerchatGE = ge as PlayerChatEvent;
                if (playerchatGE != null)
                {
                    PlayerChatEvents.Add(ge);
                }

                ///// Player death event
                PlayerDeathEvent playerdeathGE = ge as PlayerDeathEvent;
                if (playerdeathGE != null)
                {
                    PlayerDeathEvents.Add(ge);
                }

                ///// Player command event
                PlayerCommandEvent playercommGE = ge as PlayerCommandEvent;
                if (playercommGE != null)
                {
                    PlayerCommandEvents.Add(ge);

                    PlayerTellEvent playertellGE = ge as PlayerTellEvent;
                    if (playertellGE != null)
                    {
                        PlayerChatEvents.Add(ge);
                    }
                }
            }
        }
    }
}
