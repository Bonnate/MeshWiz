using UnityEngine;
using UnityEngine.UI;

public class MeshSimplifier: Singleton<MeshSimplifier>
{
    [SerializeField] private Slider mSimplifySlider;
    private Mesh mOriginMesh;

    public void Init(MeshFilter meshFilter)
    {
        mSimplifySlider.SetValueWithoutNotify(1.0f);
        mOriginMesh = meshFilter.sharedMesh;
    }

    private void SimplifyMeshFilter(MeshFilter meshFilter)
    {
        Mesh sourceMesh = mOriginMesh;
        if (sourceMesh == null) // verify that the mesh filter actually has a mesh
            return;

        // Create our mesh simplifier and setup our entire mesh in it
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(sourceMesh);

        // This is where the magic happens, lets simplify!
        meshSimplifier.SimplifyMesh(mSimplifySlider.value);

        // Create our final mesh and apply it back to our mesh filter
        meshFilter.sharedMesh = meshSimplifier.ToMesh();
    }

    public void Slider_SimplifyMesh()
    {
        SimplifyMeshFilter(MeshController.Instance.CurrentGo.GetComponent<MeshFilter>());
    }
}