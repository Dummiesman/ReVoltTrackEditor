/*
    Re-Volt Track Editor - Unity Edition
    A version of the track editor re-built from the ground up in Unity
    Copyright (C) 2022 Dummiesman

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Controls 
{
    public class SaveScreen
    {
        public static bool GamepadSpacePressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.buttonNorth.wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool GamepadBackspacePressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.buttonWest.wasPressedThisFrame)
                return true;
            return false;
        }
    }

    public class LoadScreen
    {
        public static bool DeletePressed()
        {
            if (Keyboard.current is Keyboard keyboard && keyboard.deleteKey.wasPressedThisFrame)
                return true;
            if (Gamepad.current is Gamepad gamepad && gamepad.buttonWest.wasPressedThisFrame)
                return true;
            return false;
        }
    }

    public class PickupEdit
    {
        public static bool PlacePressed()
        {
            if(Keyboard.current is Keyboard keyboard && keyboard.enterKey.wasPressedThisFrame)
                return true;
            if (Gamepad.current is Gamepad gamepad && gamepad.buttonSouth.wasPressedThisFrame)
                return true;
            return false;
        }
    }

    public class AdjustmemtMode
    {
        public static bool MoveButtonHeld()
        {
            if (Keyboard.current is Keyboard keyboard && (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed))
                return true;
            if (Gamepad.current is Gamepad gamepad && (gamepad.buttonWest.isPressed || gamepad.buttonNorth.isPressed))
                return true;
            return false;
        }
    }

    public class TrackEdit
    {
        public static bool DeletePressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.buttonEast.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard.deleteKey.wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool PlacePressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.buttonSouth.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard.enterKey.wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool PreviousVariantPressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.leftShoulder.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard[Key.Z].wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool NextVariantPressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.rightShoulder.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard[Key.X].wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool ToggleSurfacePressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.leftStickButton.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard[Key.T].wasPressedThisFrame)
                return true;
            return false;

        }
        public static bool CopyPressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.selectButton.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard[Key.C].wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool IncreaseHeightPressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.buttonNorth.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard.pageUpKey.wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool DecreaseHeightPressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.buttonWest.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard.pageDownKey.wasPressedThisFrame)
                return true;
            return false;
        }

        public static int GetHeightChangeDelta()
        {
            if (IncreaseHeightPressed())
                return 1;
            if (DecreaseHeightPressed())
                return -1;
            return 0;
        }

        public static int GetModuleRotationDelta()
        {
            if (Gamepad.current is Gamepad gamepad)
            {
                if (gamepad.rightTrigger.wasPressedThisFrame)
                    return 1;
                else if (gamepad.leftTrigger.wasPressedThisFrame)
                    return -1;
            }
            if (Keyboard.current is Keyboard keyboard && keyboard.spaceKey.wasPressedThisFrame)
            {
                return 1;
            }
            return 0;
        }

        public static int GetCameraRotationDelta()
        {
            if (Gamepad.current is Gamepad gamepad)
            {
                if (gamepad.rightStick.left.wasPressedThisFrame)
                    return 1;
                if (gamepad.rightStick.right.wasPressedThisFrame)
                    return -1;
            }
            if (Keyboard.current is Keyboard keyboard)
            {
                if (keyboard.numpadMinusKey.wasPressedThisFrame)
                    return 1;
                if (keyboard.numpadPlusKey.wasPressedThisFrame)
                    return -1;
            }
            return 0;
        }
    }

    public class DirectionalMovement
    {
        public static bool AnalogUpPressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.leftStick.up.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard.upArrowKey.wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool AnalogDownPressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.leftStick.down.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard.downArrowKey.wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool AnalogLeftPressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.leftStick.left.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard.leftArrowKey.wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool AnalogRightPressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.leftStick.right.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard.rightArrowKey.wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool DigitalUpPressed()
        {
            if(Gamepad.current is Gamepad gamepad && gamepad.dpad.up.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard.upArrowKey.wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool DigitalDownPressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.dpad.down.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard.downArrowKey.wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool DigitalLeftPressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.dpad.left.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard.leftArrowKey.wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool DigitalRightPressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.dpad.right.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard.rightArrowKey.wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool UpPressed()
        {
            return AnalogUpPressed() || DigitalUpPressed();
        }

        public static bool DownPressed()
        {
            return AnalogDownPressed() || DigitalDownPressed();
        }

        public static bool LeftPressed()
        {
            return AnalogLeftPressed() || DigitalLeftPressed();
        }

        public static bool RightPressed()
        {
            return AnalogRightPressed() || DigitalRightPressed();
        }

        public static Vector2Int GetDigitalMovementDelta()
        {
            Vector2Int vec = Vector2Int.zero;
            if (DigitalLeftPressed())
                vec.x -= 1;
            if (DigitalRightPressed())
                vec.x += 1;
            if (DigitalDownPressed())
                vec.y -= 1;
            if (DigitalUpPressed())
                vec.y += 1;
            return vec;
        }

        public static Vector2Int GetAnalogMovementDelta()
        {
            Vector2Int vec = Vector2Int.zero;
            if (AnalogLeftPressed())
                vec.x -= 1;
            if (AnalogRightPressed())
                vec.x += 1;
            if (AnalogDownPressed())
                vec.y -= 1;
            if (AnalogUpPressed())
                vec.y += 1;
            return vec;
        }

        public static Vector2Int GetMovementDelta()
        {
            Vector2Int vec = Vector2Int.zero;
            if (LeftPressed())
                vec.x -= 1;
            if (RightPressed())
                vec.x += 1;
            if (DownPressed())
                vec.y -= 1;
            if (UpPressed())
                vec.y += 1;
            return vec;
        }
    }

    public class UI
    {
        public static bool ConfirmPressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.buttonSouth.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard.enterKey.wasPressedThisFrame)
                return true;
            return false;
        }

        public static bool BackPressed()
        {
            if (Gamepad.current is Gamepad gamepad && gamepad.buttonEast.wasPressedThisFrame)
                return true;
            if (Keyboard.current is Keyboard keyboard && keyboard.escapeKey.wasPressedThisFrame)
                return true;
            return false;
        }
    }

    public static bool AnyButtonPressed()
    {
        if (Gamepad.current is Gamepad gamepad && gamepad.allControls.Any(x => x is ButtonControl button && x.IsPressed() && !x.synthetic))
            return true;
        if (Keyboard.current is Keyboard keyboard && keyboard.anyKey.wasPressedThisFrame)
            return true;
        return false;
    }

    public static bool PausePressed()
    {
        if (Gamepad.current is Gamepad gamepad && gamepad.startButton.wasPressedThisFrame)
            return true;
        if (Keyboard.current is Keyboard keyboard && keyboard.escapeKey.wasPressedThisFrame)
            return true;
        return false;
    }

    public static bool GetKeyUp(Key key)
    {
        return Keyboard.current is Keyboard keyboard && keyboard[key].wasReleasedThisFrame;
    }

    public static bool GetKeyDown(Key key)
    {
        return Keyboard.current is Keyboard keyboard && keyboard[key].wasPressedThisFrame;
    }

    public static bool GetKey(Key key)
    {
        return Keyboard.current is Keyboard keyboard && keyboard[key].isPressed;
    }
}
