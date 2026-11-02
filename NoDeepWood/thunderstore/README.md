# NoDeepWood

This mod stops scrap wood spawning when you dig in last two biomes(or anywhere you configured).

## What changes

- Digging or blasting the `ROCK_3` and `ROCK_4` layers (75+ depth) no longer spawns scrap wood.
- This applies on both maps.

## What stays the same

- Ore veins: stone, iron, gold, emerald and diamond.
- Gold currency drops from digging.
- Wood from the upper three biomes.
- Wood from chests and wooden props.

## Multiplayer

Only the host needs this mod. Spawning is decided on the host, so it applies to everyone in that lobby. It does nothing if you join a host who doesn't have it.

## Configuration

`BepInEx/config/stonewards.nodeepwood.cfg`

| Setting | Default | Meaning |
| --- | --- | --- |
| `BlockedLayers` | `ROCK_3,ROCK_4` | Layers that no longer spawn wood. Top to bottom: `DIRT`, `ROCK_1`, `ROCK_2`, `ROCK_3`, `ROCK_4`. |
