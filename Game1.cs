using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogameTEST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace monogameTEST
{
    public class Game1 : Game
    {
        Random rand = new Random();

        Deck battleDeck = new Deck();
        Deck playerDeck = new Deck();
        Deck sandSlimeDeck = new Deck();
        Deck currentEnemyDeck = new Deck();

        List<Card> playerHand = new List<Card>();

        Texture2D filledRect;
        Texture2D healthBarEmpty;
        Texture2D healthBarBar;
        Texture2D background;
        Texture2D enemy;
        Texture2D player;
        Texture2D swordCard;
        Texture2D shieldCard;
        Texture2D cardHovered;
        Texture2D debugModeOverlay;
        Texture2D deckIcon;

        SpriteFont defaultFont;

        bool spaceReadyToPress = true;
        bool leftClickReadyToPress = true;
        bool f3ReadyToPress = true;
        bool downReadyToPress = true;
        bool upReadyToPress = true;
        bool debugMode = false;
        bool holdingCard = false;

        Character playerCharacter = new Character { Health = 25, Shield = 15, Type = "PLAYER", MaxHealth = 25, MaxShield=15 };
        Character sandSlimeCharacter = new Character { Health = 85, Shield = 55, Type = "SANDSLIME", MaxShield=55, MaxHealth=85 };

        Character currentEnemy;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //Configure default graphics settings
            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            //Init player starter deck
            playerDeck.cards.Add(new Card("PLAYER","SWORD" ));
            playerDeck.cards.Add(new Card("PLAYER", "SWORD"));
            playerDeck.cards.Add(new Card("PLAYER", "SWORD"));
            playerDeck.cards.Add(new Card("PLAYER", "SWORD"));
            playerDeck.cards.Add(new Card("PLAYER", "SWORD"));
            playerDeck.cards.Add(new Card("PLAYER", "SHIELD"));
            playerDeck.cards.Add(new Card("PLAYER", "SHIELD"));
            playerDeck.cards.Add(new Card("PLAYER", "SHIELD"));
            playerDeck.cards.Add(new Card("PLAYER", "SHIELD"));

            //Init sandSlime Deck
            playerDeck.cards.Add(new Card("ENEMY", "SWORD"));
            playerDeck.cards.Add(new Card("ENEMY", "SWORD"));
            playerDeck.cards.Add(new Card("ENEMY", "SWORD"));
            playerDeck.cards.Add(new Card("ENEMY", "SWORD"));
            playerDeck.cards.Add(new Card("ENEMY", "SWORD"));
            playerDeck.cards.Add(new Card("ENEMY", "SHIELD"));
            playerDeck.cards.Add(new Card("ENEMY", "SHIELD"));
            playerDeck.cards.Add(new Card("ENEMY", "SHIELD"));
            playerDeck.cards.Add(new Card("ENEMY", "SHIELD"));

            currentEnemyDeck = sandSlimeDeck;
            currentEnemy = sandSlimeCharacter;
            ResetBattleDeck(playerDeck.cards, currentEnemyDeck.cards);
            SetupInitialPlayerHand();

            


            //Setup simple texture for health bar rectangles
            filledRect = new Texture2D(GraphicsDevice, 1, 1);
            filledRect.SetData(new Color[] { Color.White });

            base.Initialize();
        }

        protected override void LoadContent()
        {
            
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            swordCard = Content.Load<Texture2D>("swordCard");
            shieldCard = Content.Load<Texture2D>("shieldCard");
            healthBarBar = Content.Load<Texture2D>("barsBar");
            healthBarEmpty = Content.Load<Texture2D>("barsEmpty");
            background = Content.Load<Texture2D>("desertBackground");
            enemy = Content.Load<Texture2D>("Enemies/sandSlime");
            player = Content.Load<Texture2D>("Enemies/dragonHawk");
            cardHovered = Content.Load<Texture2D>("UI/cardHovered");
            debugModeOverlay = Content.Load<Texture2D>("UI/debugMode");
            deckIcon = Content.Load<Texture2D>("UI/deckIcon");

            defaultFont = Content.Load<SpriteFont>("UI/defaultFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Wipe out empty cards from hand
            for (int i = 0; i < playerHand.Count; i++) 
            {
                if (playerHand[i].Type == "") 
                {
                    playerHand.RemoveAt(i);
                }
            }

            //Check for mouse hover over cards in player hand.
            var mouseState = Mouse.GetState();
            var mousePoint = new Point(mouseState.X, mouseState.Y);
            for (int i = 0; i < playerHand.Count; i++) 
            {
                if (playerHand[i].cardArea.Contains(mousePoint))
                {
                    playerHand[i].isHovered = true;
                    if (holdingCard == false && Mouse.GetState().LeftButton == ButtonState.Pressed && i < playerHand.Count-1)
                    {
                        leftClickReadyToPress = false;
                        foreach (Card card in playerHand) 
                        {
                            if (card.isHeld)
                                holdingCard = true;
                        }
                        if (holdingCard == false)
                        {
                            playerHand[i].isHeld = true;
                            holdingCard = true;
                        }
                    }  
                }
                else
                    playerHand[i].isHovered = false;
            }

            // If card is held, move it and the next card in the hand with the mouse
            for (int i = 0; i < playerHand.Count; i++)
            {
                if (playerHand[i].isHeld)
                {
                    playerHand[i].Location = mousePoint - new Point(playerHand[i].Size.X / 2, playerHand[i].Size.Y / 2);
                    playerHand[i].cardArea = new Rectangle(playerHand[i].Location, playerHand[i].Size);

                    playerHand[i + 1].Location = (mousePoint - new Point(playerHand[i + 1].Size.X / 2, playerHand[i + 1].Size.Y / 2) + new Point(playerHand[i + 1].Size.X, 0));
                    playerHand[i + 1].cardArea = new Rectangle(playerHand[i + 1].Location, playerHand[i + 1].Size);

                    if (Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        playerHand[i].isHeld = false;
                        playerHand[i].Location = new Point((i * 187) + 399, 818);
                        playerHand[i].cardArea = new Rectangle(playerHand[i].Location, playerHand[i].Size);

                        playerHand[i + 1].Location = new Point(((i + 1) * 187) + 399, 818);
                        playerHand[i + 1].cardArea = new Rectangle(playerHand[i + 1].Location, playerHand[i + 1].Size);
                        holdingCard = false;
                    }
                    // Play Held Cards
                    else if (Mouse.GetState().LeftButton == ButtonState.Pressed && leftClickReadyToPress && Mouse.GetState().Y < 810) 
                    {
                        leftClickReadyToPress = false;
                        playerHand.RemoveAt(i);
                        playerHand.RemoveAt(i);
                        holdingCard = false;
                        playerHand.Add(battleDeck.DrawCard());
                        playerHand.Add(battleDeck.DrawCard());

                        for (int j = 0; j < playerHand.Count; j++) 
                        {
                            playerHand[j].Location = new Point((j * 187) + 399, 818);
                            playerHand[j].cardArea = new Rectangle(playerHand[j].Location, playerHand[j].Size);
                        }
                        break;
                    }
                }
            }

            if (battleDeck.cards.Count() == 0 && playerHand.Count() == 0)
            {
                ResetBattleDeck(playerDeck.cards, currentEnemyDeck.cards);
                SetupInitialPlayerHand();
            }

            // Calculate player status bar sizes
            playerCharacter.HealthBarSize = (float)playerCharacter.Health / (float)playerCharacter.MaxHealth;
            playerCharacter.ShieldBarSize = (float)playerCharacter.Shield / (float)playerCharacter.MaxShield;
            currentEnemy.HealthBarSize = (float)currentEnemy.Health / (float)currentEnemy.MaxHealth;
            currentEnemy.ShieldBarSize = (float)currentEnemy.Shield / (float)currentEnemy.MaxShield;



            // DEBUG // F3 places game in debug mde
            if (Keyboard.GetState().IsKeyDown(Keys.F3) && f3ReadyToPress) 
            {
                f3ReadyToPress = false;
                debugMode = !debugMode;
            }
            // DEBUG // Space reshuffles deck and redraws hand.
            if (debugMode && Keyboard.GetState().IsKeyDown(Keys.Space) && spaceReadyToPress )
            {
                holdingCard = false;
                spaceReadyToPress = false;
                battleDeck.cards = CreateBattleDeck(playerDeck.cards, sandSlimeDeck.cards);
                battleDeck.Shuffle();
                playerHand.Clear();
                for (int i = 0; i < 6; i++)
                {
                    Card temp = battleDeck.DrawCard();
                    playerHand.Add(new Card(temp.Owner, temp.Type ));
                    playerHand[i].Location = new Point((i * 187) + 399, 818);
                    playerHand[i].cardArea = new Rectangle(playerHand[i].Location, playerHand[i].Size);
                }
            }
            // DEBUG // Arrow Keys damage characters
            if (debugMode && Keyboard.GetState().IsKeyDown(Keys.Down) && downReadyToPress) 
            {
                downReadyToPress = false;
                playerCharacter.Health -= 1;
                playerCharacter.Shield -= 1;
                currentEnemy.Health -= 1;
                currentEnemy.Shield -= 1;
            }
            if (debugMode && Keyboard.GetState().IsKeyDown(Keys.Up) && upReadyToPress)
            {
                upReadyToPress = false;
                playerCharacter.Health += 1;
                playerCharacter.Shield += 1;
                currentEnemy.Health += 1;
                currentEnemy.Shield += 1;
            }

            // Clamp Health and Shield to 0 and Max values
            playerCharacter.Health = Math.Min(playerCharacter.MaxHealth, Math.Max(0, playerCharacter.Health));
            playerCharacter.Shield = Math.Min(playerCharacter.MaxShield, Math.Max(0, playerCharacter.Shield));
            currentEnemy.Health = Math.Min(currentEnemy.MaxHealth, Math.Max(0, currentEnemy.Health));
            currentEnemy.Shield = Math.Min(currentEnemy.MaxShield, Math.Max(0, currentEnemy.Shield));

            if (Mouse.GetState().LeftButton == ButtonState.Released)
                leftClickReadyToPress = true;
            if (Keyboard.GetState().IsKeyUp(Keys.F3))
                f3ReadyToPress = true;
            if (Keyboard.GetState().IsKeyUp(Keys.Down))
                downReadyToPress = true;
            if (Keyboard.GetState().IsKeyUp(Keys.Up))
                upReadyToPress = true;
            if (Keyboard.GetState().IsKeyUp(Keys.Space) && !spaceReadyToPress)
                spaceReadyToPress = true;
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);

            // Draw status bars for player and enemy
            _spriteBatch.Draw(filledRect, new Rectangle(new Point(50, 1000), new Point((int)(playerCharacter.HealthBarSize * (float)330), 22)), Color.Red);
            _spriteBatch.Draw(filledRect, new Rectangle(new Point(50, 1023), new Point((int)(playerCharacter.ShieldBarSize * (float)330), 22)), Color.RoyalBlue);
            _spriteBatch.Draw(filledRect, new Rectangle(new Point(1920 - 50 - 332, 1000), new Point((int)(currentEnemy.HealthBarSize * (float)330), 22)), Color.Red);
            _spriteBatch.Draw(filledRect, new Rectangle(new Point(1920 - 50 - 332, 1023), new Point((int)(currentEnemy.ShieldBarSize * (float)330), 22)), Color.RoyalBlue);

            _spriteBatch.Draw(player, new Vector2(470, 550), Color.White);
            _spriteBatch.Draw(enemy, new Vector2(1250, 615), Color.White);
            _spriteBatch.Draw(healthBarEmpty, new Vector2(50, 1000), Color.White);
            _spriteBatch.Draw(healthBarEmpty, new Vector2(1920 - 50 - 332, 1000), Color.White);
            _spriteBatch.Draw(deckIcon, new Vector2(1750, 50), Color.White);
            _spriteBatch.DrawString(defaultFont, battleDeck.cards.Count().ToString(), new Vector2(1800,80), Color.Black);

            _spriteBatch.DrawString(defaultFont, playerCharacter.Health.ToString(), new Vector2(307, 1000), Color.Black);
            _spriteBatch.DrawString(defaultFont, $" / {playerCharacter.MaxHealth.ToString()}", new Vector2(330, 1000), Color.Black);
            _spriteBatch.DrawString(defaultFont, playerCharacter.Shield.ToString(), new Vector2(307, 1021), Color.Black);
            _spriteBatch.DrawString(defaultFont, $" / {playerCharacter.MaxShield.ToString()}", new Vector2(330, 1021), Color.Black);

            _spriteBatch.DrawString(defaultFont, currentEnemy.Health.ToString(), new Vector2(1795, 1000), Color.Black);
            _spriteBatch.DrawString(defaultFont, $" / {currentEnemy.MaxHealth.ToString()}", new Vector2(1818, 1000), Color.Black);
            _spriteBatch.DrawString(defaultFont, currentEnemy.Shield.ToString(), new Vector2(1795, 1021), Color.Black);
            _spriteBatch.DrawString(defaultFont, $" / {currentEnemy.MaxShield.ToString()}", new Vector2(1818, 1021), Color.Black);

            _spriteBatch.DrawString(defaultFont, battleDeck.cards.Count().ToString(), new Vector2(1800, 80), Color.Black);

            for (int i = 0; i < playerHand.Count; i++) 
            {
                Color overlay = Color.White;
                if (playerHand[i].Owner == "PLAYER")
                    overlay = Color.LightBlue;
                else if (playerHand[i].Owner == "ENEMY")
                    overlay = Color.LightSalmon;

               
                switch (playerHand[i].Type) 
                {
                    case "SWORD":
                        _spriteBatch.Draw(swordCard, new Vector2(playerHand[i].Location.X, playerHand[i].Location.Y), overlay);
                        break;
                    case "SHIELD":
                        _spriteBatch.Draw(shieldCard, new Vector2(playerHand[i].Location.X, playerHand[i].Location.Y), overlay);
                        break;
                }
            }

            if (!holdingCard)
            {
                for (int i = 0; i < playerHand.Count; i++)
                {
                    if (playerHand[i].isHovered)
                    {
                        if (i < playerHand.Count - 1)
                            _spriteBatch.Draw(cardHovered, new Vector2(playerHand[i].Location.X, playerHand[i].Location.Y), Color.White);
                    }
                }
            }

            if (debugMode) 
            {
                _spriteBatch.Draw(debugModeOverlay, new Vector2(0, 0), Color.White);
            }


            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public List<Card> CreateBattleDeck(List<Card> playerList, List<Card> enemyList) 
        {
            return playerList.Concat(enemyList).ToList();
        }

        public void SetupInitialPlayerHand() 
        {
            //Draw cards for starter hand, setup location and area for future logic
            for (int i = 0; i < 6; i++)
            {
                Card temp = battleDeck.DrawCard();
                playerHand.Add(new Card(temp.Owner, temp.Type ));
                playerHand[i].Location = new Point((i * 187) + 399, 818);
                playerHand[i].cardArea = new Rectangle(playerHand[i].Location, playerHand[i].Size);
            }
        }

        public void ResetBattleDeck(List<Card> deck1, List<Card> deck2) 
        {
            //Combine enemy deck with playerdeck
            battleDeck.cards = CreateBattleDeck(deck1, deck2);
            battleDeck.Shuffle();
        }

        public void PlayCard(Card card) 
        {
            switch (card.Type) 
            {
                case "SWORD":
                    if (card.Owner == "PLAYER") 
                    {
                        
                    }
                    break;
                case "SHIELD":
                    break;

            }
        
        }
    }
}
