using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using Serilog;

namespace CreationsForge.Views;

public class AssetPreviewPaneView : UserControl
{
    private readonly ILogger Logger;
    private readonly AssetPreviewFallbackSurface FallbackSurface;
    private readonly Border RendererDiagnosticHost;
    private readonly TextBlock RendererDiagnosticText;
    private readonly AssetPreviewOpenGlControl PreviewSurface;
    private readonly AssetPreviewPaneViewModel ViewModel;

    public AssetPreviewPaneView(
        AssetPreviewPaneViewModel viewModel,
        IAssetPreviewRenderMeshFactory renderMeshFactory,
        ILogger logger)
    {
        ViewModel = viewModel;
        Logger = logger.ForContext<AssetPreviewPaneView>();
        DataContext = ViewModel;
        FallbackSurface = new AssetPreviewFallbackSurface();
        PreviewSurface = new AssetPreviewOpenGlControl(renderMeshFactory, logger);
        RendererDiagnosticText = CreateDiagnosticText();
        RendererDiagnosticHost = CreateDiagnosticHost(RendererDiagnosticText);
        PreviewSurface.DiagnosticsChanged += OnPreviewSurfaceDiagnosticsChanged;
        Content = BuildContent();
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        RefreshViewport();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        PreviewSurface.DiagnosticsChanged -= OnPreviewSurfaceDiagnosticsChanged;
        base.OnDetachedFromVisualTree(e);
    }

    private Control BuildContent()
    {
        var title = CreateBoundText(nameof(AssetPreviewPaneViewModel.PreviewTitleText), 16, FontWeight.SemiBold);
        var status = CreateBoundText(nameof(AssetPreviewPaneViewModel.PreviewStatusText), 12, FontWeight.Normal);
        var candidates = CreateCandidateSelector();
        var controls = CreatePreviewControls();

        var openButton = new Button
        {
            Content = "Open externally",
            HorizontalAlignment = HorizontalAlignment.Right,
            Padding = new Thickness(12, 6)
        };
        openButton.Bind(Button.CommandProperty, new Binding(nameof(AssetPreviewPaneViewModel.OpenExternallyCommand)));

        var header = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto"),
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            ColumnSpacing = 10,
            RowSpacing = 8,
            Children =
            {
                title,
                openButton,
                candidates
            }
        };
        Grid.SetColumn(openButton, 1);
        Grid.SetRow(candidates, 1);
        Grid.SetColumnSpan(candidates, 2);

