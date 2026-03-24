# KVibrations

A simplified vibration plugin for Unity that supports iOS and Android platforms.

## Key Components

### KVibrations
The master static class for all vibration functionality. Use this class to:
- Play predefined patterns (Selection, Success, Warning, Failure, Impacts)
- Play custom patterns with amplitude and time arrays
- Play emphasis (quick vibrations)
- Play constant vibrations
- Control global vibration enable/disable
- Adjust output level

### KVibrationsSettings
A ScriptableObject that stores all vibration pattern configurations:
- iOS-specific patterns
- Android-specific patterns
- Customizable pattern data (time and amplitude arrays)

### VibrationTypes
Enum for predefined vibration patterns:
- `Selection` - UI selection feedback
- `Success` - Success notification
- `Warning` - Warning notification
- `Failure` - Failure/error notification
- `LightImpact` - Light impact feedback
- `MediumImpact` - Medium impact feedback
- `HeavyImpact` - Heavy impact feedback
- `RigidImpact` - Rigid impact feedback
- `SoftImpact` - Soft impact feedback
- `None` - No vibration

## Setup

**No setup required!** The controller auto-initializes on first use.

### Optional: Custom Settings

If you want to customize vibration patterns:

1. **Create Settings Asset in Resources folder**
   - Create a `Resources` folder in your Assets (if it doesn't exist)
   - Right-click in the Resources folder
   - Create > KVibrations > Settings
   - Name it exactly "KVibrationsSettings"

2. **Customize Patterns**
   - Select the KVibrationsSettings asset
   - Modify iOS and Android patterns as needed
   - The controller will automatically load this on first use

## Usage Examples

### Play Predefined Patterns
```csharp
// Play a success vibration
KVibrationsController.PlayPattern(VibrationType.Success);

// Play a medium impact
KVibrationsController.PlayPattern(VibrationType.MediumImpact);
```

### Play Custom Patterns
```csharp
// Define custom pattern with amplitude and time arrays
float[] amplitudes = new float[] { 0.0f, 0.8f, 0.0f, 1.0f };
float[] times = new float[] { 0.0f, 0.05f, 0.1f, 0.15f };

KVibrationsController.PlayCustomPattern(amplitudes, times);
```

### Play Emphasis (Quick Vibration)
```csharp
// amplitude: 0.0-1.0, frequency: 0.0-1.0
KVibrationsController.PlayEmphasis(0.7f, 0.5f);
```

### Play Constant Vibration
```csharp
// amplitude: 0.0-1.0, frequency: 0.0-1.0, duration in seconds
KVibrationsController.PlayConstant(0.5f, 0.5f, 0.5f);
```

### Control Vibrations
```csharp
// Enable/disable vibrations globally
KVibrationsController.vibrationsEnabled = true;

// Adjust output level (0.0 to 1.0)
KVibrationsController.outputLevel = 0.8f;

// Stop current vibration
KVibrationsController.Stop();
```

## Platform Support

- **iOS**: Minimum iOS 11, uses native haptic feedback
- **Android**: Minimum API level 17, uses amplitude-controlled vibrations
- **Editor**: Supported for testing (no actual vibration)

## Customizing Patterns

You can customize the patterns in your KVibrationsSettings asset:

1. Select your KVibrationsSettings asset
2. Expand "iOS Patterns" or "Android Patterns"
3. Modify the time and amplitude arrays for each pattern
4. Save the asset

Each pattern has:
- **time**: Array of time points (in seconds)
- **amplitude**: Array of vibration intensities (0.0 to 1.0)

Arrays must be the same length and sorted by time.

## Migration from NiceVibrations

If you're migrating from the original NiceVibrations plugin:

1. Replace `HapticController` with `KVibrations`
2. Replace `HapticPatterns.PresetType` with `VibrationType`
3. Remove any gamepad-related code
4. Remove any `.haptic` file loading code
5. Replace `HapticController.Play(clip)` with pattern-based calls
6. Create and assign a `KVibrationsSettings` asset
7. Initialize with `KVibrationsController.Init(settings)`

## Notes

- Only one vibration can play at a time (new vibrations will stop previous ones)
- Vibrations are platform-specific and may feel different on different devices
- Always test vibrations on actual devices, not just in the editor
- Consider providing users with an option to disable vibrations in your app settings

## License

Based on NiceVibrations by Meta Platforms, Inc. and affiliates.
