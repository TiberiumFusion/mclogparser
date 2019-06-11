using com.tiberiumfusion.minecraft.logparserlib.Formats.GameEvents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace com.tiberiumfusion.minecraft.logparserlib.Formats
{
    // A gameplay session of a specific player, including events
    public class PlayerSession
    {
        [JsonProperty(Order = 0)] public long E_ID { get; set; } // E_ for "filled in by the Exporter". Index that this object occupies in the Json export catalog.
        [JsonIgnore] public ExporterOptions E_Options; // Awareness of export options for the ShouldSerialize conditionals


        //================| Serializable fields |================//

        [JsonProperty(Order = 103)] public TimeRange Range = new TimeRange();
            public bool ShouldSerializeRange() { return E_Options.PlayerSession_IncludeRange; }

        [JsonProperty(Order = 101)] public string PlayerUUID;
            public bool ShouldSerializePlayerUUID() { return E_Options.PlayerSession_IncludePlayerUUID; }

        [JsonProperty(Order = 102)] public SortedDictionary<DateTime, string> PlayerContemporaryNames = new SortedDictionary<DateTime, string>(); // List of human-readable names the player used during this session, keyed by the their first appearance
            public bool ShouldSerializePlayerContemporaryNames() { return E_Options.PlayerSession_IncludePlayerContemporaryNames; }

        [JsonProperty(Order = 301)] public List<GameEvent> AllConcurrentGameEvents = new List<GameEvent>(); // All GameEvents that happened during this session
            public bool ShouldSerializeAllConcurrentGameEvents() { return E_Options.PlayerSession_IncludeAllConcurrentGameEvents && !E_Options.PlayerSession_UseCatalogIDsForAllConcurrentGameEvents; }
        [JsonProperty(Order = 302)] public List<long> AllConcurrentGameEventIDs = new List<long>(); // Export-friendly version of the above field
            public bool ShouldSerializeAllConcurrentGameEventIDs() { return E_Options.PlayerSession_IncludeAllConcurrentGameEvents && E_Options.PlayerSession_UseCatalogIDsForAllConcurrentGameEvents; }

        [JsonProperty(Order = 311)] public List<GameEvent> AllRelevantGameEvents = new List<GameEvent>(); // Only GameEvents that are relevant to this player (own join, own leave, own deaths, to/from chat, etc.)
            public bool ShouldSerializeAllRelevantGameEvents() { return E_Options.PlayerSession_IncludeAllRelevantGameEvents && !E_Options.PlayerSession_UseCatalogIDsForAllRelevantGameEvents; }
        [JsonProperty(Order = 312)] public List<long> AllRelevantGameEventIDs = new List<long>(); // Export-friendly version of the above field
            public bool ShouldSerializeAllRelevantGameEventIDs() { return E_Options.PlayerSession_IncludeAllRelevantGameEvents && E_Options.PlayerSession_UseCatalogIDsForAllRelevantGameEvents; }

        [JsonProperty(Order = 601)] public List<DecoratedLog> SourceLogFiles = new List<DecoratedLog>(); // The physical log files which encompass the events of this PlayerSession
            public bool ShouldSerializeSourceLogFiles() { return E_Options.PlayerSession_IncludeSourceLogFiles && !E_Options.PlayerSession_UseCatalogIDsForSourceLogFiles; }
        [JsonProperty(Order = 602)] public List<long> SourceLogFileIDs = new List<long>(); // Export-friendly version of the above field
            public bool ShouldSerializeSourceLogFileIDs() { return E_Options.PlayerSession_IncludeSourceLogFiles && E_Options.PlayerSession_UseCatalogIDsForSourceLogFiles; }

        [JsonProperty(Order = 105)] public string LeaveReason; // Reason for leaving the server, as provided by the server. E.g. "Disconnected" or "Lost connection"
            public bool ShouldSerializeLeaveReason() { return E_Options.PlayerSession_IncludeLeaveReason; }

        [JsonProperty(Order = 106)] public bool EndedNormally = false; // Not ended by a crash, lost disconnect message, etc.
            public bool ShouldSerializeEndedNormally() { return E_Options.PlayerSession_IncludeEndedNormally; }

        [JsonProperty(Order = 501)] public ServerSession ConcurrentServerSession; // The ServerSession in progress during this PlayerSession
            public bool ShouldSerializeConcurrentServerSession() { return E_Options.PlayerSession_IncludeConcurrentServerSession && !E_Options.PlayerSession_UseCatalogIDForConcurrentServerSession; }
        [JsonProperty(Order = 502)] public long ConcurrentServerSessionID; // Export-friendly version of the above field
            public bool ShouldSerializeConcurrentServerSessionID() { return E_Options.PlayerSession_IncludeConcurrentServerSession && E_Options.PlayerSession_UseCatalogIDForConcurrentServerSession; }

        ///// Specific GameEvent collections for simpler lookups
        [JsonProperty(Order = 401)] public List<GameEvent> ChatEvents = new List<GameEvent>(); // All chat events, including /tell commands, that are sent by or sent to this player
            public bool ShouldSerializeChatEvents() { return E_Options.PlayerSession_IncludeChatEvents && !E_Options.PlayerSession_UseCatalogIDsForChatEvents; }
        [JsonProperty(Order = 402)] public List<long> ChatEventIDs = new List<long>(); // Export-friendly version of the above field
            public bool ShouldSerializeChatEventIDs() { return E_Options.PlayerSession_IncludeChatEvents && E_Options.PlayerSession_UseCatalogIDsForChatEvents; }

        [JsonProperty(Order = 411)] public List<GameEvent> DeathEvents = new List<GameEvent>(); // All of this player's deaths
            public bool ShouldSerializeDeathEvents() { return E_Options.PlayerSession_IncludeDeathEvents && !E_Options.PlayerSession_UseCatalogIDsForDeathEvents; }
        [JsonProperty(Order = 412)] public List<long> DeathEventIDs = new List<long>(); // Export-friendly version of the above field
            public bool ShouldSerializeDeathEventIDs() { return E_Options.PlayerSession_IncludeDeathEvents && E_Options.PlayerSession_UseCatalogIDsForDeathEvents; }

        [JsonProperty(Order = 421)] public List<GameEvent> CommandEvents = new List<GameEvent>(); // All of the commands run by this player
            public bool ShouldSerializeCommandEvents() { return E_Options.PlayerSession_IncludeCommandEvents && !E_Options.PlayerSession_UseCatalogIDsForCommandEvents; }
        [JsonProperty(Order = 422)] public List<long> CommandEventIDs = new List<long>(); // Export-friendly version of the above field
            public bool ShouldSerializeCommandEventIDs() { return E_Options.PlayerSession_IncludeCommandEvents && E_Options.PlayerSession_UseCatalogIDsForCommandEvents; }


        [JsonProperty(Order = 201)] public string PlayerAddress;
            public bool ShouldSerializePlayerAddress() { return E_Options.PlayerSession_IncludePlayerAddress; }

        [JsonProperty(Order = 202)] public string PlayerIP;
            public bool ShouldSerializePlayerIP() { return E_Options.PlayerSession_IncludePlayerIP; }

        [JsonProperty(Order = 203)] public string PlayerPort;
            public bool ShouldSerializePlayerPort() { return E_Options.PlayerSession_IncludePlayerPort; }

        [JsonProperty(Order = 204)] public Location PlayerLoginLocation;
            public bool ShouldSerializePlayerLoginLocation() { return E_Options.PlayerSession_IncludePlayerLoginLocation; }


        //////////////////////////////////////////// CTOR ////////////////////////////////////////////
        public PlayerSession(string uuid)
        {
            PlayerUUID = uuid;
        }

        // This is the first pass of session analysis, which establishes the structure of the playersession and gathers UUID data needed for the second pass
        // We only inspect player join and leave events (minimum needed to establish the range and UUID data for a session), but all time-range-included gameevents are noted
        // Input to this method must be the entire range of GameEvents, with the provided index always at the initial PlayerJoinEvent
        public void PopulateFromGameEventsFirstPass(List<GameEvent> gameEvents, int index)
        {
            // To reiterate, the very first GameEvent (specified by the index param) must be the initial PlayerJoinEvent
            Range.Start = gameEvents[index].Source.Time;
            PlayerUUID = (gameEvents[index] as PlayerJoinEvent).Player.UUID;
            
            bool foundJoinEvent = false;
            bool foundDisconnectEvent = false;
            for (; index < gameEvents.Count; index++)
            {
                GameEvent ge = gameEvents[index];

                // Always add to list of all events
                AllConcurrentGameEvents.Add(ge);

                ///// Join event
                // We can catch player name history from these
                PlayerJoinEvent joinGE = ge as PlayerJoinEvent;
                if (joinGE != null)
                {
                    if (joinGE.Player.UUID == PlayerUUID)
                    {
                        // If we've found a second join event (i.e. foundJoinEvent was previously set to true), then the leave event for this session was lost, so we'll terminate this session at this point
                        if (foundJoinEvent)
                        {
                            if (!foundDisconnectEvent)
                                AllConcurrentGameEvents.RemoveAt(AllConcurrentGameEvents.Count - 1);
                            LeaveReason = "";
                            Range.End = ge.Source.Time;

                            // This ends the player's session
                            EndedNormally = false;
                            break;
                        }
                        else
                        {
                            foundJoinEvent = true;
                            AllRelevantGameEvents.Add(ge);
                            if (!PlayerContemporaryNames.ContainsValue(joinGE.Player.Name))
                                PlayerContemporaryNames[ge.Source.Time] = joinGE.Player.Name;
                        }
                    }
                }
                
                ///// Nickname event
                // The other source of player name changes
                PlayerNicknameEvent nicknameGE = ge as PlayerNicknameEvent;
                if (nicknameGE != null)
                {
                    // The UUID pass hasn't happened yet, so we can only compare contemporary names
                    if (PlayerContemporaryNames.ContainsValue(nicknameGE.TargetPlayer.Name))
                    {
                        if (!PlayerContemporaryNames.ContainsValue(nicknameGE.AssignedPlayerNickname))
                            PlayerContemporaryNames[ge.Source.Time] = nicknameGE.AssignedPlayerNickname;
                    }
                }

                ///// Leave event
                PlayerLeaveEvent leaveGE = ge as PlayerLeaveEvent;
                if (leaveGE != null)
                {
                    if (leaveGE.Player.Name == PlayerContemporaryNames.Values.Last())
                    {
                        AllRelevantGameEvents.Add(ge);
                        PlayerLostConnEvent lostConnGE = ge as PlayerLostConnEvent;
                        if (lostConnGE != null)
                        {
                            LeaveReason = lostConnGE.LostConnReason;
                            // Typically, a PlayerLostConnEvent occurs just before a normal PlayerLeaveEvent, so for completeness, we'll keep searching for a normal leave event to end this session
                            foundDisconnectEvent = true;
                            EndedNormally = true;
                            Range.End = ge.Source.Time;
                        }
                        else // Is a normal PlayerLeaveEvent
                        {
                            if (foundDisconnectEvent)
                            {
                                EndedNormally = true;
                            }
                            else
                            {
                                EndedNormally = false;
                                LeaveReason = "";
                            }
                            Range.End = ge.Source.Time;

                            // This ends the player's session
                            break;
                        }
                    }
                }


            }

            // Running out of GameEvents before seeing a leave event (i.e. server crash) will also end the session
            if (!foundDisconnectEvent)
                EndedNormally = false;
        }

        // This is the second pass of session analysis, wherein we now have UUID data attached to plain contemporary player names in GameEvents (such as the leave server event)
        public void PopulateSecondPass()
        {
            HashSet<DecoratedLog> workingLogTrack = new HashSet<DecoratedLog>();

            // Adds GameEvents to RelevantGameEvents, then, when all is done, uses OrderBy on the LogLine's Time field to restore chronological order
            foreach (GameEvent ge in AllConcurrentGameEvents)
            {
                workingLogTrack.Add(ge.Source.ParentLogLineList.ParentDecoratedLog);

                ///// Login event. The login event is separate from the join, and carries address and location information
                PlayerLoginEvent playerLoginGE = ge as PlayerLoginEvent;
                if (playerLoginGE != null)
                {
                    if (playerLoginGE.Player.UUID == PlayerUUID)
                    {
                        AllRelevantGameEvents.Add(ge);

                        PlayerAddress = playerLoginGE.PlayerAddress;
                        PlayerIP = playerLoginGE.PlayerIP;
                        PlayerPort = playerLoginGE.PlayerPort;
                        PlayerLoginLocation = playerLoginGE.PlayerLocation;
                    }
                }

                ///// Death event
                PlayerDeathEvent playerDiedGE = ge as PlayerDeathEvent;
                if (playerDiedGE != null)
                {
                    if (playerDiedGE.Player.UUID == PlayerUUID)
                    {
                        AllRelevantGameEvents.Add(ge);
                        DeathEvents.Add(ge);
                    }
                }

                ///// Opped by server
                ServerOppedPlayerEvent playerOppedGE = ge as ServerOppedPlayerEvent;
                if (playerOppedGE != null)
                {
                    if (playerOppedGE.PromotedPlayer.UUID == PlayerUUID)
                        AllRelevantGameEvents.Add(ge);
                }

                ///// Deopped by server
                ServerDeoppedPlayerEvent playerDeoppedGE = ge as ServerDeoppedPlayerEvent;
                if (playerDeoppedGE != null)
                {
                    if (playerDeoppedGE.DemotedPlayer.UUID == PlayerUUID)
                        AllRelevantGameEvents.Add(ge);
                }

                ///// Issuing a command
                PlayerCommandEvent playerCommandGE = ge as PlayerCommandEvent;
                if (playerCommandGE != null)
                {
                    bool alreadyAdded = false;

                    if (playerCommandGE.ExecutingPlayer.UUID == PlayerUUID)
                    {
                        AllRelevantGameEvents.Add(ge);
                        alreadyAdded = true;
                        CommandEvents.Add(ge);
                    }

                    PlayerTellEvent tellGE = playerCommandGE as PlayerTellEvent;
                    if (tellGE != null)
                    {
                        if (tellGE.ReceivingPlayer.UUID == PlayerUUID)
                        {
                            if (!alreadyAdded)
                            {
                                AllRelevantGameEvents.Add(ge);
                                alreadyAdded = true;
                            }
                            ChatEvents.Add(ge);
                        }
                    }
                }

                ///// Chatting
                PlayerChatEvent chatGE = ge as PlayerChatEvent;
                if (chatGE != null)
                {
                    if (chatGE.SendingPlayer.UUID == PlayerUUID)
                    {
                        AllRelevantGameEvents.Add(ge);
                        ChatEvents.Add(ge);
                    }
                }

                ///// ChestShop interactions
                ChestShopBuyEvent csbuyGE = ge as ChestShopBuyEvent;
                if (csbuyGE != null)
                {
                    if (csbuyGE.PurchasingPlayer.UUID == PlayerUUID)
                        AllRelevantGameEvents.Add(ge);
                }
                ChestShopSellEvent cssellGE = ge as ChestShopSellEvent;
                if (cssellGE != null)
                {
                    if (cssellGE.SellingPlayer.UUID == PlayerUUID)
                        AllRelevantGameEvents.Add(ge);
                }
                ChestShopCreatedEvent cscreateGE = ge as ChestShopCreatedEvent;
                if (cscreateGE != null)
                {
                    if (cscreateGE.CreatedByPlayer.UUID == PlayerUUID)
                        AllRelevantGameEvents.Add(ge);
                }
            }

            // Sort GEs into chronological order
            AllRelevantGameEvents = AllRelevantGameEvents.OrderBy(x => x.Source.Time).ToList();

            // Fill in SourceLogFiles
            SourceLogFiles = workingLogTrack.ToList();
            SourceLogFiles = SourceLogFiles.OrderBy(x => x.TimeRange.Start).ToList();
        }
    }
}
