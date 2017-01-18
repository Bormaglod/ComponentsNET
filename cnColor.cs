/*
 * Created by SharpDevelop.
 * User: Сергей
 * Date: 20.11.2006
 * Time: 0:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace ComponentsNET
{
	public class cnColor
	{
		private double hue;
		private double saturation;
		private double luminance;

		public static implicit operator cnColor(Color c)
		{
			cnColor cn = new cnColor();

			byte minval = Math.Min(c.R, Math.Min(c.G, c.B));
			byte maxval = Math.Max(c.R, Math.Max(c.G, c.B));
			double mdiff  = (double)maxval - (double)minval;
			double msum   = (double)maxval + (double)minval;
   
			cn.luminance = msum / 510.0f;

			if (maxval == minval) 
			{
				cn.saturation = 0.0f;
				cn.hue = 0.0f; 
			}   
			else 
			{ 
				double rnorm = (maxval - c.R ) / mdiff;      
				double gnorm = (maxval - c.G ) / mdiff;
				double bnorm = (maxval - c.B ) / mdiff;   

				cn.saturation = (cn.luminance <= 0.5f) ? (mdiff / msum) : (mdiff / (510.0f - msum));

				if (c.R == maxval) cn.hue = 60.0f * (6.0f + bnorm - gnorm);
				if (c.G == maxval) cn.hue = 60.0f * (2.0f + rnorm - bnorm);
				if (c.B == maxval) cn.hue = 60.0f * (4.0f + gnorm - rnorm);
				if (cn.hue > 360.0f) cn.hue = cn.hue - 360.0f;
			}

			return cn;
		}

		public static implicit operator Color(cnColor cn)
		{
			byte r,g,b;

			if (cn.saturation == 0.0)
			{
				r = g = b = (byte)(cn.luminance * 255.0);
			}
			else
			{
				double rm1, rm2;
         
				if (cn.luminance <= 0.5f)
					rm2 = cn.luminance + cn.luminance * cn.saturation;  
				else
					rm2 = cn.luminance + cn.saturation - cn.luminance * cn.saturation;
				rm1 = 2.0f * cn.luminance - rm2;

				r = ToRGBHelper(rm1, rm2, cn.hue + 120.0f);   
				g = ToRGBHelper(rm1, rm2, cn.hue);
				b = ToRGBHelper(rm1, rm2, cn.hue - 120.0f);
			}

			return Color.FromArgb(r, g, b);
		}
	
		private static byte ToRGBHelper(double rm1, double rm2, double rh)
		{
			if      (rh > 360.0f) rh -= 360.0f;
			else if (rh <   0.0f) rh += 360.0f;
 
			if      (rh <  60.0f) rm1 = rm1 + (rm2 - rm1) * rh / 60.0f;   
			else if (rh < 180.0f) rm1 = rm2;
			else if (rh < 240.0f) rm1 = rm1 + (rm2 - rm1) * (240.0f - rh) / 60.0f;      
                   
			return (byte)(rm1 * 255);
		}

		public double Hue
		{
			get
			{
				return hue;
			}
			set
			{
				hue = value;
			}
		}

		public double Saturation
		{
			get
			{
				return saturation;
			}
			set
			{
				saturation = value;
			}
		}

		public double Luminance
		{
			get
			{
				return luminance;
			}
			set
			{
				luminance = value;
			}
		}
	}
}
