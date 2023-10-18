using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Helper;
using Animation2D;
using System.IO;

namespace Minesweeper
{
    public class Game1 : Game
    {
        static Random rng = new Random();

        static StreamWriter outFile = null;
        static StreamReader inFile = null;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        const int LOSE = 0;
        const int WIN = 1;
        const int PLAYING = 2;

        const int EASY = 0;
        const int MEDIUM = 1;
        const int HARD = 2;

        const int EASY_ROWS = 8;
        const int EASY_COLS = 10;
        const int TILE_SIZE_E = 45;
        const int HUD_HEIGHT = 60;
        const int EASY_MINES = 10;

        const int MED_ROWS = 14;
        const int MED_COLS = 18;
        const int TILE_SIZE_M = 30;
        const int MED_MINES = 40;

        const int HARD_ROWS = 20;
        const int HARD_COLS = 24;
        const int TILE_SIZE_H = 25;
        const int HARD_MINES = 99;

        const string MAX_TIME = "999";

        MouseState mouse;
        MouseState prevMouse;

        Texture2D hud;
        Texture2D[] minesImg = new Texture2D[8];
        Texture2D[] numbersImg = new Texture2D[8];
        Texture2D[] blanksImg = new Texture2D[2];
        Texture2D[] gameOverImg = new Texture2D[2];
        Texture2D[] playAgainImg = new Texture2D[2];
        Texture2D[] currBoardImg = new Texture2D[3];
        Texture2D[] currButtonImg = new Texture2D[3];
        Texture2D instructionsImg;
        Texture2D dropDownImg;
        Texture2D flagImg;
        Texture2D watchImg;
        Texture2D boardShadowImg;
        Texture2D explosionImg;
        Texture2D exitImg;
        Texture2D volumeOnImg;
        Texture2D volumeOffImg;
        Texture2D noTimeImg;
        Texture2D checkImg;

        Rectangle[] currButtonRec = new Rectangle[3];
        Rectangle[] currBoardRec = new Rectangle[3];
        Rectangle[] buttonRec = new Rectangle[3];
        Rectangle hudRec;
        Rectangle watchRec;
        Rectangle gameOverRec;
        Rectangle playAgainRec;
        Rectangle dropDownRec;
        Rectangle flagRec;
        Rectangle exitRec;
        Rectangle volumeRec;
        Rectangle boardShadowRec;
        Rectangle[,] tileShape;
        Rectangle noTimeRec;
        Rectangle noBestTimeRec;
        Rectangle checkRec;

        Vector2 currBoardLoc = new Vector2(0, HUD_HEIGHT);
        Vector2 buttonLoc = new Vector2(15, 15);
        Vector2 dropDownLoc = new Vector2(15, 45);
        Vector2 easyButtonLoc = new Vector2(15, 0);
        Vector2 mediumButtonLoc = new Vector2(15, 0);
        Vector2 hardButtonLoc = new Vector2(15, 0);
        Vector2 explosionPos = new Vector2(0, 0);
        Vector2 instructionsLoc = new Vector2(0, 0);

        Vector2 playAgainLoc;
        Vector2 gameOverLoc;
        Vector2 flagCountLoc;
        Vector2 infiniteTimerLoc;
        Vector2 watchLoc;
        Vector2 flagLoc;
        Vector2 volumeLoc;
        Vector2 exitLoc;
        Vector2 currTimeLoc;
        Vector2 bestTimeLoc;
        Vector2 checkLoc;

        Song loseMusic;
        Song winMusic;

        SoundEffect mineSnd;
        SoundEffect largeClearSnd;
        SoundEffect clearFlagSnd;
        SoundEffect smallClearSnd;
        SoundEffect placeFlagSnd;

        SpriteFont flagCountFont;
        SpriteFont timerFont;
        SpriteFont scoreTimeFont;

        Timer infiniteTimer;

        Animation explosionAnim;
        Animation instructionsAnim;


        bool areInstructionsAnimating = true;
        bool isVolumeOn = true;
        int gameState;
        bool displayLevels;
        bool isTimerActivated;
        bool gameLost;
        bool gameWon;
        string flagCount;
        string timerOutput = "";
        string newTimerOutput = "";

        int currDifficulty;
        int currRows;
        int currColumns;
        int currTileSize;
        int flagsAmount;
        int minesLocated;
        int screenWidth;
        int screenHeight;
        int tileRevealedCounter;
        string[] bestTimeScore = new string[3]; 

