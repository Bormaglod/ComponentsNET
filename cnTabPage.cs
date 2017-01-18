/*
 * Created by SharpDevelop.
 * User: —ергей
 * Date: 19.11.2006
 * Time: 20:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace ComponentsNET
{
	/// <summary>
	/// Represents a single tab page in a <see cref="cnTabControl"></see>.
	/// </summary>
	[ToolboxItem(false)]
	[Designer(typeof(ComponentsNET.cnTabPageDesigner))]
	public class cnTabPage : System.Windows.Forms.Panel
	{
		// свойства страницы
		private cnTabColor pageColor;
		
		// свойства закладок
		private string title;
		private cnTabColor tabColor;
		private string toolTip;
		private cnTabColor tabActiveColor;
		
		public cnTabPage()
		{
			base.Anchor = AnchorStyles.Top | AnchorStyles.Left | 
				AnchorStyles.Bottom | AnchorStyles.Right;
			
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			
			title = "TabPage";
			toolTip = "TabPage";
			
			CreateColors();
		}
		
		[Category("Page")]
		public string ToolTip
		{
			get { return ( this.toolTip.Length == 0 ) ? this.Title : this.toolTip; }
			set { this.toolTip = value; }
		}
		
		[Localizable(true)]
		[Category("Page")]
		[DefaultValue("TabPage")]
		public string Title
		{
			get { return title; }
			set 
			{ 
				if (title != value)
				{
					title = value;
					OnTitleChanged();
				}
			}
		}
		
		[Category("Page")]
		[NotifyParentProperty(true)]
		public cnTabColor TabColor
		{
			get { return tabColor; }
			set 
			{
				tabColor = value;
				if (Owner != null)
					tabColor.Update(Owner.GetBoundsNormalTab(), Owner.Alignment);
				tabColor.ColorChanged += new cnTabColor.ColorChangedEventHandler(TabColorChangedEvent);
			}
		}
		
		[Category("Page")]
		[NotifyParentProperty(true)]
		public cnTabColor TabActiveColor
		{
			get { return tabActiveColor; }
			set 
			{
				tabActiveColor = value;
				if (Owner != null)
					tabActiveColor.Update(Owner.GetBoundsSelectTab(), Owner.Alignment);
				tabActiveColor.ColorChanged += new cnTabColor.ColorChangedEventHandler(TabColorChangedEvent);
			}
		}
		
		[Category("Page")]
		[NotifyParentProperty(true)]
		public cnTabColor PageColor
		{
			get { return pageColor; }
			set
			{
				pageColor = value;
				if (Owner != null)
					pageColor.Update(new Rectangle(-2, -2, Width + 1, Height + 1), Owner.Alignment);
				pageColor.ColorChanged += new cnTabColor.ColorChangedEventHandler(PageColorChangedEvent);
			}
		}
		
		[DefaultValue(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right)]
		public override AnchorStyles Anchor
		{
			get
			{
				return base.Anchor;
			}
			set
			{
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		new public Point Location
		{
			get { return base.Location; }
			set { base.Location = value; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		new public Size Size
		{
			get { return base.Size; }
			set { base.Size = value; }
		}
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		new public System.Drawing.Font Font
		{
			get	{ return base.Font; }
			set	{ base.Font = value; }
		}
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		new public Color ForeColor
		{
			get	{ return base.ForeColor; }
			set	{ base.ForeColor = value; }
		}
		
		private cnTabControl Owner
		{
			get { return this.Parent as cnTabControl; }
		}
		
		protected override void InitLayout()
		{
			base.InitLayout();
			
			tabColor.Update(Owner.GetBoundsNormalTab(), Owner.Alignment);
			tabActiveColor.Update(Owner.GetBoundsSelectTab(), Owner.Alignment);
			pageColor.Update(new Rectangle(-2, -2, Width + 1, Height + 1), Owner.Alignment);
			
		}
		private void CreateColors()
		{
			tabColor = new cnTabColor(Color.White, Color.Orange, TabControlFill.Top);
			tabColor.ColorChanged += new cnTabColor.ColorChangedEventHandler(TabColorChangedEvent);
			
			pageColor = new cnTabColor(Color.White, Color.WhiteSmoke, TabControlFill.Bottom);
			pageColor.ColorChanged += new cnTabColor.ColorChangedEventHandler(PageColorChangedEvent);
			
			tabActiveColor = new cnTabColor(Color.White, Color.Orange, TabControlFill.Top);
			tabActiveColor.ColorChanged += new cnTabColor.ColorChangedEventHandler(TabColorChangedEvent);
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			Graphics g = pe.Graphics;

			g.SmoothingMode = SmoothingMode.None;
			
			Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
			
			g.FillRectangle(pageColor.Brush, rect);
			g.DrawRectangle(pageColor.Pen, rect);
		}
		
		private void PageColorChangedEvent(cnTabColor sender)
		{
			Invalidate();
		}
		
		private void TabColorChangedEvent(cnTabColor sender)
		{
			OnTabColorChanged();
		}
		
		// events
		public delegate void TitleChangedEventHandler(cnTabPage page);
		public event TitleChangedEventHandler TitleChanged;

		protected void OnTitleChanged()
		{
			if (TitleChanged != null)
				TitleChanged(this);
		}

		public delegate void TabColorChangedEventHandler();
		public event TabColorChangedEventHandler TabColorChanged;
		
		protected void OnTabColorChanged()
		{
			if(TabColorChanged != null)
				TabColorChanged();
		}
		
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (Owner != null)
			{
				pageColor.Update(new Rectangle(-2, -2, Width + 1, Height + 1), Owner.Alignment);
				tabColor.Update(Owner.GetBoundsNormalTab(), Owner.Alignment);
				tabActiveColor.Update(Owner.GetBoundsSelectTab(), Owner.Alignment);
				Invalidate();
			}
		}
	}
}
