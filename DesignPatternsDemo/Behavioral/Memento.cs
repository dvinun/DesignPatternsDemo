using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Dvinun.DesignPatterns.Behavioral
{
    // Demonstration of memento design pattern by saving a game states and restoring it when needed.
    // This game plays by itself and scores/descores points and wins/looses when reaches to final level.
    // Through out the game play, it will use memento pattern to save/restore the game state when lives are lost 
    // and the recent game state will be restored after granted the new life.
    class Memento
    {
        // client - player
        public static class Gamer
        {
            // quick play
            readonly static int[] gamePlayTimeCycles = { 1000};

                // uncomment this for random delay between game actions
                //readonly static int[] gamePlayTimeCycles = { 10000, 5000, 20000, 15000, 25000 };

            public static void Play()
            {
                // New game played by Vinny
                Game game = new Game("Vinny");
                GameSaveSnapshots gameSaveSnapshots = new GameSaveSnapshots();
                game.Start();
                Thread.Sleep(getNextGamePlayTimeCycle());
                game.Pause();
                Thread.Sleep(getNextGamePlayTimeCycle());
                game.Resume();
                Thread.Sleep(getNextGamePlayTimeCycle());
                // Pause and exit the game
                GameStateMemento gameStateMemento = game.PauseAndExit();
                gameSaveSnapshots.SetSnapshot(gameStateMemento);
                Thread.Sleep(getNextGamePlayTimeCycle());

                // Resume old game by Sony.
                game = new Game("Sony", gameSaveSnapshots.GetSnapshot());
                game.Resume();
                Thread.Sleep(getNextGamePlayTimeCycle());
                game.Pause();
                Thread.Sleep(getNextGamePlayTimeCycle());
                // Terminate
                game.Exit();

                // New game played by Anny. The game will end itself without any interruptions
                game = new Game("Anny");
                game.Start();
                Console.ReadKey();
            }

            static int getNextGamePlayTimeCycle()
            {
                Random random = new Random();
                return gamePlayTimeCycles[random.Next(0, gamePlayTimeCycles.Length)];
            }

            public static void WriteGameLog(string log, object arg1 = null)
            {
                if (arg1 == null)
                    Console.WriteLine(DateTime.Now + ": " + log);
                else
                {
                    string strArg = arg1.ToString();
                    Console.WriteLine(DateTime.Now + ": " + (new StringBuilder().AppendFormat(log, strArg)).ToString());
                }
            }
        }

        // supporting class
        enum ActionIntent
        {
            Neutral,
            Score,
            Descore,
            Lose,
            LevelComplete
        }

        // supporting class
        class GameAction
        {
            public string Action { get; set; }
            public ActionIntent Intent { get; set; }
            public int Score { get; set; }

            public GameAction(string action, ActionIntent intent, int score)
            {
                this.Action = action;
                this.Intent = intent;
                this.Score = score;
            }
        }

        // client - host
        class Game
        {
            GameState gameState;
            GameSaver gameSaver;
            GameSaveSnapshots gameSaveSnapshots;

            readonly static int[] gameTimeCycles = { 1000, 2000, 3000, 4000, 5000 };
            //readonly static int[] gameTimeCycles = { 1000 };
            Random random_gameTimeCycles;

            Thread playThread;

            public Game()
            {
                gameState = new GameState(new Random().Next().ToString(), 1, 0, 0);

                gameSaver = new GameSaver();
                gameSaveSnapshots = new GameSaveSnapshots();
                random_gameTimeCycles = new Random();

                playThread = new Thread(new ThreadStart(play));
            }

            public Game(string playedBy) : this()
            {
                gameState.SetPlayedBy(playedBy);
            }

            public Game(string playedBy, GameStateMemento gameStateMemento) : this()
            {
                gameState = gameStateMemento.GetState();
                gameState.SetPlayedBy(playedBy);
            }

            void GameTimeCycle()
            {
                Thread.Sleep(gameTimeCycles[random_gameTimeCycles.Next(0, gameTimeCycles.Length)]);
            }

            public void Start()
            {
                gameState.SetStartTime(DateTime.Now);

                Gamer.WriteGameLog("Starting the game. Played by " + gameState.GetPlayedBy() + ".");
                GameTimeCycle();

                // Take the snapshot when the game is starting. Can be used when the
                // player loses in the first level.
                gameSaveSnapshots.SetSnapshot(gameSaver.SaveGame(gameState));

                playThread.Start();
            }

            void play()
            {
                try
                {
                    Gamer.WriteGameLog("Playing the game.");
                    GameTimeCycle();

                    while (!gameState.IsGameOver())
                    {
                        gameState.AdvanceGameAction();

                        if (gameState.IsCurrentActionLevelComplete())
                        {
                            // All good and level complete. Which means this level is won.
                            gameSaveSnapshots.SetSnapshot(gameSaver.SaveGame(gameState));
                        }
                        else if (gameState.IsCurrentActionLost())
                        {
                            if (gameState.AreLivesGone() == false)
                            {
                                int currentLife = gameState.GetCurrentLife();

                                Gamer.WriteGameLog("");
                                Gamer.WriteGameLog("Player lost life. Restoring the life and game.", gameState.GetCurrentLevel());
                                Gamer.WriteGameLog("Lives Used: " + gameState.GetLivesUsed() + ",Lives Left: " + gameState.GetLivesLeft());
                                Gamer.WriteGameLog("Playing the level: " + gameState.GetCurrentLevel());
                                Gamer.WriteGameLog("");

                                // Great. Restore the previous state
                                GameStateMemento gameStateMemento = gameSaveSnapshots.GetSnapshot();
                                gameState = gameStateMemento.GetState();
                                gameState.SetCurrentLife(currentLife);
                            }
                            else
                            {
                                // No lives left. Game over man.
                            }
                        }

                        GameTimeCycle();
                    }

                    gameState.SetEndTime(DateTime.Now);

                    displayGameEndSummary();
                }
                catch (ThreadAbortException exception)
                {
                    gameSaveSnapshots.SetSnapshot(gameSaver.SaveGame(gameState));
                }
            }

            void displayGameEndSummary()
            {
                Gamer.WriteGameLog("Game Ended.");
                Gamer.WriteGameLog("Played By: {0}", gameState.GetPlayedBy());
                Gamer.WriteGameLog("Result: {0}", gameState.GetResult());
                Gamer.WriteGameLog("Last Level Played: {0}", gameState.GetLastLevel());
                Gamer.WriteGameLog("Score: {0}", gameState.GetScore());
                Gamer.WriteGameLog("Info: {0}", gameState.GetInfo());
                Gamer.WriteGameLog("Start Time: {0}", gameState.GetStartTime());
                Gamer.WriteGameLog("End Time: {0}", gameState.GetEndTime());
            }

            public GameStateMemento PauseAndExit()
            {
                Gamer.WriteGameLog("Pausing and exiting the game...");
                playThread.Abort();
                return gameSaver.SaveGame(gameState);
            }

            public void Pause()
            {
                Gamer.WriteGameLog("Pausing the game...");
                playThread.Abort();
            }

            public void Resume()
            {
                Gamer.WriteGameLog("Resuming the game...");
                playThread = new Thread(new ThreadStart(play));

                // Take the snapshot when the game is starting. Can be used when the
                // player loses in the first level.
                gameSaveSnapshots.SetSnapshot(gameSaver.SaveGame(gameState));

                playThread.Start();
            }

            public void Exit()
            {
                Gamer.WriteGameLog("Exiting the game...");
                playThread.Abort();
            }
        }

        // core game state class
        class GameState
        {
            string info;
            DateTime startTime;
            DateTime endTime;
            string playedBy;
            GameAction currentAction;
            long score;

            readonly int totalLevels = 3;
            int currentLevel;
            int lastLevelPlayed;

            readonly int totalLives = 5;
            int currentLife;

            Random random_gameActionsLevel1;
            Random random_gameActionsLevel2;
            Random random_gameActionsLevel3;

            readonly static List<GameAction> gameActionsLevel1 = new List<GameAction>() {
                                        new GameAction("Intro to this world", ActionIntent.Neutral, 0),
                                        new GameAction("Practice dude... you got a big day ahead...", ActionIntent.Neutral, 0),
                                        new GameAction("Run run. This is not enough...", ActionIntent.Neutral, 0),
                                        new GameAction("Look at that guy. I will teach you how to catch him...", ActionIntent.Neutral, 0),
                                        new GameAction("Now I will teach you how to jump...", ActionIntent.Neutral, 0),
                                        new GameAction("Alright. Looks like you are all set. Now lets go....", ActionIntent.Neutral, 0),
                                        new GameAction("Now shoot him...", ActionIntent.Neutral, 0),
                                        new GameAction("Shoot it now...", ActionIntent.Score, 10),
                                        new GameAction("Hey, dont shoot me...", ActionIntent.Neutral, 0),
                                        new GameAction("Catch them all...", ActionIntent.Neutral, 0),
                                        new GameAction("You are bunked...", ActionIntent.Lose, 0),
                                        new GameAction("You are dead...", ActionIntent.Lose, 0),
                                        new GameAction("Thats the spirit...", ActionIntent.Score, 5),
                                        new GameAction("Way to go...", ActionIntent.Score, 10),
                                        new GameAction("Great dude. You got moves...", ActionIntent.Score, 20),
                                        new GameAction("Oh.. Look at you. Handsome...", ActionIntent.Score, 15),
                                        new GameAction("Oh.. Shit!...", ActionIntent.Descore, 10),
                                        new GameAction("Lol! Thats stupidity.", ActionIntent.Descore, 5),
                                        new GameAction("I am sorry! That was wrong...", ActionIntent.Descore, 10),
                                        new GameAction("Yuk! Thats smells gross!", ActionIntent.Descore, 15),
                                        new GameAction("No. Thats the wrong way.", ActionIntent.Descore, 20),
                                        new GameAction("You should work on you skils man.", ActionIntent.Descore, 5),
                                        new GameAction("Alright. Now go home.", ActionIntent.Descore, 25),
                                        new GameAction("You nailed it...", ActionIntent.LevelComplete, 100),
                                        new GameAction("You did it...", ActionIntent.LevelComplete, 100)
        };


            readonly static List<GameAction> gameActionsLevel2 = new List<GameAction>() {
                                        new GameAction("Ok. Now take these bitches down...", ActionIntent.Neutral, 0),
                                        new GameAction("Alright. You are pro now...", ActionIntent.Score, 5),
                                        new GameAction("Goddamn! You need some more nerves man", ActionIntent.Lose, 0),
                                        new GameAction("Careful. Dont repeat this.", ActionIntent.Descore, 10),
                                        new GameAction("Hey! You. Wanna date with me.", ActionIntent.Score, 15),
                                        new GameAction("He aint a loser", ActionIntent.LevelComplete, 100),
                                        new GameAction("Now shoot him...", ActionIntent.Neutral, 0),
                                        new GameAction("Shoot it now...", ActionIntent.Score, 20),
                                        new GameAction("Hey, dont shoot me...", ActionIntent.Neutral, 0),
                                        new GameAction("Catch them all...", ActionIntent.Neutral, 0),
                                        new GameAction("You are bunked...", ActionIntent.Lose, 0),
                                        new GameAction("You are dead...", ActionIntent.Lose, 0),
                                        new GameAction("Thats the spirit...", ActionIntent.Score, 10),
                                        new GameAction("Way to go...", ActionIntent.Score, 20),
                                        new GameAction("Great dude. You got moves...", ActionIntent.Score, 15),
                                        new GameAction("Oh.. Look at you. Handsome...", ActionIntent.Score, 5),
                                        new GameAction("You nailed it...", ActionIntent.LevelComplete, 100),
                                        new GameAction("I am sorry! That was wrong...", ActionIntent.Descore, 10),
                                        new GameAction("Yuk! Thats smells gross!", ActionIntent.Descore, 15),
                                        new GameAction("No. Thats the wrong way.", ActionIntent.Descore, 20),
                                        new GameAction("You should work on you skils man.", ActionIntent.Descore, 5),
                                        new GameAction("You did it...", ActionIntent.LevelComplete, 100),
                                        new GameAction("Very close.. comeon. Try again...", ActionIntent.Lose, 0)
        };

            readonly static List<GameAction> gameActionsLevel3 = new List<GameAction>() {
                                        new GameAction("Ok. Now things will get real. Here you go...", ActionIntent.Neutral, 0),
                                        new GameAction("Comeon man! You got to be careful.", ActionIntent.Descore, 5),
                                        new GameAction("Sweet. There you go.", ActionIntent.Score, 10),
                                        new GameAction("Hey dude. How you are so smart...", ActionIntent.Score, 10),
                                        new GameAction("Can you do this task for me?", ActionIntent.Neutral, 0),
                                        new GameAction("You got it man. I am very happy. Thank you so much for saving my life", ActionIntent.LevelComplete, 100),
                                        new GameAction("Final countdown begin...", ActionIntent.Neutral, 0),
                                        new GameAction("Oh! Sorry. Try again.", ActionIntent.Lose, 15),
                                        new GameAction("Well! Someone got to be kidding.", ActionIntent.Score, 10),
                                        new GameAction("You are the beast", ActionIntent.Score, 10),
                                        new GameAction("Alright... off you go...", ActionIntent.Neutral, 0),
                                        new GameAction("Great going so far. Keep it going...", ActionIntent.Neutral, 0),
                                        new GameAction("Thats the spirit...", ActionIntent.Score, 10),
                                        new GameAction("Way to go...", ActionIntent.Score, 10),
                                        new GameAction("Great dude. You got moves...", ActionIntent.Score, 5),
                                        new GameAction("Oh.. Look at you. Handsome...", ActionIntent.Score, 15),
                                        new GameAction("You nailed it...", ActionIntent.LevelComplete, 100),
                                        new GameAction("You did it...", ActionIntent.LevelComplete, 100),
                                        new GameAction("You nailed it...", ActionIntent.LevelComplete, 100),
                                        new GameAction("I am sorry! That was wrong...", ActionIntent.Descore, 10),
                                        new GameAction("Yuk! Thats smells gross!", ActionIntent.Descore, 15),
                                        new GameAction("No. Thats the wrong way.", ActionIntent.Descore, 20),
                                        new GameAction("You should work on you skils man.", ActionIntent.Descore, 5),
                                        new GameAction("Very close.. comeon. Try again...", ActionIntent.Lose, 0)
        };

            public GameState()
            {
                random_gameActionsLevel1 = new Random();
                random_gameActionsLevel2 = new Random();
                random_gameActionsLevel3 = new Random();
            }

            public GameState(string info, int level, int life, int score) : this()
            {
                this.info = info;
                this.currentLevel = level;
                this.lastLevelPlayed = level;
                this.currentLife = life;
                this.score = score;
            }

            internal string GetPlayedBy()
            {
                return playedBy;
            }

            internal long GetScore()
            {
                return score;
            }

            internal string GetInfo()
            {
                return info;
            }

            internal DateTime GetStartTime()
            {
                return startTime;
            }

            internal DateTime GetEndTime()
            {
                return endTime;
            }

            internal void SetPlayedBy(string playedBy)
            {
                this.playedBy = playedBy;
            }

            //internal void SetInfo(string info)
            //{
            //    this.info = info;
            //}

            //internal void SetCurrentLevel(int currentLevel)
            //{
            //    this.lastLevelPlayed = this.currentLevel;
            //    this.currentLevel = currentLevel;
            //}

            //internal void SetScore(int score)
            //{
            //    this.score = score;
            //}

            internal int GetCurrentLevel()
            {
                return currentLevel;
            }

            internal object GetLastLevel()
            {
                return lastLevelPlayed;
            }

            internal void SetStartTime(DateTime startTime)
            {
                this.startTime = startTime;
            }

            internal void AdvanceGameAction()
            {
                // dont do anything if game is over
                if (IsGameOver()) return;

                currentAction = getNextGameAction();

                // advance to next level
                if (currentAction.Intent == ActionIntent.LevelComplete)
                {
                    score += currentAction.Score;

                    DisplayCurrentGameAction("+");

                    Gamer.WriteGameLog("");
                    Gamer.WriteGameLog("**** Level {0} Complete ****", currentLevel);
                    Gamer.WriteGameLog("");

                    lastLevelPlayed = currentLevel;
                    currentLevel++;
                }
                else if (currentAction.Intent == ActionIntent.Descore)
                {
                    // deduct the score
                    score -= currentAction.Score;
                    DisplayCurrentGameAction("-");
                }
                else if (currentAction.Intent == ActionIntent.Score)
                {
                    // count the score
                    score += currentAction.Score;
                    DisplayCurrentGameAction("+");
                }
                else if (currentAction.Intent == ActionIntent.Lose)
                {
                    // increment life
                    currentLife++;
                    DisplayCurrentGameAction("");
                }
            }

            GameAction getNextGameAction()
            {
                if (currentLevel == 1) return gameActionsLevel1[random_gameActionsLevel1.Next(0, gameActionsLevel1.Count)];
                if (currentLevel == 2) return gameActionsLevel2[random_gameActionsLevel2.Next(0, gameActionsLevel2.Count)];
                if (currentLevel == 3) return gameActionsLevel3[random_gameActionsLevel3.Next(0, gameActionsLevel3.Count)];
                return null;
            }

            internal void DisplayCurrentGameAction(string scorePrefix)
            {
                Gamer.WriteGameLog("");
                Gamer.WriteGameLog("=== Current Game Action ===");
                Gamer.WriteGameLog("Level: " + currentLevel);
                Gamer.WriteGameLog("Player: " + playedBy);
                Gamer.WriteGameLog("Action: " + currentAction.Action);
                Gamer.WriteGameLog("Intent: " + currentAction.Intent.ToString());
                Gamer.WriteGameLog("Score: " + scorePrefix + currentAction.Score.ToString());
                Gamer.WriteGameLog("Total Score: " + score);
                Gamer.WriteGameLog("===========================");
                Gamer.WriteGameLog("");
            }

            internal bool IsGameOver()
            {
                return AreAllLevelsComplete() || AreLivesGone();
            }

            internal bool IsCurrentActionLevelComplete()
            {
                return currentAction.Intent == ActionIntent.LevelComplete;
            }

            internal void SetCurrentLife(int currentLife)
            {
                this.currentLife = currentLife;
            }

            internal bool IsCurrentActionLost()
            {
                return currentAction.Intent == ActionIntent.Lose;
            }

            internal bool AreLivesGone()
            {
                return currentLife > totalLives;
            }

            internal bool AreAllLevelsComplete()
            {
                return currentLevel > totalLevels;
            }

            internal int GetCurrentLife()
            {
                return currentLife;
            }

            internal string GetResult()
            {
                if (AreAllLevelsComplete())
                    return "All levels complete and game won.";
                else if (AreLivesGone())
                    return "Ran out of lives and game lost.";
                else return "Game is still in progress.";
            }

            internal int GetLivesLeft()
            {
                return totalLives - currentLife;
            }

            internal int GetLivesUsed()
            {
                return currentLife;
            }

            internal void SetEndTime(DateTime endTime)
            {
                this.endTime = endTime;
            }
        }

        // memento
        class GameStateMemento
        {
            GameState state;
            public GameStateMemento(GameState gameState) { state = gameState; }
            public GameState GetState() { return state; }
        }

        // originator
        class GameSaver
        {
            public GameStateMemento SaveGame(GameState state)
            {
                return new GameStateMemento(state);
            }
        }

        // care taker
        class GameSaveSnapshots
        {
            GameStateMemento gameStateMemento;

            public void SetSnapshot(GameStateMemento gameStateMemento)
            {
                this.gameStateMemento = gameStateMemento;
            }

            public GameStateMemento GetSnapshot()
            {
                return gameStateMemento;
            }
        }
    }
}