        Grid.SetRow(header, 0);
        Grid.SetRow(controls, 1);
        var previewHost = BuildPreviewHost();
        Grid.SetRow(previewHost, 2);
        Grid.SetRow(status, 3);
        var root = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto,*,Auto"),
            RowSpacing = 10,
            Children =
            {
                header,
                controls,
                previewHost,
                status
            }
        };

        return new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(1, 0, 0, 0),
            Padding = new Thickness(14),
            Child = root
        };
    }

    private Control BuildPreviewHost()
    {
        return new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(10, 15, 20)),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(1),
            Child = new Grid
            {
                MinHeight = 260,
                Children =
                {
                    FallbackSurface,
                    PreviewSurface,
                    RendererDiagnosticHost,
                    CreateLoadingOverlay()
                }
            }
        };
    }

    private static Control CreateLoadingOverlay()
    {
        var progressBar = new ProgressBar
        {
            IsIndeterminate = true,
            Height = 4,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        var textBlock = new TextBlock
        {
            Text = "Loading asset preview...",
            FontSize = 13,
            FontWeight = FontWeight.SemiBold,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        App.ApplyApplicationTextForeground(textBlock);

        var overlay = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0)),
            Padding = new Thickness(18),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Child = new StackPanel
            {
                Width = 220,
                Spacing = 10,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    textBlock,
                    progressBar
                }
            }
        };
        overlay.Bind(IsVisibleProperty, new Binding(nameof(AssetPreviewPaneViewModel.IsPreviewLoading)));
        return overlay;
    }

    private ComboBox CreateCandidateSelector()
    {
        var comboBox = new ComboBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MinHeight = 32,
            MaxDropDownHeight = 220
        };
        comboBox.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(AssetPreviewPaneViewModel.PreviewCandidates)));
        comboBox.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(AssetPreviewPaneViewModel.SelectedCandidate))
        {
            Mode = BindingMode.TwoWay
        });
        comboBox.ItemTemplate = new FuncDataTemplate<AssetPreviewCandidateDTO>(
            (candidate, _) =>
            {
                var textBlock = new TextBlock
                {
                    Text = candidate?.DisplayName ?? string.Empty,
                    TextTrimming = TextTrimming.CharacterEllipsis
                };
                App.ApplyApplicationTextForeground(textBlock);
                return textBlock;
            });
        return comboBox;
    }

    private Control CreatePreviewControls()
    {
        var viewModeSelector = new ComboBox
        {
            Width = 110,
            MinHeight = 30
        };
        viewModeSelector.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(AssetPreviewPaneViewModel.ViewModes)));
        viewModeSelector.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(AssetPreviewPaneViewModel.SelectedViewMode))
        {
            Mode = BindingMode.TwoWay
        });

        var renderModeSelector = new ComboBox
        {
            Width = 105,
            MinHeight = 30
        };
        renderModeSelector.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(AssetPreviewPaneViewModel.RenderModes)));
        renderModeSelector.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(AssetPreviewPaneViewModel.SelectedRenderMode))
        {
            Mode = BindingMode.TwoWay
        });

        var orbitToggle = new CheckBox
        {
            Content = "Orbit",
            VerticalAlignment = VerticalAlignment.Center
        };
        orbitToggle.Bind(ToggleButton.IsCheckedProperty, new Binding(nameof(AssetPreviewPaneViewModel.IsOrbitEnabled))
        {
            Mode = BindingMode.TwoWay
        });

        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 10,
            Children =
            {
                viewModeSelector,
                renderModeSelector,
                orbitToggle
            }
        };
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AssetPreviewPaneViewModel.PreviewModel) ||
            e.PropertyName == nameof(AssetPreviewPaneViewModel.SelectedViewMode) ||
            e.PropertyName == nameof(AssetPreviewPaneViewModel.SelectedMeshSelection) ||
            e.PropertyName == nameof(AssetPreviewPaneViewModel.SelectedRenderMode) ||
            e.PropertyName == nameof(AssetPreviewPaneViewModel.IsOrbitEnabled) ||
            e.PropertyName == nameof(AssetPreviewPaneViewModel.IsPreviewLoading))
        {
            RefreshViewport();
        }
    }

    private void RefreshViewport()
    {
        var previewContentVisible = !ViewModel.IsPreviewLoading;
        FallbackSurface.IsVisible = previewContentVisible;
        PreviewSurface.IsVisible = previewContentVisible;
        RendererDiagnosticHost.IsVisible = previewContentVisible;
        FallbackSurface.PreviewModel = ViewModel.PreviewModel;
        FallbackSurface.StatusText = ViewModel.PreviewModel is null
            ? "No asset selected"
            : "Fallback preview";
        FallbackSurface.InvalidateVisual();
        PreviewSurface.PreviewModel = ViewModel.PreviewModel;
        PreviewSurface.RenderOptions = new AssetPreviewRenderOptions
        {
            MeshIndex = null
        };
        PreviewSurface.ViewMode = ViewModel.SelectedViewMode;
        PreviewSurface.RenderMode = ViewModel.SelectedRenderMode;
        PreviewSurface.IsOrbitEnabled = ViewModel.IsOrbitEnabled;
        PreviewSurface.RequestNextFrameRendering();
        Logger.Information(
            "Asset preview surfaces refreshed with {MeshCount} meshes",
            ViewModel.PreviewModel?.Meshes.Count ?? 0);
    }

    private void OnPreviewSurfaceDiagnosticsChanged(object? sender, EventArgs e)
    {
        RendererDiagnosticText.Text = PreviewSurface.DiagnosticText;
    }

    private static TextBlock CreateBoundText(string boundProperty, double fontSize, FontWeight fontWeight)
    {
        var textBlock = new TextBlock
        {
            FontSize = fontSize,
            FontWeight = fontWeight,
            TextWrapping = TextWrapping.Wrap
        };
        App.ApplyApplicationTextForeground(textBlock);
        textBlock.Bind(TextBlock.TextProperty, new Binding(boundProperty));
        return textBlock;
    }

    private static Border CreateDiagnosticHost(TextBlock diagnosticText)
    {
        return new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(190, 0, 0, 0)),
            Padding = new Thickness(8, 4),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Bottom,
            Child = diagnosticText
        };
    }

    private static TextBlock CreateDiagnosticText()
    {
        return new TextBlock
        {
            Text = "OpenGL: waiting",
            FontSize = 11,
            TextWrapping = TextWrapping.Wrap,
            Foreground = new SolidColorBrush(Color.FromRgb(230, 236, 244))
        };
    }

    private class AssetPreviewFallbackSurface : Control
    {
        public AssetPreviewFallbackSurface()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            MinHeight = 260;
        }

        public AssetPreviewModelDTO? PreviewModel { get; set; }

        public string StatusText { get; set; } = "No asset selected";

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            var bounds = Bounds;
            context.FillRectangle(new SolidColorBrush(Color.FromRgb(10, 15, 20)), bounds);
            DrawGrid(context, bounds);
            if (PreviewModel is null)
            {
                DrawEmptyState(context, bounds, StatusText);
                return;
            }

            DrawSampleGeometry(context, bounds);
        }

        private static void DrawGrid(DrawingContext context, Rect bounds)
        {
            var gridPen = new Pen(new SolidColorBrush(Color.FromArgb(70, 80, 96, 112)), 1);
            for (var x = bounds.Left; x < bounds.Right; x += 32)
            {
                context.DrawLine(gridPen, new Point(x, bounds.Top), new Point(x, bounds.Bottom));
            }

            for (var y = bounds.Top; y < bounds.Bottom; y += 32)
            {
                context.DrawLine(gridPen, new Point(bounds.Left, y), new Point(bounds.Right, y));
            }
        }

        private static void DrawEmptyState(DrawingContext context, Rect bounds, string textValue)
        {
            var text = new FormattedText(
                textValue,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                Typeface.Default,
                14,
                new SolidColorBrush(Color.FromRgb(190, 200, 210)));
            context.DrawText(text, new Point(
                bounds.Center.X - text.Width / 2,
                bounds.Center.Y - text.Height / 2));
        }

        private static void DrawSampleGeometry(DrawingContext context, Rect bounds)
        {
            var center = bounds.Center;
            var size = Math.Min(bounds.Width, bounds.Height) * 0.34;
            var top = new Point(center.X, center.Y - size);
            var left = new Point(center.X - size, center.Y + size * 0.65);
            var right = new Point(center.X + size, center.Y + size * 0.65);
            var rear = new Point(center.X + size * 0.34, center.Y - size * 0.08);

            var faceBrush = new SolidColorBrush(Color.FromArgb(210, 42, 132, 220));
            var sideBrush = new SolidColorBrush(Color.FromArgb(180, 24, 88, 160));
            var highlightPen = new Pen(new SolidColorBrush(Color.FromRgb(255, 215, 64)), 2);

            var front = new StreamGeometry();
            using (var geometry = front.Open())
            {
                geometry.BeginFigure(top, true);
                geometry.LineTo(left);
                geometry.LineTo(right);
                geometry.EndFigure(true);
            }

            var side = new StreamGeometry();
            using (var geometry = side.Open())
            {
                geometry.BeginFigure(top, true);
                geometry.LineTo(right);
                geometry.LineTo(rear);
                geometry.EndFigure(true);
            }

            context.DrawGeometry(sideBrush, highlightPen, side);
            context.DrawGeometry(faceBrush, highlightPen, front);
            context.DrawLine(highlightPen, left, rear);
            context.DrawLine(highlightPen, rear, top);
        }
    }
}
