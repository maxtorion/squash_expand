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
        private double noise_parameter = 0;
        private double counter_to_error;

        internal void Set_noise_parameter()
        {
            noise_parameter = 0;
        }

        //jeżeli 1 to strzelaj w lewo, jak 0 to w prawo
        public double shoot_left;

        public enum level
        {
            Easy_Level, Middle_Level, Hard_Level
        }

        public struct Podouble
        {
            public double x_coordinate { get; set; }
            public double y_coordinate { get; set; }
        }

        public void Set_direction_of_the_shoot()
        {
               this.shoot_left = random_generator.Next(0, 2);
        }
        //TODO: przyjrzeć się temu równaniu
        public double generate_valid_x_location_to_shoot_ball(double left_rectangle_x_location, double left_rectangle_y_location, Texture2D left_rectangle, 
                      double right_rectangle_x_location, double right_rectangle_y_location, Texture2D right_rectangle,
                      double paddle_y, double delta_X, double main_rectangle_width, double ball_width, double paddle_width)
        {
            double x = 0;
            if (shoot_left==1)
            {
                double alfa = left_rectangle_x_location + left_rectangle.Width;
                double distance_from_ball = (paddle_y-ball_width) - (left_rectangle_y_location + left_rectangle.Height);
                double lower_bound = alfa + (delta_X * distance_from_ball);
                double upper_bound = lower_bound + main_rectangle_width+1;

                x = random_generator.Next((int)lower_bound, (int)upper_bound)+(paddle_width/2)-(ball_width/2);
            }
            else
            {
                double betta = right_rectangle_x_location;
                double distance_from_ball = (paddle_y-ball_width) - (right_rectangle_y_location + right_rectangle.Height);
                double upper_bound = betta + (delta_X * distance_from_ball)-1;
                double lower_bound = upper_bound - main_rectangle_width;

                x = random_generator.Next((int)lower_bound, (int)upper_bound)-paddle_width-ball_width;
            }
            return x;
        }

        double chaneg_new = 0;
        double change_old = 0;

        public double Generate_x_where_paddle_can_deflect(double x_ball_location, double y_ball_location,double ball_width, 
                      double y_paddle_location,double paddle_width , double direction_x)
        {
            double x = 0;
            double ball_bottom_location = y_ball_location + ball_width;
            double center_of_the_ball_x = x_ball_location + (ball_width / 2);
            double distance_from_paddle = y_paddle_location - ball_bottom_location;

            chaneg_new = Game1.Get_change();
            if (change_old != chaneg_new)
            {
                change_old = chaneg_new;
                if (Game1.Get_level() == level.Easy_Level) noise_parameter += 0.005;
                else if (Game1.Get_level() == level.Middle_Level) noise_parameter += 0.003;
                else if (Game1.Get_level() == level.Hard_Level) noise_parameter += 0.001;
            }
            Console.WriteLine("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB" + noise_parameter);
            x = center_of_the_ball_x + (distance_from_paddle * direction_x)+(noise_parameter*paddle_width);

            return x;
        }

    }
}
