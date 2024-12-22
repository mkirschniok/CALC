namespace CALC_Config;

public partial class Preview : ContentPage
{
	private int arg1, arg2;
	private string operation;
    private string result;
    private int digitMaxWidth = 0;

    public Preview()
	{
		InitializeComponent();
		TcpServer.UpdateResult = UpdateResult;
		TcpServer.UpdateValue = UpdateValue;

        // Get the width of the grid
        double screenWidth = MainGrid.Width;
        digitMaxWidth = (int)screenWidth / 5;
    }

    public static Action<string, string> SendResult;

	public void UpdateValue(string role, string value)
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            if (role == "arg1")
            {
                arg1 = int.Parse(value);
                Arg1_Label.Text = arg1.ToString();
                AdjustFontSize(Arg1_Label);
            }
            else if (role == "arg2")
            {
                arg2 = int.Parse(value);
                Arg2_Label.Text = arg2.ToString();
                AdjustFontSize(Arg2_Label);
            }
            else if (role == "oper")
            {
                operation = value;
                Oper_Label.Text = operation;
                AdjustFontSize(Oper_Label);
            }
            UpdateResult("4");
        });
    }

    public void UpdateResult(string id)
    {
        switch (operation)
        {
            case "+":
                result = (arg1 + arg2).ToString();
                break;
            case "-":
                result = (arg1 - arg2).ToString();
                break;
            case "×":
                result = (arg1 * arg2).ToString();
                break;
            case "÷":
                if (arg2 == 0)
                {
                    result = "B³¹d";
                    break;
                }
                else if (arg1 % arg2 == 0) {
                    result = (arg1 / arg2).ToString();
                }
                else
                {
                    result = (arg1 / arg2).ToString() + " r " + (arg1 % arg2).ToString();
                }
                break;
        }
        Result_Label.Text = result;
        AdjustFontSize(Result_Label);
        SendResult.Invoke(result, id);
    }

    private void AdjustFontSize(Label label)
    {
        label.FontSize = 300 / label.Text.Length;
    }
}