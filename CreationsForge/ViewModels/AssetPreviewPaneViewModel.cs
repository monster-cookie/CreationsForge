using System.Collections.ObjectModel;
using CreationsForge.Commands;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services.Interfaces;
using Serilog;

namespace CreationsForge.ViewModels;

public class AssetPreviewPaneViewModel : ViewModelBase
{
    private readonly IAssetPreviewPathResolverService AssetPreviewPathResolverService;
    private readonly IAssetPreviewSceneService AssetPreviewSceneService;
    private readonly IExternalAssetOpenService ExternalAssetOpenService;
    private readonly ILogger Logger;
    private AssetPreviewCandidateDTO? SelectedCandidateValue;
    private AssetPreviewModelDTO? PreviewModelValue;
    private string PreviewTitleTextValue = "Asset preview";
    private string PreviewStatusTextValue = "Select a model-bearing record to preview assets.";

    public AssetPreviewPaneViewModel(
        IAssetPreviewPathResolverService assetPreviewPathResolverService,
        IAssetPreviewSceneService assetPreviewSceneService,
        IExternalAssetOpenService externalAssetOpenService,
        ILogger logger)
    {
        AssetPreviewPathResolverService = assetPreviewPathResolverService;
        AssetPreviewSceneService = assetPreviewSceneService;
        ExternalAssetOpenService = externalAssetOpenService;
        Logger = logger.ForContext<AssetPreviewPaneViewModel>();
        PreviewCandidates = new ObservableCollection<AssetPreviewCandidateDTO>();
        OpenExternallyCommand = new RelayCommand(OpenSelectedCandidateExternally, () => SelectedCandidate?.CanOpenExternally == true);
    }

    public ObservableCollection<AssetPreviewCandidateDTO> PreviewCandidates { get; }

    public RelayCommand OpenExternallyCommand { get; }

    public bool HasPreviewCandidates => PreviewCandidates.Count > 0;

    public AssetPreviewCandidateDTO? SelectedCandidate
    {
        get => SelectedCandidateValue;
        set
        {
            if (!SetProperty(ref SelectedCandidateValue, value))
            {
                return;
            }

            LoadSelectedCandidatePreview();
            OpenExternallyCommand.RaiseCanExecuteChanged();
        }
    }

    public AssetPreviewModelDTO? PreviewModel
    {
        get => PreviewModelValue;
        private set
        {
            if (!SetProperty(ref PreviewModelValue, value))
            {
                return;
            }

            OnPropertyChanged(nameof(HasPreviewModel));
        }
    }

    public bool HasPreviewModel => PreviewModel is not null;

    public string PreviewTitleText
    {
        get => PreviewTitleTextValue;
        private set => SetProperty(ref PreviewTitleTextValue, value);
    }

    public string PreviewStatusText
    {
        get => PreviewStatusTextValue;
        private set => SetProperty(ref PreviewStatusTextValue, value);
    }

    public void LoadPreviewForRecord(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        PreviewCandidates.Clear();
        OnPropertyChanged(nameof(HasPreviewCandidates));
        PreviewModel = null;
        PreviewTitleText = "Asset preview";

        var candidates = AssetPreviewPathResolverService.GetPreviewCandidates(game, recordType, formKey);
        foreach (var candidate in candidates)
        {
            PreviewCandidates.Add(candidate);
        }

        OnPropertyChanged(nameof(HasPreviewCandidates));

        if (PreviewCandidates.Count == 0)
        {
            SelectedCandidate = null;
            PreviewStatusText = "No model path is persisted for this record.";
            Logger.Information(
                "Asset preview has no model candidates for {Game} {RecordType} {FormKeyId}",
                game,
                recordType,
                formKey.Id);
            return;
        }

        SelectedCandidate = PreviewCandidates.FirstOrDefault(candidate => candidate.CanPreview) ?? PreviewCandidates.First();
    }

    public void ClearPreview()
    {
        PreviewCandidates.Clear();
        OnPropertyChanged(nameof(HasPreviewCandidates));
        SelectedCandidate = null;
        PreviewModel = null;
        PreviewTitleText = "Asset preview";
        PreviewStatusText = "Select a model-bearing record to preview assets.";
    }

    private void LoadSelectedCandidatePreview()
    {
        PreviewModel = null;
        if (SelectedCandidate is null)
        {
            PreviewStatusText = "Select a model-bearing record to preview assets.";
            return;
        }

        PreviewTitleText = SelectedCandidate.DisplayName;
        if (!SelectedCandidate.CanPreview)
        {
            PreviewStatusText = SelectedCandidate.UnsupportedReason ?? "This asset type is not supported by the experimental preview renderer.";
            Logger.Warning(
                "Asset preview does not support path {MeshPath} for {Game} {RecordType} {FormKeyId}",
                SelectedCandidate.MeshPath,
                SelectedCandidate.Game,
                SelectedCandidate.RecordType,
                SelectedCandidate.FormKey.Id);
            return;
        }

        PreviewModel = AssetPreviewSceneService.CreateSamplePreview(SelectedCandidate);
        PreviewStatusText = $"Experimental sample render for {SelectedCandidate.MeshPath}";
    }

    private void OpenSelectedCandidateExternally()
    {
        if (SelectedCandidate is null)
        {
            return;
        }

        if (!ExternalAssetOpenService.OpenExternally(SelectedCandidate.MeshPath))
        {
            PreviewStatusText = $"Unable to open {SelectedCandidate.MeshPath} externally.";
        }
    }
}
