﻿// Decompiled with JetBrains decompiler
// Type: Terraria.Initializers.UILinksInitializer
// Assembly: TerrariaServer, Version=1.3.5.3, Culture=neutral, PublicKeyToken=null
// MVID: AA3606A2-F3DB-4481-937B-7295FB97CD3E
// Assembly location: E:\TSHOCK\TerrariaServer.exe

using Microsoft.Xna.Framework;
using System;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Terraria.Initializers
{
  public class UILinksInitializer
  {
    public static bool NothingMoreImportantThanNPCChat()
    {
      if (!Main.hairWindow && Main.npcShop == 0)
        return Main.player[Main.myPlayer].chest == -1;
      return false;
    }

    public static float HandleSlider(float currentValue, float min, float max, float deadZone = 0.2f, float sensitivity = 0.5f)
    {
      float x = PlayerInput.GamepadThumbstickLeft.X;
      float num = (double) x < -(double) deadZone || (double) x > (double) deadZone ? MathHelper.Lerp(0.0f, sensitivity / 60f, (float) (((double) Math.Abs(x) - (double) deadZone) / (1.0 - (double) deadZone))) * (float) Math.Sign(x) : 0.0f;
      return MathHelper.Clamp((float) (((double) currentValue - (double) min) / ((double) max - (double) min)) + num, 0.0f, 1f) * (max - min) + min;
    }

    public static void Load()
    {
      Func<string> func1 = (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[53].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]));
      UILinkPage page1 = new UILinkPage();
      page1.UpdateEvent += (Action) (() => PlayerInput.GamepadAllowScrolling = true);
      for (int index = 0; index < 20; ++index)
        page1.LinkMap.Add(2000 + index, new UILinkPoint(2000 + index, true, -3, -4, -1, -2));
      page1.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[53].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + PlayerInput.BuildCommand(Lang.misc[82].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]));
      page1.UpdateEvent += (Action) (() =>
      {
        if (PlayerInput.Triggers.JustPressed.Inventory)
          UILinksInitializer.FancyExit();
        UILinkPointNavigator.Shortcuts.BackButtonInUse = PlayerInput.Triggers.JustPressed.Inventory;
        UILinksInitializer.HandleOptionsSpecials();
      });
      page1.IsValidEvent += (Func<bool>) (() =>
      {
        if (Main.gameMenu)
          return !Main.MenuUI.IsVisible;
        return false;
      });
      page1.CanEnterEvent += (Func<bool>) (() =>
      {
        if (Main.gameMenu)
          return !Main.MenuUI.IsVisible;
        return false;
      });
      UILinkPointNavigator.RegisterPage(page1, 1000, true);
      UILinkPage cp1 = new UILinkPage();
      cp1.LinkMap.Add(2500, new UILinkPoint(2500, true, -3, 2501, -1, -2));
      cp1.LinkMap.Add(2501, new UILinkPoint(2501, true, 2500, 2502, -1, -2));
      cp1.LinkMap.Add(2502, new UILinkPoint(2502, true, 2501, -4, -1, -2));
      cp1.UpdateEvent += (Action) (() => cp1.LinkMap[2501].Right = UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight ? 2502 : -4);
      cp1.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[53].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + PlayerInput.BuildCommand(Lang.misc[56].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]));
      cp1.IsValidEvent += (Func<bool>) (() =>
      {
        if (Main.player[Main.myPlayer].talkNPC != -1 || Main.player[Main.myPlayer].sign != -1)
          return UILinksInitializer.NothingMoreImportantThanNPCChat();
        return false;
      });
      cp1.CanEnterEvent += (Func<bool>) (() =>
      {
        if (Main.player[Main.myPlayer].talkNPC != -1 || Main.player[Main.myPlayer].sign != -1)
          return UILinksInitializer.NothingMoreImportantThanNPCChat();
        return false;
      });
      cp1.EnterEvent += (Action) (() => Main.player[Main.myPlayer].releaseInventory = false);
      cp1.LeaveEvent += (Action) (() =>
      {
        Main.npcChatRelease = false;
        Main.player[Main.myPlayer].releaseUseTile = false;
      });
      UILinkPointNavigator.RegisterPage(cp1, 1003, true);
      UILinkPage cp2 = new UILinkPage();
      cp2.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      Func<string> func2 = (Func<string>) (() =>
      {
        int currentPoint = UILinkPointNavigator.CurrentPoint;
        return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].inventory, 0, currentPoint);
      });
      Func<string> func3 = (Func<string>) (() => ItemSlot.GetGamepadInstructions(ref Main.player[Main.myPlayer].trashItem, 6));
      for (int index = 0; index <= 49; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, index - 1, index + 1, index - 10, index + 10);
        uiLinkPoint.OnSpecialInteracts += func2;
        int num1 = index;
        int num2 = 10;
        if (num1 < num2)
          uiLinkPoint.Up = -1;
        int num3 = 40;
        if (num1 >= num3)
          uiLinkPoint.Down = -2;
        int num4 = 10;
        if (num1 % num4 == 9)
          uiLinkPoint.Right = -4;
        int num5 = 10;
        if (num1 % num5 == 0)
          uiLinkPoint.Left = -3;
        cp2.LinkMap.Add(index, uiLinkPoint);
      }
      cp2.LinkMap[9].Right = 0;
      cp2.LinkMap[19].Right = 50;
      cp2.LinkMap[29].Right = 51;
      cp2.LinkMap[39].Right = 52;
      cp2.LinkMap[49].Right = 53;
      cp2.LinkMap[0].Left = 9;
      cp2.LinkMap[10].Left = 54;
      cp2.LinkMap[20].Left = 55;
      cp2.LinkMap[30].Left = 56;
      cp2.LinkMap[40].Left = 57;
      cp2.LinkMap.Add(300, new UILinkPoint(300, true, 302, 301, 49, -2));
      cp2.LinkMap.Add(301, new UILinkPoint(301, true, 300, 302, 53, 50));
      cp2.LinkMap.Add(302, new UILinkPoint(302, true, 301, 300, 57, 54));
      cp2.LinkMap[301].OnSpecialInteracts += func1;
      cp2.LinkMap[302].OnSpecialInteracts += func1;
      cp2.LinkMap[300].OnSpecialInteracts += func3;
      cp2.UpdateEvent += (Action) (() =>
      {
        bool inReforgeMenu = Main.InReforgeMenu;
        bool flag1 = Main.player[Main.myPlayer].chest != -1;
        bool flag2 = (uint) Main.npcShop > 0U;
        for (int index = 40; index <= 49; ++index)
          cp2.LinkMap[index].Down = !inReforgeMenu ? (!flag1 ? (!flag2 ? -2 : 2700 + index - 40) : 400 + index - 40) : (index < 45 ? 303 : 304);
        if (flag1)
        {
          cp2.LinkMap[300].Up = 439;
          cp2.LinkMap[300].Right = -4;
          cp2.LinkMap[300].Left = -3;
        }
        else if (flag2)
        {
          cp2.LinkMap[300].Up = 2739;
          cp2.LinkMap[300].Right = -4;
          cp2.LinkMap[300].Left = -3;
        }
        else
        {
          cp2.LinkMap[300].Up = 49;
          cp2.LinkMap[300].Right = 301;
          cp2.LinkMap[300].Left = 302;
          cp2.LinkMap[49].Down = 300;
        }
        cp2.LinkMap[10].Left = 54;
        cp2.LinkMap[20].Left = 55;
        cp2.LinkMap[30].Left = 56;
        cp2.LinkMap[40].Left = 57;
        if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 8)
        {
          cp2.LinkMap[0].Left = 4000;
          cp2.LinkMap[10].Left = 4002;
          cp2.LinkMap[20].Left = 4004;
          cp2.LinkMap[30].Left = 4006;
          cp2.LinkMap[40].Left = 4008;
        }
        else
        {
          cp2.LinkMap[0].Left = 9;
          if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 0)
            cp2.LinkMap[10].Left = 4000;
          if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 2)
            cp2.LinkMap[20].Left = 4002;
          if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 4)
            cp2.LinkMap[30].Left = 4004;
          if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 6)
            cp2.LinkMap[40].Left = 4006;
        }
        cp2.PageOnLeft = Main.InReforgeMenu ? 5 : 9;
      });
      cp2.IsValidEvent += (Func<bool>) (() => Main.playerInventory);
      cp2.PageOnLeft = 9;
      cp2.PageOnRight = 2;
      UILinkPointNavigator.RegisterPage(cp2, 0, true);
      UILinkPage cp3 = new UILinkPage();
      cp3.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      Func<string> func4 = (Func<string>) (() =>
      {
        int currentPoint = UILinkPointNavigator.CurrentPoint;
        return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].inventory, 1, currentPoint);
      });
      for (int index = 50; index <= 53; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, -3, -4, index - 1, index + 1);
        uiLinkPoint.OnSpecialInteracts += func4;
        cp3.LinkMap.Add(index, uiLinkPoint);
      }
      cp3.LinkMap[50].Left = 19;
      cp3.LinkMap[51].Left = 29;
      cp3.LinkMap[52].Left = 39;
      cp3.LinkMap[53].Left = 49;
      cp3.LinkMap[50].Right = 54;
      cp3.LinkMap[51].Right = 55;
      cp3.LinkMap[52].Right = 56;
      cp3.LinkMap[53].Right = 57;
      cp3.LinkMap[50].Up = -1;
      cp3.LinkMap[53].Down = -2;
      cp3.UpdateEvent += (Action) (() =>
      {
        if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
        {
          cp3.LinkMap[50].Up = 301;
          cp3.LinkMap[53].Down = 301;
        }
        else
        {
          cp3.LinkMap[50].Up = 504;
          cp3.LinkMap[53].Down = 500;
        }
      });
      cp3.IsValidEvent += (Func<bool>) (() => Main.playerInventory);
      cp3.PageOnLeft = 0;
      cp3.PageOnRight = 2;
      UILinkPointNavigator.RegisterPage(cp3, 1, true);
      UILinkPage cp4 = new UILinkPage();
      cp4.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      Func<string> func5 = (Func<string>) (() =>
      {
        int currentPoint = UILinkPointNavigator.CurrentPoint;
        return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].inventory, 2, currentPoint);
      });
      for (int index = 54; index <= 57; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, -3, -4, index - 1, index + 1);
        uiLinkPoint.OnSpecialInteracts += func5;
        cp4.LinkMap.Add(index, uiLinkPoint);
      }
      cp4.LinkMap[54].Left = 50;
      cp4.LinkMap[55].Left = 51;
      cp4.LinkMap[56].Left = 52;
      cp4.LinkMap[57].Left = 53;
      cp4.LinkMap[54].Right = 10;
      cp4.LinkMap[55].Right = 20;
      cp4.LinkMap[56].Right = 30;
      cp4.LinkMap[57].Right = 40;
      cp4.LinkMap[54].Up = -1;
      cp4.LinkMap[57].Down = -2;
      cp4.UpdateEvent += (Action) (() =>
      {
        if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
        {
          cp4.LinkMap[54].Up = 302;
          cp4.LinkMap[57].Down = 302;
        }
        else
        {
          cp4.LinkMap[54].Up = 504;
          cp4.LinkMap[57].Down = 500;
        }
      });
      cp4.PageOnLeft = 0;
      cp4.PageOnRight = 8;
      UILinkPointNavigator.RegisterPage(cp4, 2, true);
      UILinkPage cp5 = new UILinkPage();
      cp5.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      Func<string> func6 = (Func<string>) (() =>
      {
        int slot = UILinkPointNavigator.CurrentPoint - 100;
        return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].armor, slot < 10 ? 8 : 9, slot);
      });
      Func<string> func7 = (Func<string>) (() =>
      {
        int slot = UILinkPointNavigator.CurrentPoint - 120;
        return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].dye, 12, slot);
      });
      for (int index = 100; index <= 119; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, index + 10, index - 10, index - 1, index + 1);
        uiLinkPoint.OnSpecialInteracts += func6;
        int num = index - 100;
        if (num == 0)
          uiLinkPoint.Up = 305;
        if (num == 10)
          uiLinkPoint.Up = 306;
        if (num == 9 || num == 19)
          uiLinkPoint.Down = -2;
        if (num >= 10)
          uiLinkPoint.Left = 120 + num % 10;
        else
          uiLinkPoint.Right = -4;
        cp5.LinkMap.Add(index, uiLinkPoint);
      }
      for (int index = 120; index <= 129; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, -3, index - 10, index - 1, index + 1);
        uiLinkPoint.OnSpecialInteracts += func7;
        int num1 = index - 120;
        if (num1 == 0)
          uiLinkPoint.Up = 307;
        int num2 = 9;
        if (num1 == num2)
        {
          uiLinkPoint.Down = 308;
          uiLinkPoint.Left = 1557;
        }
        cp5.LinkMap.Add(index, uiLinkPoint);
      }
      cp5.IsValidEvent += (Func<bool>) (() =>
      {
        if (Main.playerInventory)
          return Main.EquipPage == 0;
        return false;
      });
      cp5.UpdateEvent += (Action) (() =>
      {
        int num1 = 107;
        int extraAccessorySlots = Main.player[Main.myPlayer].extraAccessorySlots;
        for (int index = 0; index < extraAccessorySlots; ++index)
        {
          cp5.LinkMap[num1 + index].Down = num1 + index + 1;
          cp5.LinkMap[num1 - 100 + 120 + index].Down = num1 - 100 + 120 + index + 1;
          cp5.LinkMap[num1 + 10 + index].Down = num1 + 10 + index + 1;
        }
        cp5.LinkMap[num1 + extraAccessorySlots].Down = 308;
        cp5.LinkMap[num1 - 100 + 120 + extraAccessorySlots].Down = 308;
        cp5.LinkMap[num1 + 10 + extraAccessorySlots].Down = 308;
        bool shouldPvpDraw = Main.ShouldPVPDraw;
        for (int index = 120; index <= 129; ++index)
        {
          UILinkPoint link = cp5.LinkMap[index];
          int num2 = index - 120;
          if (num2 == 0)
            link.Left = shouldPvpDraw ? 1550 : -3;
          int num3 = 1;
          if (num2 == num3)
            link.Left = shouldPvpDraw ? 1552 : -3;
          int num4 = 2;
          if (num2 == num4)
            link.Left = shouldPvpDraw ? 1556 : -3;
          int num5 = 3;
          if (num2 == num5)
            link.Left = UILinkPointNavigator.Shortcuts.INFOACCCOUNT >= 1 ? 1558 : -3;
          int num6 = 4;
          if (num2 == num6)
            link.Left = UILinkPointNavigator.Shortcuts.INFOACCCOUNT >= 5 ? 1562 : -3;
          int num7 = 5;
          if (num2 == num7)
            link.Left = UILinkPointNavigator.Shortcuts.INFOACCCOUNT >= 9 ? 1566 : -3;
          int num8 = 7;
          if (num2 == num8)
            link.Left = shouldPvpDraw ? 1557 : -3;
        }
      });
      cp5.PageOnLeft = 8;
      cp5.PageOnRight = 8;
      UILinkPointNavigator.RegisterPage(cp5, 3, true);
      UILinkPage page2 = new UILinkPage();
      page2.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      Func<string> func8 = (Func<string>) (() =>
      {
        int slot = UILinkPointNavigator.CurrentPoint - 400;
        int context = 4;
        Item[] inv = Main.player[Main.myPlayer].bank.item;
        switch (Main.player[Main.myPlayer].chest)
        {
          case -4:
            inv = Main.player[Main.myPlayer].bank3.item;
            goto case -2;
          case -3:
            inv = Main.player[Main.myPlayer].bank2.item;
            goto case -2;
          case -2:
            return ItemSlot.GetGamepadInstructions(inv, context, slot);
          case -1:
            return "";
          default:
            inv = Main.chest[Main.player[Main.myPlayer].chest].item;
            context = 3;
            goto case -2;
        }
      });
      for (int index = 400; index <= 439; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, index - 1, index + 1, index - 10, index + 10);
        uiLinkPoint.OnSpecialInteracts += func8;
        int num = index - 400;
        if (num < 10)
          uiLinkPoint.Up = 40 + num;
        if (num >= 30)
          uiLinkPoint.Down = -2;
        if (num % 10 == 9)
          uiLinkPoint.Right = -4;
        if (num % 10 == 0)
          uiLinkPoint.Left = -3;
        page2.LinkMap.Add(index, uiLinkPoint);
      }
      page2.LinkMap.Add(500, new UILinkPoint(500, true, 409, -4, 53, 501));
      page2.LinkMap.Add(501, new UILinkPoint(501, true, 419, -4, 500, 502));
      page2.LinkMap.Add(502, new UILinkPoint(502, true, 429, -4, 501, 503));
      page2.LinkMap.Add(503, new UILinkPoint(503, true, 439, -4, 502, 505));
      page2.LinkMap.Add(505, new UILinkPoint(505, true, 439, -4, 503, 504));
      page2.LinkMap.Add(504, new UILinkPoint(504, true, 439, -4, 505, 50));
      page2.LinkMap[500].OnSpecialInteracts += func1;
      page2.LinkMap[501].OnSpecialInteracts += func1;
      page2.LinkMap[502].OnSpecialInteracts += func1;
      page2.LinkMap[503].OnSpecialInteracts += func1;
      page2.LinkMap[504].OnSpecialInteracts += func1;
      page2.LinkMap[505].OnSpecialInteracts += func1;
      page2.LinkMap[409].Right = 500;
      page2.LinkMap[419].Right = 501;
      page2.LinkMap[429].Right = 502;
      page2.LinkMap[439].Right = 503;
      page2.LinkMap[439].Down = 300;
      page2.PageOnLeft = 0;
      page2.PageOnRight = 0;
      page2.DefaultPoint = 500;
      UILinkPointNavigator.RegisterPage(page2, 4, false);
      page2.IsValidEvent += (Func<bool>) (() =>
      {
        if (Main.playerInventory)
          return Main.player[Main.myPlayer].chest != -1;
        return false;
      });
      UILinkPage page3 = new UILinkPage();
      page3.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      Func<string> func9 = (Func<string>) (() =>
      {
        int slot = UILinkPointNavigator.CurrentPoint - 2700;
        return ItemSlot.GetGamepadInstructions(Main.instance.shop[Main.npcShop].item, 15, slot);
      });
      for (int index = 2700; index <= 2739; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, index - 1, index + 1, index - 10, index + 10);
        uiLinkPoint.OnSpecialInteracts += func9;
        int num = index - 2700;
        if (num < 10)
          uiLinkPoint.Up = 40 + num;
        if (num >= 30)
          uiLinkPoint.Down = -2;
        if (num % 10 == 9)
          uiLinkPoint.Right = -4;
        if (num % 10 == 0)
          uiLinkPoint.Left = -3;
        page3.LinkMap.Add(index, uiLinkPoint);
      }
      page3.LinkMap[2739].Down = 300;
      page3.PageOnLeft = 0;
      page3.PageOnRight = 0;
      UILinkPointNavigator.RegisterPage(page3, 13, true);
      page3.IsValidEvent += (Func<bool>) (() =>
      {
        if (Main.playerInventory)
          return (uint) Main.npcShop > 0U;
        return false;
      });
      UILinkPage cp6 = new UILinkPage();
      cp6.LinkMap.Add(303, new UILinkPoint(303, true, 304, 304, 40, -2));
      cp6.LinkMap.Add(304, new UILinkPoint(304, true, 303, 303, 40, -2));
      cp6.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      Func<string> func10 = (Func<string>) (() => ItemSlot.GetGamepadInstructions(ref Main.reforgeItem, 5));
      cp6.LinkMap[303].OnSpecialInteracts += func10;
      cp6.LinkMap[304].OnSpecialInteracts += (Func<string>) (() => Lang.misc[53].Value);
      cp6.UpdateEvent += (Action) (() =>
      {
        if (Main.reforgeItem.type > 0)
        {
          cp6.LinkMap[303].Left = cp6.LinkMap[303].Right = 304;
        }
        else
        {
          if (UILinkPointNavigator.OverridePoint == -1 && cp6.CurrentPoint == 304)
            UILinkPointNavigator.ChangePoint(303);
          cp6.LinkMap[303].Left = -3;
          cp6.LinkMap[303].Right = -4;
        }
      });
      cp6.IsValidEvent += (Func<bool>) (() =>
      {
        if (Main.playerInventory)
          return Main.InReforgeMenu;
        return false;
      });
      cp6.PageOnLeft = 0;
      cp6.PageOnRight = 0;
      UILinkPointNavigator.RegisterPage(cp6, 5, true);
      UILinkPage cp7 = new UILinkPage();
      cp7.OnSpecialInteracts += (Func<string>) (() =>
      {
        if (PlayerInput.Triggers.JustPressed.Grapple)
        {
          Point tileCoordinates = Main.player[Main.myPlayer].Center.ToTileCoordinates();
          if (UILinkPointNavigator.CurrentPoint == 600)
          {
            if (WorldGen.MoveTownNPC(tileCoordinates.X, tileCoordinates.Y, -1))
              Main.NewText(Lang.inter[39].Value, byte.MaxValue, (byte) 240, (byte) 20, false);
            Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
          }
          else if (WorldGen.MoveTownNPC(tileCoordinates.X, tileCoordinates.Y, UILinkPointNavigator.Shortcuts.NPCS_LastHovered))
          {
            WorldGen.moveRoom(tileCoordinates.X, tileCoordinates.Y, UILinkPointNavigator.Shortcuts.NPCS_LastHovered);
            Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
          }
        }
        if (PlayerInput.Triggers.JustPressed.SmartSelect)
          UILinkPointNavigator.Shortcuts.NPCS_IconsDisplay = !UILinkPointNavigator.Shortcuts.NPCS_IconsDisplay;
        return PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]) + PlayerInput.BuildCommand(Lang.misc[70].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]) + PlayerInput.BuildCommand(Lang.misc[69].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["SmartSelect"]);
      });
      for (int index = 600; index <= 650; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, index + 10, index - 10, index - 1, index + 1);
        cp7.LinkMap.Add(index, uiLinkPoint);
      }
      cp7.UpdateEvent += (Action) (() =>
      {
        int num = UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn;
        if (num == 0)
          num = 100;
        for (int index = 0; index < 50; ++index)
        {
          cp7.LinkMap[600 + index].Up = index % num == 0 ? -1 : 600 + index - 1;
          if (cp7.LinkMap[600 + index].Up == -1)
            cp7.LinkMap[600 + index].Up = index < num * 2 ? (index < num ? 305 : 306) : 307;
          cp7.LinkMap[600 + index].Down = (index + 1) % num == 0 || index == UILinkPointNavigator.Shortcuts.NPCS_IconsTotal - 1 ? 308 : 600 + index + 1;
          cp7.LinkMap[600 + index].Left = index < UILinkPointNavigator.Shortcuts.NPCS_IconsTotal - num ? 600 + index + num : -3;
          cp7.LinkMap[600 + index].Right = index < num ? -4 : 600 + index - num;
        }
      });
      cp7.IsValidEvent += (Func<bool>) (() =>
      {
        if (Main.playerInventory)
          return Main.EquipPage == 1;
        return false;
      });
      cp7.PageOnLeft = 8;
      cp7.PageOnRight = 8;
      UILinkPointNavigator.RegisterPage(cp7, 6, true);
      UILinkPage cp8 = new UILinkPage();
      cp8.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      Func<string> func11 = (Func<string>) (() =>
      {
        int slot = UILinkPointNavigator.CurrentPoint - 180;
        return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 20, slot);
      });
      Func<string> func12 = (Func<string>) (() =>
      {
        int slot = UILinkPointNavigator.CurrentPoint - 180;
        return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 19, slot);
      });
      Func<string> func13 = (Func<string>) (() =>
      {
        int slot = UILinkPointNavigator.CurrentPoint - 180;
        return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 18, slot);
      });
      Func<string> func14 = (Func<string>) (() =>
      {
        int slot = UILinkPointNavigator.CurrentPoint - 180;
        return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 17, slot);
      });
      Func<string> func15 = (Func<string>) (() =>
      {
        int slot = UILinkPointNavigator.CurrentPoint - 180;
        return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 16, slot);
      });
      Func<string> func16 = (Func<string>) (() =>
      {
        int slot = UILinkPointNavigator.CurrentPoint - 185;
        return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscDyes, 12, slot);
      });
      for (int index = 180; index <= 184; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, 185 + index - 180, -4, index - 1, index + 1);
        int num1 = index - 180;
        if (num1 == 0)
          uiLinkPoint.Up = 305;
        int num2 = 4;
        if (num1 == num2)
          uiLinkPoint.Down = 308;
        cp8.LinkMap.Add(index, uiLinkPoint);
        switch (index)
        {
          case 180:
            uiLinkPoint.OnSpecialInteracts += func12;
            break;
          case 181:
            uiLinkPoint.OnSpecialInteracts += func11;
            break;
          case 182:
            uiLinkPoint.OnSpecialInteracts += func13;
            break;
          case 183:
            uiLinkPoint.OnSpecialInteracts += func14;
            break;
          case 184:
            uiLinkPoint.OnSpecialInteracts += func15;
            break;
        }
      }
      for (int index = 185; index <= 189; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, -3, index - 5, index - 1, index + 1);
        uiLinkPoint.OnSpecialInteracts += func16;
        int num1 = index - 185;
        if (num1 == 0)
          uiLinkPoint.Up = 306;
        int num2 = 4;
        if (num1 == num2)
          uiLinkPoint.Down = 308;
        cp8.LinkMap.Add(index, uiLinkPoint);
      }
      cp8.UpdateEvent += (Action) (() =>
      {
        cp8.LinkMap[184].Down = UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0 ? 9000 : 308;
        cp8.LinkMap[189].Down = UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0 ? 9000 : 308;
      });
      cp8.IsValidEvent += (Func<bool>) (() =>
      {
        if (Main.playerInventory)
          return Main.EquipPage == 2;
        return false;
      });
      cp8.PageOnLeft = 8;
      cp8.PageOnRight = 8;
      UILinkPointNavigator.RegisterPage(cp8, 7, true);
      UILinkPage cp9 = new UILinkPage();
      cp9.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      cp9.LinkMap.Add(305, new UILinkPoint(305, true, 306, -4, 308, -2));
      cp9.LinkMap.Add(306, new UILinkPoint(306, true, 307, 305, 308, -2));
      cp9.LinkMap.Add(307, new UILinkPoint(307, true, -3, 306, 308, -2));
      cp9.LinkMap.Add(308, new UILinkPoint(308, true, -3, -4, -1, 305));
      cp9.LinkMap[305].OnSpecialInteracts += func1;
      cp9.LinkMap[306].OnSpecialInteracts += func1;
      cp9.LinkMap[307].OnSpecialInteracts += func1;
      cp9.LinkMap[308].OnSpecialInteracts += func1;
      cp9.UpdateEvent += (Action) (() =>
      {
        switch (Main.EquipPage)
        {
          case 0:
            cp9.LinkMap[305].Down = 100;
            cp9.LinkMap[306].Down = 110;
            cp9.LinkMap[307].Down = 120;
            cp9.LinkMap[308].Up = 108 + Main.player[Main.myPlayer].extraAccessorySlots - 1;
            break;
          case 1:
            cp9.LinkMap[305].Down = 600;
            cp9.LinkMap[306].Down = UILinkPointNavigator.Shortcuts.NPCS_IconsTotal / UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn > 0 ? 600 + UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn : -2;
            cp9.LinkMap[307].Down = UILinkPointNavigator.Shortcuts.NPCS_IconsTotal / UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn > 1 ? 600 + UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn * 2 : -2;
            int num = UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn;
            if (num == 0)
              num = 100;
            if (num == 100)
              num = UILinkPointNavigator.Shortcuts.NPCS_IconsTotal;
            cp9.LinkMap[308].Up = 600 + num - 1;
            break;
          case 2:
            cp9.LinkMap[305].Down = 180;
            cp9.LinkMap[306].Down = 185;
            cp9.LinkMap[307].Down = -2;
            cp9.LinkMap[308].Up = UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0 ? 9000 : 184;
            break;
        }
      });
      cp9.IsValidEvent += (Func<bool>) (() => Main.playerInventory);
      cp9.PageOnLeft = 0;
      cp9.PageOnRight = 0;
      UILinkPointNavigator.RegisterPage(cp9, 8, true);
      UILinkPage cp10 = new UILinkPage();
      cp10.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      Func<string> func17 = (Func<string>) (() => ItemSlot.GetGamepadInstructions(ref Main.guideItem, 7));
      Func<string> HandleItem2 = (Func<string>) (() =>
      {
        if (Main.mouseItem.type < 1)
          return "";
        return ItemSlot.GetGamepadInstructions(ref Main.mouseItem, 22);
      });
      for (int index = 1500; index < 1550; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, index, index, -1, -2);
        if (index != 1500)
          uiLinkPoint.OnSpecialInteracts += HandleItem2;
        cp10.LinkMap.Add(index, uiLinkPoint);
      }
      cp10.LinkMap[1500].OnSpecialInteracts += func17;
      cp10.UpdateEvent += (Action) (() =>
      {
        int num1 = UILinkPointNavigator.Shortcuts.CRAFT_CurrentIngridientsCount;
        int num2 = num1;
        if (Main.numAvailableRecipes > 0)
          num2 += 2;
        if (num1 < num2)
          num1 = num2;
        if (UILinkPointNavigator.OverridePoint == -1 && cp10.CurrentPoint > 1500 + num1)
          UILinkPointNavigator.ChangePoint(1500);
        if (UILinkPointNavigator.OverridePoint == -1 && cp10.CurrentPoint == 1500 && !Main.InGuideCraftMenu)
          UILinkPointNavigator.ChangePoint(1501);
        for (int index = 1; index < num1; ++index)
        {
          cp10.LinkMap[1500 + index].Left = 1500 + index - 1;
          cp10.LinkMap[1500 + index].Right = index == num1 - 2 ? -4 : 1500 + index + 1;
        }
        cp10.LinkMap[1501].Left = -3;
        cp10.LinkMap[1500 + num1 - 1].Right = -4;
        cp10.LinkMap[1500].Down = num1 >= 2 ? 1502 : -2;
        cp10.LinkMap[1500].Left = num1 >= 1 ? 1501 : -3;
        cp10.LinkMap[1502].Up = Main.InGuideCraftMenu ? 1500 : -1;
      });
      cp10.LinkMap[1501].OnSpecialInteracts += (Func<string>) (() =>
      {
        if (Main.InGuideCraftMenu)
          return "";
        string str = "";
        Player player = Main.player[Main.myPlayer];
        bool flag1 = false;
        if (Main.mouseItem.type == 0 && player.ItemSpace(Main.recipe[Main.availableRecipe[Main.focusRecipe]].createItem) && !player.IsStackingItems())
        {
          flag1 = true;
          if (PlayerInput.Triggers.Current.Grapple && Main.stackSplit <= 1)
          {
            if (PlayerInput.Triggers.JustPressed.Grapple)
              UILinksInitializer.SomeVarsForUILinkers.SequencedCraftingCurrent = Main.recipe[Main.availableRecipe[Main.focusRecipe]];
            Main.stackSplit = Main.stackSplit != 0 ? Main.stackDelay : 15;
            if (UILinksInitializer.SomeVarsForUILinkers.SequencedCraftingCurrent == Main.recipe[Main.availableRecipe[Main.focusRecipe]])
            {
              Main.CraftItem(Main.recipe[Main.availableRecipe[Main.focusRecipe]]);
              Main.mouseItem = player.GetItem(player.whoAmI, Main.mouseItem, false, false);
            }
          }
        }
        else if (Main.mouseItem.type > 0 && Main.mouseItem.maxStack == 1 && ItemSlot.Equippable(ref Main.mouseItem, 0))
        {
          str += PlayerInput.BuildCommand(Lang.misc[67].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
          if (PlayerInput.Triggers.JustPressed.Grapple)
          {
            ItemSlot.SwapEquip(ref Main.mouseItem, 0);
            if (Main.player[Main.myPlayer].ItemSpace(Main.mouseItem))
              Main.mouseItem = player.GetItem(player.whoAmI, Main.mouseItem, false, false);
          }
        }
        bool flag2 = Main.mouseItem.stack <= 0;
        if (flag2 || Main.mouseItem.type == Main.recipe[Main.availableRecipe[Main.focusRecipe]].createItem.type && Main.mouseItem.stack < Main.mouseItem.maxStack)
        {
          if (flag2)
            str += PlayerInput.BuildCommand(Lang.misc[72].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"], PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
          else
            str += PlayerInput.BuildCommand(Lang.misc[72].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
        }
        if (!flag2 && Main.mouseItem.type == Main.recipe[Main.availableRecipe[Main.focusRecipe]].createItem.type && Main.mouseItem.stack < Main.mouseItem.maxStack)
          str += PlayerInput.BuildCommand(Lang.misc[93].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
        if (flag1)
          str += PlayerInput.BuildCommand(Lang.misc[71].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
        return str + HandleItem2();
      });
      cp10.ReachEndEvent += (Action<int, int>) ((current, next) =>
      {
        if (current == 1500)
          return;
        if (current == 1501)
        {
          if (next == -1)
          {
            if (Main.focusRecipe <= 0)
              return;
            --Main.focusRecipe;
          }
          else
          {
            if (next != -2 || Main.focusRecipe >= Main.numAvailableRecipes - 1)
              return;
            ++Main.focusRecipe;
          }
        }
        else if (next == -1)
        {
          if (Main.focusRecipe <= 0)
            return;
          UILinkPointNavigator.ChangePoint(1501);
          --Main.focusRecipe;
        }
        else
        {
          if (next != -2 || Main.focusRecipe >= Main.numAvailableRecipes - 1)
            return;
          UILinkPointNavigator.ChangePoint(1501);
          ++Main.focusRecipe;
        }
      });
      cp10.EnterEvent += (Action) (() => Main.recBigList = false);
      cp10.CanEnterEvent += (Func<bool>) (() =>
      {
        if (!Main.playerInventory)
          return false;
        if (Main.numAvailableRecipes <= 0)
          return Main.InGuideCraftMenu;
        return true;
      });
      cp10.IsValidEvent += (Func<bool>) (() =>
      {
        if (!Main.playerInventory)
          return false;
        if (Main.numAvailableRecipes <= 0)
          return Main.InGuideCraftMenu;
        return true;
      });
      cp10.PageOnLeft = 10;
      cp10.PageOnRight = 0;
      UILinkPointNavigator.RegisterPage(cp10, 9, true);
      UILinkPage cp11 = new UILinkPage();
      cp11.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      for (int index1 = 700; index1 < 1500; ++index1)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index1, true, index1, index1, index1, index1);
        int IHateLambda = index1;
        uiLinkPoint.OnSpecialInteracts += (Func<string>) (() =>
        {
          string str1 = "";
          bool flag = false;
          Player player = Main.player[Main.myPlayer];
          if (IHateLambda + Main.recStart < Main.numAvailableRecipes)
          {
            int index = Main.recStart + IHateLambda - 700;
            if (Main.mouseItem.type == 0 && player.ItemSpace(Main.recipe[Main.availableRecipe[index]].createItem) && !player.IsStackingItems())
            {
              flag = true;
              if (PlayerInput.Triggers.JustPressed.Grapple)
                UILinksInitializer.SomeVarsForUILinkers.SequencedCraftingCurrent = Main.recipe[Main.availableRecipe[index]];
              if (PlayerInput.Triggers.Current.Grapple && Main.stackSplit <= 1)
              {
                Main.stackSplit = Main.stackSplit != 0 ? Main.stackDelay : 15;
                if (UILinksInitializer.SomeVarsForUILinkers.SequencedCraftingCurrent == Main.recipe[Main.availableRecipe[index]])
                {
                  Main.CraftItem(Main.recipe[Main.availableRecipe[index]]);
                  Main.mouseItem = player.GetItem(player.whoAmI, Main.mouseItem, false, false);
                }
              }
            }
          }
          string str2 = str1 + PlayerInput.BuildCommand(Lang.misc[73].Value, (!flag ? 1 : 0) != 0, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
          if (flag)
            str2 += PlayerInput.BuildCommand(Lang.misc[71].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
          return str2;
        });
        cp11.LinkMap.Add(index1, uiLinkPoint);
      }
      cp11.UpdateEvent += (Action) (() =>
      {
        int num1 = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerRow;
        int craftIconsPerColumn = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerColumn;
        if (num1 == 0)
          num1 = 100;
        int num2 = num1 * craftIconsPerColumn;
        if (num2 > 800)
          num2 = 800;
        if (num2 > Main.numAvailableRecipes)
          num2 = Main.numAvailableRecipes;
        for (int index = 0; index < num2; ++index)
        {
          cp11.LinkMap[700 + index].Left = index % num1 == 0 ? -3 : 700 + index - 1;
          cp11.LinkMap[700 + index].Right = (index + 1) % num1 == 0 || index == Main.numAvailableRecipes - 1 ? -4 : 700 + index + 1;
          cp11.LinkMap[700 + index].Down = index < num2 - num1 ? 700 + index + num1 : -2;
          cp11.LinkMap[700 + index].Up = index < num1 ? -1 : 700 + index - num1;
        }
      });
      cp11.ReachEndEvent += (Action<int, int>) ((current, next) =>
      {
        int craftIconsPerRow = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerRow;
        if (next == -1)
        {
          Main.recStart -= craftIconsPerRow;
          if (Main.recStart >= 0)
            return;
          Main.recStart = 0;
        }
        else
        {
          if (next != -2)
            return;
          Main.recStart += craftIconsPerRow;
          Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
          if (Main.recStart <= Main.numAvailableRecipes - craftIconsPerRow)
            return;
          Main.recStart = Main.numAvailableRecipes - craftIconsPerRow;
        }
      });
      cp11.EnterEvent += (Action) (() => Main.recBigList = true);
      cp11.LeaveEvent += (Action) (() => Main.recBigList = false);
      cp11.CanEnterEvent += (Func<bool>) (() =>
      {
        if (Main.playerInventory)
          return Main.numAvailableRecipes > 0;
        return false;
      });
      cp11.IsValidEvent += (Func<bool>) (() =>
      {
        if (Main.playerInventory && Main.recBigList)
          return Main.numAvailableRecipes > 0;
        return false;
      });
      cp11.PageOnLeft = 0;
      cp11.PageOnRight = 9;
      UILinkPointNavigator.RegisterPage(cp11, 10, true);
      UILinkPage cp12 = new UILinkPage();
      cp12.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      for (int index = 2605; index < 2620; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, index, index, index, index);
        uiLinkPoint.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[73].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]));
        cp12.LinkMap.Add(index, uiLinkPoint);
      }
      cp12.UpdateEvent += (Action) (() =>
      {
        int num1 = 5;
        int num2 = 3;
        int num3 = num1 * num2;
        int num4 = Main.UnlockedMaxHair();
        for (int index = 0; index < num3; ++index)
        {
          cp12.LinkMap[2605 + index].Left = index % num1 == 0 ? -3 : 2605 + index - 1;
          cp12.LinkMap[2605 + index].Right = (index + 1) % num1 == 0 || index == num4 - 1 ? -4 : 2605 + index + 1;
          cp12.LinkMap[2605 + index].Down = index < num3 - num1 ? 2605 + index + num1 : -2;
          cp12.LinkMap[2605 + index].Up = index < num1 ? -1 : 2605 + index - num1;
        }
      });
      cp12.ReachEndEvent += (Action<int, int>) ((current, next) =>
      {
        int num = 5;
        if (next == -1)
        {
          Main.hairStart -= num;
          Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
        }
        else
        {
          if (next != -2)
            return;
          Main.hairStart += num;
          Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
        }
      });
      cp12.CanEnterEvent += (Func<bool>) (() => Main.hairWindow);
      cp12.IsValidEvent += (Func<bool>) (() => Main.hairWindow);
      cp12.PageOnLeft = 12;
      cp12.PageOnRight = 12;
      UILinkPointNavigator.RegisterPage(cp12, 11, true);
      UILinkPage page4 = new UILinkPage();
      page4.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      page4.LinkMap.Add(2600, new UILinkPoint(2600, true, -3, -4, -1, 2601));
      page4.LinkMap.Add(2601, new UILinkPoint(2601, true, -3, -4, 2600, 2602));
      page4.LinkMap.Add(2602, new UILinkPoint(2602, true, -3, -4, 2601, 2603));
      page4.LinkMap.Add(2603, new UILinkPoint(2603, true, -3, 2604, 2602, -2));
      page4.LinkMap.Add(2604, new UILinkPoint(2604, true, 2603, -4, 2602, -2));
      page4.UpdateEvent += (Action) (() =>
      {
        Vector3 hsl = Main.rgbToHsl(Main.selColor);
        float interfaceDeadzoneX = PlayerInput.CurrentProfile.InterfaceDeadzoneX;
        float x = PlayerInput.GamepadThumbstickLeft.X;
        float num1 = (double) x < -(double) interfaceDeadzoneX || (double) x > (double) interfaceDeadzoneX ? MathHelper.Lerp(0.0f, 0.008333334f, (float) (((double) Math.Abs(x) - (double) interfaceDeadzoneX) / (1.0 - (double) interfaceDeadzoneX))) * (float) Math.Sign(x) : 0.0f;
        int currentPoint = UILinkPointNavigator.CurrentPoint;
        int num2 = 2600;
        if (currentPoint == num2)
          Main.hBar = MathHelper.Clamp(Main.hBar + num1, 0.0f, 1f);
        int num3 = 2601;
        if (currentPoint == num3)
          Main.sBar = MathHelper.Clamp(Main.sBar + num1, 0.0f, 1f);
        int num4 = 2602;
        if (currentPoint == num4)
          Main.lBar = MathHelper.Clamp(Main.lBar + num1, 0.15f, 1f);
        Vector3 zero = Vector3.Zero;
        Vector3 one = Vector3.One;
        Vector3.Clamp(hsl, zero, one);
        if ((double) num1 == 0.0)
          return;
        if (Main.hairWindow)
          Main.player[Main.myPlayer].hairColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
        Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
      });
      page4.CanEnterEvent += (Func<bool>) (() => Main.hairWindow);
      page4.IsValidEvent += (Func<bool>) (() => Main.hairWindow);
      int num9 = 11;
      page4.PageOnLeft = num9;
      int num10 = 11;
      page4.PageOnRight = num10;
      int ID1 = 12;
      int num11 = 1;
      UILinkPointNavigator.RegisterPage(page4, ID1, num11 != 0);
      UILinkPage cp13 = new UILinkPage();
      for (int index = 0; index < 30; ++index)
      {
        cp13.LinkMap.Add(2900 + index, new UILinkPoint(2900 + index, true, -3, -4, -1, -2));
        cp13.LinkMap[2900 + index].OnSpecialInteracts += func1;
      }
      cp13.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      cp13.TravelEvent += (Action) (() =>
      {
        if (UILinkPointNavigator.CurrentPage != cp13.ID)
          return;
        int num1 = cp13.CurrentPoint - 2900;
        if (num1 >= 4)
          return;
        IngameOptions.category = num1;
      });
      cp13.UpdateEvent += (Action) (() =>
      {
        int num1 = UILinkPointNavigator.Shortcuts.INGAMEOPTIONS_BUTTONS_LEFT;
        if (num1 == 0)
          num1 = 5;
        if (UILinkPointNavigator.OverridePoint == -1 && cp13.CurrentPoint < 2930 && cp13.CurrentPoint > 2900 + num1 - 1)
          UILinkPointNavigator.ChangePoint(2900);
        for (int index = 2900; index < 2900 + num1; ++index)
        {
          cp13.LinkMap[index].Up = index - 1;
          cp13.LinkMap[index].Down = index + 1;
        }
        cp13.LinkMap[2900].Up = 2900 + num1 - 1;
        cp13.LinkMap[2900 + num1 - 1].Down = 2900;
        int num2 = cp13.CurrentPoint - 2900;
        if (num2 >= 4 || !PlayerInput.Triggers.JustPressed.MouseLeft)
          return;
        IngameOptions.category = num2;
        UILinkPointNavigator.ChangePage(1002);
      });
      cp13.EnterEvent += (Action) (() => cp13.CurrentPoint = 2900 + IngameOptions.category);
      cp13.PageOnLeft = cp13.PageOnRight = 1002;
      cp13.IsValidEvent += (Func<bool>) (() =>
      {
        if (Main.ingameOptionsWindow)
          return !Main.InGameUI.IsVisible;
        return false;
      });
      cp13.CanEnterEvent += (Func<bool>) (() =>
      {
        if (Main.ingameOptionsWindow)
          return !Main.InGameUI.IsVisible;
        return false;
      });
      UILinkPointNavigator.RegisterPage(cp13, 1001, true);
      UILinkPage cp14 = new UILinkPage();
      for (int index = 0; index < 30; ++index)
      {
        cp14.LinkMap.Add(2930 + index, new UILinkPoint(2930 + index, true, -3, -4, -1, -2));
        cp14.LinkMap[2930 + index].OnSpecialInteracts += func1;
      }
      cp14.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      cp14.UpdateEvent += (Action) (() =>
      {
        int num1 = UILinkPointNavigator.Shortcuts.INGAMEOPTIONS_BUTTONS_RIGHT;
        if (num1 == 0)
          num1 = 5;
        if (UILinkPointNavigator.OverridePoint == -1 && cp14.CurrentPoint >= 2930 && cp14.CurrentPoint > 2930 + num1 - 1)
          UILinkPointNavigator.ChangePoint(2930);
        for (int index = 2930; index < 2930 + num1; ++index)
        {
          cp14.LinkMap[index].Up = index - 1;
          cp14.LinkMap[index].Down = index + 1;
        }
        cp14.LinkMap[2930].Up = -1;
        cp14.LinkMap[2930 + num1 - 1].Down = -2;
        int num2 = PlayerInput.Triggers.JustPressed.Inventory ? 1 : 0;
        UILinksInitializer.HandleOptionsSpecials();
      });
      cp14.PageOnLeft = cp14.PageOnRight = 1001;
      cp14.IsValidEvent += (Func<bool>) (() => Main.ingameOptionsWindow);
      cp14.CanEnterEvent += (Func<bool>) (() => Main.ingameOptionsWindow);
      UILinkPointNavigator.RegisterPage(cp14, 1002, true);
      UILinkPage cp15 = new UILinkPage();
      cp15.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      for (int index = 1550; index < 1558; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, -3, -4, -1, -2);
        switch (index - 1550)
        {
          case 1:
          case 3:
          case 5:
            uiLinkPoint.Up = uiLinkPoint.ID - 2;
            uiLinkPoint.Down = uiLinkPoint.ID + 2;
            uiLinkPoint.Right = uiLinkPoint.ID + 1;
            break;
          case 2:
          case 4:
          case 6:
            uiLinkPoint.Up = uiLinkPoint.ID - 2;
            uiLinkPoint.Down = uiLinkPoint.ID + 2;
            uiLinkPoint.Left = uiLinkPoint.ID - 1;
            break;
        }
        cp15.LinkMap.Add(index, uiLinkPoint);
      }
      cp15.LinkMap[1550].Down = 1551;
      cp15.LinkMap[1550].Right = 120;
      cp15.LinkMap[1550].Up = 307;
      cp15.LinkMap[1551].Up = 1550;
      cp15.LinkMap[1552].Up = 1550;
      cp15.LinkMap[1552].Right = 121;
      cp15.LinkMap[1554].Right = 121;
      cp15.LinkMap[1555].Down = 1557;
      cp15.LinkMap[1556].Down = 1557;
      cp15.LinkMap[1556].Right = 122;
      cp15.LinkMap[1557].Up = 1555;
      cp15.LinkMap[1557].Down = 308;
      cp15.LinkMap[1557].Right = (int) sbyte.MaxValue;
      for (int index = 0; index < 7; ++index)
        cp15.LinkMap[1550 + index].OnSpecialInteracts += func1;
      cp15.UpdateEvent += (Action) (() =>
      {
        if (!Main.ShouldPVPDraw)
        {
          if (UILinkPointNavigator.OverridePoint == -1 && cp15.CurrentPoint != 1557)
            UILinkPointNavigator.ChangePoint(1557);
          cp15.LinkMap[1557].Up = -1;
          cp15.LinkMap[1557].Down = 308;
          cp15.LinkMap[1557].Right = (int) sbyte.MaxValue;
        }
        else
        {
          cp15.LinkMap[1557].Up = 1555;
          cp15.LinkMap[1557].Down = 308;
          cp15.LinkMap[1557].Right = (int) sbyte.MaxValue;
        }
        int infoacccount = UILinkPointNavigator.Shortcuts.INFOACCCOUNT;
        if (infoacccount > 0)
          cp15.LinkMap[1557].Up = 1558 + (infoacccount - 1) / 2 * 2;
        if (!Main.ShouldPVPDraw)
          return;
        if (infoacccount >= 1)
        {
          cp15.LinkMap[1555].Down = 1558;
          cp15.LinkMap[1556].Down = 1558;
        }
        else
        {
          cp15.LinkMap[1555].Down = 1557;
          cp15.LinkMap[1556].Down = 1557;
        }
        if (infoacccount >= 2)
          cp15.LinkMap[1556].Down = 1559;
        else
          cp15.LinkMap[1556].Down = 1557;
      });
      cp15.IsValidEvent += (Func<bool>) (() => Main.playerInventory);
      cp15.PageOnLeft = 8;
      cp15.PageOnRight = 8;
      UILinkPointNavigator.RegisterPage(cp15, 16, true);
      UILinkPage cp16 = new UILinkPage();
      cp16.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      for (int index = 1558; index < 1570; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, -3, -4, -1, -2);
        uiLinkPoint.OnSpecialInteracts += func1;
        switch (index - 1558)
        {
          case 1:
          case 3:
          case 5:
            uiLinkPoint.Up = uiLinkPoint.ID - 2;
            uiLinkPoint.Down = uiLinkPoint.ID + 2;
            uiLinkPoint.Right = uiLinkPoint.ID + 1;
            break;
          case 2:
          case 4:
          case 6:
            uiLinkPoint.Up = uiLinkPoint.ID - 2;
            uiLinkPoint.Down = uiLinkPoint.ID + 2;
            uiLinkPoint.Left = uiLinkPoint.ID - 1;
            break;
        }
        cp16.LinkMap.Add(index, uiLinkPoint);
      }
      cp16.UpdateEvent += (Action) (() =>
      {
        int infoacccount = UILinkPointNavigator.Shortcuts.INFOACCCOUNT;
        if (UILinkPointNavigator.OverridePoint == -1 && cp16.CurrentPoint - 1558 >= infoacccount)
          UILinkPointNavigator.ChangePoint(1558 + infoacccount - 1);
        for (int index1 = 0; index1 < infoacccount; ++index1)
        {
          bool flag = index1 % 2 == 0;
          int index2 = index1 + 1558;
          cp16.LinkMap[index2].Down = index1 < infoacccount - 2 ? index2 + 2 : 1557;
          cp16.LinkMap[index2].Up = index1 > 1 ? index2 - 2 : (Main.ShouldPVPDraw ? (flag ? 1555 : 1556) : -1);
          cp16.LinkMap[index2].Right = !flag || index1 + 1 >= infoacccount ? 123 + index1 / 4 : index2 + 1;
          cp16.LinkMap[index2].Left = flag ? -3 : index2 - 1;
        }
      });
      cp16.IsValidEvent += (Func<bool>) (() =>
      {
        if (Main.playerInventory)
          return UILinkPointNavigator.Shortcuts.INFOACCCOUNT > 0;
        return false;
      });
      cp16.PageOnLeft = 8;
      cp16.PageOnRight = 8;
      UILinkPointNavigator.RegisterPage(cp16, 17, true);
      UILinkPage cp17 = new UILinkPage();
      cp17.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      for (int index = 4000; index < 4010; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, -3, -4, -1, -2);
        switch (index - 4000)
        {
          case 0:
          case 1:
            uiLinkPoint.Right = 0;
            break;
          case 2:
          case 3:
            uiLinkPoint.Right = 10;
            break;
          case 4:
          case 5:
            uiLinkPoint.Right = 20;
            break;
          case 6:
          case 7:
            uiLinkPoint.Right = 30;
            break;
          case 8:
          case 9:
            uiLinkPoint.Right = 40;
            break;
        }
        cp17.LinkMap.Add(index, uiLinkPoint);
      }
      cp17.UpdateEvent += (Action) (() =>
      {
        int builderacccount = UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT;
        if (UILinkPointNavigator.OverridePoint == -1 && cp17.CurrentPoint - 4000 >= builderacccount)
          UILinkPointNavigator.ChangePoint(4000 + builderacccount - 1);
        for (int index1 = 0; index1 < builderacccount; ++index1)
        {
          int num1 = index1 % 2;
          int index2 = index1 + 4000;
          cp17.LinkMap[index2].Down = index1 < builderacccount - 1 ? index2 + 1 : -2;
          cp17.LinkMap[index2].Up = index1 > 0 ? index2 - 1 : -1;
        }
      });
      cp17.IsValidEvent += (Func<bool>) (() =>
      {
        if (Main.playerInventory)
          return UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 0;
        return false;
      });
      cp17.PageOnLeft = 8;
      cp17.PageOnRight = 8;
      UILinkPointNavigator.RegisterPage(cp17, 18, true);
      UILinkPage page5 = new UILinkPage();
      page5.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      page5.LinkMap.Add(2806, new UILinkPoint(2806, true, 2805, 2807, -1, 2808));
      page5.LinkMap.Add(2807, new UILinkPoint(2807, true, 2806, -4, -1, 2809));
      page5.LinkMap.Add(2808, new UILinkPoint(2808, true, 2805, 2809, 2806, -2));
      page5.LinkMap.Add(2809, new UILinkPoint(2809, true, 2808, -4, 2807, -2));
      page5.LinkMap.Add(2805, new UILinkPoint(2805, true, -3, 2806, -1, -2));
      page5.LinkMap[2806].OnSpecialInteracts += func1;
      page5.LinkMap[2807].OnSpecialInteracts += func1;
      page5.LinkMap[2808].OnSpecialInteracts += func1;
      page5.LinkMap[2809].OnSpecialInteracts += func1;
      page5.LinkMap[2805].OnSpecialInteracts += func1;
      page5.CanEnterEvent += (Func<bool>) (() => Main.clothesWindow);
      page5.IsValidEvent += (Func<bool>) (() => Main.clothesWindow);
      page5.EnterEvent += (Action) (() => Main.player[Main.myPlayer].releaseInventory = false);
      page5.LeaveEvent += (Action) (() => Main.player[Main.myPlayer].releaseUseTile = false);
      int num12 = 15;
      page5.PageOnLeft = num12;
      int num13 = 15;
      page5.PageOnRight = num13;
      int ID2 = 14;
      int num14 = 1;
      UILinkPointNavigator.RegisterPage(page5, ID2, num14 != 0);
      UILinkPage page6 = new UILinkPage();
      page6.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 1 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      page6.LinkMap.Add(2800, new UILinkPoint(2800, true, -3, -4, -1, 2801));
      page6.LinkMap.Add(2801, new UILinkPoint(2801, true, -3, -4, 2800, 2802));
      page6.LinkMap.Add(2802, new UILinkPoint(2802, true, -3, -4, 2801, 2803));
      page6.LinkMap.Add(2803, new UILinkPoint(2803, true, -3, 2804, 2802, -2));
      page6.LinkMap.Add(2804, new UILinkPoint(2804, true, 2803, -4, 2802, -2));
      page6.LinkMap[2800].OnSpecialInteracts += func1;
      page6.LinkMap[2801].OnSpecialInteracts += func1;
      page6.LinkMap[2802].OnSpecialInteracts += func1;
      page6.LinkMap[2803].OnSpecialInteracts += func1;
      page6.LinkMap[2804].OnSpecialInteracts += func1;
      page6.UpdateEvent += (Action) (() =>
      {
        Vector3 hsl = Main.rgbToHsl(Main.selColor);
        float interfaceDeadzoneX = PlayerInput.CurrentProfile.InterfaceDeadzoneX;
        float x = PlayerInput.GamepadThumbstickLeft.X;
        float num1 = (double) x < -(double) interfaceDeadzoneX || (double) x > (double) interfaceDeadzoneX ? MathHelper.Lerp(0.0f, 0.008333334f, (float) (((double) Math.Abs(x) - (double) interfaceDeadzoneX) / (1.0 - (double) interfaceDeadzoneX))) * (float) Math.Sign(x) : 0.0f;
        int currentPoint = UILinkPointNavigator.CurrentPoint;
        int num2 = 2800;
        if (currentPoint == num2)
          Main.hBar = MathHelper.Clamp(Main.hBar + num1, 0.0f, 1f);
        int num3 = 2801;
        if (currentPoint == num3)
          Main.sBar = MathHelper.Clamp(Main.sBar + num1, 0.0f, 1f);
        int num4 = 2802;
        if (currentPoint == num4)
          Main.lBar = MathHelper.Clamp(Main.lBar + num1, 0.15f, 1f);
        Vector3 zero = Vector3.Zero;
        Vector3 one = Vector3.One;
        Vector3.Clamp(hsl, zero, one);
        if ((double) num1 == 0.0)
          return;
        if (Main.clothesWindow)
        {
          Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
          switch (Main.selClothes)
          {
            case 0:
              Main.player[Main.myPlayer].shirtColor = Main.selColor;
              break;
            case 1:
              Main.player[Main.myPlayer].underShirtColor = Main.selColor;
              break;
            case 2:
              Main.player[Main.myPlayer].pantsColor = Main.selColor;
              break;
            case 3:
              Main.player[Main.myPlayer].shoeColor = Main.selColor;
              break;
          }
        }
        Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
      });
      page6.CanEnterEvent += (Func<bool>) (() => Main.clothesWindow);
      page6.IsValidEvent += (Func<bool>) (() => Main.clothesWindow);
      page6.EnterEvent += (Action) (() => Main.player[Main.myPlayer].releaseInventory = false);
      page6.LeaveEvent += (Action) (() => Main.player[Main.myPlayer].releaseUseTile = false);
      int num15 = 14;
      page6.PageOnLeft = num15;
      int num16 = 14;
      page6.PageOnRight = num16;
      int ID3 = 15;
      int num17 = 1;
      UILinkPointNavigator.RegisterPage(page6, ID3, num17 != 0);
      UILinkPage cp18 = new UILinkPage();
      cp18.UpdateEvent += (Action) (() => PlayerInput.GamepadAllowScrolling = true);
      for (int index = 0; index < 200; ++index)
        cp18.LinkMap.Add(3000 + index, new UILinkPoint(3000 + index, true, -3, -4, -1, -2));
      cp18.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[53].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + PlayerInput.BuildCommand(Lang.misc[82].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + UILinksInitializer.FancyUISpecialInstructions());
      cp18.UpdateEvent += (Action) (() =>
      {
        if (PlayerInput.Triggers.JustPressed.Inventory)
          UILinksInitializer.FancyExit();
        UILinkPointNavigator.Shortcuts.BackButtonInUse = false;
      });
      cp18.EnterEvent += (Action) (() => cp18.CurrentPoint = 3002);
      cp18.CanEnterEvent += (Func<bool>) (() =>
      {
        if (!Main.MenuUI.IsVisible)
          return Main.InGameUI.IsVisible;
        return true;
      });
      cp18.IsValidEvent += (Func<bool>) (() =>
      {
        if (!Main.MenuUI.IsVisible)
          return Main.InGameUI.IsVisible;
        return true;
      });
      UILinkPointNavigator.RegisterPage(cp18, 1004, true);
      UILinkPage cp19 = new UILinkPage();
      cp19.OnSpecialInteracts += (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[56].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]));
      Func<string> func18 = (Func<string>) (() => PlayerInput.BuildCommand(Lang.misc[94].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]));
      for (int index = 9000; index <= 9050; ++index)
      {
        UILinkPoint uiLinkPoint = new UILinkPoint(index, true, index + 10, index - 10, index - 1, index + 1);
        cp19.LinkMap.Add(index, uiLinkPoint);
        uiLinkPoint.OnSpecialInteracts += func18;
      }
      cp19.UpdateEvent += (Action) (() =>
      {
        int num1 = UILinkPointNavigator.Shortcuts.BUFFS_PER_COLUMN;
        if (num1 == 0)
          num1 = 100;
        for (int index = 0; index < 50; ++index)
        {
          cp19.LinkMap[9000 + index].Up = index % num1 == 0 ? -1 : 9000 + index - 1;
          if (cp19.LinkMap[9000 + index].Up == -1)
            cp19.LinkMap[9000 + index].Up = index < num1 ? 189 : 184;
          cp19.LinkMap[9000 + index].Down = (index + 1) % num1 == 0 || index == UILinkPointNavigator.Shortcuts.BUFFS_DRAWN - 1 ? 308 : 9000 + index + 1;
          cp19.LinkMap[9000 + index].Left = index < UILinkPointNavigator.Shortcuts.BUFFS_DRAWN - num1 ? 9000 + index + num1 : -3;
          cp19.LinkMap[9000 + index].Right = index < num1 ? -4 : 9000 + index - num1;
        }
      });
      cp19.IsValidEvent += (Func<bool>) (() =>
      {
        if (Main.playerInventory && Main.EquipPage == 2)
          return UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0;
        return false;
      });
      cp19.PageOnLeft = 8;
      cp19.PageOnRight = 8;
      UILinkPointNavigator.RegisterPage(cp19, 19, true);
      UILinkPage page7 = UILinkPointNavigator.Pages[UILinkPointNavigator.CurrentPage];
      int defaultPoint = page7.DefaultPoint;
      page7.CurrentPoint = defaultPoint;
      page7.Enter();
    }

    public static void FancyExit()
    {
      switch (UILinkPointNavigator.Shortcuts.BackButtonCommand)
      {
        case 1:
          Main.PlaySound(11, -1, -1, 1, 1f, 0.0f);
          Main.menuMode = 0;
          break;
        case 2:
          Main.PlaySound(11, -1, -1, 1, 1f, 0.0f);
          Main.menuMode = Main.menuMultiplayer ? 12 : 1;
          break;
        case 3:
          Main.menuMode = 0;
          IngameFancyUI.Close();
          break;
        case 4:
          Main.PlaySound(11, -1, -1, 1, 1f, 0.0f);
          Main.menuMode = 11;
          break;
        case 5:
          Main.PlaySound(11, -1, -1, 1, 1f, 0.0f);
          Main.menuMode = 11;
          break;
        case 6:
          UIVirtualKeyboard.Cancel();
          break;
      }
    }

    public static string FancyUISpecialInstructions()
    {
      string str1 = "";
      if (UILinkPointNavigator.Shortcuts.FANCYUI_SPECIAL_INSTRUCTIONS == 1)
      {
        if (PlayerInput.Triggers.JustPressed.HotbarMinus)
          UIVirtualKeyboard.CycleSymbols();
        string str2 = str1 + PlayerInput.BuildCommand(Lang.menu[235].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"]);
        if (PlayerInput.Triggers.JustPressed.MouseRight)
          UIVirtualKeyboard.BackSpace();
        string str3 = str2 + PlayerInput.BuildCommand(Lang.menu[236].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
        if (PlayerInput.Triggers.JustPressed.SmartCursor)
          UIVirtualKeyboard.Write(" ");
        str1 = str3 + PlayerInput.BuildCommand(Lang.menu[238].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["SmartCursor"]);
        if (UIVirtualKeyboard.CanSubmit)
        {
          if (PlayerInput.Triggers.JustPressed.HotbarPlus)
            UIVirtualKeyboard.Submit();
          str1 += PlayerInput.BuildCommand(Lang.menu[237].Value, 0 != 0, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
        }
      }
      return str1;
    }

    public static void HandleOptionsSpecials()
    {
      switch (UILinkPointNavigator.Shortcuts.OPTIONS_BUTTON_SPECIALFEATURE)
      {
        case 1:
          Main.bgScroll = (int) UILinksInitializer.HandleSlider((float) Main.bgScroll, 0.0f, 100f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 1f);
          Main.caveParallax = (float) (1.0 - (double) Main.bgScroll / 500.0);
          break;
        case 2:
          Main.musicVolume = UILinksInitializer.HandleSlider(Main.musicVolume, 0.0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
          break;
        case 3:
          Main.soundVolume = UILinksInitializer.HandleSlider(Main.soundVolume, 0.0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
          break;
        case 4:
          Main.ambientVolume = UILinksInitializer.HandleSlider(Main.ambientVolume, 0.0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
          break;
        case 5:
          double hBar = (double) Main.hBar;
          double num1 = 0.0;
          double num2 = 1.0;
          double num3 = 0.200000002980232;
          double num4 = 0.5;
          float num5 = Main.hBar = UILinksInitializer.HandleSlider((float) hBar, (float) num1, (float) num2, (float) num3, (float) num4);
          double num6 = (double) num5;
          if (hBar == num6)
            break;
          switch (Main.menuMode)
          {
            case 17:
              Main.player[Main.myPlayer].hairColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 18:
              Main.player[Main.myPlayer].eyeColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 19:
              Main.player[Main.myPlayer].skinColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 21:
              Main.player[Main.myPlayer].shirtColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 22:
              Main.player[Main.myPlayer].underShirtColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 23:
              Main.player[Main.myPlayer].pantsColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 24:
              Main.player[Main.myPlayer].shoeColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 25:
              Main.mouseColorSlider.Hue = num5;
              break;
            case 252:
              Main.mouseBorderColorSlider.Hue = num5;
              break;
          }
          Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
          break;
        case 6:
          double sBar = (double) Main.sBar;
          double num7 = 0.0;
          double num8 = 1.0;
          double interfaceDeadzoneX1 = (double) PlayerInput.CurrentProfile.InterfaceDeadzoneX;
          double num9 = 0.5;
          float num10 = Main.sBar = UILinksInitializer.HandleSlider((float) sBar, (float) num7, (float) num8, (float) interfaceDeadzoneX1, (float) num9);
          double num11 = (double) num10;
          if (sBar == num11)
            break;
          switch (Main.menuMode)
          {
            case 17:
              Main.player[Main.myPlayer].hairColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 18:
              Main.player[Main.myPlayer].eyeColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 19:
              Main.player[Main.myPlayer].skinColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 21:
              Main.player[Main.myPlayer].shirtColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 22:
              Main.player[Main.myPlayer].underShirtColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 23:
              Main.player[Main.myPlayer].pantsColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 24:
              Main.player[Main.myPlayer].shoeColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 25:
              Main.mouseColorSlider.Saturation = num10;
              break;
            case 252:
              Main.mouseBorderColorSlider.Saturation = num10;
              break;
          }
          Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
          break;
        case 7:
          double lBar = (double) Main.lBar;
          float num12 = 0.15f;
          if (Main.menuMode == 252)
            num12 = 0.0f;
          double num13 = (double) num12;
          double num14 = 1.0;
          double interfaceDeadzoneX2 = (double) PlayerInput.CurrentProfile.InterfaceDeadzoneX;
          double num15 = 0.5;
          float num16 = Main.lBar = UILinksInitializer.HandleSlider((float) lBar, (float) num13, (float) num14, (float) interfaceDeadzoneX2, (float) num15);
          double num17 = (double) num16;
          if (lBar == num17)
            break;
          switch (Main.menuMode)
          {
            case 17:
              Main.player[Main.myPlayer].hairColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 18:
              Main.player[Main.myPlayer].eyeColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 19:
              Main.player[Main.myPlayer].skinColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 21:
              Main.player[Main.myPlayer].shirtColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 22:
              Main.player[Main.myPlayer].underShirtColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 23:
              Main.player[Main.myPlayer].pantsColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 24:
              Main.player[Main.myPlayer].shoeColor = Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
              break;
            case 25:
              Main.mouseColorSlider.Luminance = num16;
              break;
            case 252:
              Main.mouseBorderColorSlider.Luminance = num16;
              break;
          }
          Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
          break;
        case 8:
          double aBar = (double) Main.aBar;
          double num18 = 0.0;
          double num19 = 1.0;
          double interfaceDeadzoneX3 = (double) PlayerInput.CurrentProfile.InterfaceDeadzoneX;
          double num20 = 0.5;
          float num21 = Main.aBar = UILinksInitializer.HandleSlider((float) aBar, (float) num18, (float) num19, (float) interfaceDeadzoneX3, (float) num20);
          double num22 = (double) num21;
          if (aBar == num22)
            break;
          if (Main.menuMode == 252)
            Main.mouseBorderColorSlider.Alpha = num21;
          Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
          break;
        case 9:
          bool left = PlayerInput.Triggers.Current.Left;
          bool right = PlayerInput.Triggers.Current.Right;
          if (PlayerInput.Triggers.JustPressed.Left || PlayerInput.Triggers.JustPressed.Right)
            UILinksInitializer.SomeVarsForUILinkers.HairMoveCD = 0;
          else if (UILinksInitializer.SomeVarsForUILinkers.HairMoveCD > 0)
            --UILinksInitializer.SomeVarsForUILinkers.HairMoveCD;
          if (UILinksInitializer.SomeVarsForUILinkers.HairMoveCD == 0 && left | right)
          {
            if (left)
              --Main.PendingPlayer.hair;
            if (right)
              ++Main.PendingPlayer.hair;
            UILinksInitializer.SomeVarsForUILinkers.HairMoveCD = 12;
          }
          int num23 = 51;
          if (Main.PendingPlayer.hair >= num23)
            Main.PendingPlayer.hair = 0;
          if (Main.PendingPlayer.hair >= 0)
            break;
          Main.PendingPlayer.hair = num23 - 1;
          break;
        case 10:
          Main.GameZoomTarget = UILinksInitializer.HandleSlider(Main.GameZoomTarget, 1f, 2f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
          break;
        case 11:
          Main.UIScale = UILinksInitializer.HandleSlider(Main.UIScaleWanted, 1f, 2f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
          Main.temporaryGUIScaleSlider = Main.UIScaleWanted;
          break;
      }
    }

    public class SomeVarsForUILinkers
    {
      public static Recipe SequencedCraftingCurrent;
      public static int HairMoveCD;
    }
  }
}
