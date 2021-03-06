﻿// Decompiled with JetBrains decompiler
// Type: Terraria.Chat.Commands.ChatCommandAttribute
// Assembly: TerrariaServer, Version=1.3.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 13381DB9-8FD8-4EBB-8CED-9CF82DC89291
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Terraria\TerrariaServer.exe

using System;

namespace Terraria.Chat.Commands
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
  public sealed class ChatCommandAttribute : Attribute
  {
    public readonly string Name;

    public ChatCommandAttribute(string name)
    {
      this.Name = name;
    }
  }
}
