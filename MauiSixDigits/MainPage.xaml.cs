using System.Text;

namespace MauiSixDigits;

public partial class MainPage : ContentPage {
    private readonly Random _rng = new Random();

    private string _currentRandom = "------";
    private string _expectedAnswer = "------";

    private int _secondsLeft = 20;
    private CancellationTokenSource? _cts;
    private bool _roundActive = false;

    public MainPage() {
        InitializeComponent();
        ResetUi();
    }

    private void ResetUi() {
        _roundActive = false;
        _secondsLeft = 20;

        _currentRandom = "------";
        _expectedAnswer = "------";

        RandomNumberLabel.Text = _currentRandom;
        TimerLabel.Text = "Χρόνος: 20";

        UserEntry.Text = "";
        UserEntry.IsEnabled = false;

        ResultLabel.Text = "—";

        StartButton.IsEnabled = true;
    }

    private void OnStartClicked(object sender, EventArgs e) {
        StartNewRound();
    }

    private void StartNewRound() {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        _roundActive = true;
        _secondsLeft = 20;

        _currentRandom = _rng.Next(100000, 1000000).ToString();
        _expectedAnswer = ComputeExpected(_currentRandom);

        RandomNumberLabel.Text = _currentRandom;
        TimerLabel.Text = $"Χρόνος: {_secondsLeft}";

        ResultLabel.Text = "Περιμένω την απάντησή σου...";
        StartButton.IsEnabled = false;

        UserEntry.Text = "";
        UserEntry.IsEnabled = true;
        UserEntry.Focus();

        _ = RunCountdownAsync(_cts.Token);
    }

    private async Task RunCountdownAsync(CancellationToken token) {
        try {
            while (_secondsLeft > 0 && !token.IsCancellationRequested) {
                await Task.Delay(1000, token);
                _secondsLeft--;

                MainThread.BeginInvokeOnMainThread(() => {
                    TimerLabel.Text = $"Χρόνος: {_secondsLeft}";
                });
            }

            if (!token.IsCancellationRequested) {
                MainThread.BeginInvokeOnMainThread(() => {
                    EndRoundTimeout();
                });
            }
        }
        catch (TaskCanceledException) {
        }
    }

    private void EndRoundTimeout() {
        if (!_roundActive) return;

        _roundActive = false;
        UserEntry.IsEnabled = false;

        ResultLabel.Text =
            $"ΤΕΛΟΣ ΧΡΟΝΟΥ!\n" +
            $"Ο σωστός αριθμός που έπρεπε να γράψεις είναι: {_expectedAnswer}\n" +
            $"(Από τον τυχαίο: {_currentRandom})";

        StartButton.IsEnabled = true;
    }

    // ⭐ ΝΕΑ μέθοδος αυτόματου submit
    private void SubmitAnswer(string user) {
        if (!_roundActive) return;

        _cts?.Cancel();

        _roundActive = false;
        UserEntry.IsEnabled = false;
        StartButton.IsEnabled = true;

        if (user == _expectedAnswer) {
            ResultLabel.Text =
                $"ΣΩΣΤΟ ✅\nΤυχαίος: {_currentRandom}\nΣωστό: {_expectedAnswer}";
        }
        else {
            ResultLabel.Text =
                $"ΛΑΘΟΣ ❌\nΤυχαίος: {_currentRandom}\nΔικό σου: {user}\nΣωστό: {_expectedAnswer}";
        }
    }

    // ⭐ ΕΔΩ γίνεται το AUTO FINISH όταν φτάσει 6 ψηφία
    private void OnEntryTextChanged(object sender, TextChangedEventArgs e) {
        if (!_roundActive) return;

        var text = (e.NewTextValue ?? "").Trim();

        // κράτα μόνο digits
        var digitsOnly = new string(text.Where(char.IsDigit).ToArray());

        if (digitsOnly.Length > 6)
            digitsOnly = digitsOnly.Substring(0, 6);

        if (digitsOnly != text) {
            UserEntry.Text = digitsOnly;
            return;
        }

        // ⭐ AUTO SUBMIT όταν φτάσει 6 ψηφία
        if (digitsOnly.Length == 6) {
            SubmitAnswer(digitsOnly);
        }
    }

    private static string ComputeExpected(string sixDigits) {
        int[] sub = { 6, 5, 4, 3, 2, 1 };

        var sb = new StringBuilder(6);
        for (int i = 0; i < 6; i++) {
            int d = sixDigits[i] - '0';
            int v = d - sub[i];
            if (v < 0) v = 0;
            sb.Append(v);
        }
        return sb.ToString();
    }
}