        Tile[,] tiles;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        protected override void Initialize()
        {
            ResetBoard(EASY_MINES, EASY, EASY_ROWS, EASY_COLS, TILE_SIZE_E);

            base.Initialize();
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            explosionImg = Content.Load<Texture2D>("Images/Sprites/explode");
            instructionsImg = Content.Load<Texture2D>("Images/Sprites/Instructions");

            mineSnd = Content.Load<SoundEffect>("Audio/Sounds/Mine");
            placeFlagSnd = Content.Load<SoundEffect>("Audio/Sounds/PlaceFlag");
            clearFlagSnd = Content.Load<SoundEffect>("Audio/Sounds/ClearFlag");
            smallClearSnd = Content.Load<SoundEffect>("Audio/Sounds/SmallClear");
            largeClearSnd = Content.Load<SoundEffect>("Audio/Sounds/LargeClear");

            SoundEffect.MasterVolume = 0.7f;

            winMusic = Content.Load<Song>("Audio/Music/Win");
            loseMusic = Content.Load<Song>("Audio/Music/Lose");

            MediaPlayer.Volume = 0.8f;
            MediaPlayer.IsRepeating = true;

            currBoardImg[EASY] = Content.Load<Texture2D>("Images/Backgrounds/board_easy");
            currBoardImg[MEDIUM] = Content.Load<Texture2D>("Images/Backgrounds/board_med");
            currBoardImg[HARD] = Content.Load<Texture2D>("Images/Backgrounds/board_hard");

            hud = Content.Load<Texture2D>("Images/Sprites/HUDBar");
            flagImg = Content.Load<Texture2D>("Images/Sprites/flag");
            watchImg = Content.Load<Texture2D>("Images/Sprites/Watch");
            dropDownImg = Content.Load<Texture2D>("Images/Sprites/DropDown");
            checkImg = Content.Load<Texture2D>("Images/Sprites/Check");
            volumeOnImg = Content.Load<Texture2D>("Images/Sprites/SoundOn");
            volumeOffImg = Content.Load<Texture2D>("Images/Sprites/SoundOff");
            exitImg = Content.Load<Texture2D>("Images/Sprites/Exit");
            noTimeImg = Content.Load<Texture2D>("Images/Sprites/GameOver_NoTime");
            boardShadowImg = Content.Load<Texture2D>("Images/Sprites/GameOverBoardShadow");

            currButtonImg[EASY] = Content.Load<Texture2D>("Images/Sprites/EasyButton");
            currButtonImg[MEDIUM] = Content.Load<Texture2D>("Images/Sprites/MedButton");
            currButtonImg[HARD] = Content.Load<Texture2D>("Images/Sprites/HardButton");

            blanksImg[0] = Content.Load<Texture2D>("Images/Sprites/Clear_Dark");
            blanksImg[1] = Content.Load<Texture2D>("Images/Sprites/Clear_Light");

            gameOverImg[0] = Content.Load<Texture2D>("Images/Sprites/GameOver_Results");
            gameOverImg[1] = Content.Load<Texture2D>("Images/Sprites/GameOver_WinResults");

            playAgainImg[0] = Content.Load<Texture2D>("Images/Sprites/GameOver_TryAgain");
            playAgainImg[1] = Content.Load<Texture2D>("Images/Sprites/GameOver_PlayAgain");

            minesImg[0] = Content.Load<Texture2D>("Images/Sprites/Mine1");
            minesImg[1] = Content.Load<Texture2D>("Images/Sprites/Mine2");
            minesImg[2] = Content.Load<Texture2D>("Images/Sprites/Mine3");
            minesImg[3] = Content.Load<Texture2D>("Images/Sprites/Mine4");
            minesImg[4] = Content.Load<Texture2D>("Images/Sprites/Mine5");
            minesImg[5] = Content.Load<Texture2D>("Images/Sprites/Mine6");
            minesImg[6] = Content.Load<Texture2D>("Images/Sprites/Mine7");
            minesImg[7] = Content.Load<Texture2D>("Images/Sprites/Mine8");

            numbersImg[0] = Content.Load<Texture2D>("Images/Sprites/1");
            numbersImg[1] = Content.Load<Texture2D>("Images/Sprites/2");
            numbersImg[2] = Content.Load<Texture2D>("Images/Sprites/3");
            numbersImg[3] = Content.Load<Texture2D>("Images/Sprites/4");
            numbersImg[4] = Content.Load<Texture2D>("Images/Sprites/5");
            numbersImg[5] = Content.Load<Texture2D>("Images/Sprites/6");
            numbersImg[6] = Content.Load<Texture2D>("Images/Sprites/7");
            numbersImg[7] = Content.Load<Texture2D>("Images/Sprites/8");

            watchLoc.X = currBoardImg[currDifficulty].Width / 2 - watchImg.Width / 2 * 1 / 3;
            watchLoc.Y = HUD_HEIGHT / 2 - watchImg.Height / 2 * 1 / 3;

            flagCountLoc.X = watchLoc.X - 45;
            flagCountLoc.Y = watchLoc.Y;

            flagLoc.X = flagCountLoc.X - 45;
            flagLoc.Y = watchLoc.Y;

            exitLoc.X = currBoardImg[currDifficulty].Width - 45;
            exitLoc.Y = watchLoc.Y;

            volumeLoc.X = exitLoc.X - 45;
            volumeLoc.Y = watchLoc.Y;

            gameOverLoc.X = currBoardImg[currDifficulty].Width / 2 - gameOverImg[0].Width / 2;
            gameOverLoc.Y = HUD_HEIGHT;

            playAgainLoc.X = gameOverLoc.X;
            playAgainLoc.Y = gameOverLoc.Y + gameOverImg[0].Height + 10;
            
            bestTimeLoc.X = currBoardImg[currDifficulty].Width / 2 + gameOverImg[0].Width * 1/6;
            bestTimeLoc.Y = currBoardImg[currDifficulty].Height / 2 - 20;

            currTimeLoc.X = currBoardImg[currDifficulty].Width / 2 - gameOverImg[0].Width * 1 / 3;
            currTimeLoc.Y = currBoardImg[currDifficulty].Height / 2 - 20;

            dropDownLoc.Y = buttonLoc.Y + currButtonImg[HARD].Height;

            easyButtonLoc.Y = dropDownLoc.Y;
            mediumButtonLoc.Y = dropDownLoc.Y + dropDownImg.Height * 1/3;
            hardButtonLoc.Y = dropDownLoc.Y + dropDownImg.Height * 2/3;

            infiniteTimerLoc.X = currBoardImg[currDifficulty].Width / 2 + watchImg.Width / 3;
            infiniteTimerLoc.Y = watchLoc.Y;

            hudRec = new Rectangle(0, 0,
                currBoardImg[currDifficulty].Width, HUD_HEIGHT);
            watchRec = new Rectangle((int)watchLoc.X,
                (int)watchLoc.Y, watchImg.Width / 3, watchImg.Height / 3);
            flagRec = new Rectangle((int)flagLoc.X,
                (int)flagLoc.Y, flagImg.Width / 3, flagImg.Height / 3);
            volumeRec = new Rectangle((int)volumeLoc.X,
               (int)volumeLoc.Y, volumeOnImg.Width, volumeOnImg.Height);
            exitRec = new Rectangle((int)exitLoc.X,
               (int)exitLoc.Y, exitImg.Width / 3, exitImg.Height / 3);

            currBoardRec[EASY] = new Rectangle((int)currBoardLoc.X,
                (int)currBoardLoc.Y, currBoardImg[EASY].Width, currBoardImg[EASY].Height);
            currBoardRec[MEDIUM] = new Rectangle((int)currBoardLoc.X,
                (int)currBoardLoc.Y, currBoardImg[MEDIUM].Width, currBoardImg[MEDIUM].Height);
            currBoardRec[HARD] = new Rectangle((int)currBoardLoc.X,
                (int)currBoardLoc.Y, currBoardImg[HARD].Width, currBoardImg[HARD].Height);

            currButtonRec[EASY] = new Rectangle((int)buttonLoc.X,
                (int)buttonLoc.Y, currButtonImg[EASY].Width, currButtonImg[EASY].Height);
            currButtonRec[MEDIUM] = new Rectangle((int)buttonLoc.X,
                (int)buttonLoc.Y, currButtonImg[MEDIUM].Width, currButtonImg[MEDIUM].Height);
            currButtonRec[HARD] = new Rectangle((int)buttonLoc.X,
                (int)buttonLoc.Y, currButtonImg[HARD].Width, currButtonImg[HARD].Height);

            dropDownRec = new Rectangle((int)dropDownLoc.X,
                (int)dropDownLoc.Y, dropDownImg.Width, dropDownImg.Height);

            buttonRec[EASY] = new Rectangle((int)easyButtonLoc.X,
                (int)easyButtonLoc.Y, dropDownImg.Width, dropDownImg.Height * 1/3);
            buttonRec[MEDIUM] = new Rectangle((int)mediumButtonLoc.X,
                (int)mediumButtonLoc.Y, dropDownImg.Width, dropDownImg.Height * 1/3);
            buttonRec[HARD] = new Rectangle((int)hardButtonLoc.X,
                (int)hardButtonLoc.Y, dropDownImg.Width, dropDownImg.Height * 1/3);

            checkLoc.X = buttonRec[currDifficulty].X + 5;
            checkLoc.Y = buttonRec[currDifficulty].Y + checkImg.Height/2;

            checkRec = new Rectangle((int)checkLoc.X,
                (int)checkLoc.Y, checkImg.Width, checkImg.Height);

            gameOverRec = new Rectangle((int)gameOverLoc.X,
                (int)gameOverLoc.Y, gameOverImg[0].Width, gameOverImg[0].Height);
            playAgainRec = new Rectangle((int)playAgainLoc.X,
                (int)playAgainLoc.Y, gameOverImg[0].Width, playAgainImg[0].Height);
            boardShadowRec = new Rectangle(0, 0, currBoardImg[currDifficulty].Width,
                currBoardImg[currDifficulty].Height + HUD_HEIGHT);

            noTimeRec = new Rectangle((int)currTimeLoc.X,
                (int)currTimeLoc.Y, noTimeImg.Width, noTimeImg.Height);
            noBestTimeRec = new Rectangle((int)bestTimeLoc.X,
                (int)bestTimeLoc.Y, noTimeImg.Width, noTimeImg.Height);

            flagCountFont = Content.Load<SpriteFont>("Fonts/FlagCountFont");
            timerFont = Content.Load<SpriteFont>("Fonts/TimerFont");
            scoreTimeFont = Content.Load<SpriteFont>("Fonts/NewTimerFont");

            explosionAnim = new Animation(explosionImg, 5, 5, 23, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 5,
                                        explosionPos, 2f, false);
            instructionsAnim = new Animation(instructionsImg, 2, 1, 2, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 180,
                                        instructionsLoc, 0.5f, true);

            infiniteTimer = new Timer(Timer.INFINITE_TIMER, true);

        }


