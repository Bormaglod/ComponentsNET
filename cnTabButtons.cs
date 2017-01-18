/*
 * Created by SharpDevelop.
 * User: Сергей
 * Date: 22.11.2006
 * Time: 13:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ComponentsNET
{
	public enum TabButtonAction { None, Left, Right, Up, Down, Action, Close };
	
	public class cnTabButton : ButtonBase
	{
		private TabButtonAction buttonAction;
		private bool selected;
		private Brush bg;
		private Brush bg_select = new SolidBrush(Color.FromArgb(255, 194, 207, 229));
		private Pen border = new Pen(Color.FromArgb(255, 51, 94, 168));
		private Brush fig = new SolidBrush(Color.Black);
		private Pen p_fig = new Pen(Color.Black);
		private Brush bg_press = new SolidBrush(Color.FromArgb(255, 153, 175, 212));		
		private cnTabControl tab;
		private bool down;
		
		public cnTabButton(cnTabControl Tab, TabButtonAction action)
		{
			buttonAction = action;
			selected = false;
			down = false;
			
			tab = Tab;
			bg = new SolidBrush(tab.BackColor);
			
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			
			SetStyle(ControlStyles.FixedHeight, true);
			SetStyle(ControlStyles.FixedWidth, true);
			
			this.Size = new Size(15, 15);
			
			this.Visible = false;
			this.TabStop = false;
		}
		
		public static int SizeButton
		{
			get { return 15; }
		}
		
		public TabButtonAction ButtonAction
		{
			get { return buttonAction; }
			set
			{
				if (this.buttonAction != value)
				{
					this.buttonAction = value;
					Invalidate();
				}
			}
		}
		
		protected void DrawButton(Graphics g)
		{
			Point[] points;
			
			switch (this.buttonAction)
			{
				case TabButtonAction.Action:
					g.FillRectangle(this.fig, new Rectangle(3, 3, 9, 2));
					points = new Point[3];
					points[0] = new Point(3, 7);
					points[1] = new Point(12, 7);
					points[2] = new Point(7, 12);
					g.FillPolygon(this.fig, points);
					break;
				case TabButtonAction.Close:
					p_fig.Width = 2;
					g.DrawLine(p_fig, 3, 3, 11, 11);
					g.DrawLine(p_fig, 3, 11, 11, 3);
					p_fig.Width = 1;
					g.DrawLine(p_fig, 3, 3, 11, 11);
					g.DrawLine(p_fig, 3, 11, 11, 3);
					break;
				case TabButtonAction.Down:
					points = new Point[3];
					points[0] = new Point(3, 5);
					points[1] = new Point(12, 5);
					points[2] = new Point(7, 10);
					g.FillPolygon(this.fig, points);
					break;
				case TabButtonAction.Up:
					points = new Point[3];
					points[0] = new Point(2, 10);
					points[1] = new Point(13, 10);
					points[2] = new Point(7, 4);
					g.FillPolygon(this.fig, points);
					break;
				case TabButtonAction.Left:
					points = new Point[3];
					points[0] = new Point(5, 7);
					points[1] = new Point(10, 2);
					points[2] = new Point(10, 12);
					g.FillPolygon(this.fig, points);
					break;
				case TabButtonAction.Right:
					points = new Point[3];
					points[0] = new Point(5, 2);
					points[1] = new Point(10, 7);
					points[2] = new Point(5, 12);
					g.FillPolygon(this.fig, points);
					break;
			}
		}
		
		protected override void OnPaint(PaintEventArgs pevent)
		{
			Graphics g = pevent.Graphics;

			g.SmoothingMode = SmoothingMode.None;
			
			Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
			if (selected)
			{
				if (down)
					g.FillRectangle(bg_press, rect);
				else
					g.FillRectangle(bg_select, rect);
				g.DrawRectangle(border, rect);
			}
			else
			{
				rect.Width++;
				rect.Height++;
				g.FillRectangle(bg, rect);	
			}
			DrawButton(g);
		}
		
		protected override void OnMouseMove(MouseEventArgs mevent)
		{
			selected = (this.ClientRectangle.Contains(mevent.X, mevent.Y));
			Invalidate();
			base.OnMouseMove(mevent);
		}
		
		protected override void OnMouseLeave(EventArgs eventargs)
		{
			if (selected)
			{
				selected = false;
				Invalidate();
			}
			base.OnMouseLeave(eventargs);
		}
		
		protected override void OnMouseDown(MouseEventArgs mevent)
		{
			if (!down)
			{
				down = true;
				Invalidate();
			}
			base.OnMouseDown(mevent);
		}
		
		public delegate void ActionEventHandler(cnTabButton sender, TabButtonAction action);
		public event ActionEventHandler Action;
		
		public void OnAction(cnTabButton sender, TabButtonAction action)
		{
			if (Action != null)
				Action(this, action);
		}
		
		protected override void OnMouseUp(MouseEventArgs mevent)
		{
			if (down)
			{
				down = false;
				Invalidate();
				OnAction(this, buttonAction);
			}
			base.OnMouseUp(mevent);
		}
	}
}
