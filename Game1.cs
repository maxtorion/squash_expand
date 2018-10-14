using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace Squash
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        struct ball_colision_point {

            private int x_location;
            private int y_location;

            public int Y_location { get => y_location; set => y_location = value; }
            public int X_location { get => x_location; set => x_location = value; }

          

            public void  update_location(int x_location,int y_location)
            {
                X_location = x_location;
                Y_location = y_location;

            }
        }

        private ball_colision_point top_point;
        private ball_colision_point bottom_point;
        private ball_colision_point left_point;
        private ball_colision_point right_point;

        private int window_width;
        private int window_height;

        private Texture2D background;
        private Texture2D paddle;
        private Texture2D ball;
        private Texture2D frame;

        private Texture2D main_rectangle;
        private Texture2D left_rectangle;
        private Texture2D right_rectangle;

        private SpriteFont font;

        private MouseState newMouseState;
        private MouseState oldMouseState;

        private bool wasBallShoot;

        private int[] movmentVector = { 0, 0 };
        private int x_speed = 4;
        private int y_speed = 7;

        private int points;
        private int penalty_points;

        private bool[] where_colision= {false,false,false,false};
        //[0] - lewa
        //[1] - prawa
        //[2] - góra
        //[3] - dół


        private int x_paddle_location;
        private int y_paddle_location;

      
        private int x_ball_location;
        private int y_ball_location;

  

        private int main_rectangle_x_location;
        private int main_rectangle_y_location;

        private int left_rectangle_x_location;
        private int left_rectangle_y_location;

        private int right_rectangle_x_location;
        private int right_rectangle_y_location;



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
            y_ball_location = y_paddle_location - 50;

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
        protected void reset_collision_array()
        {
            for (int i = 0; i < where_colision.Length; i++)
            {

                where_colision[i] = false;

            }

        }

        private bool is_intersection(Texture2D collision_object, int collision_object_x, int collision_object_y, ball_colision_point coll_point)
        {
            bool answer = false;


            if ((coll_point.X_location + movmentVector[0] >= collision_object_x)
                && (coll_point.X_location + movmentVector[0] <= (collision_object_x + collision_object.Width))
                && (coll_point.Y_location + movmentVector[1] >= collision_object_y)
                && (coll_point.Y_location + movmentVector[1] <= (collision_object_y + collision_object.Height)))
            {

                answer = true;
            }

            return answer;
        }

        

      

        protected bool is_collision_with_a_ball(Texture2D collision_object, int collision_object_x, int collision_object_y)
        {

            bool answer = false;


            if(is_intersection(collision_object, collision_object_x, collision_object_y, right_point))
            {
                where_colision[0] = true;
            }
            else if (is_intersection(collision_object, collision_object_x, collision_object_y, left_point))
            {
                where_colision[1] = true;
            }
            else if (is_intersection(collision_object, collision_object_x, collision_object_y, top_point))
            {
                where_colision[2] = true;
            }
            else if (is_intersection(collision_object, collision_object_x, collision_object_y, bottom_point))
            {
                where_colision[3] = true;
            }




            for (int i = 0; i < where_colision.Length; i++)
            {
                if (where_colision[i] == true)
                {
                    answer = true;
                }

            }



            return answer;

        }

        public bool is_negative(int number)
        {
            return number < 0;
        }

        protected void deflect_ball()
        {

            //czy w poziomie
            if ((where_colision[0] == true) || (where_colision[1] == true))
            {
                shoot_ball(movmentVector[0] * (-1), movmentVector[1]);

            }
            else
            {
                shoot_ball(movmentVector[0], movmentVector[1] * (-1));
            }



        }

        protected bool is_ball_out()
        {
            bool answer = false;

            if ((x_ball_location + ball.Width <= 0) || (x_ball_location >= window_width) || (y_ball_location + ball.Width <= 0) || (y_ball_location >= window_height))
                answer = true;

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

            main_rectangle = Content.Load<Texture2D>("images\\duzy_prostokat");
            left_rectangle = Content.Load<Texture2D>("images\\maly_prostokat");
            right_rectangle = Content.Load<Texture2D>("images\\maly_prostokat");
            background = Content.Load<Texture2D>("images\\tlo");
            ball = Content.Load<Texture2D>("images\\pilka");
            paddle = Content.Load<Texture2D>("images\\paletka");
            font = Content.Load<SpriteFont>("fonts\\wynik");

           
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

            update_window_bounds();

            if (newMouseState.X >= 0 && newMouseState.X <= window_width - paddle.Width)
            {
                x_paddle_location = newMouseState.X;
            }


            if (wasBallShoot == false)
            {
                x_ball_location = x_paddle_location + 30;
            }

            newMouseState = Mouse.GetState();
            if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released && wasBallShoot == false)
            {
                shoot_ball(-x_speed, -y_speed);

            }
            if (newMouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released && wasBallShoot == false)
            {

                shoot_ball(x_speed, -y_speed);

            }
            oldMouseState = newMouseState;

            if (wasBallShoot == true)
            {


                if (is_collision_with_a_ball(main_rectangle, main_rectangle_x_location, main_rectangle_y_location)
                    || is_collision_with_a_ball(left_rectangle, left_rectangle_x_location, left_rectangle_y_location)
                    || is_collision_with_a_ball(right_rectangle, right_rectangle_x_location, right_rectangle_y_location)
                    || is_collision_with_a_ball(paddle, x_paddle_location, y_paddle_location))
                {
                    points++;
                    deflect_ball();
                    reset_collision_array();
                    
                }

                x_ball_location += movmentVector[0];
                y_ball_location += movmentVector[1];

                top_point.update_location(x_ball_location+ball.Width/2,y_ball_location);
                left_point.update_location(x_ball_location, y_ball_location + ball.Width / 2);
                right_point.update_location(x_ball_location + ball.Width, y_ball_location + ball.Height / 2);
                bottom_point.update_location(x_ball_location + ball.Width / 2, y_ball_location+ ball.Height);
            }

            if (is_ball_out())
            {
                penalty_points--;
                set_up_ball();

            }




            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            int points_x_location = 10;


            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Rectangle(0, 0, window_width, window_height), Color.White);
            spriteBatch.Draw(main_rectangle, new Vector2(main_rectangle_x_location, main_rectangle_y_location), Color.White);
            spriteBatch.Draw(left_rectangle, new Vector2(left_rectangle_x_location, left_rectangle_y_location), Color.White);
            spriteBatch.Draw(right_rectangle, new Vector2(right_rectangle_x_location, right_rectangle_y_location), Color.White);
            spriteBatch.Draw(paddle, new Vector2(x_paddle_location, y_paddle_location), Color.White);
            spriteBatch.Draw(ball, new Vector2(x_ball_location, y_ball_location), Color.White);
            spriteBatch.DrawString(font, "Punkty: " + points, new Vector2(points_x_location, 10), Color.Green);
            spriteBatch.DrawString(font, "Punkty ujemne: " + penalty_points, new Vector2(window_width - 180, 10)
                , Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
