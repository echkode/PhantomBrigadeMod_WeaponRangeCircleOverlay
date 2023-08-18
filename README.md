# WeaponRangeCircleOverlay

A library mod for [Phantom Brigade](https://braceyourselfgames.com/phantom-brigade/) that demonstrates how to add a custom range overlay.

It is compatible with game version **1.1.2-b5993** (Epic/Steam).

Range overlays are extra graphical details that appear when you order an attack action. There's a range circle overlay that gives the player some idea of the effective range of the selected weapon when placing the action on the timeline and a reticule overlay when selecting the target unit.

<video controls src="https://github.com/echkode/PhantomBrigadeMod_WeaponRangeCircleOverlay/assets/48565771/454e4f71-6157-49e9-99cb-10007ae0bf62">
  <p>Range circle and reticule overlays when placing an attack action.</p>
</video>

Hovering over an attack action or dragging the action will cause the reticule overlay to appear but not the range circle. This mod makes the range circle overlay appear on drag in addition to the reticule overlay so that it's similar to how it looks when placing the attack action.

<video controls src="https://github.com/echkode/PhantomBrigadeMod_WeaponRangeCircleOverlay/assets/48565771/df95cdc5-bb06-42c6-8577-e527f6aa43c5">
  <p>Range circle and reticule overlays when dragging an attack action with this mod enabled.</p>
</video>

I chose to show the range circle overlay because it's built-in and thus keeps the code short for demonstration purposes. However, you could add your own custom overlay instead. In fact, you can stack up any number of custom overlays.

The key to showing your own custom overlays is the map of `rangeLink` instances in the `WorldUICombat` class. Each overlay is keyed by a unique ID and it's best to use small negative numbers to avoid potentional collisions with IDs that the game uses.
