using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Verdant;

namespace ChargeGame;

public class ChargeGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Color backgroundColor = Color.Black;

    private SceneManager sceneManager;

    public ChargeGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
    }

    protected override void Initialize()
    {
        base.Initialize();

        Renderer.Initialize(_graphics, 2, 2);

        sceneManager = new();

        TitleScene titleScene = new();
        sceneManager.AddScene(titleScene);

        Renderer.Cursor = Resources.Cursor;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Resources.LoadResources(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        sceneManager.ActiveScene.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(backgroundColor);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        sceneManager.ActiveScene.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
