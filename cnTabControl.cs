/*
 * Created by SharpDevelop.
 * User: Сергей
 * Date: 18.11.2006
 * Time: 16:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace ComponentsNET
{
	public enum TabControlView { OneNote, Square, Rounded, Buttons };
	public enum TabControlFill { Top, Center, Bottom, Linear, Solid };
	public enum TabTextDirection { Horizontal, Vertical };
	
	/// <summary>
	/// Варианты отображения кнопок управления
	/// </summary>
	public enum TabButtonStyles { None, NextPrev, Context, All };
	
	[ToolboxBitmap(typeof(System.Windows.Forms.TabControl))]
	//[Designer(typeof(ComponentsNET.cnTabControlDesigner))]
	public class cnTabControl : UserControl, ISupportInitialize
	{
		#region private property
		
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Цвет закладки
		/// </summary>
		private cnTabColor tabColor;
		
		/// <summary>
		/// Цвет закладки при наведении на нее указателем мыши
		/// </summary>
		private cnTabColor tabHover;
		
		/// <summary>
		/// Высота закладки в пикселах
		/// </summary>
		private int tabHeight;
		
		/// <summary>
		/// Расположение закладок. Возможные значения:
		/// <para>Left - слева</para>
		/// <para>Right - справа</para>
		/// <para>Top - сверху</para>
		/// <para>Bottom - снизу</para>
		/// </summary>
		private TabAlignment tabAlignment;
		
		/// <summary>
		/// Смещение выбранной закладки относительно верхней границы
		/// панели заголовков
		/// </summary>
		private int tabOffsetSel;
		
		/// <summary>
		/// Смещение закладки относительно верхней границы
		/// панели заголовков
		/// </summary>
		private int tabOffsetNormal;
		
		/// <summary>
		/// Цвет границы страницы и панели закладок
		/// </summary>
		private Color borderColor;
		
		/// <summary>
		/// Карандаш для borderColor
		/// </summary>
		private Pen borderPen;
		
		/// <summary>
		/// Кисть для фона
		/// </summary>
		private SolidBrush backBrush;
		
		/// <summary>
		/// Кисть для цвета символов заголовка страницы
		/// </summary>
		private SolidBrush foreBrush;
		
		/// <summary>
		/// Массив кнопок управления, состоящий из четырех элементов:
		/// <para>0 - left/down</para>
		/// <para>1 - right/up</para>
		/// <para>2 - action</para>
		/// <para>3 - close</para>
		/// </summary>
		private cnTabButton[] buttons;
		
		/// <summary>
		/// Указывает, что процесс создания кнопок не завершен и
		/// размещение их функцией RelocationButtons невозможно.
		/// </summary>
		private bool initProcess = true;
		
		/// <summary>
		/// Варианты использования кнопок:
		/// <para>None - кнопки отсутствуют</para>
		/// <para>NextPrev - кнопку для управления закладками</para>
		/// <para>Context - кнопка вызова контекстного меню,
		/// содержащего список закладок</para>
		/// <para>All - сочетание NextPrev и Context</para>
		/// </summary>
		private TabButtonStyles buttonStyles;
		
		/// <summary>
		/// Указывает на присутствие кнопки, предназначенной для 
		/// закрытия окна
		/// </summary>
		private bool closeButton;
		
		/// <summary>
		/// Текущая страница
		/// </summary>
		private int currentPage;
		
		/// <summary>
		/// Список страниц
		/// </summary>
		private cnTabPageCollection tabPages;
		
		/// <summary>
		/// Способ представления закладок
		/// </summary>
		private TabControlView tabView;
		
		/// <summary>
		/// Смещение закладок по горизонтали в случаях, когда
		/// пследние не умещаются в строке полностью
		/// </summary>
		private int offsetX;
		
		/// <summary>
		/// Ширина закладки в пикселах
		/// </summary>
		private int tabWidth;
		
		/// <summary>
		/// Графический путь для выделенной закладки
		/// </summary>
		private GraphicsPath tabPathSelected;
		
		/// <summary>
		/// Графический путь для невыделенной закладки
		/// </summary>
		private GraphicsPath tabPathNormal;
		
		/// <summary>
		/// Указывает на необходимость выделять цветом закладку при наведении
		/// на нее указателя мыши
		/// </summary>
		private bool hoverEnable;
		
		/// <summary>
		/// Номер подсвеченой закладки с помощью указателя мыши
		/// </summary>
		private int hoverPage;
		
		/// <summary>
		/// Направление текста на закладке. Возможные значения:
		/// <para>Horizontal - горизонтально</para>
		/// <para>Vertical - вертикально</para>
		/// </summary>
		private TabTextDirection textDirection;
		
		/// <summary>
		/// Радиус углов закладки
		/// </summary>
		private int tabRadius;

		/// <summary>
		/// Номер страницы для которой выдается подсказка
		/// </summary>
		private int toolTipPage;
		
		// Подсказка для закладки
		private ToolTip toolTip;
		
		#endregion
		
		#region constructors and destructors
		
		public cnTabControl()
		{
			SuspendLayout();
			AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			BackColor = System.Drawing.Color.FromArgb(255, 251, 250, 251);
			ForeColor = System.Drawing.SystemColors.ControlText;
			Name = "cnTabControl";
			Size = new System.Drawing.Size(200, 100);
			ResumeLayout();
			
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			
			tabPages = new cnTabPageCollection(this);
			this.tabPages.ClearComplete += new cnTabPageCollection.ClearCompleteEventHandler(TabPagesClearComplete);
			this.tabPages.InsertComplete += new cnTabPageCollection.InsertCompleteEventHandler(TabPagesInsertComplete);
			this.tabPages.RemoveComplete += new cnTabPageCollection.RemoveCompleteEventHandler(TabPagesRemoveComplete);
			this.tabPages.SetComplete += new cnTabPageCollection.SetCompleteEventHandler(TabPagesSetComplete);
			tabHeight = 25;
			tabAlignment = TabAlignment.Top;
			tabOffsetSel = 3;
			tabOffsetNormal = 6;
			borderColor = Color.FromArgb(255, 135, 155, 179);
			buttonStyles = TabButtonStyles.All;
			closeButton = false;
			currentPage = -1;
			offsetX = 0;
			tabWidth = 70;
			tabView = TabControlView.OneNote;
			tabPathSelected = new GraphicsPath();
			tabPathNormal = new GraphicsPath();
			hoverEnable = true;
			hoverPage = -1;
			textDirection = TabTextDirection.Horizontal;
			tabRadius = 10;
			toolTipPage = -1;
			toolTip = new ToolTip();
			toolTip.Active = true;
			
			CreateColors();
			GenerateTitlePath();
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) 
			{
				if (components != null) {
					components.Dispose();
				}
				borderPen.Dispose();
				backBrush.Dispose();
				foreBrush.Dispose();
			}
			base.Dispose(disposing);
		}
		
		#endregion
		
		#region private methods
		
		private void TabColorChangedEvent(cnTabColor sender)
		{
			Invalidate();
		}
		
		private void CreateColors()
		{
			tabColor = new cnTabColor(Color.White, Color.Orange, TabControlFill.Top);
			tabHover = new cnTabColor(Color.Orange, Color.White, TabControlFill.Top);
			borderPen = new Pen(borderColor);
			UpdateColors();
			backBrush = new SolidBrush(this.BackColor);
			foreBrush = new SolidBrush(this.ForeColor);
		}
		
		private void ItemClickMenu(object sender, System.EventArgs e)
		{
			int n = Convert.ToInt32((sender as ToolStripMenuItem).Tag);
			this.SetCurentPage(n);
		}
		
		private void buttonAction(cnTabButton sender, TabButtonAction action)
		{
			if ((action == TabButtonAction.Left) || (action == TabButtonAction.Up))
			{
				offsetX -= tabWidth;
				if (offsetX < 0) offsetX = 0;
			}
			else if ((action == TabButtonAction.Right) || (action == TabButtonAction.Down))
			{
				int l = tabWidth;
				if (tabView == TabControlView.OneNote)
					l += tabHeight / 2 + 3;
				offsetX += l;
				int full_l = l * TabPages.Count;
				Rectangle r = this.GetBoundsHeader(0);
				Rectangle rb = this.GetBoundsButtons();
				int d;
				if ((Alignment == TabAlignment.Top) || (Alignment == TabAlignment.Bottom))
					d = r.Width - rb.Width;
				else
					d = r.Height - rb.Height;
				
				if (full_l < d)
					offsetX = 0;
				else
					if (d + offsetX > full_l)
					offsetX = full_l - d;
			}
			else if (action == TabButtonAction.Action)
			{
				ContextMenuStrip menu = new ContextMenuStrip();
				foreach (cnTabPage tb in TabPages)
				{
					ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
					item.Text = tb.Title;
					int n = TabPages.IndexOf(tb);
					item.Tag = n;
					item.Checked = (n == currentPage);
					item.Click += new System.EventHandler(ItemClickMenu);
					menu.Items.Add(item);
				}
				menu.Show(sender, 0, sender.Size.Height);
			}
			Invalidate();
		}
		
		private Rectangle GetBoundsHeader(int offset)
		{
			int x = 0, y = 0;
			int w = Width - 1, h = Height - 1;
			switch (tabAlignment)
			{
				case TabAlignment.Top:
					y = offset;
					h = tabHeight - offset - 1;
					break;
				case TabAlignment.Bottom:
					y = Height - tabHeight;
					h = tabHeight - offset - 1;
					break;
				case TabAlignment.Left:
					x = offset;
					w = tabHeight - offset - 1;
					break;
				case TabAlignment.Right:
					x = Width - tabHeight;
					w = tabHeight - offset - 1;
					break;
			}
			Rectangle r = new Rectangle(x, y, w, h);
			return r;
		}
		
		private Rectangle GetBoundsPage()
		{
			int x = 0, y = 0;
			int w = Width - 1, h = Height - 1;
			switch (tabAlignment)
			{
				case TabAlignment.Top:
					y = tabHeight - 1;
					h -= tabHeight - 1;
					break;
				case TabAlignment.Bottom:
					h -= tabHeight - 1;
					break;
				case TabAlignment.Left:
					x = tabHeight - 1;
					w -= tabHeight - 1;
					break;
				case TabAlignment.Right:
					w -= tabHeight - 1;
					break;
			}
			return new Rectangle(x, y, w, h);
		}
		
		private Rectangle GetBoundsButtons()
		{
			int x = 0, y = 0, h = 0, w = 0;
			int l = this.CountButtons() * cnTabButton.SizeButton + 6;
			
			switch (tabAlignment)
			{
				case TabAlignment.Top:
					x = Width - l; y = 0;
					w = l; h = tabHeight;
					break;
				case TabAlignment.Right:
					x = Width - tabHeight - 1; y = Height - l;
					w = tabHeight; h = l;
					break;
				case TabAlignment.Left:
					x = 0; y = Height - l;
					w = tabHeight; h = l;
					break;
				case TabAlignment.Bottom:
					x = Width - l; y = Height - tabHeight - 1;
					w = l; h = tabHeight;
					break;
			}
			return new Rectangle(x, y, w, h);
		}
		
		protected Rectangle GetBoundsTab(int tab)
		{
			int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
			int d = tabWidth;
			if (tabView == TabControlView.OneNote)
				d += tabHeight >> 1;
			
			int dh = (tab == currentPage)? this.tabOffsetSel : this.tabOffsetNormal;
			
			switch (tabAlignment)
			{
				case TabAlignment.Top:
					x1 = tab * d - offsetX; 
					x2 = (tab + 1) * d - offsetX;
					y1 = dh; 
					y2 = tabHeight;
					break;
				case TabAlignment.Right:
					x1 = Width - tabHeight - 1; 
					x2 = Width - 1 - dh;
					y1 = tab * d - offsetX; 
					y2 = (tab + 1) * d - offsetX;
					break;
				case TabAlignment.Left:
					x1 = dh; 
					x2 = tabHeight;
					y1 = tab * d - offsetX; 
					y2 = (tab + 1) * d - offsetX;
					break;
				case TabAlignment.Bottom:
					x1 = tab * d - offsetX; 
					x2 = (tab + 1) * d - offsetX;
					y1 = Height - tabHeight - 1; 
					y2 = Height - 1 - dh;
					break;
			}
			return new Rectangle(x1, y1, x2 - x1 - 1, y2 - y1 - 1);
		}
		
		/// <summary>
		/// Эта функция используется для определения количества видимых
		/// кнопок
		/// </summary>
		/// <returns>Количество видимых кнопок</returns>
		private int CountButtons()
		{
			int res = 0;
			foreach (Control c in this.Controls)
			{
				if (c is cnTabButton)
					if (c.Visible) res++;
			}
			return res;
		}
		
		/// <summary>
		/// В зависимости от свойства Alignment кнопки размещаются в 
		/// соответствующем месте cnTabControl
		/// Количество и тип кнопок определяет свойство PageManager
		/// </summary>
		private void RelocationButtons()
		{
			if (!initProcess)
			{
				for (int i = 0; i < 4; i++)
				{
					switch (buttons[i].ButtonAction)
					{
						case TabButtonAction.None:
							buttons[i].Visible = true;
							break;
						case TabButtonAction.Action:
							buttons[i].Visible = (buttonStyles == TabButtonStyles.All) || (buttonStyles == TabButtonStyles.Context);
							break;
						case TabButtonAction.Close:
							buttons[i].Visible = this.closeButton;
							break;
						case TabButtonAction.Down:
						case TabButtonAction.Right:
						case TabButtonAction.Up:
						case TabButtonAction.Left:
							buttons[i].Visible = (buttonStyles == TabButtonStyles.All) || (buttonStyles == TabButtonStyles.NextPrev);
							break;
					}
				}
				int x = 0, y = 0;
				int d = (tabHeight - cnTabButton.SizeButton + 1) >> 1;
				int dx = 0, dy = 0;
				AnchorStyles styles = AnchorStyles.None;
				switch (Alignment)
				{
					case TabAlignment.Top:
						y = d; dx = -cnTabButton.SizeButton; x = Width - 3 + dx;
						buttons[0].ButtonAction = TabButtonAction.Left;
						buttons[1].ButtonAction = TabButtonAction.Right;
						styles = AnchorStyles.Right | AnchorStyles.Top;
						break;
					case TabAlignment.Bottom:
						y = Height - tabHeight - 1 + d;
						dx = -cnTabButton.SizeButton; x = Width - 3 + dx;
						buttons[0].ButtonAction = TabButtonAction.Left;
						buttons[1].ButtonAction = TabButtonAction.Right;
						styles = AnchorStyles.Right | AnchorStyles.Bottom;
						break;
					case TabAlignment.Left:
						x = d; dy = -cnTabButton.SizeButton; y = Height - 3 + dy;
						buttons[0].ButtonAction = TabButtonAction.Up;
						buttons[1].ButtonAction = TabButtonAction.Down;
						styles = AnchorStyles.Left | AnchorStyles.Bottom;
						break;
					case TabAlignment.Right:
						x = Width - tabHeight - 1 + d;
						dy = -cnTabButton.SizeButton; y = Height - 3 + dy;
						buttons[0].ButtonAction = TabButtonAction.Up;
						buttons[1].ButtonAction = TabButtonAction.Down;
						styles = AnchorStyles.Right | AnchorStyles.Bottom;
						break;
				}
				
				for (int i = 3; i >= 0; i--)
				{
					if (buttons[i].Visible)
					{
						buttons[i].Location = new Point(x, y);
						buttons[i].Anchor = styles;
						x += dx;
						y += dy;
					}
				}
			}
		}
		
		/// <summary>
		/// Устанавливает текущей новую страницу
		/// </summary>
		/// <param name="n">Индекс новой страницы</param>
		private void SetCurentPage(int n)
		{
			TabPages[n].Visible = true;

			if ((n != currentPage) && (n >= 0) && (n < TabPages.Count))
			{
				if ((currentPage >= 0) && (currentPage < TabPages.Count))
					TabPages[currentPage].Visible = false;

				if (this.OnCanSelect(TabPages[n]))
				{
					currentPage = n;
					Rectangle rTab = this.GetBoundsTab(currentPage);
					Rectangle rBar = this.GetBoundsHeader(0);
					if ((Alignment == TabAlignment.Top) || (Alignment == TabAlignment.Bottom))
						rBar.Width -= GetBoundsButtons().Width;
					else
						rBar.Height -= GetBoundsButtons().Height;
					int d = 0;
					if (tabView == TabControlView.OneNote)
						d += tabHeight / 2;
					if ((tabAlignment == TabAlignment.Top) || (tabAlignment == TabAlignment.Bottom))
					{
						if (rTab.Right + d > rBar.Right)
						{
							offsetX += rTab.Right - rBar.Right + d;
							if (currentPage != TabPages.Count - 1)
								offsetX += 20;
						}
						else if (rTab.Left < rBar.Left)
						{
							if (currentPage != 0)
								offsetX -= rBar.Left - rTab.Left + 20;
							else
								offsetX = 0;
						}
					}
					else
					{
						if (rTab.Bottom + d > rBar.Bottom)
						{
							offsetX += rTab.Bottom - rBar.Bottom + d;
							if (currentPage != TabPages.Count - 1)
								offsetX += 20;
						}
						else if (rTab.Top < rBar.Top)
						{
							if (currentPage != 0)
								offsetX -= rBar.Top - rTab.Top + 20;
							else
								offsetX = 0;
						}
					}
					Invalidate();

					OnSelect(TabPages[currentPage]);
				}
			}
		}
		
		private GraphicsPath GetTitlePath(int nPage)
		{
			GraphicsPath path = new GraphicsPath();

			path.Reset();
			if (nPage == currentPage)
				path.AddPath(tabPathSelected, true);
			else
				path.AddPath(tabPathNormal, true);

			Matrix m = new Matrix();
			
			int d = tabWidth;;
			if (tabView == TabControlView.OneNote)
				d += tabHeight / 2;
			
			switch (tabAlignment)
			{
				case TabAlignment.Top:
					m.Translate(d * nPage - offsetX, 0);
					break;
				case TabAlignment.Right:
					m.Rotate(90);
					m.Translate(d * nPage - offsetX, -Width + 1);
					break;
				case TabAlignment.Left:
					m.Translate(0, nPage * d - offsetX);
					break;
				case TabAlignment.Bottom:
					m.Rotate(-90);
					m.Translate(-Height + 1, nPage * d - offsetX);
					break;
			}
			
			path.Transform(m);

			return path;
		}
		
		private void DrawTab(Graphics g, int nPage)
		{
			GraphicsPath path = GetTitlePath(nPage);
			Rectangle rect = GetBoundsTab(nPage);
			
			Brush brush;
			
			if (hoverPage == nPage)
				brush = tabHover.Brush;
			else if (currentPage == nPage)
				brush = TabPages[nPage].TabActiveColor.Brush;
			else
				brush = TabPages[nPage].TabColor.Brush;
			
			g.FillPath(brush, path);
			g.DrawPath(borderPen, path);
			
			if ((currentPage == nPage) && (tabView != TabControlView.Buttons))
			{
				PointF p1 = new PointF(path.PathPoints[0].X, path.PathPoints[0].Y);
				PointF p2 = new PointF(path.PathPoints[path.PointCount - 1].X, path.PathPoints[path.PointCount - 1].Y);
				if ((Alignment == TabAlignment.Bottom) || (Alignment == TabAlignment.Top))
				{
					p1.X++; p2.X--;
				}
				else
				{
					p1.Y++; p2.Y--;
				}
				
				g.SmoothingMode = SmoothingMode.None;
				g.DrawLine(tabPages[currentPage].PageColor.BorderPen, p1, p2);
			}
			StringFormat f = new StringFormat();
			f.Alignment = StringAlignment.Center;
			f.LineAlignment = StringAlignment.Center;
			if (textDirection == TabTextDirection.Vertical)
			{
				f.FormatFlags = StringFormatFlags.DirectionVertical;
			}
			Font font;
			if (currentPage == nPage)
				font = new Font(this.Font, FontStyle.Bold);
			else
				font = this.Font.Clone() as Font;
			Rectangle textRect = new Rectangle(rect.Location, rect.Size);
			if (this.TabView == TabControlView.OneNote)
			{
				if ((this.tabAlignment == TabAlignment.Bottom) || (this.tabAlignment == TabAlignment.Top))
					textRect.Width -= tabHeight / 2;
				else
					textRect.Height -= tabHeight / 2;
			}
			g.DrawString(TabPages[nPage].Title, font, foreBrush, textRect, f);
			/*if (this.Focused)
			{
				textRect.X += 3; textRect.Y += 3;
				textRect.Width -= 7; textRect.Height -= 7;
				Pen p = new Pen(Color.Black);
				p.DashStyle = DashStyle.Dot;
				SmoothingMode sm = g.SmoothingMode;
				try
				{
					g.SmoothingMode = SmoothingMode.None;
					g.DrawRectangle(p, textRect);
				}
				finally
				{
					g.SmoothingMode = sm;
				}
			}*/
		}
		
		private void DrawButtons(Graphics g)
		{
			g.SmoothingMode = SmoothingMode.None;
			Rectangle r = this.GetBoundsButtons();
			r.Inflate(-1, -1);
			g.FillRectangle(backBrush, r);
			switch (Alignment)
			{
				case TabAlignment.Top:
					g.DrawLine(borderPen, r.Left, r.Bottom, r.Right, r.Bottom);
					break;
				case TabAlignment.Bottom:
					g.DrawLine(borderPen, r.Left, r.Top, r.Right, r.Top);
					break;
				case TabAlignment.Left:
					g.DrawLine(borderPen, r.Right, r.Top, r.Right, r.Bottom);
					break;
				case TabAlignment.Right:
					g.DrawLine(borderPen, r.Left, r.Top, r.Left, r.Bottom);
					break;
			}
		}
		
		private void GenerateTitlePathOffset(GraphicsPath path, int offset)
		{
			path.Reset();
			
			path.StartFigure();
			if ((tabAlignment == TabAlignment.Left) || (tabAlignment == TabAlignment.Bottom))
			{
				switch (tabView)
				{
					case TabControlView.Square:
						path.AddLine(tabHeight - 1, 0, offset, 0);
						path.AddLine(offset, 0, offset, tabWidth - 1);
						path.AddLine(offset, tabWidth - 1, tabHeight - 1, tabWidth - 1);
						break;
					case TabControlView.OneNote:
						path.AddLine(tabHeight - 1, 0, tabRadius + offset, 0);
						path.AddArc(offset, 0, tabRadius, tabRadius, -90, -90);
						path.AddLine(offset, tabRadius, offset, tabWidth - 1);
						path.AddLine(offset, tabWidth - 1, tabHeight - 1, tabWidth + tabHeight);
						break;
					case TabControlView.Rounded:
						path.AddLine(tabHeight - 1, 0, tabRadius + offset, 0);
						path.AddArc(offset, 0, tabRadius, tabRadius, -90, -90);
						path.AddLine(offset, tabRadius, offset, tabWidth - tabRadius);
						path.AddArc(offset, tabWidth - tabRadius, tabRadius, tabRadius, -180, -90);
						path.AddLine(tabRadius + offset, tabWidth, tabHeight - 1, tabWidth);
						break;
					case TabControlView.Buttons:
						path.AddLine(tabHeight - 4, 3, offset, 3);
						path.AddLine(offset, 3, offset, tabWidth - 1);
						path.AddLine(offset, tabWidth, tabHeight - 4, tabWidth);
						break;
				}
			}
			else
			{
				switch (tabView)
				{
					case TabControlView.Square:
						path.AddLine(0, tabHeight - 1, 0, offset);
						path.AddLine(0, offset, tabWidth - 1, offset);
						path.AddLine(tabWidth - 1, offset, tabWidth - 1, tabHeight - 1);
						break;
					case TabControlView.OneNote:
						path.AddLine(0, tabHeight - 1, 0, tabRadius + offset);
						path.AddArc(0, offset, tabRadius, tabRadius, 180, 90);
						path.AddLine(tabRadius, offset, tabWidth - 1, offset);
						path.AddLine(tabWidth - 1, offset, tabWidth + tabHeight, tabHeight - 1);
						break;
					case TabControlView.Rounded:
						path.AddLine(0, tabHeight - 1, 0, tabRadius + offset);
						path.AddArc(0, offset, tabRadius, tabRadius, 180, 90);
						path.AddLine(tabRadius, offset, tabWidth - tabRadius, offset);
						path.AddArc(tabWidth - tabRadius, offset, tabRadius, tabRadius, -90, 90);
						path.AddLine(tabWidth, tabRadius + offset, tabWidth, tabHeight - 1);
						break;
					case TabControlView.Buttons:
						path.AddLine(3, tabHeight - 4, 3, offset);
						path.AddLine(3, offset, tabWidth, offset);
						path.AddLine(tabWidth, offset, tabWidth, tabHeight - 4);
						break;
				}
			}
			path.CloseFigure();
		}
		
		/// <summary>
		/// Создает два пути Graphics.Path для выбранной закладки и
		/// не выбранной
		/// </summary>
		private void GenerateTitlePath()
		{
			GenerateTitlePathOffset(tabPathSelected, this.tabOffsetSel);
			GenerateTitlePathOffset(tabPathNormal, this.tabOffsetNormal);
		}
		
		private void UpdateColors()
		{
			tabColor.Update(GetBoundsNormalTab(), Alignment);
			tabColor.ColorChanged += new cnTabColor.ColorChangedEventHandler(TabColorChangedEvent);
			tabHover.Update(GetBoundsNormalTab(), Alignment);
			tabHover.ColorChanged += new cnTabColor.ColorChangedEventHandler(TabColorChangedEvent);
		}
		
		#endregion
		
		#region published methods
		
		[Category("TabControl common")]
		[NotifyParentProperty(true)]
		public cnTabColor TabColor
		{
			get { return tabColor; }
			set
			{
				tabColor = value;
				UpdateColors();
			}
		}
		
		[Category("TabControl common")]
		[NotifyParentProperty(true)]
		public cnTabColor TabHover
		{
			get { return tabHover; }
			set
			{
				tabHover = value;
				UpdateColors();
			}
		}
		
		[Category("TabControl common")]
		[DefaultValue(TabAlignment.Top)]
		public TabAlignment Alignment
		{
			get { return this.tabAlignment;	}
			set
			{
				if (tabAlignment != value)
				{
					this.tabAlignment = value;
					TabPages.UpdatePagesSettings();
					this.GenerateTitlePath();
					RelocationButtons();
					UpdateColors();
					Invalidate();
					if (currentPage >= 0)
						TabPages[currentPage].Visible = true;
				}
			}
		}
		
		[Category("TabControl common")]
		[DefaultValue(typeof(Color), "135; 155; 179")]
		public Color BorderColor
		{
			get { return borderColor; }
			set
			{
				if (borderColor != value)
				{
					if (IsHandleCreated)
						borderPen.Dispose();
					borderColor = value;
					if (IsHandleCreated)
						borderPen = new Pen(borderColor);
					Invalidate();
				}
			}
		}
		
		new public Color BackColor
		{
			get { return base.BackColor; }
			set
			{
				if (IsHandleCreated)
					backBrush.Dispose();
				base.BackColor = value;
				if (IsHandleCreated)
					backBrush = new SolidBrush(value);
				Invalidate();
			}
		}
		
		new public Color ForeColor
		{
			get { return base.ForeColor; }
			set
			{
				if (IsHandleCreated)
					foreBrush.Dispose();
				base.ForeColor = value;
				if (IsHandleCreated)
					foreBrush = new SolidBrush(value);
				Invalidate();
			}
		}
		
		/// <summary>
		/// Список страниц <see cref="cnTabPage"></see>.
		/// </summary>
		[Category("TabControl common")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public cnTabPageCollection TabPages { get { return this.tabPages; } }
		
		/// <summary>
		/// Текущая выбрнная страница.
		/// </summary>
		/// <seealso cref="cnTabPage"></seealso>
		[Category("TabControl common")]
		[DefaultValue(-1)]
		public int SelectedIndex
		{
			get { return currentPage; }
			set {
				if ((value >= 0) && (value < TabPages.Count) && (TabPages.Count > 0) && (currentPage != value))
					this.SetCurentPage(value);
			}
		}
		
		[Category("TabControl common")]
		[DefaultValue(25)]
		public int TabHeight
		{
			get { return tabHeight; }
			set
			{
				if ((value > 16) && (tabHeight != value))
				{
					tabHeight = value;
					TabPages.UpdatePagesSettings();
					this.GenerateTitlePath();
					UpdateColors();
					Invalidate();
					if (currentPage >= 0)
						TabPages[currentPage].Visible = true;
				}
			}
		}
		
		[Category("TabControl common")]
		[DefaultValue(70)]
		public int TabWidth
		{
			get { return tabWidth; }
			set
			{
				if ((value > 25) && (tabWidth != value))
				{
					tabWidth = value;
					TabPages.UpdatePagesSettings();
					this.GenerateTitlePath();
					UpdateColors();
					Invalidate();
					if (currentPage >= 0)
						TabPages[currentPage].Visible = true;
				}
			}
		}
		
		[Category("TabControl common")]
		[DefaultValue(TabControlView.OneNote)]
		public TabControlView TabView
		{
			get { return tabView; }
			set
			{
				if (tabView != value)
				{
					tabView = value;
					if (tabView == TabControlView.Buttons)
					{
						tabOffsetSel = 3;
						tabOffsetNormal = 3;
					}
					else
					{
						tabOffsetSel = 3;
						tabOffsetNormal = 6;
					}
					TabPages.UpdatePagesSettings();
					this.GenerateTitlePath();
					UpdateColors();
					Invalidate();
					if (currentPage >= 0)
						TabPages[currentPage].Visible = true;
				}
			}
		}
		
		/// <summary>
		/// Варинты отображения кнопок управления <see cref="PageManager"></see> отображаемых на <see cref="cnTabControl"></see>
		/// </summary>
		/// <remarks>
		/// Есть несколько вариантов отображения кнопок:
		/// <list type="bullet">
		///	<listheader>
		///		<term>None</term>
		///		<description>Не отображается ни одной кнопки.</description>
		///	</listheader>
		///	<item>
		///		<term>NextPrev</term>
		///		<description>Отображаются только кнопки управления закладками.</description>
		///	</item>
		///	<item>
		///		<term>Context</term>
		///		<description>Отображается кнопка для вызова контекстного меню, содержащего весь список закладок.</description>
		///	</item>
		///	<item>
		///		<term>All</term>
		///		<description>Отображаются все кнопки.</description>
		///	</item>
		/// </list>
		/// </remarks>
		[Category("TabControl common")]
		[DefaultValue(TabButtonStyles.All)]
		public TabButtonStyles ButtonStyles
		{
			get { return buttonStyles; }
			set
			{
				if (buttonStyles != value)
				{
					buttonStyles = value;
					this.RelocationButtons();
				}
			}
		}
		
		[Category("TabControl common")]
		[DefaultValue(TabAlignment.Top)]
		public bool CloseButton
		{
			get { return this.closeButton; }
			set
			{
				if (closeButton != value)
				{
					closeButton = value;
					this.RelocationButtons();
				}
			}
		}
		
		[Category("TabControl common")]
		[DefaultValue(TabTextDirection.Horizontal)]
		public TabTextDirection TextDirection
		{
			get { return this.textDirection;	}
			set
			{
				if (textDirection != value)
				{
					this.textDirection = value;
					TabPages.UpdatePagesSettings();
					this.GenerateTitlePath();
					Invalidate();
					if (currentPage >= 0)
						TabPages[currentPage].Visible = true;
				}
			}
		}
		
		[Category("TabControl common")]
		[DefaultValue(10)]
		public int TabRadius
		{
			get { return tabRadius; }
			set
			{
				if ((value <= tabHeight) && (value > 0) && (tabRadius != value))
				{
					tabRadius = value;
					TabPages.UpdatePagesSettings();
					this.GenerateTitlePath();
					UpdateColors();
					Invalidate();
					if (currentPage >= 0)
						TabPages[currentPage].Visible = true;
				}
			}
		}
		
		[Category("Tabs property")]
		[DefaultValue(6)]
		public int TabOffset
		{
			get { return tabOffsetNormal; }
			set
			{
				if ((tabOffsetNormal != value) && (tabOffsetNormal > tabOffsetSel) && (tabOffsetNormal < tabHeight - 2))
				{
					tabOffsetNormal = value;
					UpdateColors();
					Invalidate();
				}
			}
		}
		
		[Category("HoverPage property")]
		[DefaultValue(true)]
		public bool HoverEnable
		{
			get { return this.hoverEnable; }
			set 
			{
				if (hoverEnable != value)
				{
					hoverEnable = value;
					Invalidate();
				}
			}
		}
		
		[Category("TabControl common")]
		[DefaultValue(true)]
		public bool EnableToolTip
		{
			get { return toolTip.Active; }
			set
			{
				toolTip.RemoveAll();
				toolTip.Active = value;
			}
		}
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public Control.ControlCollection Controls
		{
			get { return base.Controls; }
		}
		
		#endregion
		
		#region internal methods
		
		internal Rectangle GetBoundsNormalTab()
		{
			return GetBoundsHeader(tabOffsetNormal);
		}
		
		internal Rectangle GetBoundsSelectTab()
		{
			return GetBoundsHeader(tabOffsetSel);
		}
		
		#endregion
		
		#region ISupportInitialize methods
		
		public void BeginInit()	{}
		
		public void EndInit()
		{
			// Создадим кнопки
			buttons = new cnTabButton[4];
			buttons[0] = new cnTabButton(this, TabButtonAction.Left);
			buttons[1] = new cnTabButton(this, TabButtonAction.Right);
			buttons[2] = new cnTabButton(this, TabButtonAction.Action);
			buttons[3] = new cnTabButton(this, TabButtonAction.Close);
			
			// Процесс создания кнопок завершен, можно их размещать на форме
			initProcess = false;
			// Разместим кнопки на форме...
			this.RelocationButtons();
			// и добавим их на нее
			for (int i = 0; i < 4; i++)
			{
				this.Controls.Add(buttons[i]);
				buttons[i].Action += new cnTabButton.ActionEventHandler(buttonAction);
			}
		}
			
		#endregion
	
		#region protected methods
		
			#region overrided methods
			
		protected override void OnPaint(PaintEventArgs pe)
		{
			Graphics g = pe.Graphics;

			g.DrawRectangle(borderPen, this.GetBoundsHeader(0));
			
			g.SmoothingMode = SmoothingMode.AntiAlias;
			
			if (tabPages.Count > 0)
			{
				if( currentPage < 0 )
					tabPages[currentPage= 0].Visible = true;
				
				for (int i = tabPages.Count - 1; i >= 0; i--)
				{
					if (currentPage != i)
						DrawTab(g, i);
				}
				if ((currentPage >= 0) && (currentPage < tabPages.Count))
				{
					DrawTab(g, currentPage);
				}
				DrawButtons(g);
			}
			g.SmoothingMode = SmoothingMode.None;
			g.DrawRectangle(borderPen, new Rectangle(0, 0, Width - 1, Height - 1));
		}
		
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (this.IsHandleCreated)
				UpdateColors();
			Invalidate();
		}
		
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			e.Graphics.FillRectangle(backBrush, e.ClipRectangle);
		}
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (GetBoundsButtons().Contains(e.X, e.Y))
			{
				bool needRedraw = hoverPage != -1;
				hoverPage = -1;
				if (needRedraw)
					Invalidate();
			}
			else
			{
				int newHoverPage = GetHitTest(e.X, e.Y);

				bool needRedraw = false;
				
				if( HoverEnable )
				{
					if (newHoverPage != hoverPage)
					{
						if (newHoverPage == currentPage)
							hoverPage = -1;
						else
							hoverPage = newHoverPage;
						needRedraw = true;
					}
				}
				if (needRedraw) Invalidate();

				if( toolTip.Active )
				{
					if( toolTipPage != newHoverPage )
					{
						toolTipPage = newHoverPage;

						toolTip.RemoveAll();
						if( newHoverPage != -1 )
							toolTip.SetToolTip(this, TabPages[newHoverPage].ToolTip);
					}
				}
			}
		}
		
		protected override void OnMouseLeave(EventArgs e)
		{
			if( HoverEnable )
			{
				bool needRedraw = hoverPage != -1;

				hoverPage = -1;
				
				if( needRedraw )
					Invalidate();
			}

			if( toolTip.Active )
			{
				toolTipPage = -1;
				toolTip.RemoveAll();
			}
		}
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (!GetBoundsButtons().Contains(e.X, e.Y))
			{
				int newPage = GetHitTest(e.X, e.Y);

				if (newPage >= 0 && newPage != currentPage)
				{
					SetCurentPage(newPage);
					hoverPage = -1;
					Invalidate();
				}
			}
		}
		
		protected override void OnFontChanged(EventArgs e)
		{
			if (TabPages.Count > 0)
			{
				foreach (cnTabPage tp in TabPages)
					tp.Font = (System.Drawing.Font)Font.Clone();
				Invalidate();
			}
		}
		
		protected override void OnForeColorChanged(EventArgs e)
		{
			if (TabPages.Count > 0)
			{
				foreach (cnTabPage tp in TabPages)
					tp.ForeColor = this.ForeColor;
				Invalidate();
			}
		}

			#endregion
			
			#region new
		
		protected void TabPagesClearComplete()
		{
			Invalidate();
		}
			
		protected void TabPagesRemoveComplete(int index, cnTabPage value)
		{
			value.Hide();
			if( TabPages.Count > 0 )
			{
				if( currentPage < TabPages.Count )
					SetCurentPage(currentPage);
				else
					SetCurentPage(currentPage - 1);
			}
			else
			{
				currentPage = -1;
			}

			value.TabColorChanged -= new cnTabPage.TabColorChangedEventHandler(TabPagesClearComplete);
			value.TitleChanged -= new cnTabPage.TitleChangedEventHandler(PageTitleChanged);
			Invalidate();
		}
		
		protected void TabPagesInsertComplete(int index, cnTabPage value)
		{
			value.TabColorChanged += new cnTabPage.TabColorChangedEventHandler(TabPagesClearComplete);
			value.TitleChanged += new cnTabPage.TitleChangedEventHandler(PageTitleChanged);
			value.Font = (System.Drawing.Font)this.Font.Clone();
			value.ForeColor = this.ForeColor;
			Invalidate();
		}
		
		protected void TabPagesSetComplete(int index, cnTabPage oldValue, cnTabPage newValue)
		{
			oldValue.TabColorChanged -= new cnTabPage.TabColorChangedEventHandler(TabPagesClearComplete);
			newValue.TabColorChanged += new cnTabPage.TabColorChangedEventHandler(TabPagesClearComplete);
			
			oldValue.TitleChanged -= new cnTabPage.TitleChangedEventHandler(PageTitleChanged);
			newValue.TitleChanged += new cnTabPage.TitleChangedEventHandler(PageTitleChanged);
			Invalidate();
		}
		
		protected void PageTitleChanged(cnTabPage page)
		{
			Invalidate();
		}
			
			#endregion
		
		#endregion
	
		#region events and delefates
		
		public delegate void SelectEventHandler(cnTabControl sender, cnTabPage page);
		public delegate bool CanSelectEventHandler(cnTabControl sender, cnTabPage page);
		
		
		[Category("Page action")]
		public event SelectEventHandler SelectPage;
		
		[Category("Page action")]
		public event CanSelectEventHandler CanSelectPage;
		
		#endregion
		
		#region public methods
		
		public void OnSelect(cnTabPage page)
		{
			if (SelectPage != null)
				SelectPage(this, page);
		}
		
		public bool OnCanSelect(cnTabPage page)
		{
			if (CanSelectPage != null)
				return CanSelectPage(this, page);
			else
				return true;
		}
		
		public int GetHitTest(int X, int Y)
		{
			if( GetTitlePath(currentPage).IsVisible(X, Y) )
				return currentPage;

			for (int i = 0; i < TabPages.Count; i++)
			{
				if (currentPage != i)
				{
					if(GetTitlePath(i).IsVisible(X, Y))
						return i;
				}
			}

			return -1;
		}
		
		#endregion
	}
}
