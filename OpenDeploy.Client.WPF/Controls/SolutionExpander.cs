using System.Windows.Controls;
using System.Windows;

namespace OpenDeploy.Client.Controls;

/// <summary>
/// 解决方案 Expander
/// </summary>
public class SolutionExpander : Expander
{
    public static readonly DependencyProperty SolutionExpanderButtonAreaContentProperty
        = DependencyProperty.Register
        (
            "SolutionExpanderButtonAreaContent",
            typeof(object),
            typeof(SolutionExpander),
            new PropertyMetadata(default(object))
        );

    public object SolutionExpanderButtonAreaContent
    {
        get => GetValue(SolutionExpanderButtonAreaContentProperty);
        set => SetValue(SolutionExpanderButtonAreaContentProperty, value);
    }
}
