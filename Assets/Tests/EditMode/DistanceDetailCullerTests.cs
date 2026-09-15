using NUnit.Framework;
using UnityEngine;

namespace ForgeBench.Tests
{
    public sealed class DistanceDetailCullerTests
    {
        [Test]
        public void LegacyMicroDetail_ReceivesExpectedDistance()
        {
            GameObject go=GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name="PanelScrew_Test";
            try
            {
                Renderer r=go.GetComponent<Renderer>();
                Assert.IsTrue(DistanceDetailCuller.ShouldManage(r));
                Assert.AreEqual(7.5f,DistanceDetailCuller.ResolveDistance(r),.001f);
            }
            finally{Object.DestroyImmediate(go);}
        }

        [Test]
        public void LodGroupRenderer_IsNeverManaged()
        {
            GameObject parent=new GameObject("LODParent");
            GameObject child=GameObject.CreatePrimitive(PrimitiveType.Cube);
            child.name="PanelScrew_LOD";child.transform.SetParent(parent.transform,false);
            parent.AddComponent<LODGroup>();
            try{Assert.IsFalse(DistanceDetailCuller.ShouldManage(child.GetComponent<Renderer>()));}
            finally{Object.DestroyImmediate(parent);}
        }

        [Test]
        public void InteractiveRenderer_IsNeverManaged()
        {
            GameObject parent=new GameObject("Interactive");
            parent.AddComponent<WorldInteractable>();
            GameObject child=GameObject.CreatePrimitive(PrimitiveType.Cube);
            child.name="PanelScrew_Interactive";child.transform.SetParent(parent.transform,false);
            try{Assert.IsFalse(DistanceDetailCuller.ShouldManage(child.GetComponent<Renderer>()));}
            finally{Object.DestroyImmediate(parent);}
        }

        [Test]
        public void ExplicitHint_OverridesLegacyNameDistance()
        {
            GameObject parent=new GameObject("DecorRoot");
            DistanceCullHint hint=parent.AddComponent<DistanceCullHint>();hint.maxDistance=22f;
            GameObject child=GameObject.CreatePrimitive(PrimitiveType.Cube);
            child.name="PanelScrew_Authored";child.transform.SetParent(parent.transform,false);
            try{Assert.AreEqual(22f,DistanceDetailCuller.ResolveDistance(child.GetComponent<Renderer>()),.001f);}
            finally{Object.DestroyImmediate(parent);}
        }

        [Test]
        public void UnclassifiedRenderer_IsNotDistanceCulled()
        {
            GameObject go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name="MainCaseShell";
            try{Assert.AreEqual(0f,DistanceDetailCuller.ResolveDistance(go.GetComponent<Renderer>()),.001f);}
            finally{Object.DestroyImmediate(go);}
        }
    }
}
