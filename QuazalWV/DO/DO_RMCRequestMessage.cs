﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuazalWV
{
    public static class DO_RMCRequestMessage
    {
        enum DOC_METHOD
        {
            AddDuplicaLocation = 1,
            DeleteDuplica = 2,
            RemoveFromCachedDuplicationSet = 3,
            AdjustTime = 4,
            SyncRequest = 5,
            SyncResponse = 6,
            AskForSettingPlayerParameters = 7,
            AskForSettingPlayerState = 8,
            SetPlayerParameters = 9,
            SetPlayerState = 10,
            SetPlayerIdentity = 11,
            SetPlayerRDVInfo = 12,
            AskForSettingSessionParameters = 13,
            Disconnect = 14,
            IncreasePlayerNb = 15,
            OnEndMatch = 16,
            OnStartMatch = 17,
            PlayVoiceChat = 18,
            PlayVoiceChatWithMutedPlayers = 19,
            UpdateSessionHostAfterMigration = 20,
            RequestIDRangeFromMaster = 21,
            ConfirmElection = 22,
            DeclinePromotion = 23,
            ElectNewMaster = 24,
            KickOut = 25,
            ReportFault = 26,
            RetrieveURLs = 27,
            SynchronizeTermination = 28,
            SignalAsFaulty = 29,
            ProcessMessage = 30,
            RouteMessage = 31
        }

        public static byte[] HandleMessage(ClientInfo client, byte[] data)
        {
            Log.WriteLine(1, "[DO] Handling DO_RMCRequestMessage...");
            MemoryStream m = new MemoryStream(data);
            m.Seek(1, 0);
            ushort callID = Helper.ReadU16(m);
            uint flags = Helper.ReadU32(m);
            uint station = Helper.ReadU32(m);
            uint targetObject = Helper.ReadU32(m);
            DOC_METHOD method = (DOC_METHOD)Helper.ReadU16(m);
            Log.WriteLine(2, "[DO] RMC Call ID      : 0x" + callID.ToString("X4"));
            Log.WriteLine(2, "[DO] RMC Call Flags   : 0x" + flags.ToString("X8"));
            Log.WriteLine(2, "[DO] RMC Call Station : 0x" + station.ToString("X8"));
            Log.WriteLine(2, "[DO] RMC Call DupObj  : 0x" + targetObject.ToString("X8") + " " + Helper.DupObjToStr(targetObject));
            switch (method)
            {
                case DOC_METHOD.SyncRequest:
                    Log.WriteLine(1, "[DO] Handling DOC_SyncRequest...");
                    ulong time = Helper.ReadU64(m);
                    return Create(callID, 0x83C, Helper.MakeDupObj(DO.CLASS.DOC_Station, 1), Helper.MakeDupObj(DO.CLASS.DOC_SessionClock, 1), 6, Payload_SyncResponse.Create(time));
                default:
                    Log.WriteLine(1, "[DO] Error: Unhandled DOC method: " + method + "!");
                    return new byte[0];
            }
        }

        public static byte[] Create(ushort callID, uint flags, uint station, uint target, ushort method, byte[] payload)
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU8(m, 0xA);
            Helper.WriteU16(m, callID);
            Helper.WriteU32(m, flags);
            Helper.WriteU32(m, station);
            Helper.WriteU32(m, target);
            Helper.WriteU16(m, method);
            m.Write(payload, 0, payload.Length);
            return m.ToArray();
        }
    }
}