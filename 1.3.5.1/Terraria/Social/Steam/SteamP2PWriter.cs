﻿// Decompiled with JetBrains decompiler
// Type: Terraria.Social.Steam.SteamP2PWriter
// Assembly: TerrariaServer, Version=1.3.5.1, Culture=neutral, PublicKeyToken=null
// MVID: 5CBA2320-074B-43F7-8CDC-BF1E2B81EE4B
// Assembly location: C:\Users\kevzhao\Downloads\TerrariaServer.exe

using Steamworks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Terraria.Social.Steam
{
  public class SteamP2PWriter
  {
    private Dictionary<CSteamID, Queue<SteamP2PWriter.WriteInformation>> _pendingSendData = new Dictionary<CSteamID, Queue<SteamP2PWriter.WriteInformation>>();
    private Dictionary<CSteamID, Queue<SteamP2PWriter.WriteInformation>> _pendingSendDataSwap = new Dictionary<CSteamID, Queue<SteamP2PWriter.WriteInformation>>();
    private Queue<byte[]> _bufferPool = new Queue<byte[]>();
    private object _lock = new object();
    private const int BUFFER_SIZE = 1024;
    private int _channel;

    public SteamP2PWriter(int channel)
    {
      this._channel = channel;
    }

    public void QueueSend(CSteamID user, byte[] data, int length)
    {
      bool flag = false;
      object obj;
      try
      {
        Monitor.Enter(obj = this._lock, ref flag);
        Queue<SteamP2PWriter.WriteInformation> writeInformationQueue;
        if (this._pendingSendData.ContainsKey(user))
          writeInformationQueue = this._pendingSendData[user];
        else
          this._pendingSendData[user] = writeInformationQueue = new Queue<SteamP2PWriter.WriteInformation>();
        int val1 = length;
        int sourceIndex = 0;
        while (val1 > 0)
        {
          SteamP2PWriter.WriteInformation writeInformation;
          if (writeInformationQueue.Count == 0 || 1024 - writeInformationQueue.Peek().Size == 0)
          {
            writeInformation = this._bufferPool.Count <= 0 ? new SteamP2PWriter.WriteInformation() : new SteamP2PWriter.WriteInformation(this._bufferPool.Dequeue());
            writeInformationQueue.Enqueue(writeInformation);
          }
          else
            writeInformation = writeInformationQueue.Peek();
          int length1 = Math.Min(val1, 1024 - writeInformation.Size);
          Array.Copy((Array) data, sourceIndex, (Array) writeInformation.Data, writeInformation.Size, length1);
          writeInformation.Size += length1;
          val1 -= length1;
          sourceIndex += length1;
        }
      }
      finally
      {
        if (flag)
          Monitor.Exit(obj);
      }
    }

    public void ClearUser(CSteamID user)
    {
      bool flag = false;
      object obj;
      try
      {
        Monitor.Enter(obj = this._lock, ref flag);
        if (this._pendingSendData.ContainsKey(user))
        {
          Queue<SteamP2PWriter.WriteInformation> writeInformationQueue = this._pendingSendData[user];
          while (writeInformationQueue.Count > 0)
            this._bufferPool.Enqueue(writeInformationQueue.Dequeue().Data);
        }
        if (!this._pendingSendDataSwap.ContainsKey(user))
          return;
        Queue<SteamP2PWriter.WriteInformation> writeInformationQueue1 = this._pendingSendDataSwap[user];
        while (writeInformationQueue1.Count > 0)
          this._bufferPool.Enqueue(writeInformationQueue1.Dequeue().Data);
      }
      finally
      {
        if (flag)
          Monitor.Exit(obj);
      }
    }

    public void SendAll()
    {
      bool flag = false;
      object obj;
      try
      {
        Monitor.Enter(obj = this._lock, ref flag);
        Utils.Swap<Dictionary<CSteamID, Queue<SteamP2PWriter.WriteInformation>>>(ref this._pendingSendData, ref this._pendingSendDataSwap);
      }
      finally
      {
        if (flag)
          Monitor.Exit(obj);
      }
      using (Dictionary<CSteamID, Queue<SteamP2PWriter.WriteInformation>>.Enumerator enumerator = this._pendingSendDataSwap.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          KeyValuePair<CSteamID, Queue<SteamP2PWriter.WriteInformation>> current = enumerator.Current;
          Queue<SteamP2PWriter.WriteInformation> writeInformationQueue = current.Value;
          while (writeInformationQueue.Count > 0)
          {
            SteamP2PWriter.WriteInformation writeInformation = writeInformationQueue.Dequeue();
            SteamNetworking.SendP2PPacket(current.Key, writeInformation.Data, (uint) writeInformation.Size, (EP2PSend) 2, this._channel);
            this._bufferPool.Enqueue(writeInformation.Data);
          }
        }
      }
    }

    public class WriteInformation
    {
      public byte[] Data;
      public int Size;

      public WriteInformation()
      {
        this.Data = new byte[1024];
        this.Size = 0;
      }

      public WriteInformation(byte[] data)
      {
        this.Data = data;
        this.Size = 0;
      }
    }
  }
}