        protected override void Update(GameTime gameTime)
        {
            mouse = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (!newTimerOutput.Equals(MAX_TIME))
            {
                infiniteTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
            }

            timerOutput = infiniteTimer.GetTimePassedAsString(Timer.FORMAT_HOR_MIN_SEC_MIL);
            timerOutput = timerOutput.Substring(6);
            newTimerOutput = timerOutput.PadLeft(3, '0');

            if (isTimerActivated && gameState == PLAYING)
            {
                infiniteTimer.Activate();
            }

            else if (!isTimerActivated)
            {
                infiniteTimer.Deactivate();
                infiniteTimer.ResetTimer(true);
            }

            if (!isVolumeOn)
            {
                SoundEffect.MasterVolume = 0f;
                MediaPlayer.Volume = 0f;
            }

            if (isVolumeOn)
            {
                SoundEffect.MasterVolume = 0.7f;
                MediaPlayer.Volume = 0.8f;
            }

            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                areInstructionsAnimating = false;

                if (exitRec.Contains(mouse.Position))
                {
                    Exit();
                }

                if (volumeRec.Contains(mouse.Position) && isVolumeOn)
                {
                    isVolumeOn = false;
                }

                else if (volumeRec.Contains(mouse.Position) && !isVolumeOn)
                {
                    isVolumeOn = true;
                }

                if (gameState == PLAYING)
                {
                    if (!displayLevels && mouse.Y >= HUD_HEIGHT)
                    {
                        TileLeftClicked();
                        CheckForWin();

                        isTimerActivated = true;
                    }

                    if (displayLevels && !dropDownRec.Contains(mouse.Position))
                    {
                        displayLevels = false;
                    }

                    if ((dropDownRec.Contains(mouse.Position) && displayLevels) ||
                        currButtonRec[currDifficulty].Contains(mouse.Position))
                    {
                        DropDownClicked();
                    }

                }

                else if (gameState == LOSE || gameState == WIN)
                {
                    if (playAgainRec.Contains(mouse.Position))
                    {
                        PlayAgainClicked();
                        MediaPlayer.Pause();
                    }
                }
            }

