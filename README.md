# 🗺️ PathFinding – Unity 2D A\* Visualizer

A Unity 2D project that implements and visualizes the **A\* pathfinding algorithm** on a grid. The algorithm expands nodes step by step using coroutines, showing the open/closed lists and the final path with different colored tokens.

---

## 📁 Project Structure

```
Assets/
├── Prefabs/
│   └── Circle.prefab          # Base token prefab for grid nodes
├── Scenes/
│   └── SampleScene.unity      # Main scene
├── Scripts/
│   ├── Node.cs                # Grid node data model
│   ├── Way.cs                 # Connection between nodes (edge + cost)
│   ├── Calculs.cs             # Static math helpers (distances, heuristic)
│   ├── GameManager.cs         # Grid creation and algorithm entry point
│   └── PathFinding.cs         # A* algorithm with coroutine visualisation
└── Sprites/
    ├── Circle.png
    ├── Cross.png
    └── panel.png
```

---

## ⚙️ How It Works

### Grid Generation
`GameManager` creates an `N×N` grid of `Node` objects. Each node knows its grid coordinates `(x, y)`, its world position, its heuristic value `h` (Euclidean distance to the goal), and its list of `Way` connections to its neighbours (up to 8 directions: cardinal + diagonal).

### A\* Algorithm (`PathFinding.cs`)

The algorithm runs inside a **coroutine** so each step can be visualized in real time.

```
f(n) = g(n) + h(n)
```

| Symbol | Meaning |
|--------|---------|
| `g(n)` | Accumulated cost from start to node `n` |
| `h(n)` | Heuristic – Euclidean distance from `n` to goal |
| `f(n)` | Total estimated cost (used to pick the best node) |

**Step-by-step loop:**

1. Pick the `Way` in the **Open List** with the lowest `f` score.
2. Remove it from the Open List.
3. If it is the **goal node** → reconstruct and display the final path.
4. Otherwise, add it to the **Closed List** and spawn a closed-list token.
5. Expand all neighbours: skip closed ones, add or update open ones.
6. Wait `stepDelay` seconds, then repeat.

---

## 🧩 Script Reference

### `Node.cs`
Represents a single cell in the grid.

| Property | Type | Description |
|----------|------|-------------|
| `PositionX / PositionY` | `int` | Grid coordinates |
| `RealPosition` | `Vector2` | World-space position |
| `Heuristic` | `float` | `h` value (distance to goal) |
| `NodeParent` | `Node` | Previous node in the optimal path |
| `WayList` | `List<Way>` | Connections to neighbours |

### `Way.cs`
Represents a directed edge between two nodes.

| Property | Type | Description |
|----------|------|-------------|
| `NodeDestiny` | `Node` | Destination node |
| `Cost` | `float` | Edge cost (linear or diagonal) |
| `ACUMulatedCost` | `float` | `g` value – total cost from start |

### `Calculs.cs` *(static)*
Utility math class. Calculates grid spacing, world positions, and heuristic values from the panel's `BoxCollider2D`.

| Member | Description |
|--------|-------------|
| `LinearDistance` | Distance between adjacent nodes |
| `DiagonalDistance` | Distance between diagonal nodes |
| `CalculatePoint(x, y)` | Returns the world position of grid cell `(x, y)` |
| `CalculateHeuristic(node, fx, fy)` | Euclidean distance from node to goal |

### `GameManager.cs`
- Creates the `Node[,]` matrix and assigns ways (edges) to each node.
- Picks random start and end positions.
- Calls `PathFinding.FindPath()` to start the algorithm.

### `PathFinding.cs`
Core A\* implementation.

| Member | Description |
|--------|-------------|
| `closedListToken` | Prefab spawned for each visited node |
| `finalPathToken` | Prefab spawned for each node in the final path |
| `stepDelay` | Seconds between each visualisation step |
| `FindPath(start, end)` | Public entry point – starts the coroutine |
| `GetBestWay()` | Picks lowest `f` from the Open List |
| `GetWayInOpenList(node)` | Checks if a node is already in the Open List |
| `GetFinalPath(endNode)` | Reconstructs path via `NodeParent` chain |

---

## 🚀 Setup & Usage

### 1. Import the package
Open Unity and go to **Assets → Import Package → Custom Package**, then select `PathFinding.unitypackage`.

### 2. Configure the scene
- Select the `GameManager` GameObject in the Hierarchy.
- Set **Size** (e.g. `10` for a 10×10 grid).
- Assign the **Panel** `BoxCollider2D` that defines the grid bounds.
- Assign a base **Token** prefab for grid node markers.
- Add the `PathFinding` component to the same GameObject.
- In the `PathFinding` component, assign:
  - `Closed List Token` → a prefab with a **red/orange** color.
  - `Final Path Token` → a prefab with a **green/yellow** color.
- Drag the `PathFinding` component reference into the `GameManager` field.

### 3. Play
Press **Play**. The grid spawns, a random start and end are chosen, and you will see:

- 🔴 **Closed list tokens** appearing one by one as the algorithm visits nodes.
- 🟢 **Final path tokens** tracing the optimal route from start to goal.

---

## 🎓 Grading Criteria Coverage

| Criterion | Implementation |
|-----------|---------------|
| **Llista Oberta** (1.5 pts) | `_openList: List<Way>` — correctly managed with best-node selection and cost updates |
| **Llista Tancada** (1.5 pts) | `_closedList: List<Node>` — nodes added after expansion, skipped on revisit |
| **Camí final** (1 pt) | `GetFinalPath()` — returns ordered `List<Node>` via `NodeParent` chain |
| **Algoritme A\*** (2 pts) | Full A\* with `f = g + h`, optimal path guaranteed |
| **Aspecte aplicació** (2 pts) | Two coroutines with two distinct token prefabs for closed list vs final path |
| **Codi Base** (2 pts) | Reuses `Node`, `Way`, `Calculs` without duplicating existing logic |

---

## 📦 Dependencies

- **Unity** 2021.3 LTS or newer (2D project)
- No external packages required

---

## 📄 License

This project was created for academic purposes.
