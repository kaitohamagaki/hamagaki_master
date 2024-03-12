using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class jumpMovement : MonoBehaviour
{
    Process pr = null;
    int label = 2; // 現在のアクションラベル
    int jumpCount = 0; // ジャンプラベルが連続で検出された回数

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
        pr.StartInfo.Arguments = @"-u F:\MTG\personal\hamagaki\realtime.py"; // スクリプトのパス

        pr.StartInfo.CreateNoWindow = true;
        pr.StartInfo.UseShellExecute = false;

        // 標準出力と標準エラー出力をリダイレクト
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
                jumpCount = 0; // ジャンプカウントをリセット
            }
            else if (output.Equals("jump"))
            {
                label = 1;
                jumpCount++; // ジャンプラベルが検出されたらカウントアップ
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
        if (labelReceived) // ラベルが少なくとも一度受け取られた場合のみ実行
        {
            if (label == 1 && jumpCount >= 9 && Mathf.Abs(rb.velocity.y) < 0.05f)
            {
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                jumpCount = 0; // ジャンプ後はカウントをリセット
            }
            else
            {
                // ジャンプの条件が満たされない場合、プレイヤーを右方向に移動させる
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            }
        }

    }
}