            else if (mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton != ButtonState.Pressed && gameState == PLAYING)
            {
                areInstructionsAnimating = false;
                TileRightClicked();
            }

            if (gameState == LOSE && !explosionAnim.isAnimating && !gameLost)
            {
                GameLose();
                gameLost = true;
            }

            if (gameState == WIN && !gameWon)
            {
                GameWin();
                gameWon = true;
            }

            flagCount = Convert.ToString(flagsAmount);
            prevMouse = mouse;

            explosionAnim.Update(gameTime);
            instructionsAnim.Update(gameTime);

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(currBoardImg[currDifficulty], currBoardRec[currDifficulty], Color.White);

            _spriteBatch.Draw(hud, hudRec, Color.White);

            _spriteBatch.Draw(currButtonImg[currDifficulty], currButtonRec[currDifficulty], Color.White);

            _spriteBatch.Draw(watchImg, watchRec, Color.White);

            _spriteBatch.Draw(flagImg, flagRec, Color.White);

            DrawTile();

            _spriteBatch.DrawString(flagCountFont, flagCount, flagCountLoc, Color.White);

            _spriteBatch.DrawString(timerFont, newTimerOutput, infiniteTimerLoc, Color.White);

            if (displayLevels)
            {
                _spriteBatch.Draw(dropDownImg, dropDownRec, Color.White);
                _spriteBatch.Draw(checkImg, checkRec, Color.White);
            }

