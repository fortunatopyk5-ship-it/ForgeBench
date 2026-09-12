using System;
using System.Collections.Generic;

namespace ForgeBench
{
    public enum PartCategory { Case, Motherboard, CPU, RAM, GPU, Storage, PSU, Cooler, Fan, Network, Battery, Display, Controller, Consumable, Tool }
    public enum DeviceCategory { Desktop, Laptop, Phone, Tablet, Console, Handheld, NAS, Server, Router, Controller }
    public enum JobType { Repair, CustomBuild, Upgrade, Software, Cleaning, Diagnostics, BoardRepair, Network }
    public enum JobStage { Offered, Accepted, InProgress, ReadyToSubmit, Completed, Failed }
    public enum ShipmentStatus { Ordered, InTransit, Delivered, Received, Returned }
    public enum BootState { Off, Posting, PostFailed, Bios, Bootloader, OperatingSystem }
    public enum FaultType { None, DeadPart, Overheating, UnstableMemory, BadStorage, MissingCable, Dust, FanFailure, ConnectorDamage, BatteryWear, DisplayFault, Firmware }
    public enum DamageType { None, BentPins, StrippedFastener, BurnedConnector, LiquidContamination, CrackedSolder, Corrosion, ImpactDamage }
    public enum BenchmarkStatus { NotRun, Passed, Warning, Failed }

    [Serializable]
    public class HardwareCatalogData { public List<HardwareDefinition> parts = new List<HardwareDefinition>(); }

    [Serializable]
    public class HardwareDefinition
    {
        public string id;
        public string brand;
        public string model;
        public PartCategory category;
        public float price;
        public int performance;
        public float powerWatts;
        public float heatWatts;
        public float noiseDb;
        public string socket;
        public string formFactor;
        public string memoryType;
        public int capacityGB;
        public int speed;
        public float lengthMm;
        public float heightMm;
        public string storageInterface;
        public int storageGB;
        public int psuWattage;
        public int pcieGeneration;
        public int vramGB;
        public int quality;
        public int coreCount;
        public int threadCount;
        public int baseClockMHz;
        public int boostClockMHz;
        public float idlePowerWatts;
        public int dimmSlots;
        public int maxMemoryGB;
        public int memoryChannels;
        public int m2Slots;
        public int sataPorts;
        public int pcieX16Slots;
        public int fanHeaders;
        public int driveBays25;
        public int driveBays35;
        public int radiatorSupportMm;
        public float gpuSlotWidth;
        public int fanSizeMm;
        public int maxRpm;
        public float airflowCfm;
        public float staticPressure;
        public int readMBs;
        public int writeMBs;
        public int enduranceTBW;
        public bool modularPsu;
        public int efficiencyClass;
        public int batteryMah;
        public int displayHz;
        public string firmwareTier;
        public List<string> connectors = new List<string>();
        public List<string> tags = new List<string>();
    }

    [Serializable]
    public class ItemInstance
    {
        public string instanceId;
        public string definitionId;
        public float condition = 1f;
        public float dust;
        public float wear;
        public bool reserved;
        public bool customerOwned;
        public string ownerJobId;
        public FaultType fault;
        public DamageType damage;
        public int powerCycles;
        public float readGB;
        public float writtenGB;
        public float lastTempC = 24f;
        public string note;
    }

    [Serializable]
    public class BiosState
    {
        public bool memoryProfileEnabled;
        public int cpuPowerLimitWatts = 95;
        public int fanProfile = 1;
        public bool virtualizationEnabled;
        public bool secureBoot = true;
        public int bootDriveIndex;
        public int memorySpeedOverride;
        public int cpuMultiplierOffset;
        public float voltageOffset;
        public float cpuVoltage = 1.10f;
        public float socVoltage = 1.00f;
        public float memoryVoltage = 1.20f;
        public bool resizableBar = true;
        public bool above4G = true;
        public bool csm;
        public bool lastTrainingPassed = true;
        public string lastTrainingMessage = "Not trained yet";
    }

    [Serializable]
    public class CableState
    {
        public bool atx24;
        public bool cpuEps;
        public bool gpuPower;
        public bool sataPower;
        public bool sataData;
        public bool frontPanel;
        public bool cpuFan;
        public bool pump;
        public bool rgb;
    }

    [Serializable]
    public class FastenerState
    {
        public string fastenerId;
        public string type = "Phillips";
        public float tightness = 1f;
        public bool captive = true;
        public bool damaged;
    }

    [Serializable]
    public class PanelState
    {
        public string panelId = "side-panel";
        public bool installed = true;
        public bool clipDamaged;
        public List<FastenerState> fasteners = new List<FastenerState>();
    }

