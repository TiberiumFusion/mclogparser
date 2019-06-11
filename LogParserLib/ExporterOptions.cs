using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.tiberiumfusion.minecraft.logparserlib
{
    // Grouper of options for the exporter
    public class ExporterOptions
    {
        [JsonIgnore] private const string parsingExceptionPrefix = "Invalid configuration in exporter options: ";

        //================| General options |================//

        public bool IncludeExportOptions; // If true, these options will be included in the serialized josn

        public bool IndentationAndFormatting; // If true, json will be formatted with indentation and newlines. Otherwise it will be compacted.

        public string SerializedCatalogFormat; // User chooses this value as string, which is parsed into the enum field below
        public ExportCatalogFormat CatalogFormat; // See comments in ExportCatalogFormat.cs

        public bool IncludeAnalysisStats; // Include some statistics about the analysis process

        public bool IncludeGrandLogLineTotal; // Includes the GrandLogLineTotal field (count of all LogLines parsed)


        //================| DecoratedLog fields & subfields |================//

        public bool IncludeDecoratedLogCatalog; // Includes the catalog of DecoratedLogs

        public bool DecoratedLog_IncludeSourceFilepath; // Includes the SourceFilepath field

        public bool DecoratedLog_IncludeSourceFilename; // Includes the SourceFilename field

        public bool DecoratedLog_IncludeSourceFilenameNoExt; // Includes the SourceFilenameNoExt field

        public bool DecoratedLog_IncludeSourceDirectory; // Includes the SourceDirectory field

        public bool DecoratedLog_IncludeFilenameDate; // Includes the FilenameDate field

        public bool DecoratedLog_IncludeFilenameNumber; // Includes the FilenameNumber field

        public bool DecoratedLog_IncludeDissectedLogLines; // Includes the DissectedLines part of the LogLines field

        public bool DecoratedLog_IncludeRawLogLines; // Includes the RawLines part of the LogLines field

        public bool DecoratedLog_IncludeTimeRange; // Includes the TimeRange field


        //================| GameEvent fields |================//

        public bool IncludeGameEventCatalog; // Includes the catalog of GameEvents

        public bool GameEvent_IncludeSourceLogLine; // Includes the LogLine object that the GameEvent was built from

        public bool GameEvent_UseCatalogIDsForSourceLogLine; // Use a catalog ID for the above object, instead of inlining it


        //================| Subclassed & misc GameEvent fields |================//

        public bool FormattedChatString_UseHTMLFormatting; // If true, FormattedChatString objects are exported as an HTML-formatted string. If false, they will use the formatting enum (see ChatColorType.cs).

        public string FormattedChatString_HTMLTag; // The HTML element to wrap around the text

        public string FormattedChatString_HTMLClassForMagic; // The name of your special CSS class to apply to text with the "magic" formatting (§k)


        //================| PlayerChatEvent fields |================//

        public bool PlayerChatEvent_IncludeRawFullMessage; // Includes the RawFullMessage field

        public bool PlayerChatEvent_IncludePlainFullMessage; // Includes the PlainFullMessage field

        public bool PlayerChatEvent_IncludePlainTag; // Includes the PlainTag field

        public bool PlayerChatEvent_IncludePlainBody; // Includes the PlainBody field

        public bool PlayerChatEvent_IncludeReEncodedFullMessage; // Includes the ReEncodedFullMessage field

        public bool PlayerChatEvent_IncludeReEncodedTag; // Includes the ReEncodedTag field

        public bool PlayerChatEvent_IncludeReEncodedBody; // Includes the ReEncodedBody field

        public bool PlayerChatEvent_IncludeFormatGroupedTag; // Includes the FormatGroupedTag field

        public bool PlayerChatEvent_IncludeFormatGroupedBody; // Includes the FormatGroupedBody field


        //================| PlayerSession fields |================//

        public bool IncludePlayerSessionCatalog; // Includes the catalog of PlayerSessions

        public bool PlayerSession_IncludeRange; // Includes the Range field

        public bool PlayerSession_IncludeDuration; // Includes the Duration field

        public bool PlayerSession_IncludePlayerUUID; // Includes the PlayerUUID field

        public bool PlayerSession_IncludePlayerContemporaryNames; // Includes the PlayerContemporaryNames field

        public bool PlayerSession_IncludeAllConcurrentGameEvents; // Includes the AllConcurrentGameEvents field
        public bool PlayerSession_UseCatalogIDsForAllConcurrentGameEvents; // Use catalog IDs for the above object, instead of inlining it

        public bool PlayerSession_IncludeAllRelevantGameEvents; // Includes the AllRelevantGameEvents field
        public bool PlayerSession_UseCatalogIDsForAllRelevantGameEvents; // Use catalog IDs for the above object, instead of inlining it

        public bool PlayerSession_IncludeSourceLogFiles; // Includes the SourceLogFiles field
        public bool PlayerSession_UseCatalogIDsForSourceLogFiles; // Use catalog IDs for the above object, instead of inlining it

        public bool PlayerSession_IncludeLeaveReason; // Includes the LeaveReason field

        public bool PlayerSession_IncludeEndedNormally; // Includes the EndedNormally field

        public bool PlayerSession_IncludeConcurrentServerSession; // Includes the ConcurrentServerSession field
        public bool PlayerSession_UseCatalogIDForConcurrentServerSession; // Use catalog IDs for the above object, instead of inlining it

        public bool PlayerSession_IncludeChatEvents; // Includes the ChatEvents field
        public bool PlayerSession_UseCatalogIDsForChatEvents; // Use catalog IDs for the above object, instead of inlining it

        public bool PlayerSession_IncludeDeathEvents; // Includes the DeathEvents field
        public bool PlayerSession_UseCatalogIDsForDeathEvents; // Use catalog IDs for the above object, instead of inlining it

        public bool PlayerSession_IncludeCommandEvents; // Includes the CommandEvents field
        public bool PlayerSession_UseCatalogIDsForCommandEvents; // Use catalog IDs for the above object, instead of inlining it

        public bool PlayerSession_IncludePlayerAddress; // Includes the PlayerAddress field

        public bool PlayerSession_IncludePlayerIP; // Includes the PlayerIP field

        public bool PlayerSession_IncludePlayerPort; // Includes the PlayerPort field

        public bool PlayerSession_IncludePlayerLoginLocation; // Includes the PlayerLoginLocation field


        //================| ServerSession fields |================//

        public bool IncludeServerSessionCatalog; // Includes the catalog of ServerSessions

        public bool ServerSession_IncludeRange; // Includes the Range field

        public bool ServerSession_IncludeDuration; // Includes the Duration field

        public bool ServerSession_IncludeAllGameEvents; // Includes the AllGameEvents field
        public bool ServerSession_UseCatalogIDsForAllGameEvents; // Use catalog IDs for the above object, instead of inlining it

        public bool ServerSession_IncludeBootGameEvents; // Includes the BootGameEvents field
        public bool ServerSession_UseCatalogIDsForBootGameEvents; // Use catalog IDs for the above object, instead of inlining it

        public bool ServerSession_IncludeRunningGameEvents; // Includes the RunningGameEvents field
        public bool ServerSession_UseCatalogIDsForRunningGameEvents; // Use catalog IDs for the above object, instead of inlining it

        public bool ServerSession_IncludeShutdownGameEvents; // Includes the ShutdownGameEvents field
        public bool ServerSession_UseCatalogIDsForShutdownGameEvents; // Use catalog IDs for the above object, instead of inlining it

        public bool ServerSession_IncludeSourceLogFiles; // Includes the SourceLogFiles field
        public bool ServerSession_UseCatalogIDsForSourceLogFiles; // Use catalog IDs for the above object, instead of inlining it

        public bool ServerSession_IncludeEndedNormally; // Includes the EndedNormally field
        public bool ServerSession_IncludeShutdownReason; // Includes the ShutdownReason field

        public bool ServerSession_IncludePlayerChatEvents; // Includes the PlayerChatEvents field
        public bool ServerSession_UseCatalogIDsForPlayerChatEvents; // Use catalog IDs for the above object, instead of inlining it

        public bool ServerSession_IncludePlayerDeathEvents; // Includes the PlayerDeathEvents field
        public bool ServerSession_UseCatalogIDsForPlayerDeathEvents; // Use catalog IDs for the above object, instead of inlining it

        public bool ServerSession_IncludePlayerCommandEvents; // Includes the PlayerCommandEvents field
        public bool ServerSession_UseCatalogIDsForPlayerCommandEvents; // Use catalog IDs for the above object, instead of inlining it

        public bool ServerSession_IncludeServerMinecraftVersion; // Includes the Range field

        public bool ServerSession_IncludeServerDefaultGametype; // Includes the Range field

        public bool ServerSession_IncludeServerAddress; // Includes the Range field

        public bool ServerSession_IncludeServerIP; // Includes the Range field

        public bool ServerSession_IncludeServerPort; // Includes the Range field

        public bool ServerSession_IncludeServerFlavor; // Includes the Range field

        public bool ServerSession_IncludeServerFlavorVersion; // Includes the Range field

        public bool ServerSession_IncludeServerFlavorAPIVersion; // Includes the Range field

        public bool ServerSession_IncludeServerBootTime; // Includes the Range field

        public bool ServerSession_IncludeTimeOfBootDone; // Includes the Range field

        public bool ServerSession_IncludeServerLoadedLevels; // Includes the Range field

        public bool ServerSession_IncludeConcurrentPlayerSessions; // Includes the ConcurrentPlayerSessions field
        public bool ServerSession_UseCatalogIDsForConcurrentPlayerSessions; // Use catalog IDs for the above object, instead of inlining it


        //================| GameEvent fields |================//

        public bool IncludeAllPlayerStats; // Includes the IncludeAllPlayerStats collection

        public bool PlayerStats_IncludeSessions; // Includes the Sessions field

        public bool PlayerStats_UseCatalogIDsForSessions; // Use catalog IDs for the above object, instead of inlining it

        public bool PlayerStats_IncludeTotalGametime; // Includes the TotalGametime field

        public bool PlayerStats_IncludeAllPlayerContemporaryNames; // Includes the AllPlayerContemporaryNames field



        // Defaults
        public ExporterOptions()
        {
            IncludeExportOptions = false;

            IndentationAndFormatting = true;

            SerializedCatalogFormat = "lists";

            IncludeAnalysisStats = true;

            IncludeGrandLogLineTotal = true;

            IncludeDecoratedLogCatalog = true;
            DecoratedLog_IncludeSourceFilepath = true;
            DecoratedLog_IncludeSourceFilename = true;
            DecoratedLog_IncludeSourceFilenameNoExt = true;
            DecoratedLog_IncludeSourceDirectory = true;
            DecoratedLog_IncludeFilenameDate = true;
            DecoratedLog_IncludeFilenameNumber = true;
            DecoratedLog_IncludeDissectedLogLines = true;
            DecoratedLog_IncludeRawLogLines = false;
            DecoratedLog_IncludeTimeRange = true;

            IncludeGameEventCatalog = true;
            GameEvent_IncludeSourceLogLine = true;
            GameEvent_UseCatalogIDsForSourceLogLine = true;

            FormattedChatString_UseHTMLFormatting = true;
            FormattedChatString_HTMLTag = "span";
            FormattedChatString_HTMLClassForMagic = "magicText";

            PlayerChatEvent_IncludeRawFullMessage = false;
	        PlayerChatEvent_IncludePlainFullMessage = true;
	        PlayerChatEvent_IncludePlainTag = false;
	        PlayerChatEvent_IncludePlainBody = false;
	        PlayerChatEvent_IncludeReEncodedFullMessage = false;
	        PlayerChatEvent_IncludeReEncodedTag = false;
	        PlayerChatEvent_IncludeReEncodedBody = false;
	        PlayerChatEvent_IncludeFormatGroupedTag = true;
	        PlayerChatEvent_IncludeFormatGroupedBody = true;

            IncludePlayerSessionCatalog = true;
            PlayerSession_IncludeRange = true;
            PlayerSession_IncludeDuration = true;
            PlayerSession_IncludePlayerUUID = true;
            PlayerSession_IncludePlayerContemporaryNames = true;
            PlayerSession_IncludeAllConcurrentGameEvents = true;
            PlayerSession_UseCatalogIDsForAllConcurrentGameEvents = true;
            PlayerSession_IncludeAllRelevantGameEvents = true;
            PlayerSession_UseCatalogIDsForAllRelevantGameEvents = true;
            PlayerSession_IncludeSourceLogFiles = true;
            PlayerSession_UseCatalogIDsForSourceLogFiles = true;
            PlayerSession_IncludeLeaveReason = true;
            PlayerSession_IncludeEndedNormally = true;
            PlayerSession_IncludeConcurrentServerSession = true;
            PlayerSession_UseCatalogIDForConcurrentServerSession = true;
            PlayerSession_IncludeChatEvents = true;
            PlayerSession_UseCatalogIDsForChatEvents = true;
            PlayerSession_IncludeDeathEvents = true;
            PlayerSession_UseCatalogIDsForDeathEvents = true;
            PlayerSession_IncludeCommandEvents = true;
            PlayerSession_UseCatalogIDsForCommandEvents = true;
            PlayerSession_IncludePlayerAddress = true;
            PlayerSession_IncludePlayerIP = true;
            PlayerSession_IncludePlayerPort = true;
            PlayerSession_IncludePlayerLoginLocation = true;

            IncludeServerSessionCatalog = true;
            ServerSession_IncludeRange = true;
	        ServerSession_IncludeDuration = true;
	        ServerSession_IncludeAllGameEvents = true;
	        ServerSession_UseCatalogIDsForAllGameEvents = true;
	        ServerSession_IncludeBootGameEvents = true;
	        ServerSession_UseCatalogIDsForBootGameEvents = true;
	        ServerSession_IncludeRunningGameEvents = true;
	        ServerSession_UseCatalogIDsForRunningGameEvents = true;
	        ServerSession_IncludeShutdownGameEvents = true;
	        ServerSession_UseCatalogIDsForShutdownGameEvents = true;
	        ServerSession_IncludeSourceLogFiles = true;
	        ServerSession_UseCatalogIDsForSourceLogFiles = true;
	        ServerSession_IncludeEndedNormally = true;
	        ServerSession_IncludeShutdownReason = true;
	        ServerSession_IncludePlayerChatEvents = true;
	        ServerSession_UseCatalogIDsForPlayerChatEvents = true;
	        ServerSession_IncludePlayerDeathEvents = true;
	        ServerSession_UseCatalogIDsForPlayerDeathEvents = true;
	        ServerSession_IncludePlayerCommandEvents = true;
	        ServerSession_UseCatalogIDsForPlayerCommandEvents = true;
	        ServerSession_IncludeServerMinecraftVersion = true;
	        ServerSession_IncludeServerDefaultGametype = true;
	        ServerSession_IncludeServerAddress = true;
	        ServerSession_IncludeServerIP = true;
	        ServerSession_IncludeServerPort = true;
	        ServerSession_IncludeServerFlavor = true;
	        ServerSession_IncludeServerFlavorVersion = true;
	        ServerSession_IncludeServerFlavorAPIVersion = true;
	        ServerSession_IncludeServerBootTime = true;
	        ServerSession_IncludeTimeOfBootDone = true;
	        ServerSession_IncludeServerLoadedLevels = true;
	        ServerSession_IncludeConcurrentPlayerSessions = true;
	        ServerSession_UseCatalogIDsForConcurrentPlayerSessions = true;

            IncludeAllPlayerStats = true;
            PlayerStats_IncludeSessions = true;
            PlayerStats_UseCatalogIDsForSessions = true;
            PlayerStats_IncludeTotalGametime = true;
            PlayerStats_IncludeAllPlayerContemporaryNames = true;
        }

        public static ExporterOptions CreateFromJson(string rawJson)
        {
            ExporterOptions opts = JsonConvert.DeserializeObject<ExporterOptions>(rawJson);


            ///// Final parsing
            
            ExportCatalogFormat catFormatParsed;
            bool catFormatRes = Enum.TryParse(opts.SerializedCatalogFormat.ToLowerInvariant(), true, out catFormatParsed);
            if (!catFormatRes || !Enum.IsDefined(typeof(ExportCatalogFormat), catFormatParsed))
                throw new Exception(parsingExceptionPrefix + "Provided value for SerializedCatalogFormat is invalid.");
            opts.CatalogFormat = catFormatParsed;


            ///// Validation

            // Dissected loglines requires the Dlog catalog
            if (opts.DecoratedLog_IncludeDissectedLogLines && !opts.IncludeDecoratedLogCatalog)
                throw new Exception(parsingExceptionPrefix + "DecoratedLog_IncludeDissectedLogLines = true requires: IncludeDecoratedLogCatalog = true");

            // GE source log IDs require the loglines logs catalog
            if (opts.GameEvent_UseCatalogIDsForSourceLogLine && !opts.DecoratedLog_IncludeDissectedLogLines)
                throw new Exception(parsingExceptionPrefix + "GameEvent_UseCatalogIDsForSourceLogLine = true requires: DecoratedLog_IncludeDissectedLogLines = true");

            // PS game event IDs require the GE catalog
            if (opts.PlayerSession_UseCatalogIDsForAllConcurrentGameEvents && !opts.IncludeGameEventCatalog)
                throw new Exception(parsingExceptionPrefix + "PlayerSession_UseCatalogIDsForAllConcurrentGameEvents = true requires: IncludeGameEventCatalog = true");
            
            // PS game event IDs require the GE catalog
            if (opts.PlayerSession_UseCatalogIDsForAllRelevantGameEvents && !opts.IncludeGameEventCatalog)
                throw new Exception(parsingExceptionPrefix + "PlayerSession_UseCatalogIDsForAllRelevantGameEvents = true requires: IncludeGameEventCatalog = true");

            // PS source log IDs require the Dlog catalog
            if (opts.PlayerSession_UseCatalogIDsForSourceLogFiles && !opts.IncludeDecoratedLogCatalog)
                throw new Exception(parsingExceptionPrefix + "PlayerSession_UseCatalogIDsForSourceLogFiles = true requires: IncludeDecoratedLogCatalog = true");

            // PS concurrent server session ID requires the server sessions catalog
            if (opts.PlayerSession_UseCatalogIDForConcurrentServerSession && !opts.IncludeServerSessionCatalog)
                throw new Exception(parsingExceptionPrefix + "PlayerSession_UseCatalogIDForConcurrentServerSession = true requires: IncludeServerSessionCatalog = true");

            // PS game event IDs require the GE catalog
            if (opts.PlayerSession_UseCatalogIDsForChatEvents && !opts.IncludeGameEventCatalog)
                throw new Exception(parsingExceptionPrefix + "PlayerSession_UseCatalogIDsForChatEvents = true requires: IncludeGameEventCatalog = true");

            // PS game event IDs require the GE catalog
            if (opts.PlayerSession_UseCatalogIDsForDeathEvents && !opts.IncludeGameEventCatalog)
                throw new Exception(parsingExceptionPrefix + "PlayerSession_UseCatalogIDsForDeathEvents = true requires: IncludeGameEventCatalog = true");

            // PS game event IDs require the GE catalog
            if (opts.PlayerSession_UseCatalogIDsForCommandEvents && !opts.IncludeGameEventCatalog)
                throw new Exception(parsingExceptionPrefix + "PlayerSession_UseCatalogIDsForCommandEvents = true requires: IncludeGameEventCatalog = true");

            // SS game event IDs require the GE catalog
            if (opts.ServerSession_UseCatalogIDsForAllGameEvents && !opts.IncludeGameEventCatalog)
                throw new Exception(parsingExceptionPrefix + "ServerSession_UseCatalogIDsForAllGameEvents = true requires: IncludeGameEventCatalog = true");

            // SS game event IDs require the GE catalog
            if (opts.ServerSession_UseCatalogIDsForBootGameEvents && !opts.IncludeGameEventCatalog)
                throw new Exception(parsingExceptionPrefix + "ServerSession_UseCatalogIDsForBootGameEvents = true requires: IncludeGameEventCatalog = true");

            // SS game event IDs require the GE catalog
            if (opts.ServerSession_UseCatalogIDsForRunningGameEvents && !opts.IncludeGameEventCatalog)
                throw new Exception(parsingExceptionPrefix + "ServerSession_UseCatalogIDsForRunningGameEvents = true requires: IncludeGameEventCatalog = true");

            // SS game event IDs require the GE catalog
            if (opts.ServerSession_UseCatalogIDsForShutdownGameEvents && !opts.IncludeGameEventCatalog)
                throw new Exception(parsingExceptionPrefix + "ServerSession_UseCatalogIDsForShutdownGameEvents = true requires: IncludeGameEventCatalog = true");

            // SS source log IDs require the Dlog catalog
            if (opts.ServerSession_UseCatalogIDsForSourceLogFiles && !opts.IncludeDecoratedLogCatalog)
                throw new Exception(parsingExceptionPrefix + "ServerSession_UseCatalogIDsForSourceLogFiles = true requires: IncludeDecoratedLogCatalog = true");

            // SS game event IDs require the GE catalog
            if (opts.ServerSession_UseCatalogIDsForPlayerChatEvents && !opts.IncludeGameEventCatalog)
                throw new Exception(parsingExceptionPrefix + "ServerSession_UseCatalogIDsForPlayerChatEvents = true requires: IncludeGameEventCatalog = true");

            // SS game event IDs require the GE catalog
            if (opts.ServerSession_UseCatalogIDsForPlayerDeathEvents && !opts.IncludeGameEventCatalog)
                throw new Exception(parsingExceptionPrefix + "ServerSession_UseCatalogIDsForPlayerDeathEvents = true requires: IncludeGameEventCatalog = true");

            // SS game event IDs require the GE catalog
            if (opts.ServerSession_UseCatalogIDsForPlayerCommandEvents && !opts.IncludeGameEventCatalog)
                throw new Exception(parsingExceptionPrefix + "ServerSession_UseCatalogIDsForPlayerCommandEvents = true requires: IncludeGameEventCatalog = true");

            // SS concurrent player session IDs require the player sessions catalog
            if (opts.ServerSession_UseCatalogIDsForConcurrentPlayerSessions && !opts.IncludePlayerSessionCatalog)
                throw new Exception(parsingExceptionPrefix + "ServerSession_UseCatalogIDsForConcurrentPlayerSessions = true requires: IncludePlayerSessionCatalog = true");


            return opts;
        }
    }
}
