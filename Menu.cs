using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squash
{
    class Menu : Game
    {
        private bool old_pause_button_state, new_pause_button_state;
        private bool is_game_paused = false;

        public enum game_mode
        {
            Splash, Menu, Game, Summary
        }

        public bool Get_is_game_paused()
        {
            return is_game_paused;
        }

        public void Try_to_pause_a_game()
        {
            new_pause_button_state = Keyboard.GetState().IsKeyDown(Keys.P);

            if (new_pause_button_state && !old_pause_button_state)
            {
                if (is_game_paused == true)
                {
                    is_game_paused = false;
                    this.IsMouseVisible = false;
                    double x_paddle_location = Game1.x_paddle_location;
                    double y_paddle_location = Game1.y_paddle_location;
                    Mouse.SetPosition((int)x_paddle_location, (int)y_paddle_location);
                }
                else
                {
                    is_game_paused = true; ;
                    this.IsMouseVisible = true;
                }
            }
            old_pause_button_state = new_pause_button_state;
        }
    }
}
