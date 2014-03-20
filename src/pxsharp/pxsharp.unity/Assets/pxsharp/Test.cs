using UnityEngine;
using System.Collections;
using PxSharp;
using System;

public static class TestUtils {

    public const int TestRows = 8;
    public const int TestCols = 8;
    public const int TestHeight = 64;

    public static Vector3 GetXZOffset (Vector3 pos) {
        pos.x += UnityEngine.Random.Range(-0.25f, 0.25f);
        pos.z += UnityEngine.Random.Range(-0.25f, 0.25f);
        return pos;
    }
}

public class Test : MonoBehaviour {
    IntPtr errorCallback;
    PxFoundation f;
    PxPhysics p;
    PxScene s;
    PxMaterial m;
    PxRigidDynamic rd;

    GameObject go;

    void Start () {
        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        errorCallback = Pxs.CreateErrorCallback((e, msg) => Debug.Log(e.ToString() + ": " + msg));

        f = Px.CreateFoundation(errorCallback);
        if (!f) f = Px.GetFoundation();
        Debug.Log(f);

        p = Px.CreatePhysics(f, new PxTolerancesScale { Length = 1, Mass = 1000, Speed = 10 });
        Debug.Log(p);

        if (Px.InitExtensions(p)) {
            Debug.Log("Extensions inited");
        } else {
            Debug.LogError("Could not init extensions");
        }

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
        d.NbContactDataBlocks = 4096;
        d.MaxNbContactDataBlocks = 1 << 14;
        d.Flags |= PxSceneFlag.eENABLE_ACTIVETRANSFORMS;
        d.Gravity = new PxVec3 { Y = -9.81f };
        d.CpuDispatcher = Px.DefaultCpuDispatcherCreate(8);

        s = p.CreateScene(d);
        Debug.Log(s);

        m = p.CreateMaterial(0.5f, 0.5f, 0.1f);
        Debug.Log(m);

        using (var plane = PxPlane.New(new PxVec3 { Y = 1 }, 0)) {
            var planeRB = Px.CreatePlane(p, plane, m);
            s.AddActor(planeRB);
        }

        using (var sphere = PxSphereGeometry.New(1f)) {
            rd = Px.CreateDynamic(p, new PxTransform { P = new PxVec3 { Y = 15 }, Q = PxQuat.Identity }, sphere, m, 1, PxTransform.Identity);

			PxRigidbodyExt.SetMassAndUpdateInertia(rd, 1f, PxVec3.Zero);

            s.AddActor(rd);
        }
    }

    PxActiveTransform[] transforms = new PxActiveTransform[5];

    void FixedUpdate () {
        s.Simulate(Time.fixedDeltaTime);

        uint errorState = 0;

        if (s.FetchResults(true, ref errorState)) {
            uint count = s.GetActiveTransforms(transforms);

            for (uint i = 0; i < count; ++i) {
                PxVec3 pos = transforms[i].Actor2World.P;
                go.transform.position = new Vector3(pos.X, pos.Y, pos.Z);
            }
        }
    }
}
