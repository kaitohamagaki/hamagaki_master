using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    Process pr = null;
    int label = 2;

    public float moveSpeed = 2f;
    public float jumpForce = 12f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ExecutePythonScript();
        UnityEngine.Debug.Log("Some debug message");        // ���s���J�n���ꂽ���Ƃ����O�ɏo��
    }


    public void ExecutePythonScript()
    {
        pr = new Process();

        pr.StartInfo.FileName = @"C:\Users\kabot\AppData\Local\Programs\Python\Python310\python.exe";
        pr.StartInfo.Arguments = @" -u F:\MTG\personal\hamagaki\realtime.py"; //�l�ɉ����ăp�X��ύX



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
            // print("Output: " + output); // �󂯎�����f�[�^�����O�ɏo��
            if (output.Equals("walk"))
            {
                // print("Movement activated");
                label = 0;
            }
            if (output.Equals("jump"))
            {
                // print("Jump activated");
                label = 1;
            }
        }
    }

    // �ǉ������G���[�o�͂̃C�x���g�n���h��
    public void process_ErrorReceived(object sender, DataReceivedEventArgs e)
    {
        string errorOutput = e.Data;

        if (!string.IsNullOrEmpty(errorOutput))
        {
            UnityEngine.Debug.LogError("Python Error: " + errorOutput);
        }
    }

    void Update()
    {
        if (label == 0) // "walk" �Ɣ��f���ꂽ�ꍇ
        {
            // ��ɉE�����Ɉړ�
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }

    }
    void FixedUpdate()
    {
        if (label == 1 && Mathf.Abs(rb.velocity.y) < 0.05f) // 0.001f ���� 0.05f �ɕύX
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

}


