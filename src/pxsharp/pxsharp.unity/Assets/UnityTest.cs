using UnityEngine;
using System.Collections;

public class UnityTest : MonoBehaviour {
    [SerializeField]
    GameObject prefab;

	void Start () {
        for (int x = 0; x < TestUtils.TestRows; ++x) {
            for (int z = 0; z < TestUtils.TestCols; ++z) {
                for (int y = 0; y < TestUtils.TestHeight; ++y) {
                    GameObject.Instantiate(prefab, TestUtils.GetXZOffset(new Vector3(x, y * 1.5f, z)), Quaternion.identity);
                }
            }
        }

	}
}
