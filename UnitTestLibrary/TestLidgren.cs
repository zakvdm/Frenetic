using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Frenetic.Network.Lidgren;
using Frenetic;

using Lidgren.Network;

using NUnit.Framework;

namespace UnitTestLibrary
{
    [TestFixture]
    public class TestLidgren
    {
        INetworkSession serverNS;
        INetworkSession clientNS;

        #region Sample Test
        //[Test]
        public void runner()
        {
            LidgrenNetworkSessionFactory factory = new LidgrenNetworkSessionFactory();

            serverNS = factory.MakeServerNetworkSession();

            Thread.Sleep(500);

            clientNS = factory.MakeClientNetworkSession();
            Thread.Sleep(500);

            int sender = 0;

            while (true)
            {
                serverNS.ReadMessage();
                clientNS.ReadMessage();

                //clientNS.Send(BitConverter.GetBytes(sender), NetChannel.Unreliable);
                sender++;
            }
        }
        /*
        public void updateServer()
        {
            // create a buffer to read data into
            NetBuffer buffer = server.CreateBuffer();

            // keep running until the user presses a key
            //Console.WriteLine("Press ESC to quit server");
            bool keepRunning = true;
            //while (keepRunning)
            {
                NetMessageType type;
                INetConnection sender;

                // check if any messages has been received
                while (server.ReadMessage(buffer, out type, out sender))
                {
                    switch (type)
                    {
                        case NetMessageType.DebugMessage:
                            Console.WriteLine(buffer.ReadString());
                            break;
                        case NetMessageType.ConnectionApproval:
                            Console.WriteLine("Approval; hail is " + buffer.ReadString());
                            sender.Approve();
                            break;
                        /*
                    case NetMessageType.StatusChanged:
                        Console.WriteLine("New status for " + sender + ": " + sender.Status + " (" + buffer.ReadString() + ")");
                        break;
                    case NetMessageType.Data:
                        // A client sent this data!
                        string msg = buffer.ReadString();

                        // send to everyone, including sender
                        NetBuffer sendBuffer = server.CreateBuffer();
                        sendBuffer.Write(sender.RemoteEndpoint.ToString() + " wrote: " + msg);

                        // send using ReliableInOrder
                        server.SendToAll(sendBuffer, NetChannel.ReliableInOrder1);
                        break;
                    }
                }

                /*
				// User pressed ESC?
				while (Console.KeyAvailable)
				{
					ConsoleKeyInfo info = Console.ReadKey();
					if (info.Key == ConsoleKey.Escape)
						keepRunning = false;
				}

                //Thread.Sleep(1);
            }

            //server.Shutdown("Application exiting");
        }
        */
        //static bool s_keepGoing = true;
        /*
        public void updateClient()
        {
            // create a buffer to read data into
            NetBuffer buffer = client.CreateBuffer();

            // current input string
            string input = "";

            // keep running until the user presses a key
            //Console.WriteLine("Type 'quit' to exit client");
            s_keepGoing = true;
            //while (s_keepGoing)
            {
                NetMessageType type;

                // check if any messages has been received
                while (client.ReadMessage(buffer, out type))
                {
                    switch (type)
                    {
                        case NetMessageType.ServerDiscovered:
                            // just connect to any server found!

                            // make hail
                            NetBuffer buf = client.CreateBuffer();
                            buf.Write("Hail from " + Environment.MachineName);
                            client.Connect(buffer.ReadIPEndPoint(), buf.ToArray());
                            break;
                        /*
                    case NetMessageType.ConnectionRejected:
                        Console.WriteLine("Rejected: " + buffer.ReadString());
                        break;
                    case NetMessageType.DebugMessage:
                    case NetMessageType.VerboseDebugMessage:
                        Console.WriteLine(buffer.ReadString());
                        break;
                    case NetMessageType.StatusChanged:
                        Console.WriteLine("New status: " + client.Status + " (" + buffer.ReadString() + ")");
                        break;
                    case NetMessageType.Data:
                        // The server sent this data!
                        string msg = buffer.ReadString();
                        Console.WriteLine(msg);
                        break;
                    }
                }

                /*
				while (Console.KeyAvailable)
				{
					ConsoleKeyInfo ki = Console.ReadKey();
					if (ki.Key == ConsoleKey.Enter)
					{
						if (!string.IsNullOrEmpty(input))
						{
							if (input == "quit")
							{
								// exit application
								s_keepGoing = false;
							}
							else
							{
								// Send chat message
								NetBuffer sendBuffer = new NetBuffer();
								sendBuffer.Write(input);
								client.SendMessage(sendBuffer, NetChannel.ReliableInOrder1);
								input = "";
							}
						}
					}
					else
					{
						input += ki.KeyChar;
					}
				}

                //Thread.Sleep(1);
            }

            //client.Shutdown("Application exiting");
        }
        */
        #endregion

    }
}
