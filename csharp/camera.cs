using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform player; // プレーヤーのTransform
    public float smoothSpeed = 0.125f; // カメラの移動の滑らかさを調整
    public float scrollStartOffsetX = 0; // カメラがスクロールを開始するX軸のオフセット
    public Vector3 offset; // カメラとプレーヤーの相対距離を設定

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        Vector3 point = Camera.main.WorldToViewportPoint(player.position); // プレーヤーの位置をビューポート座標に変換
        Vector3 delta = player.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); // 中心からの差分を計算
        Vector3 destination = transform.position + delta; // 目的地を決定

        if (delta.x > scrollStartOffsetX) // プレーヤーが右のオフセットより右にいる場合
        {
            // カメラをスムーズに目的地に移動
            destination.x = player.position.x - scrollStartOffsetX;
            destination.y = transform.position.y;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, smoothSpeed);
        }
    }
}
