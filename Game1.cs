using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static Squash.Colision;

namespace Squash
{
    public class Game1 : Game
    {
        private Menu menuObject = new Menu();
        private Ai ai = new Ai();

        public struct Ball_colision_point
        {
            private double x_location;
            private double y_location;

            public double Y_location { get => y_location; set => y_location = value; }
            public double X_location { get => x_location; set => x_location = value; }

            public void update_location(double x_location, double y_location)
            {
                X_location = x_location;
                Y_location = y_location;
            }
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private double window_width, window_height;
        static private int change = 0;
        private bool change_check = false;

        static internal int Get_change()
        {
            return change;
        }

        private Texture2D splash_screen, menu, background, paddle, ball, menu_hard, menu_middle;

        private Rectangle start_text = new Rectangle(640, 150, 240, 70);
        private Rectangle exit_text = new Rectangle(675, 225, 240, 70);

        private Texture2D main_rectangle, left_rectangle, right_rectangle;
        private Rectangle left_wall_rectangle, right_wall_rectnangle;

        private SpriteFont font;
        private SpriteFont fontPause;

        private MouseState newMouseState, oldMouseState;

        //private bool old_pause_button_state, new_pause_button_state;
        private bool was_ai_set_up_for_a_shoot = false;

        private bool wasBallShoot;

        private double[] movmentVector = { 0, 0 };
        private double x_speed = 4, y_speed = 4;

        private double points, penalty_points;
        // [0] - lewa [1] - prawa [2] - góra [3] - dół
        // [4] - góra-lewo [5] - góra-prawo [6] - dół-lewo [7] - dół-prawo
        private bool[] where_colision = { false, false, false, false, false, false, false, false };

        static public double x_paddle_location;
        static public double y_paddle_location;

        public double paddle_width, x_target_paddle_location;
        private double x_ball_location, y_ball_location, ball_width;

        private double main_rectangle_x_location, main_rectangle_y_location;
        private double left_rectangle_x_location, left_rectangle_y_location;
        private double right_rectangle_x_location, right_rectangle_y_location;

        private Ball_colision_point top_point, bottom_point, left_point, right_point,
            top_left_point, top_right_point, bottom_left_point, bottom_right_point;

        private Menu.game_mode game_state = Menu.game_mode.Splash;
        static private Ai.level level = Ai.level.Easy_Level;

        static internal Ai.level Get_level()
        {
            return level;
        }

        private enum player_type
        {
            Human, AI
        }
        private player_type active_player = player_type.Human;

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

            x_ball_location = x_paddle_location+(paddle_width/2)-(ball_width/2);
            y_ball_location = y_paddle_location - 20;
        }

        protected void shoot_ball(double x_direction, double y_direction)
        {
            movmentVector[0] = x_direction;
            movmentVector[1] = y_direction;

            if(wasBallShoot==false)
                switch_players();

            wasBallShoot = true;
        }

        protected void set_up_game()
        {
            change = 0;
            points = 0;
            penalty_points = 0;
            x_paddle_location = window_width / 2;
            y_paddle_location = window_height - 80;
            set_up_ball();
        }

        protected void reset_collision_array()
        {
            for (int i = 0; i < where_colision.Length; i++)
                where_colision[i] = false;
        }

        protected void switch_players()
        {
            if (active_player == player_type.Human)
                active_player = player_type.AI;
            else
                active_player = player_type.Human;
        }
        protected bool is_game_over()
        {
            return penalty_points == -1;
        }

        private bool is_intersection(Texture2D collision_object, double collision_object_x, double collision_object_y, Ball_colision_point coll_point)
        {
            bool answer = false;

            if ((coll_point.X_location + movmentVector[0] >= collision_object_x)
                && (coll_point.X_location + movmentVector[0] <= (collision_object_x + collision_object.Width))
                && (coll_point.Y_location + movmentVector[1] >= collision_object_y)
                && (coll_point.Y_location + movmentVector[1] <= (collision_object_y + collision_object.Height)))
                answer = true;

            return answer;
        }

        protected bool is_collision_with_a_ball(Texture2D collision_object, double collision_object_x, double collision_object_y)
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
                if (where_colision[i] == true)
                    answer = true;

            return answer;
        }

