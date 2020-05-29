using System;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Chess.Service
{
    [ServiceContract(CallbackContract = typeof(IChessServiceCommuniCallback), SessionMode = SessionMode.Required)]
    public interface IChessServiceCommuni
    {
        [OperationContract(IsInitiating=true)]
        List<RoomMirror> Connect(string name);

        [OperationContract]
        RoomMirror JoinRoom(int roomID);

        [OperationContract]
        void Send(string message);

        [OperationContract]
        void Move(int srcX, int srcY, int targX, int targY);
    }

    [DataContract]
    public class RoomMirror
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public List<string> Players { get; set; }

        [DataMember]
        public List<string> Watchers { get; set; }
    }

    public class ChessRoom
    {
        public int RoomID { get; set; }

        private Dictionary<string, PlayerColor> playersColors = new Dictionary<string, PlayerColor>(2);

        private Dictionary<IChessServiceCommuniCallback, string> players = new Dictionary<IChessServiceCommuniCallback, string>(2);
        private Dictionary<string, IChessServiceCommuniCallback> playersCallbacks = new Dictionary<string, IChessServiceCommuniCallback>(2);

        private Dictionary<IChessServiceCommuniCallback, string> watchers = new Dictionary<IChessServiceCommuniCallback, string>();
        private Dictionary<string, IChessServiceCommuniCallback> watchersCallbacks = new Dictionary<string, IChessServiceCommuniCallback>();

        private List<IChessServiceCommuniCallback> callbacks = new List<IChessServiceCommuniCallback>();

        public ChessRoom(int id)
        {
            this.RoomID = id;
        }

        private bool gamestarted = false;
        private PlayerColor colorTurn;


        private void StartGame(string starterUser)
        {
            colorTurn = PlayerColor.White;
            gamestarted = true;
            foreach (string playerName in playersCallbacks.Keys)
            {
                if (playerName == starterUser)
                {
                    try
                    {
                        playersCallbacks[playerName].R_StartGame(PlayerColor.White);
                    }
                    catch
                    {
                        LeaveRoom(playerName);
                    }
                }
                else
                {
                    try
                    {
                        playersCallbacks[playerName].R_StartGame(PlayerColor.Black);
                    }
                    catch
                    {
                        LeaveRoom(playerName);
                    }
                }
            }

            Action<IChessServiceCommuniCallback> invokeAction = delegate(IChessServiceCommuniCallback callback)
            {
                try
                {
                    callback.R_StartGame(PlayerColor.White);
                }
                catch
                {
                    if (players.ContainsKey(callback))
                        players.Remove(callback);
                    if (watchers.ContainsKey(callback))
                        watchers.Remove(callback);

                    callbacks.Remove(callback);
                }
            };
            callbacks.ForEach(invokeAction);

        }

        public void MovePiece(string sender, int srcX, int srcY, int targX, int targY)
        {
            if (gamestarted && playersCallbacks.ContainsKey(sender))
            {
                if (playersColors[sender] == colorTurn)
                {
                    Action<IChessServiceCommuniCallback> invokeAction = delegate(IChessServiceCommuniCallback callback)
                    {
                        try
                        {
                            callback.R_MoveCallback(srcX, srcY, targX, targY);
                        }
                        catch
                        {
                            if (players.ContainsKey(callback))
                                players.Remove(callback);
                            if (watchers.ContainsKey(callback))
                                watchers.Remove(callback);

                            callbacks.Remove(callback);
                        }
                    };
                    callbacks.ForEach(invokeAction);

                    if (colorTurn == PlayerColor.White)
                        colorTurn = PlayerColor.Black;
                    else
                        colorTurn = PlayerColor.White;
                }
            }
        }

        public void JoinRoom(string userName, IChessServiceCommuniCallback userCallback)
        {
            if (players.Count == 0)
            {
                players.Add(userCallback, userName);
                playersCallbacks.Add(userName, userCallback);
                CallbackRoomUsers(MessageType.PlayerJoined, userName);
            }
            else if (players.Count == 1)
            {
                players.Add(userCallback, userName);
                playersCallbacks.Add(userName, userCallback);
                CallbackRoomUsers(MessageType.PlayerJoined, userName);
                StartGame(userName);
            }
            else
            {
                watchersCallbacks.Add(userName, userCallback);
                CallbackRoomUsers(MessageType.WatcherJoined, userName);
            }
        }

        public void LeaveRoom(string userName)
        {
            if (playersCallbacks.ContainsKey(userName))
            {
                players.Remove(playersCallbacks[userName]);
                callbacks.Remove(playersCallbacks[userName]);
                playersCallbacks.Remove(userName);
                CallbackRoomUsers(MessageType.PlayerLeft, userName);
            }
            if (watchersCallbacks.ContainsKey(userName))
            {
                watchers.Remove(watchersCallbacks[userName]);
                callbacks.Remove(watchersCallbacks[userName]);
                watchersCallbacks.Remove(userName);
                CallbackRoomUsers(MessageType.WatcherLeft, userName);
            }
        }

        private void CallbackRoomUsers(MessageType msgType, string userName)
        {
            Action<IChessServiceCommuniCallback> invokeAction = delegate(IChessServiceCommuniCallback callback)
            {
                try
                {
                    callback.R_ReceiveRoomMessage(msgType, userName);
                }
                catch 
                {
                    if (players.ContainsKey(callback))
                        players.Remove(callback);
                    if (watchers.ContainsKey(callback))
                        watchers.Remove(callback);

                    callbacks.Remove(callback);
                }
            };
            callbacks.ForEach(invokeAction);
        }

        public RoomMirror GetRoomMirror()
        {
            RoomMirror mirror = new RoomMirror();
            mirror.ID = RoomID;
            foreach (string playerName in players.Values)
            {
                mirror.Players.Add(playerName);
            }
            foreach (string watcherName in watchers.Values)
            {
                mirror.Watchers.Add(watcherName);
            }
            return mirror;
        }
    }

    public interface IChessServiceCommuniCallback
    {
        [OperationContract]
        void ClientConnected(string name);

        [OperationContract]
        void ClientDisconnected(string name);

        [OperationContract]
        void Receive(string message);

        [OperationContract]
        void R_StartGame(PlayerColor color);

        [OperationContract]
        void R_MoveCallback(int srcX, int srcY, int targX, int targY);

        [OperationContract]
        void R_ReceiveRoomMessage(MessageType msgType, string userName);
    }

    [DataContract]
    public enum MessageType
    {
        [EnumMember]
        PlayerJoined,
        [EnumMember]
        PlayerLeft,
        [EnumMember]
        WatcherJoined,
        [EnumMember]
        WatcherLeft
    }

    [DataContract]
    public enum PlayerColor
    {
        [EnumMember]
        White,
        [EnumMember]
        Black
    }
}
