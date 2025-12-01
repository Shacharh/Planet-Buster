using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTT.BubbleShooter
{
	/// <summary>
	/// This class respresents a hexagon grid which holds <see cref="HexagonCell"/> instances.
	/// </summary>
	public class HexagonGrid
	{
		/// <summary>
		/// The Cells property is a collection of cells that is held by the grid.
		/// </summary>
		public HexagonCell[,] Cells { get; private set; }

		/// <summary>
		/// The Width property indicates the width of the grid.
		/// </summary>
		public int Width => Cells.GetLength(0);

		/// <summary>
		/// List of cell to pop.
		/// </summary>
		private List<HexagonCell> cellToPop;

		/// <summary>
		/// The Height property indicates the height of the grid, excluding empty rows.
		/// </summary>
		public int Height
        {
			get
            {
				IEnumerable<HexagonCell> filteredCells = Cells.Cast<HexagonCell>().Where(cell => cell.Node != null);
				if (!filteredCells.Any())
					return 0;

				return filteredCells.Max(cell => cell.Position.y) + 1;
			}
        }

		/// <summary>
		/// The RealHeight property indicates the height of the grid, including empty rows.
		/// </summary>
		public int RealHeight => Cells.GetLength(1);

		/// <summary>
		/// The _mode field is the mode used to properly get adjacent cells in a specific offset hexagon grid.
		/// </summary>
		internal HexagonRelativityMode i_mode;

		/// <summary>
		/// The delegate invoked upon generating a new random bubble.
		/// </summary>
		private Func<Bubble> _generateBubbleDelegate;

		/// <summary>
		/// The Updated event is invoked upon cell changes in the grid.
		/// </summary>
		public event Action<IEnumerable<HexagonCell>, HexagonRelativityMode, bool> Updated;

		/// <summary>
		/// The Attached event is invoked upon a new <see cref="Bubble"/> instance attaching to the grid.
		/// </summary>
		public event Action<Bubble, Vector2Int, bool, List<HexagonCell>> Attached;

		/// <summary>
		/// The _updateQueue field contains a queue of <see cref="HexagonCell"/> that have been updated since
		/// the last time the <see cref="Updated"/> event was invoked.
		/// </summary>
		private readonly Queue<HexagonCell> _updateQueue;

		/// <summary>
		/// A collection containing the default offset vectors when retrieving adjacent cells.
		/// </summary>
		private readonly Vector2Int[] DEFAULT_OFFSETS;

		/// <summary>
		/// Constructs a grid of new instances of cells.
		/// </summary>
		/// <param name="width">The width of the grid.</param>
		/// <param name="height">The height of the grid.</param>
		/// <param name="mode">The hexagon relativity of the grid.</param>
		/// <param name="generateBubbleDelegate">Delegate for generating bubble.</param>
		public HexagonGrid(int width, int height, HexagonRelativityMode mode, Func<Bubble> generateBubbleDelegate)
		{
			DEFAULT_OFFSETS = new Vector2Int[]
			{
				new Vector2Int(0, -1),
				new Vector2Int(1, 0),
				new Vector2Int(0, 1),
				new Vector2Int(-1, 0)
			};

			Cells = new HexagonCell[width, height];

			i_mode = mode;
			_generateBubbleDelegate = generateBubbleDelegate;
			_updateQueue = new Queue<HexagonCell>();

			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					Cells[x, y] = new HexagonCell(this, new Vector2Int(x, y));
		}

		/// <summary>
		/// The Clear methods clears all cells in the grid.
		/// </summary>
		public void Clear()
        {
			foreach (HexagonCell cell in Cells)
				cell.Node = null;

			ForceNotifyUpdate();
        }

		/// <summary>
		/// The GetAdjacentCells method retrieves all adjacent cells of a given position in the grid.
		/// </summary>
		/// <param name="position">The position to retrieve its adjacent cells from.</param>
		/// <returns>An array of <see cref="HexagonCell"/> instances surrounding the cell at the given position.</returns>
		public IEnumerable<HexagonCell> GetAdjacentCells(Vector2Int position)
		{
			List<HexagonCell> cells = new List<HexagonCell>();

			List<Vector2Int> offsets = new List<Vector2Int>(DEFAULT_OFFSETS);

			if(i_mode == HexagonRelativityMode.ODD_Q && position.x % 2 != 0 ||
				i_mode == HexagonRelativityMode.EVEN_Q && position.x % 2 == 0)
                offsets.AddRange(new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(1, 1) });

			if (i_mode == HexagonRelativityMode.ODD_Q && position.x % 2 == 0 ||
				i_mode == HexagonRelativityMode.EVEN_Q && position.x % 2 != 0)
				offsets.AddRange(new Vector2Int[] { new Vector2Int(-1, -1), new Vector2Int(1, -1) });

			if (i_mode == HexagonRelativityMode.ODD_R && position.y % 2 != 0 ||
				i_mode == HexagonRelativityMode.EVEN_R && position.y % 2 == 0)
				offsets.AddRange(new Vector2Int[] { new Vector2Int(1, -1), new Vector2Int(1, 1) });

			if (i_mode == HexagonRelativityMode.ODD_R && position.y % 2 == 0 ||
				i_mode == HexagonRelativityMode.EVEN_R && position.y % 2 != 0)
				offsets.AddRange(new Vector2Int[] { new Vector2Int(-1, -1), new Vector2Int(-1, 1) });

			foreach (Vector2Int offset in offsets)
			{
				HexagonCell cell = this[position + offset];
				if (cell == null) continue;

				cells.Add(cell);
			}

			return cells;
		}

		/// <summary>
		/// The GetAdjacentCellsRecursively recursively looks for adjacent cells as long as the adjacent cell meets
		/// the predicate function.
		/// </summary>
		/// <param name="position">The position to retrieve its adjacent cells from.</param>
		/// <param name="predicate">The predicate function that has to pass to continue the recursive process.</param>
		/// <param name="includeSelf">Whether to include the cell on the given position in the returned collection of cells.</param>
		/// <param name="excludedCells">The <see cref="HexagonCell"/> instance in the grid to exclude from the recusion
		/// to prevent a <see cref="StackOverflowException"/>. Leave empty if it is the first iteration of the call.</param>
		/// <returns>A collection of <see cref="HexagonCell"/> instances that represent adjecant cells.</returns>
		public IEnumerable<HexagonCell> GetAdjacentCellsRecursively(Vector2Int position, Func<HexagonCell, bool> predicate, bool includeSelf = false, List<HexagonCell> excludedCells = null)
		{
			HexagonCell currentCell = this[position];

			if(excludedCells != null && excludedCells.Contains(currentCell))
				return Enumerable.Empty<HexagonCell>();

			IEnumerable<HexagonCell> adjacentCells = GetAdjacentCells(position);

			if (excludedCells != null)
				adjacentCells = adjacentCells.Except(excludedCells);
			else
				excludedCells = new List<HexagonCell>();

			excludedCells.Add(currentCell);

			if (!adjacentCells.Any())
				return Enumerable.Empty<HexagonCell>();

			List<HexagonCell> cells = new List<HexagonCell>();

			if (includeSelf)
				cells.Add(this[position]);

			foreach (HexagonCell cell in adjacentCells)
			{
				if (excludedCells.Contains(cell))
					continue;

				if (cell.Node == null)
					continue;

				if (!predicate.Invoke(cell))
					continue;

				cells.Add(cell);
				cells.AddRange(GetAdjacentCellsRecursively(cell.Position, predicate, false, excludedCells));
			}

			return cells;
		}

		/// <summary>
		/// The Pop methods pops a potential <see cref="Bubble"/> at the given position of a <see cref="HexagonCell"/>.
		/// Additionally, it pops adjacent bubbles and left-over floating bubbles on the grid.
		/// </summary>
		/// <param name="position">The zero-based position of a cell to pop the bubble for.</param>
		internal void Pop(Vector2Int position)
		{
			// Pop the bubble group.
			HexagonCell centerCell = this[position];
			Bubble centerBubble = centerCell.Node;

			if (centerBubble == null)
				return;

			PopGroup(centerCell);

			// Pop potential floating bubbles
			List<HexagonCell> checkedCells = new List<HexagonCell>();
			IEnumerable<HexagonCell> cellsToCheck = Cells
				.Cast<HexagonCell>()
				.Where(cell => cell.Node != null && cell.Position.y != 0);
			foreach (HexagonCell cell in cellsToCheck)
			{
				if (checkedCells.Contains(cell))
					continue;

				IEnumerable<HexagonCell> cellGroup = GetAdjacentCellsRecursively(cell.Position, c => c.Node != null, true);

				checkedCells.Add(cell);
				checkedCells.AddRange(cellGroup);

				if (cellGroup.Any(c => c.Position.y == 0))
					continue;

				foreach (HexagonCell c in cellGroup)
				{
					PopSingle(c);
					cellToPop.Add(c);
				}
			}
		}

		/// <summary>
		/// The PopGroup method pops a potential <see cref="Bubble"/> at the given position of a <see cref="HexagonCell"/>.
		/// </summary>
		/// <param name="cell">The <see cref="HexagonCell"/> instance to pop the bubble and corresponding bubbles for.</param>
		private void PopGroup(HexagonCell cell)
		{
			Bubble bubble = cell.Node;

			if (bubble == null)
				return;

			PopSingle(cell);

			foreach (HexagonCell adjacentCell in GetAdjacentCellsRecursively(
				cell.Position,
				other => other.Node != null && bubble.IsMatch(other.Node)))
			{
				Bubble adjacentBubble = adjacentCell.Node;
				PopGroup(adjacentCell);
			}
		}

		/// <summary>
		/// The PopSingle method pops a bubble at the given cell without recursion.
		/// </summary>
		/// <param name="cell"></param>
		internal void PopSingle(HexagonCell cell)
		{
			Bubble bubble = cell.Node;

			if (bubble == null)
				return;

			bubble.Pop();
			cell.Node = null;

			_updateQueue.Enqueue(cell);
		}

		/// <summary>
		/// The NotifyUpdate method notifies listeners for the <see cref="Updated"/> event that the grid has been updated.
		/// </summary>
		/// <param name="instant">Whether to notify listeners of the event that it was an instant update or delayed.</param>
		public void NotifyUpdate(bool instant)
		{
			if(_updateQueue.Count == 0)
				return;

			IEnumerable<HexagonCell> distinctUpdateQueue = _updateQueue.Distinct();

			Updated?.Invoke(distinctUpdateQueue, i_mode, instant);
			_updateQueue.Clear();
		}

		/// <summary>
		/// The ForceNotifyUpdate method notifies listeners for the <see cref="Updated"/> event that the entire grid has been updated.
		/// </summary>
		public void ForceNotifyUpdate()
		{
			_updateQueue.Clear();
			foreach (HexagonCell cell in Cells)
				_updateQueue.Enqueue(cell);
			NotifyUpdate(true);
		}

		/// <summary>
		/// The Touch method tests if the cell on the given position has any adjacent cells with bubbles.
		/// </summary>
		/// <param name="position">The zero-based coordinates of a cell in the grid.</param>
		/// <returns>True, if the given cell has surrounding cells with bubbles. False, if otherwise.</returns>
		public bool Touch(Vector2Int position) => GetAdjacentCells(position).Any(cell => cell.Node != null);

		/// <summary>
		/// The Attach method attaches a bubble to a given <see cref="HexagonCell"/> in the grid and
		/// potentially initiates a pop.
		/// </summary>
		/// <param name="bubble">The <see cref="Bubble"/> instance to attach to the grid.</param>
		/// <param name="position">The zero-based coordinates of a cell in the grid.</param>
		public void Attach(Bubble bubble, Vector2Int position)
		{
			cellToPop = new List<HexagonCell>();
			HexagonCell cell = this[position];
			cell.Node = bubble;

			IEnumerable<HexagonCell> cellGroup = GetAdjacentCellsRecursively(position, other => other.Node != null && cell.Node.IsMatch(other.Node));
			IEnumerable<Bubble> bubbleGroup = cellGroup.Select(c => c.Node);
			cellToPop = cellToPop.Union(cellGroup).ToList();
			bool shouldPop = cellGroup.Count() >= 2 && bubble.IsConnectMatch(bubbleGroup);
			if (shouldPop)
				Pop(position);
			else
				_updateQueue.Enqueue(cell);

			Attached?.Invoke(bubble, position, shouldPop, cellToPop);

			NotifyUpdate(false);
		}

		/// <summary>
		/// The AddRow method adds a new row of randomly generated bubbles to the top of the grid.
		/// </summary>
		/// <returns>True if a row has been added. False if the grid was not able to add a row due to it reaching its threshold.</returns>
		public bool AddRow()
        {
			if (Height + 1 >= RealHeight)
				return false;

			for(int y = Height - 1; y >= 0; y--)
            {
				for(int x = 0; x < Width; x++)
                {
					HexagonCell fromCell = this[x, y];
					HexagonCell toCell = this[x, y + 1];

					toCell.Node = fromCell.Node;
                    _updateQueue.Enqueue(toCell);
                }
            }

			if(i_mode == HexagonRelativityMode.ODD_R || i_mode == HexagonRelativityMode.EVEN_R)
				i_mode = i_mode.GetOpposite();

			Populate(1);
			NotifyUpdate(true);

			return true;
        }

		/// <summary>
		/// The Populate method populates the given row with randomly generated bubbles.
		/// </summary>
		/// <param name="row">The row in the grid to populate.</param>
		internal void Populate(int row)
        {
			for (int x = 0; x < Width; x++) {
				HexagonCell cell = this[x, row - 1];
				cell.Node = _generateBubbleDelegate.Invoke();
				_updateQueue.Enqueue(cell);
			}
		}

		/// <summary>
		/// The indexer retrieves a <see cref="HexagonCell"/> instance from the grid by a given position.
		/// </summary>
		/// <param name="x">The zero-based x-coordinate of a position in the grid.</param>
		/// <param name="y">The zero-based y-coordinate of a position in the grid.</param>
		/// <returns>A <see cref="HexagonCell"/> instance positioned in the grid on the given position.</returns>
		public HexagonCell this[int x, int y]
		{
			get
			{
				if (x < 0 || x >= Width || y < 0 || y >= RealHeight)
					return null;

				return Cells[x, y];
			}
		}

		/// <summary>
		/// The indexer retrieves a <see cref="HexagonCell"/> instance from the grid by a given position.
		/// </summary>
		/// <param name="position">The zero-based coordinates of a position in the grid.</param>
		/// <returns>A <see cref="HexagonCell"/> instance positioned in the grid on the given position.</returns>
		public HexagonCell this[Vector2Int position] => this[position.x, position.y];
	}
}