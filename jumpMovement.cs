using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class jumpMovement : MonoBehaviour
{
    Process pr = null;
    int label = 2; // ���݂̃A�N�V�������x��
    int jumpCount = 0; // �W�����v���x�����A���Ō��o���ꂽ��

    public float moveSpeed = 3f;
    public float jumpForce = 5f;
    private Rigidbody2D rb;
    bool labelReceived = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ExecutePythonScript();
        UnityEngine.Debug.Log("Start script and ready to receive command");
    }

    public void ExecutePythonScript()
    {
        pr = new Process();

        pr.StartInfo.FileName = @"C:\Users\kabot\AppData\Local\Programs\Python\Python310\python.exe";
        pr.StartInfo.Arguments = @"-u F:\MTG\personal\hamagaki\realtime.py"; // �X�N���v�g�̃p�X

        pr.StartInfo.CreateNoWindow = true;
        pr.StartInfo.UseShellExecute = false;

        // �W���o�͂ƕW���G���[�o�͂����_�C���N�g
        pr.StartInfo.RedirectStandardOutput = true;
        pr.StartInfo.RedirectStandardError = true;

        pr.OutputDataReceived += Process_DataReceived;
        pr.ErrorDataReceived += Process_ErrorReceived;

        pr.EnableRaisingEvents = true;

        pr.Start();

        pr.BeginOutputReadLine();
        pr.BeginErrorReadLine();
    }

    private void Process_DataReceived(object sender, DataReceivedEventArgs e)
    {
        string output = e.Data;
        if (!string.IsNullOrEmpty(output))
        {

            labelReceived = true;
            if (output.Equals("walk"))
            {
                label = 0;
                jumpCount = 0; // �W�����v�J�E���g�����Z�b�g
            }
            else if (output.Equals("jump"))
            {
                label = 1;
                jumpCount++; // �W�����v���x�������o���ꂽ��J�E���g�A�b�v
            }
        }
    }

    private void Process_ErrorReceived(object sender, DataReceivedEventArgs e)
    {
        string errorOutput = e.Data;
        if (!string.IsNullOrEmpty(errorOutput))
        {
            UnityEngine.Debug.LogError("Python Error: " + errorOutput);
        }
    }

    void Update()
    {
        if (label == 0)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
    }

    void FixedUpdate()
    {
        if (labelReceived) // ���x�������Ȃ��Ƃ���x�󂯎��ꂽ�ꍇ�̂ݎ��s
        {
            if (label == 1 && jumpCount >= 9 && Mathf.Abs(rb.velocity.y) < 0.05f)
            {
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                jumpCount = 0; // �W�����v��̓J�E���g�����Z�b�g
            }
            else
            {
                // �W�����v�̏�������������Ȃ��ꍇ�A�v���C���[���E�����Ɉړ�������
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            }
        }

    }
}
