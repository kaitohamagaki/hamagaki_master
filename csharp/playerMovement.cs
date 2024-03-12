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
        UnityEngine.Debug.Log("Some debug message");        // 実行が開始されたことをログに出力
    }


    public void ExecutePythonScript()
    {
        pr = new Process();

        pr.StartInfo.FileName = @"C:\Users\kabot\AppData\Local\Programs\Python\Python310\python.exe";
        pr.StartInfo.Arguments = @" -u F:\MTG\personal\hamagaki\realtime.py"; //人に応じてパスを変更



        pr.StartInfo.CreateNoWindow = true;
        pr.StartInfo.UseShellExecute = false;

        // 標準出力と標準エラー出力をリダイレクトする
        pr.StartInfo.RedirectStandardOutput = true;
        pr.StartInfo.RedirectStandardError = true;

        pr.OutputDataReceived += process_DataReceived;
        pr.ErrorDataReceived += process_ErrorReceived;  // エラー出力のイベントハンドラを追加

        pr.EnableRaisingEvents = true;

        pr.Start();

        pr.BeginOutputReadLine();  // 標準出力の読み取りを開始
        pr.BeginErrorReadLine();   // 標準エラー出力の読み取りを開始
    }

    public void process_DataReceived(object sender, DataReceivedEventArgs e)
    {
        string output = e.Data;
        if (!string.IsNullOrEmpty(output)) // 空でないことを確認
        {
            // print("Output: " + output); // 受け取ったデータをログに出力
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

    // 追加したエラー出力のイベントハンドラ
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
        if (label == 0) // "walk" と判断された場合
        {
            // 常に右方向に移動
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }

    }
    void FixedUpdate()
    {
        if (label == 1 && Mathf.Abs(rb.velocity.y) < 0.05f) // 0.001f から 0.05f に変更
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

}


