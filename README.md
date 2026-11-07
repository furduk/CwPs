# CwPs — Cockroach with Pockets in Space 🪳🚀

> **Disclaimer:** This README was written by **DeepSeek**, because the author was too lazy to write a giant wall of text himself. All the code, sweat, and 5 hours of suffering — that's his. 😄

![README - RU версия](README-ru.md)

A tiny 3D voxel interpreter / visualizer written in C# WinForms, where a heroic cockroach with pockets wanders through a 50×50×50 grid of bytes, executing a custom stack-based scripting language.

---

### The Code Editor
![Code screenshot](docs/screenshots/code.png)
*Write your script right inside the app.*

### The 3D Visualizer (layer view)
![Visualizer screenshot](docs/screenshots/visualizer.png)
*One Z-layer at a time, with the red cursor standing out.*

### 🪳 Dancing Cockroach
![Dancing cockroach](docs/screenshots/cockroach.gif)
*Why? Because why not.*

---

## ✨ What is this?

**CwPs** (Cockroach with Pockets in Space) is a small interpreter for a custom language that manipulates a 3-dimensional array of bytes (`byte[50, 50, 50]`). The interpreter is "driven" by a virtual cockroach — a 3D cursor that moves, rotates, and interacts with the cells it stands on.

Everything is stored in the cockroach's **pockets** — a LIFO stack of bytes. You can bring values into the pockets, drop them back into the world, do arithmetic with them, and control the cockroach with an if/jump system.

The visualizer shows a single horizontal slice (Z-layer) at a time, and you can scroll through layers with a trackbar. The cockroach itself is rendered as a red pixel so you don't lose it.

---

## 🧠 Features

- 🐛 **3D cursor** ("cockroach") with yaw/pitch-based movement
- 🎒 **Stack of pockets** (byte stack) with `bring` / `drop` / `padd` / `psub`
- 🏗️ **50×50×50 voxel world** (byte per cell, monochrome rendering)
- 🎨 **Layer visualizer** with adjustable Z slice
- 🧾 **Custom scripting language** with labels, jumps, conditions
- 🧪 **Debug commands** (`cpd`, `crd`, `get`, `chpk`) that dump info to the console
- 💥 **Fail-Fast philosophy** — bad input crashes instantly. No training wheels. 😎

---

## 🕹️ Language Reference

Each line is one command. Multiple spaces are ignored. Command names are case-insensitive.

### Rotation
| Command | Description |
|---------|-------------|
| `<`     | Rotate horizontal (yaw) left |
| `>`     | Rotate horizontal (yaw) right |
| `^`     | Look up (vertical, clamped to `+1`) |
| `v`     | Look down (vertical, clamped to `-1`) |

### Movement
| Command       | Description |
|---------------|-------------|
| `move`        | Move 1 step forward (with wrap-around) |
| `move <n>`    | Move `n` steps forward |

Vertical rotation (`^` / `v`) takes priority over horizontal. When `^` or `v` is set, `move` shifts along the Z axis instead of X/Y.

### Pockets (stack)
| Command    | Description |
|------------|-------------|
| `bring`    | Take cell's value into pockets and zero the cell |
| `drop`     | Write last pocket value into cell, then pop it |
| `padd`     | Add last pocket value into cell, then pop it |
| `psub`     | Subtract last pocket value from cell, then pop it |
| `pmusor`   | Pop (discard) the last pocket value |

### Cell operations
| Command          | Description |
|------------------|-------------|
| `set p`          | Set cell to last pocket value (peek) |
| `set <byte>`     | Set cell to a numeric value (0–255) |
| `incr`           | Increment cell |
| `decr`           | Decrement cell |

### Control flow
| Command              | Description |
|----------------------|-------------|
| `flag <name>`        | Mark the current line as a jump target |
| `jump <name>`        | Jump to a previously declared flag |
| `ifcell <op> <byte>` | Compare cell with value (`==`, `!=`, `>`, `>=`, `<`, `<=`) |
| `sct`                | Force condition = true |
| `scf`                | Force condition = false |
| `tcon <command>`     | Execute `<command>` if condition is true |
| `fcon <command>`     | Execute `<command>` if condition is false |

### Debug
| Command  | Description |
|----------|-------------|
| `cpd`    | Print cockroach position (X, Y, Z) |
| `crd`    | Print the current movement direction |
| `get`    | Print the value of the cell under the cockroach |
| `chpk`   | Print the last pocket value |

---

## 🚀 Getting Started

### Requirements
- Windows
- .NET Framework (WinForms)
- Visual Studio (or `msbuild`)

### Build
1. Clone the repo.
2. Open the `.sln` in Visual Studio.
3. Build & run (`F5`).

### Run
1. Paste your script into the code editor.
2. Click **CodeTransform** to run it.
3. Scroll the trackbar to browse layers.
4. Watch the output console for `get` / `cpd` / `crd` / `chpk` results.

---

## ⚠️ Known limitations (a.k.a. why it's Fail-Fast)

- Any malformed command → **exception**. That's the point. 🎉
- Accessing an empty pocket → **exception** (no seatbelts on this ride).
- Cell values wrap around silently (byte arithmetic).
- The `MaxX / MaxY / MaxZ` constants are capped low on purpose — a 1000³ world would eat **~1 GB of RAM**, and the author's PC only has 3 GB. Don't raise them unless you have the RAM to burn.
- Only the current Z-layer is rendered at a time. It's a visualizer, not a raytracer. 😅

---

## 🗺️ Roadmap (maybe)

- [ ] Non-fail-fast mode (if the cockroach ever learns to be careful)
- [ ] Full 3D rendering instead of a single Z-slice
- [ ] Save/load world state
- [ ] More commands (swap, conditional move, arithmetic on pockets...)
- [ ] The dancing cockroach gets its own menu

---

## License

MIT © 2026 furduk

---

## 🙏 Credits

- **Author:** the human who wrote this in 5 hours and now needs a break
- **README author:** DeepSeek (who was very confused why the cockroach dances, but didn't ask)


Special thanks to DeepSeek from author for writing this README

🪳 Happy scripting! 🪳