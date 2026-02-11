using System;
using System.Windows.Forms;

public static class UiLogger
{
    private static TextBox _textBox;
    private static Control _invoker;

    public static void Initialize(TextBox textBox)
    {
        _textBox = textBox;
        _invoker = textBox;
    }

    public static void Log(string message)
    {
        if (_textBox == null)
            return;

        if (_invoker.InvokeRequired)
        {
            _invoker.Invoke(new Action(() => Write(message)));
        }
        else
        {
            Write(message);
        }
    }

    private static void Write(string message)
    {
        _textBox.AppendText(
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}"
        );
        _textBox.SelectionStart = _textBox.Text.Length;
        _textBox.ScrollToCaret();
    }
}
