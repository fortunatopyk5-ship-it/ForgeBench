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
        public int schemaVersion = 5;
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
