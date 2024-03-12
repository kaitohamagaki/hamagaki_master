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
    private bool isGrounded;  // 地面に接地しているかどうか

    private int jumpCount = 0;  // ジャンプ判定のカウンタ
    private int requiredJumps = 3;  // ジャンプを実行するために必要な連続判定数

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ExecutePythonScript();
        UnityEngine.Debug.Log("Some debug message");  // 実行が開始されたことをログに出力
    }

    public void ExecutePythonScript()
    {
        pr = new Process();

        pr.StartInfo.FileName = @"C:\Users\kabot\AppData\Local\Programs\Python\Python310\python.exe";
        pr.StartInfo.Arguments = @" -u F:\MTG\personal\ooyoshi\realtime.py"; //人に応じてパスを変更

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
            // 受け取ったデータをログに出力
            UnityEngine.Debug.Log("Output: " + output);

            if (output.Equals("walk"))
            {
                // Movement activated メッセージをログに出力
                UnityEngine.Debug.Log("Movement activated");
                label = 0;
            }
            else if (output.Equals("jump"))
            {
                // Jump activated メッセージをログに出力
                UnityEngine.Debug.Log("Jump activated");
                label = 1;
            }
        }
    }


    private void process_ErrorReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            // エラー処理をここに記述します。
            // 例: 受け取ったエラーメッセージをログに出力
            UnityEngine.Debug.LogError("Error from script: " + e.Data);
        }
    }

    // 以下は元のコードの続きです
    void Update()
    {
        if (label != 1)  // ジャンプ以外の判定が行われた場合
        {
            jumpCount = 0;  // ジャンプカウントをリセット
        }
    }

    void FixedUpdate()
    {
        if (label == 0)  // "walk" と判断された場合
        {
            // 常に右方向に移動
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }

        if (jumpCount >= requiredJumps && isGrounded)  // ジャンプ判定が連続して行われ、かつプレイヤーが地面にいる場合
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            label = 2;  // ジャンプ後はラベルをリセット
            jumpCount = 0;  // ジャンプカウントをリセット
            isGrounded = false;  // ジャンプ直後は地面から離れる
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