            if ((gameState == LOSE || gameState == WIN) && !explosionAnim.isAnimating)
            {
                _spriteBatch.Draw(boardShadowImg, boardShadowRec, Color.White * 0.75f);
                _spriteBatch.Draw(gameOverImg[gameState], gameOverRec, Color.White);
                _spriteBatch.Draw(playAgainImg[gameState], playAgainRec, Color.White);
            }

            if (gameState == WIN)
            {
                _spriteBatch.DrawString(scoreTimeFont, newTimerOutput, currTimeLoc, Color.White);
                _spriteBatch.DrawString(scoreTimeFont, bestTimeScore[currDifficulty], bestTimeLoc, Color.White);
            }

            if (gameState == LOSE && !explosionAnim.isAnimating)
            {
                _spriteBatch.Draw(noTimeImg, noTimeRec, Color.White);

                if (bestTimeScore[currDifficulty].Equals("0"))
                {
                    _spriteBatch.Draw(noTimeImg, noBestTimeRec, Color.White);
                }

                else
                {
                    _spriteBatch.DrawString(scoreTimeFont, bestTimeScore[currDifficulty], bestTimeLoc, Color.White);
                }
            }

            instructionsAnim.destRec.X = currBoardImg[currDifficulty].Width / 2 - Convert.ToInt32(instructionsAnim.frameHeight / (2 * 2));
            instructionsAnim.destRec.Y = currBoardImg[currDifficulty].Height / 2 - Convert.ToInt32(instructionsAnim.frameWidth / (2 * 2));

            if (areInstructionsAnimating)
            {
                instructionsAnim.Draw(_spriteBatch, Color.White * 0.75f, Animation.FLIP_NONE);
            }

            if (isVolumeOn)
            {
                _spriteBatch.Draw(volumeOnImg, volumeRec, Color.White);
            }
            if (!isVolumeOn)
            {
                _spriteBatch.Draw(volumeOffImg, volumeRec, Color.White);
            }

            _spriteBatch.Draw(exitImg, exitRec, Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);
        }


        private void DrawTile()
        {
            for (int row = 1; row <= currRows; row++)
            {
                for (int col = 1; col <= currColumns; col++)
                {
                    if (tiles[row, col].GetIfMine() && !tiles[row, col].GetIfFlag() && gameState == LOSE)
                    {
                        _spriteBatch.Draw(minesImg[tiles[row, col].GetMineColour()], tiles[row, col].GetPosition(), Color.White);

                        explosionAnim.destRec = tiles[row, col].GetPosition();
                        explosionAnim.Draw(_spriteBatch, Color.White, Animation.FLIP_NONE);
                    }

                    else if (tiles[row, col].GetIfFlag() && !tiles[row, col].GetIfUncovered())
                    {
                        _spriteBatch.Draw(flagImg, tiles[row, col].GetPosition(), Color.White);
                    }

                    else if (tiles[row, col].GetIfUncovered())
                    {
                        if (row % 2 != 0 && col % 2 != 0 || row % 2 == 0 && col % 2 == 0)
                        {
                            _spriteBatch.Draw(blanksImg[1], tiles[row, col].GetPosition(), Color.White);
                        }
                        else
                        {
                            _spriteBatch.Draw(blanksImg[0], tiles[row, col].GetPosition(), Color.White);
                        }

                        if (tiles[row, col].GetAdjacentMines() != 0)
                        {
                            _spriteBatch.Draw(numbersImg[tiles[row, col].GetAdjacentMines() - 1], tiles[row, col].GetPosition(), Color.White);
                        }

                    }

                }
            }
        }
        

