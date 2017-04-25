﻿// Decompiled with JetBrains decompiler
// Type: Terraria.MessageBuffer
// Assembly: TerrariaServer, Version=1.3.5.3, Culture=neutral, PublicKeyToken=null
// MVID: AA3606A2-F3DB-4481-937B-7295FB97CD3E
// Assembly location: E:\TSHOCK\TerrariaServer.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Events;
using Terraria.GameContent.Tile_Entities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Net;
using Terraria.UI;

namespace Terraria
{
  public class MessageBuffer
  {
    public byte[] readBuffer = new byte[131070];
    public byte[] writeBuffer = new byte[131070];
    public const int readBufferMax = 131070;
    public const int writeBufferMax = 131070;
    public bool broadcast;
    public bool writeLocked;
    public int messageLength;
    public int totalData;
    public int whoAmI;
    public int spamCount;
    public int maxSpam;
    public bool checkBytes;
    public MemoryStream readerStream;
    public MemoryStream writerStream;
    public BinaryReader reader;
    public BinaryWriter writer;

    public static event TileChangeReceivedEvent OnTileChangeReceived;

    public void Reset()
    {
      Array.Clear((Array) this.readBuffer, 0, this.readBuffer.Length);
      Array.Clear((Array) this.writeBuffer, 0, this.writeBuffer.Length);
      this.writeLocked = false;
      this.messageLength = 0;
      this.totalData = 0;
      this.spamCount = 0;
      this.broadcast = false;
      this.checkBytes = false;
      this.ResetReader();
      this.ResetWriter();
    }

    public void ResetReader()
    {
      if (this.readerStream != null)
        this.readerStream.Close();
      this.readerStream = new MemoryStream(this.readBuffer);
      this.reader = new BinaryReader((Stream) this.readerStream);
    }

    public void ResetWriter()
    {
      if (this.writerStream != null)
        this.writerStream.Close();
      this.writerStream = new MemoryStream(this.writeBuffer);
      this.writer = new BinaryWriter((Stream) this.writerStream);
    }

