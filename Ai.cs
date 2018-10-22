using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Squash
{
    class Ai
    {

       
        private Random random_generator = new Random();
        private int noise_parameter = 0;
        private int counter_to_error;


        //jeżeli 1 to strzelaj w lewo, jak 0 to w prawo
        public int shoot_left;

        public struct Point
        {
            public int x_coordinate { get; set; }
            public int y_coordinate { get; set; }

        }

        public void set_direction_of_the_shoot()
        {

            this.shoot_left = random_generator.Next(0, 2);

        }
        //TODO: przyjrzeć się temu równaniu
        public int generate_valid_x_location_to_shoot_ball(int left_rectangle_x_location,int left_rectangle_y_location, Texture2D left_rectangle, 
                                                           int right_rectangle_x_location,int right_rectangle_y_location  ,Texture2D right_rectangle,
                                                           int paddle_y ,int delta_X, int main_rectangle_width, int ball_width,int paddle_width)
        {
            int x = 0;
            if (shoot_left==1)
            {
                int alfa = left_rectangle_x_location + left_rectangle.Width;
                int distance_from_ball = (paddle_y-ball_width) - (left_rectangle_y_location + left_rectangle.Height);
                int lower_bound = alfa + (delta_X * distance_from_ball);
                int upper_bound = lower_bound + main_rectangle_width+1;

                x = random_generator.Next(lower_bound, upper_bound)+(paddle_width/2)-(ball_width/2);


            }
            else
            {
                int betta = right_rectangle_x_location;
                int distance_from_ball = (paddle_y-ball_width) - (right_rectangle_y_location + right_rectangle.Height);
                int upper_bound = betta + (delta_X * distance_from_ball)-1;
                int lower_bound = upper_bound - main_rectangle_width;

                x = random_generator.Next(lower_bound, upper_bound)-paddle_width-ball_width;


            }
            return x;

        }

        public int generate_x_where_paddle_can_deflect(int x_ball_location, int y_ball_location,int ball_width, 
                                                       int y_paddle_location,int paddle_width , int direction_x)
        {
            int x = 0;
            int ball_bottom_location = y_ball_location + ball_width;
            int center_of_the_ball_x = x_ball_location + (ball_width / 2);
            int distance_from_paddle = y_paddle_location - ball_bottom_location;

          
            x = center_of_the_ball_x + (distance_from_paddle * direction_x)+(noise_parameter*paddle_width);


            return x;
        }

    }
}
