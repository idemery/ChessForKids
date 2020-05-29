using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Chess.Service
{
    public class ChessService : IChessServiceCommuni
    {
        private static List<RoomMirror> roomMirrors = new List<RoomMirror>(6);

        private static Dictionary<string, int> roomIDs = new Dictionary<string, int>();
        private static List<ChessRoom> rooms = new List<ChessRoom>(6);

        public IChessServiceCommuniCallback CurrentUserCallback { get { return OperationContext.Current.GetCallbackChannel<IChessServiceCommuniCallback>(); } }
        public IContextChannel CurrentUserChannel { get { return OperationContext.Current.Channel; } }
        private static object syncObj = new object();
        private static Dictionary<IContextChannel, string> usersChannels = new Dictionary<IContextChannel, string>();

        private static Dictionary<string, IChessServiceCommuniCallback> usersCallbacks = new Dictionary<string, IChessServiceCommuniCallback>();
        private static List<IChessServiceCommuniCallback> callbacks = new List<IChessServiceCommuniCallback>();
        

        void CurrentUserChannel_Faulted(object sender, EventArgs e)
        {
            HandleChannel(sender as IContextChannel);
        }
        void CurrentUserChannel_Closed(object sender, EventArgs e)
        {
            HandleChannel(sender as IContextChannel);
        }
        void HandleChannel(IContextChannel channel)
        {
            if (channel.State == CommunicationState.Closed || channel.State == CommunicationState.Faulted)
            {
                string userName;
                bool channelExist = usersChannels.TryGetValue(channel, out userName);
                if (channelExist)
                {
                    lock (syncObj)
                    {
                        if (usersChannels.ContainsKey(channel))
                        {
                            usersChannels.Remove(channel);
                            rooms[roomIDs[userName]].LeaveRoom(userName);

                            callbacks.Remove(usersCallbacks[userName]);
                            usersCallbacks.Remove(userName);
                            roomIDs.Remove(userName);

                            CallbackUsers("Server: " + userName + " left game.");
                        }
                    }
                }
            }
        }

        private void CallbackUsers(string message)
        {
            Action<IChessServiceCommuniCallback> invokeAction = delegate(IChessServiceCommuniCallback callback)
            {
                try
                {
                    callback.Receive(message);
                }
                catch 
                {
                    callbacks.Remove(callback);
                }
            };
            callbacks.ForEach(invokeAction);
        }
        

        public ChessService()
        {
            for (int i = 0; i < 6; i++)
            {
                ChessRoom room = new ChessRoom(i);
                room.RoomID = i;
                rooms.Add(room);

                RoomMirror roomMirror = new RoomMirror();
                roomMirror.ID = i;
                roomMirrors.Add(roomMirror);
            }
        }

        #region IChessServiceCommuni Members

        public List<RoomMirror> Connect(string name)
        {
            lock (syncObj)
            {
                if (usersChannels.ContainsValue(name))
                {
                    name += "1";
                }


                usersChannels.Add(CurrentUserChannel, name);

                if (usersCallbacks.ContainsKey(name))
                    usersCallbacks.Remove(name);

                usersCallbacks.Add(name, CurrentUserCallback);

                if (callbacks.Contains(CurrentUserCallback))
                    callbacks.Remove(CurrentUserCallback);

                callbacks.Add(CurrentUserCallback);
            }

            CurrentUserChannel.Closed += new EventHandler(CurrentUserChannel_Closed);
            CurrentUserChannel.Faulted += new EventHandler(CurrentUserChannel_Faulted);

            //return rooms;
            List<RoomMirror> roomMrrs = new List<RoomMirror>();
            for (int i = 0; i < rooms.Count; i++)
            {
                roomMrrs.Add(rooms[i].GetRoomMirror());
            }
            return roomMrrs;
        }

        public RoomMirror JoinRoom(int roomID)
        {   
            string userName = usersChannels[CurrentUserChannel];
            RoomMirror mirror = null;
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].RoomID == roomID)
                {
                    rooms[i].JoinRoom(userName, CurrentUserCallback);
                    mirror = rooms[i].GetRoomMirror();
                    roomIDs.Add(userName, roomID);
                    break;
                }
            }
            return mirror;
        }

        public void Send(string message)
        {
            string userName = usersChannels[CurrentUserChannel];
            CallbackUsers(userName + " : " + message);
        }

        public void Move(int srcX, int srcY, int targX, int targY)
        {
            string userName = usersChannels[CurrentUserChannel];
            int roomID = roomIDs[userName];
            rooms[roomID].MovePiece(userName, srcX, srcY, targX, targY);
        }

        #endregion
    }
}
