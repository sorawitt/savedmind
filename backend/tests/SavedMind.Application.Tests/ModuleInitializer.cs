using System.Runtime.CompilerServices;
using FluentAssertions;

namespace SavedMind.Application.Tests;

/// <summary>
/// Module initializer to configure FluentAssertions license for open-source project.
/// SavedMind is licensed under MIT License and qualifies for free use.
/// </summary>
public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // Suppress FluentAssertions commercial license warning
        // This project is open-source (MIT License) and qualifies for free use
        License.Accepted = true;
    }
}