        private void CheckForWin()
        {
            tileRevealedCounter = 0;

            for (int row = 1; row <= currRows; row++)
            {
                for (int col = 1; col <= currColumns; col++)
                {
                    if (tiles[row, col].GetIfMine())
                    {
                        tiles[row, col].SetIfUncovered(false);
                    }
                    if (tiles[row, col].GetIfUncovered())
                    {
                        tileRevealedCounter++;
                    }
                }
            }

            if (tileRevealedCounter >= currColumns * currRows - minesLocated)
            {
                gameState = WIN;
            }
        }


        private void TileLeftClicked()
        {
            for (int row = 1; row < tileShape.GetLength(0); row++)
            {
                for (int col = 1; col < tileShape.GetLength(1); col++)
                {
                    if (tileShape[row, col].Contains(mouse.Position))
                    {
                        if (tiles[row, col].GetIfFlag())
                        {
                            tiles[row, col].SetIfUncovered(false);
                        }

                        else if (!tiles[row, col].GetIfUncovered() && tiles[row, col].GetIfMine())
                        {
                            explosionAnim.isAnimating = true;
                            mineSnd.CreateInstance().Play();

                            WriteToFile();
                            gameState = LOSE;
                        }

                        else if (!tiles[row, col].GetIfUncovered() && tiles[row, col].GetAdjacentMines() == 0)
                        {
                            RecursiveClearing(row, col);

                            SoundEffectInstance largeSnd = largeClearSnd.CreateInstance();
                            largeSnd.Volume = 0.4f;
                            largeSnd.Play();

                        }

                        else if (!tiles[row, col].GetIfUncovered() && !tiles[row, col].GetIfFlag() && !tiles[row, col].GetIfMine()
                            && !displayLevels)
                        {
                            tiles[row, col].SetIfUncovered(true);
                            smallClearSnd.CreateInstance().Play();
                        }

                    }
                }
            }
        }


        private void TileRightClicked()
        {
            for (int row = 1; row < tileShape.GetLength(0); row++)
            {
                for (int col = 1; col < tileShape.GetLength(1); col++)
                {
                    if (tileShape[row, col].Contains(mouse.Position))
                    {
                        if (!tiles[row, col].GetIfUncovered() && !tiles[row, col].GetIfFlag())
                        {
                            if (flagsAmount > 0)
                            {
                                tiles[row, col].SetFlag(true);
                                placeFlagSnd.CreateInstance().Play();

                                flagsAmount--;
                                flagCount = Convert.ToString(flagsAmount);
                            }
                        }

                        else if (!tiles[row, col].GetIfUncovered() && tiles[row, col].GetIfFlag())
                        {
                            tiles[row, col].SetFlag(false);
                            clearFlagSnd.CreateInstance().Play();

                            flagsAmount++;
                            flagCount = Convert.ToString(flagsAmount);

                            if (tiles[row, col].GetIfMine())
                            {
                                minesLocated++;
                            }

                        }
                    }
                }
            }
        }


        private void DropDownClicked()
        {
            if (currButtonRec[currDifficulty].Contains(mouse.Position) && !displayLevels)
            {
                displayLevels = true;
            }

            else if (currButtonRec[currDifficulty].Contains(mouse.Position) && displayLevels)
            {
                displayLevels = false;
            }

            else if (buttonRec[EASY].Contains(mouse.Position) && currDifficulty == EASY
                || buttonRec[MEDIUM].Contains(mouse.Position) && currDifficulty == MEDIUM
                || buttonRec[HARD].Contains(mouse.Position) && currDifficulty == HARD)
            {
                displayLevels = false;
            }

            else if (displayLevels && buttonRec[EASY].Contains(mouse.Position) && currDifficulty != EASY)
            {
                ResetBoard(EASY_MINES, EASY, EASY_ROWS, EASY_COLS, TILE_SIZE_E);
                RelocateImages();

                displayLevels = false;
            }

            else if (displayLevels && buttonRec[MEDIUM].Contains(mouse.Position) && currDifficulty != MEDIUM)
            {
                ResetBoard(MED_MINES, MEDIUM, MED_ROWS, MED_COLS, TILE_SIZE_M);
                RelocateImages();

                displayLevels = false;

            }

            else if (displayLevels && buttonRec[HARD].Contains(mouse.Position) && currDifficulty != HARD)
            {
                ResetBoard(HARD_MINES, HARD, HARD_ROWS, HARD_COLS, TILE_SIZE_H);
                RelocateImages();

                displayLevels = false;
            }

        }