    public void GetData(int start, int length, out int messageType)
    {
      if (this.whoAmI < 256)
        Netplay.Clients[this.whoAmI].TimeOutTimer = 0;
      else
        Netplay.Connection.TimeOutTimer = 0;
      int bufferStart = start + 1;
      byte num1 = this.readBuffer[start];
      messageType = (int) num1;
      if ((int) num1 >= 120)
        return;
      ++Main.rxMsg;
      Main.rxData += length;
      ++Main.rxMsgType[(int) num1];
      Main.rxDataType[(int) num1] += length;
      if (Main.netMode == 1 && Netplay.Connection.StatusMax > 0)
        ++Netplay.Connection.StatusCount;
      if (Main.verboseNetplay)
      {
        int num2 = start;
        while (num2 < start + length)
          ++num2;
        for (int index = start; index < start + length; ++index)
        {
          int num3 = (int) this.readBuffer[index];
        }
      }
      if (Main.netMode == 2 && (int) num1 != 38 && Netplay.Clients[this.whoAmI].State == -1)
      {
        NetMessage.SendData(2, this.whoAmI, -1, Lang.mp[1].ToNetworkText(), 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
      }
      else
      {
        if (Main.netMode == 2 && Netplay.Clients[this.whoAmI].State < 10 && ((int) num1 > 12 && (int) num1 != 93) && ((int) num1 != 16 && (int) num1 != 42 && ((int) num1 != 50 && (int) num1 != 38)) && (int) num1 != 68)
          NetMessage.BootPlayer(this.whoAmI, Lang.mp[2].ToNetworkText());
        if (this.reader == null)
          this.ResetReader();
        this.reader.BaseStream.Position = (long) bufferStart;
        switch (num1)
        {
          case 1:
            if (Main.netMode != 2)
              break;
            if (Main.dedServ && Netplay.IsBanned(Netplay.Clients[this.whoAmI].Socket.GetRemoteAddress()))
            {
              NetMessage.SendData(2, this.whoAmI, -1, Lang.mp[3].ToNetworkText(), 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              break;
            }
            if (Netplay.Clients[this.whoAmI].State != 0)
              break;
            if (this.reader.ReadString() == "Terraria" + (object) 194)
            {
              if (string.IsNullOrEmpty(Netplay.ServerPassword))
              {
                Netplay.Clients[this.whoAmI].State = 1;
                NetMessage.SendData(3, this.whoAmI, -1, (NetworkText) null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                break;
              }
              Netplay.Clients[this.whoAmI].State = -1;
              NetMessage.SendData(37, this.whoAmI, -1, (NetworkText) null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              break;
            }
            NetMessage.SendData(2, this.whoAmI, -1, Lang.mp[4].ToNetworkText(), 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 2:
            if (Main.netMode != 1)
              break;
            Netplay.disconnect = true;
            Main.statusText = NetworkText.Deserialize(this.reader).ToString();
            break;
          case 3:
            if (Main.netMode != 1)
              break;
            if (Netplay.Connection.State == 1)
              Netplay.Connection.State = 2;
            int number1 = (int) this.reader.ReadByte();
            if (number1 != Main.myPlayer)
            {
              Main.player[number1] = Main.ActivePlayerFileData.Player;
              Main.player[Main.myPlayer] = new Player();
            }
            Main.player[number1].whoAmI = number1;
            Main.myPlayer = number1;
            Player player1 = Main.player[number1];
            NetMessage.SendData(4, -1, -1, (NetworkText) null, number1, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            NetMessage.SendData(68, -1, -1, (NetworkText) null, number1, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            NetMessage.SendData(16, -1, -1, (NetworkText) null, number1, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            NetMessage.SendData(42, -1, -1, (NetworkText) null, number1, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            NetMessage.SendData(50, -1, -1, (NetworkText) null, number1, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            for (int index = 0; index < 59; ++index)
              NetMessage.SendData(5, -1, -1, (NetworkText) null, number1, (float) index, (float) player1.inventory[index].prefix, 0.0f, 0, 0, 0);
            for (int index = 0; index < player1.armor.Length; ++index)
              NetMessage.SendData(5, -1, -1, (NetworkText) null, number1, (float) (59 + index), (float) player1.armor[index].prefix, 0.0f, 0, 0, 0);
            for (int index = 0; index < player1.dye.Length; ++index)
              NetMessage.SendData(5, -1, -1, (NetworkText) null, number1, (float) (58 + player1.armor.Length + 1 + index), (float) player1.dye[index].prefix, 0.0f, 0, 0, 0);
            for (int index = 0; index < player1.miscEquips.Length; ++index)
              NetMessage.SendData(5, -1, -1, (NetworkText) null, number1, (float) (58 + player1.armor.Length + player1.dye.Length + 1 + index), (float) player1.miscEquips[index].prefix, 0.0f, 0, 0, 0);
            for (int index = 0; index < player1.miscDyes.Length; ++index)
              NetMessage.SendData(5, -1, -1, (NetworkText) null, number1, (float) (58 + player1.armor.Length + player1.dye.Length + player1.miscEquips.Length + 1 + index), (float) player1.miscDyes[index].prefix, 0.0f, 0, 0, 0);
            for (int index = 0; index < player1.bank.item.Length; ++index)
              NetMessage.SendData(5, -1, -1, (NetworkText) null, number1, (float) (58 + player1.armor.Length + player1.dye.Length + player1.miscEquips.Length + player1.miscDyes.Length + 1 + index), (float) player1.bank.item[index].prefix, 0.0f, 0, 0, 0);
            for (int index = 0; index < player1.bank2.item.Length; ++index)
              NetMessage.SendData(5, -1, -1, (NetworkText) null, number1, (float) (58 + player1.armor.Length + player1.dye.Length + player1.miscEquips.Length + player1.miscDyes.Length + player1.bank.item.Length + 1 + index), (float) player1.bank2.item[index].prefix, 0.0f, 0, 0, 0);
            NetMessage.SendData(5, -1, -1, (NetworkText) null, number1, (float) (58 + player1.armor.Length + player1.dye.Length + player1.miscEquips.Length + player1.miscDyes.Length + player1.bank.item.Length + player1.bank2.item.Length + 1), (float) player1.trashItem.prefix, 0.0f, 0, 0, 0);
            for (int index = 0; index < player1.bank3.item.Length; ++index)
              NetMessage.SendData(5, -1, -1, (NetworkText) null, number1, (float) (58 + player1.armor.Length + player1.dye.Length + player1.miscEquips.Length + player1.miscDyes.Length + player1.bank.item.Length + player1.bank2.item.Length + 2 + index), (float) player1.bank3.item[index].prefix, 0.0f, 0, 0, 0);
            NetMessage.SendData(6, -1, -1, (NetworkText) null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            if (Netplay.Connection.State != 2)
              break;
            Netplay.Connection.State = 3;
            break;
          case 4:
            int number2 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              number2 = this.whoAmI;
            if (number2 == Main.myPlayer && !Main.ServerSideCharacter)
              break;
            Player player2 = Main.player[number2];
            player2.whoAmI = number2;
            player2.skinVariant = (int) this.reader.ReadByte();
            player2.skinVariant = (int) MathHelper.Clamp((float) player2.skinVariant, 0.0f, 9f);
            player2.hair = (int) this.reader.ReadByte();
            if (player2.hair >= 134)
              player2.hair = 0;
            player2.name = this.reader.ReadString().Trim().Trim();
            player2.hairDye = this.reader.ReadByte();
            BitsByte bitsByte1 = (BitsByte) this.reader.ReadByte();
            for (int index = 0; index < 8; ++index)
              player2.hideVisual[index] = bitsByte1[index];
            bitsByte1 = (BitsByte) this.reader.ReadByte();
            for (int index = 0; index < 2; ++index)
              player2.hideVisual[index + 8] = bitsByte1[index];
            player2.hideMisc = (BitsByte) this.reader.ReadByte();
            player2.hairColor = this.reader.ReadRGB();
            player2.skinColor = this.reader.ReadRGB();
            player2.eyeColor = this.reader.ReadRGB();
            player2.shirtColor = this.reader.ReadRGB();
            player2.underShirtColor = this.reader.ReadRGB();
            player2.pantsColor = this.reader.ReadRGB();
            player2.shoeColor = this.reader.ReadRGB();
            BitsByte bitsByte2 = (BitsByte) this.reader.ReadByte();
            player2.difficulty = (byte) 0;
            if (bitsByte2[0])
              ++player2.difficulty;
            if (bitsByte2[1])
              player2.difficulty += (byte) 2;
            if ((int) player2.difficulty > 2)
              player2.difficulty = (byte) 2;
            player2.extraAccessory = bitsByte2[2];
            if (Main.netMode != 2)
              break;
            bool flag1 = false;
            if (Netplay.Clients[this.whoAmI].State < 10)
            {
              for (int index = 0; index < (int) byte.MaxValue; ++index)
              {
                if (index != number2 && player2.name == Main.player[index].name && Netplay.Clients[index].IsActive)
                  flag1 = true;
              }
            }
            if (flag1)
            {
              NetMessage.SendData(2, this.whoAmI, -1, NetworkText.FromFormattable("{0} {1}", (object) player2.name, (object) Lang.mp[5].ToNetworkText()), 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              break;
            }
            if (player2.name.Length > Player.nameLen)
            {
              NetMessage.SendData(2, this.whoAmI, -1, NetworkText.FromKey("Net.NameTooLong"), 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              break;
            }
            if (player2.name == "")
            {
              NetMessage.SendData(2, this.whoAmI, -1, NetworkText.FromKey("Net.EmptyName"), 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              break;
            }
            Netplay.Clients[this.whoAmI].Name = player2.name;
            Netplay.Clients[this.whoAmI].Name = player2.name;
            NetMessage.SendData(4, -1, this.whoAmI, (NetworkText) null, number2, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 5:
            int number3 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              number3 = this.whoAmI;
            if (number3 == Main.myPlayer && !Main.ServerSideCharacter && !Main.player[number3].IsStackingItems())
              break;
            Player player3 = Main.player[number3];
            lock (player3)
            {
              int local_26 = (int) this.reader.ReadByte();
              int local_27 = (int) this.reader.ReadInt16();
              int local_28 = (int) this.reader.ReadByte();
              int local_29 = (int) this.reader.ReadInt16();
              Item[] local_30 = (Item[]) null;
              int local_31 = 0;
              bool local_32 = false;
              if (local_26 > 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length + player3.bank.item.Length + player3.bank2.item.Length + 1)
              {
                local_31 = local_26 - 58 - (player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length + player3.bank.item.Length + player3.bank2.item.Length + 1) - 1;
                local_30 = player3.bank3.item;
              }
              else if (local_26 > 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length + player3.bank.item.Length + player3.bank2.item.Length)
                local_32 = true;
              else if (local_26 > 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length + player3.bank.item.Length)
              {
                local_31 = local_26 - 58 - (player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length + player3.bank.item.Length) - 1;
                local_30 = player3.bank2.item;
              }
              else if (local_26 > 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length)
              {
                local_31 = local_26 - 58 - (player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length) - 1;
                local_30 = player3.bank.item;
              }
              else if (local_26 > 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length)
              {
                local_31 = local_26 - 58 - (player3.armor.Length + player3.dye.Length + player3.miscEquips.Length) - 1;
                local_30 = player3.miscDyes;
              }
              else if (local_26 > 58 + player3.armor.Length + player3.dye.Length)
              {
                local_31 = local_26 - 58 - (player3.armor.Length + player3.dye.Length) - 1;
                local_30 = player3.miscEquips;
              }
              else if (local_26 > 58 + player3.armor.Length)
              {
                local_31 = local_26 - 58 - player3.armor.Length - 1;
                local_30 = player3.dye;
              }
              else if (local_26 > 58)
              {
                local_31 = local_26 - 58 - 1;
                local_30 = player3.armor;
              }
              else
              {
                local_31 = local_26;
                local_30 = player3.inventory;
              }
              if (local_32)
              {
                player3.trashItem = new Item();
                player3.trashItem.netDefaults(local_29);
                player3.trashItem.stack = local_27;
                player3.trashItem.Prefix(local_28);
              }
              else if (local_26 <= 58)
              {
                int local_33 = local_30[local_31].type;
                int local_34 = local_30[local_31].stack;
                local_30[local_31] = new Item();
                local_30[local_31].netDefaults(local_29);
                local_30[local_31].stack = local_27;
                local_30[local_31].Prefix(local_28);
                if (number3 == Main.myPlayer && local_31 == 58)
                  Main.mouseItem = local_30[local_31].Clone();
                if (number3 == Main.myPlayer && Main.netMode == 1)
                {
                  Main.player[number3].inventoryChestStack[local_26] = false;
                  if (local_30[local_31].stack != local_34 || local_30[local_31].type != local_33)
                  {
                    Recipe.FindRecipes();
                    Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
                  }
                }
              }
              else
              {
                local_30[local_31] = new Item();
                local_30[local_31].netDefaults(local_29);
                local_30[local_31].stack = local_27;
                local_30[local_31].Prefix(local_28);
              }
              if (Main.netMode != 2 || number3 != this.whoAmI || local_26 > 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length)
                break;
              NetMessage.SendData(5, -1, this.whoAmI, (NetworkText) null, number3, (float) local_26, (float) local_28, 0.0f, 0, 0, 0);
              break;
            }
          case 6:
            if (Main.netMode != 2)
              break;
            if (Netplay.Clients[this.whoAmI].State == 1)
              Netplay.Clients[this.whoAmI].State = 2;
            NetMessage.SendData(7, this.whoAmI, -1, (NetworkText) null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            Main.SyncAnInvasion(this.whoAmI);
            break;
          case 7:
            if (Main.netMode != 1)
              break;
            Main.time = (double) this.reader.ReadInt32();
            BitsByte bitsByte3 = (BitsByte) this.reader.ReadByte();
            Main.dayTime = bitsByte3[0];
            Main.bloodMoon = bitsByte3[1];
            Main.eclipse = bitsByte3[2];
            Main.moonPhase = (int) this.reader.ReadByte();
            Main.maxTilesX = (int) this.reader.ReadInt16();
            Main.maxTilesY = (int) this.reader.ReadInt16();
            Main.spawnTileX = (int) this.reader.ReadInt16();
            Main.spawnTileY = (int) this.reader.ReadInt16();
            Main.worldSurface = (double) this.reader.ReadInt16();
            Main.rockLayer = (double) this.reader.ReadInt16();
            Main.worldID = this.reader.ReadInt32();
            Main.worldName = this.reader.ReadString();
            Main.ActiveWorldFileData.UniqueId = new Guid(this.reader.ReadBytes(16));
            Main.ActiveWorldFileData.WorldGeneratorVersion = this.reader.ReadUInt64();
            Main.moonType = (int) this.reader.ReadByte();
            WorldGen.setBG(0, (int) this.reader.ReadByte());
            WorldGen.setBG(1, (int) this.reader.ReadByte());
            WorldGen.setBG(2, (int) this.reader.ReadByte());
            WorldGen.setBG(3, (int) this.reader.ReadByte());
            WorldGen.setBG(4, (int) this.reader.ReadByte());
            WorldGen.setBG(5, (int) this.reader.ReadByte());
            WorldGen.setBG(6, (int) this.reader.ReadByte());
            WorldGen.setBG(7, (int) this.reader.ReadByte());
            Main.iceBackStyle = (int) this.reader.ReadByte();
            Main.jungleBackStyle = (int) this.reader.ReadByte();
            Main.hellBackStyle = (int) this.reader.ReadByte();
            Main.windSpeedSet = this.reader.ReadSingle();
            Main.numClouds = (int) this.reader.ReadByte();
            for (int index = 0; index < 3; ++index)
              Main.treeX[index] = this.reader.ReadInt32();
            for (int index = 0; index < 4; ++index)
              Main.treeStyle[index] = (int) this.reader.ReadByte();
            for (int index = 0; index < 3; ++index)
              Main.caveBackX[index] = this.reader.ReadInt32();
            for (int index = 0; index < 4; ++index)
              Main.caveBackStyle[index] = (int) this.reader.ReadByte();
            Main.maxRaining = this.reader.ReadSingle();
            Main.raining = (double) Main.maxRaining > 0.0;
            BitsByte bitsByte4 = (BitsByte) this.reader.ReadByte();
            WorldGen.shadowOrbSmashed = bitsByte4[0];
            NPC.downedBoss1 = bitsByte4[1];
            NPC.downedBoss2 = bitsByte4[2];
            NPC.downedBoss3 = bitsByte4[3];
            Main.hardMode = bitsByte4[4];
            NPC.downedClown = bitsByte4[5];
            Main.ServerSideCharacter = bitsByte4[6];
            NPC.downedPlantBoss = bitsByte4[7];
            BitsByte bitsByte5 = (BitsByte) this.reader.ReadByte();
            NPC.downedMechBoss1 = bitsByte5[0];
            NPC.downedMechBoss2 = bitsByte5[1];
            NPC.downedMechBoss3 = bitsByte5[2];
            NPC.downedMechBossAny = bitsByte5[3];
            Main.cloudBGActive = bitsByte5[4] ? 1f : 0.0f;
            WorldGen.crimson = bitsByte5[5];
            Main.pumpkinMoon = bitsByte5[6];
            Main.snowMoon = bitsByte5[7];
            BitsByte bitsByte6 = (BitsByte) this.reader.ReadByte();
            Main.expertMode = bitsByte6[0];
            Main.fastForwardTime = bitsByte6[1];
            Main.UpdateSundial();
            int num2 = bitsByte6[2] ? 1 : 0;
            NPC.downedSlimeKing = bitsByte6[3];
            NPC.downedQueenBee = bitsByte6[4];
            NPC.downedFishron = bitsByte6[5];
            NPC.downedMartians = bitsByte6[6];
            NPC.downedAncientCultist = bitsByte6[7];
            BitsByte bitsByte7 = (BitsByte) this.reader.ReadByte();
            NPC.downedMoonlord = bitsByte7[0];
            NPC.downedHalloweenKing = bitsByte7[1];
            NPC.downedHalloweenTree = bitsByte7[2];
            NPC.downedChristmasIceQueen = bitsByte7[3];
            NPC.downedChristmasSantank = bitsByte7[4];
            NPC.downedChristmasTree = bitsByte7[5];
            NPC.downedGolemBoss = bitsByte7[6];
            BirthdayParty.ManualParty = bitsByte7[7];
            BitsByte bitsByte8 = (BitsByte) this.reader.ReadByte();
            NPC.downedPirates = bitsByte8[0];
            NPC.downedFrost = bitsByte8[1];
            NPC.downedGoblins = bitsByte8[2];
            Sandstorm.Happening = bitsByte8[3];
            DD2Event.Ongoing = bitsByte8[4];
            DD2Event.DownedInvasionT1 = bitsByte8[5];
            DD2Event.DownedInvasionT2 = bitsByte8[6];
            DD2Event.DownedInvasionT3 = bitsByte8[7];
            if (num2 != 0)
              Main.StartSlimeRain(true);
            else
              Main.StopSlimeRain(true);
            Main.invasionType = (int) this.reader.ReadSByte();
            Main.LobbyId = this.reader.ReadUInt64();
            Sandstorm.IntendedSeverity = this.reader.ReadSingle();
            if (Netplay.Connection.State != 3)
              break;
            Netplay.Connection.State = 4;
            break;
          case 8:
            if (Main.netMode != 2)
              break;
            int num3 = this.reader.ReadInt32();
            int y1 = this.reader.ReadInt32();
            bool flag2 = true;
            if (num3 == -1 || y1 == -1)
              flag2 = false;
            else if (num3 < 10 || num3 > Main.maxTilesX - 10)
              flag2 = false;
            else if (y1 < 10 || y1 > Main.maxTilesY - 10)
              flag2 = false;
            int number4 = Netplay.GetSectionX(Main.spawnTileX) - 2;
            int num4 = Netplay.GetSectionY(Main.spawnTileY) - 1;
            int num5 = number4 + 5;
            int num6 = num4 + 3;
            if (number4 < 0)
              number4 = 0;
            if (num5 >= Main.maxSectionsX)
              num5 = Main.maxSectionsX - 1;
            if (num4 < 0)
              num4 = 0;
            if (num6 >= Main.maxSectionsY)
              num6 = Main.maxSectionsY - 1;
            int num7 = (num5 - number4) * (num6 - num4);
            List<Point> dontInclude = new List<Point>();
            for (int x = number4; x < num5; ++x)
            {
              for (int y2 = num4; y2 < num6; ++y2)
                dontInclude.Add(new Point(x, y2));
            }
            int num8 = -1;
            int num9 = -1;
            if (flag2)
            {
              num3 = Netplay.GetSectionX(num3) - 2;
              y1 = Netplay.GetSectionY(y1) - 1;
              num8 = num3 + 5;
              num9 = y1 + 3;
              if (num3 < 0)
                num3 = 0;
              if (num8 >= Main.maxSectionsX)
                num8 = Main.maxSectionsX - 1;
              if (y1 < 0)
                y1 = 0;
              if (num9 >= Main.maxSectionsY)
                num9 = Main.maxSectionsY - 1;
              for (int x = num3; x < num8; ++x)
              {
                for (int y2 = y1; y2 < num9; ++y2)
                {
                  if (x < number4 || x >= num5 || (y2 < num4 || y2 >= num6))
                  {
                    dontInclude.Add(new Point(x, y2));
                    ++num7;
                  }
                }
              }
            }
            int num10 = 1;
            List<Point> portals;
            List<Point> portalCenters;
            PortalHelper.SyncPortalsOnPlayerJoin(this.whoAmI, 1, dontInclude, out portals, out portalCenters);
            int number5 = num7 + portals.Count;
            if (Netplay.Clients[this.whoAmI].State == 2)
              Netplay.Clients[this.whoAmI].State = 3;
            NetMessage.SendData(9, this.whoAmI, -1, Lang.inter[44].ToNetworkText(), number5, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            Netplay.Clients[this.whoAmI].StatusText2 = Language.GetTextValue("Net.IsReceivingTileData");
            Netplay.Clients[this.whoAmI].StatusMax += number5;
            for (int sectionX = number4; sectionX < num5; ++sectionX)
            {
              for (int sectionY = num4; sectionY < num6; ++sectionY)
                NetMessage.SendSection(this.whoAmI, sectionX, sectionY, false);
            }
            NetMessage.SendData(11, this.whoAmI, -1, (NetworkText) null, number4, (float) num4, (float) (num5 - 1), (float) (num6 - 1), 0, 0, 0);
            if (flag2)
            {
              for (int sectionX = num3; sectionX < num8; ++sectionX)
              {
                for (int sectionY = y1; sectionY < num9; ++sectionY)
                  NetMessage.SendSection(this.whoAmI, sectionX, sectionY, true);
              }
              NetMessage.SendData(11, this.whoAmI, -1, (NetworkText) null, num3, (float) y1, (float) (num8 - 1), (float) (num9 - 1), 0, 0, 0);
            }
            for (int index = 0; index < portals.Count; ++index)
              NetMessage.SendSection(this.whoAmI, portals[index].X, portals[index].Y, true);
            for (int index = 0; index < portalCenters.Count; ++index)
              NetMessage.SendData(11, this.whoAmI, -1, (NetworkText) null, portalCenters[index].X - num10, (float) (portalCenters[index].Y - num10), (float) (portalCenters[index].X + num10 + 1), (float) (portalCenters[index].Y + num10 + 1), 0, 0, 0);
            for (int number6 = 0; number6 < 400; ++number6)
            {
              if (Main.item[number6].active)
              {
                NetMessage.SendData(21, this.whoAmI, -1, (NetworkText) null, number6, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                NetMessage.SendData(22, this.whoAmI, -1, (NetworkText) null, number6, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              }
            }
            for (int number6 = 0; number6 < 200; ++number6)
            {
              if (Main.npc[number6].active)
                NetMessage.SendData(23, this.whoAmI, -1, (NetworkText) null, number6, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            }
            for (int number6 = 0; number6 < 1000; ++number6)
            {
              if (Main.projectile[number6].active && (Main.projPet[Main.projectile[number6].type] || Main.projectile[number6].netImportant))
                NetMessage.SendData(27, this.whoAmI, -1, (NetworkText) null, number6, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            }
            for (int number6 = 0; number6 < 267; ++number6)
              NetMessage.SendData(83, this.whoAmI, -1, (NetworkText) null, number6, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            NetMessage.SendData(49, this.whoAmI, -1, (NetworkText) null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            NetMessage.SendData(57, this.whoAmI, -1, (NetworkText) null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            NetMessage.SendData(7, this.whoAmI, -1, (NetworkText) null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            NetMessage.SendData(103, -1, -1, (NetworkText) null, NPC.MoonLordCountdown, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            NetMessage.SendData(101, this.whoAmI, -1, (NetworkText) null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 9:
            if (Main.netMode != 1)
              break;
            Netplay.Connection.StatusMax += this.reader.ReadInt32();
            Netplay.Connection.StatusText = NetworkText.Deserialize(this.reader).ToString();
            break;
          case 10:
            if (Main.netMode != 1)
              break;
            NetMessage.DecompressTileBlock(this.readBuffer, bufferStart, length);
            break;
          case 11:
            if (Main.netMode != 1)
              break;
            WorldGen.SectionTileFrame((int) this.reader.ReadInt16(), (int) this.reader.ReadInt16(), (int) this.reader.ReadInt16(), (int) this.reader.ReadInt16());
            break;
          case 12:
            int index1 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              index1 = this.whoAmI;
            Player player4 = Main.player[index1];
            int num11 = (int) this.reader.ReadInt16();
            player4.SpawnX = num11;
            int num12 = (int) this.reader.ReadInt16();
            player4.SpawnY = num12;
            player4.Spawn();
            if (index1 == Main.myPlayer && Main.netMode != 2)
            {
              Main.ActivePlayerFileData.StartPlayTimer();
              Player.Hooks.EnterWorld(Main.myPlayer);
            }
            if (Main.netMode != 2 || Netplay.Clients[this.whoAmI].State < 3)
              break;
            if (Netplay.Clients[this.whoAmI].State == 3)
            {
              Netplay.Clients[this.whoAmI].State = 10;
              NetMessage.greetPlayer(this.whoAmI);
              NetMessage.buffer[this.whoAmI].broadcast = true;
              NetMessage.SyncConnectedPlayer(this.whoAmI);
              NetMessage.SendData(12, -1, this.whoAmI, (NetworkText) null, this.whoAmI, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              NetMessage.SendData(74, this.whoAmI, -1, NetworkText.FromLiteral(Main.player[this.whoAmI].name), Main.anglerQuest, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              break;
            }
            NetMessage.SendData(12, -1, this.whoAmI, (NetworkText) null, this.whoAmI, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 13:
            int number7 = (int) this.reader.ReadByte();
            if (number7 == Main.myPlayer && !Main.ServerSideCharacter)
              break;
            if (Main.netMode == 2)
              number7 = this.whoAmI;
            Player player5 = Main.player[number7];
            BitsByte bitsByte9 = (BitsByte) this.reader.ReadByte();
            player5.controlUp = bitsByte9[0];
            player5.controlDown = bitsByte9[1];
            player5.controlLeft = bitsByte9[2];
            player5.controlRight = bitsByte9[3];
            player5.controlJump = bitsByte9[4];
            player5.controlUseItem = bitsByte9[5];
            player5.direction = bitsByte9[6] ? 1 : -1;
            BitsByte bitsByte10 = (BitsByte) this.reader.ReadByte();
            if (bitsByte10[0])
            {
              player5.pulley = true;
              player5.pulleyDir = bitsByte10[1] ? (byte) 2 : (byte) 1;
            }
            else
              player5.pulley = false;
            player5.selectedItem = (int) this.reader.ReadByte();
            player5.position = this.reader.ReadVector2();
            if (bitsByte10[2])
              player5.velocity = this.reader.ReadVector2();
            else
              player5.velocity = Vector2.Zero;
            player5.vortexStealthActive = bitsByte10[3];
            player5.gravDir = bitsByte10[4] ? 1f : -1f;
            if (Main.netMode != 2 || Netplay.Clients[this.whoAmI].State != 10)
              break;
            NetMessage.SendData(13, -1, this.whoAmI, (NetworkText) null, number7, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 14:
            int playerIndex = (int) this.reader.ReadByte();
            int num13 = (int) this.reader.ReadByte();
            if (Main.netMode != 1)
              break;
            int num14 = Main.player[playerIndex].active ? 1 : 0;
            if (num13 == 1)
            {
              if (!Main.player[playerIndex].active)
                Main.player[playerIndex] = new Player();
              Main.player[playerIndex].active = true;
            }
            else
              Main.player[playerIndex].active = false;
            int num15 = Main.player[playerIndex].active ? 1 : 0;
            if (num14 == num15)
              break;
            if (Main.player[playerIndex].active)
            {
              Player.Hooks.PlayerConnect(playerIndex);
              break;
            }
            Player.Hooks.PlayerDisconnect(playerIndex);
            break;
          case 16:
            int number8 = (int) this.reader.ReadByte();
            if (number8 == Main.myPlayer && !Main.ServerSideCharacter)
              break;
            if (Main.netMode == 2)
              number8 = this.whoAmI;
            Player player6 = Main.player[number8];
            player6.statLife = (int) this.reader.ReadInt16();
            player6.statLifeMax = (int) this.reader.ReadInt16();
            if (player6.statLifeMax < 100)
              player6.statLifeMax = 100;
            player6.dead = player6.statLife <= 0;
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(16, -1, this.whoAmI, (NetworkText) null, number8, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 17:
            byte num16 = this.reader.ReadByte();
            int index2 = (int) this.reader.ReadInt16();
            int index3 = (int) this.reader.ReadInt16();
            short num17 = this.reader.ReadInt16();
            int num18 = (int) this.reader.ReadByte();
            bool fail = (int) num17 == 1;
            if (!WorldGen.InWorld(index2, index3, 3))
              break;
            if (Main.tile[index2, index3] == null)
              Main.tile[index2, index3] = new Tile();
            if (Main.netMode == 2)
            {
              if (!fail)
              {
                if ((int) num16 == 0 || (int) num16 == 2 || (int) num16 == 4)
                  ++Netplay.Clients[this.whoAmI].SpamDeleteBlock;
                if ((int) num16 == 1 || (int) num16 == 3)
                  ++Netplay.Clients[this.whoAmI].SpamAddBlock;
              }
              if (!Netplay.Clients[this.whoAmI].TileSections[Netplay.GetSectionX(index2), Netplay.GetSectionY(index3)])
                fail = true;
            }
            if ((int) num16 == 0)
              WorldGen.KillTile(index2, index3, fail, false, false);
            if ((int) num16 == 1)
              WorldGen.PlaceTile(index2, index3, (int) num17, false, true, -1, num18);
            if ((int) num16 == 2)
              WorldGen.KillWall(index2, index3, fail);
            if ((int) num16 == 3)
              WorldGen.PlaceWall(index2, index3, (int) num17, false);
            if ((int) num16 == 4)
              WorldGen.KillTile(index2, index3, fail, false, true);
            if ((int) num16 == 5)
              WorldGen.PlaceWire(index2, index3);
            if ((int) num16 == 6)
              WorldGen.KillWire(index2, index3);
            if ((int) num16 == 7)
              WorldGen.PoundTile(index2, index3);
            if ((int) num16 == 8)
              WorldGen.PlaceActuator(index2, index3);
            if ((int) num16 == 9)
              WorldGen.KillActuator(index2, index3);
            if ((int) num16 == 10)
              WorldGen.PlaceWire2(index2, index3);
            if ((int) num16 == 11)
              WorldGen.KillWire2(index2, index3);
            if ((int) num16 == 12)
              WorldGen.PlaceWire3(index2, index3);
            if ((int) num16 == 13)
              WorldGen.KillWire3(index2, index3);
            if ((int) num16 == 14)
              WorldGen.SlopeTile(index2, index3, (int) num17);
            if ((int) num16 == 15)
              Minecart.FrameTrack(index2, index3, true, false);
            if ((int) num16 == 16)
              WorldGen.PlaceWire4(index2, index3);
            if ((int) num16 == 17)
              WorldGen.KillWire4(index2, index3);
            if ((int) num16 == 18)
            {
              Wiring.SetCurrentUser(this.whoAmI);
              Wiring.PokeLogicGate(index2, index3);
              Wiring.SetCurrentUser(-1);
              break;
            }
            if ((int) num16 == 19)
            {
              Wiring.SetCurrentUser(this.whoAmI);
              Wiring.Actuate(index2, index3);
              Wiring.SetCurrentUser(-1);
              break;
            }
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(17, -1, this.whoAmI, (NetworkText) null, (int) num16, (float) index2, (float) index3, (float) num17, num18, 0, 0);
            if ((int) num16 != 1 || (int) num17 != 53)
              break;
            NetMessage.SendTileSquare(-1, index2, index3, 1, TileChangeType.None);
            break;
          case 18:
            if (Main.netMode != 1)
              break;
            Main.dayTime = (int) this.reader.ReadByte() == 1;
            Main.time = (double) this.reader.ReadInt32();
            Main.sunModY = this.reader.ReadInt16();
            Main.moonModY = this.reader.ReadInt16();
            break;
          case 19:
            byte num19 = this.reader.ReadByte();
            int num20 = (int) this.reader.ReadInt16();
            int num21 = (int) this.reader.ReadInt16();
            if (!WorldGen.InWorld(num20, num21, 3))
              break;
            int direction1 = (int) this.reader.ReadByte() == 0 ? -1 : 1;
            if ((int) num19 == 0)
              WorldGen.OpenDoor(num20, num21, direction1);
            else if ((int) num19 == 1)
              WorldGen.CloseDoor(num20, num21, true);
            else if ((int) num19 == 2)
              WorldGen.ShiftTrapdoor(num20, num21, direction1 == 1, 1);
            else if ((int) num19 == 3)
              WorldGen.ShiftTrapdoor(num20, num21, direction1 == 1, 0);
            else if ((int) num19 == 4)
              WorldGen.ShiftTallGate(num20, num21, false);
            else if ((int) num19 == 5)
              WorldGen.ShiftTallGate(num20, num21, true);
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(19, -1, this.whoAmI, (NetworkText) null, (int) num19, (float) num20, (float) num21, direction1 == 1 ? 1f : 0.0f, 0, 0, 0);
            break;
          case 20:
            int num22 = (int) this.reader.ReadUInt16();
            int maxValue = (int) short.MaxValue;
            short num23 = (short) (num22 & maxValue);
            int num24 = 32768;
            int num25 = (uint) (num22 & num24) > 0U ? 1 : 0;
            byte num26 = 0;
            if (num25 != 0)
              num26 = this.reader.ReadByte();
            int num27 = (int) this.reader.ReadInt16();
            int num28 = (int) this.reader.ReadInt16();
            if (!WorldGen.InWorld(num27, num28, 3))
              break;
            TileChangeType type1 = TileChangeType.None;
            if (Enum.IsDefined(typeof (TileChangeType), (object) num26))
              type1 = (TileChangeType) num26;
            // ISSUE: reference to a compiler-generated field
            if (MessageBuffer.OnTileChangeReceived != null)
            {
              // ISSUE: reference to a compiler-generated field
              MessageBuffer.OnTileChangeReceived(num27, num28, (int) num23, type1);
            }
            BitsByte bitsByte11 = (BitsByte) (byte) 0;
            BitsByte bitsByte12 = (BitsByte) (byte) 0;
            for (int index4 = num27; index4 < num27 + (int) num23; ++index4)
            {
              for (int index5 = num28; index5 < num28 + (int) num23; ++index5)
              {
                if (Main.tile[index4, index5] == null)
                  Main.tile[index4, index5] = new Tile();
                Tile tile = Main.tile[index4, index5];
                bool flag3 = tile.active();
                BitsByte bitsByte13 = (BitsByte) this.reader.ReadByte();
                BitsByte bitsByte14 = (BitsByte) this.reader.ReadByte();
                tile.active(bitsByte13[0]);
                tile.wall = bitsByte13[2] ? (byte) 1 : (byte) 0;
                bool flag4 = bitsByte13[3];
                if (Main.netMode != 2)
                  tile.liquid = flag4 ? (byte) 1 : (byte) 0;
                tile.wire(bitsByte13[4]);
                tile.halfBrick(bitsByte13[5]);
                tile.actuator(bitsByte13[6]);
                tile.inActive(bitsByte13[7]);
                tile.wire2(bitsByte14[0]);
                tile.wire3(bitsByte14[1]);
                if (bitsByte14[2])
                  tile.color(this.reader.ReadByte());
                if (bitsByte14[3])
                  tile.wallColor(this.reader.ReadByte());
                if (tile.active())
                {
                  int type2 = (int) tile.type;
                  tile.type = this.reader.ReadUInt16();
                  if (Main.tileFrameImportant[(int) tile.type])
                  {
                    tile.frameX = this.reader.ReadInt16();
                    tile.frameY = this.reader.ReadInt16();
                  }
                  else if (!flag3 || (int) tile.type != type2)
                  {
                    tile.frameX = (short) -1;
                    tile.frameY = (short) -1;
                  }
                  byte slope = 0;
                  if (bitsByte14[4])
                    ++slope;
                  if (bitsByte14[5])
                    slope += (byte) 2;
                  if (bitsByte14[6])
                    slope += (byte) 4;
                  tile.slope(slope);
                }
                tile.wire4(bitsByte14[7]);
                if ((int) tile.wall > 0)
                  tile.wall = this.reader.ReadByte();
                if (flag4)
                {
                  tile.liquid = this.reader.ReadByte();
                  tile.liquidType((int) this.reader.ReadByte());
                }
              }
            }
            WorldGen.RangeFrame(num27, num28, num27 + (int) num23, num28 + (int) num23);
            if (Main.netMode != 2)
              break;
            NetMessage.SendData((int) num1, -1, this.whoAmI, (NetworkText) null, (int) num23, (float) num27, (float) num28, 0.0f, 0, 0, 0);
            break;
          case 21:
          case 90:
            int index6 = (int) this.reader.ReadInt16();
            Vector2 vector2_1 = this.reader.ReadVector2();
            Vector2 vector2_2 = this.reader.ReadVector2();
            int Stack = (int) this.reader.ReadInt16();
            int pre1 = (int) this.reader.ReadByte();
            int num29 = (int) this.reader.ReadByte();
            int type3 = (int) this.reader.ReadInt16();
            if (Main.netMode == 1)
            {
              if (type3 == 0)
              {
                Main.item[index6].active = false;
                break;
              }
              int index4 = index6;
              Item obj = Main.item[index4];
              bool flag3 = (obj.newAndShiny || obj.netID != type3) && ItemSlot.Options.HighlightNewItems && (type3 < 0 || type3 >= 3930 || !ItemID.Sets.NeverShiny[type3]);
              obj.netDefaults(type3);
              obj.newAndShiny = flag3;
              obj.Prefix(pre1);
              obj.stack = Stack;
              obj.position = vector2_1;
              obj.velocity = vector2_2;
              obj.active = true;
              if ((int) num1 == 90)
              {
                obj.instanced = true;
                obj.owner = Main.myPlayer;
                obj.keepTime = 600;
              }
              obj.wet = Collision.WetCollision(obj.position, obj.width, obj.height);
              break;
            }
            if (Main.itemLockoutTime[index6] > 0)
              break;
            if (type3 == 0)
            {
              if (index6 >= 400)
                break;
              Main.item[index6].active = false;
              NetMessage.SendData(21, -1, -1, (NetworkText) null, index6, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              break;
            }
            bool flag5 = false;
            if (index6 == 400)
              flag5 = true;
            if (flag5)
            {
              Item obj = new Item();
              obj.netDefaults(type3);
              index6 = Item.NewItem((int) vector2_1.X, (int) vector2_1.Y, obj.width, obj.height, obj.type, Stack, true, 0, false, false);
            }
            Item obj1 = Main.item[index6];
            int type4 = type3;
            obj1.netDefaults(type4);
            int pre2 = pre1;
            obj1.Prefix(pre2);
            int num30 = Stack;
            obj1.stack = num30;
            Vector2 vector2_3 = vector2_1;
            obj1.position = vector2_3;
            Vector2 vector2_4 = vector2_2;
            obj1.velocity = vector2_4;
            int num31 = 1;
            obj1.active = num31 != 0;
            int player7 = Main.myPlayer;
            obj1.owner = player7;
            if (flag5)
            {
              NetMessage.SendData(21, -1, -1, (NetworkText) null, index6, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              if (num29 == 0)
              {
                Main.item[index6].ownIgnore = this.whoAmI;
                Main.item[index6].ownTime = 100;
              }
              Main.item[index6].FindOwner(index6);
              break;
            }
            NetMessage.SendData(21, -1, this.whoAmI, (NetworkText) null, index6, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 22:
            int number9 = (int) this.reader.ReadInt16();
            int num32 = (int) this.reader.ReadByte();
            if (Main.netMode == 2 && Main.item[number9].owner != this.whoAmI)
              break;
            Main.item[number9].owner = num32;
            Main.item[number9].keepTime = num32 != Main.myPlayer ? 0 : 15;
            if (Main.netMode != 2)
              break;
            Main.item[number9].owner = (int) byte.MaxValue;
            Main.item[number9].keepTime = 15;
            NetMessage.SendData(22, -1, -1, (NetworkText) null, number9, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 23:
            if (Main.netMode != 1)
              break;
            int index7 = (int) this.reader.ReadInt16();
            Vector2 vector2_5 = this.reader.ReadVector2();
            Vector2 vector2_6 = this.reader.ReadVector2();
            int num33 = (int) this.reader.ReadUInt16();
            if (num33 == (int) ushort.MaxValue)
              num33 = 0;
            BitsByte bitsByte15 = (BitsByte) this.reader.ReadByte();
            float[] numArray1 = new float[NPC.maxAI];
            for (int index4 = 0; index4 < NPC.maxAI; ++index4)
              numArray1[index4] = !bitsByte15[index4 + 2] ? 0.0f : this.reader.ReadSingle();
            int Type1 = (int) this.reader.ReadInt16();
            int num34 = 0;
            if (!bitsByte15[7])
            {
              switch (this.reader.ReadByte())
              {
                case 2:
                  num34 = (int) this.reader.ReadInt16();
                  break;
                case 4:
                  num34 = this.reader.ReadInt32();
                  break;
                default:
                  num34 = (int) this.reader.ReadSByte();
                  break;
              }
            }
            int oldType = -1;
            NPC npc1 = Main.npc[index7];
            if (!npc1.active || npc1.netID != Type1)
            {
              if (npc1.active)
                oldType = npc1.type;
              npc1.active = true;
              npc1.SetDefaults(Type1, -1f);
            }
            if ((double) Vector2.DistanceSquared(npc1.position, vector2_5) < 6400.0)
              npc1.visualOffset = npc1.position - vector2_5;
            npc1.position = vector2_5;
            npc1.velocity = vector2_6;
            npc1.target = num33;
            npc1.direction = bitsByte15[0] ? 1 : -1;
            npc1.directionY = bitsByte15[1] ? 1 : -1;
            npc1.spriteDirection = bitsByte15[6] ? 1 : -1;
            if (bitsByte15[7])
              num34 = npc1.life = npc1.lifeMax;
            else
              npc1.life = num34;
            if (num34 <= 0)
              npc1.active = false;
            for (int index4 = 0; index4 < NPC.maxAI; ++index4)
              npc1.ai[index4] = numArray1[index4];
            if (oldType > -1 && oldType != npc1.type)
              npc1.TransformVisuals(oldType, npc1.type);
            if (Type1 == 262)
              NPC.plantBoss = index7;
            if (Type1 == 245)
              NPC.golemBoss = index7;
            if (npc1.type < 0 || npc1.type >= 580 || !Main.npcCatchable[npc1.type])
              break;
            npc1.releaseOwner = (short) this.reader.ReadByte();
            break;
          case 24:
            int number10 = (int) this.reader.ReadInt16();
            int index8 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              index8 = this.whoAmI;
            Player player8 = Main.player[index8];
            Main.npc[number10].StrikeNPC(player8.inventory[player8.selectedItem].damage, player8.inventory[player8.selectedItem].knockBack, player8.direction, false, false, false);
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(24, -1, this.whoAmI, (NetworkText) null, number10, (float) index8, 0.0f, 0.0f, 0, 0, 0);
            NetMessage.SendData(23, -1, -1, (NetworkText) null, number10, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 27:
            int num35 = (int) this.reader.ReadInt16();
            Vector2 vector2_7 = this.reader.ReadVector2();
            Vector2 vector2_8 = this.reader.ReadVector2();
            float num36 = this.reader.ReadSingle();
            int num37 = (int) this.reader.ReadInt16();
            int index9 = (int) this.reader.ReadByte();
            int Type2 = (int) this.reader.ReadInt16();
            BitsByte bitsByte16 = (BitsByte) this.reader.ReadByte();
            float[] numArray2 = new float[Projectile.maxAI];
            for (int index4 = 0; index4 < Projectile.maxAI; ++index4)
              numArray2[index4] = !bitsByte16[index4] ? 0.0f : this.reader.ReadSingle();
            int index10 = bitsByte16[Projectile.maxAI] ? (int) this.reader.ReadInt16() : -1;
            if (index10 >= 1000)
              index10 = -1;
            if (Main.netMode == 2)
            {
              index9 = this.whoAmI;
              if (Main.projHostile[Type2])
                break;
            }
            int number11 = 1000;
            for (int index4 = 0; index4 < 1000; ++index4)
            {
              if (Main.projectile[index4].owner == index9 && Main.projectile[index4].identity == num35 && Main.projectile[index4].active)
              {
                number11 = index4;
                break;
              }
            }
            if (number11 == 1000)
            {
              for (int index4 = 0; index4 < 1000; ++index4)
              {
                if (!Main.projectile[index4].active)
                {
                  number11 = index4;
                  break;
                }
              }
            }
            Projectile projectile1 = Main.projectile[number11];
            if (!projectile1.active || projectile1.type != Type2)
            {
              projectile1.SetDefaults(Type2);
              if (Main.netMode == 2)
                ++Netplay.Clients[this.whoAmI].SpamProjectile;
            }
            projectile1.identity = num35;
            projectile1.position = vector2_7;
            projectile1.velocity = vector2_8;
            projectile1.type = Type2;
            projectile1.damage = num37;
            projectile1.knockBack = num36;
            projectile1.owner = index9;
            for (int index4 = 0; index4 < Projectile.maxAI; ++index4)
              projectile1.ai[index4] = numArray2[index4];
            if (index10 >= 0)
            {
              projectile1.projUUID = index10;
              Main.projectileIdentity[index9, index10] = number11;
            }
            projectile1.ProjectileFixDesperation();
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(27, -1, this.whoAmI, (NetworkText) null, number11, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 28:
            int number12 = (int) this.reader.ReadInt16();
            int Damage1 = (int) this.reader.ReadInt16();
            float num38 = this.reader.ReadSingle();
            int hitDirection = (int) this.reader.ReadByte() - 1;
            byte num39 = this.reader.ReadByte();
            if (Main.netMode == 2)
            {
              if (Damage1 < 0)
                Damage1 = 0;
              Main.npc[number12].PlayerInteraction(this.whoAmI);
            }
            if (Damage1 >= 0)
            {
              Main.npc[number12].StrikeNPC(Damage1, num38, hitDirection, (int) num39 == 1, false, true);
            }
            else
            {
              Main.npc[number12].life = 0;
              Main.npc[number12].HitEffect(0, 10.0);
              Main.npc[number12].active = false;
            }
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(28, -1, this.whoAmI, (NetworkText) null, number12, (float) Damage1, num38, (float) hitDirection, (int) num39, 0, 0);
            if (Main.npc[number12].life <= 0)
              NetMessage.SendData(23, -1, -1, (NetworkText) null, number12, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            else
              Main.npc[number12].netUpdate = true;
            if (Main.npc[number12].realLife < 0)
              break;
            if (Main.npc[Main.npc[number12].realLife].life <= 0)
            {
              NetMessage.SendData(23, -1, -1, (NetworkText) null, Main.npc[number12].realLife, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              break;
            }
            Main.npc[Main.npc[number12].realLife].netUpdate = true;
            break;
          case 29:
            int number13 = (int) this.reader.ReadInt16();
            int num40 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              num40 = this.whoAmI;
            for (int index4 = 0; index4 < 1000; ++index4)
            {
              if (Main.projectile[index4].owner == num40 && Main.projectile[index4].identity == number13 && Main.projectile[index4].active)
              {
                Main.projectile[index4].Kill();
                break;
              }
            }
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(29, -1, this.whoAmI, (NetworkText) null, number13, (float) num40, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 30:
            int number14 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              number14 = this.whoAmI;
            bool flag6 = this.reader.ReadBoolean();
            Main.player[number14].hostile = flag6;
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(30, -1, this.whoAmI, (NetworkText) null, number14, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            LocalizedText localizedText1 = flag6 ? Lang.mp[11] : Lang.mp[12];
            Color color1 = Main.teamColor[Main.player[number14].team];
            NetMessage.BroadcastChatMessage(NetworkText.FromKey(localizedText1.Key, (object) Main.player[number14].name), color1, -1);
            break;
          case 31:
            if (Main.netMode != 2)
              break;
            int chest1 = Chest.FindChest((int) this.reader.ReadInt16(), (int) this.reader.ReadInt16());
            if (chest1 <= -1 || Chest.UsingChest(chest1) != -1)
              break;
            for (int index4 = 0; index4 < 40; ++index4)
              NetMessage.SendData(32, this.whoAmI, -1, (NetworkText) null, chest1, (float) index4, 0.0f, 0.0f, 0, 0, 0);
            NetMessage.SendData(33, this.whoAmI, -1, (NetworkText) null, chest1, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            Main.player[this.whoAmI].chest = chest1;
            if (Main.myPlayer == this.whoAmI)
              Main.recBigList = false;
            NetMessage.SendData(80, -1, this.whoAmI, (NetworkText) null, this.whoAmI, (float) chest1, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 32:
            int index11 = (int) this.reader.ReadInt16();
            int index12 = (int) this.reader.ReadByte();
            int num41 = (int) this.reader.ReadInt16();
            int pre3 = (int) this.reader.ReadByte();
            int type5 = (int) this.reader.ReadInt16();
            if (Main.chest[index11] == null)
              Main.chest[index11] = new Chest(false);
            if (Main.chest[index11].item[index12] == null)
              Main.chest[index11].item[index12] = new Item();
            Main.chest[index11].item[index12].netDefaults(type5);
            Main.chest[index11].item[index12].Prefix(pre3);
            Main.chest[index11].item[index12].stack = num41;
            Recipe.FindRecipes();
            break;
          case 33:
            int num42 = (int) this.reader.ReadInt16();
            int index13 = (int) this.reader.ReadInt16();
            int index14 = (int) this.reader.ReadInt16();
            int num43 = (int) this.reader.ReadByte();
            string str1 = string.Empty;
            if (num43 != 0)
            {
              if (num43 <= 20)
                str1 = this.reader.ReadString();
              else if (num43 != (int) byte.MaxValue)
                num43 = 0;
            }
            if (Main.netMode == 1)
            {
              Player player9 = Main.player[Main.myPlayer];
              if (player9.chest == -1)
              {
                Main.playerInventory = true;
                Main.PlaySound(10, -1, -1, 1, 1f, 0.0f);
              }
              else if (player9.chest != num42 && num42 != -1)
              {
                Main.playerInventory = true;
                Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
                Main.recBigList = false;
              }
              else if (player9.chest != -1 && num42 == -1)
              {
                Main.PlaySound(11, -1, -1, 1, 1f, 0.0f);
                Main.recBigList = false;
              }
              player9.chest = num42;
              player9.chestX = index13;
              player9.chestY = index14;
              Recipe.FindRecipes();
              if ((int) Main.tile[index13, index14].frameX < 36 || (int) Main.tile[index13, index14].frameX >= 72)
                break;
              AchievementsHelper.HandleSpecialEvent(Main.player[Main.myPlayer], 16);
              break;
            }
            if (num43 != 0)
            {
              int chest2 = Main.player[this.whoAmI].chest;
              Chest chest3 = Main.chest[chest2];
              chest3.name = str1;
              NetMessage.SendData(69, -1, this.whoAmI, (NetworkText) null, chest2, (float) chest3.x, (float) chest3.y, 0.0f, 0, 0, 0);
            }
            Main.player[this.whoAmI].chest = num42;
            Recipe.FindRecipes();
            NetMessage.SendData(80, -1, this.whoAmI, (NetworkText) null, this.whoAmI, (float) num42, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 34:
            byte num44 = this.reader.ReadByte();
            int index15 = (int) this.reader.ReadInt16();
            int index16 = (int) this.reader.ReadInt16();
            int style1 = (int) this.reader.ReadInt16();
            int id = (int) this.reader.ReadInt16();
            if (Main.netMode == 2)
              id = 0;
            if (Main.netMode == 2)
            {
              if ((int) num44 == 0)
              {
                int number5_1 = WorldGen.PlaceChest(index15, index16, (ushort) 21, false, style1);
                if (number5_1 == -1)
                {
                  NetMessage.SendData(34, this.whoAmI, -1, (NetworkText) null, (int) num44, (float) index15, (float) index16, (float) style1, number5_1, 0, 0);
                  Item.NewItem(index15 * 16, index16 * 16, 32, 32, Chest.chestItemSpawn[style1], 1, true, 0, false, false);
                  break;
                }
                NetMessage.SendData(34, -1, -1, (NetworkText) null, (int) num44, (float) index15, (float) index16, (float) style1, number5_1, 0, 0);
                break;
              }
              if ((int) num44 == 1 && (int) Main.tile[index15, index16].type == 21)
              {
                Tile tile = Main.tile[index15, index16];
                if ((int) tile.frameX % 36 != 0)
                  --index15;
                if ((int) tile.frameY % 36 != 0)
                  --index16;
                int chest2 = Chest.FindChest(index15, index16);
                WorldGen.KillTile(index15, index16, false, false, false);
                if (tile.active())
                  break;
                NetMessage.SendData(34, -1, -1, (NetworkText) null, (int) num44, (float) index15, (float) index16, 0.0f, chest2, 0, 0);
                break;
              }
              if ((int) num44 == 2)
              {
                int number5_1 = WorldGen.PlaceChest(index15, index16, (ushort) 88, false, style1);
                if (number5_1 == -1)
                {
                  NetMessage.SendData(34, this.whoAmI, -1, (NetworkText) null, (int) num44, (float) index15, (float) index16, (float) style1, number5_1, 0, 0);
                  Item.NewItem(index15 * 16, index16 * 16, 32, 32, Chest.dresserItemSpawn[style1], 1, true, 0, false, false);
                  break;
                }
                NetMessage.SendData(34, -1, -1, (NetworkText) null, (int) num44, (float) index15, (float) index16, (float) style1, number5_1, 0, 0);
                break;
              }
              if ((int) num44 == 3 && (int) Main.tile[index15, index16].type == 88)
              {
                Tile tile = Main.tile[index15, index16];
                int num45 = index15 - (int) tile.frameX % 54 / 18;
                if ((int) tile.frameY % 36 != 0)
                  --index16;
                int chest2 = Chest.FindChest(num45, index16);
                WorldGen.KillTile(num45, index16, false, false, false);
                if (tile.active())
                  break;
                NetMessage.SendData(34, -1, -1, (NetworkText) null, (int) num44, (float) num45, (float) index16, 0.0f, chest2, 0, 0);
                break;
              }
              if ((int) num44 == 4)
              {
                int number5_1 = WorldGen.PlaceChest(index15, index16, (ushort) 467, false, style1);
                if (number5_1 == -1)
                {
                  NetMessage.SendData(34, this.whoAmI, -1, (NetworkText) null, (int) num44, (float) index15, (float) index16, (float) style1, number5_1, 0, 0);
                  Item.NewItem(index15 * 16, index16 * 16, 32, 32, Chest.chestItemSpawn2[style1], 1, true, 0, false, false);
                  break;
                }
                NetMessage.SendData(34, -1, -1, (NetworkText) null, (int) num44, (float) index15, (float) index16, (float) style1, number5_1, 0, 0);
                break;
              }
              if ((int) num44 != 5 || (int) Main.tile[index15, index16].type != 467)
                break;
              Tile tile1 = Main.tile[index15, index16];
              if ((int) tile1.frameX % 36 != 0)
                --index15;
              if ((int) tile1.frameY % 36 != 0)
                --index16;
              int chest3 = Chest.FindChest(index15, index16);
              WorldGen.KillTile(index15, index16, false, false, false);
              if (tile1.active())
                break;
              NetMessage.SendData(34, -1, -1, (NetworkText) null, (int) num44, (float) index15, (float) index16, 0.0f, chest3, 0, 0);
              break;
            }
            if ((int) num44 == 0)
            {
              if (id == -1)
              {
                WorldGen.KillTile(index15, index16, false, false, false);
                break;
              }
              WorldGen.PlaceChestDirect(index15, index16, (ushort) 21, style1, id);
              break;
            }
            if ((int) num44 == 2)
            {
              if (id == -1)
              {
                WorldGen.KillTile(index15, index16, false, false, false);
                break;
              }
              WorldGen.PlaceDresserDirect(index15, index16, (ushort) 88, style1, id);
              break;
            }
            if ((int) num44 == 4)
            {
              if (id == -1)
              {
                WorldGen.KillTile(index15, index16, false, false, false);
                break;
              }
              WorldGen.PlaceChestDirect(index15, index16, (ushort) 467, style1, id);
              break;
            }
            Chest.DestroyChestDirect(index15, index16, id);
            WorldGen.KillTile(index15, index16, false, false, false);
            break;
          case 35:
            int number15 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              number15 = this.whoAmI;
            int healAmount1 = (int) this.reader.ReadInt16();
            if (number15 != Main.myPlayer || Main.ServerSideCharacter)
              Main.player[number15].HealEffect(healAmount1, true);
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(35, -1, this.whoAmI, (NetworkText) null, number15, (float) healAmount1, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 36:
            int number16 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              number16 = this.whoAmI;
            Player player10 = Main.player[number16];
            BitsByte bitsByte17 = (BitsByte) this.reader.ReadByte();
            player10.zone1 = bitsByte17;
            BitsByte bitsByte18 = (BitsByte) this.reader.ReadByte();
            player10.zone2 = bitsByte18;
            BitsByte bitsByte19 = (BitsByte) this.reader.ReadByte();
            player10.zone3 = bitsByte19;
            BitsByte bitsByte20 = (BitsByte) this.reader.ReadByte();
            player10.zone4 = bitsByte20;
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(36, -1, this.whoAmI, (NetworkText) null, number16, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 37:
            if (Main.netMode != 1)
              break;
            if (Main.autoPass)
            {
              NetMessage.SendData(38, -1, -1, (NetworkText) null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              Main.autoPass = false;
              break;
            }
            Netplay.ServerPassword = "";
            Main.menuMode = 31;
            break;
          case 38:
            if (Main.netMode != 2)
              break;
            if (this.reader.ReadString() == Netplay.ServerPassword)
            {
              Netplay.Clients[this.whoAmI].State = 1;
              NetMessage.SendData(3, this.whoAmI, -1, (NetworkText) null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              break;
            }
            NetMessage.SendData(2, this.whoAmI, -1, Lang.mp[1].ToNetworkText(), 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 39:
            if (Main.netMode != 1)
              break;
            int number17 = (int) this.reader.ReadInt16();
            Main.item[number17].owner = (int) byte.MaxValue;
            NetMessage.SendData(22, -1, -1, (NetworkText) null, number17, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 40:
            int number18 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              number18 = this.whoAmI;
            int num46 = (int) this.reader.ReadInt16();
            Main.player[number18].talkNPC = num46;
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(40, -1, this.whoAmI, (NetworkText) null, number18, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 41:
            int number19 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              number19 = this.whoAmI;
            Player player11 = Main.player[number19];
            float num47 = this.reader.ReadSingle();
            int num48 = (int) this.reader.ReadInt16();
            player11.itemRotation = num47;
            player11.itemAnimation = num48;
            player11.channel = player11.inventory[player11.selectedItem].channel;
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(41, -1, this.whoAmI, (NetworkText) null, number19, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 42:
            int index17 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              index17 = this.whoAmI;
            else if (Main.myPlayer == index17 && !Main.ServerSideCharacter)
              break;
            int num49 = (int) this.reader.ReadInt16();
            int num50 = (int) this.reader.ReadInt16();
            Main.player[index17].statMana = num49;
            Main.player[index17].statManaMax = num50;
            break;
          case 43:
            int number20 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              number20 = this.whoAmI;
            int manaAmount = (int) this.reader.ReadInt16();
            if (number20 != Main.myPlayer)
              Main.player[number20].ManaEffect(manaAmount);
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(43, -1, this.whoAmI, (NetworkText) null, number20, (float) manaAmount, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 45:
            int number21 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              number21 = this.whoAmI;
            int index18 = (int) this.reader.ReadByte();
            Player player12 = Main.player[number21];
            int team = player12.team;
            player12.team = index18;
            Color color2 = Main.teamColor[index18];
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(45, -1, this.whoAmI, (NetworkText) null, number21, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            LocalizedText localizedText2 = Lang.mp[13 + index18];
            if (index18 == 5)
              localizedText2 = Lang.mp[22];
            for (int playerId = 0; playerId < (int) byte.MaxValue; ++playerId)
            {
              if (playerId == this.whoAmI || team > 0 && Main.player[playerId].team == team || index18 > 0 && Main.player[playerId].team == index18)
                NetMessage.SendChatMessageToClient(NetworkText.FromKey(localizedText2.Key, (object) player12.name), color2, playerId);
            }
            break;
          case 46:
            if (Main.netMode != 2)
              break;
            int number22 = Sign.ReadSign((int) this.reader.ReadInt16(), (int) this.reader.ReadInt16(), true);
            if (number22 < 0)
              break;
            NetMessage.SendData(47, this.whoAmI, -1, (NetworkText) null, number22, (float) this.whoAmI, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 47:
            int index19 = (int) this.reader.ReadInt16();
            int num51 = (int) this.reader.ReadInt16();
            int num52 = (int) this.reader.ReadInt16();
            string text1 = this.reader.ReadString();
            string str2 = (string) null;
            if (Main.sign[index19] != null)
              str2 = Main.sign[index19].text;
            Main.sign[index19] = new Sign();
            Main.sign[index19].x = num51;
            Main.sign[index19].y = num52;
            Sign.TextSign(index19, text1);
            int num53 = (int) this.reader.ReadByte();
            if (Main.netMode == 2 && str2 != text1)
            {
              num53 = this.whoAmI;
              NetMessage.SendData(47, -1, this.whoAmI, (NetworkText) null, index19, (float) num53, 0.0f, 0.0f, 0, 0, 0);
            }
            if (Main.netMode != 1 || num53 != Main.myPlayer || Main.sign[index19] == null)
              break;
            Main.playerInventory = false;
            Main.player[Main.myPlayer].talkNPC = -1;
            Main.npcChatCornerItem = 0;
            Main.editSign = false;
            Main.PlaySound(10, -1, -1, 1, 1f, 0.0f);
            Main.player[Main.myPlayer].sign = index19;
            Main.npcChatText = Main.sign[index19].text;
            break;
          case 48:
            int i1 = (int) this.reader.ReadInt16();
            int j1 = (int) this.reader.ReadInt16();
            byte num54 = this.reader.ReadByte();
            byte num55 = this.reader.ReadByte();
            if (Main.netMode == 2 && Netplay.spamCheck)
            {
              int whoAmI = this.whoAmI;
              int num45 = (int) ((double) Main.player[whoAmI].position.X + (double) (Main.player[whoAmI].width / 2));
              int num56 = (int) ((double) Main.player[whoAmI].position.Y + (double) (Main.player[whoAmI].height / 2));
              int num57 = 10;
              int num58 = num45 - num57;
              int num59 = num45 + num57;
              int num60 = num57;
              int num61 = num56 - num60;
              int num62 = num57;
              int num63 = num56 + num62;
              if (i1 < num58 || i1 > num59 || (j1 < num61 || j1 > num63))
              {
                NetMessage.BootPlayer(this.whoAmI, NetworkText.FromKey("Net.CheatingLiquidSpam"));
                break;
              }
            }
            if (Main.tile[i1, j1] == null)
              Main.tile[i1, j1] = new Tile();
            lock (Main.tile[i1, j1])
            {
              Main.tile[i1, j1].liquid = num54;
              Main.tile[i1, j1].liquidType((int) num55);
              if (Main.netMode != 2)
                break;
              WorldGen.SquareTileFrame(i1, j1, true);
              break;
            }
          case 49:
            if (Netplay.Connection.State != 6)
              break;
            Netplay.Connection.State = 10;
            Main.ActivePlayerFileData.StartPlayTimer();
            Player.Hooks.EnterWorld(Main.myPlayer);
            Main.player[Main.myPlayer].Spawn();
            break;
          case 50:
            int number23 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              number23 = this.whoAmI;
            else if (number23 == Main.myPlayer && !Main.ServerSideCharacter)
              break;
            Player player13 = Main.player[number23];
            for (int index4 = 0; index4 < 22; ++index4)
            {
              player13.buffType[index4] = (int) this.reader.ReadByte();
              player13.buffTime[index4] = player13.buffType[index4] <= 0 ? 0 : 60;
            }
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(50, -1, this.whoAmI, (NetworkText) null, number23, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 51:
            byte num64 = this.reader.ReadByte();
            byte num65 = this.reader.ReadByte();
            switch (num65)
            {
              case 1:
                NPC.SpawnSkeletron();
                return;
              case 2:
                if (Main.netMode == 2)
                {
                  NetMessage.SendData(51, -1, this.whoAmI, (NetworkText) null, (int) num64, (float) num65, 0.0f, 0.0f, 0, 0, 0);
                  return;
                }
                Main.PlaySound(SoundID.Item1, (int) Main.player[(int) num64].position.X, (int) Main.player[(int) num64].position.Y);
                return;
              case 3:
                if (Main.netMode != 2)
                  return;
                Main.Sundialing();
                return;
              case 4:
                Main.npc[(int) num64].BigMimicSpawnSmoke();
                return;
              default:
                return;
            }
          case 52:
            int num66 = (int) this.reader.ReadByte();
            int num67 = (int) this.reader.ReadInt16();
            int num68 = (int) this.reader.ReadInt16();
            if (num66 == 1)
            {
              Chest.Unlock(num67, num68);
              if (Main.netMode == 2)
              {
                NetMessage.SendData(52, -1, this.whoAmI, (NetworkText) null, 0, (float) num66, (float) num67, (float) num68, 0, 0, 0);
                NetMessage.SendTileSquare(-1, num67, num68, 2, TileChangeType.None);
              }
            }
            if (num66 != 2)
              break;
            WorldGen.UnlockDoor(num67, num68);
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(52, -1, this.whoAmI, (NetworkText) null, 0, (float) num66, (float) num67, (float) num68, 0, 0, 0);
            NetMessage.SendTileSquare(-1, num67, num68, 2, TileChangeType.None);
            break;
          case 53:
            int number24 = (int) this.reader.ReadInt16();
            int type6 = (int) this.reader.ReadByte();
            int time1 = (int) this.reader.ReadInt16();
            Main.npc[number24].AddBuff(type6, time1, true);
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(54, -1, -1, (NetworkText) null, number24, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 54:
            if (Main.netMode != 1)
              break;
            int index20 = (int) this.reader.ReadInt16();
            NPC npc2 = Main.npc[index20];
            for (int index4 = 0; index4 < 5; ++index4)
            {
              npc2.buffType[index4] = (int) this.reader.ReadByte();
              npc2.buffTime[index4] = (int) this.reader.ReadInt16();
            }
            break;
          case 55:
            int index21 = (int) this.reader.ReadByte();
            int type7 = (int) this.reader.ReadByte();
            int time1_1 = this.reader.ReadInt32();
            if (Main.netMode == 2 && index21 != this.whoAmI && !Main.pvpBuff[type7])
              break;
            if (Main.netMode == 1 && index21 == Main.myPlayer)
            {
              Main.player[index21].AddBuff(type7, time1_1, true);
              break;
            }
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(55, index21, -1, (NetworkText) null, index21, (float) type7, (float) time1_1, 0.0f, 0, 0, 0);
            break;
          case 56:
            int number25 = (int) this.reader.ReadInt16();
            if (number25 < 0 || number25 >= 200)
              break;
            if (Main.netMode == 1)
            {
              string str3 = this.reader.ReadString();
              Main.npc[number25].GivenName = str3;
              break;
            }
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(56, this.whoAmI, -1, (NetworkText) null, number25, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 57:
            if (Main.netMode != 1)
              break;
            WorldGen.tGood = this.reader.ReadByte();
            WorldGen.tEvil = this.reader.ReadByte();
            WorldGen.tBlood = this.reader.ReadByte();
            break;
          case 58:
            int index22 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              index22 = this.whoAmI;
            float number2_1 = this.reader.ReadSingle();
            if (Main.netMode == 2)
            {
              NetMessage.SendData(58, -1, this.whoAmI, (NetworkText) null, this.whoAmI, number2_1, 0.0f, 0.0f, 0, 0, 0);
              break;
            }
            Player player14 = Main.player[index22];
            Main.harpNote = number2_1;
            LegacySoundStyle type8 = SoundID.Item26;
            if (player14.inventory[player14.selectedItem].type == 507)
              type8 = SoundID.Item35;
            Main.PlaySound(type8, player14.position);
            break;
          case 59:
            int num69 = (int) this.reader.ReadInt16();
            int j2 = (int) this.reader.ReadInt16();
            Wiring.SetCurrentUser(this.whoAmI);
            Wiring.HitSwitch(num69, j2);
            Wiring.SetCurrentUser(-1);
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(59, -1, this.whoAmI, (NetworkText) null, num69, (float) j2, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 60:
            int n = (int) this.reader.ReadInt16();
            int x1 = (int) this.reader.ReadInt16();
            int y3 = (int) this.reader.ReadInt16();
            byte num70 = this.reader.ReadByte();
            if (n >= 200)
            {
              NetMessage.BootPlayer(this.whoAmI, NetworkText.FromKey("Net.CheatingInvalid"));
              break;
            }
            if (Main.netMode == 1)
            {
              Main.npc[n].homeless = (int) num70 == 1;
              Main.npc[n].homeTileX = x1;
              Main.npc[n].homeTileY = y3;
              if ((int) num70 == 1)
              {
                WorldGen.TownManager.KickOut(Main.npc[n].type);
                break;
              }
              if ((int) num70 != 2)
                break;
              WorldGen.TownManager.SetRoom(Main.npc[n].type, x1, y3);
              break;
            }
            if ((int) num70 == 1)
            {
              WorldGen.kickOut(n);
              break;
            }
            WorldGen.moveRoom(x1, y3, n);
            break;
          case 61:
            int plr = (int) this.reader.ReadInt16();
            int Type3 = (int) this.reader.ReadInt16();
            if (Main.netMode != 2)
              break;
            if (Type3 >= 0 && Type3 < 580 && NPCID.Sets.MPAllowedEnemies[Type3])
            {
              if (NPC.AnyNPCs(Type3))
                break;
              NPC.SpawnOnPlayer(plr, Type3);
              break;
            }
            if (Type3 == -4)
            {
              if (Main.dayTime || DD2Event.Ongoing)
                break;
              NetMessage.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[31].Key), new Color(50, (int) byte.MaxValue, 130), -1);
              Main.startPumpkinMoon();
              NetMessage.SendData(7, -1, -1, (NetworkText) null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              NetMessage.SendData(78, -1, -1, (NetworkText) null, 0, 1f, 2f, 1f, 0, 0, 0);
              break;
            }
            if (Type3 == -5)
            {
              if (Main.dayTime || DD2Event.Ongoing)
                break;
              NetMessage.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[34].Key), new Color(50, (int) byte.MaxValue, 130), -1);
              Main.startSnowMoon();
              NetMessage.SendData(7, -1, -1, (NetworkText) null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              NetMessage.SendData(78, -1, -1, (NetworkText) null, 0, 1f, 1f, 1f, 0, 0, 0);
              break;
            }
            if (Type3 == -6)
            {
              if (!Main.dayTime || Main.eclipse)
                break;
              NetMessage.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[20].Key), new Color(50, (int) byte.MaxValue, 130), -1);
              Main.eclipse = true;
              NetMessage.SendData(7, -1, -1, (NetworkText) null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              break;
            }
            if (Type3 == -7)
            {
              Main.invasionDelay = 0;
              Main.StartInvasion(4);
              NetMessage.SendData(7, -1, -1, (NetworkText) null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              NetMessage.SendData(78, -1, -1, (NetworkText) null, 0, 1f, (float) (Main.invasionType + 3), 0.0f, 0, 0, 0);
              break;
            }
            if (Type3 == -8)
            {
              if (!NPC.downedGolemBoss || !Main.hardMode || (NPC.AnyDanger() || NPC.AnyoneNearCultists()))
                break;
              WorldGen.StartImpendingDoom();
              NetMessage.SendData(7, -1, -1, (NetworkText) null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
              break;
            }
            if (Type3 >= 0)
              break;
            int type9 = 1;
            if (Type3 > -5)
              type9 = -Type3;
            if (type9 > 0 && Main.invasionType == 0)
            {
              Main.invasionDelay = 0;
              Main.StartInvasion(type9);
            }
            NetMessage.SendData(78, -1, -1, (NetworkText) null, 0, 1f, (float) (Main.invasionType + 3), 0.0f, 0, 0, 0);
            break;
          case 62:
            int number26 = (int) this.reader.ReadByte();
            int num71 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              number26 = this.whoAmI;
            if (num71 == 1)
              Main.player[number26].NinjaDodge();
            if (num71 == 2)
              Main.player[number26].ShadowDodge();
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(62, -1, this.whoAmI, (NetworkText) null, number26, (float) num71, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 63:
            int num72 = (int) this.reader.ReadInt16();
            int y4 = (int) this.reader.ReadInt16();
            byte color3 = this.reader.ReadByte();
            WorldGen.paintTile(num72, y4, color3, false);
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(63, -1, this.whoAmI, (NetworkText) null, num72, (float) y4, (float) color3, 0.0f, 0, 0, 0);
            break;
          case 64:
            int num73 = (int) this.reader.ReadInt16();
            int y5 = (int) this.reader.ReadInt16();
            byte color4 = this.reader.ReadByte();
            WorldGen.paintWall(num73, y5, color4, false);
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(64, -1, this.whoAmI, (NetworkText) null, num73, (float) y5, (float) color4, 0.0f, 0, 0, 0);
            break;
          case 65:
            BitsByte bitsByte21 = (BitsByte) this.reader.ReadByte();
            int index23 = (int) this.reader.ReadInt16();
            if (Main.netMode == 2)
              index23 = this.whoAmI;
            Vector2 vector2_9 = this.reader.ReadVector2();
            int num74 = 0;
            int num75 = 0;
            if (bitsByte21[0])
              ++num74;
            if (bitsByte21[1])
              num74 += 2;
            if (bitsByte21[2])
              ++num75;
            if (bitsByte21[3])
              num75 += 2;
            if (num74 == 0)
              Main.player[index23].Teleport(vector2_9, num75, 0);
            else if (num74 == 1)
              Main.npc[index23].Teleport(vector2_9, num75, 0);
            else if (num74 == 2)
            {
              Main.player[index23].Teleport(vector2_9, num75, 0);
              if (Main.netMode == 2)
              {
                RemoteClient.CheckSection(this.whoAmI, vector2_9, 1);
                NetMessage.SendData(65, -1, -1, (NetworkText) null, 0, (float) index23, vector2_9.X, vector2_9.Y, num75, 0, 0);
                int index4 = -1;
                float num45 = 9999f;
                for (int index5 = 0; index5 < (int) byte.MaxValue; ++index5)
                {
                  if (Main.player[index5].active && index5 != this.whoAmI)
                  {
                    Vector2 vector2_10 = Main.player[index5].position - Main.player[this.whoAmI].position;
                    if ((double) vector2_10.Length() < (double) num45)
                    {
                      num45 = vector2_10.Length();
                      index4 = index5;
                    }
                  }
                }
                if (index4 >= 0)
                  NetMessage.BroadcastChatMessage(NetworkText.FromKey("Game.HasTeleportedTo", (object) Main.player[this.whoAmI].name, (object) Main.player[index4].name), new Color(250, 250, 0), -1);
              }
            }
            if (Main.netMode != 2 || num74 != 0)
              break;
            NetMessage.SendData(65, -1, this.whoAmI, (NetworkText) null, 0, (float) index23, vector2_9.X, vector2_9.Y, num75, 0, 0);
            break;
          case 66:
            int number27 = (int) this.reader.ReadByte();
            int healAmount2 = (int) this.reader.ReadInt16();
            if (healAmount2 <= 0)
              break;
            Player player15 = Main.player[number27];
            player15.statLife += healAmount2;
            if (player15.statLife > player15.statLifeMax2)
              player15.statLife = player15.statLifeMax2;
            player15.HealEffect(healAmount2, false);
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(66, -1, this.whoAmI, (NetworkText) null, number27, (float) healAmount2, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 68:
            this.reader.ReadString();
            break;
          case 69:
            int number28 = (int) this.reader.ReadInt16();
            int X = (int) this.reader.ReadInt16();
            int Y = (int) this.reader.ReadInt16();
            if (Main.netMode == 1)
            {
              if (number28 < 0 || number28 >= 1000)
                break;
              Chest chest2 = Main.chest[number28];
              if (chest2 == null)
              {
                chest2 = new Chest(false);
                chest2.x = X;
                chest2.y = Y;
                Main.chest[number28] = chest2;
              }
              else if (chest2.x != X || chest2.y != Y)
                break;
              chest2.name = this.reader.ReadString();
              break;
            }
            if (number28 < -1 || number28 >= 1000)
              break;
            if (number28 == -1)
            {
              number28 = Chest.FindChest(X, Y);
              if (number28 == -1)
                break;
            }
            Chest chest4 = Main.chest[number28];
            if (chest4.x != X || chest4.y != Y)
              break;
            NetMessage.SendData(69, this.whoAmI, -1, (NetworkText) null, number28, (float) X, (float) Y, 0.0f, 0, 0, 0);
            break;
          case 70:
            if (Main.netMode != 2)
              break;
            int i2 = (int) this.reader.ReadInt16();
            int who = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              who = this.whoAmI;
            if (i2 >= 200 || i2 < 0)
              break;
            NPC.CatchNPC(i2, who);
            break;
          case 71:
            if (Main.netMode != 2)
              break;
            int x2 = this.reader.ReadInt32();
            int num76 = this.reader.ReadInt32();
            int num77 = (int) this.reader.ReadInt16();
            byte num78 = this.reader.ReadByte();
            int y6 = num76;
            int Type4 = num77;
            int Style1 = (int) num78;
            int whoAmI1 = this.whoAmI;
            NPC.ReleaseNPC(x2, y6, Type4, Style1, whoAmI1);
            break;
          case 72:
            if (Main.netMode != 1)
              break;
            for (int index4 = 0; index4 < 40; ++index4)
              Main.travelShop[index4] = (int) this.reader.ReadInt16();
            break;
          case 73:
            Main.player[this.whoAmI].TeleportationPotion();
            break;
          case 74:
            if (Main.netMode != 1)
              break;
            Main.anglerQuest = (int) this.reader.ReadByte();
            Main.anglerQuestFinished = this.reader.ReadBoolean();
            break;
          case 75:
            if (Main.netMode != 2)
              break;
            string name = Main.player[this.whoAmI].name;
            if (Main.anglerWhoFinishedToday.Contains(name))
              break;
            Main.anglerWhoFinishedToday.Add(name);
            break;
          case 76:
            int number29 = (int) this.reader.ReadByte();
            if (number29 == Main.myPlayer && !Main.ServerSideCharacter)
              break;
            if (Main.netMode == 2)
              number29 = this.whoAmI;
            Main.player[number29].anglerQuestsFinished = this.reader.ReadInt32();
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(76, -1, this.whoAmI, (NetworkText) null, number29, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 77:
            int type10 = (int) this.reader.ReadInt16();
            ushort num79 = this.reader.ReadUInt16();
            short num80 = this.reader.ReadInt16();
            short num81 = this.reader.ReadInt16();
            int num82 = (int) num79;
            int x3 = (int) num80;
            int y7 = (int) num81;
            Animation.NewTemporaryAnimation(type10, (ushort) num82, x3, y7);
            break;
          case 78:
            if (Main.netMode != 1)
              break;
            Main.ReportInvasionProgress(this.reader.ReadInt32(), this.reader.ReadInt32(), (int) this.reader.ReadSByte(), (int) this.reader.ReadSByte());
            break;
          case 79:
            int x4 = (int) this.reader.ReadInt16();
            int y8 = (int) this.reader.ReadInt16();
            short num83 = this.reader.ReadInt16();
            int style2 = (int) this.reader.ReadInt16();
            int num84 = (int) this.reader.ReadByte();
            int random = (int) this.reader.ReadSByte();
            int direction2 = !this.reader.ReadBoolean() ? -1 : 1;
            if (Main.netMode == 2)
            {
              ++Netplay.Clients[this.whoAmI].SpamAddBlock;
              if (!WorldGen.InWorld(x4, y8, 10) || !Netplay.Clients[this.whoAmI].TileSections[Netplay.GetSectionX(x4), Netplay.GetSectionY(y8)])
                break;
            }
            WorldGen.PlaceObject(x4, y8, (int) num83, false, style2, num84, random, direction2);
            if (Main.netMode != 2)
              break;
            NetMessage.SendObjectPlacment(this.whoAmI, x4, y8, (int) num83, style2, num84, random, direction2);
            break;
          case 80:
            if (Main.netMode != 1)
              break;
            int index24 = (int) this.reader.ReadByte();
            int num85 = (int) this.reader.ReadInt16();
            if (num85 < -3 || num85 >= 1000)
              break;
            Main.player[index24].chest = num85;
            Recipe.FindRecipes();
            break;
          case 81:
            if (Main.netMode != 1)
              break;
            int x5 = (int) this.reader.ReadSingle();
            int num86 = (int) this.reader.ReadSingle();
            Color color5 = this.reader.ReadRGB();
            int amount = this.reader.ReadInt32();
            int y9 = num86;
            int width1 = 0;
            int height1 = 0;
            CombatText.NewText(new Rectangle(x5, y9, width1, height1), color5, amount, false, false);
            break;
          case 82:
            NetManager.Instance.Read(this.reader, this.whoAmI);
            break;
          case 83:
            if (Main.netMode != 1)
              break;
            int index25 = (int) this.reader.ReadInt16();
            int num87 = this.reader.ReadInt32();
            if (index25 < 0 || index25 >= 267)
              break;
            NPC.killCount[index25] = num87;
            break;
          case 84:
            int number30 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              number30 = this.whoAmI;
            float num88 = this.reader.ReadSingle();
            Main.player[number30].stealth = num88;
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(84, -1, this.whoAmI, (NetworkText) null, number30, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 85:
            int whoAmI2 = this.whoAmI;
            byte num89 = this.reader.ReadByte();
            if (Main.netMode != 2 || whoAmI2 >= (int) byte.MaxValue || (int) num89 >= 58)
              break;
            Chest.ServerPlaceItem(this.whoAmI, (int) num89);
            break;
          case 86:
            if (Main.netMode != 1)
              break;
            int key1 = this.reader.ReadInt32();
            if (!this.reader.ReadBoolean())
            {
              TileEntity tileEntity;
              if (!TileEntity.ByID.TryGetValue(key1, out tileEntity) || !(tileEntity is TETrainingDummy) && !(tileEntity is TEItemFrame) && !(tileEntity is TELogicSensor))
                break;
              TileEntity.ByID.Remove(key1);
              TileEntity.ByPosition.Remove(tileEntity.Position);
              break;
            }
            TileEntity tileEntity1 = TileEntity.Read(this.reader, true);
            tileEntity1.ID = key1;
            TileEntity.ByID[tileEntity1.ID] = tileEntity1;
            TileEntity.ByPosition[tileEntity1.Position] = tileEntity1;
            break;
          case 87:
            if (Main.netMode != 2)
              break;
            int num90 = (int) this.reader.ReadInt16();
            int num91 = (int) this.reader.ReadInt16();
            int type11 = (int) this.reader.ReadByte();
            if (!WorldGen.InWorld(num90, num91, 0) || TileEntity.ByPosition.ContainsKey(new Point16(num90, num91)))
              break;
            TileEntity.PlaceEntityNet(num90, num91, type11);
            break;
          case 88:
            if (Main.netMode != 1)
              break;
            int index26 = (int) this.reader.ReadInt16();
            if (index26 < 0 || index26 > 400)
              break;
            Item obj2 = Main.item[index26];
            BitsByte bitsByte22 = (BitsByte) this.reader.ReadByte();
            if (bitsByte22[0])
              obj2.color.PackedValue = this.reader.ReadUInt32();
            if (bitsByte22[1])
              obj2.damage = (int) this.reader.ReadUInt16();
            if (bitsByte22[2])
              obj2.knockBack = this.reader.ReadSingle();
            if (bitsByte22[3])
              obj2.useAnimation = (int) this.reader.ReadUInt16();
            if (bitsByte22[4])
              obj2.useTime = (int) this.reader.ReadUInt16();
            if (bitsByte22[5])
              obj2.shoot = (int) this.reader.ReadInt16();
            if (bitsByte22[6])
              obj2.shootSpeed = this.reader.ReadSingle();
            if (!bitsByte22[7])
              break;
            bitsByte22 = (BitsByte) this.reader.ReadByte();
            if (bitsByte22[0])
              obj2.width = (int) this.reader.ReadInt16();
            if (bitsByte22[1])
              obj2.height = (int) this.reader.ReadInt16();
            if (bitsByte22[2])
              obj2.scale = this.reader.ReadSingle();
            if (bitsByte22[3])
              obj2.ammo = (int) this.reader.ReadInt16();
            if (bitsByte22[4])
              obj2.useAmmo = (int) this.reader.ReadInt16();
            if (!bitsByte22[5])
              break;
            obj2.notAmmo = this.reader.ReadBoolean();
            break;
          case 89:
            if (Main.netMode != 2)
              break;
            int x6 = (int) this.reader.ReadInt16();
            int num92 = (int) this.reader.ReadInt16();
            int num93 = (int) this.reader.ReadInt16();
            int num94 = (int) this.reader.ReadByte();
            int num95 = (int) this.reader.ReadInt16();
            int y10 = num92;
            int netid = num93;
            int prefix = num94;
            int stack = num95;
            TEItemFrame.TryPlacing(x6, y10, netid, prefix, stack);
            break;
          case 91:
            if (Main.netMode != 1)
              break;
            int key2 = this.reader.ReadInt32();
            int type12 = (int) this.reader.ReadByte();
            if (type12 == (int) byte.MaxValue)
            {
              if (!EmoteBubble.byID.ContainsKey(key2))
                break;
              EmoteBubble.byID.Remove(key2);
              break;
            }
            int meta = (int) this.reader.ReadUInt16();
            int time2 = (int) this.reader.ReadByte();
            int emotion = (int) this.reader.ReadByte();
            int num96 = 0;
            if (emotion < 0)
              num96 = (int) this.reader.ReadInt16();
            WorldUIAnchor bubbleAnchor = EmoteBubble.DeserializeNetAnchor(type12, meta);
            lock (EmoteBubble.byID)
            {
              if (!EmoteBubble.byID.ContainsKey(key2))
              {
                EmoteBubble.byID[key2] = new EmoteBubble(emotion, bubbleAnchor, time2);
              }
              else
              {
                EmoteBubble.byID[key2].lifeTime = time2;
                EmoteBubble.byID[key2].lifeTimeStart = time2;
                EmoteBubble.byID[key2].emote = emotion;
                EmoteBubble.byID[key2].anchor = bubbleAnchor;
              }
              EmoteBubble.byID[key2].ID = key2;
              EmoteBubble.byID[key2].metadata = num96;
              break;
            }
          case 92:
            int number31 = (int) this.reader.ReadInt16();
            float num97 = this.reader.ReadSingle();
            float num98 = this.reader.ReadSingle();
            float num99 = this.reader.ReadSingle();
            if (number31 < 0 || number31 > 200)
              break;
            if (Main.netMode == 1)
            {
              Main.npc[number31].moneyPing(new Vector2(num98, num99));
              Main.npc[number31].extraValue = num97;
              break;
            }
            Main.npc[number31].extraValue += num97;
            NetMessage.SendData(92, -1, -1, (NetworkText) null, number31, Main.npc[number31].extraValue, num98, num99, 0, 0, 0);
            break;
          case 95:
            ushort num100 = this.reader.ReadUInt16();
            if (Main.netMode != 2 || (int) num100 < 0 || (int) num100 >= 1000)
              break;
            Projectile projectile2 = Main.projectile[(int) num100];
            if (projectile2.type != 602)
              break;
            projectile2.Kill();
            NetMessage.SendData(29, -1, -1, (NetworkText) null, projectile2.whoAmI, (float) projectile2.owner, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 96:
            int index27 = (int) this.reader.ReadByte();
            Player player16 = Main.player[index27];
            int num101 = (int) this.reader.ReadInt16();
            Vector2 vector2_11 = this.reader.ReadVector2();
            Vector2 vector2_12 = this.reader.ReadVector2();
            int num102 = num101 + (num101 % 2 == 0 ? 1 : -1);
            player16.lastPortalColorIndex = num102;
            Vector2 newPos1 = vector2_11;
            int Style2 = 4;
            int extraInfo1 = num101;
            player16.Teleport(newPos1, Style2, extraInfo1);
            Vector2 vector2_13 = vector2_12;
            player16.velocity = vector2_13;
            break;
          case 97:
            if (Main.netMode != 1)
              break;
            AchievementsHelper.NotifyNPCKilledDirect(Main.player[Main.myPlayer], (int) this.reader.ReadInt16());
            break;
          case 98:
            if (Main.netMode != 1)
              break;
            AchievementsHelper.NotifyProgressionEvent((int) this.reader.ReadInt16());
            break;
          case 99:
            int number32 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              number32 = this.whoAmI;
            Main.player[number32].MinionRestTargetPoint = this.reader.ReadVector2();
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(99, -1, this.whoAmI, (NetworkText) null, number32, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 100:
            int index28 = (int) this.reader.ReadUInt16();
            NPC npc3 = Main.npc[index28];
            int num103 = (int) this.reader.ReadInt16();
            Vector2 vector2_14 = this.reader.ReadVector2();
            Vector2 vector2_15 = this.reader.ReadVector2();
            int num104 = num103 + (num103 % 2 == 0 ? 1 : -1);
            npc3.lastPortalColorIndex = num104;
            Vector2 newPos2 = vector2_14;
            int Style3 = 4;
            int extraInfo2 = num103;
            npc3.Teleport(newPos2, Style3, extraInfo2);
            Vector2 vector2_16 = vector2_15;
            npc3.velocity = vector2_16;
            break;
          case 101:
            if (Main.netMode == 2)
              break;
            NPC.ShieldStrengthTowerSolar = (int) this.reader.ReadUInt16();
            NPC.ShieldStrengthTowerVortex = (int) this.reader.ReadUInt16();
            NPC.ShieldStrengthTowerNebula = (int) this.reader.ReadUInt16();
            NPC.ShieldStrengthTowerStardust = (int) this.reader.ReadUInt16();
            if (NPC.ShieldStrengthTowerSolar < 0)
              NPC.ShieldStrengthTowerSolar = 0;
            if (NPC.ShieldStrengthTowerVortex < 0)
              NPC.ShieldStrengthTowerVortex = 0;
            if (NPC.ShieldStrengthTowerNebula < 0)
              NPC.ShieldStrengthTowerNebula = 0;
            if (NPC.ShieldStrengthTowerStardust < 0)
              NPC.ShieldStrengthTowerStardust = 0;
            if (NPC.ShieldStrengthTowerSolar > NPC.LunarShieldPowerExpert)
              NPC.ShieldStrengthTowerSolar = NPC.LunarShieldPowerExpert;
            if (NPC.ShieldStrengthTowerVortex > NPC.LunarShieldPowerExpert)
              NPC.ShieldStrengthTowerVortex = NPC.LunarShieldPowerExpert;
            if (NPC.ShieldStrengthTowerNebula > NPC.LunarShieldPowerExpert)
              NPC.ShieldStrengthTowerNebula = NPC.LunarShieldPowerExpert;
            if (NPC.ShieldStrengthTowerStardust <= NPC.LunarShieldPowerExpert)
              break;
            NPC.ShieldStrengthTowerStardust = NPC.LunarShieldPowerExpert;
            break;
          case 102:
            int index29 = (int) this.reader.ReadByte();
            byte num105 = this.reader.ReadByte();
            Vector2 Other = this.reader.ReadVector2();
            if (Main.netMode == 2)
            {
              NetMessage.SendData(102, -1, -1, (NetworkText) null, this.whoAmI, (float) num105, Other.X, Other.Y, 0, 0, 0);
              break;
            }
            Player player17 = Main.player[index29];
            for (int index4 = 0; index4 < (int) byte.MaxValue; ++index4)
            {
              Player player9 = Main.player[index4];
              if (player9.active && !player9.dead && (player17.team == 0 || player17.team == player9.team) && (double) player9.Distance(Other) < 700.0)
              {
                Vector2 vector2_10 = player17.Center - player9.Center;
                Vector2 vec = Vector2.Normalize(vector2_10);
                if (!vec.HasNaNs())
                {
                  int num45 = 90;
                  float num56 = 0.0f;
                  float num57 = 0.2094395f;
                  Vector2 spinningpoint = new Vector2(0.0f, -8f);
                  Vector2 vector2_17 = new Vector2(-3f);
                  float num58 = 0.0f;
                  float num59 = 0.005f;
                  if ((int) num105 != 173)
                  {
                    if ((int) num105 != 176)
                    {
                      if ((int) num105 == 179)
                        num45 = 86;
                    }
                    else
                      num45 = 88;
                  }
                  else
                    num45 = 90;
                  for (int index5 = 0; (double) index5 < (double) vector2_10.Length() / 6.0; ++index5)
                  {
                    Vector2 Position = player9.Center + 6f * (float) index5 * vec + spinningpoint.RotatedBy((double) num56, new Vector2()) + vector2_17;
                    num56 += num57;
                    int Width = 6;
                    int Height = 6;
                    int Type5 = num45;
                    double num60 = 0.0;
                    double num61 = 0.0;
                    int Alpha = 100;
                    Color newColor = new Color();
                    double num62 = 1.5;
                    int index30 = Dust.NewDust(Position, Width, Height, Type5, (float) num60, (float) num61, Alpha, newColor, (float) num62);
                    Main.dust[index30].noGravity = true;
                    Main.dust[index30].velocity = Vector2.Zero;
                    Main.dust[index30].fadeIn = (num58 += num59);
                    Main.dust[index30].velocity += vec * 1.5f;
                  }
                }
                player9.NebulaLevelup((int) num105);
              }
            }
            break;
          case 103:
            if (Main.netMode != 1)
              break;
            NPC.MoonLordCountdown = this.reader.ReadInt32();
            break;
          case 104:
            if (Main.netMode != 1 || Main.npcShop <= 0)
              break;
            Item[] objArray = Main.instance.shop[Main.npcShop].item;
            int index31 = (int) this.reader.ReadByte();
            int type13 = (int) this.reader.ReadInt16();
            int num106 = (int) this.reader.ReadInt16();
            int pre4 = (int) this.reader.ReadByte();
            int num107 = this.reader.ReadInt32();
            BitsByte bitsByte23 = (BitsByte) this.reader.ReadByte();
            if (index31 >= objArray.Length)
              break;
            objArray[index31] = new Item();
            objArray[index31].netDefaults(type13);
            objArray[index31].stack = num106;
            objArray[index31].Prefix(pre4);
            objArray[index31].value = num107;
            objArray[index31].buyOnce = bitsByte23[0];
            break;
          case 105:
            if (Main.netMode == 1)
              break;
            int i3 = (int) this.reader.ReadInt16();
            int num108 = (int) this.reader.ReadInt16();
            bool flag7 = this.reader.ReadBoolean();
            int j3 = num108;
            int num109 = flag7 ? 1 : 0;
            WorldGen.ToggleGemLock(i3, j3, num109 != 0);
            break;
          case 106:
            if (Main.netMode != 1)
              break;
            Utils.PoofOfSmoke(new HalfVector2()
            {
              PackedValue = this.reader.ReadUInt32()
            }.ToVector2());
            break;
          case 107:
            if (Main.netMode != 1)
              break;
            Color color6 = this.reader.ReadRGB();
            string text2 = NetworkText.Deserialize(this.reader).ToString();
            int num110 = (int) this.reader.ReadInt16();
            int num111 = 0;
            Color c = color6;
            int WidthLimit = num110;
            Main.NewTextMultiline(text2, num111 != 0, c, WidthLimit);
            break;
          case 108:
            if (Main.netMode != 1)
              break;
            int Damage2 = (int) this.reader.ReadInt16();
            float KnockBack = this.reader.ReadSingle();
            int x7 = (int) this.reader.ReadInt16();
            int y11 = (int) this.reader.ReadInt16();
            int angle = (int) this.reader.ReadInt16();
            int ammo = (int) this.reader.ReadInt16();
            int owner = (int) this.reader.ReadByte();
            if (owner != Main.myPlayer)
              break;
            WorldGen.ShootFromCannon(x7, y11, angle, ammo, Damage2, KnockBack, owner);
            break;
          case 109:
            if (Main.netMode != 2)
              break;
            int x8 = (int) this.reader.ReadInt16();
            int num112 = (int) this.reader.ReadInt16();
            int x9 = (int) this.reader.ReadInt16();
            int y12 = (int) this.reader.ReadInt16();
            int num113 = (int) this.reader.ReadByte();
            int whoAmI3 = this.whoAmI;
            WiresUI.Settings.MultiToolMode toolMode = WiresUI.Settings.ToolMode;
            WiresUI.Settings.ToolMode = (WiresUI.Settings.MultiToolMode) num113;
            int y13 = num112;
            Wiring.MassWireOperation(new Point(x8, y13), new Point(x9, y12), Main.player[whoAmI3]);
            WiresUI.Settings.ToolMode = toolMode;
            break;
          case 110:
            if (Main.netMode != 1)
              break;
            int type14 = (int) this.reader.ReadInt16();
            int num114 = (int) this.reader.ReadInt16();
            int index32 = (int) this.reader.ReadByte();
            if (index32 != Main.myPlayer)
              break;
            Player player18 = Main.player[index32];
            for (int index4 = 0; index4 < num114; ++index4)
              player18.ConsumeItem(type14, false);
            player18.wireOperationsCooldown = 0;
            break;
          case 111:
            if (Main.netMode != 2)
              break;
            BirthdayParty.ToggleManualParty();
            break;
          case 112:
            int number33 = (int) this.reader.ReadByte();
            int x10 = (int) this.reader.ReadInt16();
            int y14 = (int) this.reader.ReadInt16();
            int height2 = (int) this.reader.ReadByte();
            int num115 = (int) this.reader.ReadInt16();
            if (number33 != 1)
              break;
            if (Main.netMode == 1)
              WorldGen.TreeGrowFX(x10, y14, height2, num115);
            if (Main.netMode != 2)
              break;
            NetMessage.SendData((int) num1, -1, -1, (NetworkText) null, number33, (float) x10, (float) y14, (float) height2, num115, 0, 0);
            break;
          case 113:
            int x11 = (int) this.reader.ReadInt16();
            int y15 = (int) this.reader.ReadInt16();
            if (Main.netMode != 2 || Main.snowMoon || Main.pumpkinMoon)
              break;
            if (DD2Event.WouldFailSpawningHere(x11, y15))
              DD2Event.FailureMessage(this.whoAmI);
            DD2Event.SummonCrystal(x11, y15);
            break;
          case 114:
            if (Main.netMode != 1)
              break;
            DD2Event.WipeEntities();
            break;
          case 115:
            int number34 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              number34 = this.whoAmI;
            Main.player[number34].MinionAttackTargetNPC = (int) this.reader.ReadInt16();
            if (Main.netMode != 2)
              break;
            NetMessage.SendData(115, -1, this.whoAmI, (NetworkText) null, number34, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            break;
          case 116:
            if (Main.netMode != 1)
              break;
            DD2Event.TimeLeftBetweenWaves = this.reader.ReadInt32();
            break;
          case 117:
            int playerTargetIndex1 = (int) this.reader.ReadByte();
            if (Main.netMode == 2 && this.whoAmI != playerTargetIndex1 && (!Main.player[playerTargetIndex1].hostile || !Main.player[this.whoAmI].hostile))
              break;
            PlayerDeathReason playerDeathReason1 = PlayerDeathReason.FromReader(this.reader);
            int num116 = (int) this.reader.ReadInt16();
            int num117 = (int) this.reader.ReadByte() - 1;
            BitsByte bitsByte24 = (BitsByte) this.reader.ReadByte();
            bool flag8 = bitsByte24[0];
            bool pvp1 = bitsByte24[1];
            int num118 = (int) this.reader.ReadSByte();
            Main.player[playerTargetIndex1].Hurt(playerDeathReason1, num116, num117, pvp1, true, flag8, num118);
            if (Main.netMode != 2)
              break;
            NetMessage.SendPlayerHurt(playerTargetIndex1, playerDeathReason1, num116, num117, flag8, pvp1, num118, -1, this.whoAmI);
            break;
          case 118:
            int playerTargetIndex2 = (int) this.reader.ReadByte();
            if (Main.netMode == 2)
              playerTargetIndex2 = this.whoAmI;
            PlayerDeathReason playerDeathReason2 = PlayerDeathReason.FromReader(this.reader);
            int damage = (int) this.reader.ReadInt16();
            int num119 = (int) this.reader.ReadByte() - 1;
            bool pvp2 = (BitsByte) this.reader.ReadByte()[0];
            Main.player[playerTargetIndex2].KillMe(playerDeathReason2, (double) damage, num119, pvp2);
            if (Main.netMode != 2)
              break;
            NetMessage.SendPlayerDeath(playerTargetIndex2, playerDeathReason2, damage, num119, pvp2, -1, this.whoAmI);
            break;
          case 119:
            if (Main.netMode != 1)
              break;
            int x12 = (int) this.reader.ReadSingle();
            int num120 = (int) this.reader.ReadSingle();
            Color color7 = this.reader.ReadRGB();
            NetworkText networkText = NetworkText.Deserialize(this.reader);
            int y16 = num120;
            int width2 = 0;
            int height3 = 0;
            CombatText.NewText(new Rectangle(x12, y16, width2, height3), color7, networkText.ToString(), false, false);
            break;
        }
      }
    }
  }
}
