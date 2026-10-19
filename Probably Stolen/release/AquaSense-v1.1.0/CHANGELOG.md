# AquaSense Changelog

## 1.1.0 — 2026-09-26

- Fixed the UV lamp (`uv_filter`, 紫外线灯) being mistaken for a depleted filter and swapped out for a water filter. The game's UV lamp identifier contains `filter`, so the previous name check treated it as a filter.
- Fixed the industrial cloth (`hydroreed_filter`, 韧棉织物) having the same problem. Its game description states it cannot be used for water purification, yet its identifier also contains `filter`.
- Added an explicit blocklist of non-filter items that share the water-container slot (`uv_filter`, `black_lamp`, `hydroreed`).
- Filter replacement now verifies that the item in the container slot really is a filter before considering a swap.
- Added a filter durability line to the water-container tooltip, showing the filter name and durability (for example `[滤水器]  耐久度:  140/400`), drawn in `#CCB987`.
- The durability line is shown even when the water container is empty.
- The durability line is only shown for real filters, not for other items in the slot.
- Added a bilingual in-game notice when a depleted filter is replaced.
- Added MelonPreferences switches for automatic filter replacement, durability display, and automatic label refresh. All default to on.
- Added post-swap verification: both filters must land where intended, otherwise a detached filter is returned to its original position so the filter slot never ends up empty.
- Fixed intermittent missed replacements at zero durability: a failed durability read is now treated as "depleted" instead of skipping the container.
- Fixed the tooltip durability line being skipped for empty containers.
- Fixed a full-width background band behind the durability line caused by passing a colour as `backgroundColor`.
- Changed the MelonPreferences category display name to `AquaSense`.
- Removed unused code (`GridInventoryInfo.TypeName` and its constructor parameter).

## 1.0.2 — 2026-09-22

- Added standard bottled water (`bottled_water`) to the recognized water containers.
- Added the game's original water-quality label after the purity title.
- Reused `WaterFeatureHelper.GetWaterDisplayName(GameItem)` instead of maintaining a separate label mapping.
- Finalized the bilingual release README and installation guide.
- Added the AquaSense pixel-art cover image and player-facing release notes.
- Rebuilt the 1.0.2 distribution archive with the final documentation set.

## 1.0.1 — 2026-09-22

- Fixed automatic water-label refresh running without a water-quality scanner.
- Automatic label refresh now requires the same scanner ownership check as the composition tooltip.

## 1.0.0 — 2026-09-22

- First public release under the AquaSense name.
- Added scanner-gated water composition tooltip.
- Added purity title formatting for Chinese and non-Chinese locales.
- Added `ml` composition amounts and hidden zero-percentage entries.
- Added automatic water-quality label refresh after liquid transfer.
- Added recursive traversal of nested player-owned containers.
- Added depleted-filter replacement with advanced, vanilla, and mod-filter priority.
- Added shape, boundary, and overlap checks before exchanging filters.
- Removed development diagnostics and normal runtime log output.
- Kept the stable low-frequency replacement poller at two seconds.
