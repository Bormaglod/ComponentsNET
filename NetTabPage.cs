//-----------------------------------------------------------------------
// <copyright file="NetTabPage.cs" company="Sergey Teplyashin">
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
// <summary>Defines the NetTabPage class.</summary>
//-----------------------------------------------------------------------

namespace ComponentsNET
{
    #region Using directives
    
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using ComponentsNET.Design;
    
    #endregion
    
    [ToolboxItem(false)]
    [Designer(typeof(NetTabPageDesigner))]
    public class NetTabPage : Panel
    {
        private string title;
        
        public NetTabPage()
        {
            base.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            
            this.TabColor = new GradientColor(Color.White, Color.Orange, GradientFill.Top);
            this.PageColor = new GradientColor(Color.White, Color.WhiteSmoke, GradientFill.Bottom);
            this.TabActiveColor = new GradientColor(Color.White, Color.Orange, GradientFill.Top);
        }
        
        [Category("Свойство изменено")]
        public event EventHandler<EventArgs> TitleChanged;
        
        [Category("Внешний вид")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GradientColor TabColor { get; set; }
        
        [Category("Внешний вид")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GradientColor PageColor { get; set; }
        
        [Category("Внешний вид")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GradientColor TabActiveColor { get; set; }
        
        [Localizable(true)]
        [Category("Внешний вид")]
        public string Title
        {
            get
            {
                return this.title;
            }
            
            set 
            { 
                if (this.title != value)
                {
                    this.title = value;
                    this.OnTitleChanged();
                }
            }
        }
        
        [DefaultValue(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
            get    { return base.Font; }
            set    { base.Font = value; }
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        new public Color ForeColor
        {
            get    { return base.ForeColor; }
            set    { base.ForeColor = value; }
        }
        
        protected void OnTitleChanged()
        {
            if (this.TitleChanged != null)
            {
                this.TitleChanged(this, new EventArgs());
            }
        }
    }
}
