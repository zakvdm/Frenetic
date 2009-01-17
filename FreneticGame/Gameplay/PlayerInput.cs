#region File Description
//-----------------------------------------------------------------------------
// ShipInput.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
#endregion

namespace Frenetic
{
    /// <summary>
    /// Player input abstraction for ships.
    /// </summary>
    public struct PlayerInput
    {
        #region Static Constants


        /// <summary>
        /// The empty version of this structure.
        /// </summary>
        private static PlayerInput empty =
            new PlayerInput(Vector2.Zero, Vector2.Zero, false);
        public static PlayerInput Empty
        {
            get { return empty; }
        }


        #endregion


        #region Input Data


        /// <summary>
        /// The left-stick value of the ship input, used for movement.
        /// </summary>
        public Vector2 movementVector;

        public Vector2 mousePosition;

        /// <summary>
        /// If true, the player is trying to fire.
        /// </summary>
        public bool fired;


        #endregion


        #region Initialization Methods
        /// <summary>
        /// Constructs a new PlayerInput object.
        /// </summary>
        /// <param name="movementVector">The movement vector of the player input.</param>
        /// <param name="fired">If true, the player is firing.</param>
        public PlayerInput(Vector2 movementVector, Vector2 mousePosition, bool fired)
        {
            this.movementVector = movementVector;
            this.mousePosition = mousePosition;
            this.fired = fired;
        }


        /// <summary>
        /// Create a new PlayerInput object based on the data in the packet.
        /// </summary>
        /// <param name="packetReader">The packet with the data.</param>
        public PlayerInput(PacketReader packetReader)
        {
            // safety-check the parameters, as they must be valid
            if (packetReader == null)
            {
                throw new ArgumentNullException("packetReader");
            }

            // read the data
            movementVector = packetReader.ReadVector2();
            mousePosition = packetReader.ReadVector2();
            fired = packetReader.ReadBoolean();
        }


        
        public void GetButtonInput()
        {
            /*
            KeyboardState keyboardState = Keyboard.GetState();
            // check for movement action
            movementVector = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.W))
            {
                // Jump:
                movementVector += Vector2.Multiply(Vector2.UnitY, -10.0f);
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                movementVector += Vector2.Multiply(Vector2.UnitX, -0.5f);
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                //movementVector += Vector2.UnitY;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                movementVector += Vector2.Multiply(Vector2.UnitX, 0.5f);
            }

            // check for firing a mine
            fired = false;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                fired = true;
            }
            */
        }


        #endregion


        #region Networking Methods


        /// <summary>
        /// Serialize the object out to a packet.
        /// </summary>
        /// <param name="packetWriter">The packet to write to.</param>
        public void Serialize(PacketWriter packetWriter)
        {
            // safety-check the parameters, as they must be valid
            if (packetWriter == null)
            {
                throw new ArgumentNullException("packetWriter");
            }

            packetWriter.Write((int)WorldManager.PacketTypes.PlayerInput);

            packetWriter.Write(movementVector);
            packetWriter.Write(mousePosition);
            packetWriter.Write(fired);
        }


        #endregion
    }
}
