using UnityEngine;

public class waterMovement: MonoBehaviour {
    public Material waterMat;
    public float scrollSpeed = 0.05f;

    void Update() {
        float offset = Time.time * scrollSpeed;
        waterMat.mainTextureOffset = new Vector2(offset, offset);
    }
}
