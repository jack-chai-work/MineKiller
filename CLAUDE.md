# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build / Run

```bash
# Build the mod (Debug)
dotnet build MineKiller/MineKiller.csproj

# Build for release
dotnet build MineKiller/MineKiller.csproj -c Release

# The build output (MineKiller.dll) is automatically copied to the Mods folder
# for Stardew Valley via Pathoschild.Stardew.ModBuildConfig.
```

The project targets .NET 6.0 (SDK 6.0.0, see `global.json`) and requires the .NET 6 SDK installed.

## Architecture

This is a **Stardew Valley SMAPI mod** that automatically kills monsters in the mines.

**Entry point:** `ModEntry` extends SMAPI's `Mod` base class. `Entry()` is called when the mod loads — it reads config, initializes the `Killer`, and wires up three SMAPI events:
- `Input.ButtonPressed` — toggles the killer on/off via a configurable keybind
- `Player.Warped` — logs when the player changes locations
- `GameLoop.OneSecondUpdateTicked` — runs the kill loop once per second

**Core logic:** `Killer` class. `KillMonsters()` runs each tick:
1. Only acts in `MineShaft`, `VolcanoDungeon`, and `Woods` locations
2. Iterates location NPCs, filters for `Monster` instances
3. Checks the monster is within the configured range (straight-line distance)
4. Calls `MakeWeak()` to strip armor from `Bug` and `RockCrab` enemies via SMAPI reflection
5. Deals damage via `location.damageMonster()`

**Config:** `Config` class with four properties:
- `Key` — toggle hotkey (default `J`)
- `IsEnable` — whether killing is active (default `true`)
- `Damage` — damage per tick (default `1000`)
- `Range` — kill radius in pixels (default `500`)

**Build system:** Uses `Pathoschild.Stardew.ModBuildConfig` NuGet package, which auto-packages the mod and copies output to the Stardew Valley Mods directory on build.

**Manifest:** `manifest.json` follows SMAPI's manifest format (`UniqueID: Jack_chai.MineKiller`, minimum API version 4.5.2).
