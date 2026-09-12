using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Turns the warehouse shelf from decoration into a read-only physical projection of
    /// real inventory instances. Available parts occupy visible bins; their condition and
    /// category affect the visual, and interacting opens the pooled inventory browser.
    /// </summary>
    public sealed class PhysicalInventoryShelves : MonoBehaviour
    {
        private GameRuntime game;
        private GameObject root;
        private Shader shader;
        private readonly List<Material> materials=new List<Material>();
        private string signature=string.Empty;
        private float nextPoll;
        private const int VisibleItems=28;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if(FindAnyObjectByType<PhysicalInventoryShelves>()!=null)return;
            GameObject go=new GameObject("ForgeBench_PhysicalInventoryShelves");DontDestroyOnLoad(go);go.AddComponent<PhysicalInventoryShelves>();
        }

        private IEnumerator Start()
        {
            while(GameRuntime.Instance==null||GameRuntime.Instance.World==null||GameRuntime.Instance.State==null)yield return null;
            game=GameRuntime.Instance;shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");Rebuild();
        }

        private void Update()
        {
            if(Time.unscaledTime<nextPoll)return;nextPoll=Time.unscaledTime+1f;game=GameRuntime.Instance;if(game?.State==null)return;string s=Signature();if(s!=signature)Rebuild();
        }

        private string Signature()
        {
            if(game?.State?.inventory==null)return "none";
            var rows=game.State.inventory.Where(x=>!x.reserved).OrderBy(x=>x.instanceId).Take(VisibleItems).Select(x=>x.instanceId+":"+x.condition.ToString("0.00")+":"+x.fault+":"+x.damage);
            return game.State.inventory.Count(x=>!x.reserved)+"|"+string.Join("|",rows);
        }

        private void Rebuild()
        {
            game=GameRuntime.Instance;if(shader==null)shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");if(root!=null)Destroy(root);foreach(Material m in materials)if(m!=null)Destroy(m);materials.Clear();root=new GameObject("PhysicalWarehouseInventory");signature=Signature();if(game?.State?.inventory==null)return;
            Vector3 origin=new Vector3(-5.40f,.30f,-3.76f);
            Material labelMat=Mat(new Color(.10f,.13f,.16f),.40f,.35f);
            GameObject header=Box("InventoryCount",origin+new Vector3(0,2.12f,-.05f),new Vector3(2.15f,.24f,.12f),labelMat);WorldInteractable hw=header.AddComponent<WorldInteractable>();hw.label="Warehouse · "+game.State.inventory.Count(x=>!x.reserved)+" available part(s) — open inventory";hw.priority=11;hw.maxDistance=3.4f;hw.action=VirtualizedInventoryPanel.Open;
            List<ItemInstance> visible=game.State.inventory.Where(x=>!x.reserved).OrderBy(x=>game.Inventory.Def(x)?.category).ThenBy(x=>game.Inventory.Def(x)?.brand).ThenBy(x=>game.Inventory.Def(x)?.model).Take(VisibleItems).ToList();
            for(int i=0;i<visible.Count;i++)
            {
                ItemInstance item=visible[i];HardwareDefinition d=game.Inventory.Def(item);if(d==null)continue;int shelf=i/7;int col=i%7;float x=-.90f+col*.30f;float y=.20f+shelf*.71f;float z=-.08f+(col%2)*.05f;Color category=CategoryColor(d.category);float health=Mathf.Lerp(.42f,1f,Mathf.Clamp01(item.condition));if(item.fault!=FaultType.None||item.damage!=DamageType.None)category=Color.Lerp(category,new Color(.78f,.10f,.06f),.62f);Material m=Mat(category*health,.36f,.14f);Vector3 scale=ScaleFor(d.category);GameObject box=Box("StockItem_"+item.instanceId,origin+new Vector3(x,y,z),scale,m);WorldInteractable wi=box.AddComponent<WorldInteractable>();wi.label=d.category+" · "+d.brand+" "+d.model+" · condition "+Mathf.RoundToInt(item.condition*100f)+"% — open inventory";wi.priority=12;wi.maxDistance=3.1f;wi.action=VirtualizedInventoryPanel.Open;
            }
            int hidden=Mathf.Max(0,game.State.inventory.Count(x=>!x.reserved)-visible.Count);if(hidden>0){GameObject bin=Box("WarehouseOverflowBin",origin+new Vector3(.72f,2.05f,0),new Vector3(.62f,.28f,.46f),labelMat);WorldInteractable wi=bin.AddComponent<WorldInteractable>();wi.label=hidden+" more warehouse item(s) — open inventory";wi.priority=10;wi.maxDistance=3.3f;wi.action=VirtualizedInventoryPanel.Open;}
        }

        private GameObject Box(string name,Vector3 pos,Vector3 scale,Material material){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(root.transform,true);g.transform.position=pos;g.transform.localScale=scale;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=material;return g;}
        private Material Mat(Color color,float smooth,float metallic){Material m=new Material(shader);m.color=color;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallic);materials.Add(m);return m;}
        private static Vector3 ScaleFor(PartCategory c){switch(c){case PartCategory.Case:return new Vector3(.24f,.30f,.25f);case PartCategory.GPU:return new Vector3(.27f,.10f,.19f);case PartCategory.Motherboard:return new Vector3(.25f,.06f,.24f);case PartCategory.Fan:return new Vector3(.18f,.18f,.08f);case PartCategory.Cooler:return new Vector3(.18f,.24f,.18f);default:return new Vector3(.20f,.15f,.17f);}}
        private static Color CategoryColor(PartCategory c){switch(c){case PartCategory.CPU:return new Color(.34f,.48f,.78f);case PartCategory.GPU:return new Color(.30f,.66f,.38f);case PartCategory.RAM:return new Color(.60f,.36f,.76f);case PartCategory.Storage:return new Color(.25f,.58f,.65f);case PartCategory.PSU:return new Color(.38f,.40f,.44f);case PartCategory.Case:return new Color(.50f,.53f,.56f);case PartCategory.Motherboard:return new Color(.36f,.52f,.35f);case PartCategory.Battery:return new Color(.70f,.54f,.20f);case PartCategory.Display:return new Color(.18f,.47f,.70f);case PartCategory.Tool:return new Color(.72f,.38f,.18f);default:return new Color(.42f,.50f,.58f);}}
        private void OnDestroy(){if(root!=null)Destroy(root);foreach(Material m in materials)if(m!=null)Destroy(m);}
    }
}
