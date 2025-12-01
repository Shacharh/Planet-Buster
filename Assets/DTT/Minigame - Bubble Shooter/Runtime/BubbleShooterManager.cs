using DTT.MinigameBase;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace DTT.BubbleShooter
{
    /// <summary>
    /// This class acts as the entry point for the Bubble Shooter minigame.
    /// </summary>
    public class BubbleShooterManager : MonoBehaviour, IMinigame<BubbleShooterConfig, BubbleShooterResult>
    {
        /// <summary>
        /// The Config property is a reference to the configuration passed through the inspector.
        /// </summary>
        public BubbleShooterConfig Config { get; private set; }

        /// <summary>
        /// The Grid property is the <see cref="HexagonGrid"/> instance that bubbles are placed on.
        /// </summary>
        public HexagonGrid Grid { get; private set; }

        /// <summary>
        /// The Pool property is referencing to an instance of a <see cref="BubblePool"/> that holds possible
        /// answers for popping bubbles.
        /// </summary>
        public BubblePool Pool { get; private set; }

        /// <summary>
        /// The Turret property is the <see cref="BubbleShooter.Turret"/> instance that is responsible for shooting bubbles.
        /// </summary>
        public Turret Turret { get; private set; }

        /// <summary>
        /// The _timer field is an instance of a stopwatch that keeps track of the time taken.
        /// </summary>
        private readonly Stopwatch _timer = new Stopwatch();

        /// <summary>
        /// The _isPaused field indicates whether the game has been paused or not.
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// The _isGameActive field indicates whether the game is currently active or not.
        /// </summary>
        private bool _isGameActive;

        /// <summary>
        /// The _initialTimeScale field is used to revert back to the normal time scale when resuming the game.
        /// </summary>
        private float _initialTimeScale;

        /// <summary>
        /// Score of the game.
        /// </summary>
        private int _score = 0;

        /// <summary>
        /// The TimeElapsed property is the time the bubble shooter game has been running for in seconds.
        /// </summary>
        public float TimeElapsed => _timer.ElapsedMilliseconds / 1000f;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsPaused => _isPaused;

        /// <summary>
        /// Score of the game.
        /// </summary>
        public int Score => _score;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsGameActive => _isGameActive;

        /// <summary>
        /// The Finish event is fired when the game is won and gives a result.
        /// </summary>
        public event System.Action<BubbleShooterResult> Finish;

        /// <summary>
        /// The Fail event is fired when the game is failed and gives a result.
        /// </summary>
        public event System.Action<BubbleShooterResult> Fail;

        /// <summary>
        /// The Initialized event is fired when the game's objects are initialized.
        /// This is called before <see cref="Started"/>.
        /// </summary>
        public event System.Action Initialized;

        /// <summary>
        /// The Started event is fired when the game is started.
        /// This is called after <see cref="Initialized"/>.
        /// </summary>
        public event System.Action Started;

        /// <summary>
        /// The Paused event is fired when the game is being paused.
        /// </summary>
        public event System.Action Paused;

        /// <summary>
        /// The Continued event is fired when the game is being unpaused.
        /// </summary>
        public event System.Action Continued;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Continue()
        {
            if (!_isGameActive)
                return;

            _isPaused = false;
            _timer.Start();

            Turret.canShoot = true;

            Time.timeScale = _initialTimeScale;

            Continued?.Invoke();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void ForceFinish()
        {
            if (!IsGameActive)
                return;

            _isGameActive = false;
            _isPaused = false;
            _timer.Stop();

            Turret.canShoot = false;

            float timeElapsed = TimeElapsed;
            int shotsFired = Turret.i_Shots;
            int amountOfMissedPops = Turret.i_totalMissedShots;
            bool hasWon = Grid.Height == 0;

            BubbleShooterResult results = new BubbleShooterResult(
                timeElapsed,
                shotsFired,
                amountOfMissedPops,
                hasWon,
                Score);

            if(hasWon)
                Finish?.Invoke(results);
            else
                Fail?.Invoke(results);
        }

        public void Stop()
        {
            _isGameActive = false;
            _isPaused = false;
            _timer.Stop();

            Time.timeScale = _initialTimeScale;

            Turret.canShoot = false;

            Turret.Reload(null);
            Grid.Clear();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Pause()
        {
            if (!_isGameActive)
                return;

            _isPaused = true;
            _timer.Stop();

            Turret.canShoot = false;

            _initialTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            Paused?.Invoke();
        }

        /// <summary>
        /// <inheritdoc>/>
        /// </summary>
        /// <param name="config">The configuration file to start the game.</param>
        public void StartGame(BubbleShooterConfig config)
        {
            if (IsGameActive)
                return;

            Config = config;

            _score = 0;
            _isPaused = false;
            _isGameActive = true;
            _timer.Start();

            _initialTimeScale = Time.timeScale;

            InitializeGame();

            Started?.Invoke();
        }

        /// <summary>
        /// The Restart method restarts the currently running game to its initial starting state.
        /// </summary>
        public void Restart()
        {
            _isGameActive = true;
            _isPaused = false;
            _timer.Restart();

            Time.timeScale = _initialTimeScale;

            InitializeGame();

            Started?.Invoke();
        }

        /// <summary>
        /// The InitializeGame method sets up necessary objects and event listeners.
        /// </summary>
        private void InitializeGame()
        {
            System.Func<Bubble> generateBubbleDelegate = () =>
            {
                Color randomBubbleColor = GetColor();

                if (Config.UseEducativeElement)
                    return new NumberedBubble(randomBubbleColor, Random.Range(Config.MinimumBubbleNumber, Config.MaximumBubbleNumber));
                else
                    return new ColoredBubble(randomBubbleColor);
            };
            Grid = new HexagonGrid(Config.GridWidth, Config.GridHeightThreshold, Config.RelativityMode, generateBubbleDelegate);

            Turret = new Turret();
            Grid.Attached += HandleBubbleAttachment;
            Grid.Updated += (cells, mode, animated) => CheckIfEndGame();

            float chanceToPopThreshold = Config.ChanceToPopThreshold;
            if (Config.UseEducativeElement)
                Pool = new NumberedBubblePool(Grid, chanceToPopThreshold);
            else
                Pool = new ColoredBubblePool(Grid, chanceToPopThreshold);

            Initialized?.Invoke();

            for (int y = 0; y < Config.InitialGridHeight; y++)
                Grid.Populate(y + 1);

            Pool.Recompute();
            Turret.Reload(Pool.PickBubble());
        }
        
        /// <summary>
        /// Choose a color for the bubble in the bubble pool.
        /// </summary>
        /// <returns>Return the color.</returns>
        private Color GetColor()
        {
            int totalValue = 0;
            int runningValue = 0;
            foreach (var weight in Config.ColorConfiguration)
                totalValue += weight.Weight;
            // if no weight are set up.
            if (totalValue == 0)
                return Config.ColorConfiguration[Random.Range(0, Config.ColorConfiguration.Length)].BubbleColors;

            // if weight are set up.
            int hitValue = Random.Range(1, totalValue + 1);
            for(int i=0;i<Config.ColorConfiguration.Length;i++)
            {
                runningValue += Config.ColorConfiguration[i].Weight;
                if (hitValue < runningValue)
                    return Config.ColorConfiguration[i].BubbleColors;
            }
            
            return Config.ColorConfiguration[Random.Range(0, Config.ColorConfiguration.Length)].BubbleColors;
        }

        /// <summary>
        /// The ShootTurret method notifies the <see cref="Turret"/> to shoot a bubble.
        /// </summary>
        /// <param name="direction">The direction to shoot the bubble at.</param>
        public void ShootTurret(Vector2 direction)
        {
            if (!IsGameActive)
                return;

            if (!Turret.canShoot)
                return;

            Turret.Shoot(direction);
        }

        /// <summary>
        /// The HandleBubbleAttachment method handles the game's flow upon a <see cref="Bubble"/> attaches to the grid.
        /// </summary>
        /// <param name="attachedBubble">The <see cref="Bubble"/> instance that attached to the grid.</param>
        /// <param name="position">The zero-based position of the bubble that attached to the grid.</param>
        /// <param name="didPop">Whether the bubble popped a group upon attaching to the grid.</param>
        /// <param name="popSize">The number of bubble pop.</param>
        private void HandleBubbleAttachment(Bubble attachedBubble, Vector2Int position, bool didPop,List<HexagonCell> toPop)
        {
            if (!didPop)
            {
                Turret.i_missedShots++;
                Turret.i_totalMissedShots++;
            }
            
            if(!didPop && position.y + 1 == Grid.RealHeight)
            {
                ForceFinish();
                return;
            }

            if (didPop)
            {
                int bonus = (int) (toPop.Count / 2) - 1;
                _score += toPop.Count * (10 + bonus * 5);
            }

            CheckForNewRow();

            Pool.Recompute();
            Turret.Reload(Pool.PickBubble());
        }

        /// <summary>
        /// The CheckForNewRow method checks if a new row should be added after a set amount of missed shots by the turret.
        /// </summary>
        private void CheckForNewRow()
        {
            if ((Turret.i_missedShots == 0 || Turret.i_missedShots % Config.MissedShotsTillNewRow != 0) 
                && (Turret.i_Shots == 0 || Turret.i_Shots % Config.ShotsTillNewRow !=0))
                return;

            if (Grid.AddRow())
            {
                Turret.i_missedShots = 0;
                return;
            }

            ForceFinish();
        }

        /// <summary>
        /// The CheckForEmptyGrid method checks if the grid is empty. If it is, the game will finish.
        /// </summary>
        private void CheckIfEndGame()
        {
            if (Grid.Height == 0)
                ForceFinish();
        }
    }
}