        private bool is_intersection(Rectangle collision_object, double collision_object_x, double collision_object_y, Ball_colision_point coll_point)
        {
            bool answer = false;

            if ((coll_point.X_location + movmentVector[0] >= collision_object_x)
                && (coll_point.X_location + movmentVector[0] <= (collision_object_x + collision_object.Width))
                && (coll_point.Y_location + movmentVector[1] >= collision_object_y)
                && (coll_point.Y_location + movmentVector[1] <= (collision_object_y + collision_object.Height)))
                answer = true;

            return answer;
        }
        protected bool is_collision_with_a_ball(Rectangle collision_object, double collision_object_x, double collision_object_y)
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
                if (where_colision[i] == true)
                    answer = true;

            return answer;
        }

        public bool is_negative(double number)
        {
            return number < 0;
        }

        protected void deflect_ball()
        {
            //czy w poziomie
            if ((where_colision[0] == true) || (where_colision[1] == true))
                shoot_ball(movmentVector[0] * (-1), movmentVector[1] );
            else
                shoot_ball(movmentVector[0], movmentVector[1]*(-1));
        }

        protected bool is_ball_out()
        {
            bool answer = false;

            if (y_ball_location >= window_height)
                answer = true;

            return answer;
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1000;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            update_window_bounds();
            
            base.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            main_rectangle = Content.Load<Texture2D>("images\\duzy_prostokat");
            left_rectangle = Content.Load<Texture2D>("images\\maly_prostokat");
            right_rectangle = Content.Load<Texture2D>("images\\maly_prostokat");
            background = Content.Load<Texture2D>("images\\tlo");
            ball = Content.Load<Texture2D>("images\\pilka");
            paddle = Content.Load<Texture2D>("images\\paletka");
            font = Content.Load<SpriteFont>("fonts\\wynik");
            //fontPause = Content.Load<SpriteFont>("fonts\\paues");
            splash_screen = Content.Load<Texture2D>("images\\splash");
            menu = Content.Load<Texture2D>("images\\menu_easy");
            menu_hard = Content.Load<Texture2D>("images\\menu_hard");
            menu_middle = Content.Load<Texture2D>("images\\menu_middle");

            main_rectangle_x_location = window_width / 2 - main_rectangle.Width / 2;
            main_rectangle_y_location = 0;

            left_rectangle_x_location = main_rectangle_x_location - left_rectangle.Width;
            left_rectangle_y_location = 0;

            right_rectangle_x_location = main_rectangle_x_location + main_rectangle.Width;
            right_rectangle_y_location = 0;

            left_wall_rectangle = new Rectangle(-10, 0, 5, (int)window_height);
            right_wall_rectnangle = new Rectangle(0, (int)window_width, 10, (int)window_height);

            paddle_width = paddle.Width;
            ball_width = ball.Width;
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            update_window_bounds();

            newMouseState = Mouse.GetState();

            if ((newMouseState.LeftButton == ButtonState.Pressed) &&
                (oldMouseState.LeftButton == ButtonState.Released) &&
                (game_state == Menu.game_mode.Splash))
            {
                game_state = Menu.game_mode.Menu;
                this.IsMouseVisible = true;
            }
            else if (game_state == Menu.game_mode.Menu)
            {
                if ((newMouseState.LeftButton == ButtonState.Pressed) &&
                    (oldMouseState.LeftButton == ButtonState.Released) &&
                    (newMouseState.Position.X >= 640 && newMouseState.Position.X <= 800) &&
                    (newMouseState.Position.Y >= 150 && newMouseState.Position.Y <= 220))
                {
                    game_state = Menu.game_mode.Game;
                    set_up_game();
                    this.IsMouseVisible = false;
                }
                else if ((newMouseState.LeftButton == ButtonState.Pressed) &&
                    (oldMouseState.LeftButton == ButtonState.Released) &&
                    (newMouseState.Position.X >= 675 && newMouseState.Position.X <= 915) &&
                    (newMouseState.Position.Y >= 295 && newMouseState.Position.Y <= 365))
                         Exit();
                else if((level == Ai.level.Easy_Level) &&
                    (newMouseState.Position.X >= 870 && newMouseState.Position.X <= 920) &&
                    (newMouseState.Position.Y >= 225 && newMouseState.Position.Y <= 295) &&
                    (newMouseState.LeftButton == ButtonState.Pressed) &&
                    (oldMouseState.LeftButton == ButtonState.Released))
                         level = Ai.level.Middle_Level;
                else if ((level == Ai.level.Middle_Level) &&
                    (newMouseState.Position.X >= 870 && newMouseState.Position.X <= 920) &&
                    (newMouseState.Position.Y >= 225 && newMouseState.Position.Y <= 295) &&
                    (newMouseState.LeftButton == ButtonState.Pressed) &&
                    (oldMouseState.LeftButton == ButtonState.Released))
                         level = Ai.level.Hard_Level;
                else if ((level == Ai.level.Hard_Level) &&
                    (newMouseState.Position.X >= 870 && newMouseState.Position.X <= 920) &&
                    (newMouseState.Position.Y >= 225 && newMouseState.Position.Y <= 295) &&
                    (newMouseState.LeftButton == ButtonState.Pressed) &&
                    (oldMouseState.LeftButton == ButtonState.Released))
                         level = Ai.level.Easy_Level;
            }
            else if (game_state == Menu.game_mode.Game && menuObject.Get_is_game_paused().Equals(false))
            {
                menuObject.Try_to_pause_a_game();

                if (active_player == player_type.Human)
                {
                    if (newMouseState.X >= 0 && newMouseState.X <= window_width - paddle.Width)
                        x_paddle_location = newMouseState.X;

                    if (wasBallShoot == false)
                        set_up_ball();

                    newMouseState = Mouse.GetState();
                    if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released && wasBallShoot == false)
                        shoot_ball(-x_speed, -y_speed);

                    if (newMouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released && wasBallShoot == false)
                        shoot_ball(x_speed, -y_speed);

                    change_check = true;
                }
                //Tura Ai
                else
                {
                    double x_direction;
                    //wystrzeliwanie piłki
                    if (wasBallShoot == false)
                    {
                        //ustalić parametry
                        if (was_ai_set_up_for_a_shoot==false)
                        {
                           ai.Set_direction_of_the_shoot();

                           x_target_paddle_location = ai.generate_valid_x_location_to_shoot_ball(left_rectangle_x_location, left_rectangle_y_location, left_rectangle,
                                        right_rectangle_x_location, right_rectangle_y_location, right_rectangle,
                                        y_paddle_location,movmentVector[0],main_rectangle.Width, ball_width, paddle_width);
                            was_ai_set_up_for_a_shoot = true;
                        }
                        // jeżeli paletka osiągnęła założoną pozycję, wystrzel
                        if (x_paddle_location == x_target_paddle_location)
                        {
                            if (ai.shoot_left == 1)
                                shoot_ball(-x_speed, -y_speed);
                            else
                                shoot_ball(x_speed, -y_speed);
                            was_ai_set_up_for_a_shoot = false;
                        }
                        //jeżeli nie, przesuń paletkę
                        else
                        {
                            if (x_target_paddle_location > x_paddle_location)
                                x_paddle_location+=1;
                            else
                                x_paddle_location-=1;
                            set_up_ball();
                        }
                    }
                    if (movmentVector[1]>0)
                    {
                        if(is_negative(movmentVector[0]))
                            x_direction = -1;
                        else
                            x_direction = 1;
                        x_target_paddle_location = ai.Generate_x_where_paddle_can_deflect(x_ball_location, y_ball_location, 
                                                                                   ball_width,y_paddle_location, paddle_width,x_direction);
                        if(x_target_paddle_location>=0 && x_target_paddle_location <= (window_width - paddle.Width))
                            if (x_target_paddle_location > x_paddle_location)
                                x_paddle_location += movmentVector[1]+1 ;
                            else
                                x_paddle_location -= movmentVector[1]+1;
                    }

                    Mouse.SetPosition((int)x_paddle_location, (int)y_paddle_location);

                    if (change_check == true)
                    {
                        change_check = false;
                        change++;
                    }
                }

                if (wasBallShoot == true)
                {
                    if (is_collision_with_a_ball(main_rectangle, main_rectangle_x_location, main_rectangle_y_location))
                    {
                        if (active_player == player_type.AI)
                            points++;

                        deflect_ball();
                        reset_collision_array();
                    }
                    else if (is_collision_with_a_ball(left_rectangle, left_rectangle_x_location, left_rectangle_y_location) ||
                        is_collision_with_a_ball(right_rectangle, right_rectangle_x_location, right_rectangle_y_location) ||
                        is_collision_with_a_ball(left_wall_rectangle, 0, 0) ||
                        is_collision_with_a_ball(right_wall_rectnangle,window_width,0) ||
                        y_ball_location<=0)
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

                if (is_ball_out())
                {
                    if(active_player == player_type.Human)
                        penalty_points--;

                    switch_players();
                    set_up_ball();
                }
                if (is_game_over())
                {
                    game_state = Menu.game_mode.Summary;
                    this.IsMouseVisible = true;
                }
            }
            else if (menuObject.Get_is_game_paused().Equals(true))
                menuObject.Try_to_pause_a_game();
            //Tryb podsumowania gry
            else
            {
                if ((newMouseState.LeftButton == ButtonState.Pressed) && 
                    (oldMouseState.LeftButton == ButtonState.Released) &&
                    (newMouseState.Position.X >= (window_width / 2) - 100 && newMouseState.Position.X <= (window_width / 2) - 60) &&
                    (newMouseState.Position.Y >= 150 && newMouseState.Position.Y <= 170))
                {
                    game_state = Menu.game_mode.Game;
                    this.IsMouseVisible = false;
                    set_up_game();
                }
                else if ((newMouseState.LeftButton == ButtonState.Pressed) &&
                    (oldMouseState.LeftButton == ButtonState.Released) &&
                    (newMouseState.Position.X >= (window_width / 2) && newMouseState.Position.X <= (window_width / 2)+40) &&
                    (newMouseState.Position.Y >= 150 && newMouseState.Position.Y <= 170))
                        game_state = Menu.game_mode.Menu;
            }
            oldMouseState = newMouseState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, (int)window_width, (int)window_height), Color.White);
            spriteBatch.End();

            if (game_state == Menu.game_mode.Splash)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(splash_screen, new Rectangle(0, 0, (int)window_width, (int)window_height), Color.White);
                spriteBatch.End();
            }
            else if (game_state == Menu.game_mode.Menu)
            {
                spriteBatch.Begin();
                if(level == Ai.level.Easy_Level)
                    spriteBatch.Draw(menu, new Rectangle(0, 0, (int)window_width, (int)window_height), Color.White);
                else if(level == Ai.level.Middle_Level)
                    spriteBatch.Draw(menu_middle, new Rectangle(0, 0, (int)window_width, (int)window_height), Color.White);
                else if(level == Ai.level.Hard_Level)
                    spriteBatch.Draw(menu_hard, new Rectangle(0, 0, (int)window_width, (int)window_height), Color.White);
                spriteBatch.End();
            }
            else if (game_state == Menu.game_mode.Game)
            {
                double points_x_location = 10;

                spriteBatch.Begin();
                spriteBatch.Draw(left_rectangle, new Vector2((int)left_rectangle_x_location, (int)left_rectangle_y_location), Color.White);
                spriteBatch.Draw(right_rectangle, new Vector2((int)right_rectangle_x_location, (int)right_rectangle_y_location), Color.White);
                spriteBatch.Draw(main_rectangle, new Vector2((int)main_rectangle_x_location, (int)main_rectangle_y_location), Color.White);

                if (active_player == player_type.Human)
                    spriteBatch.Draw(paddle, new Vector2((int)x_paddle_location, (int)y_paddle_location), Color.Green);
                else
                    spriteBatch.Draw(paddle, new Vector2((int)x_paddle_location, (int)y_paddle_location), Color.White);

                spriteBatch.Draw(ball, new Vector2((int)x_ball_location, (int)y_ball_location), Color.White);
                spriteBatch.DrawString(font, "Punkty: " + points, new Vector2((int)points_x_location, 10), Color.Green);
                spriteBatch.DrawString(font, "Punkty ujemne: " + penalty_points, new Vector2((int)window_width - 180, 10), Color.Red);
                if (menuObject.Get_is_game_paused())
                    spriteBatch.DrawString(font, "PAUZA", new Vector2(((int)window_width / 2) - 20, ((int)window_height / 2) - 20), Color.White);

                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "Koniec gry", new Vector2(((int)window_width / 2) - 80, 20), Color.White);
                spriteBatch.DrawString(font, "Liczba zdobytych punktow: " + points, new Vector2(((int)window_width / 2) - 160, 60), Color.White);
                spriteBatch.DrawString(font, "Czy chcesz ropoczac nowa gre?", new Vector2(((int)window_width / 2) - 180, 110), Color.White);
                spriteBatch.DrawString(font, "TAK", new Vector2(((int)window_width / 2) - 100, 150), Color.White);
                spriteBatch.DrawString(font, "NIE", new Vector2(((int)window_width / 2), 150), Color.White);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}