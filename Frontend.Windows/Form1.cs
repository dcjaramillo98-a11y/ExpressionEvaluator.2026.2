using Backend;
using System.Globalization;

namespace Frontend.Windows;

public partial class Form1 : Form
{
    private readonly TextBox display = new();
    private readonly Label resultLabel = new();

    public Form1()
    {
        InitializeComponent();
        BuildCalculator();
    }

    private void BuildCalculator()
    {
        Text = "Functions Evaluator";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(520, 520);
        MinimumSize = new Size(430, 480);
        BackColor = Color.Black;
        Font = new Font("Segoe UI", 12F, FontStyle.Regular);

        display.ReadOnly = true;
        display.Dock = DockStyle.Fill;
        display.Font = new Font("Segoe UI", 22F, FontStyle.Regular);
        display.BackColor = Color.DarkGreen;
        display.ForeColor = Color.White;
        display.TextAlign = HorizontalAlignment.Left;
        display.Margin = new Padding(5);

        resultLabel.Dock = DockStyle.Fill;
        resultLabel.Text = "Ingrese una expresión";
        resultLabel.ForeColor = Color.White;
        resultLabel.Font = new Font("Segoe UI", 10F);
        resultLabel.TextAlign = ContentAlignment.MiddleLeft;
        resultLabel.AutoEllipsis = true;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Black,
            ColumnCount = 6,
            RowCount = 6,
            Padding = new Padding(5)
        };

        layout.ColumnStyles.Clear();
        for (int i = 0; i < 6; i++)
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, i == 5 ? 28F : 14.4F));

        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 17F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 8F));
        for (int i = 0; i < 4; i++)
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 18.75F));

        layout.Controls.Add(display, 0, 0);
        layout.SetColumnSpan(display, 6);
        layout.Controls.Add(resultLabel, 0, 1);
        layout.SetColumnSpan(resultLabel, 6);

        string[,] keys =
        {
            { "7", "8", "9", "(", ")", "Delete" },
            { "4", "5", "6", "*", "/", "Clear" },
            { "1", "2", "3", "+", "-", "^" },
            { "0", "", ".", "", "", "=" }
        };

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 6; col++)
            {
                string key = keys[row, col];
                if (string.IsNullOrEmpty(key))
                    continue;

                var button = new Button
                {
                    Text = key,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(3),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", key.Length > 3 ? 11F : 16F, FontStyle.Regular),
                    BackColor = "0123456789.".Contains(key) ? Color.White : Color.DarkOrange,
                    ForeColor = Color.Black,
                    UseVisualStyleBackColor = false
                };

                button.Click += (_, _) => HandleButton(key);
                int targetRow = row + 2;

                if (key == "0")
                {
                    layout.Controls.Add(button, col, targetRow);
                    layout.SetColumnSpan(button, 2);
                    col++;
                    continue;
                }

                if (key == "Delete" || key == "Clear" || key == "^" || key == "=")
                {
                    if (key == "=")
                    {
                        layout.Controls.Add(button, 3, targetRow);
                        layout.SetColumnSpan(button, 3);
                    }
                    else
                    {
                        layout.Controls.Add(button, col, targetRow);
                    }

                    continue;
                }

                layout.Controls.Add(button, col, targetRow);
            }
        }

        Controls.Add(layout);
    }

    private void HandleButton(string key)
    {
        if (key == "Clear")
        {
            display.Clear();
            resultLabel.Text = "Ingrese una expresión";
            return;
        }

        if (key == "Delete")
        {
            if (display.Text.Length > 0)
                display.Text = display.Text[..^1];
            return;
        }

        if (key == "=")
        {
            try
            {
                double result = ExpressionEvaluator.Evaluate(display.Text);
                resultLabel.Text = "= " + result.ToString("G15", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                resultLabel.Text = "Error: " + ex.Message;
            }
            return;
        }

        display.Text += key;
        resultLabel.Text = "";
    }
}
