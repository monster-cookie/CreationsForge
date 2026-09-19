using System.Globalization;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.ViewModels;

namespace CreationsForge.Views;

/// <summary>Presents every independent native float-setting field as one direct staged-record form.</summary>
public sealed class GameSettingFloatEditorView : UserControl
{
    private readonly MajorRecordBrowserViewModel ViewModel;
    private readonly TextBlock Heading;
    private readonly TextBlock Message;
    private readonly TextBox EditorId;
    private readonly TextBox Data;
    private readonly TextBox RawFlags;
    private readonly TextBox FormVersion;
    private readonly TextBox Version2;
    private readonly TextBox VersionControl;
    private readonly TextBox Xalg;
    private readonly Control XalgRow;
    private readonly Button SaveButton;
    private GameSettingFloatEditorSession? Session;
    private bool Busy;

    /// <summary>Initializes the direct native-field editor inside the shared record-selection screen.</summary>
    public GameSettingFloatEditorView(MajorRecordBrowserViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        Heading = new TextBlock { FontSize = 16, FontWeight = FontWeight.SemiBold };
        Message = new TextBlock { TextWrapping = TextWrapping.Wrap };
        EditorId = Field("GameSettingFloatEditorId");
        Data = Field("GameSettingFloatData");
        RawFlags = Field("GameSettingFloatRawFlags");
        FormVersion = Field("GameSettingFloatFormVersion");
        Version2 = Field("GameSettingFloatVersion2");
        VersionControl = Field("GameSettingFloatVersionControl");
        Xalg = Field("GameSettingFloatXalg");
        XalgRow = Row("XALG (Starfield)", Xalg);
        SaveButton = new Button { Content = "Save record", Padding = new Thickness(16, 8) };
        SaveButton.Click += async (_, _) => await SaveAsync();
        AutomationProperties.SetAutomationId(SaveButton, "GameSettingFloatSaveButton");
        Content = new ScrollViewer
        {
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
            Content = new StackPanel
            {
                Spacing = 10,
                Children =
                {
                    Heading,
                    new TextBlock { Text = "Edit the record fields, then save the record. Save Changes writes staged records to the plugin.", TextWrapping = TextWrapping.Wrap },
                    Row("EditorID", EditorId),
                    Row("Value", Data),
                    Row("Major record flags (raw integer)", RawFlags),
                    Row("Form version", FormVersion),
                    Row("Version 2", Version2),
                    Row("Version control", VersionControl),
                    XalgRow,
                    SaveButton,
                    Message
                }
            }
        };
        AutomationProperties.SetAutomationId(this, "GameSettingFloatEditorView");
        SetBusy(false);
    }

    /// <summary>Begins a new record when <paramref name="row"/> is null, or the exact selected record edit.</summary>
    public async Task BeginAsync(MajorRecordViewModel? row)
    {
        if (Busy)
        {
            return;
        }

        Session = null;
        SetBusy(true);
        Message.Text = "Opening record fields...";
        try
        {
            var result = await ViewModel.BeginGameSettingFloatAsync(row);
            if (!result.Succeeded || result.Value is null)
            {
                ShowError(result.Error?.Message ?? "The record could not be opened for editing.");
                return;
            }

            Session = result.Value;
            FillForm(result.Value);
            Message.Text = string.Empty;
        }
        catch (Exception exception)
        {
            ShowError($"The record editor failed: {exception.Message}");
        }
        finally
        {
            SetBusy(false);
        }
    }

    /// <summary>Clears a form retained from an earlier workspace.</summary>
    public void Clear()
    {
        Session = null;
        Heading.Text = string.Empty;
        Message.Text = string.Empty;
        SetBusy(false);
    }

    private async Task SaveAsync()
    {
        if (Busy || Session is null)
        {
            return;
        }

        if (!TryCreateRequest(Session, out var request, out var error))
        {
            ShowError(error);
            return;
        }

        SetBusy(true);
        Message.Text = "Saving record fields...";
        try
        {
            var result = await ViewModel.SaveGameSettingFloatAsync(Session, request!);
            if (!result.Succeeded || result.Value is null)
            {
                ShowError(result.Error?.Message ?? "The record fields could not be saved.");
                return;
            }

            Session = result.Value;
            FillForm(result.Value);
            Message.Foreground = App.GetApplicationBrush(App.ApplicationForegroundBrushKey);
            Message.Text = "Record staged. Save Changes writes it to the plugin.";
        }
        catch (Exception exception)
        {
            ShowError($"The record save failed: {exception.Message}");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private bool TryCreateRequest(
        GameSettingFloatEditorSession session,
        out GameSettingFloatEditRequest? request,
        out string error)
    {
        request = null;
        error = string.Empty;
        if (string.IsNullOrWhiteSpace(EditorId.Text) || !EditorId.Text.StartsWith('f'))
        {
            error = "A float game-setting EditorID must begin with f.";
            return false;
        }

        float? data = null;
        if (!string.IsNullOrWhiteSpace(Data.Text))
        {
            if (!float.TryParse(Data.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedData) || !float.IsFinite(parsedData))
            {
                error = "Value must be a finite number, or blank for no value.";
                return false;
            }

            data = parsedData;
        }

        if (!int.TryParse(RawFlags.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var flags) ||
            !ushort.TryParse(FormVersion.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var formVersion) ||
            !ushort.TryParse(Version2.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var version2) ||
            !uint.TryParse(VersionControl.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var versionControl))
        {
            error = "Flags and version fields must be whole numbers in their native ranges.";
            return false;
        }

        ulong? xalg = null;
        if (session.SupportsXalg && !string.IsNullOrWhiteSpace(Xalg.Text))
        {
            if (!ulong.TryParse(Xalg.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedXalg))
            {
                error = "XALG must be a whole number in the native unsigned 64-bit range.";
                return false;
            }

            xalg = parsedXalg;
        }

        request = new GameSettingFloatEditRequest(
            Guid.NewGuid(), session.Revision, session.EditId, EditorId.Text!, data,
            flags, formVersion, version2, versionControl, xalg);
        return true;
    }

    private void FillForm(GameSettingFloatEditorSession session)
    {
        Heading.Text = $"GameSettingFloat {session.FormKey}";
        EditorId.Text = session.EditorId;
        Data.Text = session.Data?.ToString("R", CultureInfo.InvariantCulture) ?? string.Empty;
        RawFlags.Text = session.MajorRecordFlagsRaw.ToString(CultureInfo.InvariantCulture);
        FormVersion.Text = session.FormVersion.ToString(CultureInfo.InvariantCulture);
        Version2.Text = session.Version2.ToString(CultureInfo.InvariantCulture);
        VersionControl.Text = session.VersionControl.ToString(CultureInfo.InvariantCulture);
        Xalg.Text = session.Xalg?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        XalgRow.IsVisible = session.SupportsXalg;
    }

    private void SetBusy(bool busy)
    {
        Busy = busy;
        SaveButton.IsEnabled = !busy && Session is not null;
    }

    private void ShowError(string message)
    {
        Message.Foreground = new SolidColorBrush(Color.FromRgb(204, 73, 73));
        Message.Text = message;
    }

    private static TextBox Field(string automationId)
    {
        var box = new TextBox { MinWidth = 240, HorizontalAlignment = HorizontalAlignment.Left };
        AutomationProperties.SetAutomationId(box, automationId);
        return box;
    }

    private static Control Row(string label, Control field) => new StackPanel
    {
        Spacing = 4,
        Children = { new TextBlock { Text = label, FontWeight = FontWeight.SemiBold }, field }
    };
}
