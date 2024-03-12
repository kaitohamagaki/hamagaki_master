using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class move : MonoBehaviour
{
    Process pr = null;
    int label = 2;

    public float moveSpeed = 2f;
    public float jumpForce = 12f;
    private Rigidbody2D rb;
    private bool isGrounded;  // �n�ʂɐڒn���Ă��邩�ǂ���

    private int jumpCount = 0;  // �W�����v����̃J�E���^
    private int requiredJumps = 3;  // �W�����v�����s���邽�߂ɕK�v�ȘA�����萔

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ExecutePythonScript();
        UnityEngine.Debug.Log("Some debug message");  // ���s���J�n���ꂽ���Ƃ����O�ɏo��
    }

    public void ExecutePythonScript()
    {
        pr = new Process();

        pr.StartInfo.FileName = @"C:\Users\kabot\AppData\Local\Programs\Python\Python310\python.exe";
        pr.StartInfo.Arguments = @" -u F:\MTG\personal\ooyoshi\realtime.py"; //�l�ɉ����ăp�X��ύX

        pr.StartInfo.CreateNoWindow = true;
        pr.StartInfo.UseShellExecute = false;

        // �W���o�͂ƕW���G���[�o�͂����_�C���N�g����
        pr.StartInfo.RedirectStandardOutput = true;
        pr.StartInfo.RedirectStandardError = true;

        pr.OutputDataReceived += process_DataReceived;
        pr.ErrorDataReceived += process_ErrorReceived;  // �G���[�o�͂̃C�x���g�n���h����ǉ�

        pr.EnableRaisingEvents = true;

        pr.Start();

        pr.BeginOutputReadLine();  // �W���o�͂̓ǂݎ����J�n
        pr.BeginErrorReadLine();   // �W���G���[�o�͂̓ǂݎ����J�n
    }

    public void process_DataReceived(object sender, DataReceivedEventArgs e)
    {
        string output = e.Data;
        if (!string.IsNullOrEmpty(output)) // ��łȂ����Ƃ��m�F
        {
            // �󂯎�����f�[�^�����O�ɏo��
            UnityEngine.Debug.Log("Output: " + output);

            if (output.Equals("walk"))
            {
                // Movement activated ���b�Z�[�W�����O�ɏo��
                UnityEngine.Debug.Log("Movement activated");
                label = 0;
            }
            else if (output.Equals("jump"))
            {
                // Jump activated ���b�Z�[�W�����O�ɏo��
                UnityEngine.Debug.Log("Jump activated");
                label = 1;
            }
        }
    }


    private void process_ErrorReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            // �G���[�����������ɋL�q���܂��B
            // ��: �󂯎�����G���[���b�Z�[�W�����O�ɏo��
            UnityEngine.Debug.LogError("Error from script: " + e.Data);
        }
    }

    // �ȉ��͌��̃R�[�h�̑����ł�
    void Update()
    {
        if (label != 1)  // �W�����v�ȊO�̔��肪�s��ꂽ�ꍇ
        {
            jumpCount = 0;  // �W�����v�J�E���g�����Z�b�g
        }
    }

    void FixedUpdate()
    {
        if (label == 0)  // "walk" �Ɣ��f���ꂽ�ꍇ
        {
            // ��ɉE�����Ɉړ�
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }

        if (jumpCount >= requiredJumps && isGrounded)  // �W�����v���肪�A�����čs���A���v���C���[���n�ʂɂ���ꍇ
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            label = 2;  // �W�����v��̓��x�������Z�b�g
            jumpCount = 0;  // �W�����v�J�E���g�����Z�b�g
            isGrounded = false;  // �W�����v����͒n�ʂ��痣���
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
