using UnityEngine;
using System.Diagnostics;

public class wasd : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;
    private Rigidbody2D rb;
    private bool isGrounded;
    private Process pythonProcess;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ExecutePythonScript();
    }

    void ExecutePythonScript()
    {
        pythonProcess = new Process();
        pythonProcess.StartInfo.FileName = @"C:\Users\kabot\AppData\Local\Programs\Python\Python310\python.exe"; // Pythonインタープリタへのパス
        pythonProcess.StartInfo.Arguments = @"C:\Users\kabot\ramspeed.py"; // Pythonスクリプトのパス
        pythonProcess.StartInfo.UseShellExecute = false;
        pythonProcess.StartInfo.RedirectStandardOutput = true;
        pythonProcess.StartInfo.CreateNoWindow = true;
        pythonProcess.OutputDataReceived += OnOutputDataReceived;
        pythonProcess.Start();
        pythonProcess.BeginOutputReadLine();
    }

    void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received data: " + e.Data); // この行を追加
        if (!string.IsNullOrEmpty(e.Data))
        {
            try
            {
                //moveSpeed = float.Parse(e.Data);
                UnityEngine.Debug.Log("Move Speed Updated: " + moveSpeed);
            }
            catch (System.FormatException)
            {
                UnityEngine.Debug.LogError("Invalid output received: " + e.Data);
            }
        }
    }

    void OnApplicationQuit()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
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
