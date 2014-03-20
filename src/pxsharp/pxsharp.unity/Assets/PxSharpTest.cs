using UnityEngine;
using System.Collections;
using PxSharp;
using System;

public class PxSharpTest : MonoBehaviour {
    [SerializeField]
    GameObject prefab;

    Transform[] transforms;
    PxActiveTransform[] activeTransforms;
    PxsScratchBlock sb;

    PxsErrorCallback ec;
    PxFoundation f;
    PxPhysics p;
    PxScene s;
    PxMaterial m;

    IntPtr pvdCn;

    void Start () {
        // error callback
        sb = Pxs.AllocScratchBlock(1024 * 1024 * 64); // 64mb scratch block
        ec = PxsErrorCallback.New((e, msg) => Debug.Log(e.ToString() + ": " + msg));

        // sdk foundation
        f = Px.CreateFoundation(ec);
        if (!f) f = Px.GetFoundation();
        Debug.Log(f);

        // physics object
        p = Px.CreatePhysics(f, new PxTolerancesScale { Length = 1, Mass = 1000, Speed = 10 });
        Debug.Log(p);

        // init all extensions
        if (Px.InitExtensions(p)) Debug.Log("Extensions inited");
        else Debug.LogError("Could not init extensions");

        // create scene descriptor
        PxSceneDesc d = PxSceneDesc.New(p.GetTolerancesScale());
        Debug.Log(d);

        d.Limits = new PxSceneLimits {
            MaxNbActors = 4096,
            MaxNbBodies = 4096,
            MaxNbConstraints = 4096,
            MaxNbDynamicShapes = 4096,
            MaxNbObjectsPerRegion = 4096,
            MaxNbRegions = 256,
            MaxNbStaticShapes = 4096
        };
        d.Flags |= PxSceneFlag.eENABLE_ACTIVETRANSFORMS;
        d.Flags |= PxSceneFlag.eENABLE_PCM;
        d.SolverBatchSize = 128;
        d.BroadPhaseType = PxBroadPhaseType.eMBP;
        d.NbContactDataBlocks = 1024;
        d.MaxNbContactDataBlocks = 1 << 16;
        d.Gravity = new PxVec3 { Y = -9.81f };
        d.CpuDispatcher = Px.DefaultCpuDispatcherCreate(4);

        // create scene
        s = p.CreateScene(d);
        Debug.Log(s);

        var pvcConMgr = p.GetPvdConnectionManager();
        Debug.Log("pvcConMgr: " + pvcConMgr);

        pvdCn = PxVisualDebuggerExt.CreateConnection0(p.GetPvdConnectionManager(), "127.0.0.1", 5425, 1000, PxVisualDebuggerExt.GetAllConnectionFlags());
        Debug.Log("pvdCn: " + pvdCn);

        Debug.LogWarning(s.AddBroadPhaseRegion(new PxBroadPhaseRegion { Bounds = new PxBounds3() { Minimum = new PxVec3 { X = -1024, Y = -1024, Z = -1024 }, Maximum = new PxVec3 { X = 1024, Y = 1024, Z = 1024 } }, UserData = IntPtr.Zero }, true));

        // create material
        m = p.CreateMaterial(0.5f, 0.5f, 0.1f);
        Debug.Log(m);

        // create floor plane
        using (var plane = PxBoxGeometry.New(1024, 0.25f, 1024)) {
            s.AddActor(Px.CreateStatic(p, new PxTransform { P = new PxVec3 { Y = -0.25f }, Q = PxQuat.Identity }, plane, m, PxTransform.Identity));
        }

        using (var sphere = PxSphereGeometry.New(0.5f)) {
            transforms = new Transform[TestUtils.TestRows * TestUtils.TestCols * TestUtils.TestHeight];
            activeTransforms = new PxActiveTransform[transforms.Length];

            int t = 0;

            for (int x = 0; x < TestUtils.TestRows; ++x) {
                for (int z = 0; z < TestUtils.TestCols; ++z) {
                    for (int y = 0; y < TestUtils.TestHeight; ++y) {
                        GameObject go = GameObject.Instantiate(prefab, TestUtils.GetXZOffset(new Vector3(x, y * 1.5f, z)), Quaternion.identity) as GameObject;
                        transforms[t] = go.transform;

                        Vector3 pos = TestUtils.GetXZOffset(new Vector3(x, y, z));
                        PxRigidDynamic rd = Px.CreateDynamic(p, new PxTransform { P = new PxVec3 { X = pos.x, Y = pos.y, Z = pos.z }, Q = PxQuat.Identity }, sphere, m, 1, PxTransform.Identity);
                        PxRigidbodyExt.SetMassAndUpdateInertia(rd, 1, PxVec3.Zero);

                        rd.UserData = new IntPtr(t);

                        s.AddActor(rd);

                        t += 1;
                    }
                }
            }
        }
    }

    void FixedUpdate () {
        s.Simulate(Time.fixedDeltaTime, sb);
        uint errorState = 0;

        if (s.FetchResults(true, ref errorState)) {
            uint count = s.GetActiveTransforms(activeTransforms);

            for (int i = 0; i < count; ++i) {
                PxTransform pt = activeTransforms[i].Actor2World;
                Transform t = transforms[activeTransforms[i].UserData.ToInt32()];
                t.position = new Vector3(pt.P.X, pt.P.Y, pt.P.Z);
                t.rotation = new Quaternion(pt.Q.X, pt.Q.Y, pt.Q.Z, pt.Q.W);
            }
        }
    }
}