    [Serializable]
    public class CustomizationState
    {
        public float themeR = 0.08f;
        public float themeG = 0.65f;
        public float themeB = 1f;
        public int rgbEffect;
        public int cableColorIndex;
        public int caseFinishIndex;
        public bool rgbSync = true;
    }

    [Serializable]
    public class LiquidLoopState
    {
        public bool pumpInstalled;
        public bool reservoirInstalled;
        public int radiatorMm;
        public int fittingCount;
        public int tightFittings;
        public float coolantLitres;
        public float airFraction = 1f;
        public float flowLpm;
        public float pressureKpa;
        public bool leakDetected;
        public bool leakTestPassed;
        public int coolantAgeDays;
        public List<string> history = new List<string>();
    }

    [Serializable]
    public class BoardRepairState
    {
        public bool esdGrounded;
        public bool microscopeInspected;
        public bool powerRailMeasured;
        public float measuredRailV;
        public bool shortLocated;
        public bool fluxApplied;
        public float solderQuality;
        public int reworkCycles;
        public bool padsIntact = true;
        public bool repaired;
        public string lastMeasurement;
        public List<string> history = new List<string>();
    }

    [Serializable]
    public class PortableDeviceState
    {
        public int screwsRemaining = 8;
        public bool backCoverRemoved;
        public bool batteryDisconnected;
        public float batteryHealth = .82f;
        public float chargingPortHealth = .80f;
        public float displayHealth = .90f;
        public float adhesiveIntegrity = 1f;
        public float waterDamage;
        public bool displaySeparated;
        public bool sealed = true;
        public float sealQuality = 1f;
        public float controllerDrift;
        public List<string> history = new List<string>();
    }

    [Serializable]
    public class NetworkLabState
    {
        public bool linkUp;
        public bool dhcp = true;
        public string ipAddress = "192.168.1.10";
        public string subnetMask = "255.255.255.0";
        public float throughputMbps;
        public float packetLoss;
        public int latencyMs;
        public int raidLevel = 1;
        public int disksTotal = 2;
        public int disksHealthy = 2;
        public bool arrayDegraded;
        public bool scrubComplete;
        public List<string> history = new List<string>();
    }

    [Serializable]
    public class OsRuntimeState
    {
        public string filesystem = "ForgeFS";
        public int partitionCount;
        public int freeStorageGB;
        public bool systemFilesHealthy = true;
        public int driverRevision;
        public int pendingUpdates = 3;
        public bool firewallEnabled = true;
        public bool networkStackReady;
        public int crashCount;
        public string lastCrashCode;
        public List<string> history = new List<string>();
    }

    [Serializable]
    public class BenchmarkRunState
    {
        public float cpuScore;
        public float gpuScore;
        public float memoryScore;
        public float storageScore;
        public float totalScore;
        public int stressMinutes;
        public int memoryErrors;
        public float peakCpuC;
        public float peakGpuC;
        public float peakPowerW;
        public float minimum12V = 12f;
        public float maxRippleMv;
        public BenchmarkStatus status;
        public string lastResult;
        public List<string> history = new List<string>();
    }

    [Serializable]
    public class ThermalRuntimeState
    {
        public float ambientC = 23f;
        public float caseAirC = 26f;
        public float vrmC = 35f;
        public float storageC = 32f;
        public float coolantC = 25f;
        public float airflowCfm;
        public float pressureBalance;
        public bool cpuThrottling;
        public bool gpuThrottling;
        public int intakeFans;
        public int exhaustFans;
        public List<string> history = new List<string>();
    }

    [Serializable]
    public class PowerRuntimeState
    {
        public float rail12V = 12f;
        public float rail5V = 5f;
        public float rail33V = 3.3f;
        public float rippleMv;
        public float efficiency = .86f;
        public float headroomW;
        public float transientPeakW;
        public bool ocpTriggered;
        public bool stable = true;
        public List<string> history = new List<string>();
    }

    [Serializable]
    public class MaintenanceState
    {
        public float filterDust;
        public int thermalPasteAgeDays;
        public int serviceAgeDays;
        public float corrosion;
        public bool esdIncident;
        public int cleaningCycles;
        public List<string> history = new List<string>();
    }

