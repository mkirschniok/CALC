namespace CALC_Config;


/// <summary>
/// Ekran podgl¹du kalkulatora
/// </summary>
public partial class Preview : ContentPage
{
    /// <summary>
    /// Wartoœæ pierwszego i drugiego argumentu
    /// </summary>
	private int arg1, arg2;
    /// <summary>
    /// Znak operacji
    /// </summary>
	private string operation;
    /// <summary>
    /// Wynik operacji
    /// </summary>
    private string result;

    /// <summary>
    /// Konstruktor klasy, domyœlne dzia³anie: 1 + 1 = 2
    /// </summary>
    public Preview()
	{
        arg1 = 1;
        arg2 = 1;
        operation = "+";
        result = "2";
        InitializeComponent();
		TcpServer.UpdateValue = UpdateValue;
    }

    /// <summary>
    /// Akcja wysy³aj¹ca wynik
    /// </summary>
    public static Action<string, string>? SendResult;

    /// <summary>
    /// Metoda aktualizuj¹ca wartoœæ argumentów i wynik
    /// </summary>
    /// <param name="role">rola telefonu, od którego przysz³a wartoœæ</param>
    /// <param name="value">rartoœæ, jak¹ przes³a³ telefon</param>
	public void UpdateValue(string role, string value)
    {
        MainThread.BeginInvokeOnMainThread(() =>
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

    /// <summary>
    /// Aktualizacja wyniku
    /// </summary>
    /// <param name="id">identyfikator telefonu, do którego nale¿y przes³aæ wynik</param>
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
        SendResult?.Invoke(result, id);
    }

    /// <summary>
    /// Metoda dostosowuj¹ca rozmiar czcionki
    /// </summary>
    /// <param name="label">etykieta, której czcionkê nale¿y dopasowaæ</param>
    private void AdjustFontSize(Label label)
    {
        label.FontSize = 200 / label.Text.Length;
    }
}