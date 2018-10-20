using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Squash
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// ////////////////////////// ////////////////////////// ///
        // TODO: Klasa Colision
        struct ball_colision_point
        {
            private int x_location;
            private int y_location;

            public int Y_location { get => y_location; set => y_location = value; }
            public int X_location { get => x_location; set => x_location = value; }

            public void update_location(int x_location, int y_location)
            {
                X_location = x_location;
                Y_location = y_location;
            }
        }

        private ball_colision_point top_point, bottom_point, left_point, right_point,
                top_left_point, top_right_point, bottom_left_point, bottom_right_point;
        /// ////////////////////////// ////////////////////////// ///

        private int window_width, window_height;

        private Texture2D splash_screen, menu, background, paddle, ball;

        /// ////////////////////////// ////////////////////////// ///
        // TODO: Przenieść wartości do tablicy/kolekcji - użycie w wielu miejscach
        private Rectangle start_text = new Rectangle(640, 150, 240, 70);
        private Rectangle exit_text = new Rectangle(675, 225, 240, 70);
        /// //////////////////////////

        private Texture2D main_rectangle, left_rectangle, right_rectangle;

        //////////////////////////////////////////////////
        ///////////////////////////////////////////////////
        ///////////////////////////////////////////////////
        ///////////////////////////////////////////////////
        private SpriteFont font;
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////
        //////////////////////////////////////////////////

        private MouseState newMouseState, oldMouseState;

        private bool old_pause_button_state, new_pause_button_state;


        private bool was_p_pressed = false, was_p_released = false;

        private bool wasBallShoot;

        private int[] movmentVector = { 0, 0 };
        private int x_speed = 4, y_speed = 7;

        private int points, penalty_points;
        /// ////////////////////////// ////////////////////////// ///
        /// // TODO: Klasa Colision
        // [0] - lewa [1] - prawa [2] - góra [3] - dół
        // [4] - góra-lewo [5] - góra-prawo [6] - dół-lewo [7] - dół-prawo
        private bool[] where_colision = { false, false, false, false, false, false, false, false };
        /// ////////////////////////// ////////////////////////// ///
        private int x_paddle_location, y_paddle_location;
        private int x_ball_location, y_ball_location;

        private int main_rectangle_x_location, main_rectangle_y_location;
        private int left_rectangle_x_location, left_rectangle_y_location;
        private int right_rectangle_x_location, right_rectangle_y_location;


        /// ////////////////////////// ////////////////////////// ///
        // TODO: Klasa Menu
        private enum game_mode
        {
            Splash, Menu, Game, Summary, Pause
        }
        private game_mode game_state = game_mode.Splash;

        private enum player_type
        {
            Human, AI
        }
        private player_type active_player = player_type.Human;
        /// ////////////////////////// ////////////////////////// ///

        /// ////////////////////////// ////////////////////////// ///
        /// TODO: Klasa Game
        /// 
        protected void try_to_pause_a_game()
        {
            new_pause_button_state = Keyboard.GetState().IsKeyDown(Keys.P);

            if (new_pause_button_state && !old_pause_button_state)
            {
                was_p_pressed = false;
                was_p_released = false;

                if (game_state == game_mode.Pause)
                {
                    game_state = game_mode.Game;
                    this.IsMouseVisible = false;
                    Mouse.SetPosition(x_paddle_location, y_paddle_location);
                }
                   
                else
                {
                    game_state = game_mode.Pause;
                    this.IsMouseVisible = true;

                }
                    
            }
            old_pause_button_state = new_pause_button_state;

        }

        protected void update_window_bounds()
        {
            window_height = GraphicsDevice.Viewport.Bounds.Height;
            window_width = GraphicsDevice.Viewport.Bounds.Width;
        }

        protected void set_up_ball()
        {
            movmentVector[0] = 0;
            movmentVector[1] = 0;
            wasBallShoot = false;

            x_ball_location = x_paddle_location + 30;
            y_ball_location = y_paddle_location - 20;
        }

        protected void shoot_ball(int x_direction, int y_direction)
        {
            movmentVector[0] = x_direction;
            movmentVector[1] = y_direction;
            wasBallShoot = true;
        }

        protected void set_up_game()
        {
            points = 0;
            penalty_points = 0;
            x_paddle_location = window_width / 2;
            y_paddle_location = window_height - 80;
            set_up_ball();
        }
        /// ////////////////////////// ////////////////////////// ///

        /// ////////////////////////// ////////////////////////// ///
        // TODO: Klasa Colision
        protected void reset_collision_array()
        {
            for (int i = 0; i < where_colision.Length; i++)
            {
                where_colision[i] = false;
            }
        }

        protected void switch_players()
        {
            if (active_player == player_type.Human)
            {

                active_player = player_type.AI;

            }
            else
            {
                active_player = player_type.Human;
            }
        }

        private bool is_intersection(Texture2D collision_object, int collision_object_x, int collision_object_y, ball_colision_point coll_point)
        {
            bool answer = false;

            if ((coll_point.X_location + movmentVector[0] >= collision_object_x)
                && (coll_point.X_location + movmentVector[0] <= (collision_object_x + collision_object.Width))
                && (coll_point.Y_location + movmentVector[1] >= collision_object_y)
                && (coll_point.Y_location + movmentVector[1] <= (collision_object_y + collision_object.Height)))
                answer = true;

            return answer;
        }

        protected bool is_collision_with_a_ball(Texture2D collision_object, int collision_object_x, int collision_object_y)
        {
            bool answer = false;

            if (is_intersection(collision_object, collision_object_x, collision_object_y, right_point)) where_colision[0] = true;
            else if (is_intersection(collision_object, collision_object_x, collision_object_y, left_point)) where_colision[1] = true;
            else if (is_intersection(collision_object, collision_object_x, collision_object_y, top_point)) where_colision[2] = true;
            else if (is_intersection(collision_object, collision_object_x, collision_object_y, bottom_point)) where_colision[3] = true;
            else if (is_intersection(collision_object, collision_object_x, collision_object_y, top_left_point)) where_colision[4] = true;
            else if (is_intersection(collision_object, collision_object_x, collision_object_y, top_right_point)) where_colision[5] = true;
            else if (is_intersection(collision_object, collision_object_x, collision_object_y, bottom_left_point)) where_colision[6] = true;
            else if (is_intersection(collision_object, collision_object_x, collision_object_y, bottom_right_point)) where_colision[7] = true;

            for (int i = 0; i < where_colision.Length; i++)
            {
                if (where_colision[i] == true)
                    answer = true;
            }

            return answer;
        }
        /// ////////////////////////// ////////////////////////// ///

        public bool is_negative(int number)
        {
            return number < 0;
        }

        protected void deflect_ball()
        {
            //czy w poziomie
            if ((where_colision[0] == true) || (where_colision[1] == true))
                shoot_ball(movmentVector[0] * (-1), movmentVector[1]);
            else
                shoot_ball(movmentVector[0], movmentVector[1] * (-1));
        }

        protected bool is_ball_out()
        {
            bool answer = false;

            if ((x_ball_location + ball.Width <= 0) ||
                (x_ball_location >= window_width) ||
                (y_ball_location + ball.Width <= 0) ||
                (y_ball_location >= window_height))
                answer = true;

            return answer;
        }

        // Jak na razie nie używane - ewentualnie Klasa Menu - lub usunąć
        protected bool if_mouse_within_rectangle(Rectangle rectangle)
        {
            bool answer = false;
            return answer;
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1000;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            update_window_bounds();
            set_up_game();

            base.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // TODO: Jakaś funkcja do tworzenia textur
            main_rectangle = Content.Load<Texture2D>("images\\duzy_prostokat");
            left_rectangle = Content.Load<Texture2D>("images\\maly_prostokat");
            right_rectangle = Content.Load<Texture2D>("images\\maly_prostokat");
            background = Content.Load<Texture2D>("images\\tlo");
            ball = Content.Load<Texture2D>("images\\pilka");
            paddle = Content.Load<Texture2D>("images\\paletka");
            //////////////////////////////////////////////////
            //////////////////////////////////////////////////
            //////////////////////////////////////////////////
            font = Content.Load<SpriteFont>("fonts\\wynik");
            //////////////////////////////////////////////////
            //////////////////////////////////////////////////
            //////////////////////////////////////////////////
            splash_screen = Content.Load<Texture2D>("images\\splash");
            menu = Content.Load<Texture2D>("images\\menu");

            // Jakaś funkcja 
            main_rectangle_x_location = window_width / 2 - main_rectangle.Width / 2;
            main_rectangle_y_location = 0;

            left_rectangle_x_location = main_rectangle_x_location - left_rectangle.Width;
            left_rectangle_y_location = 0;

            right_rectangle_x_location = main_rectangle_x_location + main_rectangle.Width;
            right_rectangle_y_location = 0;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            // TODO: Add your update logic here

            /// ////////////////////////// ////////////////////////// ///
            /// TODO: Klasa Menu
            update_window_bounds();
            newMouseState = Mouse.GetState();
            if ((newMouseState.LeftButton == ButtonState.Pressed) &&
                (oldMouseState.LeftButton == ButtonState.Released) &&
                (game_state == game_mode.Splash))
            {
                /// /////////////////////
                game_state = game_mode.Menu;
                /// /////////////////////
                this.IsMouseVisible = true;
            }
            else if (game_state == game_mode.Menu)
            {

                newMouseState = Mouse.GetState();

                if ((newMouseState.LeftButton == ButtonState.Pressed)
                    && (oldMouseState.LeftButton == ButtonState.Released)
                    && (newMouseState.Position.X >= 640 && newMouseState.Position.X <= 800)
                    && (newMouseState.Position.Y >= 150 && newMouseState.Position.Y <= 220))
                {
                    game_state = game_mode.Game;
                    oldMouseState = newMouseState;
                    this.IsMouseVisible = false;
                }
                else if ((newMouseState.LeftButton == ButtonState.Pressed)
                    && (oldMouseState.LeftButton == ButtonState.Released)
                    && (newMouseState.Position.X >= 675 && newMouseState.Position.X <= 915)
                    && (newMouseState.Position.Y >= 225 && newMouseState.Position.Y <= 295))
                    Exit();
            }
            else if (game_state == game_mode.Game)
            {
                /// /// /// /// /// /// /// /// /// /// 
                /// TODO: Klasa Game
                try_to_pause_a_game();

                if (newMouseState.X >= 0 && newMouseState.X <= window_width - paddle.Width)
                    x_paddle_location = newMouseState.X;

                if (wasBallShoot == false)
                    x_ball_location = x_paddle_location + 30;

                newMouseState = Mouse.GetState();
                if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released && wasBallShoot == false)
                    shoot_ball(-x_speed, -y_speed);

                if (newMouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released && wasBallShoot == false)
                    shoot_ball(x_speed, -y_speed);

                oldMouseState = newMouseState;

                /// ////////////////////////// ////////////////////////// ///
                /// TODO: Klasa Colision
                if (wasBallShoot == true)
                {
                    if (is_collision_with_a_ball(main_rectangle, main_rectangle_x_location, main_rectangle_y_location))


                    {
                        points++;
                        deflect_ball();
                        reset_collision_array();

                    }
                    else if (is_collision_with_a_ball(left_rectangle, left_rectangle_x_location, left_rectangle_y_location)
                            || is_collision_with_a_ball(right_rectangle, right_rectangle_x_location, right_rectangle_y_location))
                    {
                        deflect_ball();
                        reset_collision_array();
                    }
                    else if (is_collision_with_a_ball(paddle, x_paddle_location, y_paddle_location))
                    {
                        deflect_ball();
                        reset_collision_array();
                        switch_players();
                    }

                    x_ball_location += movmentVector[0];
                    y_ball_location += movmentVector[1];


                    top_point.update_location(x_ball_location + ball.Width / 2, y_ball_location);
                    left_point.update_location(x_ball_location, y_ball_location + ball.Width / 2);
                    right_point.update_location(x_ball_location + ball.Width, y_ball_location + ball.Height / 2);
                    bottom_point.update_location(x_ball_location + ball.Width / 2, y_ball_location + ball.Height);
                    top_right_point.update_location(x_ball_location + (ball.Width / 2 + 6), y_ball_location + (ball.Height / 2 - 7));
                    top_left_point.update_location(x_ball_location + (ball.Width / 2 - 7), y_ball_location + (ball.Width / 2 - 7));
                    bottom_right_point.update_location(x_ball_location + (ball.Width / 2 + 6), y_ball_location + (ball.Height / 2 + 6));
                    bottom_left_point.update_location(x_ball_location + (ball.Width / 2 - 7), y_ball_location + (ball.Width / 2 + 6));
                }
                /// ////////////////////////// ////////////////////////// ///

                if (is_ball_out())
                {
                    penalty_points--;
                    switch_players();
                    set_up_ball();

                }
            }
            else if (game_state == game_mode.Pause)
            {
                try_to_pause_a_game();
            }
            base.Update(gameTime);
            /// /// /// /// /// /// /// /// /// /// 
        }
        /// ////////////////////////// ////////////////////////// ///

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        /// ////////////////////////// ////////////////////////// ///
        /// TODO: Jakaś klasa do rysowania lub zostawić - do rozwarzenia
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, window_width, window_height), Color.White);
            spriteBatch.End();

            if (game_state == game_mode.Splash)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(splash_screen, new Rectangle(0, 0, window_width, window_height), Color.White);
                spriteBatch.End();
            }
            else if (game_state == game_mode.Menu)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(menu, new Rectangle(0, 0, window_width, window_height), Color.White);
                spriteBatch.End();
            }
            else
            {
                int points_x_location = 10;

                spriteBatch.Begin();
                spriteBatch.Draw(left_rectangle, new Vector2(left_rectangle_x_location, left_rectangle_y_location), Color.White);
                spriteBatch.Draw(right_rectangle, new Vector2(right_rectangle_x_location, right_rectangle_y_location), Color.White);
                spriteBatch.Draw(main_rectangle, new Vector2(main_rectangle_x_location, main_rectangle_y_location), Color.White);

                if (active_player == player_type.Human)
                    spriteBatch.Draw(paddle, new Vector2(x_paddle_location, y_paddle_location), Color.Green);
                else
                    spriteBatch.Draw(paddle, new Vector2(x_paddle_location, y_paddle_location), Color.White);

                spriteBatch.Draw(ball, new Vector2(x_ball_location, y_ball_location), Color.White);
                //////////////////////////////////////////////////
                //////////////////////////////////////////////////
                //////////////////////////////////////////////////
                spriteBatch.DrawString(font, "Punkty: " + points, new Vector2(points_x_location, 10), Color.Green);
                spriteBatch.DrawString(font, "Punkty ujemne: " + penalty_points, new Vector2(window_width - 180, 10), Color.Red);
                //////////////////////////////////////////////////
                //////////////////////////////////////////////////
                //////////////////////////////////////////////////
                //TODO:zrobić osobną czcionkę od pazy i odpowiednio to wymierzyć
                if (game_state == game_mode.Pause)
                {
                    spriteBatch.DrawString(font, "PAUZA", new Vector2((window_width/2)-20, (window_height/2)-20), Color.White);
                }

                spriteBatch.End();
            }
            // TODO: Add your drawing code here
            base.Draw(gameTime);

        }
        /// ////////////////////////// ////////////////////////// ///
    }
}