        private void PlayAgainClicked()
        {
            gameState = PLAYING;

            switch (currDifficulty)
            {
                case EASY:

                    ResetBoard(EASY_MINES, EASY, EASY_ROWS, EASY_COLS, TILE_SIZE_E);
                    RelocateImages();

                    break;

                case MEDIUM:

                    ResetBoard(MED_MINES, MEDIUM, MED_ROWS, MED_COLS, TILE_SIZE_M);
                    RelocateImages();

                    break;

                case HARD:

                    ResetBoard(HARD_MINES, HARD, HARD_ROWS, HARD_COLS, TILE_SIZE_H);
                    RelocateImages();

                    break;
            }
        }


        private void ResetBoard(int mines, int difficulty, int rows, int cols, int tileSize)
        {
            gameLost = false;
            gameWon = false;
            isTimerActivated = false;
            displayLevels = false;

            gameState = PLAYING;

            tiles = new Tile[rows + 2, cols + 2];
            tileShape = new Rectangle[rows + 2, cols + 2];

            screenWidth = tileSize * cols;
            screenHeight = tileSize * rows;

            flagsAmount = mines;
            minesLocated = mines;

            currDifficulty = difficulty;

            currRows = rows;
            currColumns = cols;

            currTileSize = tileSize;

            _graphics.PreferredBackBufferWidth = screenWidth;
            _graphics.PreferredBackBufferHeight = screenHeight + HUD_HEIGHT;

            _graphics.ApplyChanges();

            DefineBoard();
            PlantMines(rows, cols);
            CountAdjacent();
        }


        private void DefineBoard()
        {
            for (int row = 0; row < tileShape.GetLength(0); row++)
            {
                for (int col = 0; col < tileShape.GetLength(1); col++)
                {
                    tileShape[row, col] = new Rectangle((col - 1) * currTileSize, (row - 1) * currTileSize + HUD_HEIGHT,
                                                        currTileSize, currTileSize);
                }
            }

            for (int row = 0; row < currRows + 2; row++)
            {
                for (int col = 0; col < currColumns + 2; col++)
                {
                    Rectangle position = new Rectangle((col - 1) * currTileSize, (row - 1) * currTileSize + HUD_HEIGHT,
                                                        currTileSize, currTileSize);

                    tiles[row, col] = new Tile(position);
                    tiles[row, col].SetIfUncovered(false);
                    tiles[row, col].SetFlag(false);
                    tiles[row, col].SetAdjacentMines(0);

                }
            }
        }


        private void RelocateImages()
        {
            hudRec.Width = currBoardImg[currDifficulty].Width;
            watchRec.X = currBoardImg[currDifficulty].Width / 2 - watchImg.Width / 2 * 1 / 3;

            flagCountLoc.X = watchRec.X - 45;
            flagCountLoc.Y = watchRec.Y;

            flagRec.X = (int)flagCountLoc.X - 45;

            gameOverRec.X = currBoardImg[currDifficulty].Width / 2 - gameOverImg[0].Width / 2;
            playAgainRec.X = gameOverRec.X;

            boardShadowRec.Width = currBoardImg[currDifficulty].Width;
            boardShadowRec.Height = currBoardImg[currDifficulty].Height + HUD_HEIGHT;

            infiniteTimerLoc.X = currBoardImg[currDifficulty].Width / 2 + watchRec.Width;

            exitRec.X = currBoardImg[currDifficulty].Width - 45;
            volumeRec.X = exitRec.X - 45;

            noBestTimeRec.X = currBoardImg[currDifficulty].Width / 2 + gameOverImg[0].Width * 1 / 6;
            noTimeRec.X = currBoardImg[currDifficulty].Width / 2 - gameOverImg[0].Width * 1 / 3;

            bestTimeLoc.X = currBoardImg[currDifficulty].Width / 2 + gameOverImg[0].Width * 1 / 6;
            currTimeLoc.X = currBoardImg[currDifficulty].Width / 2 - gameOverImg[0].Width * 1 / 3;

            checkRec.X = buttonRec[currDifficulty].X + 5;
            checkRec.Y = buttonRec[currDifficulty].Y + checkImg.Height / 2;
        }


        private void PlantMines(int maxRows, int maxCols)
        {
            int count = 0;

            while (count < minesLocated)
            {
                int mineLocCol = rng.Next(1, maxCols); rng.Next(1, maxCols);
                int mineLocRow = rng.Next(1, maxRows);
                int randMineColour = rng.Next(1, 8);

                if (!tiles[mineLocRow, mineLocCol].GetIfMine())
                {
                    tiles[mineLocRow, mineLocCol].SetToMine(true);
                    tiles[mineLocRow, mineLocCol].SetMineColour(randMineColour);
                    count++;
                }
            }
        }


