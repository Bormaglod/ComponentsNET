//-----------------------------------------------------------------------
// <copyright file="NetTabControl.cs" company="Sergey Teplyashin">
//     Copyright (c) 2010-2012 Sergey Teplyashin. All rights reserved.
// </copyright>
// <author>Тепляшин Сергей Васильевич</author>
// <email>sergey-teplyashin@yandex.ru</email>
// <license>
//     This program is free software; you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation; either version 3 of the License, or
//     (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </license>
// <date>28.02.2011</date>
// <time>9:50</time>
// <summary>Defines the NetTabControl class.</summary>
//-----------------------------------------------------------------------

namespace ComponentsNET
{
    #region Using directives
    
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;
    
    #endregion
    
    public class NetTabControl : ContainerControl, ISupportInitialize
    {
        /// <summary>
        /// Список страниц.
        /// </summary>
        private NetTabPageCollection tabPages;
        
        /// <summary>
        /// Расположение закладок. Возможные значения:
        /// <para>Left - слева</para>
        /// <para>Right - справа</para>
        /// <para>Top - сверху</para>
        /// <para>Bottom - снизу</para>
        /// </summary>
        private TabAlignment tabAlignment;
        
        /// <summary>
        /// Высота закладки в пикселах.
        /// </summary>
        private int tabHeight;
        
        /// <summary>
        /// Карандаш для borderColor.
        /// </summary>
        private Pen borderPen;
        
        /// <summary>
        /// Цвет границы страницы и панели закладок.
        /// </summary>
        private Color borderColor;
        
        /// <summary>
        /// Текущая страница
        /// </summary>
        private int currentPage;
        
        public NetTabControl()
        {
            BackColor = Color.FromArgb(255, 251, 250, 251);
            ForeColor = SystemColors.ControlText;
            Size = new Size(200, 100);
            
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            
            this.tabPages = new NetTabPageCollection(this);
            this.tabPages.ClearComplete += new EventHandler<EventArgs>(this.TabPagesClearComplete);
            this.tabPages.InsertComplete += new EventHandler<NetTabPageEventArgs>(this.TabPagesInsertComplete);
            this.tabPages.RemoveComplete += new EventHandler<NetTabPageEventArgs>(this.TabPagesRemoveComplete);
            
            this.tabHeight = 25;
            this.tabAlignment = TabAlignment.Top;
            this.currentPage = -1;
            
            this.CreateColors();
        }
        
        /// <summary>
        /// Свойство возвращает или устанавливает цвет закладки.
        /// </summary>
        [Category("Внешний вид")]
        [NotifyParentProperty(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GradientColor TabColor { get; set; }
        
        /// <summary>
        /// Свойство возвращает или устанавливает цвет закладки при наведении на нее указателя мыши.
        /// </summary>
        [Category("Внешний вид")]
        [NotifyParentProperty(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GradientColor TabHover { get; set; }
        
        [Category("Внешний вид")]
        [DefaultValue(typeof(Color), "135; 155; 179")]
        public Color BorderColor
        {
            get
            {
                return this.borderColor;
            }
            
            set
            {
                if (this.borderColor != value)
                {
                    if (IsHandleCreated)
                    {
                        this.borderPen.Dispose();
                    }
                    
                    this.borderColor = value;
                    if (IsHandleCreated)
                    {
                        this.borderPen = new Pen(this.borderColor);
                    }
                    
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Свойство возвращает список страниц <see cref="NetTabPage"></see>.
        /// </summary>
        [Category("Поведение")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public NetTabPageCollection TabPages
        {
            get
            {
                return this.tabPages;
            }
        }
        
        /// <summary>
        /// Свойство возвращает или устанавливает расположение закладок. Возможные значения:
        /// <para>Left - слева;</para>
        /// <para>Right - справа;</para>
        /// <para>Top - сверху;</para>
        /// <para>Bottom - снизу.</para>
        /// </summary>
        [Category("Поведение")]
        [DefaultValue(TabAlignment.Top)]
        public TabAlignment Alignment
        {
            get
            {
                return this.tabAlignment;
            }
            
            set
            {
                if (this.tabAlignment != value)
                {
                    this.tabAlignment = value;
                }
            }
        }
        
        [Category("Поведение")]
        [DefaultValue(25)]
        public int TabHeight
        {
            get
            {
                return this.tabHeight;
            }
            
            set
            {
                if (value > 16 && this.tabHeight != value)
                {
                    this.tabHeight = value;
                }
            }
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CurrentPage
        {
            get
            {
                return this.currentPage;
            }
            
            set
            {
                this.SetCurentPage(value);
            }
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public Control.ControlCollection Controls
        {
            get { return base.Controls; }
        }
        
        private Rectangle DefaultBoundsHeader
        {
            get { return this.GetBoundsHeader(0); }
        }
        
        #region ISupportInitialize implemented
        
        public void BeginInit()
        {
        }
        
        public void EndInit()
        {
        }
        
        #endregion
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.borderPen.Dispose();
            }
            
            base.Dispose(disposing);
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawRectangle(borderPen, this.DefaultBoundsHeader);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            g.SmoothingMode = SmoothingMode.None;
            g.DrawRectangle(borderPen, new Rectangle(0, 0, Width - 1, Height - 1));
        }
        
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            NetTabPage page = e.Control as NetTabPage;
            if (page != null && string.IsNullOrEmpty(page.Title))
            {
                page.Title = page.Name;
            }
        }
        
        private void TabPagesClearComplete(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.currentPage = -1;
        }
        
        private void TabPagesRemoveComplete(object sender, NetTabPageEventArgs e)
        {
            this.Controls.Remove(e.Page);
            if (this.CurrentPage >= this.TabPages.Count)
            {
                this.CurrentPage = this.TabPages.Count - 1;
            }
        }
        
        private void TabPagesInsertComplete(object sender, NetTabPageEventArgs e)
        {
            this.Controls.Add(e.Page);
            if (this.CurrentPage == -1)
            {
                this.CurrentPage = 0;
            }
        }
        
        private void CreateColors()
        {
            this.TabColor = new GradientColor(Color.White, Color.Orange, GradientFill.Top);
            this.TabHover = new GradientColor(Color.Orange, Color.White, GradientFill.Top);
            this.borderColor = Color.FromArgb(255, 135, 155, 179);
            this.borderPen = new Pen(this.borderColor);
        }
        
        private Rectangle GetBoundsHeader(int offset)
        {
            int x = 0;
            int y = 0;
            int w = Width - 1;
            int h = Height - 1;
            switch (this.Alignment)
            {
                case TabAlignment.Top:
                    y = offset;
                    h = this.TabHeight - offset - 1;
                    break;
                case TabAlignment.Bottom:
                    y = Height - this.TabHeight;
                    h = this.TabHeight - offset - 1;
                    break;
                case TabAlignment.Left:
                    x = offset;
                    w = this.TabHeight - offset - 1;
                    break;
                case TabAlignment.Right:
                    x = Width - this.TabHeight;
                    w = this.TabHeight - offset - 1;
                    break;
            }
            
            return new Rectangle(x, y, w, h);
        }
        
        /// <summary>
        /// Устанавливает текущей новую страницу
        /// </summary>
        /// <param name="newPage">Индекс новой страницы</param>
        private void SetCurentPage(int newPage)
        {
            if (currentPage != -1)
            {
                TabPages[currentPage].Visible = false;
            }
            
            currentPage = newPage;
            TabPages[currentPage].Visible = true;
        }
    }
}
