using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace test4;

// https://docs.monogame.net/articles/getting_started/index.html

public class Game2 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    public Game2() {
        // The main game constructor is used to initialize the starting variables.
        // In this case, a new GraphicsDeviceManager is created, and the root directory containing
        // the game's content files is set.
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        // szerokosc okienka
        _graphics.PreferredBackBufferWidth = 200;
        _graphics.PreferredBackBufferHeight = 200;
        _graphics.ApplyChanges();
    }

    protected override void Initialize() {
        // The Initialize method is called after the constructor but before the main game loop (Update/Draw).
        // This is where you can query any required services and load any non-graphic related content.
        // TODO: Add your initialization logic here
    }

    protected override void LoadContent() {
        // The LoadContent method is used to load your game content.
        // It is called only once per game, within the Initialize method, before the main game loop starts.
        // Create a new SpriteBatch, which can be used to draw textures.
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime) {
        // The Update method is called multiple times per second, and it is used to update your game state
        // (checking for collisions, gathering input, playing audio, etc.).
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        // Similar to the Update method, the Draw method is also called multiple times per second.
        // This, as the name suggests, is responsible for drawing content to the screen.
        GraphicsDevice.Clear(Color.White);
        _spriteBatch.Begin();
        
        // TODO: Add your drawing code here
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}