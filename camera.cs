using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform player; // �v���[���[��Transform
    public float smoothSpeed = 0.125f; // �J�����̈ړ��̊��炩���𒲐�
    public float scrollStartOffsetX = 0; // �J�������X�N���[�����J�n����X���̃I�t�Z�b�g
    public Vector3 offset; // �J�����ƃv���[���[�̑��΋�����ݒ�

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        Vector3 point = Camera.main.WorldToViewportPoint(player.position); // �v���[���[�̈ʒu���r���[�|�[�g���W�ɕϊ�
        Vector3 delta = player.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); // ���S����̍������v�Z
        Vector3 destination = transform.position + delta; // �ړI�n������

        if (delta.x > scrollStartOffsetX) // �v���[���[���E�̃I�t�Z�b�g���E�ɂ���ꍇ
        {
            // �J�������X���[�Y�ɖړI�n�Ɉړ�
            destination.x = player.position.x - scrollStartOffsetX;
            destination.y = transform.position.y;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, smoothSpeed);
        }
    }
}
