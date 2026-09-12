using System.Linq;
using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class ProgressionTests
    {
        private static HardwareCatalog Catalog(){HardwareCatalog c=new HardwareCatalog();c.Load();return c;}

        [Test]
        public void Level_IncreasesFromExperience()
        {
            GameState state=new GameState();HardwareCatalog catalog=Catalog();ProgressionService p=new ProgressionService(state,catalog);int start=p.Level;state.experience=4000;Assert.Greater(p.Level,start);Assert.GreaterOrEqual(p.EarnedSkillPoints,1);
        }

        [Test]
        public void Unlock_RequiresLevelReputationAndPoint()
        {
            GameState state=new GameState();HardwareCatalog catalog=Catalog();ProgressionService p=new ProgressionService(state,catalog);ActionResult locked=p.Unlock("procurement");Assert.IsFalse(locked.ok);state.experience=4000;state.reputation=500;ActionResult unlocked=p.Unlock("procurement");Assert.IsTrue(unlocked.ok,unlocked.message);Assert.IsTrue(p.IsUnlocked("procurement"));Assert.IsTrue(state.milestones.Contains("SKILL:procurement"));
        }

        [Test]
        public void ApplyPermanentEffects_PersistsCapabilityFloor()
        {
            GameState state=new GameState();HardwareCatalog catalog=Catalog();state.experience=12000;state.reputation=1000;state.milestones.Add("SKILL:diagnostics");state.milestones.Add("SKILL:microsolder");ProgressionService p=new ProgressionService(state,catalog);p.ApplyPermanentEffects();Assert.GreaterOrEqual(state.workshop.diagnosticsLevel,2);Assert.GreaterOrEqual(state.workshop.boardRepairLevel,2);
        }

        [Test]
        public void RefreshPartUnlocks_ExpandsCatalogWithLevel()
        {
            GameState state=new GameState();HardwareCatalog catalog=Catalog();ProgressionService p=new ProgressionService(state,catalog);p.RefreshPartUnlocks();int low=state.unlockedPartIds.Count;state.experience=50000;p.RefreshPartUnlocks();Assert.GreaterOrEqual(state.unlockedPartIds.Count,low);Assert.Greater(state.unlockedPartIds.Count,0);Assert.LessOrEqual(state.unlockedPartIds.Count,catalog.All.Count);
        }

        [Test]
        public void AchievementSnapshot_DerivesFromPersistentState()
        {
            GameState state=new GameState{money=6000f,reputation=300,experience=4000};state.jobs.Add(new JobState{jobId="T1",stage=JobStage.Completed});HardwareCatalog catalog=Catalog();ProgressionService p=new ProgressionService(state,catalog);var a=p.AchievementSnapshot();Assert.IsTrue(a.Any(x=>x.Contains("FIRST REPAIR")));Assert.IsTrue(a.Any(x=>x.Contains("POSITIVE CASHFLOW")));Assert.IsTrue(a.Any(x=>x.Contains("LOCAL FAVORITE")));
        }
    }
}