    [Serializable]
    public class MachineState
    {
        public string machineId;
        public string displayName;
        public DeviceCategory category = DeviceCategory.Desktop;
        public string ownerJobId;
        public string caseItemId;
        public string motherboardItemId;
        public string cpuItemId;
        public List<string> ramItemIds = new List<string>();
        public List<int> ramSlotIndices = new List<int>();
        public string gpuItemId;
        public List<string> storageItemIds = new List<string>();
        public string psuItemId;
        public string coolerItemId;
        public List<string> fanItemIds = new List<string>();
        public CableState cables = new CableState();
        public BiosState bios = new BiosState();
        public BootState bootState = BootState.Off;
        public string postCode = "OFF";
        public bool osInstalled;
        public bool driversInstalled;
        public bool partitioned;
        public bool activated;
        public bool sidePanelInstalled;
        public PanelState sidePanel = new PanelState();
        public CustomizationState customization = new CustomizationState();
        public LiquidLoopState liquidLoop = new LiquidLoopState();
        public BoardRepairState boardRepair = new BoardRepairState();
        public PortableDeviceState portable = new PortableDeviceState();
        public NetworkLabState network = new NetworkLabState();
        public OsRuntimeState osState = new OsRuntimeState();
        public BenchmarkRunState benchmarkState = new BenchmarkRunState();
        public ThermalRuntimeState thermalState = new ThermalRuntimeState();
        public PowerRuntimeState powerState = new PowerRuntimeState();
        public MaintenanceState maintenance = new MaintenanceState();
        public bool thermalPasteApplied;
        public float thermalPasteQuality;
        public float dust;
        public float cableManagementScore = 0.5f;
        public float aestheticScore = 0.5f;
        public float cpuTempC;
        public float gpuTempC;
        public float systemPowerW;
        public float benchmarkScore;
        public float noiseDb;
        public bool stressStable;
        public string lastDiagnostic;
        public List<string> history = new List<string>();
    }

    [Serializable]
    public class JobState
    {
        public string jobId;
        public string customerName;
        public JobType type;
        public JobStage stage;
        public DeviceCategory deviceCategory;
        public string machineId;
        public string title;
        public string description;
        public float reward;
        public float budget;
        public int dueDay;
        public int targetBenchmark;
        public float maxNoiseDb;
        public bool requireOs;
        public bool requireDrivers;
        public bool requireClean;
        public bool requireStable;
        public bool requireNoFaults;
        public bool hiddenPreferenceLowNoise;
        public List<string> requiredPartCategories = new List<string>();
        public List<string> optionalObjectives = new List<string>();
        public string completionMessage;
    }

    [Serializable]
    public class ShipmentLine
    {
        public string definitionId;
        public int quantity;
        public float unitPrice;
    }

    [Serializable]
    public class ShipmentState
    {
        public string shipmentId;
        public int orderedDay;
        public int deliveryDay;
        public int speedTier;
        public ShipmentStatus status;
        public List<ShipmentLine> lines = new List<ShipmentLine>();
        public bool delayed;
        public bool wrongItemEvent;
    }

    [Serializable]
    public class LedgerEntry
    {
        public int day;
        public string reason;
        public float amount;
        public float balanceAfter;
    }

    [Serializable]
    public class WorkshopState
    {
        public int level = 1;
        public int benchLevel = 1;
        public int storageLevel = 1;
        public int diagnosticsLevel = 1;
        public int boardRepairLevel;
        public bool deliveryAutomation;
        public int helperStaff;
        public int specialistStaff;
        public float cleanliness = 0.9f;
    }

    [Serializable]
    public class AccessibilitySettings
    {
        public string language = "uk";
        public float textScale = 1f;
        public bool reducedMotion;
        public bool haptics = true;
        public bool captions = true;
        public int colorblindMode;
        public float musicVolume = 0.6f;
        public float sfxVolume = 0.8f;
        public int fpsLimit = 60;
        public bool batterySaver;
        public float lookSensitivity = 1f;
        public float joystickSize = 1f;
    }

    [Serializable]
    public class GameState
    {
        public int schemaVersion = 7;
        public string saveId;
        public int day = 1;
        public float money = 1800f;
        public int reputation;
        public int experience;
        public int nextJobSerial = 1;
        public int nextItemSerial = 1;
        public int nextShipmentSerial = 1;
        public bool tutorialEnabled = true;
        public int tutorialStep;
        public WorkshopState workshop = new WorkshopState();
        public AccessibilitySettings settings = new AccessibilitySettings();
        public List<ItemInstance> inventory = new List<ItemInstance>();
        public List<MachineState> machines = new List<MachineState>();
        public List<JobState> jobs = new List<JobState>();
        public List<ShipmentState> shipments = new List<ShipmentState>();
        public List<LedgerEntry> ledger = new List<LedgerEntry>();
        public List<string> unlockedPartIds = new List<string>();
        public List<string> milestones = new List<string>();
    }

    public struct ActionResult
    {
        public bool ok;
        public string message;
        public ActionResult(bool success, string text) { ok = success; message = text; }
        public static ActionResult Success(string message) => new ActionResult(true, message);
        public static ActionResult Fail(string message) => new ActionResult(false, message);
    }
}
