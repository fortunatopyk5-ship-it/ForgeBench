using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Persistent panel/fastener state. Screws are captive so a dropped screw cannot soft-lock a job.</summary>
    public sealed class AssemblyService
    {
        private readonly InventoryService inventory;
        public AssemblyService(InventoryService i) { inventory = i; }

        public void EnsureCaseHardware(MachineState m)
        {
            if (m == null) return;
            if (m.sidePanel == null) m.sidePanel = new PanelState();
            if (m.sidePanel.fasteners == null) m.sidePanel.fasteners = new List<FastenerState>();
            if (m.sidePanel.fasteners.Count == 0)
            {
                for (int i = 0; i < 4; i++)
                    m.sidePanel.fasteners.Add(new FastenerState
                    {
                        fastenerId = "SP-" + (i + 1),
                        type = i < 2 ? "Thumbscrew" : "Phillips",
                        tightness = 1f,
                        captive = true
                    });
            }
            m.sidePanel.installed = m.sidePanelInstalled;
        }

        private bool HasDriver()
        {
            return inventory.Available(PartCategory.Tool).Any(i => inventory.Def(i)?.tags.Contains("driver") == true);
        }

        public ActionResult LoosenNext(MachineState m)
        {
            EnsureCaseHardware(m);
            if (m == null || string.IsNullOrEmpty(m.caseItemId)) return ActionResult.Fail("Install a case first.");
            if (!m.sidePanelInstalled) return ActionResult.Fail("The side panel is already removed.");
            if (!HasDriver()) return ActionResult.Fail("A compatible screwdriver is required.");
            FastenerState f = m.sidePanel.fasteners.OrderByDescending(x => x.tightness).FirstOrDefault(x => x.tightness > .02f);
            if (f == null) return ActionResult.Success("All side-panel fasteners are loose.");
            f.tightness = Mathf.Max(0f, f.tightness - .5f);
            m.history.Add("Loosened " + f.fastenerId + " (" + f.type + ") to " + Mathf.RoundToInt(f.tightness * 100f) + "%");
            return ActionResult.Success(f.fastenerId + " loosened. Captive screw retained by the panel.");
        }

        public ActionResult TightenNext(MachineState m)
        {
            EnsureCaseHardware(m);
            if (m == null || !m.sidePanelInstalled) return ActionResult.Fail("Install the side panel before tightening it.");
            if (!HasDriver()) return ActionResult.Fail("A compatible screwdriver is required.");
            FastenerState f = m.sidePanel.fasteners.OrderBy(x => x.tightness).FirstOrDefault(x => x.tightness < .99f);
            if (f == null) return ActionResult.Success("All panel fasteners are correctly tightened.");
            f.tightness = Mathf.Min(1f, f.tightness + .5f);
            m.history.Add("Tightened " + f.fastenerId + " to " + Mathf.RoundToInt(f.tightness * 100f) + "%");
            return ActionResult.Success(f.fastenerId + " tightened to controlled torque.");
        }

        public ActionResult TogglePanel(MachineState m)
        {
            EnsureCaseHardware(m);
            if (m == null || string.IsNullOrEmpty(m.caseItemId)) return ActionResult.Fail("Install a case first.");
            if (m.sidePanelInstalled)
            {
                FastenerState blocked = m.sidePanel.fasteners.FirstOrDefault(x => x.tightness > .05f);
                if (blocked != null) return ActionResult.Fail("Loosen all side-panel fasteners first. " + blocked.fastenerId + " is still tight.");
                m.sidePanelInstalled = false; m.sidePanel.installed = false;
                m.history.Add("Side panel removed");
                return ActionResult.Success("Side panel removed; internal slots are accessible.");
            }
            m.sidePanelInstalled = true; m.sidePanel.installed = true;
            foreach (FastenerState f in m.sidePanel.fasteners) f.tightness = Mathf.Max(f.tightness, .05f);
            m.history.Add("Side panel fitted");
            return ActionResult.Success("Side panel fitted. Tighten the captive fasteners before delivery.");
        }

        public bool InternalsAccessible(MachineState m) => m != null && (!m.sidePanelInstalled || string.IsNullOrEmpty(m.caseItemId));
        public float TighteningQuality(MachineState m)
        {
            EnsureCaseHardware(m);
            if (m == null || m.sidePanel.fasteners.Count == 0) return 1f;
            return Mathf.Clamp01(m.sidePanel.fasteners.Average(x => 1f - Mathf.Abs(1f - x.tightness)));
        }
    }

    /// <summary>Cosmetic state is intentionally isolated from performance scoring.</summary>
    public sealed class CustomizationService
    {
        private readonly InventoryService inventory;
        private static readonly Color[] Presets =
        {
            new Color(.08f,.65f,1f), new Color(.75f,.18f,1f), new Color(1f,.24f,.10f),
            new Color(.16f,1f,.46f), new Color(1f,.82f,.15f), new Color(.92f,.92f,.96f)
        };
        public CustomizationService(InventoryService i) { inventory = i; }
        public Color CurrentColor(MachineState m) => m == null || m.customization == null ? Presets[0] : new Color(m.customization.themeR,m.customization.themeG,m.customization.themeB,1f);
        public bool HasRgbDevice(MachineState m)
        {
            if (m == null) return false;
            IEnumerable<string> ids = m.fanItemIds.Concat(new[]{m.caseItemId,m.coolerItemId,m.gpuItemId});
            return ids.Select(inventory.Get).Select(inventory.Def).Any(d => d != null && d.tags.Contains("rgb"));
        }
        public ActionResult CyclePreset(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No device on the bench.");
            if (m.customization == null) m.customization = new CustomizationState();
            Color current=CurrentColor(m); int closest=0; float best=float.MaxValue;
            for(int i=0;i<Presets.Length;i++){float dr=Presets[i].r-current.r,dg=Presets[i].g-current.g,db=Presets[i].b-current.b;float d=dr*dr+dg*dg+db*db;if(d<best){best=d;closest=i;}}
            Color c=Presets[(closest+1)%Presets.Length]; return SetColor(m,c);
        }
        public ActionResult SetColor(MachineState m, Color c)
        {
            if (m == null) return ActionResult.Fail("No device on the bench.");
            if (m.customization == null) m.customization = new CustomizationState();
            m.customization.themeR=Mathf.Clamp01(c.r);m.customization.themeG=Mathf.Clamp01(c.g);m.customization.themeB=Mathf.Clamp01(c.b);
            RecalculateAesthetics(m); m.history.Add("Build theme color changed");
            return ActionResult.Success("Build theme color updated. Performance is unchanged.");
        }
        public ActionResult CycleEffect(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No device on the bench.");
            if (m.customization == null) m.customization = new CustomizationState();
            m.customization.rgbEffect=(m.customization.rgbEffect+1)%3; RecalculateAesthetics(m);
            string[] names={"Static","Breathing","Spectrum cycle"}; return ActionResult.Success("RGB effect: "+names[m.customization.rgbEffect]+".");
        }
        public ActionResult CycleCableColor(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No device on the bench.");
            if (m.customization == null) m.customization = new CustomizationState();
            m.customization.cableColorIndex=(m.customization.cableColorIndex+1)%5; RecalculateAesthetics(m);
            return ActionResult.Success("Cable color set #"+(m.customization.cableColorIndex+1)+".");
        }
        public void RecalculateAesthetics(MachineState m)
        {
            if (m == null) return;
            float rgb=HasRgbDevice(m) ? .12f : 0f;
            // The calculation is cosmetic only; BenchmarkService never reads aestheticScore/customization.
            m.aestheticScore=Mathf.Clamp01(.5f+rgb+(m.customization != null && m.customization.rgbSync ? .08f : 0f)+m.cableManagementScore*.18f);
        }
    }
}
