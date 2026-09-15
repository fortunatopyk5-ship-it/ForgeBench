#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using ForgeBench;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ForgeBench.EditorTools
{
    /// <summary>Fails a player build instead of silently producing an incomplete ForgeBench APK.</summary>
    public sealed class ProductionBuildGate : IPreprocessBuildWithReport
    {
        public int callbackOrder => -9000;
        private static readonly string[] CriticalFiles =
        {
            "Assets/Scenes/Workshop.unity","Assets/Resources/Data/hardware.json","Assets/Resources/Localization/en.json","Assets/Resources/Localization/uk.json","Assets/link.xml",
            "Assets/Scripts/Production/ProductionBootstrap.cs","Assets/Scripts/Production/MainMenuController.cs","Assets/Scripts/Production/WorkshopProductionLayer.cs","Assets/Scripts/Production/PhysicalAssemblyController.cs","Assets/Scripts/Production/HardwarePresentationLayout.cs","Assets/Scripts/Production/AssemblyInteractionRules.cs","Assets/Scripts/Production/ObjectHandlingController.cs","Assets/Scripts/Production/MobileInteractionBridge.cs","Assets/Scripts/Production/RuntimeQualityController.cs",
            "Assets/Scripts/Production/SpecialistRepairPanel.cs","Assets/Scripts/Production/SpecialistStationsLayer.cs","Assets/Scripts/Production/SpecialistContractBoard.cs","Assets/Scripts/Production/SpecialistContractTerminalLayer.cs","Assets/Scripts/Production/SpecialistLifecycle.cs","Assets/Scripts/Production/SpecialistDeviceVisuals.cs",
            "Assets/Scripts/Production/EngineeringDiagnosticsPanel.cs","Assets/Scripts/Production/EngineeringTerminalLayer.cs","Assets/Scripts/Production/AdvancedDiagnosticWorkflowPanel.cs","Assets/Scripts/Production/AdvancedDiagnosticTerminalLayer.cs","Assets/Scripts/Production/FitmentPlannerPanel.cs","Assets/Scripts/Production/FitmentPlannerTerminalLayer.cs","Assets/Scripts/Production/JobDispatchPanel.cs","Assets/Scripts/Production/JobDispatchTerminalLayer.cs","Assets/Scripts/Production/JobDispatchLifecycle.cs","Assets/Scripts/Production/WorkshopBusinessPanel.cs","Assets/Scripts/Production/WorkshopManagementTerminalLayer.cs","Assets/Scripts/Production/WorkshopBusinessLifecycle.cs","Assets/Scripts/Production/SupplyChainPanel.cs","Assets/Scripts/Production/SupplyChainTerminalLayer.cs","Assets/Scripts/Production/AdvancedJobLifecycle.cs",
            "Assets/Scripts/Production/CustomerRelationsDirector.cs","Assets/Scripts/Production/CustomerRelationsPanel.cs","Assets/Scripts/Production/CustomerRelationsTerminalLayer.cs","Assets/Scripts/Production/ProgressionDirector.cs","Assets/Scripts/Production/ProgressionPanel.cs","Assets/Scripts/Production/ProgressionTerminalLayer.cs","Assets/Scripts/Production/WorkshopExpansionLayer.cs",
            "Assets/Scripts/Production/SensoryFeedbackDirector.cs","Assets/Scripts/Production/MobilePlatformController.cs","Assets/Scripts/Production/RuntimeLocalizationDirector.cs","Assets/Scripts/Production/AccessibilityRuntimeDirector.cs","Assets/Scripts/Production/WorkshopAtmosphereDirector.cs","Assets/Scripts/Production/DistanceDetailCuller.cs",
            "Assets/Scripts/Production/VirtualizedInventoryPanel.cs","Assets/Scripts/Production/InventoryTerminalLayer.cs","Assets/Scripts/Production/ReliabilityLifecycle.cs","Assets/Scripts/Production/MaintenancePanel.cs","Assets/Scripts/Production/MaintenanceTerminalLayer.cs","Assets/Scripts/Production/PreflightInspectionPanel.cs","Assets/Scripts/Production/PreflightInspectionTerminalLayer.cs",
            "Assets/Scripts/Production/StaffRosterDirector.cs","Assets/Scripts/Production/StaffRosterPanel.cs","Assets/Scripts/Production/StaffRosterTerminalLayer.cs",
            "Assets/Scripts/Production/WarrantyDirector.cs","Assets/Scripts/Production/WarrantyPanel.cs","Assets/Scripts/Production/WarrantyTerminalLayer.cs","Assets/Scripts/Production/WarrantySpecialistBridge.cs","Assets/Scripts/Production/OperationsDashboardPanel.cs","Assets/Scripts/Production/OperationsTerminalLayer.cs",
            "Assets/Scripts/Production/CrashTelemetryLogger.cs","Assets/Scripts/Production/TutorialDirector.cs","Assets/Scripts/Production/InGameManualPanel.cs","Assets/Scripts/Production/ManualTerminalLayer.cs","Assets/Scripts/Production/SaveManagerPanel.cs",
            "Assets/Scripts/Simulation/SpecialistRepairServices.cs","Assets/Scripts/Simulation/SpecialistJobService.cs","Assets/Scripts/Simulation/EngineeringSimulationService.cs","Assets/Scripts/Simulation/AdvancedDiagnosticWorkflowService.cs","Assets/Scripts/Simulation/FitmentPlanningService.cs","Assets/Scripts/Simulation/JobDispatchService.cs","Assets/Scripts/Simulation/WorkshopBusinessService.cs","Assets/Scripts/Simulation/SupplyChainService.cs","Assets/Scripts/Simulation/AdvancedJobGeneratorService.cs","Assets/Scripts/Simulation/CustomerRelationsService.cs","Assets/Scripts/Simulation/ProgressionService.cs","Assets/Scripts/Simulation/ReliabilitySimulationService.cs","Assets/Scripts/Simulation/PreflightInspectionService.cs","Assets/Scripts/Simulation/StaffRosterService.cs","Assets/Scripts/Simulation/WarrantyService.cs",
            "Assets/Scripts/Runtime/SpecialistRuntimeExtensions.cs","Assets/Scripts/Runtime/EngineeringRuntimeExtensions.cs","Assets/Scripts/Runtime/BusinessRuntimeExtensions.cs","Assets/Scripts/Runtime/SupplyChainRuntimeExtensions.cs","Assets/Scripts/Runtime/MaintenanceRuntimeExtensions.cs"
        };

        private static readonly string[] CriticalTests =
        {
            "Assets/Tests/EditMode/EngineeringSimulationTests.cs",
            "Assets/Tests/EditMode/AdvancedDiagnosticWorkflowTests.cs",
            "Assets/Tests/EditMode/FitmentPlanningTests.cs",
            "Assets/Tests/EditMode/HardwarePresentationLayoutTests.cs",
            "Assets/Tests/EditMode/AssemblyInteractionRulesTests.cs",
            "Assets/Tests/EditMode/JobDispatchTests.cs",
            "Assets/Tests/EditMode/ProgressionTests.cs",
            "Assets/Tests/EditMode/ReliabilitySimulationTests.cs",
            "Assets/Tests/EditMode/WarrantyServiceTests.cs",
            "Assets/Tests/EditMode/CrashTelemetryTests.cs"
        };

        public void OnPreprocessBuild(BuildReport report)
        {
            foreach(string path in CriticalFiles)if(!File.Exists(path))throw new BuildFailedException("ForgeBench production build is missing: "+path);
            foreach(string path in CriticalTests)if(!File.Exists(path))throw new BuildFailedException("ForgeBench critical test source is missing: "+path);

            TextAsset hardware=AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/Data/hardware.json");if(hardware==null)throw new BuildFailedException("Hardware database could not be imported.");HardwareCatalogData data=JsonUtility.FromJson<HardwareCatalogData>(hardware.text);
            if(data?.parts==null||data.parts.Count<70)throw new BuildFailedException("Hardware catalog is too small for the production build ("+(data?.parts?.Count??0)+").");if(data.parts.Any(p=>string.IsNullOrWhiteSpace(p.id)||string.IsNullOrWhiteSpace(p.model)))throw new BuildFailedException("Hardware catalog contains an unnamed definition.");if(data.parts.Select(p=>p.id).Distinct().Count()!=data.parts.Count)throw new BuildFailedException("Hardware catalog contains duplicate IDs.");
            PartCategory[] coverage={PartCategory.Case,PartCategory.Motherboard,PartCategory.CPU,PartCategory.RAM,PartCategory.GPU,PartCategory.Storage,PartCategory.PSU,PartCategory.Cooler,PartCategory.Fan,PartCategory.Network,PartCategory.Battery,PartCategory.Display,PartCategory.Controller,PartCategory.Consumable,PartCategory.Tool};foreach(PartCategory c in coverage)if(!data.parts.Any(p=>p.category==c))throw new BuildFailedException("Hardware catalog has no "+c+" definition.");
            if(SaveService.CurrentSchema<7)throw new BuildFailedException("Save schema must include engineering persistent state (schema 7+).");
            TextAsset en=AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/Localization/en.json"),uk=AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/Localization/uk.json");if(en==null||uk==null||en.text.Length<1000||uk.text.Length<1000)throw new BuildFailedException("Production localization catalogs are missing or unexpectedly small.");

            if(report.summary.platform==BuildTarget.Android)
            {
                PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,"com.originalforge.forgebench");
                PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;
                PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);
                PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
                PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;
                PlayerSettings.allowedAutorotateToPortrait=false;
                PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;
                PlayerSettings.allowedAutorotateToLandscapeLeft=true;
                PlayerSettings.allowedAutorotateToLandscapeRight=true;
            }
            Debug.Log("ForgeBench production gate passed: "+data.parts.Count+" hardware definitions; deep simulation/diagnostic-evidence/fitment/multi-job-dispatch/CRM/progression/reliability/warranty/operations/manual/save-recovery/crash-diagnostics/accessibility/mobile/preflight source present; "+CriticalTests.Length+" critical test sources present; schema "+SaveService.CurrentSchema+"; Android settings enforced.");
        }
    }
}
#endif
