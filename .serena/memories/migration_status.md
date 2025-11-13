# Logbert .NET 10 Avalonia Migration Status

## Current State
- The project is migrating from WinForms to pure Avalonia UI framework
- Target: .NET 8.0 with Avalonia 11.x
- Branch: `claude/migrate-dotnet-10-011CUoEh8j6tvFmsR8q5ACc2`

## Build Issues

### Fixed
- ✅ AvaloniaEdit package reference fixed (was "AvaloniaEdit" 11.1.0, now "Avalonia.AvaloniaEdit" 11.3.0)
- ✅ System.Resources.Extensions added (v8.0.0) for .resx file support

### Current Blockers
- ❌ 1364 Compilation errors, mostly from WinForms Designer.cs files
  - System.Windows.Forms dependencies in Receiver settings dialogs
  - Auto-generated Designer files inherit from WinForms UserControl
  - Located in: `Receiver/*/...Settings.Designer.cs`

## Root Cause
The Receiver settings dialogs (NLog, Custom, Syslog, WinDebug, etc.) still have WinForms-based Designer.cs files that:
1. Inherit from `System.Windows.Forms.UserControl`
2. Reference `System.Windows.Forms` namespace
3. Use WinForms controls (TextBox, ComboBox, etc.)

These were NOT refactored during the Avalonia migration and need to be either:
- **Option A**: Removed entirely if functionality moved to Avalonia
- **Option B**: Converted to Avalonia equivalents
- **Option C**: Stubbed out/excluded from build if not yet implemented

## Next Steps
Need to determine which Receiver dialog settings are actually required for the Avalonia version and refactor accordingly.
