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
    private AssetPreviewMeshSelectionOption SelectedMeshSelectionValue = AssetPreviewMeshSelectionOption.All;
    private AssetPreviewViewMode SelectedViewModeValue = AssetPreviewViewMode.Isometric;
    private AssetPreviewRenderMode SelectedRenderModeValue = AssetPreviewRenderMode.Solid;
    private bool IsOrbitEnabledValue;
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
        MeshSelections = new ObservableCollection<AssetPreviewMeshSelectionOption>
        {
            AssetPreviewMeshSelectionOption.All
        };
        ViewModes = new ObservableCollection<AssetPreviewViewMode>
        {
            AssetPreviewViewMode.Isometric,
            AssetPreviewViewMode.Front,
            AssetPreviewViewMode.Back,
            AssetPreviewViewMode.Side,
            AssetPreviewViewMode.Top
        };
        RenderModes = new ObservableCollection<AssetPreviewRenderMode>
        {
            AssetPreviewRenderMode.Solid,
            AssetPreviewRenderMode.Wireframe,
            AssetPreviewRenderMode.Points
        };
        OpenExternallyCommand = new RelayCommand(OpenSelectedCandidateExternally, () => SelectedCandidate?.CanOpenExternally == true);
    }

    public ObservableCollection<AssetPreviewCandidateDTO> PreviewCandidates { get; }

    public ObservableCollection<AssetPreviewMeshSelectionOption> MeshSelections { get; }

    public ObservableCollection<AssetPreviewViewMode> ViewModes { get; }

    public ObservableCollection<AssetPreviewRenderMode> RenderModes { get; }

    public RelayCommand OpenExternallyCommand { get; }

    public bool HasPreviewCandidates => PreviewCandidates.Count > 0;

    public AssetPreviewCandidateDTO? SelectedCandidate
    {
        get => SelectedCandidateValue;
        set
        {
            var previousCandidate = SelectedCandidateValue;
            if (!SetProperty(ref SelectedCandidateValue, value))
            {
                return;
            }

            OpenExternallyCommand.RaiseCanExecuteChanged();
            if (IsSameAssetCandidate(previousCandidate, value))
            {
                return;
            }

            LoadSelectedCandidatePreview();
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

    public AssetPreviewMeshSelectionOption? SelectedMeshSelection
    {
        get => SelectedMeshSelectionValue;
        set => SetProperty(ref SelectedMeshSelectionValue, value ?? AssetPreviewMeshSelectionOption.All);
    }

    public AssetPreviewViewMode SelectedViewMode
    {
        get => SelectedViewModeValue;
        set => SetProperty(ref SelectedViewModeValue, value);
    }

    public AssetPreviewRenderMode SelectedRenderMode
    {
        get => SelectedRenderModeValue;
        set => SetProperty(ref SelectedRenderModeValue, value);
    }

    public bool IsOrbitEnabled
    {
        get => IsOrbitEnabledValue;
        set => SetProperty(ref IsOrbitEnabledValue, value);
    }

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
        ResetMeshSelections(null);
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
        ResetMeshSelections(null);
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

        PreviewModel = AssetPreviewSceneService.CreatePreview(SelectedCandidate, out var statusMessage);
        ResetMeshSelections(PreviewModel);
        PreviewStatusText = statusMessage;
    }

    private void ResetMeshSelections(AssetPreviewModelDTO? previewModel)
    {
        MeshSelections.Clear();
        MeshSelections.Add(AssetPreviewMeshSelectionOption.All);
        if (previewModel != null)
        {
            for (var index = 0; index < previewModel.Meshes.Count; index++)
            {
                MeshSelections.Add(new AssetPreviewMeshSelectionOption(index, previewModel.Meshes[index].Name));
            }
        }

        SelectedMeshSelection = AssetPreviewMeshSelectionOption.All;
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

    private static bool IsSameAssetCandidate(AssetPreviewCandidateDTO? first, AssetPreviewCandidateDTO? second)
    {
        if (first is null || second is null)
        {
            return first is null && second is null;
        }

        return first.Game == second.Game &&
            string.Equals(first.RecordType, second.RecordType, StringComparison.OrdinalIgnoreCase) &&
            first.FormKey.Id == second.FormKey.Id &&
            string.Equals(first.MeshPath, second.MeshPath, StringComparison.OrdinalIgnoreCase) &&
            first.CanPreview == second.CanPreview &&
            first.CanOpenExternally == second.CanOpenExternally;
    }

    public readonly struct AssetPreviewMeshSelectionOption
    {
        public static readonly AssetPreviewMeshSelectionOption All = new AssetPreviewMeshSelectionOption(null, "All meshes");

        public AssetPreviewMeshSelectionOption(int? meshIndex, string displayName)
        {
            MeshIndex = meshIndex;
            DisplayName = displayName;
        }

        public int? MeshIndex { get; }

        public string DisplayName { get; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
