﻿// Decompiled with JetBrains decompiler
// Type: Terraria.DataStructures.ColorSlidersSet
// Assembly: TerrariaServer, Version=1.3.5.3, Culture=neutral, PublicKeyToken=null
// MVID: AA3606A2-F3DB-4481-937B-7295FB97CD3E
// Assembly location: E:\TSHOCK\TerrariaServer.exe

using Microsoft.Xna.Framework;

namespace Terraria.DataStructures
{
  public class ColorSlidersSet
  {
    public float Alpha = 1f;
    public float Hue;
    public float Saturation;
    public float Luminance;

    public void SetHSL(Color color)
    {
      Vector3 hsl = Main.rgbToHsl(color);
      this.Hue = hsl.X;
      this.Saturation = hsl.Y;
      this.Luminance = hsl.Z;
    }

    public void SetHSL(Vector3 vector)
    {
      this.Hue = vector.X;
      this.Saturation = vector.Y;
      this.Luminance = vector.Z;
    }

    public Color GetColor()
    {
      Color rgb = Main.hslToRgb(this.Hue, this.Saturation, this.Luminance);
      rgb.A = (byte) ((double) this.Alpha * (double) byte.MaxValue);
      return rgb;
    }

    public Vector3 GetHSLVector()
    {
      return new Vector3(this.Hue, this.Saturation, this.Luminance);
    }

    public void ApplyToMainLegacyBars()
    {
      Main.hBar = this.Hue;
      Main.sBar = this.Saturation;
      Main.lBar = this.Luminance;
      Main.aBar = this.Alpha;
    }
  }
}