        private void CountAdjacent()
        {
            for (int row = 1; row <= currRows; row++)
            {
                for (int col = 1; col <= currColumns; col++)
                {
                    int count = 0;

                    if (tiles[row - 1, col - 1].GetIfMine())
                    {
                        count++;
                    }

                    if (tiles[row - 1, col].GetIfMine())
                    {
                        count++;
                    }

                    if (tiles[row + 1, col].GetIfMine())
                    {
                        count++;
                    }

                    if (tiles[row, col - 1].GetIfMine())
                    {
                        count++;
                    }

                    if (tiles[row, col + 1].GetIfMine())
                    {
                        count++;
                    }

                    if (tiles[row + 1, col - 1].GetIfMine())
                    {
                        count++;
                    }

                    if (tiles[row - 1, col + 1].GetIfMine())
                    {
                        count++;
                    }

                    if (tiles[row + 1, col + 1].GetIfMine())
                    {
                        count++;
                    }

                    tiles[row, col].SetAdjacentMines(count);
                }
            }
        }


        private void RecursiveClearing(int row, int col)
        {
            int blankRowTile;
            int blankColTile;

            for (int adjRow = -1; adjRow <= 1; adjRow++)
            {
                for (int adjCol = -1; adjCol <= 1; adjCol++)
                {
                    blankRowTile = row + adjRow;
                    blankColTile = col + adjCol;

                    if (!InBoardBounds(blankRowTile, blankColTile) ||
                        tiles[blankRowTile, blankColTile].GetIfUncovered() ||
                        tiles[blankRowTile, blankColTile].GetIfMine())
                    {
                        continue;
                    }

                    if (tiles[blankRowTile, blankColTile].GetAdjacentMines() != 0)
                    {
                        if (tiles[blankRowTile, blankColTile].GetIfFlag())
                        {
                            flagsAmount++;
                        }

                        tiles[blankRowTile, blankColTile].SetIfUncovered(true);

                        continue;
                    }

                    tiles[blankRowTile, blankColTile].SetIfUncovered(true);

                    if (tiles[blankRowTile, blankColTile].GetIfFlag())
                    {
                        flagsAmount++;
                    }

                    RecursiveClearing(blankRowTile, blankColTile);
                }
            }

        }


        private bool InBoardBounds(int blankRowTile, int blankColTile)
        {
            bool inBoard = false;

            if (blankRowTile > 0 && blankRowTile <= currRows &&
                blankColTile > 0 && blankColTile <= currColumns)
            {
                inBoard = true;
            }

            return inBoard;
        }

        private void GameLose()
        {
            infiniteTimer.TogglePause();

            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Play(loseMusic);
            }

        }

        private void GameWin()
        {
            infiniteTimer.TogglePause();

            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Play(winMusic);

            }

            SaveAndGetBestTime();

        }

        private void SaveAndGetBestTime()
        {
            string[] data;

            if (File.Exists("SaveFile.txt"))
            {
                inFile = File.OpenText("SaveFile.txt");

                string line = inFile.ReadLine();

                data = line.Split(',');

                bestTimeScore[EASY] = data[0];
                bestTimeScore[MEDIUM] = data[1];
                bestTimeScore[HARD] = data[2];

                if (Convert.ToInt32(data[currDifficulty]) == 0)
                {
                    bestTimeScore[currDifficulty] = timerOutput.PadLeft(3, '0');
                }

                else if (Convert.ToInt32(data[currDifficulty]) > Convert.ToInt32(timerOutput))
                {
                    bestTimeScore[currDifficulty] = timerOutput.PadLeft(3, '0');
                }

                inFile.Close();
            }

            else
            {
                bestTimeScore[currDifficulty] = timerOutput.PadLeft(3, '0');
            }

            WriteToFile();
            
        }

        private void WriteToFile()
        {
            if (bestTimeScore[EASY] == null)
            {
                bestTimeScore[EASY] = "0";
            }

            if (bestTimeScore[MEDIUM] == null)
            {
                bestTimeScore[MEDIUM] = "0";
            }

            if (bestTimeScore[HARD] == null)
            {
                bestTimeScore[HARD] = "0";
            }

            try
            {
                outFile = File.CreateText("SaveFile.txt");
                outFile.WriteLine(bestTimeScore[EASY] + "," + bestTimeScore[MEDIUM] + "," + bestTimeScore[HARD]);

            }

            catch (IndexOutOfRangeException re)
            {
                Console.WriteLine("ERROR: " + re.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
            }
            finally
            {
                outFile.Close();
            }
        }
    }

}